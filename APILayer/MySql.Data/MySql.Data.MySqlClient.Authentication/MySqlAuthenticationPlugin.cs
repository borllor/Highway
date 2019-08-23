using MySql.Data.MySqlClient.Properties;
using System;
using System.Text;

namespace MySql.Data.MySqlClient.Authentication
{
	public abstract class MySqlAuthenticationPlugin
	{
		private NativeDriver driver;

		protected byte[] AuthenticationData;

		protected MySqlConnectionStringBuilder Settings
		{
			get
			{
				return this.driver.Settings;
			}
		}

		protected Version ServerVersion
		{
			get
			{
				return new Version(this.driver.Version.Major, this.driver.Version.Minor, this.driver.Version.Build);
			}
		}

		internal ClientFlags Flags
		{
			get
			{
				return this.driver.Flags;
			}
		}

		protected Encoding Encoding
		{
			get
			{
				return this.driver.Encoding;
			}
		}

		public abstract string PluginName
		{
			get;
		}

		internal static MySqlAuthenticationPlugin GetPlugin(string method, NativeDriver driver, byte[] authData)
		{
			if (method == "mysql_old_password")
			{
				driver.Close(true);
				throw new MySqlException(Resources.OldPasswordsNotSupported);
			}
			MySqlAuthenticationPlugin plugin = AuthenticationPluginManager.GetPlugin(method);
			if (plugin == null)
			{
				throw new MySqlException(string.Format(Resources.UnknownAuthenticationMethod, method));
			}
			plugin.driver = driver;
			plugin.SetAuthData(authData);
			return plugin;
		}

		protected virtual void SetAuthData(byte[] data)
		{
			this.AuthenticationData = data;
		}

		protected virtual void CheckConstraints()
		{
		}

		protected virtual void AuthenticationFailed(Exception ex)
		{
			string msg = string.Format(Resources.AuthenticationFailed, new object[]
			{
				this.Settings.Server,
				this.GetUsername(),
				this.PluginName,
				ex.Message
			});
			throw new MySqlException(msg, ex);
		}

		protected virtual void AuthenticationSuccessful()
		{
		}

		protected virtual byte[] MoreData(byte[] data)
		{
			return null;
		}

		internal void Authenticate(bool reset)
		{
			this.CheckConstraints();
			MySqlPacket mySqlPacket = this.driver.Packet;
			mySqlPacket.WriteString(this.GetUsername());
			this.WritePassword(mySqlPacket);
			if (((this.Flags & ClientFlags.CONNECT_WITH_DB) != (ClientFlags)0uL || reset) && !string.IsNullOrEmpty(this.Settings.Database))
			{
				mySqlPacket.WriteString(this.Settings.Database);
			}
			if (reset)
			{
				mySqlPacket.WriteInteger(8L, 2);
			}
			if ((this.Flags & ClientFlags.PLUGIN_AUTH) != (ClientFlags)0uL)
			{
				mySqlPacket.WriteString(this.PluginName);
			}
			this.driver.SetConnectAttrs();
			this.driver.SendPacket(mySqlPacket);
			mySqlPacket = this.ReadPacket();
			byte[] buffer = mySqlPacket.Buffer;
			if (buffer[0] == 254)
			{
				if (mySqlPacket.IsLastPacket)
				{
					this.driver.Close(true);
					throw new MySqlException(Resources.OldPasswordsNotSupported);
				}
				this.HandleAuthChange(mySqlPacket);
			}
			this.driver.ReadOk(false);
			this.AuthenticationSuccessful();
		}

		private void WritePassword(MySqlPacket packet)
		{
			bool flag = (this.Flags & ClientFlags.SECURE_CONNECTION) != (ClientFlags)0uL;
			object password = this.GetPassword();
			if (password is string)
			{
				if (flag)
				{
					packet.WriteLenString((string)password);
					return;
				}
				packet.WriteString((string)password);
				return;
			}
			else
			{
				if (password == null)
				{
					packet.WriteByte(0);
					return;
				}
				if (password is byte[])
				{
					packet.Write(password as byte[]);
					return;
				}
				throw new MySqlException("Unexpected password format: " + password.GetType());
			}
		}

		private MySqlPacket ReadPacket()
		{
			MySqlPacket result;
			try
			{
				MySqlPacket mySqlPacket = this.driver.ReadPacket();
				result = mySqlPacket;
			}
			catch (MySqlException ex)
			{
				this.AuthenticationFailed(ex);
				result = null;
			}
			return result;
		}

		private void HandleAuthChange(MySqlPacket packet)
		{
			packet.ReadByte();
			string method = packet.ReadString();
			byte[] array = new byte[packet.Length - packet.Position];
			Array.Copy(packet.Buffer, packet.Position, array, 0, array.Length);
			MySqlAuthenticationPlugin plugin = MySqlAuthenticationPlugin.GetPlugin(method, this.driver, array);
			plugin.AuthenticationChange();
		}

		private void AuthenticationChange()
		{
			MySqlPacket mySqlPacket = this.driver.Packet;
			mySqlPacket.Clear();
			byte[] array = this.MoreData(null);
			while (array != null && array.Length > 0)
			{
				mySqlPacket.Clear();
				mySqlPacket.Write(array);
				this.driver.SendPacket(mySqlPacket);
				mySqlPacket = this.ReadPacket();
				byte b = mySqlPacket.Buffer[0];
				if (b != 1)
				{
					return;
				}
				byte[] array2 = new byte[mySqlPacket.Length - 1];
				Array.Copy(mySqlPacket.Buffer, 1, array2, 0, array2.Length);
				array = this.MoreData(array2);
			}
			this.ReadPacket();
		}

		public virtual string GetUsername()
		{
			return this.Settings.UserID;
		}

		public virtual object GetPassword()
		{
			return null;
		}
	}
}
