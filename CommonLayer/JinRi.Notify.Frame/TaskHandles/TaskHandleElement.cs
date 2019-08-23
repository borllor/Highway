using System;
using System.Configuration;
using System.Xml;

namespace JinRi.Framework
{
    /// <summary>
    /// 任务处理句柄节点对象
    /// </summary>
    public class TaskHandleElement : ConfigurationElement
    {
        private static bool _displayIt = false;

        public TaskHandleElement()
        {
        }

        public TaskHandleElement(string task)
        {
            Task = task;
        }

        public TaskHandleElement(string task, string handle,
            int port, string protocal, int performanceValue,
            int creditValue, string serverStatus, int isDistribution)
        {
            Task = task;
            Handle = handle;
        }

        /// <summary>
        /// 服务器唯一标示
        /// </summary>
        [ConfigurationProperty("Task", DefaultValue = "任务类全名", IsRequired = true, IsKey = true)]
        public string Task
        {
            get
            {
                return (string)this["Task"];
            }
            set
            {
                this["Task"] = value;
            }
        }

        /// <summary>
        /// 任务对应的处理句柄类型
        /// </summary>
        [ConfigurationProperty("Handle", DefaultValue = "*.*.*.*", IsRequired = true)]
        public string Handle
        {
            get
            {
                return (string)this["Handle"];
            }
            set
            {
                this["Handle"] = value;
            }
        }

        protected override bool IsModified()
        {
            bool ret = base.IsModified();

            // Enter your custom processing code here.

            Console.WriteLine("UrlConfigElement.IsModified() called.");

            return ret;
        }

        public override string ToString()
        {
            return string.Format("Task:{0},Handle:{1}|\r\n", Task, Handle);
        }
    }
}
