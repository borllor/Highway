using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Remoting.Messaging;

using JinRi.Notify.Utility;
using JinRi.Notify.Frame;

namespace JinRi.Notify.ServiceModel.Profile
{
    [Serializable]
    public class ServerProfile : ILogicalThreadAffinative
    {
        private static ServerProfile _current;
        private static readonly object SyncObj = new object();
        private string _serverIP { get; set; }
        private string _serverPort { get; set; }
        private string _machineName { get; set; }
        private string _absoluteUri { get; set; }
        private string _physicalPath { get; set; }
        private string _OSInfo { get; set; }
        private string _netInfo { get; set; }
        private string _IISInfo { get; set; }
        private bool _is64BitOperatingSystem { get; set; }
        private bool _is64BitProcess { get; set; }

        public static string ServerIP
        {
            get
            {
                return Current._serverIP;
            }
            set
            {
                Current._serverIP = value;
            }
        }

        public static string ServerPort
        {
            get
            {
                return Current._serverPort;
            }
            set
            {
                Current._serverPort = value;
            }
        }

        public static string MachineName
        {
            get
            {
                return Current._machineName;
            }
            set
            {
                Current._machineName = value;
            }
        }

        public static string AbsoluteUri
        {
            get
            {
                return Current._absoluteUri;
            }
            set
            {
                Current._absoluteUri = value;
            }
        }

        public static string PhysicalPath
        {
            get
            {
                return Current._physicalPath;
            }
            set
            {
                Current._physicalPath = value;
            }
        }

        public static string OSInfo
        {
            get
            {
                return Current._OSInfo;
            }
            set
            {
                Current._OSInfo = value;
            }
        }

        public static string NetInfo
        {
            get
            {
                return Current._netInfo;
            }
            set
            {
                Current._netInfo = value;
            }
        }

        public static string IISInfo
        {
            get
            {
                return Current._IISInfo;
            }
            set
            {
                Current._IISInfo = value;
            }
        }

        public static bool Is64BitOperatingSystem
        {
            get
            {
                return Current._is64BitOperatingSystem;
            }
            set
            {
                Current._is64BitOperatingSystem = value;
            }
        }

        public static bool Is64BitProcess
        {
            get
            {
                return Current._is64BitProcess;
            }
            set
            {
                Current._is64BitProcess = value;
            }
        }

        private static ServerProfile Current
        {
            get
            {
                if (_current == null)
                {
                    lock (SyncObj)
                    {
                        if (_current == null)
                        {
                            ServerProfile tmp = new ServerProfile();
                            InitProfile(tmp);
                            _current = tmp;
                        }
                    }
                }
                return _current;
            }
        }

        private static void InitProfile(ServerProfile profile)
        {
            profile._serverIP = IPHelper.GetLocalIP();
            profile._machineName = Environment.MachineName;
            profile._OSInfo = Environment.OSVersion.ToString();
            profile._is64BitOperatingSystem = Environment.Is64BitOperatingSystem;
            profile._is64BitProcess = Environment.Is64BitProcess;
            profile._netInfo = ".NET CLR" + Environment.Version.Major + "." + Environment.Version.Minor + "." + Environment.Version.Build + "." + Environment.Version.Revision;

            if (HttpContext.Current != null)
            {
                profile._serverPort = HttpContext.Current.Request.ServerVariables["SERVER_PORT"];
                profile._absoluteUri = HttpContext.Current.Request.Url.AbsoluteUri;
                profile._physicalPath = HttpContext.Current.Request.ApplicationPath;
                profile._IISInfo = HttpContext.Current.Request.ServerVariables["SERVER_SOFTWARE"];
            }
            else
            {
                profile._serverPort = "";
                profile._absoluteUri = "";
                profile._physicalPath = "";
                profile._IISInfo = "";
            }
        }
    }
}
