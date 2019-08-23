using MySql.Data.MySqlClient.Properties;
using System;
using System.Collections.Generic;
using System.Data;

namespace MySql.Data.MySqlClient
{
	internal sealed class CommandInterceptor : Interceptor
	{
		private bool insideInterceptor;

		private List<BaseCommandInterceptor> interceptors = new List<BaseCommandInterceptor>();

		public CommandInterceptor(MySqlConnection connection)
		{
			this.connection = connection;
			base.LoadInterceptors(connection.Settings.CommandInterceptors);
		}

		public bool ExecuteScalar(string sql, ref object returnValue)
		{
			if (this.insideInterceptor)
			{
				return false;
			}
			this.insideInterceptor = true;
			bool flag = false;
			foreach (BaseCommandInterceptor current in this.interceptors)
			{
				flag |= current.ExecuteScalar(sql, ref returnValue);
			}
			this.insideInterceptor = false;
			return flag;
		}

		public bool ExecuteNonQuery(string sql, ref int returnValue)
		{
			if (this.insideInterceptor)
			{
				return false;
			}
			this.insideInterceptor = true;
			bool flag = false;
			foreach (BaseCommandInterceptor current in this.interceptors)
			{
				flag |= current.ExecuteNonQuery(sql, ref returnValue);
			}
			this.insideInterceptor = false;
			return flag;
		}

		public bool ExecuteReader(string sql, CommandBehavior behavior, ref MySqlDataReader returnValue)
		{
			if (this.insideInterceptor)
			{
				return false;
			}
			this.insideInterceptor = true;
			bool flag = false;
			foreach (BaseCommandInterceptor current in this.interceptors)
			{
				flag |= current.ExecuteReader(sql, behavior, ref returnValue);
			}
			this.insideInterceptor = false;
			return flag;
		}

		protected override void AddInterceptor(object o)
		{
			if (o == null)
			{
				throw new ArgumentException(string.Format("Unable to instantiate CommandInterceptor", new object[0]));
			}
			if (!(o is BaseCommandInterceptor))
			{
				throw new InvalidOperationException(string.Format(Resources.TypeIsNotCommandInterceptor, o.GetType()));
			}
			BaseCommandInterceptor baseCommandInterceptor = o as BaseCommandInterceptor;
			baseCommandInterceptor.Init(this.connection);
			this.interceptors.Insert(0, (BaseCommandInterceptor)o);
		}

		protected override string ResolveType(string nameOrType)
		{
			if (MySqlConfiguration.Settings != null && MySqlConfiguration.Settings.CommandInterceptors != null)
			{
				foreach (InterceptorConfigurationElement current in MySqlConfiguration.Settings.CommandInterceptors)
				{
					if (string.Compare(current.Name, nameOrType, true) == 0)
					{
						return current.Type;
					}
				}
			}
			return base.ResolveType(nameOrType);
		}
	}
}
