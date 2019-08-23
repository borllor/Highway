using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using JinRi.Notify.DTO;
using JinRi.Notify.Utility;
using JinRi.Notify.ServiceModel.Profile;
using JinRi.Notify.Frame;
using Newtonsoft.Json;

namespace JinRi.Notify.Business
{
    public class SystemHeatService
    {
        private static int m_internalSeconds = 30;
        private static IDataBufferPool m_dataBufferPool = new DataBufferPool(Heat, 2, m_internalSeconds, false);
        private static string ikey = Guid.NewGuid().ToString("N");

        public static void Register()
        {
            ThreadPool.QueueUserWorkItem(x =>
            {
                try
                {
                    BeatMessage beatMessage = new BeatMessage();
                    beatMessage.HostIP = ServerProfile.ServerIP;
                    bool result = InstructionServiceBusiness.RegisterServer(beatMessage);
                    SystemHeatService.AddDataToBuffer();
                    Process.Info(ikey, "注册系统到指令中心", "Register", "", "注册系统到指令中心，返回消息：" + result, "");
                }
                catch (Exception ex)
                {
                    Process.Error(ikey, "注册系统到指令中心", "Register", "", "注册系统到指令中心，发生异常：" + ex.GetString(), "");
                }
            });
        }

        private static void Heat(object dataBuffer)
        {
            try
            {
                BeatMessage beatMessage = new BeatMessage();
                beatMessage.HostIP = ServerProfile.ServerIP;
                BeatResult result = InstructionServiceBusiness.HeartBeat(beatMessage);
                Process.Debug(ikey, "发送心跳到指令中心", "Heat", "", "发送心跳到指令中心，返回消息：" + JsonConvert.SerializeObject(result), "");
            }
            catch (Exception ex)
            {
                Process.Error(ikey, "发送心跳到指令中心", "Heat", "", "发送心跳到指令中心，发生异常：" + ex.GetString(), "");
            }
            finally
            {
                SystemHeatService.AddDataToBuffer();
            }
        }

        private static void AddDataToBuffer()
        {
            m_dataBufferPool.Write((byte)1);
        }
    }
}
