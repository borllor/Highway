-- MySQL dump 10.13  Distrib 8.0.17, for Win64 (x86_64)
--
-- Host: 192.168.1.180    Database: jinrinotify
-- ------------------------------------------------------
-- Server version	5.6.45

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `notifysetting`
--

DROP TABLE IF EXISTS `notifysetting`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `notifysetting` (
  `notifyid` int(11) NOT NULL AUTO_INCREMENT,
  `createtime` datetime NOT NULL,
  `lastmodifytime` datetime NOT NULL,
  `settingkey` varchar(200) NOT NULL DEFAULT '',
  `remark` varchar(500) NOT NULL DEFAULT '',
  `classname` varchar(500) NOT NULL DEFAULT '',
  `settingvalue` varchar(8000) NOT NULL DEFAULT '',
  `memo` varchar(8000) NOT NULL DEFAULT '',
  PRIMARY KEY (`notifyid`),
  KEY `ind_notifysetting_settingkey` (`settingkey`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `notifysetting`
--

LOCK TABLES `notifysetting` WRITE;
/*!40000 ALTER TABLE `notifysetting` DISABLE KEYS */;
INSERT INTO `notifysetting` VALUES (1,'2015-10-29 20:16:14','2015-11-03 11:27:17','BuilderServiceSetting','服务配置','','{\"IsOpenBatchSaveNotifyMessage\":\"true\",\"AutoFlushNotifyMessage\":\"4\",\"IsOpenBatchSavePushMessage\":\"false\",\"AutoFlushPushMessage\":\"1\",\"IsOpenBatchSendPushMessage\":\"false\",\"AutoFlushSendMessage\":\"1\",\"IsOpenBatchReceiveHighMessage\":\"false\",\"AutoFlushReceiveHighMessage\":\"1\",\"IsOpenBatchReceiveMiddleMessage\":\"true\",\"AutoFlushReceiveMiddleMessage\":\"2\",\"IsOpenBatchReceiveNormalMessage\":\"true\",\"AutoFlushReceiveNormalMessage\":\"3\",\"IsOpenBatchReceiveLowMessage\":\"true\",\"AutoFlushReceiveLowMessage\":\"5\",\"PushAheadTime\":\"1\",\"ParallelSubscribeSettingList\":[{\"MessagePriority\":\"High\"},{\"MessagePriority\":\"Middle\"},{\"MessagePriority\":\"Normal\"},{\"MessagePriority\":\"Low\"}]}','新增，操作人：jinri，操作时间: 2015-10-29 20:15:59，操作人IP：192.168.6.125|编辑，操作人：jinri，操作时间: 2015-10-30 09:50:25，操作人IP：192.168.6.125|编辑，操作人：jinri，操作时间: 2015-10-30 09:50:38，操作人IP：192.168.6.125|编辑，操作人：jinri，操作时间: 2015-10-30 09:54:07，操作人IP：192.168.6.125|编辑，操作人：jinri，操作时间: 2015-10-30 09:54:22，操作人IP：192.168.6.125|编辑，操作人：jinri，操作时间: 2015-10-31 10:52:10，操作人IP：192.168.6.125|编辑，操作人：jinri，操作时间: 2015-10-31 10:52:40，操作人IP：192.168.6.125|编辑，操作人：jinri，操作时间: 2015-10-31 10:59:50，操作人IP：192.168.6.125|编辑，操作人：jinri，操作时间: 2015-10-31 11:04:50，操作人IP：192.168.6.125|编辑，操作人：jinri，操作时间: 2015-10-31 14:37:10，操作人IP：192.168.6.125|编辑，操作人：jinri，操作时间: 2015-11-02 14:47:22，操作人IP：192.168.6.125|编辑，操作人：jinri，操作时间: 2015-11-03 09:11:13，操作人IP：192.168.5.149|编辑，操作人：jinri，操作时间: 2015-11-03 11:27:37，操作人IP：192.168.6.113|'),(2,'2015-10-29 20:16:55','2015-10-29 20:16:55','ScanServiceSetting','服务配置','','{ScanSettingList:[{MessType:\'OrderTicketOut\',OrderStatus:2,IntervalTime:10,OrderBy:\'OutTime\',ScanOrderIdInit:86000,ScanCount:50,Include:\'213051,0\',IdleSleepTime:200,NextSpan:60},{MessType:\'NotifyZBNTicketOut\',OrderStatus:7,IntervalTime:10,OrderBy:\'Contingent7\',ScanOrderIdInit:86000,ScanCount:50,Include:\'662316,0\',IdleSleepTime:200,NextSpan:60},{MessType:\'OrderReturnResult\',OrderStatus:5,IntervalTime:10,OrderBy:\'OverTime\',ScanOrderIdInit:86000,ScanCount:50,Include:\'0\',IdleSleepTime:200,NextSpan:60}]}','新增，操作人：jinri，操作时间: 2015-10-29 20:16:40，操作人IP：192.168.6.125|'),(3,'2015-10-29 20:17:13','2016-01-12 18:27:28','ReceiveServiceSetting','服务配置','','{\"IsDirectRouteHighToBuilderService\":\"true\",\"IsDirectRouteMiddleToBuilderService\":\"false\",\"IsDirectRouteNormalToBuilderService\":\"false\",\"IsDirectRouteLowToBuilderService\":\"false\",\"IsOpenBatchReceiveHighMessage\":\"false\",\"AutoFlushReceiveHighMessage\":\"1\",\"IsOpenBatchReceiveMiddleMessage\":\"true\",\"AutoFlushReceiveMiddleMessage\":\"3\",\"IsOpenBatchReceiveNormalMessage\":\"true\",\"AutoFlushReceiveNormalMessage\":\"5\",\"IsOpenBatchReceiveLowMessage\":\"true\",\"AutoFlushReceiveLowMessage\":\"7\",\"EnableJudgeHasReceived\":\"true\"}','新增，操作人：jinri，操作时间: 2015-10-29 20:16:58，操作人IP：192.168.6.125|编辑，操作人：jinri，操作时间: 2015-10-31 10:51:17，操作人IP：192.168.6.125|编辑，操作人：jinri，操作时间: 2015-12-10 15:09:42，操作人IP：192.168.6.125|编辑，操作人：jinri，操作时间: 2015-12-14 16:54:05，操作人IP：192.168.6.125|编辑，操作人：jinri，操作时间: 2015-12-14 17:13:42，操作人IP：192.168.6.125|编辑，操作人：lixiaobo，操作时间: 2016-01-12 17:13:28，操作人IP：192.168.6.125|编辑，操作人：lixiaobo，操作时间: 2016-01-12 18:28:11，操作人IP：192.168.6.125|'),(4,'2015-10-29 20:17:32','2016-11-24 11:10:15','RedoServiceSetting','服务配置','','{\"IsOpenBatchSendPushMessage\":\"true\",\"AutoFlushSendMessage\":\"3\",\"PushAheadTime\":\"10\",\"ScanSettingList\":[{\"MessagePriority\":\"High\",\"MessageType\":[\"OrderTicketOut\"],\"LimitCount\":\"100\",\"PushStatus\":\"4\",\"InternalTime\":\"2\",\"IdleSleepTime\":\"100\",\"PrevScanTimes\":\"176800\",\"NextScanTimes\":\"2\"},{\"MessagePriority\":\"Middle\",\"MessageType\":[\"OrderPayResult\",\"NotifyZBNTicketOut\",\"OrderCreated\"],\"LimitCount\":\"100\",\"PushStatus\":\"4\",\"InternalTime\":\"2\",\"PrevScanTimes\":\"176800\",\"IdleSleepTime\":\"200\",\"NextScanTimes\":\"10\"},{\"MessagePriority\":\"Normal\",\"MessageType\":[\"OrderReturnResult\",\"ThirdOrderPaySuccess\",\"SearchFlightResult\"],\"LimitCount\":\"100\",\"PushStatus\":\"4\",\"InternalTime\":\"2\",\"PrevScanTimes\":\"176800\",\"IdleSleepTime\":\"200\",\"NextScanTimes\":\"10\"},{\"MessagePriority\":\"Low\",\"MessageType\":[\"OrderCancel\"],\"LimitCount\":\"100\",\"PushStatus\":\"4\",\"InternalTime\":\"2\",\"PrevScanTimes\":\"176800\",\"IdleSleepTime\":\"1000\",\"NextScanTimes\":\"10\"}]}','新增，操作人：jinri，操作时间: 2015-10-29 20:17:17，操作人IP：192.168.6.125|编辑，操作人：jinri，操作时间: 2015-10-31 11:01:50，操作人IP：192.168.6.125|编辑，操作人：jinri，操作时间: 2015-10-31 11:02:08，操作人IP：192.168.6.125|编辑，操作人：jinri，操作时间: 2015-11-02 13:38:42，操作人IP：192.168.6.125|编辑，操作人：jinri，操作时间: 2015-11-02 14:09:47，操作人IP：192.168.6.125|编辑，操作人：lixiaobo，操作时间: 2016-06-29 20:53:51，操作人IP：192.168.70.122|编辑，操作人：lixiaobo，操作时间: 2016-11-18 14:42:24，操作人IP：192.168.3.2|编辑，操作人：lixiaobo，操作时间: 2016-11-22 15:29:45，操作人IP：192.168.3.2|编辑，操作人：lixiaobo，操作时间: 2016-11-23 15:19:37，操作人IP：192.168.3.2|编辑，操作人：lixiaobo，操作时间: 2016-11-23 15:19:50，操作人IP：192.168.3.2|编辑，操作人：lixiaobo，操作时间: 2016-11-23 16:02:52，操作人IP：192.168.3.2|编辑，操作人：lixiaobo，操作时间: 2016-11-23 17:04:29，操作人IP：192.168.3.2|编辑，操作人：lixiaobo，操作时间: 2016-11-23 17:04:36，操作人IP：192.168.3.2|编辑，操作人：lixiaobo，操作时间: 2016-11-23 17:15:19，操作人IP：192.168.3.2|编辑，操作人：lixiaobo，操作时间: 2016-11-23 17:17:15，操作人IP：192.168.3.2|编辑，操作人：lixiaobo，操作时间: 2016-11-24 10:02:51，操作人IP：192.168.3.2|编辑，操作人：lixiaobo，操作时间: 2016-11-24 11:18:33，操作人IP：192.168.3.2|'),(5,'2015-10-29 20:17:48','2015-10-29 20:17:48','SendServiceSetting','服务配置','','{IsOpenBatchSendHighMessage:false, AutoFlushSendHighMessage:1, IsOpenBatchSendMiddleMessage:true, AutoFlushSendMiddleMessage:3, IsOpenBatchSendNormalMessage:true, AutoFlushSendNormalMessage:4, IsOpenBatchSendLowMessage:true, AutoFlushSendLowMessage:5, SendToClientTimeout:15000}','新增，操作人：jinri，操作时间: 2015-10-29 20:17:33，操作人IP：192.168.6.125|'),(7,'2015-10-30 09:12:12','2015-10-30 09:12:12','LogSetting','服务配置','','{\"LogLevel\":\"Debug\"}','新增，操作人：jinri，操作时间: 2015-10-30 09:11:56，操作人IP：192.168.6.125|');
/*!40000 ALTER TABLE `notifysetting` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2019-08-22 16:05:08
