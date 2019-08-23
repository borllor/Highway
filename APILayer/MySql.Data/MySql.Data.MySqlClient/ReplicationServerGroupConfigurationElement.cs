using System;
using System.Configuration;

namespace MySql.Data.MySqlClient
{
	public sealed class ReplicationServerGroupConfigurationElement : ConfigurationElement
	{
		[ConfigurationProperty("name", IsRequired = true)]
		public string Name
		{
			get
			{
				return (string)base["name"];
			}
			set
			{
				base["name"] = value;
			}
		}

		[ConfigurationProperty("groupType", IsRequired = false)]
		public string GroupType
		{
			get
			{
				return (string)base["groupType"];
			}
			set
			{
				base["groupType"] = value;
			}
		}

		[ConfigurationProperty("retryTime", IsRequired = false, DefaultValue = 60)]
		public int RetryTime
		{
			get
			{
				return (int)base["retryTime"];
			}
			set
			{
				base["retryTime"] = value;
			}
		}

		[ConfigurationCollection(typeof(ReplicationServerConfigurationElement), AddItemName = "Server"), ConfigurationProperty("Servers")]
		public GenericConfigurationElementCollection<ReplicationServerConfigurationElement> Servers
		{
			get
			{
				return (GenericConfigurationElementCollection<ReplicationServerConfigurationElement>)base["Servers"];
			}
		}
	}
}
