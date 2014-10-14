USE [master]
GO

CREATE DATABASE Herschel
	CONTAINMENT = NONE
ON  PRIMARY 
( NAME = N'Herschel_0', FILENAME = N'C:\Data\Raid6_0\vo\sql_db\Herschel\Herschel_0.mdf' , SIZE = 25GB , MAXSIZE = UNLIMITED, FILEGROWTH = 0), 
( NAME = N'Herschel_1', FILENAME = N'C:\Data\Raid6_1\vo\sql_db\Herschel\Herschel_1.ndf' , SIZE = 25GB , MAXSIZE = UNLIMITED, FILEGROWTH = 0), 
 FILEGROUP [LOAD] 
( NAME = N'Load_0', FILENAME = N'C:\Data\Raid6_0\vo\sql_db\Herschel\Load_0.ndf' , SIZE = 25GB , MAXSIZE = UNLIMITED, FILEGROWTH = 0), 
( NAME = N'Load_1', FILENAME = N'C:\Data\Raid6_1\vo\sql_db\Herschel\Load_1.ndf' , SIZE = 25GB , MAXSIZE = UNLIMITED, FILEGROWTH = 0)
LOG ON 
( NAME = N'Herschel_0_log', FILENAME = N'C:\Data\Raid6_0\vo\sql_db\Herschel\Herschel_0_log.ldf' , SIZE = 2500MB , MAXSIZE = 2048GB , FILEGROWTH = 0), 
( NAME = N'Herschel_1_log', FILENAME = N'C:\Data\Raid6_1\vo\sql_db\Herschel\Herschel_1_log.ldf' , SIZE = 2500MB , MAXSIZE = 2048GB , FILEGROWTH = 0)
GO

ALTER DATABASE [Herschel] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [Herschel] SET TRUSTWORTHY ON
GO

RECONFIGURE
GO

CREATE ASSEMBLY [System.Runtime.Serialization] 
FROM 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Runtime.Serialization.dll'
WITH PERMISSION_SET = UNSAFE;
GO

USE [Herschel]
GO

CREATE SCHEMA [sph]
GO

CREATE ROLE [User]
GO