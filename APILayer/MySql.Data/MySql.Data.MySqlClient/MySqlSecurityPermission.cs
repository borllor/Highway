using System;
using System.Net;
using System.Security;
using System.Security.Permissions;

namespace MySql.Data.MySqlClient
{
	public sealed class MySqlSecurityPermission : MarshalByRefObject
	{
		private MySqlSecurityPermission()
		{
		}

		public static PermissionSet CreatePermissionSet(bool includeReflectionPermission)
		{
			PermissionSet permissionSet = new PermissionSet(null);
			permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
			permissionSet.AddPermission(new SocketPermission(PermissionState.Unrestricted));
			permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode));
			permissionSet.AddPermission(new DnsPermission(PermissionState.Unrestricted));
			permissionSet.AddPermission(new FileIOPermission(PermissionState.Unrestricted));
			permissionSet.AddPermission(new EnvironmentPermission(PermissionState.Unrestricted));
			if (includeReflectionPermission)
			{
				permissionSet.AddPermission(new ReflectionPermission(PermissionState.Unrestricted));
			}
			return permissionSet;
		}
	}
}
