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
-- Table structure for table `notifyinterfacesetting`
--

DROP TABLE IF EXISTS `notifyinterfacesetting`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `notifyinterfacesetting` (
  `settingid` int(4) NOT NULL AUTO_INCREMENT,
  `status` int(4) NOT NULL,
  `method` varchar(10) NOT NULL,
  `pushlimitcount` int(4) NOT NULL,
  `encoding` varchar(10) NOT NULL,
  `createtime` datetime NOT NULL,
  `lastmodifytime` datetime NOT NULL,
  `appid` varchar(10) NOT NULL,
  `messagetype` varchar(255) NOT NULL,
  `address` varchar(255) NOT NULL,
  `pushinternalrule` varchar(255) NOT NULL,
  PRIMARY KEY (`settingid`)
) ENGINE=InnoDB AUTO_INCREMENT=46 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `notifyinterfacesetting`
--

LOCK TABLES `notifyinterfacesetting` WRITE;
/*!40000 ALTER TABLE `notifyinterfacesetting` DISABLE KEYS */;
INSERT INTO `notifyinterfacesetting` VALUES (1,3,'POST',5,'UTF-8','2015-10-29 20:47:29','2015-10-29 20:47:29','100001','OrderTicketOut','http://101.231.41.6:11211/MessageHandler/CommonHandler.ashx','0,60,300,600,3600'),(2,3,'POST',5,'UTF-8','2015-10-29 20:48:13','2015-10-29 20:48:13','100001','OrderPayResult','http://101.231.41.6:11211/MessageHandler/CommonHandler.ashx','0,60,300,600,3600'),(3,3,'POST',5,'UTF-8','2015-10-30 17:33:39','2015-10-30 17:33:39','100002','OrderReturnResult','http://101.231.41.6:11211/MessageHandler/CommonHandler.ashx','0,60,300,600,3600'),(4,3,'POST',5,'UTF-8','2015-10-30 17:34:28','2015-10-30 17:34:28','100003','NotifyZBNTicketOut','http://101.231.41.6:11211/MessageHandler/CommonHandler.ashx','0,60,300,600,3600'),(5,3,'POST',5,'UTF-8','2015-10-31 15:39:40','2015-10-31 15:39:40','100003','OrderCancel','http://101.231.41.6:11211/MessageHandler/CommonHandler.ashx','0,60,300,600,3600'),(6,2,'GET',4,'UTF-8','2015-12-04 10:40:13','2015-12-04 10:40:13','100001','OrderPayResult','http://ticket.notify.jinri.cn/OutTicketMessage/OutTicketMessage','0,10,30,60'),(7,3,'GET',4,'UTF-8','2015-12-24 10:48:22','2015-12-24 10:48:22','100001','OrderPayResult','http://tticket.notify.jinri.cn/OutTicketMessage/OutTicketMessage','0,10,30,60'),(8,3,'POST',5,'UTF-8','2015-12-24 11:11:35','2015-12-24 11:11:35','100001','OrderPayResult','http://101.231.41.6:11211/MessageHandler/ProviderHandler.ashx','0,60,300,600,3600'),(9,3,'POST',5,'UTF-8','2015-12-24 11:15:19','2015-12-24 11:15:19','100001','OrderApplyReturn','http://101.231.41.6:11211/MessageHandler/ProviderHandler.ashx','0,60,300,600,3600'),(10,3,'POST',5,'UTF-8','2015-12-24 11:15:43','2015-12-24 11:15:43','100001','OrderApplyRefund','http://101.231.41.6:11211/MessageHandler/ProviderHandler.ashx','0,60,300,600,3600'),(11,3,'POST',3,'UTF-8','2015-12-28 17:42:09','2015-12-28 17:42:09','100001','OrderPayResult','http://101.231.41.6:11211/MessageHandler/DataStatisticHandler.ashx','0,60,300'),(12,3,'POST',3,'UTF-8','2015-12-29 15:24:44','2015-12-29 15:24:44','100001','OrderPayResult','http://notify.handler.jinri.cn/MessageHandler/DataStatisticHandler.ashx','0,60,300'),(13,2,'POST',3,'UTF-8','2015-12-29 16:07:16','2015-12-29 16:07:16','100001','OrderPayResult','http://notify.handler.jinri.cn/MessageHandler/DataStatisticHandler.ashx','0,60,300'),(14,2,'POST',5,'UTF-8','2015-12-30 12:59:01','2015-12-30 12:59:01','100001','OrderPayResult','http://notify.handler.jinri.cn/MessageHandler/CommonHandler.ashx','0,60,300,600,3600'),(15,2,'POST',5,'UTF-8','2015-12-30 13:00:15','2015-12-30 13:00:15','100001','OrderPayResult','http://notify.handler.jinri.cn/MessageHandler/ProviderHandler.ashx','0,60,300,600,3600'),(16,2,'POST',5,'UTF-8','2015-12-30 14:12:57','2015-12-30 14:12:57','100002','OrderTicketOut','http://notify.handler.jinri.cn/MessageHandler/CommonHandler.ashx','0,60,300,600,3600'),(17,2,'POST',5,'UTF-8','2015-12-30 14:15:13','2015-12-30 14:15:13','100003','NotifyZBNTicketOut','http://notify.handler.jinri.cn/MessageHandler/CommonHandler.ashx','0,60,300,600,3600'),(18,2,'POST',5,'UTF-8','2015-12-30 17:20:02','2015-12-30 17:20:02','100004','OrderReturnResult','http://notify.handler.jinri.cn/MessageHandler/CommonHandler.ashx','0,60,300,600,3600'),(19,2,'POST',5,'UTF-8','2015-12-30 17:21:52','2015-12-30 17:21:52','100005','OrderCancel','http://notify.handler.jinri.cn/MessageHandler/CommonHandler.ashx','0,60,300,600,3600'),(20,2,'POST',5,'UTF-8','2015-12-30 17:23:36','2015-12-30 17:23:36','100006','OrderApplyReturn','http://notify.handler.jinri.cn/MessageHandler/ProviderHandler.ashx','0,60,300,600,3600'),(21,2,'POST',5,'UTF-8','2015-12-30 17:24:32','2015-12-30 17:24:32','100006','OrderApplyRefund','http://notify.handler.jinri.cn/MessageHandler/ProviderHandler.ashx','0,60,300,600,3600'),(22,2,'POST',1,'UTF-8','2016-01-07 10:59:13','2016-01-07 10:59:13','100001','OrderCreated','http://notify.handler.jinri.cn/MessageHandler/ProviderHandler.ashx','0'),(23,2,'POST',1,'UTF-8','2016-01-13 14:51:32','2016-01-13 14:51:32','100005','OrderTicketOut','http://notify.handler.jinri.cn/MessageHandler/DataStatisticHandler.ashx','0'),(24,2,'POST',1,'UTF-8','2016-01-13 14:51:52','2016-01-13 14:51:52','100005','NotifyZBNTicketOut','http://notify.handler.jinri.cn/MessageHandler/DataStatisticHandler.ashx','0'),(25,2,'POST',1,'UTF-8','2016-01-13 14:52:13','2016-01-13 14:52:13','100005','OrderApplyReturn','http://notify.handler.jinri.cn/MessageHandler/DataStatisticHandler.ashx','0'),(26,2,'POST',1,'UTF-8','2016-01-13 14:52:33','2016-01-13 14:52:33','100005','OrderApplyRefund','http://notify.handler.jinri.cn/MessageHandler/DataStatisticHandler.ashx','0'),(27,3,'POST',5,'UTF-8','2016-03-10 10:59:25','2016-03-10 10:59:25','100001','OrderPayResult','http://101.231.41.6:11212/MessageHandler/ProviderHandler.ashx','0,60,300,600,3600'),(28,2,'POST',5,'UTF-8','2016-04-29 13:47:13','2016-04-29 13:47:13','100006','OrderTicketOut','http://notify.handler.jinri.cn/MessageHandler/ProviderHandler.ashx','0,60,300,600,3600'),(29,2,'POST',10,'UTF-8','2016-06-27 11:12:46','2016-06-27 11:12:46','100001','ThirdOrderPaySuccess','http://notify.handler.jinri.cn/MessageHandler/ProviderHandler.ashx','0,10'),(30,2,'POST',5,'UTF-8','2016-11-17 10:54:28','2016-11-17 10:54:28','100000','SearchFlightResult','http://handler.lowprice.jinri.cn/LowPriceHandler.ashx','0,60,60,60,60'),(31,2,'POST',6,'UTF-8','2016-11-22 09:27:38','2016-11-22 09:27:38','100201','OrderCreated','http://handler.erasepnr.jinri.cn/api/ErasePnr','1470,30'),(32,2,'POST',1,'UTF-8','2016-12-09 13:54:35','2016-12-09 13:54:35','100007','OrderTicketOut','http://handler.order.jinri.cn/api/Notify','0'),(33,2,'POST',1,'UTF-8','2016-12-09 14:01:43','2016-12-09 14:01:43','100007','OrderCreated','http://handler.order.jinri.cn/api/Notify','0'),(34,2,'POST',1,'UTF-8','2016-12-21 15:05:23','2016-12-21 15:05:23','100001','OrderPayResult','http://handler.order.jinri.cn/api/Notify','0'),(35,2,'POST',1,'UTF-8','2017-04-26 10:45:03','2017-04-26 10:45:03','100001','NotifyZBNTicketOut','http://handler.order.jinri.cn/api/Notify','0'),(36,2,'POST',5,'UTF-8','2017-05-02 17:52:47','2017-05-02 17:52:47','100001','OrderPayResult','http://api.jinri.cn/api/ThirdCreateOrder','0,20,40,60,120'),(37,2,'POST',1,'UTF-8','2017-05-03 18:34:48','2017-05-03 18:34:48','100001','OrderApplyReturn','http://handler.order.jinri.cn/api/Notify','0'),(38,2,'POST',1,'UTF-8','2017-05-03 18:36:17','2017-05-03 18:36:17','100001','OrderApplyRefund','http://handler.order.jinri.cn/api/Notify','0'),(39,2,'POST',1,'UTF-8','2017-05-03 18:37:19','2017-05-03 18:37:19','100001','OrderRefundResult','http://handler.order.jinri.cn/api/Notify','0'),(40,2,'POST',1,'UTF-8','2017-05-03 18:39:12','2017-05-03 18:39:12','100001','OrderCancel','http://handler.order.jinri.cn/api/Notify','0'),(41,2,'POST',1,'UTF-8','2017-05-04 09:04:20','2017-05-04 09:04:20','100001','OrderReturnResult','http://handler.order.jinri.cn/api/Notify','0'),(42,3,'POST',1,'UTF-8','2017-05-24 14:18:38','2017-05-24 14:18:38','11','SearchFlightResult','1','1'),(43,2,'POST',5,'UTF-8','2017-08-25 10:04:13','2017-08-25 10:04:13','100004','OrderReturnResult','http://notify.handler.jinri.cn/MessageHandler/ProviderHandler.ashx','0,60,300,600,3600'),(44,2,'POST',1,'UTF-8','2017-09-18 14:10:06','2017-09-18 14:10:06','100001','NotifyZBNReturn','http://handler.order.jinri.cn/api/Notify','0'),(45,3,'POST',2,'UTF-8','2019-03-11 14:00:11','2019-03-11 14:00:11','100001','OrderPayResult','http://tweb.ws.jinri.org.cn/MessageHandler/ProviderHandler.ashx','0,60');
/*!40000 ALTER TABLE `notifyinterfacesetting` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2019-08-22 16:05:09
