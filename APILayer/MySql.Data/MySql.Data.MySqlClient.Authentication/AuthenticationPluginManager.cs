using MySql.Data.MySqlClient.Properties;
using System;
using System.Collections.Generic;

namespace MySql.Data.MySqlClient.Authentication
{
	internal class AuthenticationPluginManager
	{
		private static Dictionary<string, PluginInfo> plugins;

		static AuthenticationPluginManager()
		{
			AuthenticationPluginManager.plugins = new Dictionary<string, PluginInfo>();
			AuthenticationPluginManager.plugins["mysql_native_password"] = new PluginInfo("MySql.Data.MySqlClient.Authentication.MySqlNativePasswordPlugin");
			AuthenticationPluginManager.plugins["sha256_password"] = new PluginInfo("MySql.Data.MySqlClient.Authentication.Sha256AuthenticationPlugin");
			AuthenticationPluginManager.plugins["authentication_windows_client"] = new PluginInfo("MySql.Data.MySqlClient.Authentication.MySqlWindowsAuthenticationPlugin");
			if (MySqlConfiguration.Settings != null && MySqlConfiguration.Settings.AuthenticationPlugins != null)
			{
				foreach (AuthenticationPluginConfigurationElement current in MySqlConfiguration.Settings.AuthenticationPlugins)
				{
					AuthenticationPluginManager.plugins[current.Name] = new PluginInfo(current.Type);
				}
			}
		}

		public static MySqlAuthenticationPlugin GetPlugin(string method)
		{
			if (!AuthenticationPluginManager.plugins.ContainsKey(method))
			{
				throw new MySqlException(string.Format(Resources.AuthenticationMethodNotSupported, method));
			}
			return AuthenticationPluginManager.CreatePlugin(method);
		}

		private static MySqlAuthenticationPlugin CreatePlugin(string method)
		{
			PluginInfo pluginInfo = AuthenticationPluginManager.plugins[method];
			MySqlAuthenticationPlugin result;
			try
			{
				Type type = Type.GetType(pluginInfo.Type);
				MySqlAuthenticationPlugin mySqlAuthenticationPlugin = (MySqlAuthenticationPlugin)Activator.CreateInstance(type);
				result = mySqlAuthenticationPlugin;
			}
			catch (Exception ex)
			{
				throw new MySqlException(string.Format(Resources.UnableToCreateAuthPlugin, method), ex);
			}
			return result;
		}
	}
}
