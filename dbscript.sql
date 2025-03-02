-- --------------------------------------------------------
-- Host:                         sbsdomain.com
-- Server version:               10.5.25-MariaDB - MariaDB Server
-- Server OS:                    Linux
-- HeidiSQL Version:             12.10.0.7000
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Dumping database structure for sbsdosnr_ocart
CREATE DATABASE IF NOT EXISTS `blocklistdb` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci */;
USE `blocklistdb`;

-- Dumping structure for table sbsdosnr_ocart.Device
CREATE TABLE IF NOT EXISTS `Device` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `MACAddress` varchar(25) NOT NULL DEFAULT '',
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE KEY `UC_Device_MACAddress` (`MACAddress`)
) ENGINE=MyISAM AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table sbsdosnr_ocart.Device: 1 rows
/*!40000 ALTER TABLE `Device` DISABLE KEYS */;
INSERT INTO `Device` (`ID`, `MACAddress`) VALUES
	(1, '2C:3B:70:0C:DA:F5');
/*!40000 ALTER TABLE `Device` ENABLE KEYS */;

-- Dumping structure for table sbsdosnr_ocart.DeviceRemoteSite
CREATE TABLE IF NOT EXISTS `DeviceRemoteSite` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `DeviceID` int(11) NOT NULL,
  `RemoteSiteID` int(11) NOT NULL,
  `LastDownloaded` datetime NOT NULL DEFAULT '2001-01-01 00:00:00',
  PRIMARY KEY (`ID`),
  UNIQUE KEY `IX_DeviceRemoteSite_DeviceID_RemoteSiteID` (`DeviceID`,`RemoteSiteID`),
  KEY `IX_DeviceRemoteSite_RemoteSiteID` (`RemoteSiteID`),
  KEY `IX_DeviceRemoteSite_DeviceID` (`DeviceID`) USING BTREE
) ENGINE=MyISAM AUTO_INCREMENT=29 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table sbsdosnr_ocart.DeviceRemoteSite: 24 rows
/*!40000 ALTER TABLE `DeviceRemoteSite` DISABLE KEYS */;
INSERT INTO `DeviceRemoteSite` (`ID`, `DeviceID`, `RemoteSiteID`, `LastDownloaded`) VALUES
	(1, 1, 1, '2025-03-02 10:12:53'),
	(2, 1, 2, '2025-03-02 10:13:39'),
	(3, 1, 3, '2025-03-02 10:13:02'),
	(4, 1, 4, '2025-03-02 10:13:08'),
	(28, 1, 5, '2025-03-02 10:17:46'),
	(6, 1, 6, '2025-03-02 10:12:19'),
	(7, 1, 7, '2025-03-02 10:12:31'),
	(8, 1, 8, '2025-03-02 10:06:49'),
	(9, 1, 9, '2025-03-02 10:12:48'),
	(10, 1, 10, '2025-03-02 10:13:17'),
	(11, 1, 11, '2025-03-02 10:13:22'),
	(12, 1, 12, '2025-03-02 10:13:12'),
	(13, 1, 13, '2025-03-02 10:13:50'),
	(14, 1, 14, '2025-03-02 10:14:00'),
	(15, 1, 15, '2025-03-02 10:13:45'),
	(16, 1, 16, '2025-03-02 10:12:24'),
	(17, 1, 17, '2025-03-02 10:12:35'),
	(18, 1, 18, '2025-03-02 10:12:43'),
	(19, 1, 19, '2001-01-01 00:00:00'),
	(20, 1, 22, '2025-03-02 10:13:34'),
	(21, 1, 24, '2025-03-02 10:13:29'),
	(22, 1, 25, '2001-01-01 00:00:00'),
	(23, 1, 26, '2025-03-02 10:13:55'),
	(24, 1, 27, '2025-03-02 10:12:58');
/*!40000 ALTER TABLE `DeviceRemoteSite` ENABLE KEYS */;

-- Dumping structure for table sbsdosnr_ocart.FileType
CREATE TABLE IF NOT EXISTS `FileType` (
  `ID` int(11) NOT NULL,
  `Name` varchar(50) NOT NULL DEFAULT '',
  `Description` varchar(255) NOT NULL DEFAULT '',
  PRIMARY KEY (`ID`) USING BTREE,
  UNIQUE KEY `IX_FileType_Name` (`Name`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1 COLLATE=latin1_general_cs MAX_ROWS=1000 COMMENT='BlocklistManager';

-- Dumping data for table sbsdosnr_ocart.FileType: 9 rows
/*!40000 ALTER TABLE `FileType` DISABLE KEYS */;
INSERT INTO `FileType` (`ID`, `Name`, `Description`) VALUES
	(1, 'TXT', 'Single column text file listing IP addresses'),
	(2, 'JSON', 'Json'),
	(3, 'XML', 'XML'),
	(4, 'TAB', 'Tab delimited'),
	(5, 'JSONZIP', 'Zip archive containing Json'),
	(6, 'TXTZIP', 'Zip archive containing text'),
	(7, 'DELIMZIP', 'Zip archive containing delimited data'),
	(8, 'TXTALIEN', 'AlienVault text layout'),
	(9, 'CSV', 'Comma delimited');
/*!40000 ALTER TABLE `FileType` ENABLE KEYS */;

-- Dumping structure for table sbsdosnr_ocart.RemoteSite
CREATE TABLE IF NOT EXISTS `RemoteSite` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `LastDownloaded` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00.000000',
  `SiteUrl` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `FileUrls` varchar(4000) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `FileTypeID` int(11) NOT NULL,
  `Active` tinyint(1) NOT NULL,
  `MinimumIntervalMinutes` int(11) NOT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `IX_RemoteSite_Name` (`Name`,`SiteUrl`) USING HASH,
  KEY `IX_RemoteSite_FileTypeID` (`FileTypeID`)
) ENGINE=MyISAM AUTO_INCREMENT=28 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Dumping data for table sbsdosnr_ocart.RemoteSite: 24 rows
/*!40000 ALTER TABLE `RemoteSite` DISABLE KEYS */;
INSERT INTO `RemoteSite` (`ID`, `Name`, `LastDownloaded`, `SiteUrl`, `FileUrls`, `FileTypeID`, `Active`, `MinimumIntervalMinutes`) VALUES
	(1, 'Feodo', '2024-12-31 12:00:00.000000', 'https://feodotracker.abuse.ch', 'https://feodotracker.abuse.ch/downloads/ipblocklist_recommended.json, https://feodotracker.abuse.ch/downloads/ipblocklist.json', 2, 1, 0),
	(2, 'MyIP', '2024-12-31 12:00:00.000000', 'https://myip.ms', 'https://myip.ms/files/blacklist/general/latest_blacklist.txt', 9, 1, 0),
	(3, 'FireHOL Level 3', '2024-12-31 12:00:00.000000', 'https://raw.githubusercontent.com/firehol', 'https://raw.githubusercontent.com/firehol/blocklist-ipsets/master/firehol_level3.netset', 1, 1, 0),
	(4, 'GreenSnow', '2024-12-31 12:00:00.000000', 'https://greensnow.co/', 'https://blocklist.greensnow.co/greensnow.txt', 1, 1, 0),
	(5, 'AlienVault', '2024-12-31 12:00:00.000000', 'https://reputation.alienvault.com', 'https://reputation.alienvault.com/reputation.generic', 8, 1, 0),
	(6, 'Binary Defense Systems Artillery Threat Intelligence Feed and Banlist Feed', '2024-12-31 12:00:00.000000', 'https://www.binarydefense.com', 'https://www.binarydefense.com/banlist.txt', 1, 1, 0),
	(7, 'CINS Army List', '2024-12-31 12:00:00.000000', 'https://cinsscore.com', 'https://cinsscore.com/list/ci-badguys.txt', 1, 1, 0),
	(8, 'dan.me.uk torlist', '2024-12-31 12:00:00.000000', 'https://www.dan.me.uk', 'https://www.dan.me.uk/torlist/index.html', 1, 1, 45),
	(9, 'Emerging Threats Compromised and Firewall Block List', '2024-12-31 12:00:00.000000', 'https://www.emergingthreats.net', 'https://rules.emergingthreats.net/fwrules/emerging-Block-IPs.txt', 1, 1, 0),
	(10, 'Internet Storm Center DShield', '2024-12-31 12:00:00.000000', 'https://feeds.dshield.org', 'https://feeds.dshield.org/block.txt', 4, 1, 0),
	(11, 'Internet Storm Center Shodan', '2024-12-31 12:00:00.000000', 'https://isc.sans.edu', 'https://isc.sans.edu/api/threatlist/shodan/shodan.txt', 3, 1, 0),
	(12, 'IBM X-Force Exchange', '2024-12-31 12:00:00.000000', 'https://exchange.xforce.ibmcloud.com/', 'https://iplists.firehol.org/files/xforce_bccs.ipset', 1, 1, 0),
	(13, 'pgl.yoyo.org AdServers', '2024-12-31 12:00:00.000000', 'https://pgl.yoyo.org', 'https://pgl.yoyo.org/adservers/iplist.php?ipformat=&showintro=0&mimetype=plaintext', 1, 1, 0),
	(14, 'ScriptzTeam', '2024-12-31 12:00:00.000000', 'https://github.com/scriptzteam/IP-BlockList-v4/blob', 'https://raw.githubusercontent.com/scriptzteam/IP-BlockList-v4/refs/heads/main/ips.txt', 4, 1, 0),
	(15, 'PAllebone', '2024-12-31 12:00:00.000000', 'https://github.com/pallebone/StrictBlockPAllebone', 'https://raw.githubusercontent.com/pallebone/StrictBlockPAllebone/master/BlockIP.txt', 1, 1, 0),
	(16, 'Blocklist.de', '2024-12-31 12:00:00.000000', 'http://www.blocklist.de/en/index.html', 'http://lists.blocklist.de/lists/all.txt', 1, 0, 0),
	(17, 'CyberCrime-Tracker', '2024-12-31 12:00:00.000000', 'https://cybercrime-tracker.net/fuckerz.php', 'https://cybercrime-tracker.net/rss.xml', 3, 0, 0),
	(18, 'DigitalSide Threat-Intel Repository', '2024-12-31 12:00:00.000000', 'https://osint.digitalside.it/', 'https://osint.digitalside.it/Threat-Intel/lists/latestips.txt', 1, 0, 0),
	(19, 'abuse.ch', '2024-12-31 12:00:00.000000', 'https://sslbl.abuse.ch/blacklist/', 'https://sslbl.abuse.ch/blacklist/sslipblacklist.txt', 1, 0, 0),
	(22, 'Miroslav Stampar', '2024-12-31 12:00:00.000000', 'https://github.com/stamparm', 'https://raw.githubusercontent.com/stamparm/ipsum/master/ipsum.txt', 1, 0, 0),
	(24, 'James Brine', '2024-12-31 12:00:00.000000', 'https://jamesbrine.com.au', 'https://jamesbrine.com.au/csv', 9, 0, 0),
	(25, 'NoThink!', '2024-12-31 12:00:00.000000', 'https://www.nothink.org', 'https://www.nothink.org/honeypots/honeypot_ssh_blacklist_2019.txt, https://www.nothink.org/honeypots/honeypot_telnet_blacklist_2019.txt', 1, 0, 0),
	(26, 'Rutgers Blacklisted IPs', '2024-12-31 12:00:00.000000', 'https://report.cs.rutgers.edu/mrtg/drop/dropstat.cgi?start=-86400', 'https://report.cs.rutgers.edu/DROP/attackers', 1, 0, 0),
	(27, 'FireHOL Level 1', '2024-12-31 12:00:00.000000', 'http://iplists.firehol.org/?ipset=firehol_level1', 'https://raw.githubusercontent.com/ktsaou/blocklist-ipsets/master/firehol_level1.netset', 1, 0, 0);
/*!40000 ALTER TABLE `RemoteSite` ENABLE KEYS */;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
