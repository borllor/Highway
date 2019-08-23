using System;
using System.IO;

namespace MySql.Data.Common
{
	internal class Platform
	{
		private static bool inited;

		private static bool isMono;

		public static char DirectorySeparatorChar
		{
			get
			{
				return Path.DirectorySeparatorChar;
			}
		}

		private Platform()
		{
		}

		public static bool IsWindows()
		{
			OperatingSystem oSVersion = Environment.OSVersion;
			switch (oSVersion.Platform)
			{
			case PlatformID.Win32S:
			case PlatformID.Win32Windows:
			case PlatformID.Win32NT:
				return true;
			default:
				return false;
			}
		}

		public static bool IsMono()
		{
			if (!Platform.inited)
			{
				Platform.Init();
			}
			return Platform.isMono;
		}

		private static void Init()
		{
			Platform.inited = true;
			Type type = Type.GetType("Mono.Runtime");
			Platform.isMono = (type != null);
		}
	}
}
