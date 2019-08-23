using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Properties;
using System;
using System.IO;

namespace MySql.Data.Common
{
	internal class StreamCreator
	{
		private string hostList;

		private uint port;

		private string pipeName;

		private uint timeOut;

		private uint keepalive;

		private DBVersion driverVersion;

		public StreamCreator(string hosts, uint port, string pipeName, uint keepalive, DBVersion driverVersion)
		{
			this.hostList = hosts;
			if (this.hostList == null || this.hostList.Length == 0)
			{
				this.hostList = "localhost";
			}
			this.port = port;
			this.pipeName = pipeName;
			this.keepalive = keepalive;
			this.driverVersion = driverVersion;
		}

		public static Stream GetStream(string server, uint port, string pipename, uint keepalive, DBVersion v, uint timeout)
		{
			return StreamCreator.GetStream(new MySqlConnectionStringBuilder
			{
				Server = server,
				Port = port,
				PipeName = pipename,
				Keepalive = keepalive,
				ConnectionTimeout = timeout
			});
		}

		public static Stream GetStream(MySqlConnectionStringBuilder settings)
		{
			switch (settings.ConnectionProtocol)
			{
			case MySqlConnectionProtocol.Sockets:
				return StreamCreator.GetTcpStream(settings);
			case MySqlConnectionProtocol.Pipe:
				return StreamCreator.GetNamedPipeStream(settings);
			case MySqlConnectionProtocol.UnixSocket:
				return StreamCreator.GetUnixSocketStream(settings);
			case MySqlConnectionProtocol.SharedMemory:
				return StreamCreator.GetSharedMemoryStream(settings);
			default:
				throw new InvalidOperationException(Resources.UnknownConnectionProtocol);
			}
		}

		private static Stream GetTcpStream(MySqlConnectionStringBuilder settings)
		{
			return MyNetworkStream.CreateStream(settings, false);
		}

		private static Stream GetUnixSocketStream(MySqlConnectionStringBuilder settings)
		{
			if (Platform.IsWindows())
			{
				throw new InvalidOperationException(Resources.NoUnixSocketsOnWindows);
			}
			return MyNetworkStream.CreateStream(settings, true);
		}

		private static Stream GetSharedMemoryStream(MySqlConnectionStringBuilder settings)
		{
			SharedMemoryStream sharedMemoryStream = new SharedMemoryStream(settings.SharedMemoryName);
			sharedMemoryStream.Open(settings.ConnectionTimeout);
			return sharedMemoryStream;
		}

		private static Stream GetNamedPipeStream(MySqlConnectionStringBuilder settings)
		{
			return NamedPipeStream.Create(settings.PipeName, settings.Server, settings.ConnectionTimeout);
		}
	}
}
