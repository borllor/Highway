using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Threading;

namespace JinRi.Notify.Frame
{
    /// <summary>
    /// 描　　述：发送邮件辅助类
    /// 编码人员：Ranen
    /// 编码日期：2012-07-13
    /// </summary>
    public class MailHelper
    {
        public MailHelper()
        {
            m_Host = "smtp.jinri.cn";
            m_Form = "lixiaobo@jinri.cn";
            m_Password = "xbli2013";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sHost">邮件服务器地址</param>
        /// <param name="formMail">发送邮件账户地址</param>
        /// <param name="strPassword">账户密码</param>
        public MailHelper(string sHost, string formMail, string strPassword)
        {
            m_Host = sHost;
            m_Form = formMail;
            m_Password = strPassword;
        }

        #region Private Property
        string m_Host;
        string m_Form;
        /// <summary>
        /// 发送邮件的账户密码
        /// </summary>
        string m_Password;
        MailPriority priority = MailPriority.Normal;
        IList<Attachment> m_Attachments = new List<Attachment>();
        #endregion

        #region Public Property
        /// <summary>
        /// STMP服务器地址
        /// </summary>
        public string Host
        {
            get { return m_Host; }
            set { m_Host = value; }
        }

        /// <summary>
        /// 发送邮件的邮箱地址
        /// </summary>
        public string Form
        {
            get { return m_Form; }
            set { m_Form = value; }
        }

        /// <summary>
        /// 邮件优先级
        /// </summary>
        public MailPriority Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        /// <summary>
        /// 邮件附件
        /// </summary>
        public IList<Attachment> Attachments
        {
            get { return m_Attachments; }
        }
        #endregion

        #region SendMail
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="sendTo">收件人地址</param>
        /// <param name="title">标题</param>
        /// <param name="content">邮件内容</param>
        /// <param name="strCC">抄送人，多个地址用","分隔</param>
        /// <returns>是否成功</returns>
        public bool SendMail(string sendTo, string title, string content, string strCC)
        {
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Host = Host; ;
            smtpClient.Credentials = new System.Net.NetworkCredential(Form, m_Password);

            using (MailMessage mailMessage = new MailMessage(Form, sendTo))
            {
                mailMessage.Subject = title;
                mailMessage.Body = content;
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                mailMessage.IsBodyHtml = true;
                mailMessage.Priority = priority;

                #region CC
                if (!string.IsNullOrEmpty(strCC))
                {
                    string[] mailList = strCC.Split(',');
                    foreach (string str in mailList)
                    {
                        mailMessage.CC.Add(str);
                    }
                }
                #endregion

                #region Attachments
                foreach (Attachment att in this.Attachments)
                {
                    mailMessage.Attachments.Add(att);
                }
                #endregion

                try
                {
                    smtpClient.Send(mailMessage);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">邮件内容</param>
        /// <returns></returns>
        public bool SendMail(string title, string content)
        {
            return SendMail("892065228@qq.com", title, content, "");
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">邮件内容</param>
        /// <param name="strCC">抄送人，多个地址用","分隔</param>
        /// <returns></returns>
        public bool SendMail(string title, string content, string strCC)
        {
            return SendMail("892065228@qq.com", title, content, strCC);
        }
        #endregion

        #region CreateAttachment
        /// <summary>
        /// 创建附件
        /// 注意：如果附件太大，可能需要等待的时候会很长
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <returns></returns>
        public Attachment CreateAttachment(string filePath)
        {
            Attachment attachment = new Attachment(filePath, MediaTypeNames.Application.Octet);
            ContentDisposition disposition = attachment.ContentDisposition;
            disposition.CreationDate = File.GetCreationTime(filePath);
            disposition.ModificationDate = File.GetLastWriteTime(filePath);
            disposition.ReadDate = File.GetLastAccessTime(filePath);
            return attachment;
        }
        #endregion
    }

    public static class MailHelperPackage
    {
        private static Dictionary<string, MailHelper> CurrMailHelperDic { get; set; }
        private static Dictionary<string, DateTime> LastMailTimeDic { get; set; }
        private static Dictionary<string, int> UpperMailCountPerDayDic { get; set; }

        static MailHelperPackage()
        {
            CurrMailHelperDic = new Dictionary<string, MailHelper>();
            LastMailTimeDic = new Dictionary<string, DateTime>();
            UpperMailCountPerDayDic = new Dictionary<string, int>();
        }

        private static readonly object LockObj = new object();
        private static int LastRandomNum = 1000;

        public static void SendMail(string iKey, string title, string content = "", string sendTo = "", string strCC = "", int upperMailCount = 20)
        {
            try
            {
                lock (LockObj)
                {
                    if (!CurrMailHelperDic.ContainsKey(iKey))
                    {
                        CurrMailHelperDic.Add(iKey, new MailHelper());
                    }
                    if (!LastMailTimeDic.ContainsKey(iKey))
                    {
                        LastMailTimeDic.Add(iKey, DateTime.Now);
                    }
                    if (!UpperMailCountPerDayDic.ContainsKey(iKey))
                    {
                        UpperMailCountPerDayDic.Add(iKey, 0);
                    }

                    if (string.IsNullOrEmpty(sendTo))
                    {
                        sendTo = "zhoulin01@jinri.cn";
                        strCC = "funny_zhoulin@163.com";
                    }

                    if (string.IsNullOrEmpty(content))
                    {
                        content = title;
                    }

                    DateTime dtNow = DateTime.Now;
                    if (LastMailTimeDic[iKey].Date != dtNow.Date)
                    {
                        UpperMailCountPerDayDic[iKey] = 0;
                        LastMailTimeDic[iKey] = dtNow;
                    }

                    if (UpperMailCountPerDayDic[iKey] >= upperMailCount)
                    {
                        return;
                    }
                    UpperMailCountPerDayDic[iKey]++;

                    ThreadPool.QueueUserWorkItem(c =>
                    {
                        try
                        {
                            LastRandomNum = LastRandomNum > 30 * 1000 ? new Random().Next(5000, LastRandomNum) : new Random().Next(LastRandomNum, 60 * 1000);
                            Thread.Sleep(LastRandomNum);
                            CurrMailHelperDic[iKey].SendMail(sendTo, title, content, strCC);
                        }
                        catch (Exception)
                        {
                        }
                    });
                }
            }
            catch (Exception)
            {
            }
        }

    }

}
