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
-- Table structure for table `pushmessage`
--

DROP TABLE IF EXISTS `pushmessage`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pushmessage` (
  `pid` int(11) NOT NULL AUTO_INCREMENT,
  `pushid` varchar(32) NOT NULL,
  `messageid` varchar(32) NOT NULL,
  `settingid` int(4) NOT NULL,
  `pushstatus` int(4) NOT NULL,
  `nextpushtime` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `lastmodifytime` datetime NOT NULL,
  `messagecreatetime` datetime NOT NULL,
  `createtime` datetime NOT NULL,
  `pushcount` int(4) NOT NULL,
  `messagepriority` varchar(20) NOT NULL,
  `messagekey` varchar(255) NOT NULL,
  `messagetype` varchar(255) NOT NULL,
  `pushdata` varchar(8000) NOT NULL,
  `memo` varchar(8000) NOT NULL,
  PRIMARY KEY (`pid`),
  UNIQUE KEY `index_pushid` (`pushid`),
  KEY `ind_pushmessage_nextpushtime` (`nextpushtime`),
  KEY `ind_pushmessage_MessagePriority` (`messagepriority`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=197658885 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pushmessage`
--

LOCK TABLES `pushmessage` WRITE;
/*!40000 ALTER TABLE `pushmessage` DISABLE KEYS */;
/*!40000 ALTER TABLE `pushmessage` ENABLE KEYS */;
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
