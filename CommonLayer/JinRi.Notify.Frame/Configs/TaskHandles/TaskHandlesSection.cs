using System;
using System.Configuration;
using System.Xml;

namespace JinRi.Notify.Frame
{
    /// <summary>
    /// 任务处理句柄节点配置域
    /// </summary>
    public class TaskHandlesSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public TaskHandleCollection Handles
        {
            get
            {
                TaskHandleCollection col = (TaskHandleCollection)base[""];
                return col;
            }
        }
    }
}
