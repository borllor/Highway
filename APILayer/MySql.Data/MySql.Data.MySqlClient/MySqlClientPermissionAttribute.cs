using System;
using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace MySql.Data.MySqlClient
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public sealed class MySqlClientPermissionAttribute : DBDataPermissionAttribute
	{
		public MySqlClientPermissionAttribute(SecurityAction action) : base(action)
		{
		}

		public override IPermission CreatePermission()
		{
			return new MySqlClientPermission(this);
		}
	}
}
