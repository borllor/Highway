using System;

namespace MySql.Data.MySqlClient.Authentication
{
	public class Sha256AuthenticationPlugin : MySqlAuthenticationPlugin
	{
		private byte[] rawPubkey;

		public override string PluginName
		{
			get
			{
				return "sha256_password";
			}
		}

		protected override byte[] MoreData(byte[] data)
		{
			this.rawPubkey = data;
			return this.GetPassword() as byte[];
		}

		public override object GetPassword()
		{
			if (base.Settings.SslMode != MySqlSslMode.None)
			{
				byte[] bytes = base.Encoding.GetBytes(base.Settings.Password);
				byte[] array = new byte[bytes.Length + 1];
				Array.Copy(bytes, 0, array, 0, bytes.Length);
				array[bytes.Length] = 0;
				return array;
			}
			throw new NotImplementedException("You can use sha256 plugin only in SSL connections in this implementation.");
		}
	}
}
