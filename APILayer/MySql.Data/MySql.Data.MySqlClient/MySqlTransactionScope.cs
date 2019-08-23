using System;
using System.Data;
using System.Threading;
using System.Transactions;

namespace MySql.Data.MySqlClient
{
	internal class MySqlTransactionScope
	{
		public MySqlConnection connection;

		public Transaction baseTransaction;

		public MySqlTransaction simpleTransaction;

		public int rollbackThreadId;

		public MySqlTransactionScope(MySqlConnection con, Transaction trans, MySqlTransaction simpleTransaction)
		{
			this.connection = con;
			this.baseTransaction = trans;
			this.simpleTransaction = simpleTransaction;
		}

		public void Rollback(SinglePhaseEnlistment singlePhaseEnlistment)
		{
			Driver driver = this.connection.driver;
			lock (driver)
			{
				this.rollbackThreadId = Thread.CurrentThread.ManagedThreadId;
				while (this.connection.Reader != null)
				{
					Thread.Sleep(100);
				}
				this.simpleTransaction.Rollback();
				singlePhaseEnlistment.Aborted();
				DriverTransactionManager.RemoveDriverInTransaction(this.baseTransaction);
				driver.CurrentTransaction = null;
				if (this.connection.State == ConnectionState.Closed)
				{
					this.connection.CloseFully();
				}
				this.rollbackThreadId = 0;
			}
		}

		public void SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment)
		{
			this.simpleTransaction.Commit();
			singlePhaseEnlistment.Committed();
			DriverTransactionManager.RemoveDriverInTransaction(this.baseTransaction);
			this.connection.driver.CurrentTransaction = null;
			if (this.connection.State == ConnectionState.Closed)
			{
				this.connection.CloseFully();
			}
		}
	}
}
