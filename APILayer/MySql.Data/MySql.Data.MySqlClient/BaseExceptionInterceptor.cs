using System;

namespace MySql.Data.MySqlClient
{
	public abstract class BaseExceptionInterceptor
	{
		protected MySqlConnection ActiveConnection
		{
			get;
			private set;
		}

		public abstract Exception InterceptException(Exception exception);

		public virtual void Init(MySqlConnection connection)
		{
			this.ActiveConnection = connection;
		}
	}
}
