using System;

namespace MySql.Data.MySqlClient.Replication
{
	public class ReplicationRoundRobinServerGroup : ReplicationServerGroup
	{
		private int nextServer;

		public ReplicationRoundRobinServerGroup(string name, int retryTime) : base(name, retryTime)
		{
			this.nextServer = -1;
		}

		public override ReplicationServer GetServer(bool isMaster)
		{
			for (int i = 0; i < base.Servers.Count; i++)
			{
				this.nextServer++;
				if (this.nextServer == base.Servers.Count)
				{
					this.nextServer = 0;
				}
				ReplicationServer replicationServer = base.Servers[this.nextServer];
				if (replicationServer.IsAvailable && (!isMaster || replicationServer.IsMaster))
				{
					return replicationServer;
				}
			}
			return null;
		}
	}
}
