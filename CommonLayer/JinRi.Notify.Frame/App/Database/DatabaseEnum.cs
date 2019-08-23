namespace JinRi.Notify.Frame
{
    public enum DatabaseEnum
    {
        /// <summary>
        /// jinri库_读
        /// </summary>
        JinRiDB_SELECT,

        /// <summary>
        /// jinri库_写
        /// </summary>
        JinRiDB_CMD,

        JinRiBack_SELECT,

        /// <summary>
        /// jinri2库_读
        /// </summary>
        JinRi2DB_SELECT,

        /// <summary>
        /// jinri2库_写
        /// </summary>
        JinRi2DB_CMD,

        /// <summary>
        /// 政策查询库
        /// </summary>
        JinRiRateDB_SELECT,

        /// <summary>
        /// 政策编辑库
        /// </summary>
        JinRiRateDB_CMD,

        /// <summary>
        /// 特惠、包机政策查询库
        /// </summary>
        JinRiSpecialRateDB_SELECT,

        /// <summary>
        /// 特惠、包机政策查询库
        /// </summary>
        JinRiSpecialRateDB_CMD,

        /// <summary>
        /// 联程政策库
        /// </summary>
        JinRiUnionRateDB_SELECT,

        /// <summary>
        /// Log4Net_读
        /// </summary>
        Log4Net_SELECT,

        /// <summary>
        /// Log4Net_写
        /// </summary>
        Log4Net_CMD,

        /// <summary>
        /// PGLog4Net_读
        /// </summary>
        PGLog4Net_SELECT,

        /// <summary>
        /// Log4Net_写
        /// </summary>
        PGLog4Net_CMD,

        /// <summary>
        /// 价格库_读
        /// </summary>
        FlightPrice_SELECT,
        /// <summary>
        /// 分销接口系统执行CMD命令库
        /// </summary>
        DistributionInterfaceDB_CMD,

        /// <summary>
        /// 分销接口系统执行查询命令库
        /// </summary>
        DistributionInterfaceDB_SELECT,

        /// <summary>
        /// 今日DataCenter库查询
        /// </summary>
        JRDataCenter_SELECT,
        /// <summary>
        /// 今日DataCenter库CMD
        /// </summary>
        JRDataCenter_CMD,

        /// <summary>
        /// 今日政策日志库
        /// </summary>
        JinRiLogger_SELECT,

        /// <summary>
        /// 今日政策日志库-写
        /// </summary>
        JinRiLogger_CMD,

        /// <summary>
        /// JinRiPush库CMD
        /// </summary>
        JinRiPushDB_CMD,

        /// <summary>
        /// JinRiPush库查询
        /// </summary>
        JinRiPushDB_SELECT,


        /// <summary>
        /// 接口数据库_读
        /// </summary>
        JinRiInterface_SELECT,

        /// <summary>
        /// 公共库_读
        /// </summary>
        FltCommDB_SELECT,

        /// <summary>
        /// 公共库_写
        /// </summary>
        FltCommDB_CMD,

        /// <summary>
        /// 高返点限制库读取
        /// </summary>
        FltProductDB_SELECT,

        /// <summary>
        /// 高返点限制库读取
        /// </summary>
        FltProductDB_CMD,

        /// <summary>
        /// JinRiInterface库_读
        /// </summary>
        JinRiInterfaceDB_SELECT,

        /// <summary>
        /// JinRiInterface库_写
        /// </summary>
        JinRiInterfaceDB_CMD,

        /// <summary>
        /// JinriOrderDB库_读
        /// </summary>
        JinriOrderDB_SELECT,

        /// <summary>
        /// JinriOrderDB库_写
        /// </summary>
        JinriOrderDB_CMD,

        /// <summary>
        ///FltOrderDBB库_读
        /// </summary>
        FltOrderDB_SELECT,

        /// <summary>
        ///FltOrderDBB库_写
        /// </summary>
        FltOrderDB_CMD,
        /// <summary>
        /// jinri3库_读
        /// </summary>
        JinRi3DB_SELECT,

        /// <summary>
        /// JinRiAirV2_读
        /// </summary>
        JinRiAirV2_SELECT,

        /// <summary>
        /// JinRiAirV2_写
        /// </summary>
        JinRiAirV2_CMD,
        /// <summary>
        /// jinri3库_写
        /// </summary>
        JinRi3DB_CMD,

        /// <summary>
        /// 保险库
        /// </summary>
        JinRiIns_SELECT,

        /// <summary>
        /// 保险库
        /// </summary>
        JinRiIns_CMD,
        /// <summary>
        /// FltUserDB读库
        /// </summary>
        FltUserDB_SELECT,
        /// <summary>
        /// JinRiYLT写库
        /// </summary>
        JinRiNotify_CMD,
        /// <summary>
        /// JinRiYLT读库
        /// </summary>
        JinRiNotify_SELECT,
        /// <summary>
        /// JinRiYLT写库
        /// </summary>
        JinRiYLT_CMD,
        /// <summary>
        /// JinRiYLT读库
        /// </summary>
        JinRiYLT_SELECT,

        /// <summary>
        /// JinRiYLT读库
        /// </summary>
        FltAccountDB_SELECT,
        /// <summary>
        /// JinRiYLT写库
        /// </summary>
        FltAccountDB_CMD,

        JinRiMonitor_SELECT,

        FxDB_SELECT,
        FxDB_CMD,

        /// <summary>
        /// 8000yi读库
        /// </summary>
        ThirdBQYRateDB_SELECT,
        /// <summary>
        /// 8000yi写库
        /// </summary>
        ThirdBQYRateDB_CMD,
    }
}
