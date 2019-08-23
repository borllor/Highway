using System;
using System.Data;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace MySql.Data.MySqlClient
{
	[Serializable]
	public sealed class MySqlClientPermission : DBDataPermission
	{
		public MySqlClientPermission(PermissionState permissionState) : base(permissionState)
		{
		}

		private MySqlClientPermission(MySqlClientPermission permission) : base(permission)
		{
		}

		internal MySqlClientPermission(MySqlClientPermissionAttribute permissionAttribute) : base(permissionAttribute)
		{
		}

		internal MySqlClientPermission(DBDataPermission permission) : base(permission)
		{
		}

		internal MySqlClientPermission(string connectionString) : base(PermissionState.None)
		{
			if (connectionString == null || connectionString.Length == 0)
			{
				base.Add(string.Empty, string.Empty, KeyRestrictionBehavior.AllowOnly);
				return;
			}
			base.Add(connectionString, string.Empty, KeyRestrictionBehavior.AllowOnly);
		}

		public override void Add(string connectionString, string restrictions, KeyRestrictionBehavior behavior)
		{
			base.Add(connectionString, restrictions, behavior);
		}

		public override IPermission Copy()
		{
			return new MySqlClientPermission(this);
		}
	}
}
