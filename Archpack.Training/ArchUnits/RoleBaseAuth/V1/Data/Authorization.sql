USE [master]
GO

DECLARE @proc smallint
DECLARE sysproc_cur CURSOR FAST_FORWARD FOR
 SELECT spid FROM master..sysprocesses WITH(NOLOCK)
OPEN sysproc_cur
FETCH NEXT FROM sysproc_cur INTO @proc
WHILE (@@FETCH_STATUS <> -1)
BEGIN
   EXEC('KILL ' + @proc)
   FETCH NEXT FROM sysproc_cur INTO @proc
END
CLOSE sysproc_cur
DEALLOCATE sysproc_cur

declare @db_name varchar(255)
SET @db_name = 'Authorization'
 
IF exists (select name from sys.databases where name = @db_name)
BEGIN
    execute('drop database ' + @db_name)
END

declare @device_directory nvarchar(520)
select @device_directory = substring(filename, 1, charindex(N'master.mdf', lower(filename)) - 1)
from master.dbo.sysaltfiles 
where dbid = 1 AND fileid = 1
 
execute ('create database [' + @db_name + '] on primary
( name = ''' + @db_name + ''', filename = ''' + @device_directory + @db_name + '.mdf'', size = 5120KB, maxsize = unlimited, filegrowth = 1024KB)
log on
( name = ''' + @db_name + '_log'', filename = ''' + @device_directory + @db_name + '_log.ldf'' , size = 1024KB , maxsize = 2048GB , filegrowth = 10%)')
GO

ALTER DATABASE [Authorization] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Authorization].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Authorization] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Authorization] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Authorization] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Authorization] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Authorization] SET ARITHABORT OFF 
GO
ALTER DATABASE [Authorization] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Authorization] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [Authorization] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Authorization] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Authorization] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Authorization] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Authorization] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Authorization] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Authorization] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Authorization] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Authorization] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Authorization] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Authorization] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Authorization] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Authorization] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Authorization] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Authorization] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Authorization] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Authorization] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Authorization] SET  MULTI_USER 
GO
ALTER DATABASE [Authorization] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Authorization] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Authorization] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Authorization] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [Authorization]
GO
/****** Object:  Table [dbo].[Permissions]    Script Date: 7/9/2015 9:56:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permissions](
	[PermissionID] [int] IDENTITY(1,1) NOT NULL,
	[PermissionName] [nvarchar](255) NOT NULL,
	[URL] [nvarchar](1024) NOT NULL,
	[CreatedDate] [datetimeoffset](7) NOT NULL,
	[CreatedUser] [nvarchar](255) NOT NULL,
	[UpdatedDate] [datetimeoffset](7) NOT NULL,
	[UpdatedUser] [nvarchar](255) NOT NULL,
	[version] [timestamp] NOT NULL,
 CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED 
(
	[PermissionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RoleInRoles]    Script Date: 7/9/2015 9:56:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleInRoles](
	[RoleID] [int] NOT NULL,
	[ParentRoleID] [int] NOT NULL,
	[CreatedDate] [datetimeoffset](7) NOT NULL,
	[CreatedUser] [nvarchar](255) NOT NULL,
	[UpdatedDate] [datetimeoffset](7) NOT NULL,
	[UpdatedUser] [nvarchar](255) NOT NULL,
	[version] [timestamp] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RolePermissions]    Script Date: 7/9/2015 9:56:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RolePermissions](
	[RoleID] [int] NOT NULL,
	[PermissionID] [int] NOT NULL,
	[CreatedDate] [datetimeoffset](7) NOT NULL,
	[CreatedUser] [nvarchar](255) NOT NULL,
	[UpdatedDate] [datetimeoffset](7) NOT NULL,
	[UpdatedUser] [nvarchar](255) NOT NULL,
	[version] [timestamp] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Roles]    Script Date: 7/9/2015 9:56:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](255) NOT NULL,
	[CreatedDate] [datetimeoffset](7) NOT NULL,
	[CreatedUser] [nvarchar](255) NOT NULL,
	[UpdatedDate] [datetimeoffset](7) NOT NULL,
	[UpdatedUser] [nvarchar](255) NOT NULL,
	[version] [timestamp] NOT NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 7/9/2015 9:56:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoles](
	[UserID] [nvarchar](255) NOT NULL,
	[RoleID] [int] NOT NULL,
	[CreatedDate] [datetimeoffset](7) NOT NULL,
	[CreatedUser] [nvarchar](255) NOT NULL,
	[UpdatedDate] [datetimeoffset](7) NOT NULL,
	[UpdatedUser] [nvarchar](255) NOT NULL,
	[version] [timestamp] NOT NULL
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Permissions] ADD  CONSTRAINT [DF_Permissions_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Permissions] ADD  CONSTRAINT [DF_Permissions_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [dbo].[RoleInRoles] ADD  CONSTRAINT [DF_RoleInRoles_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[RoleInRoles] ADD  CONSTRAINT [DF_RoleInRoles_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [dbo].[RolePermissions] ADD  CONSTRAINT [DF_RolePermissions_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[RolePermissions] ADD  CONSTRAINT [DF_RolePermissions_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [dbo].[Roles] ADD  CONSTRAINT [DF_Roles_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Roles] ADD  CONSTRAINT [DF_Roles_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [dbo].[UserRoles] ADD  CONSTRAINT [DF_UserRoles_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[UserRoles] ADD  CONSTRAINT [DF_UserRoles_UpdatedDate]  DEFAULT (getdate()) FOR [UpdatedDate]
GO
ALTER TABLE [dbo].[RoleInRoles]  WITH CHECK ADD  CONSTRAINT [FK_RoleInRoles_Roles] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([RoleID])
GO
ALTER TABLE [dbo].[RoleInRoles] CHECK CONSTRAINT [FK_RoleInRoles_Roles]
GO
ALTER TABLE [dbo].[RoleInRoles]  WITH CHECK ADD  CONSTRAINT [FK_RoleInRoles_Roles_ParentRole] FOREIGN KEY([ParentRoleID])
REFERENCES [dbo].[Roles] ([RoleID])
GO
ALTER TABLE [dbo].[RoleInRoles] CHECK CONSTRAINT [FK_RoleInRoles_Roles_ParentRole]
GO
ALTER TABLE [dbo].[RolePermissions]  WITH CHECK ADD  CONSTRAINT [FK_RolePermissions_Permissions] FOREIGN KEY([PermissionID])
REFERENCES [dbo].[Permissions] ([PermissionID])
GO
ALTER TABLE [dbo].[RolePermissions] CHECK CONSTRAINT [FK_RolePermissions_Permissions]
GO
ALTER TABLE [dbo].[RolePermissions]  WITH CHECK ADD  CONSTRAINT [FK_RolePermissions_Roles] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([RoleID])
GO
ALTER TABLE [dbo].[RolePermissions] CHECK CONSTRAINT [FK_RolePermissions_Roles]
GO
ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD  CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([RoleID])
GO
ALTER TABLE [dbo].[UserRoles] CHECK CONSTRAINT [FK_UserRoles_Roles]
GO
USE [master]
GO
ALTER DATABASE [Authorization] SET  READ_WRITE 
GO
