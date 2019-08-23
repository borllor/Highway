using MySql.Data.Common;
using MySql.Data.MySqlClient.Properties;
using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Text;

namespace MySql.Data.MySqlClient
{
	internal class Driver : IDisposable
	{
		protected Encoding encoding;

		protected MySqlConnectionStringBuilder connectionString;

		protected bool isOpen;

		protected DateTime creationTime;

		protected string serverCharSet;

		protected int serverCharSetIndex;

		protected Dictionary<string, string> serverProps;

		protected Dictionary<int, string> charSets;

		protected long maxPacketSize;

		internal int timeZoneOffset;

		private DateTime idleSince;

		protected MySqlPromotableTransaction currentTransaction;

		protected bool inActiveUse;

		protected MySqlPool pool;

		private bool firstResult;

		protected IDriver handler;

		internal MySqlDataReader reader;

		private bool disposeInProgress;

		public DateTime IdleSince
		{
			get
			{
				return this.idleSince;
			}
			set
			{
				this.idleSince = value;
			}
		}

		public int ThreadID
		{
			get
			{
				return this.handler.ThreadId;
			}
		}

		public DBVersion Version
		{
			get
			{
				return this.handler.Version;
			}
		}

		public MySqlConnectionStringBuilder Settings
		{
			get
			{
				return this.connectionString;
			}
			set
			{
				this.connectionString = value;
			}
		}

		public Encoding Encoding
		{
			get
			{
				return this.encoding;
			}
			set
			{
				this.encoding = value;
			}
		}

		public MySqlPromotableTransaction CurrentTransaction
		{
			get
			{
				return this.currentTransaction;
			}
			set
			{
				this.currentTransaction = value;
			}
		}

		public bool IsInActiveUse
		{
			get
			{
				return this.inActiveUse;
			}
			set
			{
				this.inActiveUse = value;
			}
		}

		public bool IsOpen
		{
			get
			{
				return this.isOpen;
			}
		}

		public MySqlPool Pool
		{
			get
			{
				return this.pool;
			}
			set
			{
				this.pool = value;
			}
		}

		public long MaxPacketSize
		{
			get
			{
				return this.maxPacketSize;
			}
		}

		internal int ConnectionCharSetIndex
		{
			get
			{
				return this.serverCharSetIndex;
			}
			set
			{
				this.serverCharSetIndex = value;
			}
		}

		internal Dictionary<int, string> CharacterSets
		{
			get
			{
				return this.charSets;
			}
		}

		public bool SupportsOutputParameters
		{
			get
			{
				return this.Version.isAtLeast(5, 5, 0);
			}
		}

		public bool SupportsBatch
		{
			get
			{
				return (this.handler.Flags & ClientFlags.MULTI_STATEMENTS) != (ClientFlags)0uL;
			}
		}

		public bool SupportsConnectAttrs
		{
			get
			{
				return (this.handler.Flags & ClientFlags.CONNECT_ATTRS) != (ClientFlags)0uL;
			}
		}

		public bool SupportsPasswordExpiration
		{
			get
			{
				return (this.handler.Flags & ClientFlags.CAN_HANDLE_EXPIRED_PASSWORD) != (ClientFlags)0uL;
			}
		}

		public bool IsPasswordExpired
		{
			get;
			internal set;
		}

		public Driver(MySqlConnectionStringBuilder settings)
		{
			this.encoding = Encoding.GetEncoding("Windows-1252");
			if (this.encoding == null)
			{
				throw new MySqlException(Resources.DefaultEncodingNotFound);
			}
			this.connectionString = settings;
			this.serverCharSet = "latin1";
			this.serverCharSetIndex = -1;
			this.maxPacketSize = 1024L;
			this.handler = new NativeDriver(this);
		}

		~Driver()
		{
			this.Dispose(false);
		}

		public string Property(string key)
		{
			return this.serverProps[key];
		}

		public bool ConnectionLifetimeExpired()
		{
			TimeSpan timeSpan = DateTime.Now.Subtract(this.creationTime);
			return this.Settings.ConnectionLifeTime != 0u && timeSpan.TotalSeconds > this.Settings.ConnectionLifeTime;
		}

		public static Driver Create(MySqlConnectionStringBuilder settings)
		{
			Driver driver = null;
			try
			{
				if (MySqlTrace.QueryAnalysisEnabled || settings.Logging || settings.UseUsageAdvisor)
				{
					driver = new TracingDriver(settings);
				}
			}
			catch (TypeInitializationException ex)
			{
				if (!(ex.InnerException is SecurityException))
				{
					throw ex;
				}
			}
			if (driver == null)
			{
				driver = new Driver(settings);
			}
			driver.Open();
			return driver;
		}

		public bool HasStatus(ServerStatusFlags flag)
		{
			return (this.handler.ServerStatus & flag) != (ServerStatusFlags)0;
		}

		public virtual void Open()
		{
			this.creationTime = DateTime.Now;
			this.handler.Open();
			this.isOpen = true;
		}

		public virtual void Close()
		{
			this.Dispose();
		}

		public virtual void Configure(MySqlConnection connection)
		{
			bool flag = false;
			if (this.serverProps == null)
			{
				flag = true;
				try
				{
					if (this.Pool != null && this.Settings.CacheServerProperties)
					{
						if (this.Pool.ServerProperties == null)
						{
							this.Pool.ServerProperties = this.LoadServerProperties(connection);
						}
						this.serverProps = this.Pool.ServerProperties;
					}
					else
					{
						this.serverProps = this.LoadServerProperties(connection);
					}
					this.LoadCharacterSets(connection);
				}
				catch (MySqlException ex)
				{
					if (ex.Number == 1820)
					{
						this.IsPasswordExpired = true;
						return;
					}
					throw;
				}
			}
			if (this.Settings.ConnectionReset || flag)
			{
				string text = this.connectionString.CharacterSet;
				if (text == null || text.Length == 0)
				{
					if (this.serverCharSetIndex >= 0)
					{
						text = this.charSets[this.serverCharSetIndex];
					}
					else
					{
						text = this.serverCharSet;
					}
				}
				if (this.serverProps.ContainsKey("max_allowed_packet"))
				{
					this.maxPacketSize = Convert.ToInt64(this.serverProps["max_allowed_packet"]);
				}
				MySqlCommand mySqlCommand = new MySqlCommand("SET character_set_results=NULL", connection);
				mySqlCommand.InternallyCreated = true;
				object obj = this.serverProps["character_set_client"];
				object obj2 = this.serverProps["character_set_connection"];
				if ((obj != null && obj.ToString() != text) || (obj2 != null && obj2.ToString() != text))
				{
					new MySqlCommand("SET NAMES " + text, connection)
					{
						InternallyCreated = true
					}.ExecuteNonQuery();
				}
				mySqlCommand.ExecuteNonQuery();
				if (text != null)
				{
					this.Encoding = CharSetMap.GetEncoding(this.Version, text);
				}
				else
				{
					this.Encoding = CharSetMap.GetEncoding(this.Version, "latin1");
				}
				this.handler.Configure();
				return;
			}
		}

		private Dictionary<string, string> LoadServerProperties(MySqlConnection connection)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			MySqlCommand mySqlCommand = new MySqlCommand("SHOW VARIABLES", connection);
			Dictionary<string, string> result;
			try
			{
				using (MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader())
				{
					while (mySqlDataReader.Read())
					{
						string @string = mySqlDataReader.GetString(0);
						string string2 = mySqlDataReader.GetString(1);
						dictionary[@string] = string2;
					}
				}
				this.timeZoneOffset = this.GetTimeZoneOffset(connection);
				result = dictionary;
			}
			catch (Exception ex)
			{
				MySqlTrace.LogError(this.ThreadID, ex.Message);
				throw;
			}
			return result;
		}

		private int GetTimeZoneOffset(MySqlConnection con)
		{
			MySqlCommand mySqlCommand = new MySqlCommand("select timediff( curtime(), utc_time() )", con);
			string text = mySqlCommand.ExecuteScalar().ToString();
			return int.Parse(text.Substring(0, text.IndexOf(':')));
		}

		private void LoadCharacterSets(MySqlConnection connection)
		{
			MySqlCommand mySqlCommand = new MySqlCommand("SHOW COLLATION", connection);
			try
			{
				using (MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader())
				{
					this.charSets = new Dictionary<int, string>();
					while (mySqlDataReader.Read())
					{
						this.charSets[Convert.ToInt32(mySqlDataReader["id"], NumberFormatInfo.InvariantInfo)] = mySqlDataReader.GetString(mySqlDataReader.GetOrdinal("charset"));
					}
				}
			}
			catch (Exception ex)
			{
				MySqlTrace.LogError(this.ThreadID, ex.Message);
				throw;
			}
		}

		public virtual List<MySqlError> ReportWarnings(MySqlConnection connection)
		{
			List<MySqlError> list = new List<MySqlError>();
			using (MySqlDataReader mySqlDataReader = new MySqlCommand("SHOW WARNINGS", connection)
			{
				InternallyCreated = true
			}.ExecuteReader())
			{
				while (mySqlDataReader.Read())
				{
					list.Add(new MySqlError(mySqlDataReader.GetString(0), mySqlDataReader.GetInt32(1), mySqlDataReader.GetString(2)));
				}
			}
			MySqlInfoMessageEventArgs mySqlInfoMessageEventArgs = new MySqlInfoMessageEventArgs();
			mySqlInfoMessageEventArgs.errors = list.ToArray();
			if (connection != null)
			{
				connection.OnInfoMessage(mySqlInfoMessageEventArgs);
			}
			return list;
		}

		public virtual void SendQuery(MySqlPacket p)
		{
			this.handler.SendQuery(p);
			this.firstResult = true;
		}

		public virtual ResultSet NextResult(int statementId, bool force)
		{
			if (!force && !this.firstResult && !this.HasStatus(ServerStatusFlags.MoreResults | ServerStatusFlags.AnotherQuery))
			{
				return null;
			}
			this.firstResult = false;
			int affectedRows = -1;
			long insertedId = -1L;
			int result = this.GetResult(statementId, ref affectedRows, ref insertedId);
			if (result == -1)
			{
				return null;
			}
			if (result > 0)
			{
				return new ResultSet(this, statementId, result);
			}
			return new ResultSet(affectedRows, insertedId);
		}

		protected virtual int GetResult(int statementId, ref int affectedRows, ref long insertedId)
		{
			return this.handler.GetResult(ref affectedRows, ref insertedId);
		}

		public virtual bool FetchDataRow(int statementId, int columns)
		{
			return this.handler.FetchDataRow(statementId, columns);
		}

		public virtual bool SkipDataRow()
		{
			return this.FetchDataRow(-1, 0);
		}

		public virtual void ExecuteDirect(string sql)
		{
			MySqlPacket mySqlPacket = new MySqlPacket(this.Encoding);
			mySqlPacket.WriteString(sql);
			this.SendQuery(mySqlPacket);
			this.NextResult(0, false);
		}

		public MySqlField[] GetColumns(int count)
		{
			MySqlField[] array = new MySqlField[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = new MySqlField(this);
			}
			this.handler.GetColumnsData(array);
			return array;
		}

		public virtual int PrepareStatement(string sql, ref MySqlField[] parameters)
		{
			return this.handler.PrepareStatement(sql, ref parameters);
		}

		public IMySqlValue ReadColumnValue(int index, MySqlField field, IMySqlValue value)
		{
			return this.handler.ReadColumnValue(index, field, value);
		}

		public void SkipColumnValue(IMySqlValue valObject)
		{
			this.handler.SkipColumnValue(valObject);
		}

		public void ResetTimeout(int timeoutMilliseconds)
		{
			this.handler.ResetTimeout(timeoutMilliseconds);
		}

		public bool Ping()
		{
			return this.handler.Ping();
		}

		public virtual void SetDatabase(string dbName)
		{
			this.handler.SetDatabase(dbName);
		}

		public virtual void ExecuteStatement(MySqlPacket packetToExecute)
		{
			this.handler.ExecuteStatement(packetToExecute);
		}

		public virtual void CloseStatement(int id)
		{
			this.handler.CloseStatement(id);
		}

		public virtual void Reset()
		{
			this.handler.Reset();
		}

		public virtual void CloseQuery(MySqlConnection connection, int statementId)
		{
			if (this.handler.WarningCount > 0)
			{
				this.ReportWarnings(connection);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.disposeInProgress)
			{
				return;
			}
			this.disposeInProgress = true;
			try
			{
				this.ResetTimeout(1000);
				if (disposing)
				{
					this.handler.Close(this.isOpen);
				}
				if (this.connectionString.Pooling)
				{
					MySqlPoolManager.RemoveConnection(this);
				}
			}
			catch (Exception)
			{
				if (disposing)
				{
					throw;
				}
			}
			finally
			{
				this.reader = null;
				this.isOpen = false;
				this.disposeInProgress = false;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
