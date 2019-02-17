USE [master]
GO
/****** Object:  Database [GAMEX]    Script Date: 2/17/2019 5:22:02 PM ******/
CREATE DATABASE [GAMEX]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'GAMEX', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\GAMEX.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'GAMEX_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\GAMEX_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [GAMEX] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [GAMEX].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [GAMEX] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [GAMEX] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [GAMEX] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [GAMEX] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [GAMEX] SET ARITHABORT OFF 
GO
ALTER DATABASE [GAMEX] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [GAMEX] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [GAMEX] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [GAMEX] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [GAMEX] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [GAMEX] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [GAMEX] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [GAMEX] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [GAMEX] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [GAMEX] SET  ENABLE_BROKER 
GO
ALTER DATABASE [GAMEX] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [GAMEX] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [GAMEX] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [GAMEX] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [GAMEX] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [GAMEX] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [GAMEX] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [GAMEX] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [GAMEX] SET  MULTI_USER 
GO
ALTER DATABASE [GAMEX] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [GAMEX] SET DB_CHAINING OFF 
GO
ALTER DATABASE [GAMEX] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [GAMEX] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [GAMEX] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [GAMEX] SET QUERY_STORE = OFF
GO
USE [GAMEX]
GO
/****** Object:  Table [dbo].[AccountBookmark]    Script Date: 2/17/2019 5:22:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountBookmark](
	[AccountId] [nvarchar](128) NOT NULL,
	[BookmarkDate] [datetime] NOT NULL,
	[AccountBookmark] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_AccountBookmark] PRIMARY KEY CLUSTERED 
(
	[AccountId] ASC,
	[AccountBookmark] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AccountStatus]    Script Date: 2/17/2019 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountStatus](
	[StatusId] [int] IDENTITY(1,1) NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_AccountStatus] PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ActivityHistory]    Script Date: 2/17/2019 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActivityHistory](
	[ActivityId] [int] IDENTITY(1,1) NOT NULL,
	[AccountId] [nvarchar](128) NOT NULL,
	[Activity] [nvarchar](100) NOT NULL,
	[Time] [datetime] NOT NULL,
 CONSTRAINT [PK_ActivityHistory] PRIMARY KEY CLUSTERED 
(
	[ActivityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Agenda]    Script Date: 2/17/2019 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Agenda](
	[AgendaId] [int] IDENTITY(1,1) NOT NULL,
	[ExhibitionId] [int] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[Activity] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Agenda] PRIMARY KEY CLUSTERED 
(
	[AgendaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 2/17/2019 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 2/17/2019 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 2/17/2019 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 2/17/2019 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 2/17/2019 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[Hometown] [nvarchar](max) NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Point] [int] NOT NULL,
	[TotalPointEarned] [int] NOT NULL,
	[CompanyId] [int] NULL,
	[QR] [varchar](50) NULL,
	[StatusId] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Booth]    Script Date: 2/17/2019 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Booth](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[QR] [varchar](50) NULL,
	[Booth] [int] NOT NULL,
	[CompanyId] [int] NOT NULL,
	[ExhibitionId] [int] NOT NULL,
 CONSTRAINT [PK_Booth] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Company]    Script Date: 2/17/2019 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Company](
	[CompanyId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[Email] [nvarchar](256) NOT NULL,
	[Phone] [varchar](20) NULL,
	[Address] [nvarchar](200) NULL,
	[Location] [geography] NULL,
	[Logo] [nvarchar](50) NULL,
	[QR] [varchar](50) NULL,
	[Website] [varchar](50) NULL,
	[StatusId] [int] NOT NULL,
	[TaxNumber] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CompanyBookmark]    Script Date: 2/17/2019 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CompanyBookmark](
	[AccountId] [nvarchar](128) NOT NULL,
	[BookmarkDate] [datetime] NOT NULL,
	[CompanyBookmark] [int] NOT NULL,
 CONSTRAINT [PK_CompanyBookmark] PRIMARY KEY CLUSTERED 
(
	[AccountId] ASC,
	[CompanyBookmark] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CompanyStatus]    Script Date: 2/17/2019 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CompanyStatus](
	[StatusId] [int] IDENTITY(1,1) NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_CompanyStatus] PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Exhibition]    Script Date: 2/17/2019 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Exhibition](
	[ExhibitionId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[Address] [nvarchar](200) NULL,
	[OrganizerId] [nvarchar](128) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[Location] [geography] NULL,
	[Logo] [varchar](50) NOT NULL,
	[QR] [varchar](50) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Exhibition] PRIMARY KEY CLUSTERED 
(
	[ExhibitionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExhibitionAttendee]    Script Date: 2/17/2019 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExhibitionAttendee](
	[ExhibitionId] [int] NOT NULL,
	[AccountId] [nvarchar](128) NOT NULL,
	[BookmarkTime] [datetime] NOT NULL,
	[CheckinTime] [datetime] NULL,
 CONSTRAINT [PK_ExhibitionAttendee] PRIMARY KEY CLUSTERED 
(
	[ExhibitionId] ASC,
	[AccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProposedAnswer]    Script Date: 2/17/2019 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProposedAnswer](
	[ProposedAnswerId] [int] IDENTITY(1,1) NOT NULL,
	[QuestionId] [int] NOT NULL,
	[Content] [nvarchar](100) NULL,
 CONSTRAINT [PK_ProposedAnswer] PRIMARY KEY CLUSTERED 
(
	[ProposedAnswerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Question]    Script Date: 2/17/2019 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Question](
	[QuestionId] [int] IDENTITY(1,1) NOT NULL,
	[Content] [nvarchar](1000) NOT NULL,
	[SurveyId] [int] NOT NULL,
	[QuestionNo] [int] NOT NULL,
	[QuestionType] [int] NOT NULL,
 CONSTRAINT [PK_Question] PRIMARY KEY CLUSTERED 
(
	[QuestionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reward]    Script Date: 2/17/2019 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reward](
	[RewardId] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](50) NULL,
	[Content] [nvarchar](50) NOT NULL,
	[Quantity] [int] NOT NULL,
	[PointCost] [int] NOT NULL,
	[CreatedBy] [nvarchar](128) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndtDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Reward] PRIMARY KEY CLUSTERED 
(
	[RewardId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RewardHistory]    Script Date: 2/17/2019 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RewardHistory](
	[RewardId] [int] NOT NULL,
	[ExchangedDate] [datetime] NOT NULL,
	[AccountId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_RewardHistory] PRIMARY KEY CLUSTERED 
(
	[RewardId] ASC,
	[AccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Survey]    Script Date: 2/17/2019 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Survey](
	[SurveyId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [varchar](50) NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[Point] [int] NOT NULL,
	[AccountId] [nvarchar](128) NOT NULL,
	[CompanyId] [int] NULL,
	[ExhibitionId] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Survey] PRIMARY KEY CLUSTERED 
(
	[SurveyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SurveyAnswer]    Script Date: 2/17/2019 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SurveyAnswer](
	[QuestionId] [int] NOT NULL,
	[ProposedAnswerId] [int] NOT NULL,
	[AccountId] [nvarchar](128) NOT NULL,
	[SurveyId] [int] NOT NULL,
	[Other] [nvarchar](100) NULL,
 CONSTRAINT [PK_SurveyAnswer] PRIMARY KEY CLUSTERED 
(
	[QuestionId] ASC,
	[ProposedAnswerId] ASC,
	[AccountId] ASC,
	[SurveyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SurveyParticipation]    Script Date: 2/17/2019 5:22:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SurveyParticipation](
	[AccountId] [nvarchar](128) NOT NULL,
	[SurveyId] [int] NOT NULL,
	[CompleteDate] [datetime] NOT NULL,
 CONSTRAINT [PK_SurveyParticipation] PRIMARY KEY CLUSTERED 
(
	[AccountId] ASC,
	[SurveyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[AccountStatus] ON 

INSERT [dbo].[AccountStatus] ([StatusId], [Status]) VALUES (1, N'Activated')
SET IDENTITY_INSERT [dbo].[AccountStatus] OFF
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'5a101ef4-b073-4538-be24-ca75475e77d6', N'Company')
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'81c8eb35-cf2f-4d0f-ac60-77f22f2d38bd', N'User')
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'e5c3eff3-28f3-4636-a859-a6bd68afe25e', N'Admin')
INSERT [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'81cca029-7a8e-4c39-8e88-42408b79ada9', N'e5c3eff3-28f3-4636-a859-a6bd68afe25e')
INSERT [dbo].[AspNetUsers] ([Id], [Hometown], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName], [FirstName], [LastName], [Point], [TotalPointEarned], [CompanyId], [QR], [StatusId]) VALUES (N'81cca029-7a8e-4c39-8e88-42408b79ada9', NULL, N'ttn2101@gmail.com', 0, N'AByFbF+iu+WNnkT9oIAtUOwXfnVg2SQ4bZz5WhVdnVGvQiwIs3pSrRd6D0rE/OC0kw==', N'2f7c6e58-4d57-452d-af7a-bb9208b6d65b', NULL, 0, 0, CAST(N'2019-02-09T15:52:33.433' AS DateTime), 1, 0, N'nhanthien211', N'Thiện Nhân', N'Trần', 0, 0, NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[Company] ON 

INSERT [dbo].[Company] ([CompanyId], [Name], [Description], [Email], [Phone], [Address], [Location], [Logo], [QR], [Website], [StatusId], [TaxNumber]) VALUES (1, N'CÔNG TY TNHH VITTO - VP', NULL, N'vitto@mailprotech.com', NULL, NULL, NULL, NULL, NULL, NULL, 1, N'TN001')
INSERT [dbo].[Company] ([CompanyId], [Name], [Description], [Email], [Phone], [Address], [Location], [Logo], [QR], [Website], [StatusId], [TaxNumber]) VALUES (2, N'CÔNG TY CỔ PHẦN VIET TILES', NULL, N'viettiles@mailprotech.com', NULL, NULL, NULL, NULL, NULL, NULL, 1, N'TN002')
SET IDENTITY_INSERT [dbo].[Company] OFF
SET IDENTITY_INSERT [dbo].[CompanyStatus] ON 

INSERT [dbo].[CompanyStatus] ([StatusId], [Status]) VALUES (1, N'Pending')
INSERT [dbo].[CompanyStatus] ([StatusId], [Status]) VALUES (2, N'Activated')
INSERT [dbo].[CompanyStatus] ([StatusId], [Status]) VALUES (3, N'Deactivated')
SET IDENTITY_INSERT [dbo].[CompanyStatus] OFF
ALTER TABLE [dbo].[AccountBookmark]  WITH CHECK ADD  CONSTRAINT [FK_AccountBookmark_AspNetUsers_1] FOREIGN KEY([AccountId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[AccountBookmark] CHECK CONSTRAINT [FK_AccountBookmark_AspNetUsers_1]
GO
ALTER TABLE [dbo].[AccountBookmark]  WITH CHECK ADD  CONSTRAINT [FK_AccountBookmark_AspNetUsers_2] FOREIGN KEY([AccountBookmark])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[AccountBookmark] CHECK CONSTRAINT [FK_AccountBookmark_AspNetUsers_2]
GO
ALTER TABLE [dbo].[ActivityHistory]  WITH CHECK ADD  CONSTRAINT [FK_ActivityHistory_AspNetUsers] FOREIGN KEY([AccountId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[ActivityHistory] CHECK CONSTRAINT [FK_ActivityHistory_AspNetUsers]
GO
ALTER TABLE [dbo].[Agenda]  WITH CHECK ADD  CONSTRAINT [FK_Agenda_Exhibition] FOREIGN KEY([ExhibitionId])
REFERENCES [dbo].[Exhibition] ([ExhibitionId])
GO
ALTER TABLE [dbo].[Agenda] CHECK CONSTRAINT [FK_Agenda_Exhibition]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers]
GO
ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUsers_AccountStatus] FOREIGN KEY([StatusId])
REFERENCES [dbo].[AccountStatus] ([StatusId])
GO
ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_AspNetUsers_AccountStatus]
GO
ALTER TABLE [dbo].[AspNetUsers]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUsers_Company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Company] ([CompanyId])
GO
ALTER TABLE [dbo].[AspNetUsers] CHECK CONSTRAINT [FK_AspNetUsers_Company]
GO
ALTER TABLE [dbo].[Booth]  WITH CHECK ADD  CONSTRAINT [FK_Booth_Company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Company] ([CompanyId])
GO
ALTER TABLE [dbo].[Booth] CHECK CONSTRAINT [FK_Booth_Company]
GO
ALTER TABLE [dbo].[Booth]  WITH CHECK ADD  CONSTRAINT [FK_Booth_Exhibition] FOREIGN KEY([ExhibitionId])
REFERENCES [dbo].[Exhibition] ([ExhibitionId])
GO
ALTER TABLE [dbo].[Booth] CHECK CONSTRAINT [FK_Booth_Exhibition]
GO
ALTER TABLE [dbo].[Company]  WITH CHECK ADD  CONSTRAINT [FK_Company_CompanyStatus] FOREIGN KEY([StatusId])
REFERENCES [dbo].[CompanyStatus] ([StatusId])
GO
ALTER TABLE [dbo].[Company] CHECK CONSTRAINT [FK_Company_CompanyStatus]
GO
ALTER TABLE [dbo].[CompanyBookmark]  WITH CHECK ADD  CONSTRAINT [FK_CompanyBookmark_AspNetUsers] FOREIGN KEY([AccountId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[CompanyBookmark] CHECK CONSTRAINT [FK_CompanyBookmark_AspNetUsers]
GO
ALTER TABLE [dbo].[CompanyBookmark]  WITH CHECK ADD  CONSTRAINT [FK_CompanyBookmark_Company] FOREIGN KEY([CompanyBookmark])
REFERENCES [dbo].[Company] ([CompanyId])
GO
ALTER TABLE [dbo].[CompanyBookmark] CHECK CONSTRAINT [FK_CompanyBookmark_Company]
GO
ALTER TABLE [dbo].[Exhibition]  WITH CHECK ADD  CONSTRAINT [FK_Exhibition_AspNetUsers] FOREIGN KEY([OrganizerId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Exhibition] CHECK CONSTRAINT [FK_Exhibition_AspNetUsers]
GO
ALTER TABLE [dbo].[ExhibitionAttendee]  WITH CHECK ADD  CONSTRAINT [FK_ExhibitionAttendee_AspNetUsers] FOREIGN KEY([AccountId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[ExhibitionAttendee] CHECK CONSTRAINT [FK_ExhibitionAttendee_AspNetUsers]
GO
ALTER TABLE [dbo].[ExhibitionAttendee]  WITH CHECK ADD  CONSTRAINT [FK_ExhibitionAttendee_Exhibitio] FOREIGN KEY([ExhibitionId])
REFERENCES [dbo].[Exhibition] ([ExhibitionId])
GO
ALTER TABLE [dbo].[ExhibitionAttendee] CHECK CONSTRAINT [FK_ExhibitionAttendee_Exhibitio]
GO
ALTER TABLE [dbo].[ProposedAnswer]  WITH CHECK ADD  CONSTRAINT [FK_ProposedAnswer_Question] FOREIGN KEY([QuestionId])
REFERENCES [dbo].[Question] ([QuestionId])
GO
ALTER TABLE [dbo].[ProposedAnswer] CHECK CONSTRAINT [FK_ProposedAnswer_Question]
GO
ALTER TABLE [dbo].[Question]  WITH CHECK ADD  CONSTRAINT [FK_Question_Survey] FOREIGN KEY([SurveyId])
REFERENCES [dbo].[Survey] ([SurveyId])
GO
ALTER TABLE [dbo].[Question] CHECK CONSTRAINT [FK_Question_Survey]
GO
ALTER TABLE [dbo].[Reward]  WITH CHECK ADD  CONSTRAINT [FK_Reward_AspNetUsers] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Reward] CHECK CONSTRAINT [FK_Reward_AspNetUsers]
GO
ALTER TABLE [dbo].[RewardHistory]  WITH CHECK ADD  CONSTRAINT [FK_RewardHistory_AspNetUsers] FOREIGN KEY([AccountId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[RewardHistory] CHECK CONSTRAINT [FK_RewardHistory_AspNetUsers]
GO
ALTER TABLE [dbo].[RewardHistory]  WITH CHECK ADD  CONSTRAINT [FK_RewardHistory_Reward] FOREIGN KEY([RewardId])
REFERENCES [dbo].[Reward] ([RewardId])
GO
ALTER TABLE [dbo].[RewardHistory] CHECK CONSTRAINT [FK_RewardHistory_Reward]
GO
ALTER TABLE [dbo].[Survey]  WITH CHECK ADD  CONSTRAINT [FK_Survey_AspNetUsers] FOREIGN KEY([AccountId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Survey] CHECK CONSTRAINT [FK_Survey_AspNetUsers]
GO
ALTER TABLE [dbo].[Survey]  WITH CHECK ADD  CONSTRAINT [FK_Survey_Company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Company] ([CompanyId])
GO
ALTER TABLE [dbo].[Survey] CHECK CONSTRAINT [FK_Survey_Company]
GO
ALTER TABLE [dbo].[Survey]  WITH CHECK ADD  CONSTRAINT [FK_Survey_Exhibition] FOREIGN KEY([ExhibitionId])
REFERENCES [dbo].[Exhibition] ([ExhibitionId])
GO
ALTER TABLE [dbo].[Survey] CHECK CONSTRAINT [FK_Survey_Exhibition]
GO
ALTER TABLE [dbo].[SurveyAnswer]  WITH CHECK ADD  CONSTRAINT [FK_SurveyAnswer_AspNetUsers] FOREIGN KEY([AccountId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[SurveyAnswer] CHECK CONSTRAINT [FK_SurveyAnswer_AspNetUsers]
GO
ALTER TABLE [dbo].[SurveyAnswer]  WITH CHECK ADD  CONSTRAINT [FK_SurveyAnswer_ProposedAnswer] FOREIGN KEY([ProposedAnswerId])
REFERENCES [dbo].[ProposedAnswer] ([ProposedAnswerId])
GO
ALTER TABLE [dbo].[SurveyAnswer] CHECK CONSTRAINT [FK_SurveyAnswer_ProposedAnswer]
GO
ALTER TABLE [dbo].[SurveyAnswer]  WITH CHECK ADD  CONSTRAINT [FK_SurveyAnswer_Question] FOREIGN KEY([QuestionId])
REFERENCES [dbo].[Question] ([QuestionId])
GO
ALTER TABLE [dbo].[SurveyAnswer] CHECK CONSTRAINT [FK_SurveyAnswer_Question]
GO
ALTER TABLE [dbo].[SurveyAnswer]  WITH CHECK ADD  CONSTRAINT [FK_SurveyAnswer_Survey] FOREIGN KEY([SurveyId])
REFERENCES [dbo].[Survey] ([SurveyId])
GO
ALTER TABLE [dbo].[SurveyAnswer] CHECK CONSTRAINT [FK_SurveyAnswer_Survey]
GO
ALTER TABLE [dbo].[SurveyParticipation]  WITH CHECK ADD  CONSTRAINT [FK_SurveyParticipation_AspNetUser] FOREIGN KEY([AccountId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[SurveyParticipation] CHECK CONSTRAINT [FK_SurveyParticipation_AspNetUser]
GO
ALTER TABLE [dbo].[SurveyParticipation]  WITH CHECK ADD  CONSTRAINT [FK_SurveyParticipation_Survey] FOREIGN KEY([SurveyId])
REFERENCES [dbo].[Survey] ([SurveyId])
GO
ALTER TABLE [dbo].[SurveyParticipation] CHECK CONSTRAINT [FK_SurveyParticipation_Survey]
GO
USE [master]
GO
ALTER DATABASE [GAMEX] SET  READ_WRITE 
GO
