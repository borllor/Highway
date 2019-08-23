using MySql.Data.MySqlClient.Properties;
using System;
using System.Collections.Generic;

namespace MySql.Data.MySqlClient.Replication
{
	public abstract class ReplicationServerGroup
	{
		protected List<ReplicationServer> servers = new List<ReplicationServer>();

		public string Name
		{
			get;
			private set;
		}

		public int RetryTime
		{
			get;
			private set;
		}

		public IList<ReplicationServer> Servers
		{
			get;
			private set;
		}

		public ReplicationServerGroup(string name, int retryTime)
		{
			this.Servers = this.servers;
			this.Name = name;
			this.RetryTime = retryTime;
		}

		public ReplicationServer AddServer(string name, bool isMaster, string connectionString)
		{
			ReplicationServer replicationServer = new ReplicationServer(name, isMaster, connectionString);
			this.servers.Add(replicationServer);
			return replicationServer;
		}

		public void RemoveServer(string name)
		{
			ReplicationServer server = this.GetServer(name);
			if (server == null)
			{
				throw new MySqlException(string.Format(Resources.ReplicationServerNotFound, name));
			}
			this.servers.Remove(server);
		}

		public ReplicationServer GetServer(string name)
		{
			foreach (ReplicationServer current in this.servers)
			{
				if (string.Compare(name, current.Name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return current;
				}
			}
			return null;
		}

		public abstract ReplicationServer GetServer(bool isMaster);
	}
}
