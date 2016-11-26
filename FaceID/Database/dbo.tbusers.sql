USE [C:\USERS\MOSTRATEC\DOCUMENTS\SH2\APIREALSENSE\FACEID\DATABASE\SHDB.MDF]
GO

/****** Object: Table [dbo].[tbusers] Script Date: 17/11/2016 12:41:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbusers] (
    [userID]        INT          NOT NULL,
    [nome]          VARCHAR (50) NOT NULL,
    [tel]           VARCHAR (50) NOT NULL,
    [email]         VARCHAR (50) NOT NULL,
    [nasc]          VARCHAR (50) NOT NULL,
    [tempAr]        TINYINT      NULL,
    [statusTv]      VARCHAR (3)  NULL,
    [statusCortina] VARCHAR (5)  NULL
);


CREATE TABLE [dbo].[config]
(
	[ID] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [IP] VARCHAR(250) NOT NULL, 
    [PORT] INT NOT NULL
)