using Microsoft.Win32;
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Xml;

namespace MySql.Data.MySqlClient
{
	[RunInstaller(true)]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust"), PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public class CustomInstaller : Installer
	{
		public override void Install(IDictionary stateSaver)
		{
			base.Install(stateSaver);
		}

		private static void AddProviderToMachineConfig()
		{
			object value = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\.NETFramework\\", "InstallRoot", null);
			if (value == null)
			{
				throw new Exception("Unable to retrieve install root for .NET framework");
			}
			CustomInstaller.UpdateMachineConfigs(value.ToString(), true);
			string text = value.ToString();
			text = text.Substring(0, text.Length - 1);
			text = string.Format("{0}64{1}", text, Path.DirectorySeparatorChar);
			if (Directory.Exists(text))
			{
				CustomInstaller.UpdateMachineConfigs(text, true);
			}
		}

		private static void UpdateMachineConfigs(string rootPath, bool add)
		{
			string[] array = new string[]
			{
				"v2.0.50727",
				"v4.0.30319"
			};
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string str = array2[i];
				string arg = rootPath + str;
				string path = string.Format("{0}\\CONFIG", arg);
				if (Directory.Exists(path))
				{
					if (add)
					{
						CustomInstaller.AddProviderToMachineConfigInDir(path);
					}
					else
					{
						CustomInstaller.RemoveProviderFromMachineConfigInDir(path);
					}
				}
			}
		}

		private static void AddProviderToMachineConfigInDir(string path)
		{
			string text = string.Format("{0}\\machine.config", path);
			if (!File.Exists(text))
			{
				return;
			}
			StreamReader streamReader = new StreamReader(text);
			string xml = streamReader.ReadToEnd();
			streamReader.Close();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			XmlElement xmlElement = (XmlElement)xmlDocument.CreateNode(XmlNodeType.Element, "add", "");
			xmlElement.SetAttribute("name", "MySQL Data Provider");
			xmlElement.SetAttribute("invariant", "MySql.Data.MySqlClient");
			xmlElement.SetAttribute("description", ".Net Framework Data Provider for MySQL");
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string value = string.Format("MySql.Data.MySqlClient.MySqlClientFactory, {0}", executingAssembly.FullName.Replace("Installers", "Data"));
			xmlElement.SetAttribute("type", value);
			XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("DbProviderFactories");
			foreach (XmlNode xmlNode in elementsByTagName[0].ChildNodes)
			{
				if (xmlNode.Attributes != null)
				{
					foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
					{
						if (xmlAttribute.Name == "invariant" && xmlAttribute.Value == "MySql.Data.MySqlClient")
						{
							elementsByTagName[0].RemoveChild(xmlNode);
							break;
						}
					}
				}
			}
			elementsByTagName[0].AppendChild(xmlElement);
			XmlTextWriter xmlTextWriter = new XmlTextWriter(text, null);
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlDocument.Save(xmlTextWriter);
			xmlTextWriter.Flush();
			xmlTextWriter.Close();
		}

		public override void Uninstall(IDictionary savedState)
		{
			base.Uninstall(savedState);
		}

		private static void RemoveProviderFromMachineConfig()
		{
			object value = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\.NETFramework\\", "InstallRoot", null);
			if (value == null)
			{
				throw new Exception("Unable to retrieve install root for .NET framework");
			}
			CustomInstaller.UpdateMachineConfigs(value.ToString(), false);
			string text = value.ToString();
			text = text.Substring(0, text.Length - 1);
			text = string.Format("{0}64{1}", text, Path.DirectorySeparatorChar);
			if (Directory.Exists(text))
			{
				CustomInstaller.UpdateMachineConfigs(text, false);
			}
		}

		private static void RemoveProviderFromMachineConfigInDir(string path)
		{
			string text = string.Format("{0}\\machine.config", path);
			if (!File.Exists(text))
			{
				return;
			}
			StreamReader streamReader = new StreamReader(text);
			string xml = streamReader.ReadToEnd();
			streamReader.Close();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("DbProviderFactories");
			foreach (XmlNode xmlNode in elementsByTagName[0].ChildNodes)
			{
				if (xmlNode.Attributes != null)
				{
					string value = xmlNode.Attributes["name"].Value;
					if (value == "MySQL Data Provider")
					{
						elementsByTagName[0].RemoveChild(xmlNode);
						break;
					}
				}
			}
			XmlTextWriter xmlTextWriter = new XmlTextWriter(text, null);
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlDocument.Save(xmlTextWriter);
			xmlTextWriter.Flush();
			xmlTextWriter.Close();
		}
	}
}
