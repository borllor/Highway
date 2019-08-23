using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using EasyNetQ;
using EasyNetQ.Loggers;
using EasyNetQ.Topology;
using JinRi.Notify.Frame;
using JinRi.Notify.Business;
using JinRi.Notify.DB;
using JinRi.Notify.DTO;
using JinRi.Notify.Model;
using JinRi.Notify.Utility;
using Newtonsoft.Json;
using JinRi.Notify.ServiceModel;
using JinRi.Notify.Entity.JinRiDB;
using JinRi.Notify.ServiceModel.Condition;
using JinRi.Notify.Business.Core;
using System.Web;
using System.Collections.Specialized;


namespace JinRi.Notify.Test
{
    [TestClass]
    public class TestSender
    {
        //[TestMethod]
        //public void TestRedis()
        //{
        //    RedisCache.Add("lixiaobo", "lixiaobo");
        //    object obj = RedisCache.Get("lixiaobo");
        //    RedisCache.Remove("lixiaobo");
        //}

        [TestMethod]
        public void TestJson()
        {
            var setting = JsonConvert.DeserializeObject<ScanServiceSetting>("{ScanSettingList:[{MessType:'OrderTicketOut',OrderStatus:2,IntervalTime:100,OrderBy:'OutTime',ScanOrderIdInit:86000000,ScanCount:50,Include:'',IdleSleepTime:10000,NextSpan:10},{MessType:'NotifyZBNTicketOut',OrderStatus:7,IntervalTime:90,OrderBy:'Contingent7',ScanOrderIdInit:86000000,ScanCount:50,Include:'',IdleSleepTime:15000,NextSpan:10},{MessType:'OrderReturnResult',OrderStatus:5,IntervalTime:60,OrderBy:'OverTime',ScanOrderIdInit:86000000,ScanCount:50,Include:'',IdleSleepTime:20000,NextSpan:10},{MessType:'OrderPayResult',OrderStatus:1,IntervalTime:60,OrderBy:'PayTime',ScanOrderIdInit:86000000,ScanCount:50,Include:'',IdleSleepTime:9000,NextSpan:10},  {MessType:'OrderApplyReturn',OrderStatus:4,IntervalTime:30,OrderBy:'Rtime',ScanOrderIdInit:86000000,ScanCount:50,Include:'',IdleSleepTime:15000,NextSpan:10},{MessType:'OrderApplyRefund',OrderStatus:3,IntervalTime:20,OrderBy:'Rtime',ScanOrderIdInit:86000000,ScanCount:50,Include:'',IdleSleepTime:13000,NextSpan:10}]}");
        }

        [TestMethod]
        public void TestCallback()
        {
            SendFacade sbus = new SendFacade();
            PushMessageResult result = new PushMessageResult { PushId = "0907396caecb4ce9be68362b07276e64", PushStatus = PushResultEnum.Abort };
            result.ErrMsg = "测试：" + result.PushStatus.ToString();
            sbus.Callback(result);
        }

        [TestMethod]
        public void TestGetDatabaseTime()
        {
            HttpRuntime.Cache.Insert("aa", "aa");
            HttpRuntime.Cache.Insert("aa", "bb");
            object ret = HttpRuntime.Cache.Add("bb", "aa", null,
                System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default,
                null);
            ret = HttpRuntime.Cache.Add("bb", "cc", null,
    System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Default,
    null);



            DateTime dt = JinRiNotifyFacade.Instance.GetDatabaseTime();

        }
        [TestMethod]
        public void TestScanServerSettingCollection()
        {
            List<ScanServiceSetting.ScanSetting> current = ScanServiceSetting.ScanSettingList;
            foreach (ScanServiceSetting.ScanSetting setting in current)
            {
                DateTime stime = new DateTime(2015, 10, 1);
                DateTime etime = new DateTime(2015, 10, 30);
                ScanOrderCondition condition = new ScanOrderCondition();
                condition.OrderBy = setting.OrderBy;
                condition.ScanOrderIdInit = setting.ScanOrderIdInit;
                condition.PageSize = setting.ScanCount;
                condition.Status = setting.OrderStatus;
                condition.StartTime = stime;
                condition.EndTime = etime;
                condition.Includes = string.Join(",", setting.Include.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                List<NotifyOrderEntity> list = JinRiDBFacade.Instance.GetOrdersList(condition);
                foreach (NotifyOrderEntity ent in list)
                {

                }
            }
        }

        [TestMethod]
        public void TestScanServerSettingBuSao()
        {
            List<ScanServiceSetting.ScanSetting> current = ScanServiceSetting.ScanSettingList;
            foreach (ScanServiceSetting.ScanSetting setting in current)
            {
                DateTime stime = new DateTime(2015, 10, 1);
                DateTime etime = new DateTime(2015, 10, 30);
                ScanOrderCondition condition = new ScanOrderCondition();
                condition.OrderBy = setting.OrderBy;
                condition.ScanOrderIdInit = setting.ScanOrderIdInit;
                condition.PageSize = setting.ScanCount;
                condition.Status = setting.OrderStatus;
                condition.StartTime = stime;
                condition.EndTime = etime;
                condition.Includes = string.Join(",", setting.Include.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                List<NotifyOrderEntity> list = JinRiDBFacade.Instance.GetOrdersListBuSao(condition);
                // new ScanMessageBusiness().GetScanMessageList2();
                foreach (NotifyOrderEntity ent in list)
                {

                }
            }
        }

        [TestMethod]
        public void TestScanOrder()
        {
            ScanFacade _scanFacade = new ScanFacade();
            _scanFacade.Scan();
            Thread.Sleep(5000000);
        }

        [TestMethod]
        public void TestInit()
        {
            string url = "http://localhost:13603/MessageHandler/CommonHandler.ashx?msgid=abcdef-teceacd-aaa-bbb&msgkey=123456&msgtype=OrderTicketOut&data=" + HttpUtility.UrlEncode("OrderNo=W2015111004022651226&abc=携程");
            string result2 = url;
        }

        [TestMethod]
        public void TestPostToReceive()
        {
            NotifyMessage message = new NotifyMessage();
            message.AppId = "100201";
            message.MessageKey = "W2015000000000000000";
            message.MessageType = "OrderPayResult";
            message.NotifyData = "orderno=W2015000000000000000&OutTime=2015-12-03 09:40:39";
            message.SourceFrom = "Order.SOA";
            string data = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            for (int i = 0; i < 2; i++)
            {
                System.Net.WebClient client = new System.Net.WebClient();
                client.Encoding = System.Text.Encoding.UTF8;
                string retMsg = client.UploadString("http://192.168.5.149:8060/ReceiveHandler.ashx", data);
            }

            //client.Headers.Add("ContentLength", ("http://192.168.5.149:8060/ReceiveHandler.ashx?data=" + data).Length.ToString());
            //string retMsg = client.DownloadString("http://192.168.5.149:8060/ReceiveHandler.ashx?data=" + data);
        }
    }
}
