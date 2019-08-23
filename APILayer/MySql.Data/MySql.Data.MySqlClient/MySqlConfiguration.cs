using System;
using System.Configuration;

namespace MySql.Data.MySqlClient
{
	public sealed class MySqlConfiguration : ConfigurationSection
	{		
        private static MySqlConfiguration settings = null;

		public static MySqlConfiguration Settings
		{
			get
			{
                return MySqlConfiguration.settings;                
			}
		}

        static MySqlConfiguration()
        {
            try
            {
                settings = ConfigurationManager.GetSection("MySQL") as MySqlConfiguration;
            }
            catch { }
        }

		[ConfigurationCollection(typeof(InterceptorConfigurationElement), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove"), ConfigurationProperty("ExceptionInterceptors", IsRequired = false)]
		public GenericConfigurationElementCollection<InterceptorConfigurationElement> ExceptionInterceptors
		{
			get
			{
				return (GenericConfigurationElementCollection<InterceptorConfigurationElement>)base["ExceptionInterceptors"];
			}
		}

		[ConfigurationCollection(typeof(InterceptorConfigurationElement), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove"), ConfigurationProperty("CommandInterceptors", IsRequired = false)]
		public GenericConfigurationElementCollection<InterceptorConfigurationElement> CommandInterceptors
		{
			get
			{
				return (GenericConfigurationElementCollection<InterceptorConfigurationElement>)base["CommandInterceptors"];
			}
		}

		[ConfigurationCollection(typeof(AuthenticationPluginConfigurationElement), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove"), ConfigurationProperty("AuthenticationPlugins", IsRequired = false)]
		public GenericConfigurationElementCollection<AuthenticationPluginConfigurationElement> AuthenticationPlugins
		{
			get
			{
				return (GenericConfigurationElementCollection<AuthenticationPluginConfigurationElement>)base["AuthenticationPlugins"];
			}
		}

		[ConfigurationProperty("Replication", IsRequired = true)]
		public ReplicationConfigurationElement Replication
		{
			get
			{
				return (ReplicationConfigurationElement)base["Replication"];
			}
			set
			{
				base["Replication"] = value;
			}
		}
	}
}
