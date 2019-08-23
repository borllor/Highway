using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Messaging;

using JinRi.Notify.Utility;
using JinRi.Notify.Frame;

namespace JinRi.Notify.ServiceModel.Profile
{
    [Serializable]
    public class RequestProfile : ILogicalThreadAffinative
    {
        public static readonly string ContextKey = "JinRi.Notify.ServiceModel.RequestProfile";
        private static readonly object SyncObj = new object();
        private string _clientIP;
        private string _username;
        private string _messageKey;
        private string _requestKey;
        private string _requestType;

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
        public static string Username
        {
            get
            {
                return Current._username;
            }
            set
            {
                Current._username = value;
            }
        }
        public static string MessageKey
        {
            get
            {
                return Current._messageKey;
            }
            set
            {
                Current._messageKey = value;
            }
        }
        public static string RequestKey
        {
            get
            {
                return Current._requestKey;
            }
            set
            {
                Current._requestKey = value;
            }
        }
        public static string RequestType
        {
            get
            {
                return Current._requestType;
            }
            set
            {
                Current._requestType = value;
            }
        }

        private static RequestProfile Current
        {
            get
            {
                RequestProfile tmpObj = CallContext.GetData(ContextKey) as RequestProfile;
                if (null == tmpObj)
                {
                    lock (SyncObj)
                    {
                        tmpObj = CallContext.GetData(ContextKey) as RequestProfile;
                        if (null == tmpObj)
                        {
                            tmpObj = new RequestProfile();
                            tmpObj.InitProfile(tmpObj);
                            CallContext.LogicalSetData(ContextKey, tmpObj);
                        }
                    }
                }
                return tmpObj;
            }
        }

        private void InitProfile(RequestProfile profile)
        {
            profile._username = ConfigurationAppSetting.AppId;
            if (HttpContext.Current != null)
            {
                profile._clientIP = ClientProfile.ClientIP;
            }
            else
            {
                profile._clientIP = "";
            }
        }
    }
}
