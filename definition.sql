USE master
GO

print 'disconnect other user sessions.'

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

print 'drop exist database [Definitions].'

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'Definitions')
BEGIN
    DROP DATABASE Definitions
END

print 'get database file drive and directory.'

DECLARE @device_directory nvarchar(520)

SELECT
  @device_directory = substring(filename, 1, charindex(N'master.mdf', lower(filename)) - 1)
FROM 
  master.dbo.sysaltfiles 
WHERE 
  dbid = 1 AND fileid = 1

EXECUTE ('create database Definitions on primary
( name = ''Definitions'', filename = ''' + @device_directory + 'Definitions.mdf'', size = 3328KB, maxsize = unlimited, filegrowth = 1024KB)
log on
( name = ''Definitions_log'', filename = ''' + @device_directory + 'Definitions.ldf'' , size = 1024KB , maxsize = 2048GB , filegrowth = 10%)')
GO


USE [Definitions]
GO
/****** Object:  User [AwGenuser]    Script Date: 5/10/2016 11:59:09 AM ******/
CREATE USER [AwGenuser] FOR LOGIN [AwGenuser] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [ARCHWAY\Domain Users]    Script Date: 5/10/2016 11:59:10 AM ******/
CREATE USER [ARCHWAY\Domain Users] FOR LOGIN [ARCHWAY\Domain Users]
GO
ALTER ROLE [db_owner] ADD MEMBER [AwGenuser]
GO
/****** Object:  Table [dbo].[Page]    Script Date: 5/10/2016 11:59:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Page](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProjectNo] [int] NOT NULL,
	[No] [int] NOT NULL,
	[Name] [nvarchar](100) NULL,
	[PhysicalName] [nvarchar](100) NULL,
	[ServiceUnit] [nvarchar](100) NULL,
	[BusinessUnit] [nvarchar](100) NULL,
	[Priority] [nvarchar](10) NULL,
	[Difficulty] [nvarchar](10) NULL,
	[Status] [nvarchar](100) NULL,
	[Created] [nvarchar](100) NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [nvarchar](100) NULL,
	[UpdateDate] [datetime] NULL,
	[Deleted] [nvarchar](100) NULL,
	[DeleteDate] [datetime] NULL,
	[RowVersion] [timestamp] NULL,
	[JsonVersion] [nvarchar](10) NULL,
	[Json] [nvarchar](max) NULL,
 CONSTRAINT [PK_Page] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PageService]    Script Date: 5/10/2016 11:59:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PageService](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProjectNo] [int] NOT NULL,
	[PageId] [int] NOT NULL,
	[ServiceId] [int] NOT NULL,
	[Created] [nvarchar](100) NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [nvarchar](100) NULL,
	[UpdateDate] [datetime] NULL,
	[Deleted] [nvarchar](100) NULL,
	[DeleteDate] [datetime] NULL,
	[RowVersion] [timestamp] NULL,
 CONSTRAINT [PK_PageService] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Project]    Script Date: 5/10/2016 11:59:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Project](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProjectNo] [int] NOT NULL,
	[No] [int] NOT NULL,
	[Customer] [nvarchar](100) NULL,
	[Name] [nvarchar](100) NULL,
	[PhysicalName] [nvarchar](100) NULL,
	[ServiceUnit] [nvarchar](100) NULL,
	[Memo] [nvarchar](max) NULL,
	[Created] [nvarchar](100) NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [nvarchar](100) NULL,
	[UpdateDate] [datetime] NULL,
	[Deleted] [nvarchar](100) NULL,
	[DeleteDate] [datetime] NULL,
	[RowVersion] [timestamp] NULL,
	[JsonVersion] [nvarchar](10) NULL,
	[Json] [nvarchar](max) NULL,
 CONSTRAINT [PK_Project] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Service]    Script Date: 5/10/2016 11:59:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Service](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProjectNo] [int] NOT NULL,
	[No] [int] NOT NULL,
	[ServiceName] [nvarchar](100) NULL,
	[ProcessName] [nvarchar](100) NULL,
	[ServiceUnit] [nvarchar](100) NULL,
	[Version] [nchar](10) NULL,
	[ServiceClass] [nvarchar](100) NULL,
	[DbContext] [nvarchar](100) NULL,
	[MethodName] [nvarchar](100) NULL,
	[HttpComand] [nchar](10) NULL,
	[Priority] [nvarchar](10) NULL,
	[Difficulty] [nvarchar](10) NULL,
	[Status] [nvarchar](100) NULL,
	[Created] [nvarchar](100) NULL,
	[CreateDate] [datetime] NULL,
	[Updated] [nvarchar](100) NULL,
	[UpdateDate] [datetime] NULL,
	[Deleted] [nvarchar](100) NULL,
	[DeleteDate] [datetime] NULL,
	[RowVersion] [timestamp] NULL,
	[JsonVersion] [nchar](10) NULL,
	[Json] [nvarchar](max) NULL,
 CONSTRAINT [PK_Service] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[Page] ON 

INSERT [dbo].[Page] ([Id], [ProjectNo], [No], [Name], [PhysicalName], [ServiceUnit], [BusinessUnit], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1, 1, 1, N'架電一覧画面', N'CallList', N'ACTOS', N'ACTOS', N'高', N'中', N'2/1', N'h-saitou', CAST(0x0000A59500000000 AS DateTime), N'h-saitou', CAST(0x0000A59500000000 AS DateTime), NULL, NULL, N'1.2.0', NULL)
INSERT [dbo].[Page] ([Id], [ProjectNo], [No], [Name], [PhysicalName], [ServiceUnit], [BusinessUnit], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2, 1, 2, N'架電準備画面', N'CallPreparation', N'ACTOS', N'ACTOS', N'高', N'高', N'2/2', N'h-saitou', CAST(0x0000A59500000000 AS DateTime), N'h-saitou', CAST(0x0000A59500000000 AS DateTime), NULL, NULL, N'1.2.0', NULL)
INSERT [dbo].[Page] ([Id], [ProjectNo], [No], [Name], [PhysicalName], [ServiceUnit], [BusinessUnit], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (3, 1, 3, N'カウンセリング日程設定画面', N'CounselingScheduleSearchList', N'ACTOS', N'ACTOS', N'高', N'高', N'2/2', N'h-saitou', CAST(0x0000A59500000000 AS DateTime), N'h-saitou', CAST(0x0000A59500000000 AS DateTime), NULL, NULL, N'1.2.0', NULL)
INSERT [dbo].[Page] ([Id], [ProjectNo], [No], [Name], [PhysicalName], [ServiceUnit], [BusinessUnit], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (4, 1, 4, N'振分け設定画面', N'ApportionSearch', N'ACTOS', N'ACTOS', N'高', N'高', NULL, N'h-saitou', CAST(0x0000A5A000000000 AS DateTime), N'h-saitou', CAST(0x0000A5A000000000 AS DateTime), NULL, NULL, N'1.2.0', NULL)
INSERT [dbo].[Page] ([Id], [ProjectNo], [No], [Name], [PhysicalName], [ServiceUnit], [BusinessUnit], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (5, 1, 5, N'候補者履歴表示画面', N'StaffHistory', N'ACTOS', N'ACTOS', N'高', N'高', N'2/3', N'h-saitou', CAST(0x0000A5A000000000 AS DateTime), N'h-saitou', CAST(0x0000A5A000000000 AS DateTime), NULL, NULL, N'1.2.0', NULL)
INSERT [dbo].[Page] ([Id], [ProjectNo], [No], [Name], [PhysicalName], [ServiceUnit], [BusinessUnit], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (6, 1, 6, N'確定内容通知画面', N'ConfirmedInfoRegistration', N'ACTOS', N'ACTOS', N'高', N'高', N'2/3', N'h-saitou', CAST(0x0000A5A000000000 AS DateTime), N'h-saitou', CAST(0x0000A5A000000000 AS DateTime), NULL, NULL, N'1.2.0', NULL)
INSERT [dbo].[Page] ([Id], [ProjectNo], [No], [Name], [PhysicalName], [ServiceUnit], [BusinessUnit], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (7, 1, 7, N'こじ開け依頼画面', N'PryRequest', N'ACTOS', N'ACTOS', N'高', N'高', NULL, N'h-saitou', CAST(0x0000A5A000000000 AS DateTime), N'h-saitou', CAST(0x0000A5A000000000 AS DateTime), NULL, NULL, N'1.2.0', NULL)
INSERT [dbo].[Page] ([Id], [ProjectNo], [No], [Name], [PhysicalName], [ServiceUnit], [BusinessUnit], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (8, 1, 8, N'候補者一覧', N'StaffSearch', N'ACTOS', N'ACTOS', N'中', NULL, NULL, N'musuki', CAST(0x0000A5AD00B86BD3 AS DateTime), N'musuki', CAST(0x0000A5AD00B86BD3 AS DateTime), NULL, NULL, N'1.2.0', NULL)
INSERT [dbo].[Page] ([Id], [ProjectNo], [No], [Name], [PhysicalName], [ServiceUnit], [BusinessUnit], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (9, 1, 9, N'こじ開け回答', N'PryAnswer', N'ACTOS', N'ACTOS', N'高', N'中', NULL, N'phson', CAST(0x0000A5B100000000 AS DateTime), N'phson', CAST(0x0000A5B100000000 AS DateTime), NULL, NULL, N'1.2.0', NULL)
INSERT [dbo].[Page] ([Id], [ProjectNo], [No], [Name], [PhysicalName], [ServiceUnit], [BusinessUnit], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (10, 1, 10, N'コンタクト履歴入力', N'ContactHistoryRegist', N'ACTOS', N'ACTOS', N'高', N'高', NULL, N'phson', CAST(0x0000A5B400000000 AS DateTime), N'phson', CAST(0x0000A5AE00000000 AS DateTime), NULL, NULL, N'1.2.0', NULL)
INSERT [dbo].[Page] ([Id], [ProjectNo], [No], [Name], [PhysicalName], [ServiceUnit], [BusinessUnit], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (11, 1, 13, N'受信メール一覧', N'ReceivedMailSearch', N'ACTOS', N'ACTOS', N'中', N'中', NULL, N'hieu', CAST(0x0000A5B600000000 AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Page] ([Id], [ProjectNo], [No], [Name], [PhysicalName], [ServiceUnit], [BusinessUnit], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (12, 1, 14, N'キャンセル/日程変更連絡通知画面', N'CancelChangeInfoRegistration', N'ACTOS', N'ACTOS', N'中ー', N'中ー', NULL, N'phson', CAST(0x0000A5B600000000 AS DateTime), N'phson', CAST(0x0000A5B600000000 AS DateTime), NULL, NULL, N'1.2.0', NULL)
INSERT [dbo].[Page] ([Id], [ProjectNo], [No], [Name], [PhysicalName], [ServiceUnit], [BusinessUnit], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (13, 1, 55, N'メール送信画面', N'SendMail', N'ACTOS', N'ACTOS', N'中
', N'中
', NULL, N'phson', CAST(0x0000A5B700000000 AS DateTime), N'phson', CAST(0x0000A5B700000000 AS DateTime), NULL, NULL, N'1.2.0', NULL)
INSERT [dbo].[Page] ([Id], [ProjectNo], [No], [Name], [PhysicalName], [ServiceUnit], [BusinessUnit], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (14, 1, 60, N'人事組織選択サブ画面', N'PersonelOrgSelection', N'ACTOS', N'ACTOS', NULL, NULL, NULL, N'phson', CAST(0x0000A5C200000000 AS DateTime), N'phson', CAST(0x0000A5C200000000 AS DateTime), NULL, NULL, N'1.2.0', NULL)
INSERT [dbo].[Page] ([Id], [ProjectNo], [No], [Name], [PhysicalName], [ServiceUnit], [BusinessUnit], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (15, 1, 58, N'候補者ステータス選択', N'StaffStatusSelection', N'ACTOS', N'ACTOS', NULL, NULL, NULL, N'phson', CAST(0x0000A5C400000000 AS DateTime), N'phson', CAST(0x0000A5C400000000 AS DateTime), NULL, NULL, N'1.2.0', NULL)
INSERT [dbo].[Page] ([Id], [ProjectNo], [No], [Name], [PhysicalName], [ServiceUnit], [BusinessUnit], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (16, 1, 61, N'CO群選択', N'COSubGroupSelection', N'ACTOS', N'ACTOS', NULL, NULL, NULL, N'phson', CAST(0x0000A5C400000000 AS DateTime), N'phson', CAST(0x0000A5C400000000 AS DateTime), NULL, NULL, N'1.2.0', NULL)
INSERT [dbo].[Page] ([Id], [ProjectNo], [No], [Name], [PhysicalName], [ServiceUnit], [BusinessUnit], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (17, 1, 1, N'メール閲覧', N'BrowseMail', N'ACTOS', N'ACTOS', NULL, NULL, NULL, N'phson', CAST(0x0000A5D800000000 AS DateTime), N'phson', CAST(0x0000A5D800000000 AS DateTime), NULL, NULL, N'1.2.0', NULL)
SET IDENTITY_INSERT [dbo].[Page] OFF
SET IDENTITY_INSERT [dbo].[PageService] ON 

INSERT [dbo].[PageService] ([Id], [ProjectNo], [PageId], [ServiceId], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate]) VALUES (2, 1, 1, 1, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[PageService] ([Id], [ProjectNo], [PageId], [ServiceId], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate]) VALUES (4, 1, 1, 4, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[PageService] ([Id], [ProjectNo], [PageId], [ServiceId], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate]) VALUES (5, 1, 1, 8, NULL, NULL, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[PageService] OFF
SET IDENTITY_INSERT [dbo].[Project] ON 

INSERT [dbo].[Project] ([Id], [ProjectNo], [No], [Customer], [Name], [PhysicalName], [ServiceUnit], [Memo], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1, 1, 1, N'インテリジェンス', N'CSS統合', N'ACTOS', N'Actos', N'メモです', N'h-saitou', CAST(0x0000A59A00000000 AS DateTime), N'h-saitou', CAST(0x0000A59A00000000 AS DateTime), NULL, NULL, N'1.2.0', NULL)
INSERT [dbo].[Project] ([Id], [ProjectNo], [No], [Customer], [Name], [PhysicalName], [ServiceUnit], [Memo], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2, 2, 2, N'インテリジェンス', N'外部入稿', NULL, N'Manusctipt', N'メモです', N'h-saitou', CAST(0x0000A59A00000000 AS DateTime), N'h-saitou', CAST(0x0000A59A00000000 AS DateTime), NULL, NULL, N'1.2.0', NULL)
SET IDENTITY_INSERT [dbo].[Project] OFF
SET IDENTITY_INSERT [dbo].[Service] ON 

INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1, 1, 101, N'呼込み候補者情報', N'架電一覧取得', N'ACTOS', N'V1        ', N'ActosTargetInfoController', N'ActosInfoContext', N'GetCallTargetList', N'GET       ', N'高', N'中', NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (4, 1, 102, N'呼込み候補者情報', N'架電手一覧表示設定取得', N'ACTOS', N'V1        ', N'CallTargetConfigController', N'ActosInfoContext', N'GetCallTargetConfig', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (8, 1, 103, N'候補者実績', N'担当CO更新', N'StaffManagement', N'V1        ', N'StaffActualInfoController', N'StaffActualInfoContext', N'UpdateResponsibleCO', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (9, 1, 104, N'ACTOS候補者情報取得', N'ACTOS候補者情報取得', N'ACTOS', N'V1        ', N'ActosTargetInfoController', N'ActosInfoContext', N'GetTargetInfo', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (10, 1, 105, N'カウンセリングポリシー設定', N'カウンセリング設定ポリシー取得', N'ACTOS', N'V1        ', N'CounselingPolicyController', N'ActosInfoContext', N'GetPolicyforGroup', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (11, 1, 106, N'ブース', N'拠点別ブース情報取得', N'ACTOS', N'V1        ', N'BoothMasterController', N'ActosInfoContext', N'GetBoothByBranch', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (12, 1, 107, N'カレンダー', N'営業日カレンダー取得', N'ACTOS', N'V1        ', N'BusinessCalendarController', N'ActosInfoContext', N'GetBusinessCalendar', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (14, 1, 108, N'CO情報', N'CO群所属CO検索', N'ACTOS', N'V1        ', N'COInformationController', N'ActosInfoContext', N'GetCOfromSubGroup', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (15, 1, 109, N'CO群情報', N'拠点CO群マッピング結果取得', N'ACTOS', N'V1        ', N'COSubGroupController', N'ActosInfoContext', N'GetSubGroupByBranch', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (16, 1, 110, N'スケジュール', N'スケジュール登録', N'ACTOS', N'V1        ', N'ActosScheduleInfoController', N'ActosInfoContext', N'InsertSchedule', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (17, 1, 111, N'スケジュール', N'スケジュール削除', N'ACTOS', N'V1        ', N'ActosScheduleInfoController', N'ActosInfoContext', N'DeleteSchedule', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (18, 1, 112, N'スケジュール', N'空きスケジュール検索', N'ACTOS', N'V1        ', N'ActosScheduleInfoController', N'ActosInfoContext', N'SearchSchedule', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (19, 1, 113, N'カウンセリングポリシー設定', N'カウンセリング区分情報取得', N'ACTOS', N'V1        ', N'CounselingPolicyController', N'ActosInfoContext', N'GetCounselingCategory', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (20, 1, 114, N'呼込み候補者情報', N'架電一覧取得件数取得', N'ACTOS', N'V1        ', N'ActosTargetInfoController', N'ActosInfoContext', N'GetCountCallTarget', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (21, 1, 115, N'呼込み候補者情報', N'呼込みステータス更新
', N'ACTOS', N'V1        ', N'AttractTargetInfoController', N'ActosInfoContext', N'UpdateTrackingStatus', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (23, 1, 116, N'メール', N'メール送信', N'ACTOS', N'V1        ', N'ActosMailController', N'ActosInfoContext', N'ActosSendMail', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (24, 1, 117, N'メールパターン', N'メールパターン検索', N'ACTOS', N'V1        ', N'MailPatternController', N'ActosInfoContext', N'GetByMailPatternID', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (25, 1, 118, N'コンタクト履歴既定値マスタ', N'コンタクト履歴既定値取得', N'ACTOS', N'V1        ', N'DefaultContactHistoryController', N'ActosInfoContext', N'GetContentByID', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (26, 1, 119, N'コンタクト履歴', N'コンタクト履歴登録', N'ACTOS', N'V1        ', N'ContactHistoryController', N'ActosInfoContext', N'InsertContactHistory', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (28, 1, 120, N'ACTOS個別振分け', N'再振分け先判定', N'ACTOS', N'V1        ', N'ActosAllocateController', N'ActosInfoContext', N'GetReallocateInfo', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (29, 1, 121, N'ACTOS個別振分け', N'候補者再振分け登録', N'ACTOS', N'V1        ', N'ActosAllocateController', N'ActosInfoContext', N'SaveAllocateInfo', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (30, 1, 122, N'候補者振分け履歴', N'候補者振分け履歴登録', N'StaffManagement', N'V1        ', N'StaffAllocationHistoryController', N'StaffAllocationHistoryContext', N'POST', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (31, 1, 123, N'候補者実績', N'候補者ステータス更新（抹消）', N'StaffManagement', N'V1        ', N'StaffActualInfoController', N'StaffActualInfoContext', N'UpdateStatusErase', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (32, 1, 124, N'呼込み候補者情報', N'ACTOS候補者一覧検索', N'ACTOS', N'V1        ', N'ActosTargetInfoController', N'ActosInfoContext', N'GetTargetList', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1022, 1, 125, N'呼込み候補者情報', NULL, N'ACTOS', N'V1        ', N'ActosTargetInfoController', N'ActosInfoContext', N'GetTotalCountTargetList', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1023, 1, 126, N'CTS汎用マスタ', N'汎用マスター取得', N'ACTOS', N'V1        ', N'CTSCommonCodeController', N'ActosInfoContext', N'GetCtsList', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1024, 1, 127, N'ACTOS汎用マスタ', N'ACTOS汎用マスタ取得', N'ACTOS', N'V1        ', N'ActosCommonCodeController', N'ActosInfoContext', N'GetActosCommonCode', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1025, 1, 128, NULL, NULL, N'ACTOS', N'V1        ', N'Branch', N'ActosInfoContext', N'GET', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1026, 1, 129, NULL, NULL, N'ACTOS', N'V1        ', N'COInformationController', N'ActosInfoContext', N'GetCOFromBranch', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1027, 1, 130, NULL, NULL, N'ACTOS', N'V1        ', N'VirtualAgentController', N'ActosInfoContext', N'GetAllocateDefaultAgent', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1030, 1, 136, N'候補者履歴
', N'候補者履歴検索（全て）', N'ACTOS', N'V1        ', N'StaffHistoryController', N'ActosInfoContext', N'GetStaffAllHistory', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1031, 1, 132, N'候補者履歴', N'候補者履歴検索（カウント）
', N'ACTOS', N'V1        ', N'StaffHistoryController
', N'ActosInfoContext', N'GetCountStaffAllHistory', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1034, 1, 133, N'CO情報', N'こじ開けメール送付先取得', N'ACTOS', N'V1        ', N'COInformationController', N'ActosInfoContext', N'GetPryMailAddress', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1035, 1, 134, N'呼込み候補者情報', N'一覧取得', N'ACTOS', N'V1        ', N'AttractingStaffController', N'ActosInfoContext', N'Search', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1036, 1, 135, N'呼込み候補者情報', N'一覧件数取得', N'ACTOS', N'V1        ', N'AttractingStaffController', N'ActosInfoContext', N'Count', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1037, 1, 1, N'カウンセリング希望日時情報', N'カウンセリング希望日時登録', N'ACTOS', N'V1        ', N'CounselingDemandController', N'ActosInfoContext', N'UpdateCounselingDemandDate', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1039, 1, 137, N'コンタクト履歴', N'コンタクト履歴登録', N'ACTOS', N'V1        ', N'ContactHistoryController', N'ActosInfoContext', N'Post', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1040, 1, 138, N'呼込み候補者情報', N'候補者情報取得', N'ACTOS', N'V1        ', N'AttractingStaffController', N'ActosInfoContext', N'Get', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1041, 1, 139, N'ACTOSメール', N'ACTOS受信メール件数取得', N'ACTOS', N'V1        ', N'ActosMailController', N'ActosInfoContext', N'Count', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1042, 1, 140, N'ACTOSメール', N'ACTOS受信メール検索', N'ACTOS', N'V1        ', N'ActosMailController', N'ActosInfoContext', N'Search', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1043, 1, 167, N'呼び込み候補者情報', N'呼込み情報更新(コンタクト履歴)', N'ACTOS', N'V1        ', N'AttractingStaffStatusController', N'ActosInfoContext', N'SaveContactHistory', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1044, 1, 142, N'コンタクト履歴', N'コンタクト履歴登録(メール)', N'ACTOS', N'V1        ', N'ContactHistoryController', N'ActosInfoContext', N'SaveMail', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1045, 1, 143, N'ACTOSメール', N'ACTOS受信メール添付ファイル取得', N'ACTOS', N'V1        ', N'ActosMailController', N'ActosInfoContext', N'Download', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1046, 1, 163, N'ACTOSメール', N'ACTOS受信メール取得', N'ACTOS', N'V1        ', N'ActosMailController', N'ActosInfoContext', N'Get', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1047, 1, 162, N'ACTOSメール', N'ACTOS受信メール更新', N'ACTOS', N'V1        ', N'ActosMailController', N'ActosInfoContext', N'Save', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1048, 1, 85, N'メール送付先マスタ', N'メール送付先検索', N'ACTOS', N'V1        ', N'MailAddressMasterController', N'MasterContext', N'GetByID', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1049, 1, 169, N'コンタクト履歴 ', N'コンタクト履歴登録(コンタクト履歴明細)', N'ACTOS', N'V1        ', N'ContactHistoryController', N'ActosInfoContext', N'SaveDetail', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1050, 1, 165, N'呼込み候補者情報', N'呼込みステータス更新', N'ACTOS', N'V1        ', N'AttractingStaffStatusController', N'ActosInfoContext', N'Save', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1051, 1, 97, N'架電一覧表示設定', N'架電一覧表示設定取得', N'ACTOS', N'V1        ', N'AttractingConfigController', N'ActosInfoContext', N'Get', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (1052, 1, 96, N'架電一覧表示設定', N'架電一覧表示設定更新', N'ACTOS', N'V1        ', N'AttractingConfigController', N'ActosInfoContext', N'Post', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2052, 1, 37, N'呼込み候補者情報', N'架電一覧取得', N'ACTOS', N'V1        ', N'AttractingStaffController', N'ActosInfoContext', N'ListDial', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2053, 1, 172, N'呼込み候補者情報', N'架電一覧件数取得', N'ACTOS', N'V1        ', N'AttractingStaffController', N'ActosInfoContext', N'CountDial', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2054, 1, 71, N'候補者実績', N'担当CO更新', N'ACTOS', N'V1        ', N'StaffResultController', N'ActosInfoContext', N'UpdateResponsibleCO', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2056, 1, 1, N'拠点情報取得', N'取得', N'ACTOS', N'V1        ', N'BranchController', N'ActosInfoContext', N'Get', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2057, 1, 1, N'カウンセリング設定情報', N'取得', N'ACTOS', N'V1        ', N'ActosGroupCounselingPolicyController', N'ActosInfoContext', N'Get', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2058, 1, 1, N'営業日カレンダー', N'取得', N'ACTOS', N'V1        ', N'BusinessCalendarController', N'MasterContext', N'Get', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2061, 1, 1, N'カウンセリング区分情報', N'取得', N'ACTOS', N'V1        ', N'ActosGroupCounselingTypeController', N'ActosInfoContext', N'Get', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2062, 1, 1, N'CO群拠点マッピング', N'取得', N'ACTOS', N'V1        ', N'CoGroupBranchMapController', N'ActosInfoContext', N'Get', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2063, 1, 58, N'呼込み候補者情報', N'手動振分け対象検索', N'ACTOS', N'V1        ', N'AttractingStaffController', N'ActosInfoContext', N'GetTargetList', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2064, 1, 1, N'コンタクト履歴規定値', N'取得', N'ACTOS', N'V1        ', N'ContactHistoryDefaultValueController', N'ActosInfoContext', N'Get', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2065, 1, 1, N'呼込みセンター', N'取得', N'ACTOS', N'V1        ', N'CenterController', N'ActosInfoContext', N'Get', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2066, 1, 176, N'呼込み候補者情報', N'手動振分け対象件数取得', N'ACTOS', N'V1        ', N'AttractingStaffController', N'ActosInfoContext', N'CountTargetList', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2067, 1, 179, N'拠点', N'一覧取得', N'ACTOS', N'V1        ', N'BranchController', N'ActosInfoContext', N'List', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2068, 1, 1, N'メールテンプレート', N'検索', N'ACTOS', N'V1        ', N'MailTemplateController', N'ActosInfoContext', N'Search', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2069, 1, 175, N'振分け情報', N'手動振分け登録', N'ACTOS', N'V1        ', N'AllocationInfoController', N'ActosInfoContext', N'SaveAllocationInfo', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2070, 1, 181, N'呼込み候補者情報
', N'ステータス更新(抹消)
', N'ACTOS', N'V1        ', N'AllocationInfoController', N'ActosInfoContext', N'SaveStatusErase', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2071, 1, 180, N'振分け情報
', N'再振分け登録
', N'ACTOS', N'V1        ', N'AllocationInfoController', N'ActosInfoContext', N'SaveReAllocationInfo', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2072, 1, 1, N'メール', N'送信', N'ACTOS', N'V1        ', N'MailController', N'ActosInfoContext', N'Send', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2073, 1, 1, N'呼込み候補者', N'更新(コンタクトヒストリー)', N'ACTOS', N'V1        ', N'AttractingStaffController', N'ActosInfoContext', N'SaveContactHistory', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2074, 1, 1, N'既定担当者', N'取得', N'ACTOS', N'V1        ', N'DefaultResponserController', N'ActosInfoContext', N'List', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2075, 1, 1, N'候補者ステータスマスタ', N'一覧取得', N'ACTOS', N'V1        ', N'StaffStatusMasterController', N'MasterContext', N'List', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2076, 1, 168, N'CTS汎用マスタ', N'汎用マスター検索', N'ACTOS', N'V1        ', N'CtsCommonCodeController', N'MasterContext', N'Search', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2077, 1, 172, N'CO群マスタ', N'CO群マスタ検索', N'ACTOS', N'V1        ', N'COSubGroupController', N'ActosInfoContext', N'Search', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2078, 1, 171, N'人事組織マスタ', N'人事組織マスタ検索', N'ACTOS', N'V1        ', N'PersonnelOrgController', N'ActosInfoContext', N'Search', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2079, 1, 174, N'社員情報', N'社員情報検索', N'ACTOS', N'V1        ', N'ActosEmployeeController', N'ActosInfoContext', N'Search', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2080, 1, 1, NULL, NULL, N'ACTOS', N'V1        ', N'AttractingStaffController', N'ActosInfoContext', N'SaveUnconfirmed', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2081, 1, 1, NULL, NULL, N'ACTOS', N'V1        ', N'AttractingStaffController', N'ActosInfoContext', N'Release', N'POST      ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[Service] ([Id], [ProjectNo], [No], [ServiceName], [ProcessName], [ServiceUnit], [Version], [ServiceClass], [DbContext], [MethodName], [HttpComand], [Priority], [Difficulty], [Status], [Created], [CreateDate], [Updated], [UpdateDate], [Deleted], [DeleteDate], [JsonVersion], [Json]) VALUES (2082, 1, 194, N'コンタクト履歴', N'コンタクト履歴明細取得', N'ACTOS', N'V1        ', N'ContactHistoryController', N'ActosInfoContext', N'GetDetail', N'GET       ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'1.2.0     ', NULL)
SET IDENTITY_INSERT [dbo].[Service] OFF
ALTER TABLE [dbo].[PageService]  WITH CHECK ADD  CONSTRAINT [FK_PageService_Page] FOREIGN KEY([PageId])
REFERENCES [dbo].[Page] ([Id])
GO
ALTER TABLE [dbo].[PageService] CHECK CONSTRAINT [FK_PageService_Page]
GO
ALTER TABLE [dbo].[PageService]  WITH CHECK ADD  CONSTRAINT [FK_PageService_Service] FOREIGN KEY([ServiceId])
REFERENCES [dbo].[Service] ([Id])
GO
ALTER TABLE [dbo].[PageService] CHECK CONSTRAINT [FK_PageService_Service]
GO

