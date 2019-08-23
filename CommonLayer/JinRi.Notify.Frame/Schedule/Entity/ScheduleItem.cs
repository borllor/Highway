using System;
using System.Data;
using System.Text;
using System.Collections.Generic;

namespace JinRi.App.Framework
{
    [Serializable]
    public class ScheduleItem
    {
        #region 字段

        protected int _scheduleId;
        protected string _title;
        protected bool _enabled;
        protected string _typeFullName;
        protected bool _catchUpEnabled;
        protected int _timeLapse;
        protected string _timeLapseMeasurement;
        protected int _retryTimeLapse;
        protected string _retryTimeLapseMeasurement;
        protected int _retainHistoryNum;
        protected string _attachToEvent;
        protected string _objectDependencies;
        protected string _servers;
        private DateTime _nextStart;
        protected DateTime _lastUpdateTime;
        protected int _status;
        protected DateTime _createTime;
        #endregion

        #region 辅助字段

        private ScheduleSource _scheduleSource;
        private int _threadId;
        private int _processGroup;

        #endregion

        #region 属性

        /// <summary>
        /// 调度执行历史对应的调度任务编号
        /// </summary>
        public int ScheduleId
        {
            get { return _scheduleId; }
            set { _scheduleId = value; }
        }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// 是否启用调度任务
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        /// <summary>
        /// 包括了所有在bin目录下程序集继承于DotNetNuke.Services.Scheduling.SchedulerClient的类，DNN通过发射机制实现
        /// </summary>
        public string TypeFullName
        {
            get { return _typeFullName; }
            set { _typeFullName = value; }
        }

        /// <summary>
        /// 当这个设置为true，如果你的任务是每十分钟调度一次的话，例如在调度期间由于某种原因突然中止了（比方说服务器重起），当再次重启时，之前的所有失效任务都会被再次运行一次。举个例子，若你的调度服务中断了一个小时，则重启时它会执行之前失效的六次。而该设置为false的话则只会执行当前的一次
        /// </summary>
        public bool CatchUpEnabled
        {
            get { return _catchUpEnabled; }
            set { _catchUpEnabled = value; }
        }

        /// <summary>
        /// 调度任务运行的频率，以分钟/小时/天为单位
        /// </summary>
        public int TimeLapse
        {
            get { return _timeLapse; }
            set { _timeLapse = value; }
        }

        /// <summary>
        /// 调度任务运行频率使用的计时单位，以分钟/小时/天为单位
        /// </summary>
        public string TimeLapseMeasurement
        {
            get { return _timeLapseMeasurement; }
            set { _timeLapseMeasurement = value; }
        }

        /// <summary>
        /// 间隔时间，即是当该调度任务失败后，间隔一定时间即重起一次
        /// </summary>
        public int RetryTimeLapse
        {
            get { return _retryTimeLapse; }
            set { _retryTimeLapse = value; }
        }

        /// <summary>
        /// 间隔时间使用的计时单位，以分钟/小时/天为单位
        /// </summary>
        public string RetryTimeLapseMeasurement
        {
            get { return _retryTimeLapseMeasurement; }
            set { _retryTimeLapseMeasurement = value; }
        }

        /// <summary>
        /// 每次任务执行时，数据库都会存储反映任务情况的记录及摘要（如果你看了上面的文章的话，你会发现调度任务执行时都会调用AddLogNode()方法），而这个字段的设置就是保留在数据库里记录的数目
        /// </summary>
        public int RetainHistoryNum
        {
            get { return _retainHistoryNum; }
            set { _retainHistoryNum = value; }
        }

        /// <summary>
        /// 设置调度服务在程序运行时开始启动，目前只有APPLICATION_START这个选项
        /// </summary>
        public string AttachToEvent
        {
            get { return _attachToEvent; }
            set { _attachToEvent = value; }
        }

        /// <summary>
        /// 调度服务是多线程的，故要避免出现死锁问题。所以设置一个依赖对象会防止其他任务跟其有执行冲突。例如，你有一个任务A,其操作关联User表，而同时任务B在更新User表。这时你必须设置它们有同样的依赖对象。(当然你也可设置多个依赖对象，比方说你可设置任务B的依赖对象为”Users,UsersOnline,Portals”。)这样当任务A运行时,任务B是不会运行的，因为它们有依赖冲突。只有当任务A完成了，任务Ｂ才会开始启动
        /// </summary>
        public string ObjectDependencies
        {
            get { return _objectDependencies; }
            set { _objectDependencies = value; }
        }

        /// <summary>
        /// 运行此任务的服务器列表，以逗号分隔
        /// </summary>
        public string Servers
        {
            get { return _servers; }
            set { _servers = value; }
        }

        /// <summary>
        /// 下一次执行调度任务的时间
        /// </summary>
        public DateTime NextStart
        {
            get { return _nextStart; }
            set { _nextStart = value; }
        }

        /// <summary>
        /// 调度任务配置信息最后一次更新时间
        /// </summary>
        public DateTime LastUpdateTime
        {
            get { return _lastUpdateTime; }
            set { _lastUpdateTime = value; }
        }

        #endregion

        #region 辅助属性

        /// <summary>
        /// 运行调度任务的起因来源
        /// </summary>
        public ScheduleSource ScheduleSource
        {
            get { return _scheduleSource; }
            set { _scheduleSource = value; }
        }

        /// <summary>
        /// 运行此轮调度任务的线程编号
        /// </summary>
        public int ThreadId
        {
            get { return _threadId; }
            set { _threadId = value; }
        }

        /// <summary>
        /// 运行此轮调度任务的线程编号
        /// </summary>
        public int ProcessGroup
        {
            get { return _processGroup; }
            set { _processGroup = value; }
        }

        /// <summary>
        /// 当前调度任务状态(1表示删除，2表示正常，4表示待确认)
        /// </summary>
        public int Status
        {
            get { return _status; }
            set { _status = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; }
        }


        #endregion

        #region 构造函数

        public ScheduleItem()
            : base()
        {
            _scheduleId = Null.NullInteger;
            _title = null;
            _enabled = Null.NullBoolean;
            _typeFullName = null;
            _catchUpEnabled = Null.NullBoolean;
            _timeLapse = Null.NullInteger;
            _timeLapseMeasurement = null;
            _retryTimeLapse = Null.NullInteger;
            _retryTimeLapseMeasurement = null;
            _retainHistoryNum = Null.NullInteger;
            _attachToEvent = null;
            _objectDependencies = null;
            _servers = null;
            _nextStart = Null.NullDate;
            _lastUpdateTime = Null.NullDate;

            _scheduleSource = ScheduleSource.NOT_SET;
            _threadId = -1;
            _processGroup = -1;

            _status = Null.NullInteger;
            _createTime = Null.NullDate;
        }

        #endregion

        #region 转换器(DataRow -> BaseObject)

        public static ScheduleItem Convert(DataRow dataRow)
        {
            if (dataRow == null) return null;
            DataRowContainer row = new DataRowContainer(dataRow);
            ScheduleItem obj = new ScheduleItem();

            obj.Id = row.String("ScheduleId");
            //obj.UserId = row.String("UserId");
            //obj.Username = row.String("Username");
            obj.Title = row.String("Title");
            obj.Enabled = row.Bool("Enabled", true);
            obj.TypeFullName = row.String("TypeFullName");
            obj.CatchUpEnabled = row.Bool("CatchUpEnabled", false);
            obj.TimeLapse = row.Int("TimeLapse", 0);
            obj.TimeLapseMeasurement = row.String("TimeLapseMeasurement");
            obj.RetryTimeLapse = row.Int("RetryTimeLapse", 0);
            obj.RetryTimeLapseMeasurement = row.String("RetryTimeLapseMeasurement");
            obj.RetainHistoryNum = row.Int("RetainHistoryNum", 0);
            obj.AttachToEvent = row.String("AttachToEvent");
            obj.ObjectDependencies = row.String("ObjectDependencies");
            obj.Servers = row.String("Servers");
            obj.NextStart = row.DateTime("NextStart");
            obj.LastUpdateTime = row.DateTime("LastUpdateTime");
            obj.Status = row.Int("Status", 2);
            obj.CreateTime = row.DateTime("CreateTime");

            return obj;
        }

        #endregion

        #region 辅助函数

        public bool HasObjectDependencies(string strObjectDependencies)
        {
            if (strObjectDependencies.IndexOf(",") > -1)
            {
                string[] a;
                a = strObjectDependencies.ToLower().Split(',');
                int i;
                for (i = 0; i <= a.Length - 1; i++)
                {
                    if (ObjectDependencies.ToLower().IndexOf(a[i].Trim()) > -1)
                    {
                        return true;
                    }
                }
            }
            else if (ObjectDependencies.ToLower().IndexOf(strObjectDependencies.ToLower()) > -1)
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}
