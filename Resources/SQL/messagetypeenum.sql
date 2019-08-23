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
-- Table structure for table `messagetypeenum`
--

DROP TABLE IF EXISTS `messagetypeenum`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `messagetypeenum` (
  `messagetypeid` int(11) NOT NULL AUTO_INCREMENT COMMENT '消息类型自增id',
  `messagetype` varchar(255) NOT NULL COMMENT '消息类型',
  `messagepriority` varchar(255) NOT NULL,
  `status` int(11) NOT NULL COMMENT '状态',
  `createtime` datetime NOT NULL COMMENT '创建时间',
  `remark` varchar(1000) NOT NULL COMMENT '说明',
  PRIMARY KEY (`messagetypeid`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `messagetypeenum`
--

LOCK TABLES `messagetypeenum` WRITE;
/*!40000 ALTER TABLE `messagetypeenum` DISABLE KEYS */;
INSERT INTO `messagetypeenum` VALUES (1,'OrderTicketOut','High',2,'2015-10-28 09:30:33','订单出票'),(2,'OrderPayResult','Middle',2,'2015-10-28 09:30:53','支付通知'),(3,'OrderReturnResult','Normal',2,'2015-10-28 09:31:16','退票通知'),(4,'NotifyZBNTicketOut','Middle',2,'2015-10-28 09:31:49','订单暂不能出'),(5,'OrderCancel','Low',2,'2015-10-28 09:32:22','订单取消'),(6,'OrderCreated','Middle',2,'2015-12-14 17:27:58','订单创建成功通知'),(7,'OrderRefundResult','Normal',2,'2015-12-23 20:42:48','废票通知'),(8,'OrderApplyReturn','Normal',2,'2015-12-23 20:45:26','申请退费中'),(9,'OrderApplyRefund','Normal',2,'2015-12-23 20:45:47','申请废票中'),(10,'ThirdOrderPaySuccess','Normal',2,'2016-06-21 18:12:08','第三方订单支付成功消息'),(11,'SearchFlightResult','Normal',2,'2016-11-17 09:49:48','航班查询数据'),(12,'NotifyZBNReturn','Middle',2,'2017-09-18 13:27:33','订单暂不能退');
/*!40000 ALTER TABLE `messagetypeenum` ENABLE KEYS */;
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
