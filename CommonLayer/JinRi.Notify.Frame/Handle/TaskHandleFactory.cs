using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Text;

namespace JinRi.Notify.Frame
{
    public class TaskHandleFactory
    {
        private static Dictionary<string, ITaskHandle> m_taskHandleDic = new Dictionary<string, ITaskHandle>();
        private static readonly object m_taskHandleObj = new object();

        public static ITaskHandle CreateTaskHandle(ITaskInfo taskInfo)
        {
            ITaskHandle handle = null;
            lock (m_taskHandleObj)
            {
                m_taskHandleDic.TryGetValue(taskInfo.TaskName, out handle);
            }
            if (handle == null)
            {
                string type = TaskHandleManager.GetHandle(taskInfo.TaskName);
                if (!string.IsNullOrEmpty(type))
                {
                    Type t = Type.GetType(type);
                    if (type.IndexOf(",") > 0)
                    {
                        string[] arr = type.Split(new char[] { ',' });
                        ObjectHandle oh = Activator.CreateInstance(arr[1].Trim(), arr[0].Trim());
                        if (oh != null)
                        {
                            handle = oh.Unwrap() as ITaskHandle;
                            lock (m_taskHandleObj)
                            {
                                m_taskHandleDic.Add(taskInfo.TaskName, handle);
                            }
                        }
                    }
                }
            }
            return handle;
        }
    }
}
