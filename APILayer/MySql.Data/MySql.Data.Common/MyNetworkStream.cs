using MySql.Data.MySqlClient;
using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace MySql.Data.Common
{
    internal class MyNetworkStream : NetworkStream
    {
        // Fields
        private const int MaxRetryCount = 2;
        private Socket socket;

        // Methods
        public MyNetworkStream(Socket socket, bool ownsSocket)
            : base(socket, ownsSocket)
        {
            this.socket = socket;
        }

        private static MyNetworkStream CreateSocketStream(MySqlConnectionStringBuilder settings, IPAddress ip, bool unix)
        {
            EndPoint point;
            if (!Platform.IsWindows() && unix)
            {
                point = CreateUnixEndPoint(settings.Server);
            }
            else
            {
                point = new IPEndPoint(ip, (int)settings.Port);
            }
            Socket s = unix ? new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP) : new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            if (settings.Keepalive > 0)
            {
                SetKeepAlive(s, settings.Keepalive);
            }
            IAsyncResult asyncResult = s.BeginConnect(point, null, null);
            if (!asyncResult.AsyncWaitHandle.WaitOne((int)(settings.ConnectionTimeout * 0x3e8), false))
            {
                s.Close();
                return null;
            }
            try
            {
                s.EndConnect(asyncResult);
            }
            catch (Exception)
            {
                s.Close();
                throw;
            }
            MyNetworkStream stream = new MyNetworkStream(s, true);
            GC.SuppressFinalize(s);
            GC.SuppressFinalize(stream);
            return stream;
        }

        public static MyNetworkStream CreateStream(MySqlConnectionStringBuilder settings, bool unix)
        {
            MyNetworkStream stream = null;
            foreach (IPAddress address in GetHostEntry(settings.Server).AddressList)
            {
                try
                {
                    stream = CreateSocketStream(settings, address, unix);
                    if (stream != null)
                    {
                        return stream;
                    }
                }
                catch (Exception exception)
                {
                    SocketException exception2 = exception as SocketException;
                    if (exception2 == null)
                    {
                        throw;
                    }
                    if (exception2.SocketErrorCode != SocketError.ConnectionRefused)
                    {
                        throw;
                    }
                }
            }
            return stream;
        }

        private static EndPoint CreateUnixEndPoint(string host)
        {
            return (EndPoint)Assembly.Load("Mono.Posix, Version=2.0.0.0, \t\t\t\t\r\n                Culture=neutral, PublicKeyToken=0738eb9f132ed756").CreateInstance("Mono.Posix.UnixEndPoint", false, BindingFlags.CreateInstance, null, new object[] { host }, null, null);
        }

        public override void Flush()
        {
            int num = 0;
            Exception exception = null;
            do
            {
                try
                {
                    base.Flush();
                    return;
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    this.HandleOrRethrowException(exception2);
                }
            }
            while (++num < 2);
            throw exception;
        }

        private static IPHostEntry GetHostEntry(string hostname)
        {
            IPHostEntry entry = ParseIPAddress(hostname);
            if (entry != null)
            {
                return entry;
            }
            return Dns.GetHostEntry(hostname);
        }

        private void HandleOrRethrowException(Exception e)
        {
            for (Exception exception = e; exception != null; exception = exception.InnerException)
            {
                if (exception is SocketException)
                {
                    SocketException exception2 = (SocketException)exception;
                    if (this.IsWouldBlockException(exception2))
                    {
                        this.socket.Blocking = true;
                        return;
                    }
                    if (this.IsTimeoutException(exception2))
                    {
                        return;
                    }
                }
            }
            throw e;
        }

        private bool IsTimeoutException(SocketException e)
        {
            return (e.SocketErrorCode == SocketError.TimedOut);
        }

        private bool IsWouldBlockException(SocketException e)
        {
            return (e.SocketErrorCode == SocketError.WouldBlock);
        }

        private static IPHostEntry ParseIPAddress(string hostname)
        {
            IPHostEntry entry = null;
            IPAddress address;
            if (IPAddress.TryParse(hostname, out address))
            {
                entry = new IPHostEntry
                {
                    AddressList = new IPAddress[] { address }
                };
            }
            return entry;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int num = 0;
            Exception innerException = null;
            do
            {
                try
                {
                    return base.Read(buffer, offset, count);
                }
                catch (Exception exception2)
                {
                    innerException = exception2;
                    this.HandleOrRethrowException(exception2);
                }
            }
            while (++num < 2);
            if ((innerException.GetBaseException() is SocketException) && this.IsTimeoutException((SocketException)innerException.GetBaseException()))
            {
                throw new TimeoutException(innerException.Message, innerException);
            }
            throw innerException;
        }

        public override int ReadByte()
        {
            int num = 0;
            Exception exception = null;
            do
            {
                try
                {
                    return base.ReadByte();
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    this.HandleOrRethrowException(exception2);
                }
            }
            while (++num < 2);
            throw exception;
        }

        private static void SetKeepAlive(Socket s, uint time)
        {
            uint maxValue;
            uint num = 1;
            uint num2 = 0x3e8;
            if (time > 0x418937)
            {
                maxValue = uint.MaxValue;
            }
            else
            {
                maxValue = time * 0x3e8;
            }
            byte[] array = new byte[12];
            BitConverter.GetBytes(num).CopyTo(array, 0);
            BitConverter.GetBytes(maxValue).CopyTo(array, 4);
            BitConverter.GetBytes(num2).CopyTo(array, 8);
            try
            {
                s.IOControl(IOControlCode.KeepAliveValues, array, null);
            }
            catch (NotImplementedException)
            {
            }
            return;
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, 1);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            int num = 0;
            Exception exception = null;
            do
            {
                try
                {
                    base.Write(buffer, offset, count);
                    return;
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    this.HandleOrRethrowException(exception2);
                }
            }
            while (++num < 2);
            throw exception;
        }
    }
}
