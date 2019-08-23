using System;

namespace MySql.Data.MySqlClient
{
	public enum MySqlConnectionProtocol
	{
		Sockets = 1,
		Socket = 1,
		Tcp = 1,
		Pipe,
		NamedPipe = 2,
		UnixSocket,
		Unix = 3,
		SharedMemory,
		Memory = 4
	}
}
