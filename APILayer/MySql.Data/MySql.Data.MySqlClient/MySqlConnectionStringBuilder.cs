using MySql.Data.Common;
using MySql.Data.MySqlClient.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MySql.Data.MySqlClient
{
	public sealed class MySqlConnectionStringBuilder : DbConnectionStringBuilder
	{
		internal Dictionary<string, object> values = new Dictionary<string, object>();

		private static MySqlConnectionStringOptionCollection options;

		[Category("Connection"), Description("Server to connect to"), RefreshProperties(RefreshProperties.All)]
		public string Server
		{
			get
			{
				return this["server"] as string;
			}
			set
			{
				this["server"] = value;
			}
		}

		[Category("Connection"), Description("Database to use initially"), RefreshProperties(RefreshProperties.All)]
		public string Database
		{
			get
			{
				return this.values["database"] as string;
			}
			set
			{
				this.SetValue("database", value);
			}
		}

		[Category("Connection"), Description("Protocol to use for connection to MySQL"), DisplayName("Connection Protocol"), RefreshProperties(RefreshProperties.All)]
		public MySqlConnectionProtocol ConnectionProtocol
		{
			get
			{
				return (MySqlConnectionProtocol)this.values["protocol"];
			}
			set
			{
				this.SetValue("protocol", value);
			}
		}

		[Category("Connection"), Description("Name of pipe to use when connecting with named pipes (Win32 only)"), DisplayName("Pipe Name"), RefreshProperties(RefreshProperties.All)]
		public string PipeName
		{
			get
			{
				return (string)this.values["pipe"];
			}
			set
			{
				this.SetValue("pipe", value);
			}
		}

		[Category("Connection"), Description("Should the connection use compression"), DisplayName("Use Compression"), RefreshProperties(RefreshProperties.All)]
		public bool UseCompression
		{
			get
			{
				return (bool)this.values["compress"];
			}
			set
			{
				this.SetValue("compress", value);
			}
		}

		[Category("Connection"), Description("Allows execution of multiple SQL commands in a single statement"), DisplayName("Allow Batch"), RefreshProperties(RefreshProperties.All)]
		public bool AllowBatch
		{
			get
			{
				return (bool)this.values["allowbatch"];
			}
			set
			{
				this.SetValue("allowbatch", value);
			}
		}

		[Category("Connection"), Description("Enables output of diagnostic messages"), RefreshProperties(RefreshProperties.All)]
		public bool Logging
		{
			get
			{
				return (bool)this.values["logging"];
			}
			set
			{
				this.SetValue("logging", value);
			}
		}

		[Category("Connection"), Description("Name of the shared memory object to use"), DisplayName("Shared Memory Name"), RefreshProperties(RefreshProperties.All)]
		public string SharedMemoryName
		{
			get
			{
				return (string)this.values["sharedmemoryname"];
			}
			set
			{
				this.SetValue("sharedmemoryname", value);
			}
		}

		[Category("Connection"), Description("Allows the use of old style @ syntax for parameters"), DisplayName("Use Old Syntax"), RefreshProperties(RefreshProperties.All), Obsolete("Use Old Syntax is no longer needed.  See documentation")]
		public bool UseOldSyntax
		{
			get
			{
				return (bool)this.values["useoldsyntax"];
			}
			set
			{
				this.SetValue("useoldsyntax", value);
			}
		}

		[Category("Connection"), Description("Port to use for TCP/IP connections"), RefreshProperties(RefreshProperties.All)]
		public uint Port
		{
			get
			{
				return (uint)this.values["port"];
			}
			set
			{
				this.SetValue("port", value);
			}
		}

		[Category("Connection"), Description("The length of time (in seconds) to wait for a connection to the server before terminating the attempt and generating an error."), DisplayName("Connect Timeout"), RefreshProperties(RefreshProperties.All)]
		public uint ConnectionTimeout
		{
			get
			{
				return (uint)this.values["connectiontimeout"];
			}
			set
			{
				uint num = Math.Min(value, 2147483u);
				if (num != value)
				{
					MySqlTrace.LogWarning(-1, string.Concat(new object[]
					{
						"Connection timeout value too large (",
						value,
						" seconds). Changed to max. possible value",
						num,
						" seconds)"
					}));
				}
				this.SetValue("connectiontimeout", num);
			}
		}

		[Category("Connection"), Description("The default timeout that MySqlCommand objects will use\r\n                     unless changed."), DisplayName("Default Command Timeout"), RefreshProperties(RefreshProperties.All)]
		public uint DefaultCommandTimeout
		{
			get
			{
				return (uint)this.values["defaultcommandtimeout"];
			}
			set
			{
				this.SetValue("defaultcommandtimeout", value);
			}
		}

		[Category("Security"), Description("Indicates the user ID to be used when connecting to the data source."), DisplayName("User Id"), RefreshProperties(RefreshProperties.All)]
		public string UserID
		{
			get
			{
				return (string)this.values["user id"];
			}
			set
			{
				this.SetValue("user id", value);
			}
		}

		[Category("Security"), Description("Indicates the password to be used when connecting to the data source."), PasswordPropertyText(true), RefreshProperties(RefreshProperties.All)]
		public string Password
		{
			get
			{
				return (string)this.values["password"];
			}
			set
			{
				this.SetValue("password", value);
			}
		}

		[Category("Security"), Description("When false, security-sensitive information, such as the password, is not returned as part of the connection if the connection is open or has ever been in an open state."), DisplayName("Persist Security Info"), RefreshProperties(RefreshProperties.All)]
		public bool PersistSecurityInfo
		{
			get
			{
				return (bool)this.values["persistsecurityinfo"];
			}
			set
			{
				this.SetValue("persistsecurityinfo", value);
			}
		}

		[Category("Authentication"), Description("Should the connection use SSL."), Obsolete("Use Ssl Mode instead.")]
		internal bool Encrypt
		{
			get
			{
				return this.SslMode != MySqlSslMode.None;
			}
			set
			{
				this.SetValue("Ssl Mode", value ? MySqlSslMode.Preferred : MySqlSslMode.None);
			}
		}

		[Category("Authentication"), Description("Certificate file in PKCS#12 format (.pfx)"), DisplayName("Certificate File")]
		public string CertificateFile
		{
			get
			{
				return (string)this.values["certificatefile"];
			}
			set
			{
				this.SetValue("certificatefile", value);
			}
		}

		[Category("Authentication"), Description("Password for certificate file"), DisplayName("Certificate Password")]
		public string CertificatePassword
		{
			get
			{
				return (string)this.values["certificatepassword"];
			}
			set
			{
				this.SetValue("certificatepassword", value);
			}
		}

		[Category("Authentication"), DefaultValue(MySqlCertificateStoreLocation.None), Description("Certificate Store Location for client certificates"), DisplayName("Certificate Store Location")]
		public MySqlCertificateStoreLocation CertificateStoreLocation
		{
			get
			{
				return (MySqlCertificateStoreLocation)this.values["certificatestorelocation"];
			}
			set
			{
				this.SetValue("certificatestorelocation", value);
			}
		}

		[Category("Authentication"), Description("Certificate thumbprint. Can be used together with Certificate Store Location parameter to uniquely identify certificate to be used for SSL authentication."), DisplayName("Certificate Thumbprint")]
		public string CertificateThumbprint
		{
			get
			{
				return (string)this.values["certificatethumbprint"];
			}
			set
			{
				this.SetValue("certificatethumbprint", value);
			}
		}

		[Category("Authentication"), DefaultValue(false), Description("Use windows authentication when connecting to server"), DisplayName("Integrated Security")]
		public bool IntegratedSecurity
		{
			get
			{
				return (bool)this.values["integratedsecurity"];
			}
			set
			{
				if (!Platform.IsWindows())
				{
					throw new MySqlException("IntegratedSecurity is supported on Windows only");
				}
				this.SetValue("integratedsecurity", value);
			}
		}

		[Category("Advanced"), DefaultValue(false), Description("Should zero datetimes be supported"), DisplayName("Allow Zero Datetime"), RefreshProperties(RefreshProperties.All)]
		public bool AllowZeroDateTime
		{
			get
			{
				return (bool)this.values["allowzerodatetime"];
			}
			set
			{
				this.SetValue("allowzerodatetime", value);
			}
		}

		[Category("Advanced"), DefaultValue(false), Description("Should illegal datetime values be converted to DateTime.MinValue"), DisplayName("Convert Zero Datetime"), RefreshProperties(RefreshProperties.All)]
		public bool ConvertZeroDateTime
		{
			get
			{
				return (bool)this.values["convertzerodatetime"];
			}
			set
			{
				this.SetValue("convertzerodatetime", value);
			}
		}

		[Category("Advanced"), DefaultValue(false), Description("Logs inefficient database operations"), DisplayName("Use Usage Advisor"), RefreshProperties(RefreshProperties.All)]
		public bool UseUsageAdvisor
		{
			get
			{
				return (bool)this.values["useusageadvisor"];
			}
			set
			{
				this.SetValue("useusageadvisor", value);
			}
		}

		[Category("Advanced"), DefaultValue(25), Description("Indicates how many stored procedures can be cached at one time. A value of 0 effectively disables the procedure cache."), DisplayName("Procedure Cache Size"), RefreshProperties(RefreshProperties.All)]
		public uint ProcedureCacheSize
		{
			get
			{
				return (uint)this.values["procedurecachesize"];
			}
			set
			{
				this.SetValue("procedurecachesize", value);
			}
		}

		[Category("Advanced"), DefaultValue(false), Description("Indicates that performance counters should be updated during execution."), DisplayName("Use Performance Monitor"), RefreshProperties(RefreshProperties.All)]
		public bool UsePerformanceMonitor
		{
			get
			{
				return (bool)this.values["useperformancemonitor"];
			}
			set
			{
				this.SetValue("useperformancemonitor", value);
			}
		}

		[Category("Advanced"), DefaultValue(true), Description("Instructs the provider to ignore any attempts to prepare a command."), DisplayName("Ignore Prepare"), RefreshProperties(RefreshProperties.All)]
		public bool IgnorePrepare
		{
			get
			{
				return (bool)this.values["ignoreprepare"];
			}
			set
			{
				this.SetValue("ignoreprepare", value);
			}
		}

		[Category("Advanced"), DefaultValue(true), Description("Indicates if stored procedure bodies will be available for parameter detection."), DisplayName("Use Procedure Bodies"), Obsolete("Use CheckParameters instead")]
		public bool UseProcedureBodies
		{
			get
			{
				return (bool)this.values["useprocedurebodies"];
			}
			set
			{
				this.SetValue("useprocedurebodies", value);
			}
		}

		[Category("Advanced"), DefaultValue(true), Description("Should the connetion automatically enlist in the active connection, if there are any."), DisplayName("Auto Enlist"), RefreshProperties(RefreshProperties.All)]
		public bool AutoEnlist
		{
			get
			{
				return (bool)this.values["autoenlist"];
			}
			set
			{
				this.SetValue("autoenlist", value);
			}
		}

		[Category("Advanced"), DefaultValue(true), Description("Should binary flags on column metadata be respected."), DisplayName("Respect Binary Flags"), RefreshProperties(RefreshProperties.All)]
		public bool RespectBinaryFlags
		{
			get
			{
				return (bool)this.values["respectbinaryflags"];
			}
			set
			{
				this.SetValue("respectbinaryflags", value);
			}
		}

		[Category("Advanced"), DefaultValue(true), Description("Should the provider treat TINYINT(1) columns as boolean."), DisplayName("Treat Tiny As Boolean"), RefreshProperties(RefreshProperties.All)]
		public bool TreatTinyAsBoolean
		{
			get
			{
				return (bool)this.values["treattinyasboolean"];
			}
			set
			{
				this.SetValue("treattinyasboolean", value);
			}
		}

		[Category("Advanced"), DefaultValue(false), Description("Should the provider expect user variables to appear in the SQL."), DisplayName("Allow User Variables"), RefreshProperties(RefreshProperties.All)]
		public bool AllowUserVariables
		{
			get
			{
				return (bool)this.values["allowuservariables"];
			}
			set
			{
				this.SetValue("allowuservariables", value);
			}
		}

		[Category("Advanced"), DefaultValue(false), Description("Should this session be considered interactive?"), DisplayName("Interactive Session"), RefreshProperties(RefreshProperties.All)]
		public bool InteractiveSession
		{
			get
			{
				return (bool)this.values["interactivesession"];
			}
			set
			{
				this.SetValue("interactivesession", value);
			}
		}

		[Category("Advanced"), DefaultValue(false), Description("Should all server functions be treated as returning string?"), DisplayName("Functions Return String")]
		public bool FunctionsReturnString
		{
			get
			{
				return (bool)this.values["functionsreturnstring"];
			}
			set
			{
				this.SetValue("functionsreturnstring", value);
			}
		}

		[Category("Advanced"), DefaultValue(false), Description("Should the returned affected row count reflect affected rows instead of found rows?"), DisplayName("Use Affected Rows")]
		public bool UseAffectedRows
		{
			get
			{
				return (bool)this.values["useaffectedrows"];
			}
			set
			{
				this.SetValue("useaffectedrows", value);
			}
		}

		[Category("Advanced"), DefaultValue(false), Description("Treat binary(16) columns as guids"), DisplayName("Old Guids")]
		public bool OldGuids
		{
			get
			{
				return (bool)this.values["oldguids"];
			}
			set
			{
				this.SetValue("oldguids", value);
			}
		}

		[DefaultValue(0), Description("For TCP connections, idle connection time measured in seconds, before the first keepalive packet is sent.A value of 0 indicates that keepalive is not used."), DisplayName("Keep Alive")]
		public uint Keepalive
		{
			get
			{
				return (uint)this.values["keepalive"];
			}
			set
			{
				this.SetValue("keepalive", value);
			}
		}

		[Category("Advanced"), DefaultValue(false), Description("Allow Sql Server syntax.  A value of yes allows symbols to be enclosed with [] instead of ``.  This does incur a performance hit so only use when necessary."), DisplayName("Sql Server Mode")]
		public bool SqlServerMode
		{
			get
			{
				return (bool)this.values["sqlservermode"];
			}
			set
			{
				this.SetValue("sqlservermode", value);
			}
		}

		[Category("Advanced"), DefaultValue(false), Description("Enables or disables caching of TableDirect command.  \r\n            A value of yes enables the cache while no disables it."), DisplayName("Table Cache")]
		public bool TableCaching
		{
			get
			{
				return (bool)this.values["tablecaching"];
			}
			set
			{
				this.SetValue("tablecachig", value);
			}
		}

		[Category("Advanced"), DefaultValue(60), Description("Specifies how long a TableDirect result should be cached in seconds."), DisplayName("Default Table Cache Age")]
		public int DefaultTableCacheAge
		{
			get
			{
				return (int)this.values["defaulttablecacheage"];
			}
			set
			{
				this.SetValue("defaulttablecacheage", value);
			}
		}

		[Category("Advanced"), DefaultValue(true), Description("Indicates if stored routine parameters should be checked against the server."), DisplayName("Check Parameters")]
		public bool CheckParameters
		{
			get
			{
				return (bool)this.values["checkparameters"];
			}
			set
			{
				this.SetValue("checkparameters", value);
			}
		}

		[Category("Advanced"), DefaultValue(false), Description("Indicates if this connection is to use replicated servers."), DisplayName("Replication")]
		public bool Replication
		{
			get
			{
				return (bool)this.values["replication"];
			}
			set
			{
				this.SetValue("replication", value);
			}
		}

		[Category("Advanced"), Description("The list of interceptors that can triage thrown MySqlExceptions."), DisplayName("Exception Interceptors")]
		public string ExceptionInterceptors
		{
			get
			{
				return (string)this.values["exceptioninterceptors"];
			}
			set
			{
				this.SetValue("exceptioninterceptors", value);
			}
		}

		[Category("Advanced"), Description("The list of interceptors that can intercept command operations."), DisplayName("Command Interceptors")]
		public string CommandInterceptors
		{
			get
			{
				return (string)this.values["commandinterceptors"];
			}
			set
			{
				this.SetValue("commandinterceptors", value);
			}
		}

		[Category("Advanced"), DefaultValue(false), Description("Include security asserts to support Medium Trust"), DisplayName("Include Security Asserts")]
		public bool IncludeSecurityAsserts
		{
			get
			{
				return (bool)this.values["includesecurityasserts"];
			}
			set
			{
				this.SetValue("includesecurityasserts", value);
			}
		}

		[Category("Pooling"), DefaultValue(0), Description("The minimum amount of time (in seconds) for this connection to live in the pool before being destroyed."), DisplayName("Connection Lifetime"), RefreshProperties(RefreshProperties.All)]
		public uint ConnectionLifeTime
		{
			get
			{
				return (uint)this.values["connectionlifetime"];
			}
			set
			{
				this.SetValue("connectionlifetime", value);
			}
		}

		[Category("Pooling"), DefaultValue(true), Description("When true, the connection object is drawn from the appropriate pool, or if necessary, is created and added to the appropriate pool."), RefreshProperties(RefreshProperties.All)]
		public bool Pooling
		{
			get
			{
				return (bool)this.values["pooling"];
			}
			set
			{
				this.SetValue("pooling", value);
			}
		}

		[Category("Pooling"), DefaultValue(0), Description("The minimum number of connections allowed in the pool."), DisplayName("Minimum Pool Size"), RefreshProperties(RefreshProperties.All)]
		public uint MinimumPoolSize
		{
			get
			{
				return (uint)this.values["minpoolsize"];
			}
			set
			{
				this.SetValue("minpoolsize", value);
			}
		}

		[Category("Pooling"), DefaultValue(100), Description("The maximum number of connections allowed in the pool."), DisplayName("Maximum Pool Size"), RefreshProperties(RefreshProperties.All)]
		public uint MaximumPoolSize
		{
			get
			{
				return (uint)this.values["maxpoolsize"];
			}
			set
			{
				this.SetValue("maxpoolsize", value);
			}
		}

		[Category("Pooling"), DefaultValue(false), Description("When true, indicates the connection state is reset when removed from the pool."), DisplayName("Connection Reset"), RefreshProperties(RefreshProperties.All)]
		public bool ConnectionReset
		{
			get
			{
				return (bool)this.values["connectionreset"];
			}
			set
			{
				this.SetValue("connectionreset", value);
			}
		}

		[Category("Pooling"), DefaultValue(false), Description("When true, server properties will be cached after the first server in the pool is created"), DisplayName("Cache Server Properties"), RefreshProperties(RefreshProperties.All)]
		public bool CacheServerProperties
		{
			get
			{
				return (bool)this.values["cacheserverproperties"];
			}
			set
			{
				this.SetValue("cacheserverproperties", value);
			}
		}

		[Category("Advanced"), DefaultValue(""), Description("Character set this connection should use"), DisplayName("Character Set"), RefreshProperties(RefreshProperties.All)]
		public string CharacterSet
		{
			get
			{
				return (string)this.values["characterset"];
			}
			set
			{
				this.SetValue("characterset", value);
			}
		}

		[Category("Advanced"), DefaultValue(false), Description("Should binary blobs be treated as UTF8"), DisplayName("Treat Blobs As UTF8"), RefreshProperties(RefreshProperties.All)]
		public bool TreatBlobsAsUTF8
		{
			get
			{
				return (bool)this.values["treatblobsasutf8"];
			}
			set
			{
				this.SetValue("treatblobsasutf8", value);
			}
		}

		[Category("Advanced"), Description("Pattern that matches columns that should be treated as UTF8"), RefreshProperties(RefreshProperties.All)]
		public string BlobAsUTF8IncludePattern
		{
			get
			{
				return (string)this.values["blobasutf8includepattern"];
			}
			set
			{
				this.SetValue("blobasutf8includepattern", value);
			}
		}

		[Category("Advanced"), Description("Pattern that matches columns that should not be treated as UTF8"), RefreshProperties(RefreshProperties.All)]
		public string BlobAsUTF8ExcludePattern
		{
			get
			{
				return (string)this.values["blobasutf8excludepattern"];
			}
			set
			{
				this.SetValue("blobasutf8excludepattern", value);
			}
		}

		[Category("Security"), DefaultValue(MySqlSslMode.None), Description("SSL properties for connection"), DisplayName("Ssl Mode")]
		public MySqlSslMode SslMode
		{
			get
			{
				return (MySqlSslMode)this.values["sslmode"];
			}
			set
			{
				this.SetValue("sslmode", value);
			}
		}

		[Category("Backwards Compatibility"), DefaultValue(false), Description("Enforces the command timeout of EFMySqlCommand to the value provided in 'DefaultCommandTimeout' property"), DisplayName("Use Default Command Timeout For EF")]
		public bool UseDefaultCommandTimeoutForEF
		{
			get
			{
				return (bool)this.values["usedefaultcommandtimeoutforef"];
			}
			set
			{
				this.SetValue("usedefaultcommandtimeoutforef", value);
			}
		}

		internal bool HasProcAccess
		{
			get;
			set;
		}

		public override object this[string keyword]
		{
			get
			{
				MySqlConnectionStringOption option = this.GetOption(keyword);
				return option.Getter(this, option);
			}
			set
			{
				MySqlConnectionStringOption option = this.GetOption(keyword);
				option.Setter(this, option, value);
			}
		}

		static MySqlConnectionStringBuilder()
		{
			MySqlConnectionStringBuilder.options = new MySqlConnectionStringOptionCollection();
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("server", "host,data source,datasource,address,addr,network address", typeof(string), "", false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("database", "initial catalog", typeof(string), string.Empty, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("protocol", "connection protocol, connectionprotocol", typeof(MySqlConnectionProtocol), MySqlConnectionProtocol.Sockets, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("port", null, typeof(uint), 3306u, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("pipe", "pipe name,pipename", typeof(string), "MYSQL", false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("compress", "use compression,usecompression", typeof(bool), false, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("allowbatch", "allow batch", typeof(bool), true, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("logging", null, typeof(bool), false, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("sharedmemoryname", "shared memory name", typeof(string), "MYSQL", false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("useoldsyntax", "old syntax,oldsyntax,use old syntax", typeof(bool), false, true, delegate(MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender, object value)
			{
				MySqlTrace.LogWarning(-1, "Use Old Syntax is now obsolete.  Please see documentation");
				msb.SetValue("useoldsyntax", value);
			}, (MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender) => (bool)msb.values["useoldsyntax"]));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("connectiontimeout", "connection timeout,connect timeout", typeof(uint), 15u, false, delegate(MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender, object Value)
			{
				uint num = (uint)Convert.ChangeType(Value, sender.BaseType);
				uint num2 = Math.Min(num, 2147483u);
				if (num2 != num)
				{
					MySqlTrace.LogWarning(-1, string.Concat(new object[]
					{
						"Connection timeout value too large (",
						num,
						" seconds). Changed to max. possible value",
						num2,
						" seconds)"
					}));
				}
				msb.SetValue("connectiontimeout", num2);
			}, (MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender) => (uint)msb.values["connectiontimeout"]));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("defaultcommandtimeout", "command timeout,default command timeout", typeof(uint), 30u, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("usedefaultcommandtimeoutforef", "use default command timeout for ef", typeof(bool), false, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("user id", "uid,username,user name,user,userid", typeof(string), "", false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("password", "pwd", typeof(string), "", false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("persistsecurityinfo", "persist security info", typeof(bool), false, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("encrypt", null, typeof(bool), false, true, delegate(MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender, object value)
			{
				sender.ValidateValue(ref value);
				MySqlTrace.LogWarning(-1, "Encrypt is now obsolete. Use Ssl Mode instead");
				msb.SetValue("Ssl Mode", ((bool)value) ? MySqlSslMode.Preferred : MySqlSslMode.None);
			}, (MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender) => msb.SslMode != MySqlSslMode.None));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("certificatefile", "certificate file", typeof(string), null, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("certificatepassword", "certificate password", typeof(string), null, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("certificatestorelocation", "certificate store location", typeof(MySqlCertificateStoreLocation), MySqlCertificateStoreLocation.None, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("certificatethumbprint", "certificate thumb print", typeof(string), null, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("integratedsecurity", "integrated security", typeof(bool), false, false, delegate(MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender, object value)
			{
				if (!Platform.IsWindows())
				{
					throw new MySqlException("IntegratedSecurity is supported on Windows only");
				}
				msb.SetValue("Integrated Security", value);
			}, delegate(MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender)
			{
				object obj = msb.values["Integrated Security"];
				return (bool)obj;
			}));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("allowzerodatetime", "allow zero datetime", typeof(bool), false, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("convertzerodatetime", "convert zero datetime", typeof(bool), false, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("useusageadvisor", "use usage advisor,usage advisor", typeof(bool), false, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("procedurecachesize", "procedure cache size,procedure cache,procedurecache", typeof(uint), 25u, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("useperformancemonitor", "use performance monitor,useperfmon,perfmon", typeof(bool), false, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("ignoreprepare", "ignore prepare", typeof(bool), true, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("useprocedurebodies", "use procedure bodies,procedure bodies", typeof(bool), true, true, delegate(MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender, object value)
			{
				sender.ValidateValue(ref value);
				MySqlTrace.LogWarning(-1, "Use Procedure Bodies is now obsolete.  Use Check Parameters instead");
				msb.SetValue("checkparameters", value);
				msb.SetValue("useprocedurebodies", value);
			}, (MySqlConnectionStringBuilder msb, MySqlConnectionStringOption sender) => (bool)msb.values["useprocedurebodies"]));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("autoenlist", "auto enlist", typeof(bool), true, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("respectbinaryflags", "respect binary flags", typeof(bool), true, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("treattinyasboolean", "treat tiny as boolean", typeof(bool), true, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("allowuservariables", "allow user variables", typeof(bool), false, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("interactivesession", "interactive session,interactive", typeof(bool), false, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("functionsreturnstring", "functions return string", typeof(bool), false, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("useaffectedrows", "use affected rows", typeof(bool), false, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("oldguids", "old guids", typeof(bool), false, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("keepalive", "keep alive", typeof(uint), 0u, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("sqlservermode", "sql server mode", typeof(bool), false, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("tablecaching", "table cache,tablecache", typeof(bool), false, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("defaulttablecacheage", "default table cache age", typeof(int), 0, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("checkparameters", "check parameters", typeof(bool), true, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("replication", null, typeof(bool), false, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("exceptioninterceptors", "exception interceptors", typeof(string), null, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("commandinterceptors", "command interceptors", typeof(string), null, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("includesecurityasserts", "include security asserts", typeof(bool), false, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("connectionlifetime", "connection lifetime", typeof(uint), 0u, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("pooling", null, typeof(bool), true, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("minpoolsize", "min pool size,minimum pool size", typeof(uint), 0u, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("maxpoolsize", "max pool size,maximum pool size", typeof(uint), 1000u, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("connectionreset", "connection reset", typeof(bool), false, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("cacheserverproperties", "cache server properties", typeof(bool), false, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("characterset", "character set,charset", typeof(string), "", false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("treatblobsasutf8", "treat blobs as utf8", typeof(bool), false, false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("blobasutf8includepattern", null, typeof(string), "", false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("blobasutf8excludepattern", null, typeof(string), "", false));
			MySqlConnectionStringBuilder.options.Add(new MySqlConnectionStringOption("sslmode", "ssl mode", typeof(MySqlSslMode), MySqlSslMode.None, false));
		}

		public MySqlConnectionStringBuilder()
		{
			this.HasProcAccess = true;
			lock (this)
			{
				for (int i = 0; i < MySqlConnectionStringBuilder.options.Options.Count; i++)
				{
					this.values[MySqlConnectionStringBuilder.options.Options[i].Keyword] = MySqlConnectionStringBuilder.options.Options[i].DefaultValue;
				}
			}
		}

		public MySqlConnectionStringBuilder(string connStr)
		{
			lock (this)
			{
				base.ConnectionString = connStr;
			}
		}

		internal Regex GetBlobAsUTF8IncludeRegex()
		{
			if (string.IsNullOrEmpty(this.BlobAsUTF8IncludePattern))
			{
				return null;
			}
			return new Regex(this.BlobAsUTF8IncludePattern);
		}

		internal Regex GetBlobAsUTF8ExcludeRegex()
		{
			if (string.IsNullOrEmpty(this.BlobAsUTF8ExcludePattern))
			{
				return null;
			}
			return new Regex(this.BlobAsUTF8ExcludePattern);
		}

		public override void Clear()
		{
			base.Clear();
			lock (this)
			{
				foreach (MySqlConnectionStringOption current in MySqlConnectionStringBuilder.options.Options)
				{
					if (current.DefaultValue != null)
					{
						this.values[current.Keyword] = current.DefaultValue;
					}
					else
					{
						this.values[current.Keyword] = null;
					}
				}
			}
		}

		internal void SetValue(string keyword, object value)
		{
			MySqlConnectionStringOption option = this.GetOption(keyword);
			option.ValidateValue(ref value);
			option.Clean(this);
			if (value != null)
			{
				lock (this)
				{
					this.values[option.Keyword] = value;
					base[keyword] = value;
				}
			}
		}

		private MySqlConnectionStringOption GetOption(string key)
		{
			MySqlConnectionStringOption mySqlConnectionStringOption = MySqlConnectionStringBuilder.options.Get(key);
			if (mySqlConnectionStringOption == null)
			{
				throw new ArgumentException(Resources.KeywordNotSupported, key);
			}
			return mySqlConnectionStringOption;
		}

		public override bool ContainsKey(string keyword)
		{
			MySqlConnectionStringOption mySqlConnectionStringOption = MySqlConnectionStringBuilder.options.Get(keyword);
			return mySqlConnectionStringOption != null;
		}

		public override bool Remove(string keyword)
		{
			bool flag = false;
			lock (this)
			{
				flag = base.Remove(keyword);
			}
			if (!flag)
			{
				return false;
			}
			MySqlConnectionStringOption option = this.GetOption(keyword);
			lock (this)
			{
				this.values[option.Keyword] = option.DefaultValue;
			}
			return true;
		}

		public string GetConnectionString(bool includePass)
		{
			if (includePass)
			{
				return base.ConnectionString;
			}
			StringBuilder stringBuilder = new StringBuilder();
			string text = "";
			foreach (string text2 in this.Keys)
			{
				if (string.Compare(text2, "password", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(text2, "pwd", StringComparison.OrdinalIgnoreCase) != 0)
				{
					stringBuilder.AppendFormat(CultureInfo.CurrentCulture, "{0}{1}={2}", new object[]
					{
						text,
						text2,
						this[text2]
					});
					text = ";";
				}
			}
			return stringBuilder.ToString();
		}
	}
}
