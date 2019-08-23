using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;

using JinRi.Notify.Utility;
using JinRi.Notify.Frame;

namespace JinRi.Notify.ServiceModel.Profile
{
    [Serializable]
    public class ClientProfile : ILogicalThreadAffinative
    {
        public static readonly string ContextKey = "JinRi.Notify.ServiceModel.ClientProfile";
        private static readonly object SyncObj = new object();
        private string _clientIP;
        private string _hostname;
        private string _browserInfo;
        private string _OSInfo;

        public static string ClientIP
        {
            get
            {
                return Current._clientIP;
            }
            set
            {
                Current._clientIP = value;
            }
        }
        public static string Hostname
        {
            get
            {
                return Current._hostname;
            }
            set
            {
                Current._hostname = value;
            }
        }
        public static string BrowserInfo
        {
            get
            {
                return Current._browserInfo;
            }
            set
            {
                Current._browserInfo = value;
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

        private static ClientProfile Current
        {
            get
            {
                ClientProfile tmpObj = CallContext.GetData(ContextKey) as ClientProfile;
                if (null == tmpObj)
                {
                    lock (SyncObj)
                    {
                        tmpObj = CallContext.GetData(ContextKey) as ClientProfile;
                        if (null == tmpObj)
                        {
                            tmpObj = new ClientProfile();
                            tmpObj.InitProfile(tmpObj);
                            CallContext.LogicalSetData(ContextKey, tmpObj);
                        }
                    }
                }
                return tmpObj;
            }
        }

        private void InitProfile(ClientProfile profile)
        {
            if (HttpContext.Current != null)
            {
                profile._clientIP = ClientHelper.GetClientIP();
                profile._hostname = ClientHelper.GetClientHost();
                profile._browserInfo = ClientHelper.GetBrowser();
                profile._OSInfo = ClientHelper.GetCilentOSByUserAgent();
            }
            else
            {
                profile._clientIP = "";
                profile._hostname = "";
                profile._browserInfo = "";
                profile._OSInfo = "";
            }
        }
    }
}
