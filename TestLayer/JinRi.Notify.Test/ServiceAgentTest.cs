using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JinRi.Notify.DB;
using MySql.Data.MySqlClient;
using JinRi.Notify.Business;
using JinRi.Notify.ServiceAgent;
using JinRi.Notify.ServiceModel;
using Newtonsoft.Json;

namespace JinRi.Notify.Test
{
    [TestClass]
    public class ServiceAgentTest
    {
        [TestMethod]
        public void TestSendServiceClient()
        {

            SettingHelper.LogInfo("RedoServiceSetting", "RedoServiceSetting", "JinRi.Notify.ServiceModel.RedoServiceSetting.NewSetting()", "");


            string json = new JinRi.Notify.Business.NotifySettingBusiness().GetNotifySettingValue("BuilderServiceSetting");
            BuilderServiceSetting setting = JsonConvert.DeserializeObject<BuilderServiceSetting>(json);
        }
    }
}
