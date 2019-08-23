using System;
using System.Configuration;

namespace MySql.Data.MySqlClient
{
	public sealed class ReplicationServerConfigurationElement : ConfigurationElement
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

		[ConfigurationProperty("IsMaster", IsRequired = false, DefaultValue = false)]
		public bool IsMaster
		{
			get
			{
				return (bool)base["IsMaster"];
			}
			set
			{
				base["IsMaster"] = value;
			}
		}

		[ConfigurationProperty("connectionstring", IsRequired = true)]
		public string ConnectionString
		{
			get
			{
				return (string)base["connectionstring"];
			}
			set
			{
				base["connectionstring"] = value;
			}
		}
	}
}
