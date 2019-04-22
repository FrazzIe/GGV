/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


-- Dumping database structure for ggv
CREATE DATABASE IF NOT EXISTS `ggv` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `ggv`;

-- Dumping structure for table ggv.ban
CREATE TABLE IF NOT EXISTS `ban` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `player_id` int(11) DEFAULT NULL,
  `reason` varchar(250) DEFAULT NULL,
  `expire` timestamp NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_player_ban` (`player_id`),
  CONSTRAINT `fk_player_ban` FOREIGN KEY (`player_id`) REFERENCES `player` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.
-- Dumping structure for table ggv.player
CREATE TABLE IF NOT EXISTS `player` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `steam` varchar(250) DEFAULT NULL,
  `license` varchar(250) DEFAULT NULL,
  `ip` varchar(100) DEFAULT NULL,
  `name` varchar(250) DEFAULT NULL,
  `timestamp` timestamp NULL DEFAULT current_timestamp(),
  `lastplayed` timestamp NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `wins` int(11) DEFAULT 0,
  `games_played` int(11) DEFAULT 0,
  `kills` int(11) DEFAULT 0,
  `deaths` int(11) DEFAULT 0,
  PRIMARY KEY (`id`),
  KEY `steam` (`steam`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;