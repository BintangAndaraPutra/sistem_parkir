-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Nov 20, 2026 at 11:02 AM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `sistem_parkir`
--

-- --------------------------------------------------------

--
-- Table structure for table `area_parkir`
--

CREATE TABLE `area_parkir` (
  `id_area` int(11) NOT NULL,
  `nama_area` varchar(30) NOT NULL,
  `kapasitas` int(11) NOT NULL,
  `keterangan` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `area_parkir`
--

INSERT INTO `area_parkir` (`id_area`, `nama_area`, `kapasitas`, `keterangan`) VALUES
(1, 'Lantai 1', 100, 'Motor dan mobil'),
(2, 'Basement', 50, 'Mobil');

-- --------------------------------------------------------

--
-- Table structure for table `jenis_kendaraan`
--

CREATE TABLE `jenis_kendaraan` (
  `id_jenis` int(11) NOT NULL,
  `nama_jenis` varchar(30) NOT NULL,
  `tarif_per_jam` decimal(10,2) NOT NULL,
  `tarif_maks_harian` decimal(10,2) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `jenis_kendaraan`
--

INSERT INTO `jenis_kendaraan` (`id_jenis`, `nama_jenis`, `tarif_per_jam`, `tarif_maks_harian`) VALUES
(1, 'Motor', 2000.00, 10000.00),
(2, 'Mobil', 5000.00, 25000.00);

-- --------------------------------------------------------

--
-- Table structure for table `transaksi_parkir`
--

CREATE TABLE `transaksi_parkir` (
  `id_transaksi` int(11) NOT NULL,
  `no_tiket` varchar(20) NOT NULL,
  `plat_nomor` varchar(15) NOT NULL,
  `id_jenis` int(11) NOT NULL,
  `id_area` int(11) NOT NULL,
  `id_user` int(11) NOT NULL,
  `waktu_masuk` datetime NOT NULL DEFAULT current_timestamp(),
  `waktu_keluar` datetime DEFAULT NULL,
  `durasi_jam` int(11) DEFAULT NULL,
  `total_biaya` decimal(10,2) DEFAULT NULL,
  `status` varchar(10) NOT NULL DEFAULT 'Parkir'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `id_user` int(11) NOT NULL,
  `username` varchar(30) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `nama_lengkap` varchar(100) NOT NULL,
  `role` varchar(10) NOT NULL,
  `status_aktif` tinyint(1) NOT NULL DEFAULT 1,
  `created_at` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`id_user`, `username`, `password_hash`, `nama_lengkap`, `role`, `status_aktif`, `created_at`) VALUES
(1, 'admin', 'ganti_dengan_hash', 'Budi Santoso', 'Admin', 1, '2026-11-20 16:48:48'),
(2, 'petugas1', 'ganti_dengan_hash', 'Sari Wulandari', 'Petugas', 1, '2026-11-20 16:48:48');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `area_parkir`
--
ALTER TABLE `area_parkir`
  ADD PRIMARY KEY (`id_area`),
  ADD UNIQUE KEY `nama_area` (`nama_area`);

--
-- Indexes for table `jenis_kendaraan`
--
ALTER TABLE `jenis_kendaraan`
  ADD PRIMARY KEY (`id_jenis`),
  ADD UNIQUE KEY `nama_jenis` (`nama_jenis`);

--
-- Indexes for table `transaksi_parkir`
--
ALTER TABLE `transaksi_parkir`
  ADD PRIMARY KEY (`id_transaksi`),
  ADD UNIQUE KEY `no_tiket` (`no_tiket`),
  ADD KEY `id_jenis` (`id_jenis`),
  ADD KEY `id_area` (`id_area`),
  ADD KEY `id_user` (`id_user`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id_user`),
  ADD UNIQUE KEY `username` (`username`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `area_parkir`
--
ALTER TABLE `area_parkir`
  MODIFY `id_area` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `jenis_kendaraan`
--
ALTER TABLE `jenis_kendaraan`
  MODIFY `id_jenis` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `transaksi_parkir`
--
ALTER TABLE `transaksi_parkir`
  MODIFY `id_transaksi` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `id_user` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `transaksi_parkir`
--
ALTER TABLE `transaksi_parkir`
  ADD CONSTRAINT `transaksi_parkir_ibfk_1` FOREIGN KEY (`id_jenis`) REFERENCES `jenis_kendaraan` (`id_jenis`),
  ADD CONSTRAINT `transaksi_parkir_ibfk_2` FOREIGN KEY (`id_area`) REFERENCES `area_parkir` (`id_area`),
  ADD CONSTRAINT `transaksi_parkir_ibfk_3` FOREIGN KEY (`id_user`) REFERENCES `users` (`id_user`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
