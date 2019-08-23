using System;
using System.Data;
using System.Data.Common;

namespace MySql.Data.MySqlClient
{
	public sealed class MySqlTransaction : DbTransaction, IDisposable
	{
		private IsolationLevel level;

		private MySqlConnection conn;

		private bool open;

		protected override DbConnection DbConnection
		{
			get
			{
				return this.conn;
			}
		}

		public new MySqlConnection Connection
		{
			get
			{
				return this.conn;
			}
		}

		public override IsolationLevel IsolationLevel
		{
			get
			{
				return this.level;
			}
		}

		internal MySqlTransaction(MySqlConnection c, IsolationLevel il)
		{
			this.conn = c;
			this.level = il;
			this.open = true;
		}

		public new void Dispose()
		{
			if (((this.conn != null && this.conn.State == ConnectionState.Open) || this.conn.SoftClosed) && this.open)
			{
				this.Rollback();
			}
		}

		public override void Commit()
		{
			if (this.conn == null || (this.conn.State != ConnectionState.Open && !this.conn.SoftClosed))
			{
				throw new InvalidOperationException("Connection must be valid and open to commit transaction");
			}
			if (!this.open)
			{
				throw new InvalidOperationException("Transaction has already been committed or is not pending");
			}
			MySqlCommand mySqlCommand = new MySqlCommand("COMMIT", this.conn);
			mySqlCommand.ExecuteNonQuery();
			this.open = false;
		}

		public override void Rollback()
		{
			if (this.conn == null || (this.conn.State != ConnectionState.Open && !this.conn.SoftClosed))
			{
				throw new InvalidOperationException("Connection must be valid and open to rollback transaction");
			}
			if (!this.open)
			{
				throw new InvalidOperationException("Transaction has already been rolled back or is not pending");
			}
			MySqlCommand mySqlCommand = new MySqlCommand("ROLLBACK", this.conn);
			mySqlCommand.ExecuteNonQuery();
			this.open = false;
		}
	}
}
