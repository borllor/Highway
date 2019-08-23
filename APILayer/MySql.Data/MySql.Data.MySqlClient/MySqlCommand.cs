using MySql.Data.MySqlClient.Properties;
using MySql.Data.MySqlClient.Replication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Threading;
using System.Transactions;

namespace MySql.Data.MySqlClient
{
	[DesignerCategory("Code"), ToolboxBitmap(typeof(MySqlCommand), "MySqlClient.resources.command.bmp")]
	public sealed class MySqlCommand : DbCommand, ICloneable, IDisposable
	{
		internal delegate object AsyncDelegate(int type, CommandBehavior behavior);

		private MySqlConnection connection;

		private MySqlTransaction curTransaction;

		private string cmdText;

		private CommandType cmdType;

		private long updatedRowCount;

		private MySqlParameterCollection parameters;

		private IAsyncResult asyncResult;

		internal long lastInsertedId;

		private PreparableStatement statement;

		private int commandTimeout;

		private bool canceled;

		private bool resetSqlSelect;

		private List<MySqlCommand> batch;

		private string batchableCommandText;

		private CommandTimer commandTimer;

		private bool useDefaultTimeout;

		private bool shouldCache;

		private int cacheAge;

		private bool internallyCreated;

		private static List<string> SingleWordKeywords = new List<string>(new string[]
		{
			"COMMIT",
			"ROLLBACK",
			"USE",
			"BEGIN",
			"END"
		});

		internal MySqlCommand.AsyncDelegate caller;

		internal Exception thrownException;

		[Browsable(false)]
		public long LastInsertedId
		{
			get
			{
				return this.lastInsertedId;
			}
		}

		[Category("Data"), Description("Command text to execute"), Editor("MySql.Data.Common.Design.SqlCommandTextEditor,MySqlClient.Design", typeof(UITypeEditor))]
		public override string CommandText
		{
			get
			{
				return this.cmdText;
			}
			set
			{
				this.cmdText = (value ?? string.Empty);
				this.statement = null;
				this.batchableCommandText = null;
				if (this.cmdText != null && this.cmdText.EndsWith("DEFAULT VALUES", StringComparison.OrdinalIgnoreCase))
				{
					this.cmdText = this.cmdText.Substring(0, this.cmdText.Length - 14);
					this.cmdText += "() VALUES ()";
				}
			}
		}

		[Category("Misc"), DefaultValue(30), Description("Time to wait for command to execute")]
		public override int CommandTimeout
		{
			get
			{
				if (!this.useDefaultTimeout)
				{
					return this.commandTimeout;
				}
				return 30;
			}
			set
			{
				if (this.commandTimeout < 0)
				{
					this.Throw(new ArgumentException("Command timeout must not be negative"));
				}
				int num = Math.Min(value, 2147483);
				if (num != value)
				{
					MySqlTrace.LogWarning(this.connection.ServerThread, string.Concat(new object[]
					{
						"Command timeout value too large (",
						value,
						" seconds). Changed to max. possible value (",
						num,
						" seconds)"
					}));
				}
				this.commandTimeout = num;
				this.useDefaultTimeout = false;
			}
		}

		[Category("Data")]
		public override CommandType CommandType
		{
			get
			{
				return this.cmdType;
			}
			set
			{
				this.cmdType = value;
			}
		}

		[Browsable(false)]
		public bool IsPrepared
		{
			get
			{
				return this.statement != null && this.statement.IsPrepared;
			}
		}

		[Category("Behavior"), Description("Connection used by the command")]
		public new MySqlConnection Connection
		{
			get
			{
				return this.connection;
			}
			set
			{
				if (this.connection != value)
				{
					this.Transaction = null;
				}
				this.connection = value;
				if (this.connection != null)
				{
					if (this.useDefaultTimeout)
					{
						this.commandTimeout = (int)this.connection.Settings.DefaultCommandTimeout;
						this.useDefaultTimeout = false;
					}
					this.EnableCaching = this.connection.Settings.TableCaching;
					this.CacheAge = this.connection.Settings.DefaultTableCacheAge;
				}
			}
		}

		[Category("Data"), Description("The parameters collection"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new MySqlParameterCollection Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		[Browsable(false)]
		public new MySqlTransaction Transaction
		{
			get
			{
				return this.curTransaction;
			}
			set
			{
				this.curTransaction = value;
			}
		}

		public bool EnableCaching
		{
			get
			{
				return this.shouldCache;
			}
			set
			{
				this.shouldCache = value;
			}
		}

		public int CacheAge
		{
			get
			{
				return this.cacheAge;
			}
			set
			{
				this.cacheAge = value;
			}
		}

		internal List<MySqlCommand> Batch
		{
			get
			{
				return this.batch;
			}
		}

		internal bool Canceled
		{
			get
			{
				return this.canceled;
			}
		}

		internal string BatchableCommandText
		{
			get
			{
				return this.batchableCommandText;
			}
		}

		internal bool InternallyCreated
		{
			get
			{
				return this.internallyCreated;
			}
			set
			{
				this.internallyCreated = value;
			}
		}

		public override UpdateRowSource UpdatedRowSource
		{
			get;
			set;
		}

		[Browsable(false)]
		public override bool DesignTimeVisible
		{
			get;
			set;
		}

		protected override DbConnection DbConnection
		{
			get
			{
				return this.Connection;
			}
			set
			{
				this.Connection = (MySqlConnection)value;
			}
		}

		protected override DbParameterCollection DbParameterCollection
		{
			get
			{
				return this.Parameters;
			}
		}

		protected override DbTransaction DbTransaction
		{
			get
			{
				return this.Transaction;
			}
			set
			{
				this.Transaction = (MySqlTransaction)value;
			}
		}

		public MySqlCommand()
		{
			this.cmdType = CommandType.Text;
			this.parameters = new MySqlParameterCollection(this);
			this.cmdText = string.Empty;
			this.useDefaultTimeout = true;
			this.Constructor();
		}

		public MySqlCommand(string cmdText) : this()
		{
			this.CommandText = cmdText;
		}

		public MySqlCommand(string cmdText, MySqlConnection connection) : this(cmdText)
		{
			this.Connection = connection;
		}

		public MySqlCommand(string cmdText, MySqlConnection connection, MySqlTransaction transaction) : this(cmdText, connection)
		{
			this.curTransaction = transaction;
		}

		public override void Cancel()
		{
			this.connection.CancelQuery(this.connection.ConnectionTimeout);
			this.canceled = true;
		}

		public new MySqlParameter CreateParameter()
		{
			return (MySqlParameter)this.CreateDbParameter();
		}

		private void CheckState()
		{
			if (this.connection == null)
			{
				this.Throw(new InvalidOperationException("Connection must be valid and open."));
			}
			if (this.connection.State != ConnectionState.Open && !this.connection.SoftClosed)
			{
				this.Throw(new InvalidOperationException("Connection must be valid and open."));
			}
			if (this.connection.IsInUse && !this.internallyCreated)
			{
				this.Throw(new MySqlException("There is already an open DataReader associated with this Connection which must be closed first."));
			}
		}

		public override int ExecuteNonQuery()
		{
			int result = -1;
			if (this.connection != null && this.connection.commandInterceptor != null && this.connection.commandInterceptor.ExecuteNonQuery(this.CommandText, ref result))
			{
				return result;
			}
			int recordsAffected;
			using (MySqlDataReader mySqlDataReader = this.ExecuteReader())
			{
				mySqlDataReader.Close();
				recordsAffected = mySqlDataReader.RecordsAffected;
			}
			return recordsAffected;
		}

		internal void ClearCommandTimer()
		{
			if (this.commandTimer != null)
			{
				this.commandTimer.Dispose();
				this.commandTimer = null;
			}
		}

		internal void Close(MySqlDataReader reader)
		{
			if (this.statement != null)
			{
				this.statement.Close(reader);
			}
			this.ResetSqlSelectLimit();
			if (this.statement != null && this.connection != null && this.connection.driver != null)
			{
				this.connection.driver.CloseQuery(this.connection, this.statement.StatementId);
			}
			this.ClearCommandTimer();
		}

		private void ResetReader()
		{
			if (this.connection != null && this.connection.Reader != null)
			{
				this.connection.Reader.Close();
				this.connection.Reader = null;
			}
		}

		internal void ResetSqlSelectLimit()
		{
			if (this.resetSqlSelect)
			{
				this.resetSqlSelect = false;
				new MySqlCommand("SET SQL_SELECT_LIMIT=DEFAULT", this.connection)
				{
					internallyCreated = true
				}.ExecuteNonQuery();
			}
		}

		public new MySqlDataReader ExecuteReader()
		{
			return this.ExecuteReader(CommandBehavior.Default);
		}

		public new MySqlDataReader ExecuteReader(CommandBehavior behavior)
		{
			MySqlDataReader result = null;
			if (this.connection != null && this.connection.commandInterceptor != null && this.connection.commandInterceptor.ExecuteReader(this.CommandText, behavior, ref result))
			{
				return result;
			}
			bool flag = false;
			this.CheckState();
			Driver driver = this.connection.driver;
			this.cmdText = this.cmdText.Trim();
			if (string.IsNullOrEmpty(this.cmdText))
			{
				this.Throw(new InvalidOperationException(Resources.CommandTextNotInitialized));
			}
			string text = this.cmdText.Trim(new char[]
			{
				';'
			});
			if (this.connection.hasBeenOpen && !driver.HasStatus(ServerStatusFlags.InTransaction))
			{
				ReplicationManager.GetNewConnection(this.connection.Settings.Server, !this.IsReadOnlyCommand(text), this.connection);
			}
			MySqlDataReader result2;
			lock (driver)
			{
				if (this.connection.Reader != null)
				{
					this.Throw(new MySqlException(Resources.DataReaderOpen));
				}
				Transaction current = System.Transactions.Transaction.Current;
				if (current != null)
				{
					bool flag3 = false;
					if (driver.CurrentTransaction != null)
					{
						flag3 = driver.CurrentTransaction.InRollback;
					}
					if (!flag3)
					{
						TransactionStatus transactionStatus = TransactionStatus.InDoubt;
						try
						{
							transactionStatus = current.TransactionInformation.Status;
						}
						catch (TransactionException)
						{
						}
						if (transactionStatus == TransactionStatus.Aborted)
						{
							this.Throw(new TransactionAbortedException());
						}
					}
				}
				this.commandTimer = new CommandTimer(this.connection, this.CommandTimeout);
				this.lastInsertedId = -1L;
				if (this.CommandType == CommandType.TableDirect)
				{
					text = "SELECT * FROM " + text;
				}
				else if (this.CommandType == CommandType.Text && text.IndexOf(" ") == -1 && !MySqlCommand.SingleWordKeywords.Contains(text.ToUpper()))
				{
					text = "call " + text;
				}
				if (this.connection.Settings.Replication && !this.InternallyCreated)
				{
					this.EnsureCommandIsReadOnly(text);
				}
				if (this.statement == null || !this.statement.IsPrepared)
				{
					if (this.CommandType == CommandType.StoredProcedure)
					{
						this.statement = new StoredProcedure(this, text);
					}
					else
					{
						this.statement = new PreparableStatement(this, text);
					}
				}
				this.statement.Resolve(false);
				this.HandleCommandBehaviors(behavior);
				this.updatedRowCount = -1L;
				try
				{
					MySqlDataReader mySqlDataReader = new MySqlDataReader(this, this.statement, behavior);
					this.connection.Reader = mySqlDataReader;
					this.canceled = false;
					this.statement.Execute();
					mySqlDataReader.NextResult();
					flag = true;
					result2 = mySqlDataReader;
				}
				catch (TimeoutException ex)
				{
					this.connection.HandleTimeoutOrThreadAbort(ex);
					throw;
				}
				catch (ThreadAbortException ex2)
				{
					this.connection.HandleTimeoutOrThreadAbort(ex2);
					throw;
				}
				catch (IOException ex3)
				{
					this.connection.Abort();
					throw new MySqlException(Resources.FatalErrorDuringExecute, ex3);
				}
				catch (MySqlException ex4)
				{
					if (ex4.InnerException is TimeoutException)
					{
						throw;
					}
					try
					{
						this.ResetReader();
						this.ResetSqlSelectLimit();
					}
					catch (Exception)
					{
						this.Connection.Abort();
						throw new MySqlException(ex4.Message, true, ex4);
					}
					if (ex4.IsQueryAborted)
					{
						result2 = null;
					}
					else
					{
						if (ex4.IsFatal)
						{
							this.Connection.Close();
						}
						if (ex4.Number == 0)
						{
							throw new MySqlException(Resources.FatalErrorDuringExecute, ex4);
						}
						throw;
					}
				}
				finally
				{
					if (this.connection != null)
					{
						if (this.connection.Reader == null)
						{
							this.ClearCommandTimer();
						}
						if (!flag)
						{
							this.ResetReader();
						}
					}
				}
			}
			return result2;
		}

		private void EnsureCommandIsReadOnly(string sql)
		{
			sql = StringUtility.ToLowerInvariant(sql);
			if (!sql.StartsWith("select") && !sql.StartsWith("show"))
			{
				this.Throw(new MySqlException(Resources.ReplicatedConnectionsAllowOnlyReadonlyStatements));
			}
			if (sql.EndsWith("for update") || sql.EndsWith("lock in share mode"))
			{
				this.Throw(new MySqlException(Resources.ReplicatedConnectionsAllowOnlyReadonlyStatements));
			}
		}

		private bool IsReadOnlyCommand(string sql)
		{
			sql = sql.ToLower();
			return (sql.StartsWith("select") || sql.StartsWith("show")) && !sql.EndsWith("for update") && !sql.EndsWith("lock in share mode");
		}

		public override object ExecuteScalar()
		{
			this.lastInsertedId = -1L;
			object result = null;
			if (this.connection != null && this.connection.commandInterceptor.ExecuteScalar(this.CommandText, ref result))
			{
				return result;
			}
			using (MySqlDataReader mySqlDataReader = this.ExecuteReader())
			{
				if (mySqlDataReader.Read())
				{
					result = mySqlDataReader.GetValue(0);
				}
			}
			return result;
		}

		private void HandleCommandBehaviors(CommandBehavior behavior)
		{
			if ((behavior & CommandBehavior.SchemaOnly) != CommandBehavior.Default)
			{
				new MySqlCommand("SET SQL_SELECT_LIMIT=0", this.connection).ExecuteNonQuery();
				this.resetSqlSelect = true;
				return;
			}
			if ((behavior & CommandBehavior.SingleRow) != CommandBehavior.Default)
			{
				new MySqlCommand("SET SQL_SELECT_LIMIT=1", this.connection).ExecuteNonQuery();
				this.resetSqlSelect = true;
			}
		}

		private void Prepare(int cursorPageSize)
		{
			using (new CommandTimer(this.Connection, this.CommandTimeout))
			{
				string commandText = this.CommandText;
				if (commandText != null && commandText.Trim().Length != 0)
				{
					if (this.CommandType == CommandType.StoredProcedure)
					{
						this.statement = new StoredProcedure(this, this.CommandText);
					}
					else
					{
						this.statement = new PreparableStatement(this, this.CommandText);
					}
					this.statement.Resolve(true);
					this.statement.Prepare();
				}
			}
		}

		public override void Prepare()
		{
			if (this.connection == null)
			{
				this.Throw(new InvalidOperationException("The connection property has not been set."));
			}
			if (this.connection.State != ConnectionState.Open)
			{
				this.Throw(new InvalidOperationException("The connection is not open."));
			}
			if (this.connection.Settings.IgnorePrepare)
			{
				return;
			}
			this.Prepare(0);
		}

		internal object AsyncExecuteWrapper(int type, CommandBehavior behavior)
		{
			this.thrownException = null;
			try
			{
				object result;
				if (type == 1)
				{
					result = this.ExecuteReader(behavior);
					return result;
				}
				result = this.ExecuteNonQuery();
				return result;
			}
			catch (Exception ex)
			{
				this.thrownException = ex;
			}
			return null;
		}

		public IAsyncResult BeginExecuteReader()
		{
			return this.BeginExecuteReader(CommandBehavior.Default);
		}

		public IAsyncResult BeginExecuteReader(CommandBehavior behavior)
		{
			if (this.caller != null)
			{
				this.Throw(new MySqlException(Resources.UnableToStartSecondAsyncOp));
			}
			this.caller = new MySqlCommand.AsyncDelegate(this.AsyncExecuteWrapper);
			this.asyncResult = this.caller.BeginInvoke(1, behavior, null, null);
			return this.asyncResult;
		}

		public MySqlDataReader EndExecuteReader(IAsyncResult result)
		{
			result.AsyncWaitHandle.WaitOne();
			MySqlCommand.AsyncDelegate asyncDelegate = this.caller;
			this.caller = null;
			if (this.thrownException != null)
			{
				throw this.thrownException;
			}
			return (MySqlDataReader)asyncDelegate.EndInvoke(result);
		}

		public IAsyncResult BeginExecuteNonQuery(AsyncCallback callback, object stateObject)
		{
			if (this.caller != null)
			{
				this.Throw(new MySqlException(Resources.UnableToStartSecondAsyncOp));
			}
			this.caller = new MySqlCommand.AsyncDelegate(this.AsyncExecuteWrapper);
			this.asyncResult = this.caller.BeginInvoke(2, CommandBehavior.Default, callback, stateObject);
			return this.asyncResult;
		}

		public IAsyncResult BeginExecuteNonQuery()
		{
			if (this.caller != null)
			{
				this.Throw(new MySqlException(Resources.UnableToStartSecondAsyncOp));
			}
			this.caller = new MySqlCommand.AsyncDelegate(this.AsyncExecuteWrapper);
			this.asyncResult = this.caller.BeginInvoke(2, CommandBehavior.Default, null, null);
			return this.asyncResult;
		}

		public int EndExecuteNonQuery(IAsyncResult asyncResult)
		{
			asyncResult.AsyncWaitHandle.WaitOne();
			MySqlCommand.AsyncDelegate asyncDelegate = this.caller;
			this.caller = null;
			if (this.thrownException != null)
			{
				throw this.thrownException;
			}
			return (int)asyncDelegate.EndInvoke(asyncResult);
		}

		internal long EstimatedSize()
		{
			long num = (long)this.CommandText.Length;
			foreach (MySqlParameter mySqlParameter in this.Parameters)
			{
				num += mySqlParameter.EstimatedSize();
			}
			return num;
		}

		public MySqlCommand Clone()
		{
			MySqlCommand mySqlCommand = new MySqlCommand(this.cmdText, this.connection, this.curTransaction);
			mySqlCommand.CommandType = this.CommandType;
			mySqlCommand.commandTimeout = this.commandTimeout;
			mySqlCommand.useDefaultTimeout = this.useDefaultTimeout;
			mySqlCommand.batchableCommandText = this.batchableCommandText;
			mySqlCommand.EnableCaching = this.EnableCaching;
			mySqlCommand.CacheAge = this.CacheAge;
			this.PartialClone(mySqlCommand);
			foreach (MySqlParameter mySqlParameter in this.parameters)
			{
				mySqlCommand.Parameters.Add(mySqlParameter.Clone());
			}
			return mySqlCommand;
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		internal void AddToBatch(MySqlCommand command)
		{
			if (this.batch == null)
			{
				this.batch = new List<MySqlCommand>();
			}
			this.batch.Add(command);
		}

		internal string GetCommandTextForBatching()
		{
			if (this.batchableCommandText == null)
			{
				if (string.Compare(this.CommandText.Substring(0, 6), "INSERT", StringComparison.OrdinalIgnoreCase) == 0)
				{
					MySqlCommand mySqlCommand = new MySqlCommand("SELECT @@sql_mode", this.Connection);
					string text = StringUtility.ToUpperInvariant(mySqlCommand.ExecuteScalar().ToString());
					MySqlTokenizer mySqlTokenizer = new MySqlTokenizer(this.CommandText);
					mySqlTokenizer.AnsiQuotes = (text.IndexOf("ANSI_QUOTES") != -1);
					mySqlTokenizer.BackslashEscapes = (text.IndexOf("NO_BACKSLASH_ESCAPES") == -1);
					for (string text2 = StringUtility.ToLowerInvariant(mySqlTokenizer.NextToken()); text2 != null; text2 = mySqlTokenizer.NextToken())
					{
						if (StringUtility.ToUpperInvariant(text2) == "VALUES" && !mySqlTokenizer.Quoted)
						{
							text2 = mySqlTokenizer.NextToken();
							int num = 1;
							while (text2 != null)
							{
								this.batchableCommandText += text2;
								text2 = mySqlTokenizer.NextToken();
								if (text2 == "(")
								{
									num++;
								}
								else if (text2 == ")")
								{
									num--;
								}
								if (num == 0)
								{
									break;
								}
							}
							if (text2 != null)
							{
								this.batchableCommandText += text2;
							}
							text2 = mySqlTokenizer.NextToken();
							if (text2 != null && (text2 == "," || StringUtility.ToUpperInvariant(text2) == "ON"))
							{
								this.batchableCommandText = null;
								break;
							}
						}
					}
				}
				else
				{
					this.batchableCommandText = this.CommandText;
				}
			}
			return this.batchableCommandText;
		}

		private void Throw(Exception ex)
		{
			if (this.connection != null)
			{
				this.connection.Throw(ex);
			}
			throw ex;
		}

		public new void Dispose()
		{
			if (this.statement != null && this.statement.IsPrepared)
			{
				this.statement.CloseStatement();
			}
		}

		private void Constructor()
		{
			this.UpdatedRowSource = UpdateRowSource.Both;
		}

		private void PartialClone(MySqlCommand clone)
		{
			clone.UpdatedRowSource = this.UpdatedRowSource;
		}

		protected override DbParameter CreateDbParameter()
		{
			return new MySqlParameter();
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			return this.ExecuteReader(behavior);
		}
	}
}
