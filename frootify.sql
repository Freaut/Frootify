-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Värd: 127.0.0.1
-- Tid vid skapande: 13 mars 2024 kl 10:04
-- Serverversion: 10.4.24-MariaDB
-- PHP-version: 8.1.6

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Databas: `frootify`
--

-- --------------------------------------------------------

--
-- Tabellstruktur `playlists`
--

CREATE TABLE `playlists` (
  `Id` int(11) NOT NULL,
  `Name` text NOT NULL,
  `Description` text NOT NULL,
  `Image` text NOT NULL,
  `Songs` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumpning av Data i tabell `playlists`
--

INSERT INTO `playlists` (`Id`, `Name`, `Description`, `Image`, `Songs`) VALUES
(4, 'ELD WALLA', '', 'https://res.cloudinary.com/dcogdkkwa/image/upload/v1700783218/nnqcxyu577txkse67ayq.jpg', 'WyJDOlxcVXNlcnNcXHRoZWdhXFxEb3dubG9hZHNcXG1vc2ljXFxEanVuZ2VsdHJ1YmFkdXJlbiBhbGxhIGxcdTAwRTV0YXIubXAzLm1wMyIsIkM6XFxVc2Vyc1xcdGhlZ2FcXERvd25sb2Fkc1xcbW9zaWNcXFNUQVJTRVQgIEVBUlRIUklTRS5tcDMiLCJDOlxcVXNlcnNcXHRoZWdhXFxEb3dubG9hZHNcXG1vc2ljXFxCb3lXaXRoVWtlIGZ0IGJsYWNrYmVhciAgSURHQUYgT2ZmaWNpYWwgTXVzaWMgVmlkZW8ubXAzIiwiQzpcXFVzZXJzXFx0aGVnYVxcRG93bmxvYWRzXFxtb3NpY1xcQm95V2l0aFVrZSAgTG9hZmVycyBPZmZpY2lhbCBMeXJpYyBWaWRlby5tcDMiXQ==');

--
-- Index för dumpade tabeller
--

--
-- Index för tabell `playlists`
--
ALTER TABLE `playlists`
  ADD PRIMARY KEY (`Id`);

--
-- AUTO_INCREMENT för dumpade tabeller
--

--
-- AUTO_INCREMENT för tabell `playlists`
--
ALTER TABLE `playlists`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
