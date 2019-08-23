using System;
using System.IO;
using System.Net;
using System.Text;

using JinRi.Notify.Frame;

namespace JinRi.Notify.Utility.Helper
{
    public class HttpAsyncHelper
    {
        private static readonly ILog m_logger = LoggerSource.Instance.GetLogger(typeof(HttpAsyncHelper));
        public event EventHandler<DataResponseEventArgs> OnRequestDataComplected;       
        private PushEntity pushEntity;

        public HttpAsyncHelper(PushEntity tEntity)
        {
            pushEntity = tEntity;
        }

        protected void OnDataResponse(DataResponseEventArgs e)
        {
            if (OnRequestDataComplected != null)
            {
                OnRequestDataComplected(this, e);
            }
        }
        protected void ResponseCallback(IAsyncResult ar)
        {
            RequestState st = ar.AsyncState as RequestState;
            HttpWebResponse response = null;
            string retXml = "";
            try
            {
                response = st.WebRequest.EndGetResponse(ar) as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    response.Close();
                    st.PushEntity.RequestEndDate = DateTime.Now;
                    this.OnDataResponse(new DataResponseEventArgs("连接错误", st.PushEntity, response.StatusCode));
                    return;
                }
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), st.PushEntity.Encode))
                {
                    retXml = sr.ReadToEnd();
                    st.PushEntity.RequestEndDate = DateTime.Now;
                    this.OnDataResponse(new DataResponseEventArgs(retXml, st.PushEntity));
                }
            }
            //catch (NullReferenceException nullEx)
            //{
            //    st.PushEntity.RequestEndDate = DateTime.Now;
            //    if (!string.IsNullOrWhiteSpace(retXml))
            //    {
            //        st.PushEntity.RecievedData = retXml;
            //    }
            //    this.OnDataResponse(new DataResponseEventArgs("异常信息:" + nullEx.GetString(), st.PushEntity, nullEx));
            //}
            catch (Exception ex)
            {
                st.PushEntity.RequestEndDate = DateTime.Now;
                this.OnDataResponse(new DataResponseEventArgs("异常信息:" + ex.GetString(), st.PushEntity, ex));
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }
        protected void RequestCallback(IAsyncResult ar)
        {
            RequestState rs = ar.AsyncState as RequestState;
            try
            {
                using (Stream postStream = rs.WebRequest.EndGetRequestStream(ar))
                {
                    int writeBuffer = 2048;
                    int offset = 0;
                    byte[] dataBuffer = rs.PushEntity.Encode.GetBytes(rs.PushEntity.Data);
                    do
                    {
                        if ((offset + writeBuffer) >= dataBuffer.Length) writeBuffer = dataBuffer.Length - offset;
                        postStream.Write(dataBuffer, offset, writeBuffer);
                        offset = offset + writeBuffer;
                    } while (offset < dataBuffer.Length);
                    rs.WebRequest.BeginGetResponse(new AsyncCallback(ResponseCallback), rs);
                }
            }
            catch (Exception ex)
            {
                rs.PushEntity.RequestEndDate = DateTime.Now;
                OnDataResponse(new DataResponseEventArgs(ex.Message, rs.PushEntity, ex));
            }
        }
        public void Request(MethodType reqType)
        {
            string requestURL = pushEntity.Url;
            string requestData = pushEntity.Data;
            string url = requestURL;
            pushEntity.RequestStartDate = DateTime.Now;
            if (reqType == MethodType.GET)
            {
                url = requestURL + "?" + requestData;
            }
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = reqType.ToString();
            req.Proxy = null;
            if (pushEntity.TimeOut > 0)
            {
                req.Timeout = pushEntity.TimeOut;
            }
            RequestState rs = new RequestState() { PushEntity = pushEntity };
            if (reqType == MethodType.POST)
            {
                req.ContentType = "application/x-www-form-urlencoded";
                rs.WebRequest = req;
                req.BeginGetRequestStream(new AsyncCallback(RequestCallback), rs);
            }
            else
            {
                rs.WebRequest = req;
                req.BeginGetResponse(new System.AsyncCallback(ResponseCallback), rs);
            }
        }
    }

    public enum MethodType
    {
        POST,
        GET
    }

    internal class RequestState
    {
        public HttpWebRequest WebRequest;
        public PushEntity PushEntity { get; set; }
        public RequestState()
        {
        }
    }

    public class DataResponseEventArgs : System.EventArgs
    {
        private string _recievedData = string.Empty;
        private PushEntity _pushEntity = null;
        private Exception _exception = null;
        private HttpStatusCode _httpStatusCode;

        public HttpStatusCode HttpStatusCode
        {
            get
            {
                return _httpStatusCode;
            }
        }

        public string RecievedData
        {
            get
            {
                return _recievedData;
            }
        }

        public PushEntity PushEntity
        {
            get
            {
                return _pushEntity;
            }
        }

        public Exception Exception
        {
            get
            {
                return _exception;
            }
        }

        public DataResponseEventArgs(string recievedData, PushEntity tempEntity, Exception ex)
        {
            _recievedData = recievedData;
            _pushEntity = tempEntity;
            _exception = ex;
        }

        public DataResponseEventArgs(string recievedData, PushEntity tempEntity)
            : this(recievedData, tempEntity, HttpStatusCode.OK)
        {
        }

        public DataResponseEventArgs(string recievedData, PushEntity tempEntity, HttpStatusCode httpStatusCode)
        {
            _recievedData = recievedData;
            _pushEntity = tempEntity;
            _httpStatusCode = httpStatusCode;
        }
    }

    public class PushEntity
    {
        public string PushId { get; set; }
        public string Url { get; set; }
        public string Data { get; set; }
        public string RecievedData { get; set; }
        public int TimeOut { get; set; }
        public object ReferObject { get; set; }
        public Action<object> Callback { get; set; }
        public Encoding Encode { get; set; }
        public DateTime RequestStartDate { get; set; }
        public DateTime RequestEndDate { get; set; }

        public override string ToString()
        {
            return string.Format("PushId:{0},Url:{1},Data:{2},TimeOut:{3},Callback:{4},Encode:{5},RequestStartDate:{6},RequestEndDate:{7}",
                PushId, Url, Data, TimeOut, Callback.Method.Name, Encode, RequestStartDate, RequestEndDate);
        }
    }
}


