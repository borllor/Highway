using System;
using System.Data;
using System.Text;
using System.Collections.Generic;

namespace JinRi.App.Framework
{
    [Serializable]
    public class ScheduleHistoryItem : ScheduleItem
    {
        #region 字段

        private int _scheduleHistoryId;
        private string _server;
        private DateTime _startTime;
        private DateTime _endTime;
        private bool _succeed;
        private string _logNotes;

        #endregion

        #region 属性

        public int ScheduleHistoryId
        {
            get
            {
                return _scheduleHistoryId;
            }
            set
            {
                _scheduleHistoryId = value;
            }
        }

        /// <summary>
        /// 此次调度执行所在的服务器名称
        /// </summary>
        public string Server
        {
            get { return _server; }
            set { _server = value; }
        }

        /// <summary>
        /// 此次调度任务开始执行的时间
        /// </summary>
        public DateTime StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }

        /// <summary>
        /// 此次调度任务结束运行的时间
        /// </summary>
        public DateTime EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }

        /// <summary>
        /// 表示此次调度执行是否成功
        /// </summary>
        public bool Succeed
        {
            get { return _succeed; }
            set { _succeed = value; }
        }

        /// <summary>
        /// 执行日志
        /// </summary>
        public string LogNotes
        {
            get { return _logNotes; }
            set { _logNotes = value; }
        }

        #endregion

        #region 辅助属性

        /// <summary>
        /// 调度任务此轮或上一轮执行的秒数
        /// </summary>
        public double ElapsedTime
        {
            get
            {
                try
                {
                    if (EndTime == Null.NullDate && StartTime != Null.NullDate)
                    {
                        return DateTime.Now.Subtract(StartTime).TotalSeconds;
                    }
                    else if (StartTime != null)
                    {
                        return EndTime.Subtract(StartTime).TotalSeconds;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 调度任务此轮运行是否结束
        /// </summary>
        public bool Overdue
        {
            get
            {
                if (NextStart < DateTime.Now && EndTime == Null.NullDate)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 调度任务此轮已经运行秒数（如果此轮结束则返回0）
        /// </summary>
        public double OverdueBy
        {
            get
            {
                try
                {
                    if (NextStart <= DateTime.Now && EndTime == Null.NullDate)
                    {
                        return Math.Round(DateTime.Now.Subtract(NextStart).TotalSeconds);
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 调度任务还有多少秒会自动执行下一轮
        /// </summary>
        public double RemainingTime
        {
            get
            {
                try
                {
                    if (NextStart > DateTime.Now && EndTime == Null.NullDate)
                    {
                        return Math.Round(NextStart.Subtract(DateTime.Now).TotalSeconds);
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch
                {
                    return 0;
                }
            }
        }

        #endregion

        #region 构造函数

        public ScheduleHistoryItem()
            : base()
        {
            _scheduleId = Null.NullInteger;
            _server = null;
            _startTime = Null.NullDate;
            _endTime = Null.NullDate;
            _succeed = Null.NullBoolean;
            _logNotes = null;
        }

        public ScheduleHistoryItem(Schedule sduObj)
            : this()
        {
            this.ScheduleId = sduObj.ScheduleId;
            //this.UserId = sduObj.UserId;
            //this.Username = sduObj.Username;
            this.Title = sduObj.Title;
            this.Enabled = sduObj.Enabled;
            this.TypeFullName = sduObj.TypeFullName;
            this.CatchUpEnabled = sduObj.CatchUpEnabled;
            this.TimeLapse = sduObj.TimeLapse;
            this.TimeLapseMeasurement = sduObj.TimeLapseMeasurement;
            this.RetryTimeLapse = sduObj.RetryTimeLapse;
            this.RetryTimeLapseMeasurement = sduObj.RetryTimeLapseMeasurement;
            this.RetainHistoryNum = sduObj.RetainHistoryNum;
            this.AttachToEvent = sduObj.AttachToEvent;
            this.ObjectDependencies = sduObj.ObjectDependencies;
            this.Servers = sduObj.Servers;
            this.LastUpdateTime = sduObj.LastUpdateTime;
        }

        #endregion

        #region 辅助方法

        public void AddLogNote(string notes)
        {
            _logNotes = string.Format("{0}{1}<br />", _logNotes, notes);
        }

        #endregion

        #region 转换器(DataRow -> BaseObject)

        new public static ScheduleHistoryItem Convert(DataRow dataRow)
        {
            if (dataRow == null) return null;
            DataRowContainer row = new DataRowContainer(dataRow);
            ScheduleHistoryItem obj = new ScheduleHistoryItem();

            obj.ScheduleHistoryId = row.Int("ScheduleHistoryId");
            obj.ScheduleId = row.Int("ScheduleId");
            obj.Server = row.String("Server");
            obj.StartTime = row.DateTime("StartTime");
            obj.EndTime = row.DateTime("EndTime");
            obj.Succeed = row.Bool("Succeed", false);
            obj.NextStart = row.DateTime("NextStart");
            obj.LogNotes = row.String("LogNotes");
            obj.Status = row.Int("Status", 2);
            obj.CreateTime = row.DateTime("CreateTime");

            return obj;
        }

        #endregion
    }
}
