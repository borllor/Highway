using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace JinRi.App.Framework
{
    public class AppTelnet
    {

        private static Socket serverSocket;
        private static byte[] data = new byte[dataSize];
        private static bool newClients = true;
        private const int dataSize = 1024;
        private static Encoding defaltEncoding = Encoding.UTF8;
        private static Dictionary<Socket, TelnetClient> clientList = new Dictionary<Socket, TelnetClient>();

        private void Start()
        {
            Console.WriteLine("Starting...");
            //new Thread(new ThreadStart(BackgroundThread)) { IsBackground = false }.Start();
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, AppSetting.TelnetPort);
            serverSocket.Bind(endPoint);
            serverSocket.Listen(0);
            serverSocket.BeginAccept(new AsyncCallback(AcceptConnection), serverSocket);
            Console.WriteLine("Server socket listening to upcoming connections.");
        }

        private void BackgroundThread()
        {
            while (true)
            {
                string Input = Console.ReadLine();

                if (Input == "clients")
                {
                    if (clientList.Count == 0) continue;
                    int clientNumber = 0;
                    foreach (KeyValuePair<Socket, TelnetClient> client in clientList)
                    {
                        TelnetClient currentClient = client.Value;
                        clientNumber++;
                        Console.WriteLine(string.Format("TelnetClient #{0} (From: {1}:{2}, ECurrentState: {3}, Connection time: {4})", clientNumber,
                            currentClient.remoteEndPoint.Address.ToString(), currentClient.remoteEndPoint.Port, currentClient.clientState, currentClient.connectedAt));
                    }
                }

                if (Input.StartsWith("kill"))
                {
                    string[] _Input = Input.Split(' ');
                    int clientID = 0;
                    try
                    {
                        if (Int32.TryParse(_Input[1], out clientID) && clientID >= clientList.Keys.Count)
                        {
                            int currentClient = 0;
                            foreach (Socket currentSocket in clientList.Keys.ToArray())
                            {
                                currentClient++;
                                if (currentClient == clientID)
                                {
                                    currentSocket.Shutdown(SocketShutdown.Both);
                                    currentSocket.Close();
                                    clientList.Remove(currentSocket);
                                    Console.WriteLine("TelnetClient has been disconnected and cleared up.");
                                }
                            }
                        }
                        else { Console.WriteLine("Could not kick client: invalid client number specified."); }
                    }
                    catch { Console.WriteLine("Could not kick client: invalid client number specified."); }
                }

                if (Input == "killall")
                {
                    int deletedClients = 0;
                    foreach (Socket currentSocket in clientList.Keys.ToArray())
                    {
                        currentSocket.Shutdown(SocketShutdown.Both);
                        currentSocket.Close();
                        clientList.Remove(currentSocket);
                        deletedClients++;
                    }

                    Console.WriteLine("{0} clients have been disconnected and cleared up.", deletedClients);
                }

                if (Input == "lock") { newClients = false; Console.WriteLine("Refusing new connections."); }
                if (Input == "unlock") { newClients = true; Console.WriteLine("Accepting new connections."); }
            }
        }

        private void AcceptConnection(IAsyncResult result)
        {
            if (!newClients) return;
            Socket oldSocket = (Socket)result.AsyncState;
            Socket newSocket = oldSocket.EndAccept(result);
            TelnetClient client = new TelnetClient((IPEndPoint)newSocket.RemoteEndPoint, DateTime.Now, EClientState.NotLogged);
            clientList.Add(newSocket, client);
            Console.WriteLine("TelnetClient connected. (From: " + string.Format("{0}:{1}", client.remoteEndPoint.Address.ToString(), client.remoteEndPoint.Port) + ")");
            string output = "-- 接口日志服务器 --\n\r\n\r";
            output += "请输入密码:\n\r";
            client.clientState = EClientState.Logging;
            byte[] message = defaltEncoding.GetBytes(output);
            newSocket.BeginSend(message, 0, message.Length, SocketFlags.None, new AsyncCallback(SendData), newSocket);
            serverSocket.BeginAccept(new AsyncCallback(AcceptConnection), serverSocket);
        }

        private void SendData(IAsyncResult result)
        {
            try
            {
                Socket clientSocket = (Socket)result.AsyncState;
                clientSocket.EndSend(result);
                clientSocket.BeginReceive(data, 0, dataSize, SocketFlags.None, new AsyncCallback(ReceiveData), clientSocket);
            }
            catch { }
        }

        private void ReceiveData(IAsyncResult result)
        {
            try
            {
                Socket clientSocket = (Socket)result.AsyncState;
                TelnetClient client;
                clientList.TryGetValue(clientSocket, out client);
                int received = clientSocket.EndReceive(result);
                if (received == 0)
                {
                    clientSocket.Close();
                    clientList.Remove(clientSocket);
                    serverSocket.BeginAccept(new AsyncCallback(AcceptConnection), serverSocket);
                    Console.WriteLine("TelnetClient disconnected. (From: " + string.Format("{0}:{1}", client.remoteEndPoint.Address.ToString(), client.remoteEndPoint.Port) + ")");
                    return;
                }

                Console.WriteLine("Received '{0}' (From: {1}:{2})", BitConverter.ToString(data, 0, received), client.remoteEndPoint.Address.ToString(), client.remoteEndPoint.Port);

                // 0x2E(46-.) & 0X0D(13-newline) => return/intro
                if (data[0] == 0x2E && data[1] == 0x0D && client.commandIssued.Length == 0)
                {
                    string currentCommand = client.commandIssued;
                    Console.WriteLine(string.Format("Received '{0}' while EClientStatus '{1}' (From: {2}:{3})", currentCommand, client.clientState.ToString(), client.remoteEndPoint.Address.ToString(), client.remoteEndPoint.Port));
                    client.commandIssued = "";
                    byte[] message = defaltEncoding.GetBytes("\u001B[1J\u001B[H" + HandleCommand(clientSocket, currentCommand));
                    clientSocket.BeginSend(message, 0, message.Length, SocketFlags.None, new AsyncCallback(SendData), clientSocket);
                }

                else if (data[0] == 0x0D && data[1] == 0x0A)
                {
                    string currentCommand = client.commandIssued;
                    Console.WriteLine(string.Format("Received '{0}' (From: {1}:{2}", currentCommand, client.remoteEndPoint.Address.ToString(), client.remoteEndPoint.Port));
                    client.commandIssued = "";
                    byte[] message = defaltEncoding.GetBytes("\u001B[1J\u001B[H" + HandleCommand(clientSocket, currentCommand));
                    clientSocket.BeginSend(message, 0, message.Length, SocketFlags.None, new AsyncCallback(SendData), clientSocket);
                }

                else
                {
                    // 0x08 => remove character
                    if (data[0] == 0x08)
                    {
                        if (client.commandIssued.Length > 0)
                        {
                            client.commandIssued = client.commandIssued.Substring(0, client.commandIssued.Length - 1);
                            byte[] message = defaltEncoding.GetBytes("\u0020\u0008");
                            clientSocket.BeginSend(message, 0, message.Length, SocketFlags.None, new AsyncCallback(SendData), clientSocket);
                        }
                        else
                        {
                            clientSocket.BeginReceive(data, 0, dataSize, SocketFlags.None, new AsyncCallback(ReceiveData), clientSocket);
                        }
                    }
                    // 0x7F => delete character
                    else if (data[0] == 0x7F)
                    {
                        clientSocket.BeginReceive(data, 0, dataSize, SocketFlags.None, new AsyncCallback(ReceiveData), clientSocket);
                    }
                    else
                    {
                        string currentCommand = client.commandIssued;
                        client.commandIssued += defaltEncoding.GetString(data, 0, received);
                        clientSocket.BeginReceive(data, 0, dataSize, SocketFlags.None, new AsyncCallback(ReceiveData), clientSocket);
                    }
                }
            }
            catch { }
        }

        private string HandleCommand(Socket clientSocket, string input)
        {
            string output = "-- 接口日志服务器 --\n\r\n\r";
            byte[] dataInput = Encoding.UTF8.GetBytes(input);
            TelnetClient client;
            clientList.TryGetValue(clientSocket, out client);
            /*if (client.clientState == EClientState.NotLogged)
            {
                Console.WriteLine("TelnetClient not logged in, marking login operation in progress...");
                client.clientState = EClientState.Logging;
                Output += "Please input your password:\n\r";
            }*/
            if (client.clientState == EClientState.Logging)
            {
                if (input == "123456")
                {
                    Console.WriteLine("TelnetClient has logged in (correct password), marking as logged...");
                    client.clientState = EClientState.LoggedIn;
                    output += "登录密码.\n\r";
                }
                else
                {
                    Console.WriteLine("TelnetClient login failed (incorrect password).");
                    output += "登录失败，请输入密码: ";
                }
            }
            if (client.clientState == EClientState.LoggedIn)
            {
                if (input == "sql")
                {

                }
                else
                {
                    output += "No\n\r";
                }
            }
            return output;
        }
    }
}
