using MySql.Data.Common;
using MySql.Data.MySqlClient.Authentication;
using MySql.Data.MySqlClient.Properties;
using MySql.Data.Types;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Net.Security;
using System.Reflection;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MySql.Data.MySqlClient
{
	internal class NativeDriver : IDriver
	{
		private const string AuthenticationWindowsPlugin = "authentication_windows_client";

		private const string AuthenticationWindowsUser = "auth_windows";

		private DBVersion version;

		private int threadId;

		protected string encryptionSeed;

		protected ServerStatusFlags serverStatus;

		protected MySqlStream stream;

		protected Stream baseStream;

		private BitArray nullMap;

		private MySqlPacket packet;

		private ClientFlags connectionFlags;

		private Driver owner;

		private int warnings;

		private MySqlAuthenticationPlugin authPlugin;

		public ClientFlags Flags
		{
			get
			{
				return this.connectionFlags;
			}
		}

		public int ThreadId
		{
			get
			{
				return this.threadId;
			}
		}

		public DBVersion Version
		{
			get
			{
				return this.version;
			}
		}

		public ServerStatusFlags ServerStatus
		{
			get
			{
				return this.serverStatus;
			}
		}

		public int WarningCount
		{
			get
			{
				return this.warnings;
			}
		}

		public MySqlPacket Packet
		{
			get
			{
				return this.packet;
			}
		}

		internal MySqlConnectionStringBuilder Settings
		{
			get
			{
				return this.owner.Settings;
			}
		}

		internal Encoding Encoding
		{
			get
			{
				return this.owner.Encoding;
			}
		}

		public NativeDriver(Driver owner)
		{
			this.owner = owner;
			this.threadId = -1;
		}

		private void HandleException(MySqlException ex)
		{
			if (ex.IsFatal)
			{
				this.owner.Close();
			}
		}

		internal void SendPacket(MySqlPacket p)
		{
			this.stream.SendPacket(p);
		}

		internal void SendEmptyPacket()
		{
			byte[] buffer = new byte[4];
			this.stream.SendEntirePacketDirectly(buffer, 0);
		}

		internal MySqlPacket ReadPacket()
		{
			return this.packet = this.stream.ReadPacket();
		}

		internal void ReadOk(bool read)
		{
			try
			{
				if (read)
				{
					this.packet = this.stream.ReadPacket();
				}
				byte b = this.packet.ReadByte();
				if (b != 0)
				{
					throw new MySqlException("Out of sync with server", true, null);
				}
				this.packet.ReadFieldLength();
				this.packet.ReadFieldLength();
				if (this.packet.HasMoreData)
				{
					this.serverStatus = (ServerStatusFlags)this.packet.ReadInteger(2);
					this.packet.ReadInteger(2);
					if (this.packet.HasMoreData)
					{
						this.packet.ReadLenString();
					}
				}
			}
			catch (MySqlException ex)
			{
				this.HandleException(ex);
				throw;
			}
		}

		public void SetDatabase(string dbName)
		{
			byte[] bytes = this.Encoding.GetBytes(dbName);
			this.packet.Clear();
			this.packet.WriteByte(2);
			this.packet.Write(bytes);
			this.ExecutePacket(this.packet);
			this.ReadOk(true);
		}

		public void Configure()
		{
			this.stream.MaxPacketSize = (ulong)this.owner.MaxPacketSize;
			this.stream.Encoding = this.Encoding;
		}

		public void Open()
		{
			try
			{
				this.baseStream = StreamCreator.GetStream(this.Settings);
				if (this.Settings.IncludeSecurityAsserts)
				{
					MySqlSecurityPermission.CreatePermissionSet(false).Assert();
				}
			}
			catch (SecurityException)
			{
				throw;
			}
			catch (Exception inner)
			{
				throw new MySqlException(Resources.UnableToConnectToHost, 1042, inner);
			}
			if (this.baseStream == null)
			{
				throw new MySqlException(Resources.UnableToConnectToHost, 1042);
			}
			this.stream = new MySqlStream(this.baseStream, this.Encoding, false);
			this.stream.ResetTimeout((int)(this.Settings.ConnectionTimeout * 1000u));
			this.packet = this.stream.ReadPacket();
			this.packet.ReadByte();
			string versionString = this.packet.ReadString();
			this.version = DBVersion.Parse(versionString);
			if (!this.version.isAtLeast(5, 0, 0))
			{
				throw new NotSupportedException(Resources.ServerTooOld);
			}
			this.threadId = this.packet.ReadInteger(4);
			this.encryptionSeed = this.packet.ReadString();
			int num = 16777215;
			ClientFlags clientFlags = (ClientFlags)0uL;
			if (this.packet.HasMoreData)
			{
				clientFlags = (ClientFlags)((long)this.packet.ReadInteger(2));
			}
			this.owner.ConnectionCharSetIndex = (int)this.packet.ReadByte();
			this.serverStatus = (ServerStatusFlags)this.packet.ReadInteger(2);
			uint num2 = (uint)this.packet.ReadInteger(2);
			clientFlags |= (ClientFlags)(num2 << 16);
			this.packet.Position += 11;
			string str = this.packet.ReadString();
			this.encryptionSeed += str;
			string authMethod;
			if ((clientFlags & ClientFlags.PLUGIN_AUTH) != (ClientFlags)0uL)
			{
				authMethod = this.packet.ReadString();
			}
			else
			{
				authMethod = "mysql_native_password";
			}
			this.SetConnectionFlags(clientFlags);
			this.packet.Clear();
			this.packet.WriteInteger((long)((int)this.connectionFlags), 4);
			if ((clientFlags & ClientFlags.SSL) == (ClientFlags)0uL)
			{
				if (this.Settings.SslMode != MySqlSslMode.None && this.Settings.SslMode != MySqlSslMode.Preferred)
				{
					string msg = string.Format(Resources.NoServerSSLSupport, this.Settings.Server);
					throw new MySqlException(msg);
				}
			}
			else if (this.Settings.SslMode != MySqlSslMode.None)
			{
				this.stream.SendPacket(this.packet);
				this.StartSSL();
				this.packet.Clear();
				this.packet.WriteInteger((long)((int)this.connectionFlags), 4);
			}
			this.packet.WriteInteger((long)num, 4);
			this.packet.WriteByte(8);
			this.packet.Write(new byte[23]);
			this.Authenticate(authMethod, false);
			if ((this.connectionFlags & ClientFlags.COMPRESS) != (ClientFlags)0uL)
			{
				this.stream = new MySqlStream(this.baseStream, this.Encoding, true);
			}
			this.packet.Version = this.version;
			this.stream.MaxBlockSize = num;
		}

		private X509CertificateCollection GetClientCertificates()
		{
			X509CertificateCollection x509CertificateCollection = new X509CertificateCollection();
			if (this.Settings.CertificateFile != null)
			{
				if (!this.Version.isAtLeast(5, 1, 0))
				{
					throw new MySqlException(Resources.FileBasedCertificateNotSupported);
				}
				X509Certificate2 value = new X509Certificate2(this.Settings.CertificateFile, this.Settings.CertificatePassword);
				x509CertificateCollection.Add(value);
				return x509CertificateCollection;
			}
			else
			{
				if (this.Settings.CertificateStoreLocation == MySqlCertificateStoreLocation.None)
				{
					return x509CertificateCollection;
				}
				StoreLocation storeLocation = (this.Settings.CertificateStoreLocation == MySqlCertificateStoreLocation.CurrentUser) ? StoreLocation.CurrentUser : StoreLocation.LocalMachine;
				X509Store x509Store = new X509Store(StoreName.My, storeLocation);
				x509Store.Open(OpenFlags.OpenExistingOnly);
				if (this.Settings.CertificateThumbprint == null)
				{
					x509CertificateCollection.AddRange(x509Store.Certificates);
					return x509CertificateCollection;
				}
				x509CertificateCollection.AddRange(x509Store.Certificates.Find(X509FindType.FindByThumbprint, this.Settings.CertificateThumbprint, true));
				if (x509CertificateCollection.Count == 0)
				{
					throw new MySqlException("Certificate with Thumbprint " + this.Settings.CertificateThumbprint + " not found");
				}
				return x509CertificateCollection;
			}
		}

		private void StartSSL()
		{
			RemoteCertificateValidationCallback userCertificateValidationCallback = new RemoteCertificateValidationCallback(this.ServerCheckValidation);
			SslStream sslStream = new SslStream(this.baseStream, true, userCertificateValidationCallback, null);
			X509CertificateCollection clientCertificates = this.GetClientCertificates();
			sslStream.AuthenticateAsClient(this.Settings.Server, clientCertificates, SslProtocols.Default, false);
			this.baseStream = sslStream;
			this.stream = new MySqlStream(sslStream, this.Encoding, false);
			this.stream.SequenceByte = 2;
		}

		private bool ServerCheckValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return sslPolicyErrors == SslPolicyErrors.None || (this.Settings.SslMode == MySqlSslMode.Preferred || this.Settings.SslMode == MySqlSslMode.Required) || (this.Settings.SslMode == MySqlSslMode.VerifyCA && sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch);
		}

		private void SetConnectionFlags(ClientFlags serverCaps)
		{
			ClientFlags clientFlags = ClientFlags.LOCAL_FILES;
			if (!this.Settings.UseAffectedRows)
			{
				clientFlags |= ClientFlags.FOUND_ROWS;
			}
			clientFlags |= ClientFlags.PROTOCOL_41;
			clientFlags |= ClientFlags.TRANSACTIONS;
			if (this.Settings.AllowBatch)
			{
				clientFlags |= ClientFlags.MULTI_STATEMENTS;
			}
			clientFlags |= ClientFlags.MULTI_RESULTS;
			if ((serverCaps & ClientFlags.LONG_FLAG) != (ClientFlags)0uL)
			{
				clientFlags |= ClientFlags.LONG_FLAG;
			}
			if ((serverCaps & ClientFlags.COMPRESS) != (ClientFlags)0uL && this.Settings.UseCompression)
			{
				clientFlags |= ClientFlags.COMPRESS;
			}
			clientFlags |= ClientFlags.LONG_PASSWORD;
			if (this.Settings.InteractiveSession)
			{
				clientFlags |= ClientFlags.INTERACTIVE;
			}
			if ((serverCaps & ClientFlags.CONNECT_WITH_DB) != (ClientFlags)0uL && this.Settings.Database != null && this.Settings.Database.Length > 0)
			{
				clientFlags |= ClientFlags.CONNECT_WITH_DB;
			}
			if ((serverCaps & ClientFlags.SECURE_CONNECTION) != (ClientFlags)0uL)
			{
				clientFlags |= ClientFlags.SECURE_CONNECTION;
			}
			if ((serverCaps & ClientFlags.SSL) != (ClientFlags)0uL && this.Settings.SslMode != MySqlSslMode.None)
			{
				clientFlags |= ClientFlags.SSL;
			}
			if ((serverCaps & ClientFlags.PS_MULTI_RESULTS) != (ClientFlags)0uL)
			{
				clientFlags |= ClientFlags.PS_MULTI_RESULTS;
			}
			if ((serverCaps & ClientFlags.PLUGIN_AUTH) != (ClientFlags)0uL)
			{
				clientFlags |= ClientFlags.PLUGIN_AUTH;
			}
			if ((serverCaps & ClientFlags.CONNECT_ATTRS) != (ClientFlags)0uL)
			{
				clientFlags |= ClientFlags.CONNECT_ATTRS;
			}
			if ((serverCaps & ClientFlags.CAN_HANDLE_EXPIRED_PASSWORD) != (ClientFlags)0uL)
			{
				clientFlags |= ClientFlags.CAN_HANDLE_EXPIRED_PASSWORD;
			}
			this.connectionFlags = clientFlags;
		}

		public void Authenticate(string authMethod, bool reset)
		{
			if (authMethod != null)
			{
				byte[] bytes = this.Encoding.GetBytes(this.encryptionSeed);
				if (this.Settings.IntegratedSecurity)
				{
					authMethod = "authentication_windows_client";
				}
				this.authPlugin = MySqlAuthenticationPlugin.GetPlugin(authMethod, this, bytes);
			}
			this.authPlugin.Authenticate(reset);
		}

		public void Reset()
		{
			this.warnings = 0;
			this.stream.Encoding = this.Encoding;
			this.stream.SequenceByte = 0;
			this.packet.Clear();
			this.packet.WriteByte(17);
			this.Authenticate(null, true);
		}

		public void SendQuery(MySqlPacket queryPacket)
		{
			this.warnings = 0;
			queryPacket.SetByte(4L, 3);
			this.ExecutePacket(queryPacket);
			this.serverStatus |= ServerStatusFlags.AnotherQuery;
		}

		public void Close(bool isOpen)
		{
			try
			{
				if (isOpen)
				{
					try
					{
						this.packet.Clear();
						this.packet.WriteByte(1);
						this.ExecutePacket(this.packet);
					}
					catch (Exception)
					{
					}
				}
				if (this.stream != null)
				{
					this.stream.Close();
				}
				this.stream = null;
			}
			catch (Exception)
			{
			}
		}

		public bool Ping()
		{
			bool result;
			try
			{
				this.packet.Clear();
				this.packet.WriteByte(14);
				this.ExecutePacket(this.packet);
				this.ReadOk(true);
				result = true;
			}
			catch (Exception)
			{
				this.owner.Close();
				result = false;
			}
			return result;
		}

		public int GetResult(ref int affectedRow, ref long insertedId)
		{
			try
			{
				this.packet = this.stream.ReadPacket();
			}
			catch (TimeoutException)
			{
				throw;
			}
			catch (Exception)
			{
				this.serverStatus = (ServerStatusFlags)0;
				throw;
			}
			int num = (int)this.packet.ReadFieldLength();
			if (-1 == num)
			{
				string filename = this.packet.ReadString();
				this.SendFileToServer(filename);
				return this.GetResult(ref affectedRow, ref insertedId);
			}
			if (num == 0)
			{
				this.serverStatus &= ~(ServerStatusFlags.MoreResults | ServerStatusFlags.AnotherQuery);
				affectedRow = (int)this.packet.ReadFieldLength();
				insertedId = this.packet.ReadFieldLength();
				this.serverStatus = (ServerStatusFlags)this.packet.ReadInteger(2);
				this.warnings += this.packet.ReadInteger(2);
				if (this.packet.HasMoreData)
				{
					this.packet.ReadLenString();
				}
			}
			return num;
		}

		private void SendFileToServer(string filename)
		{
			byte[] buffer = new byte[8196];
			try
			{
				using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
				{
					int num2;
					for (long num = fileStream.Length; num > 0L; num -= (long)num2)
					{
						num2 = fileStream.Read(buffer, 4, (int)((num > 8192L) ? 8192L : num));
						this.stream.SendEntirePacketDirectly(buffer, num2);
					}
					this.stream.SendEntirePacketDirectly(buffer, 0);
				}
			}
			catch (Exception ex)
			{
				throw new MySqlException("Error during LOAD DATA LOCAL INFILE", ex);
			}
		}

		private void ReadNullMap(int fieldCount)
		{
			this.nullMap = null;
			byte[] array = new byte[(fieldCount + 9) / 8];
			this.packet.ReadByte();
			this.packet.Read(array, 0, array.Length);
			this.nullMap = new BitArray(array);
		}

		public IMySqlValue ReadColumnValue(int index, MySqlField field, IMySqlValue valObject)
		{
			long num = -1L;
			bool isNull;
			if (this.nullMap != null)
			{
				isNull = this.nullMap[index + 2];
			}
			else
			{
				num = this.packet.ReadFieldLength();
				isNull = (num == -1L);
			}
			this.packet.Encoding = field.Encoding;
			this.packet.Version = this.version;
			return valObject.ReadValue(this.packet, num, isNull);
		}

		public void SkipColumnValue(IMySqlValue valObject)
		{
			int num = -1;
			if (this.nullMap == null)
			{
				num = (int)this.packet.ReadFieldLength();
				if (num == -1)
				{
					return;
				}
			}
			if (num > -1)
			{
				this.packet.Position += num;
				return;
			}
			valObject.SkipValue(this.packet);
		}

		public void GetColumnsData(MySqlField[] columns)
		{
			for (int i = 0; i < columns.Length; i++)
			{
				this.GetColumnData(columns[i]);
			}
			this.ReadEOF();
		}

		private void GetColumnData(MySqlField field)
		{
			this.stream.Encoding = this.Encoding;
			this.packet = this.stream.ReadPacket();
			field.Encoding = this.Encoding;
			field.CatalogName = this.packet.ReadLenString();
			field.DatabaseName = this.packet.ReadLenString();
			field.TableName = this.packet.ReadLenString();
			field.RealTableName = this.packet.ReadLenString();
			field.ColumnName = this.packet.ReadLenString();
			field.OriginalColumnName = this.packet.ReadLenString();
			this.packet.ReadByte();
			field.CharacterSetIndex = this.packet.ReadInteger(2);
			field.ColumnLength = this.packet.ReadInteger(4);
			MySqlDbType mySqlDbType = (MySqlDbType)this.packet.ReadByte();
			ColumnFlags columnFlags;
			if ((this.connectionFlags & ClientFlags.LONG_FLAG) != (ClientFlags)0uL)
			{
				columnFlags = (ColumnFlags)this.packet.ReadInteger(2);
			}
			else
			{
				columnFlags = (ColumnFlags)this.packet.ReadByte();
			}
			field.Scale = this.packet.ReadByte();
			if (this.packet.HasMoreData)
			{
				this.packet.ReadInteger(2);
			}
			if (mySqlDbType == MySqlDbType.Decimal || mySqlDbType == MySqlDbType.NewDecimal)
			{
				field.Precision = (byte)(field.ColumnLength - 2);
				if ((columnFlags & ColumnFlags.UNSIGNED) != (ColumnFlags)0)
				{
					field.Precision += 1;
				}
			}
			field.SetTypeAndFlags(mySqlDbType, columnFlags);
		}

		private void ExecutePacket(MySqlPacket packetToExecute)
		{
			try
			{
				this.warnings = 0;
				this.stream.SequenceByte = 0;
				this.stream.SendPacket(packetToExecute);
			}
			catch (MySqlException ex)
			{
				this.HandleException(ex);
				throw;
			}
		}

		public void ExecuteStatement(MySqlPacket packetToExecute)
		{
			this.warnings = 0;
			packetToExecute.SetByte(4L, 23);
			this.ExecutePacket(packetToExecute);
			this.serverStatus |= ServerStatusFlags.AnotherQuery;
		}

		private void CheckEOF()
		{
			if (!this.packet.IsLastPacket)
			{
				throw new MySqlException("Expected end of data packet");
			}
			this.packet.ReadByte();
			if (this.packet.HasMoreData)
			{
				this.warnings += this.packet.ReadInteger(2);
				this.serverStatus = (ServerStatusFlags)this.packet.ReadInteger(2);
			}
		}

		private void ReadEOF()
		{
			this.packet = this.stream.ReadPacket();
			this.CheckEOF();
		}

		public int PrepareStatement(string sql, ref MySqlField[] parameters)
		{
			this.packet.Length = sql.Length * 4 + 5;
			byte[] buffer = this.packet.Buffer;
			int bytes = this.Encoding.GetBytes(sql, 0, sql.Length, this.packet.Buffer, 5);
			this.packet.Position = bytes + 5;
			buffer[4] = 22;
			this.ExecutePacket(this.packet);
			this.packet = this.stream.ReadPacket();
			int num = (int)this.packet.ReadByte();
			if (num != 0)
			{
				throw new MySqlException("Expected prepared statement marker");
			}
			int result = this.packet.ReadInteger(4);
			int num2 = this.packet.ReadInteger(2);
			int num3 = this.packet.ReadInteger(2);
			this.packet.ReadInteger(3);
			if (num3 > 0)
			{
				parameters = this.owner.GetColumns(num3);
				for (int i = 0; i < parameters.Length; i++)
				{
					parameters[i].Encoding = this.Encoding;
				}
			}
			if (num2 > 0)
			{
				while (num2-- > 0)
				{
					this.packet = this.stream.ReadPacket();
				}
				this.ReadEOF();
			}
			return result;
		}

		public bool FetchDataRow(int statementId, int columns)
		{
			this.packet = this.stream.ReadPacket();
			if (this.packet.IsLastPacket)
			{
				this.CheckEOF();
				return false;
			}
			this.nullMap = null;
			if (statementId > 0)
			{
				this.ReadNullMap(columns);
			}
			return true;
		}

		public void CloseStatement(int statementId)
		{
			this.packet.Clear();
			this.packet.WriteByte(25);
			this.packet.WriteInteger((long)statementId, 4);
			this.stream.SequenceByte = 0;
			this.stream.SendPacket(this.packet);
		}

		public void ResetTimeout(int timeout)
		{
			if (this.stream != null)
			{
				this.stream.ResetTimeout(timeout);
			}
		}

		internal void SetConnectAttrs()
		{
			if ((this.connectionFlags & ClientFlags.CONNECT_ATTRS) != (ClientFlags)0uL)
			{
				string text = string.Empty;
				MySqlConnectAttrs mySqlConnectAttrs = new MySqlConnectAttrs();
				PropertyInfo[] properties = mySqlConnectAttrs.GetType().GetProperties();
				for (int i = 0; i < properties.Length; i++)
				{
					PropertyInfo propertyInfo = properties[i];
					string text2 = propertyInfo.Name;
					object[] customAttributes = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false);
					if (customAttributes.Length > 0)
					{
						text2 = (customAttributes[0] as DisplayNameAttribute).DisplayName;
					}
					string text3 = (string)propertyInfo.GetValue(mySqlConnectAttrs, null);
					text += string.Format("{0}{1}", (char)text2.Length, text2);
					text += string.Format("{0}{1}", (char)text3.Length, text3);
				}
				this.packet.WriteLenString(text);
			}
		}
	}
}
