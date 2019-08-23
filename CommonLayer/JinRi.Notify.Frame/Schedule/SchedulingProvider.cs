#region Copyright
// 
// DotNetNuke?- http://www.dotnetnuke.com
// Copyright (c) 2002-2012
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
#region Usings

using System;
using System.Collections;
using System.Collections.Generic;

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Entities.Host;

using Microsoft.VisualBasic;

#endregion

namespace JinRi.App.Framework
{
    public enum EventName
    {
        //do not add APPLICATION_END
        //it will not reliably complete
        APPLICATION_START
    }

    public enum ScheduleSource
    {
        NOT_SET,
        STARTED_FROM_SCHEDULE_CHANGE,
        STARTED_FROM_EVENT,
        STARTED_FROM_TIMER,
        STARTED_FROM_BEGIN_REQUEST
    }

    public enum ScheduleStatus
    {
        NOT_SET,
        WAITING_FOR_OPEN_THREAD,
        RUNNING_EVENT_SCHEDULE,
        RUNNING_TIMER_SCHEDULE,
        RUNNING_REQUEST_SCHEDULE,
        WAITING_FOR_REQUEST,
        SHUTTING_DOWN,
        STOPPED
    }

    public enum SchedulerMode
    {
        DISABLED = 0,
        TIMER_METHOD = 1,
        REQUEST_METHOD = 2
    }

    //set up our delegates so we can track and react to events of the scheduler clients
    public delegate void WorkStarted(SchedulerClient objSchedulerClient);

    public delegate void WorkProgressing(SchedulerClient objSchedulerClient);

    public delegate void WorkCompleted(SchedulerClient objSchedulerClient);

    public delegate void WorkErrored(SchedulerClient objSchedulerClient, Exception objException);


    public abstract class SchedulingProvider
    {
        public EventName EventName;



        public SchedulingProvider()
        {
            ProviderPath = Settings["providerPath"];
            if (!string.IsNullOrEmpty(Settings["debug"]))
            {
                Debug = Convert.ToBoolean(Settings["debug"]);
            }
            if (!string.IsNullOrEmpty(Settings["maxThreads"]))
            {
                MaxThreads = Convert.ToInt32(Settings["maxThreads"]);
            }
            else
            {
                MaxThreads = 1;
            }
        }


        public static bool Debug { get; private set; }

        public static bool Enabled
        {
            get
            {
                if (SchedulerMode != SchedulerMode.DISABLED)
                {
                    return true;
                }

                return false;
            }
        }

        public static int MaxThreads { get; private set; }

        public string ProviderPath { get; private set; }

        public static bool ReadyForPoll
        {
            get
            {
                if (DataCache.Get("ScheduleLastPolled") == null)
                {
                    return true;
                }
                return false;
            }
        }

        public static DateTime ScheduleLastPolled
        {
            get
            {
                if (DataCache.Get("ScheduleLastPolled") != null)
                {
                    return Convert.ToDateTime(DataCache.Get("ScheduleLastPolled"));
                }
                return DateTime.MinValue;
            }
            set
            {
                DateTime nextStart;

                var nextScheduledTask = Instance().GetNextScheduledTask(ServerController.GetExecutingServerName());
                if (nextScheduledTask != null && nextScheduledTask.NextStart > DateTime.Now)
                {
                    nextStart = nextScheduledTask.NextStart;
                }
                else
                {
                    nextStart = DateTime.Now.AddMinutes(1);
                }

                DataCache.Set("ScheduleLastPolled", value, nextStart);
            }
        }

        public static SchedulerMode SchedulerMode
        {
            get
            {
                return Host.SchedulerMode;
            }
        }


        public virtual Dictionary<string, string> Settings
        {
            get
            {
                return new Dictionary<string, string>();
            }
        }

        public static SchedulingProvider Instance()
        {
            return ComponentFactory.GetComponent<SchedulingProvider>();
        }


        public abstract void Start();

        public abstract void ExecuteTasks();

        public abstract void ReStart(string sourceOfRestart);

        public abstract void StartAndWaitForResponse();

        public abstract void Halt(string sourceOfHalt);

        public abstract void PurgeScheduleHistory();

        public abstract void RunEventSchedule(EventName eventName);

        public abstract ArrayList GetSchedule();

        public abstract ArrayList GetSchedule(string server);

        public abstract ScheduleItem GetSchedule(int scheduleID);

        public abstract ScheduleItem GetSchedule(string typeFullName, string server);

        public abstract ScheduleItem GetNextScheduledTask(string server);

        public abstract ArrayList GetScheduleHistory(int scheduleID);

        public abstract Hashtable GetScheduleItemSettings(int scheduleID);

        public abstract void AddScheduleItemSetting(int scheduleID, string name, string value);

        public abstract Collection GetScheduleQueue();

        public abstract Collection GetScheduleProcessing();

        public abstract int GetFreeThreadCount();

        public abstract int GetActiveThreadCount();

        public abstract int GetMaxThreadCount();

        public abstract ScheduleStatus GetScheduleStatus();

        public abstract int AddSchedule(ScheduleItem scheduleItem);

        public abstract void UpdateSchedule(ScheduleItem scheduleItem);

        public abstract void DeleteSchedule(ScheduleItem scheduleItem);

        public virtual void RunScheduleItemNow(ScheduleItem scheduleItem)
        {
            //Do Nothing
        }

    }
}