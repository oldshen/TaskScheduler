USE [master]
GO

/****** Object:  Database [CTSchedule]    Script Date: 2016/6/23 15:09:53 ******/
CREATE DATABASE [CTSchedule]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'CTSchedule', FILENAME = N'F:\开发项目\任务调度服务\TaskScheduler\DB\CTSchedule.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'CTSchedule_log', FILENAME = N'F:\开发项目\任务调度服务\TaskScheduler\DB\CTSchedule_log.ldf' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

ALTER DATABASE [CTSchedule] SET COMPATIBILITY_LEVEL = 100
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CTSchedule].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [CTSchedule] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [CTSchedule] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [CTSchedule] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [CTSchedule] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [CTSchedule] SET ARITHABORT OFF 
GO

ALTER DATABASE [CTSchedule] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [CTSchedule] SET AUTO_CREATE_STATISTICS ON 
GO

ALTER DATABASE [CTSchedule] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [CTSchedule] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [CTSchedule] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [CTSchedule] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [CTSchedule] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [CTSchedule] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [CTSchedule] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [CTSchedule] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [CTSchedule] SET  DISABLE_BROKER 
GO

ALTER DATABASE [CTSchedule] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [CTSchedule] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [CTSchedule] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [CTSchedule] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [CTSchedule] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [CTSchedule] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [CTSchedule] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [CTSchedule] SET RECOVERY FULL 
GO

ALTER DATABASE [CTSchedule] SET  MULTI_USER 
GO

ALTER DATABASE [CTSchedule] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [CTSchedule] SET DB_CHAINING OFF 
GO

ALTER DATABASE [CTSchedule] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [CTSchedule] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO

ALTER DATABASE [CTSchedule] SET  READ_WRITE 
GO


