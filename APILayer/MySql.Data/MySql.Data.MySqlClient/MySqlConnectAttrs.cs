using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Reflection;

namespace MySql.Data.MySqlClient
{
	internal class MySqlConnectAttrs
	{
		[DisplayName("_client_name")]
		public string ClientName
		{
			get
			{
				return "MySql Connector/NET";
			}
		}

		[DisplayName("_pid")]
		public string PID
		{
			get
			{
				string result = string.Empty;
				try
				{
					result = Process.GetCurrentProcess().Id.ToString();
				}
				catch (Exception)
				{
				}
				return result;
			}
		}

		[DisplayName("_client_version")]
		public string ClientVersion
		{
			get
			{
				string result = string.Empty;
				try
				{
					result = Assembly.GetAssembly(typeof(MySqlConnectAttrs)).GetName().Version.ToString();
				}
				catch (Exception)
				{
				}
				return result;
			}
		}

		[DisplayName("_platform")]
		public string Platform
		{
			get
			{
				if (!this.Is64BitOS())
				{
					return "x86_32";
				}
				return "x86_64";
			}
		}

		[DisplayName("program_name")]
		public string ProgramName
		{
			get
			{
				string result = Environment.CommandLine;
				try
				{
					string path = Environment.CommandLine.Substring(0, Environment.CommandLine.IndexOf("\" ")).Trim(new char[]
					{
						'"'
					});
					result = Path.GetFileName(path);
					if (Assembly.GetEntryAssembly() != null)
					{
						result = Assembly.GetEntryAssembly().ManifestModule.Name;
					}
				}
				catch (Exception)
				{
					result = string.Empty;
				}
				return result;
			}
		}

		[DisplayName("_os")]
		public string OS
		{
			get
			{
				string text = string.Empty;
				try
				{
					text = Environment.OSVersion.Platform.ToString();
					if (text == "Win32NT")
					{
						text = "Win";
						text += (this.Is64BitOS() ? "64" : "32");
					}
				}
				catch (Exception)
				{
				}
				return text;
			}
		}

		[DisplayName("_os_details")]
		public string OSDetails
		{
			get
			{
				string result = string.Empty;
				try
				{
					ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
					ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
					using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectCollection.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							ManagementBaseObject current = enumerator.Current;
							result = current.GetPropertyValue("Caption").ToString();
						}
					}
				}
				catch (Exception)
				{
				}
				return result;
			}
		}

		[DisplayName("_thread")]
		public string Thread
		{
			get
			{
				string result = string.Empty;
				try
				{
					result = Process.GetCurrentProcess().Threads[0].Id.ToString();
				}
				catch (Exception)
				{
				}
				return result;
			}
		}

		private bool Is64BitOS()
		{
			return Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") == "AMD64";
		}
	}
}
