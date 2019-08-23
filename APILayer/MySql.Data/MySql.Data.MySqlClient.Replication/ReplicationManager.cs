using MySql.Data.MySqlClient.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Timers;

namespace MySql.Data.MySqlClient.Replication
{
	public static class ReplicationManager
	{
		private static List<ReplicationServerGroup> groups;

		public static IList<ReplicationServerGroup> Groups
		{
			get;
			private set;
		}

		static ReplicationManager()
		{
			ReplicationManager.groups = new List<ReplicationServerGroup>();
			ReplicationManager.Groups = ReplicationManager.groups;
			if (MySqlConfiguration.Settings == null)
			{
				return;
			}
			foreach (ReplicationServerGroupConfigurationElement current in MySqlConfiguration.Settings.Replication.ServerGroups)
			{
				ReplicationServerGroup replicationServerGroup = ReplicationManager.AddGroup(current.Name, current.GroupType, current.RetryTime);
				foreach (ReplicationServerConfigurationElement current2 in current.Servers)
				{
					replicationServerGroup.AddServer(current2.Name, current2.IsMaster, current2.ConnectionString);
				}
			}
		}

		public static ReplicationServerGroup AddGroup(string name, int retryTime)
		{
			return ReplicationManager.AddGroup(name, null, retryTime);
		}

		public static ReplicationServerGroup AddGroup(string name, string groupType, int retryTime)
		{
			if (string.IsNullOrEmpty(groupType))
			{
				groupType = "MySql.Data.MySqlClient.Replication.ReplicationRoundRobinServerGroup";
			}
			Type type = Type.GetType(groupType);
			ReplicationServerGroup replicationServerGroup = (ReplicationServerGroup)Activator.CreateInstance(type, new object[]
			{
				name,
				retryTime
			});
			ReplicationManager.groups.Add(replicationServerGroup);
			return replicationServerGroup;
		}

		public static ReplicationServer GetServer(string groupName, bool isMaster)
		{
			ReplicationServerGroup group = ReplicationManager.GetGroup(groupName);
			return group.GetServer(isMaster);
		}

		public static ReplicationServerGroup GetGroup(string groupName)
		{
			ReplicationServerGroup replicationServerGroup = null;
			foreach (ReplicationServerGroup current in ReplicationManager.groups)
			{
				if (string.Compare(current.Name, groupName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					replicationServerGroup = current;
					break;
				}
			}
			if (replicationServerGroup == null)
			{
				throw new MySqlException(string.Format(Resources.ReplicationGroupNotFound, groupName));
			}
			return replicationServerGroup;
		}

		public static bool IsReplicationGroup(string groupName)
		{
			foreach (ReplicationServerGroup current in ReplicationManager.groups)
			{
				if (string.Compare(current.Name, groupName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return true;
				}
			}
			return false;
		}

		public static void GetNewConnection(string groupName, bool master, MySqlConnection connection)
		{
			while (true)
			{
				if (!ReplicationManager.IsReplicationGroup(groupName))
				{
					break;
				}
				ReplicationServerGroup group = ReplicationManager.GetGroup(groupName);
				ReplicationServer server = group.GetServer(master);
				if (server == null)
				{
					goto Block_2;
				}
				Driver driver = Driver.Create(new MySqlConnectionStringBuilder(server.ConnectionString));
				if (connection.driver == null || driver.Settings.ConnectionString != connection.driver.Settings.ConnectionString)
				{
					connection.Close();
					connection.hasBeenOpen = false;
					try
					{
						connection.driver = driver;
						connection.Open();
					}
					catch (Exception)
					{
						connection.driver = null;
						server.IsAvailable = false;
						BackgroundWorker backgroundWorker = new BackgroundWorker();
						backgroundWorker.DoWork += delegate(object sender, DoWorkEventArgs e)
						{
							bool isRunning = false;
							ReplicationServer server1 = e.Argument as ReplicationServer;
							int retryTime = ReplicationManager.GetGroup(groupName).RetryTime;
							Timer timer = new Timer((double)retryTime * 1000.0);
							ElapsedEventHandler elapsedEventHandler = delegate(object sender1, ElapsedEventArgs e1)
							{
								if (isRunning)
								{
									return;
								}
								try
								{
									isRunning = true;
									using (MySqlConnection mySqlConnection = new MySqlConnection(server.ConnectionString))
									{
										mySqlConnection.Open();
										server1.IsAvailable = true;
										timer.Stop();
									}
								}
								catch
								{
									MySqlTrace.LogWarning(0, string.Format(Resources.Replication_ConnectionAttemptFailed, server1.Name));
								}
								finally
								{
									isRunning = false;
								}
							};
							timer.Elapsed += elapsedEventHandler;
							timer.Start();
							elapsedEventHandler(sender, null);
						};
						backgroundWorker.RunWorkerAsync(server);
						continue;
					}
					return;
				}
				return;
			}
			return;
			Block_2:
			throw new MySqlException(Resources.Replication_NoAvailableServer);
		}
	}
}
