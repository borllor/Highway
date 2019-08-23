using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Transactions;

namespace MySql.Data.MySqlClient
{
	internal sealed class MySqlPromotableTransaction : IPromotableSinglePhaseNotification, ITransactionPromoter
	{
		[ThreadStatic]
		private static Stack<MySqlTransactionScope> globalScopeStack;

		private MySqlConnection connection;

		private Transaction baseTransaction;

		private Stack<MySqlTransactionScope> scopeStack;

		public Transaction BaseTransaction
		{
			get
			{
				if (this.scopeStack.Count > 0)
				{
					return this.scopeStack.Peek().baseTransaction;
				}
				return null;
			}
		}

		public bool InRollback
		{
			get
			{
				if (this.scopeStack.Count > 0)
				{
					MySqlTransactionScope mySqlTransactionScope = this.scopeStack.Peek();
					if (mySqlTransactionScope.rollbackThreadId == Thread.CurrentThread.ManagedThreadId)
					{
						return true;
					}
				}
				return false;
			}
		}

		public MySqlPromotableTransaction(MySqlConnection connection, Transaction baseTransaction)
		{
			this.connection = connection;
			this.baseTransaction = baseTransaction;
		}

		void IPromotableSinglePhaseNotification.Initialize()
		{
			string name = Enum.GetName(typeof(System.Transactions.IsolationLevel), this.baseTransaction.IsolationLevel);
			System.Data.IsolationLevel iso = (System.Data.IsolationLevel)Enum.Parse(typeof(System.Data.IsolationLevel), name);
			MySqlTransaction simpleTransaction = this.connection.BeginTransaction(iso);
			if (MySqlPromotableTransaction.globalScopeStack == null)
			{
				MySqlPromotableTransaction.globalScopeStack = new Stack<MySqlTransactionScope>();
			}
			this.scopeStack = MySqlPromotableTransaction.globalScopeStack;
			this.scopeStack.Push(new MySqlTransactionScope(this.connection, this.baseTransaction, simpleTransaction));
		}

		void IPromotableSinglePhaseNotification.Rollback(SinglePhaseEnlistment singlePhaseEnlistment)
		{
			MySqlTransactionScope mySqlTransactionScope = this.scopeStack.Peek();
			mySqlTransactionScope.Rollback(singlePhaseEnlistment);
			this.scopeStack.Pop();
		}

		void IPromotableSinglePhaseNotification.SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment)
		{
			this.scopeStack.Pop().SinglePhaseCommit(singlePhaseEnlistment);
		}

		byte[] ITransactionPromoter.Promote()
		{
			throw new NotSupportedException();
		}
	}
}
