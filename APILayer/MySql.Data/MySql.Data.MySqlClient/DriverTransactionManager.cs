using System;
using System.Collections;
using System.Transactions;

namespace MySql.Data.MySqlClient
{
	internal class DriverTransactionManager
	{
		private static Hashtable driversInUse = new Hashtable();

		public static Driver GetDriverInTransaction(Transaction transaction)
		{
			Driver result;
			lock (DriverTransactionManager.driversInUse.SyncRoot)
			{
				Driver driver = (Driver)DriverTransactionManager.driversInUse[transaction.GetHashCode()];
				result = driver;
			}
			return result;
		}

		public static void SetDriverInTransaction(Driver driver)
		{
			lock (DriverTransactionManager.driversInUse.SyncRoot)
			{
				DriverTransactionManager.driversInUse[driver.CurrentTransaction.BaseTransaction.GetHashCode()] = driver;
			}
		}

		public static void RemoveDriverInTransaction(Transaction transaction)
		{
			lock (DriverTransactionManager.driversInUse.SyncRoot)
			{
				DriverTransactionManager.driversInUse.Remove(transaction.GetHashCode());
			}
		}
	}
}
