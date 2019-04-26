
/******  File 1 ******/

CREATE SCHEMA docstore;
GO

GRANT SELECT ON SCHEMA::docstore TO PUBLIC;
GRANT INSERT ON SCHEMA::docstore TO PUBLIC;
GRANT UPDATE ON SCHEMA::docstore TO PUBLIC;
GRANT ALTER ON SCHEMA::docstore TO PUBLIC;


GO
/****** File 2 ******/
USE [db1]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

---- Dropping Tables ----

DROP TABLE dbo.SecurityGroupMembership;
DROP TABLE dbo.SecurityKeys;
DROP TABLE dbo.SecurityGroup;
DROP TABLE dbo.DocumentMetadata;
DROP TABLE dbo.CustomMetadata;
DROP TABLE dbo.DocumentChange;
DROP TABLE dbo.DocumentACE;
DROP TABLE dbo.StorConfigs;
DROP TABLE dbo.OldDocumentVersion;

GO

---- Dropping Stored Procedures ----

DROP PROCEDURE dbo.RecordMetadataChange;
DROP PROCEDURE dbo.RecordFileChange
DROP PROCEDURE dbo.ExpireTokens;
GO


---- Document Metadata ----

CREATE TABLE dbo.DocumentMetadata(
	Id int IDENTITY(1,1) NOT NULL,
	Name nvarchar(1024) NOT NULL,
	Version int NOT NULL,
	/*FileName nvarchar(1024) NOT NULL,  --Not Needed FileName = ID.Version*/ 
	MD5Key varchar(32) NOT NULL,
	StorName varchar(10) NOT NULL,
	Extension varchar(6) NOT NULL, --commonly is 3
	BuisnessArea varchar(255) NULL, -- used to determine the ACE(s) used for accessing the Document, if is NULL then the 'DEFAULT' ACE(s) is used

	isLocked bit NOT NULL DEFAULT 0,
	LockedBy varchar(123) NULL,
	LockedAt smalldatetime NUll,
	LockExpires smalldatetime NULL,

	isArchived bit NOT NULL DEFAULT 0,
	ArchivedBy varchar(123) NULL,
	ArchivedAt smalldatetime NUll,

	CreatedBy varchar(123) NOT NULL,
	CreatedAt smalldatetime NOT NUll DEFAULT GETDATE(),
	LastUpdateBy varchar(123) NOT NULL,
	LastUpdateAt smalldatetime NOT NUll DEFAULT GETDATE(),

	CONSTRAINT PK_DocumentMetadata_Id PRIMARY KEY CLUSTERED (Id ASC ) WITH (
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	) ON [PRIMARY]
) ON [PRIMARY]
GO

--- Custom Metadata ---

CREATE TABLE dbo.CustomMetadata (
	Id int IDENTITY(1,1) NOT NULL,
	DocumentMetadataId int FOREIGN KEY REFERENCES DocumentMetadata(Id),
	[Key] varchar(255) NOT NULL,
	ValueStr nvarchar(max) NULL,
	ValueInt int NULL,
	ValueDate smalldatetime NULL,
	ValueBool bit NULL,
	CONSTRAINT PK_CustomMetadata_Id PRIMARY KEY CLUSTERED (Id ASC ) WITH (
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX IX_CustomMetadata_DocumentMetadataId
    ON dbo.CustomMetadata (DocumentMetadataId) WITH (
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	); 
GO

CREATE NONCLUSTERED INDEX IX_CustomMetadata_Key
	ON dbo.CustomMetadata ([Key]) WITH (
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	); 
GO

--- Document Change Control ----

CREATE TABLE dbo.DocumentChange (
	Id int IDENTITY(1,1) NOT NULL,
	DocumentMetadataId int FOREIGN KEY REFERENCES DocumentMetadata(Id),
	ChangeType char(3) NOT NULL, /* Document Change=DOC|Metadata Change=MDC*/
	[key] varchar(255) NULL, /* Paired with value - For ChangeType 'MDC' */
	[value] varchar(MAX) NULL, /* Paired with key - For ChangeType 'MDC' */
	OldVersionID int NULL, /* For Change Type 'DOC'*/
	UpdatedBy varchar(255) NOT NULL,
	UpdatedAt smalldatetime NOT NULL,
	CONSTRAINT PK_DocumentChange_Id PRIMARY KEY CLUSTERED (Id ASC ) WITH (
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX IX_DocumentChange_DocumentMetadataId
    ON dbo.DocumentChange (DocumentMetadataId) WITH (
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	); 
GO

CREATE TABLE dbo.OldDocumentVersion (
	Id int IDENTITY(1,1) NOT NULL,
	DocumentMetadataId int FOREIGN KEY REFERENCES DocumentMetadata(Id),
	Name nvarchar(1024) NOT NULL,
	Version int NOT NULL,
	MD5Key varchar(32) NOT NULL,
	StorName varchar(10) NOT NULL,
	Extension varchar(6) NOT NULL,
	CONSTRAINT PK_OldDocumentVersion_Id PRIMARY KEY CLUSTERED (Id ASC ) WITH (
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX IX_OldDocumentVersion_DocumentMetadataId
    ON dbo.OldDocumentVersion (DocumentMetadataId) WITH (
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	); 
GO


---- Document Access Control Entry -------

CREATE TABLE dbo.DocumentACE ( 
	Id int IDENTITY(1,1) NOT NULL,
	BuisnessArea varchar(255) NOT NULL, --'DEFAULT' is the permissions for documents without a business area
	GroupName varchar(255) NOT NULL, --Access is controlled by Buisness Area not by individual document
	FC bit NOT NULL DEFAULT(0), -- Full Control
	RM bit NOT NULL DEFAULT(0), -- Read & Modify
	RO bit NOT NULL DEFAULT(0), -- Read Only
	A  bit NOT NULL DEFAULT(0), -- Archive
	D  bit NOT NULL DEFAULT(0), -- Delete
	CONSTRAINT PK_DocumentACE_Id PRIMARY KEY CLUSTERED ( Id ) WITH (
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX IX_DocumentACE_BuisnessArea
    ON dbo.DocumentACE (BuisnessArea)
GO

---- Security And Group Managment ----

CREATE TABLE dbo.SecurityKeys(
	Id int IDENTITY(1,1) NOT NULL,
	AccessKey char(64) NOT NULL,
	Username varchar(20) NOT NULL,
	FullName nvarchar(100) NULL,
	ExpiresAt smalldatetime NOT NULL,
	CONSTRAINT PK_SecurityKey_Id PRIMARY KEY CLUSTERED ( Id ASC) WITH (
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX IX_SecurityKeys_AccessKey
    ON dbo.SecurityKeys (AccessKey) WITH (
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	); 
GO


CREATE TABLE dbo.SecurityGroup(
	Id int IDENTITY(1,1) NOT NULL,
	GroupName varchar(255) NOT NULL,
	isAdmin bit NOT NULL DEFAULT 0, /*Grants Full Control Access to Everything*/
	isAudit bit NOT NULL DEFAULT 0, /*Grant Read-Only Access to Everything (All other pemissions are ignored)*/
	CONSTRAINT PK_SecurityGroup_Id PRIMARY KEY CLUSTERED (Id ASC ) WITH (
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX IX_SecurityGroup_GroupName
    ON dbo.SecurityGroup (GroupName) WITH (
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	); 
GO

CREATE TABLE dbo.SecurityGroupMembership(
	Id int IDENTITY(1,1) NOT NULL,
	Username varchar(20) NOT NULL,
	GroupName varchar(255) NOT NULL FOREIGN KEY REFERENCES SecurityGroup(GroupName),
	CONSTRAINT PK_SecurityGroupMembership_Id PRIMARY KEY CLUSTERED (Id ASC ) WITH (
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX IX_SecurityGroupMembership_GroupName
    ON dbo.SecurityGroupMembership (GroupName)
GO
CREATE NONCLUSTERED INDEX IX_SecurityGroupMembership_Username
    ON dbo.SecurityGroupMembership (Username)
GO

---- Stor Configuration ----

CREATE TABLE dbo.StorConfigs (
	Id int IDENTITY(1,1) NOT NULL,
	ShortName varchar(10) NOT NULL,
	StorType varchar(5) NOT NULL, /* File Share='SHARE' | Azure Blob Storage='AZBLB' |  */
	Args varchar(max) NOT NULL,
	ENV varchar(255) NULL
	CONSTRAINT PK_StorConfigs_Id PRIMARY KEY CLUSTERED (Id ASC ) WITH (
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	) ON [PRIMARY]
) ON [PRIMARY];

GO

CREATE NONCLUSTERED INDEX IX_StorConfigs_ShortName
    ON dbo.StorConfigs (ShortName) WITH (
		PAD_INDEX = OFF,
		STATISTICS_NORECOMPUTE = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	); 
GO

---- Stored Procedures ----

-- =============================================
-- Author:		Liam Townsend
-- Create date: 21/03/19
-- Description:	Stored Procedure to Record Metadata changes
-- =============================================
CREATE PROCEDURE dbo.RecordMetadataChange
	@MetadataId int,
	@Key varchar(255),
	@Value varchar(MAX),
	@Username varchar(255)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.DocumentChange (DocumentMetadataId, [key], [value], UpdatedBy, UpdatedAt) 
		VALUES (@MetadataId, @Key, @Value, @Username, GETDATE())
END
GO

-- =============================================
-- Author:		Liam Townsend
-- Create date: 21/03/19
-- Description:	Stored Procedure to Record changes to Documents
-- =============================================
CREATE PROCEDURE dbo.RecordFileChange
	@MetadataId int,
	@Key varchar(32),
	@Username varchar(255)
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO dbo.DocumentChange (DocumentMetadataId, MD5Key, UpdatedBy, UpdatedAt) 
		VALUES (@MetadataId, @Key,  @Username, GETDATE())
END
GO

-- =============================================
-- Author:		Liam Townsend
-- Create date: 25/03/19
-- Description:	Stored Procedure to Remove Expired Tokens.
-- =============================================
CREATE PROCEDURE dbo.ExpireTokens
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM dbo.SecurityKeys WHERE ExpiresAt<=GETDATE()
END
GO



---- Test Data ----

DELETE FROM dbo.SecurityKeys;
DELETE FROM dbo.SecurityGroup;
DELETE FROM dbo.SecurityGroupMembership;

GO

INSERT INTO dbo.SecurityGroup (GroupName, isAdmin, isAudit) VALUES ('Admins',1,0);
INSERT INTO dbo.SecurityGroup (GroupName, isAdmin, isAudit) VALUES ('Auditors',0,1);
INSERT INTO dbo.SecurityGroup (GroupName, isAdmin, isAudit) VALUES ('All Users',0,0);

INSERT INTO dbo.SecurityKeys (AccessKey, Username, FullName, ExpiresAt) VALUES ('9acd13c2bc7896a2a93c99319cdaf66f9c121cfafafeb3a129f72971ec6b5ee9','liamt','Liam Townsend', DATEADD(HOUR, 5, GETDATE()));;
INSERT INTO dbo.SecurityKeys (AccessKey, Username, FullName, ExpiresAt) VALUES ('aefde630fb4758f60ce9d9f017f439f59fffcf02f98333c5199bef4ef30849e8','jcarr','Jimmy Carr', DATEADD(HOUR, 6, GETDATE()));
INSERT INTO dbo.SecurityKeys (AccessKey, Username, FullName, ExpiresAt) VALUES ('775d38f6b4309710e849881879ec7a5a74d4c3c7686614803d95610d31cdec11','lbrindley','Lewis Brindley', DATEADD(HOUR, 3, GETDATE()));
INSERT INTO dbo.SecurityKeys (AccessKey, Username, FullName, ExpiresAt) VALUES ('5dfcddb2b0d3fd8cf3080e22d7993f7253e75bd712db9543196c4e4217284e5f','jacaster','James Acaster', DATEADD(HOUR, 6, GETDATE()));
INSERT INTO dbo.SecurityKeys (AccessKey, Username, FullName, ExpiresAt) VALUES ('3fd27f38e8f349d161339149f1c3794f8bc82f507b91c7e624e304407e0e66aa','sclark','Simon Clark', DATEADD(HOUR, 6, GETDATE()));
INSERT INTO dbo.SecurityKeys (AccessKey, Username, FullName, ExpiresAt) VALUES ('c4c562836cb820647349da99a0f39a51f75ef980cfc3474de4265344619474a8','djones','Duncan Jones', DATEADD(HOUR, 6, GETDATE()));
INSERT INTO dbo.SecurityKeys (AccessKey, Username, FullName, ExpiresAt) VALUES ('c4c562836cb820647349da99a0f39a51f75ef980cfc3474de4265344619474L8','delteme','Delete Me', DATEADD(HOUR, -6, GETDATE()));

INSERT INTO dbo.SecurityGroupMembership (Username, GroupName) 
	VALUES
		('liamt', 'Admins'),
		('lbrindley', 'Admins'),
		('jcarr', 'Auditors'),
		('liamt', 'All Users'),
		('lbrindley', 'All Users'),
		('jcarr', 'All Users'),
		('jacaster', 'All Users'),
		('sclark', 'All Users'),
		('djones', 'All Users');

INSERT INTO dbo.DocumentMetadata (Name,Version,MD5Key,StorName,Extension,BuisnessArea,CreatedBy,LastUpdateBy) 
	VALUES ('Document35987412',1,'82C37C3E27020AF6C2E018E944284676','FC01/D','docx','HR','Liam Townsend (liamt)','Liam Townsend (liamt)');

INSERT INTO dbo.CustomMetadata (DocumentMetadataId, [Key], ValueStr) VALUES (1, 'CustomerName', 'Joe Bloggs');
INSERT INTO dbo.CustomMetadata (DocumentMetadataId, [Key], ValueInt) VALUES (1, 'CustomerID', 561);

GO

SELECT * FROM dbo.SecurityGroupMembership;
SELECT * FROM dbo.SecurityKeys;
SELECT * FROM dbo.SecurityGroup;
SELECT * FROM dbo.DocumentMetadata;
SELECT * FROM dbo.CustomMetadata;

GO