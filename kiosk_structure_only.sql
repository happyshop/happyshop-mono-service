-- phpMyAdmin SQL Dump
-- version 3.4.11.1deb2+deb7u8
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: May 05, 2017 at 08:32 AM
-- Server version: 5.5.54
-- PHP Version: 5.4.45-0+deb7u8

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `kiosk_live`
--

-- --------------------------------------------------------

--
-- Table structure for table `items`
--
-- Creation: Mar 24, 2017 at 12:50 PM
--

DROP TABLE IF EXISTS `items`;
CREATE TABLE IF NOT EXISTS `items` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `barcode` varchar(45) DEFAULT NULL,
  `description` varchar(45) DEFAULT NULL,
  `price` float DEFAULT NULL,
  `image` blob,
  `count` int(11) DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `barcode_UNIQUE` (`barcode`),
  KEY `barcode_index` (`barcode`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=93 ;

-- --------------------------------------------------------

--
-- Table structure for table `old_cashups`
--
-- Creation: Mar 24, 2017 at 12:50 PM
--

DROP TABLE IF EXISTS `old_cashups`;
CREATE TABLE IF NOT EXISTS `old_cashups` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `uid` int(11) NOT NULL,
  `count` int(11) NOT NULL,
  `catid` int(11) NOT NULL,
  `ts` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=latin1 AUTO_INCREMENT=1054 ;

-- --------------------------------------------------------

--
-- Table structure for table `old_payments`
--
-- Creation: Mar 24, 2017 at 12:50 PM
-- Last update: Mar 24, 2017 at 12:50 PM
--

DROP TABLE IF EXISTS `old_payments`;
CREATE TABLE IF NOT EXISTS `old_payments` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `uid` int(11) NOT NULL,
  `ts` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `amount` decimal(10,2) NOT NULL,
  `comment` text NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=240 ;

-- --------------------------------------------------------

--
-- Table structure for table `orders`
--
-- Creation: May 04, 2017 at 04:03 PM
--

DROP TABLE IF EXISTS `orders`;
CREATE TABLE IF NOT EXISTS `orders` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `order_id` int(11) NOT NULL,
  `barcode` varchar(45) NOT NULL,
  `old_count` int(11) NOT NULL,
  `count` int(11) NOT NULL,
  `netprice` float DEFAULT '0',
  `price` float NOT NULL,
  `mwst` float DEFAULT '0',
  `shipping_cost` float DEFAULT '0',
  `timestamp` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Table structure for table `payments`
--
-- Creation: Mar 24, 2017 at 12:50 PM
-- Last update: Mar 28, 2017 at 02:32 PM
-- Last check: Apr 12, 2017 at 12:17 PM
--

DROP TABLE IF EXISTS `payments`;
CREATE TABLE IF NOT EXISTS `payments` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `uid` int(11) NOT NULL,
  `ts` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `amount` decimal(10,2) NOT NULL,
  `comment` text NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=283 ;

-- --------------------------------------------------------

--
-- Table structure for table `purchases`
--
-- Creation: Mar 24, 2017 at 12:50 PM
-- Last update: May 05, 2017 at 07:23 AM
-- Last check: Apr 12, 2017 at 12:17 PM
--

DROP TABLE IF EXISTS `purchases`;
CREATE TABLE IF NOT EXISTS `purchases` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `barcode` varchar(45) DEFAULT NULL,
  `short` varchar(3) DEFAULT NULL,
  `sum` float DEFAULT NULL,
  `when` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `short_index` (`short`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=8608 ;

-- --------------------------------------------------------

--
-- Table structure for table `users`
--
-- Creation: Mar 24, 2017 at 12:50 PM
-- Last update: May 05, 2017 at 07:23 AM
-- Last check: Apr 12, 2017 at 12:17 PM
--

DROP TABLE IF EXISTS `users`;
CREATE TABLE IF NOT EXISTS `users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `short` varchar(3) CHARACTER SET utf8 NOT NULL,
  `firstname` varchar(60) CHARACTER SET utf8 NOT NULL,
  `lastname` varchar(60) CHARACTER SET utf8 NOT NULL,
  `mail` varchar(120) CHARACTER SET utf8 NOT NULL,
  `is_mgmt` tinyint(1) NOT NULL,
  `balance` float NOT NULL,
  PRIMARY KEY (`id`),
  KEY `short_index` (`short`)
) ENGINE=MyISAM  DEFAULT CHARSET=latin1 AUTO_INCREMENT=100 ;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
