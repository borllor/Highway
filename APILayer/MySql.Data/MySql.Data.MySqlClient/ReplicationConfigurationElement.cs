using System;
using System.Configuration;

namespace MySql.Data.MySqlClient
{
	public sealed class ReplicationConfigurationElement : ConfigurationElement
	{
		[ConfigurationCollection(typeof(ReplicationServerGroupConfigurationElement), AddItemName = "Group"), ConfigurationProperty("ServerGroups", IsRequired = true)]
		public GenericConfigurationElementCollection<ReplicationServerGroupConfigurationElement> ServerGroups
		{
			get
			{
				return (GenericConfigurationElementCollection<ReplicationServerGroupConfigurationElement>)base["ServerGroups"];
			}
		}
	}
}
