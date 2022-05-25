CREATE DATABASE FakeUsersApi
GO
USE FakeUsersApi
GO
CREATE TABLE Users(
	[IdUser] [uniqueidentifier] NOT NULL,
	[SecondNameUser] [nvarchar](100) NULL,
	[FirstNameUser] [nvarchar](100) NULL,
	[OS] [varchar](30) NULL,
	[Browser] [varchar](30) NULL,
	[DateBirth] [date] NULL,
	[Login] [varchar](100) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[IPAddress] [char](15) NULL,
	[UserAgent] [varchar](MAX) NULL,
	[Lock] [bit] NULL,
	[DateIn] [datetime] NOT NULL
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[IdUser] ASC
))
GO
CREATE TABLE EnterLogs(
	[IdUser] [uniqueidentifier] NOT NULL,
	[DateIn] [datetime] NOT NULL,
	[SecondNameUser] [nvarchar](100) NULL,
	[FirstNameUser] [nvarchar](100) NULL,
	[OS] [varchar](30) NULL,
	[Browser] [varchar](30) NULL,
	[Email] [varchar](100) NOT NULL,
	[IPAddress] [char](15) NULL,
	[UserAgent] [varchar](MAX) NULL,
	[Series] [char](4) NULL,
	[Number] [char](6) NULL,
	[UnitCode] [char](7) NULL,
	[UnitName] [nvarchar](MAX) NULL,
	[DateIssue] [date] NULL
)
GO
CREATE TABLE Passports(
	[IdPass] [int] IDENTITY(1,1) NOT NULL,
	[Series] [char](4) NULL,
	[Number] [char](6) NULL,
	[UnitCode] [char](7) NULL,
	[UnitName] [nvarchar](MAX) NULL,
	[DateIssue] [date] NULL,
	[UserId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_passports] PRIMARY KEY CLUSTERED 
(
	[IdPass] ASC
))
GO
CREATE TABLE OKATO(
	[IdOkato] [int] IDENTITY(1,1) NOT NULL,
	[CodeOkato] [char](2) NOT NULL,
	[CodeRegion] [char](2) NOT NULL,
	[NameDistrict] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_okato] PRIMARY KEY CLUSTERED 
(
	[IdOkato] ASC
))
GO
CREATE TABLE UnitCode(
	[IdUnitCode] [int] IDENTITY(1,1) NOT NULL,
	[UnitCode] [nvarchar](7) NOT NULL,
	[Unit] [nvarchar](MAX) NOT NULL,
 CONSTRAINT [PK_unitCode] PRIMARY KEY CLUSTERED 
(
	[IdUnitCode] ASC
))
GO
CREATE TABLE AnalyzePass(
	[IdAnalyzeP] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NOT NULL,
	[IdUser] [uniqueidentifier] NOT NULL,
	[Serie] [char](10) NULL,
	[Number] [char](10) NULL,
	[SerieEqualRegion] [char](10) NULL,
	[Code] [char](10) NULL,
	[PassIsOut] [char](10) NULL,
	[Valid] bit NOT NULL,
 CONSTRAINT [PK_AnalyzePass] PRIMARY KEY CLUSTERED 
(
	[IdAnalyzeP] ASC
))
GO
CREATE TABLE AnalyzeEnter(
	[IdAnalyzeE] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NOT NULL,
	[IdUser] [uniqueidentifier] NOT NULL,
	[IPLocate] [char](10) NULL,
	[IPReason] [nvarchar](max) NULL,
	[PassChange] [char](10) NULL,
	[PassReason] [nvarchar](max) NULL,
	[InCorrectLogin] [char](10) NULL,
	[BrowserFail] [char](10) NULL,
	[OSFail] [char](10) NULL,
	[Valid] bit NOT NULL,
 CONSTRAINT [PK_AnalyzeEnter] PRIMARY KEY CLUSTERED 
(
	[IdAnalyzeE] ASC
))
GO
CREATE PROCEDURE AddEnter_v1 @EnterStructure NVARCHAR(MAX)
WITH EXECUTE AS OWNER AS
BEGIN
	INSERT INTO [EnterLogs]
           	([IdUser],
			[DateIn],
			[SecondNameUser],
			[FirstNameUser],
		[OS],
		[Browser],
		[Email],
		[IPAddress],
		[UserAgent],
		[Series],
	[Number],
	[UnitCode],
	[UnitName],
	[DateIssue])
		VALUES
	(
		(SELECT [IdUser] FROM OPENJSON (@EnterStructure) WITH ([IdUser] [uniqueidentifier])),
		(SELECT [DateIn] FROM OPENJSON (@EnterStructure) WITH ([DateIn] [datetime])),
		(SELECT [SecondNameUser] FROM OPENJSON (@EnterStructure) WITH ([SecondNameUser] [nvarchar](100))),
	(SELECT [FirstNameUser] FROM OPENJSON (@EnterStructure) WITH ([FirstNameUser] [nvarchar](100))),
	(SELECT [OS] FROM OPENJSON (@EnterStructure) WITH ([OS] [varchar](30))),
	(SELECT [Browser] FROM OPENJSON (@EnterStructure) WITH ([Browser] [varchar](30))),
	(SELECT [Email] FROM OPENJSON (@EnterStructure) WITH ([Email] [varchar](100))),
	(SELECT [IPAddress] FROM OPENJSON (@EnterStructure) WITH ([IPAddress] [char](15))),
	(SELECT [UserAgent] FROM OPENJSON (@EnterStructure) WITH ([UserAgent] [varchar](MAX))),
	(SELECT [Series] FROM OPENJSON (@EnterStructure) WITH ([Series] [char](4))),
	(SELECT [Number] FROM OPENJSON (@EnterStructure) WITH ([Number] [char](6))),
	(SELECT [UnitCode] FROM OPENJSON (@EnterStructure) WITH ([UnitCode] [char](7))),
	(SELECT [UnitName] FROM OPENJSON (@EnterStructure) WITH ([UnitName] [nvarchar](MAX))),
	(SELECT [DateIssue] FROM OPENJSON (@EnterStructure) WITH ([DateIssue] [date]))
	)
END;
GO
CREATE PROCEDURE UpdateUserPass_v1 @EnterStructure NVARCHAR(MAX)
WITH EXECUTE AS OWNER AS
BEGIN
	UPDATE [Users]
    SET 
	[SecondNameUser] = (SELECT [SecondNameUser] FROM OPENJSON (@EnterStructure) WITH ([SecondNameUser] [nvarchar](100))),
	[FirstNameUser] = (SELECT [FirstNameUser] FROM OPENJSON (@EnterStructure) WITH ([FirstNameUser] [nvarchar](100))),
	[OS] = (SELECT [OS] FROM OPENJSON (@EnterStructure) WITH ([OS] [varchar](30))),
	[Browser] = (SELECT [Browser] FROM OPENJSON (@EnterStructure) WITH ([Browser] [varchar](30))),
	[Email] = (SELECT [Email] FROM OPENJSON (@EnterStructure) WITH ([Email] [varchar](100))),
	[IPAddress] = (SELECT [IPAddress] FROM OPENJSON (@EnterStructure) WITH ([IPAddress] [char](15))),
	[UserAgent] = (SELECT [UserAgent] FROM OPENJSON (@EnterStructure) WITH ([UserAgent] [varchar](MAX))),
	[DateIn] = (SELECT [DateIn] FROM OPENJSON (@EnterStructure) WITH ([DateIn] [datetime]))
	WHERE [IdUser] = (SELECT [IdUser] FROM OPENJSON (@EnterStructure) WITH ([IdUser] [uniqueidentifier]));
	UPDATE [Passports]
    SET 
	[Series] = (SELECT [Series] FROM OPENJSON (@EnterStructure) WITH ([Series] [char](4))),
	[Number] = (SELECT [Number] FROM OPENJSON (@EnterStructure) WITH ([Number] [char](6))),
	[UnitCode] = (SELECT [UnitCode] FROM OPENJSON (@EnterStructure) WITH ([UnitCode] [char](7))),
	[UnitName] = (SELECT [UnitName] FROM OPENJSON (@EnterStructure) WITH ([UnitName] [nvarchar](MAX))),
	[DateIssue] = (SELECT [DateIssue] FROM OPENJSON (@EnterStructure) WITH ([DateIssue] [date]))
	WHERE [UserId] = (SELECT [IdUser] FROM OPENJSON (@EnterStructure) WITH ([IdUser] [uniqueidentifier]));
END;
GO
CREATE PROCEDURE AddUser_v1 @UserStructure NVARCHAR(MAX)
WITH EXECUTE AS OWNER AS
BEGIN
	INSERT INTO [Users]
           	([IdUser],
			[SecondNameUser],
			[FirstNameUser],
		[OS],
		[Browser],
		[DateBirth],
		[Login],
		[Email],
		[IPAddress],
		[UserAgent],
		[Lock],
		[DateIn])
		
	SELECT [IdUser],
		[SecondNameUser],
			[FirstNameUser],
		[OS],
		[Browser],
		[DateBirth],
		[Login],
		[Email],
		[IPAddress],
		[UserAgent],
		[Lock],
		[DateIn]
	FROM OPENJSON (@UserStructure)
	WITH ([IdUser] [uniqueidentifier],
	[SecondNameUser] [nvarchar](100),
		[FirstNameUser] [nvarchar](100),
		[OS] [varchar](30),
		[Browser] [varchar](30),
		[DateBirth] [date],
		[Login] [varchar](100),
		[Email] [varchar](100),
		[IPAddress] [char](15),
		[UserAgent] [nvarchar](MAX),
		[Lock] [bit],
		[DateIn] [datetime])
END;
GO
CREATE PROCEDURE AddPass_v1 @PassStructure NVARCHAR(MAX)
WITH EXECUTE AS OWNER AS
BEGIN
	INSERT INTO Passports(
	[Series],
	[Number],
	[UnitCode],
	[DateIssue],
	[UnitName],
	[UserId])
	VALUES
	(
	(SELECT [Series] FROM OPENJSON (@PassStructure) WITH ([Series] [char](4))),
	(SELECT [Number] FROM OPENJSON (@PassStructure) WITH ([Number] [char](6))),
	(SELECT [UnitCode] FROM OPENJSON (@PassStructure) WITH ([UnitCode] [char](7))),
	(SELECT [DateIssue] FROM OPENJSON (@PassStructure) WITH ([DateIssue] [date])),
	(SELECT [UnitName] FROM OPENJSON (@PassStructure) WITH ([UnitName] [nvarchar](MAX))),
	(SELECT [UserId] FROM OPENJSON (@PassStructure) WITH ([UserId] [uniqueidentifier]))
	)
END;
GO
CREATE FUNCTION FindIdUser_v1 
(
@Login VARCHAR(100)
)
RETURNS uniqueidentifier 
AS
BEGIN
DECLARE @Id uniqueidentifier
  SELECT @Id = IdUser
  FROM [Users]
  WHERE [Login]=@Login;
RETURN @Id
END;
GO
CREATE TABLE [Ip2Location](
	[ip_from] bigint NOT NULL,
	[ip_to] bigint NOT NULL,
	[country_code] nvarchar(2) NOT NULL,
	[country_name] nvarchar(64) NOT NULL,
	[region_name] nvarchar(128) NOT NULL,
	[city_name] nvarchar(128) NOT NULL,
	[latitude] nvarchar(64) NOT NULL,
	[longitude] nvarchar(64) NOT NULL
) ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [ip_to] ON [Ip2Location]([ip_to]) ON [PRIMARY]
GO
CREATE FUNCTION IpStringToInt 
( 
    @ip varchar(max)
) 
RETURNS BIGINT 
AS 
BEGIN 
DECLARE @idx1 int
    DECLARE @idx2 int
    DECLARE @idx3 int
    DECLARE @idx4 int
    DECLARE @ret bigint
     
    SELECT @idx1 = CHARINDEX('.', @ip)
    SELECT @idx2 = CHARINDEX('.', @ip, @idx1+1);
    SELECT @idx3 = CHARINDEX('.', @ip, @idx2+1);
     
    SELECT @ret = CONVERT(bigint, SUBSTRING(@ip, 0, @idx1)) * POWER(2, 24) +
                    CONVERT(bigint, SUBSTRING(@ip, @idx1 + 1, @idx2 - @idx1 - 1)) * POWER(2, 16) +
                    CONVERT(bigint, SUBSTRING(@ip, @idx2 + 1, @idx3 - @idx2 - 1)) * POWER(2, 8) +
                    CONVERT(bigint, SUBSTRING(@ip, @idx3 + 1, LEN(@ip) - @idx3))
    RETURN @ret
END;
GO
CREATE FUNCTION IpIntToString 
( 
    @ip bigINT 
) 
RETURNS CHAR(15) 
AS 
BEGIN 
    DECLARE @o1 bigINT, 
        @o2 bigINT, 
        @o3 bigINT, 
        @o4 bigINT 

    IF ABS(@ip) > 4294967295 
        RETURN '255.255.255.255' 

    SET @o1 = @ip / 16777216 

    IF @o1 = 0 
        SELECT @o1 = 255, @ip = @ip + 16777216 

    ELSE IF @o1 < 0 
    BEGIN 
        IF @ip % 16777216 = 0 
            SET @o1 = @o1 + 256 
        ELSE 
        BEGIN 
            SET @o1 = @o1 + 255 
            IF @o1 = 128 
                SET @ip = @ip + 2147483648 
            ELSE 
                SET @ip = @ip + (16777216 * (256 - @o1)) 
        END 
    END 
    ELSE 
    BEGIN 
        SET @ip = @ip - (16777216 * @o1) 
    END 

    SET @ip = @ip % 16777216 
    SET @o2 = @ip / 65536 
    SET @ip = @ip % 65536 
    SET @o3 = @ip / 256 
    SET @ip = @ip % 256 
    SET @o4 = @ip 

    RETURN 
        CONVERT(VARCHAR(4), @o1) + '.' + 
        CONVERT(VARCHAR(4), @o2) + '.' + 
        CONVERT(VARCHAR(4), @o3) + '.' + 
        CONVERT(VARCHAR(4), @o4) 
END;
GO
CREATE FUNCTION GetCoordsByIpInRow 
(
@ip CHAR(15)
)
RETURNS @tabl TABLE ([ip_from] bigint NOT NULL,
	[ip_to] bigint NOT NULL,
	[country_code] nvarchar(2) NOT NULL,
	[country_name] nvarchar(64) NOT NULL,
	[region_name] nvarchar(128) NOT NULL,
	[city_name] nvarchar(128) NOT NULL,
	[latitude] nvarchar(64) NOT NULL,
	[longitude] nvarchar(64) NOT NULL)
AS
BEGIN
INSERT INTO @tabl
  SELECT * FROM (
    SELECT TOP 1 * FROM [Ip2Location] WHERE ip_to >= (SELECT dbo.IpStringToInt(@ip))
) AS tmp WHERE ip_from <= (SELECT dbo.IpStringToInt(@ip));
RETURN;
END;
GO
CREATE FUNCTION GetUserWithPassByLogin 
(
@login varchar(100)
)
RETURNS @tabl TABLE ([IdUser] [uniqueidentifier] NOT NULL,
	[DateIn] [datetime] NOT NULL,
	[SecondNameUser] [nvarchar](100) NULL,
	[FirstNameUser] [nvarchar](100) NULL,
	[OS] [varchar](30) NULL,
	[Browser] [varchar](30) NULL,
	[Email] [varchar](100) NOT NULL,
	[IPAddress] [char](15) NULL,
	[UserAgent] [varchar](MAX) NULL,
	[Series] [char](4) NULL,
	[Number] [char](6) NULL,
	[UnitCode] [char](7) NULL,
	[UnitName] [nvarchar](MAX) NULL,
	[DateIssue] [date] NULL)
AS
BEGIN
INSERT INTO @tabl
SELECT Users.IdUser, Users.DateIn, Users.SecondNameUser, Users.FirstNameUser, Users.OS, Users.Browser, Users.Email, Users.IPAddress, Users.UserAgent,
Pass.Series, Pass.Number, Pass.UnitCode, Pass.UnitName, Pass.DateIssue
FROM 
(SELECT IdUser, DateIn, SecondNameUser, FirstNameUser, OS, Browser, Email, IPAddress, UserAgent FROM Users WHERE Login = @login) Users,
(SELECT Series, Number, UnitCode, UnitName, DateIssue FROM Passports WHERE UserId = (SELECT dbo.FindIdUser_v1 (@login))) Pass;
  
RETURN;
END;
GO
CREATE VIEW AllStat
AS
SELECT        (SELECT        COUNT(*) AS Expr1
				FROM            dbo.EnterLogs AS Users_1) AS Total_enters,
				(SELECT        COUNT(*) AS Expr2
				FROM            dbo.Users AS Users_2) AS Total_users,
				(SELECT        COUNT(*) AS Expr3
				FROM            dbo.Users
				WHERE        (Lock = 1)) AS Blocked_users,
				(SELECT        COUNT(*) AS Expr4
				FROM            dbo.AnalyzeEnter AS Users_4
				WHERE        (Valid = 0)) AS Total_invalid_enter_check,
				(SELECT        COUNT(*) AS Expr5
				FROM            dbo.AnalyzeEnter AS Users_5
				WHERE        (Valid = 1)) AS Total_valid_enter_check
GO
CREATE VIEW StatEnters
AS
SELECT        (SELECT        COUNT(*) AS Expr1
                          FROM            dbo.AnalyzeEnter AS Users_1) AS Total_check,
                             (SELECT        COUNT(*) AS Expr2
                               FROM            dbo.AnalyzeEnter AS Users_2
                               WHERE        (Valid = 1)) AS Total_valid_check,
                             (SELECT        COUNT(*) AS Expr3
                               FROM            dbo.AnalyzeEnter AS Users_3
                               WHERE        (IPLocate = 'fail')) AS Total_IP_incorrect_enters,
                             (SELECT        COUNT(*) AS Expr4
                               FROM            dbo.AnalyzeEnter AS Users_4
                               WHERE        (PassChange = 'fail')) AS Total_pass_change,
                             (SELECT        COUNT(*) AS Expr5
                               FROM            dbo.AnalyzeEnter AS Users_5
                               WHERE        (InCorrectLogin = 'fail')) AS Total_email_login_incorrect_enters,
                             (SELECT        COUNT(*) AS Expr6
                               FROM            dbo.AnalyzeEnter AS Users_6
                               WHERE        (BrowserFail = 'fail')) AS Total_browser_lock,
                             (SELECT        COUNT(*) AS Expr7
                               FROM            dbo.AnalyzeEnter AS Users_7
                               WHERE        (OSFail = 'fail')) AS Total_OS_lock
GO
CREATE VIEW StatPass
AS
SELECT        (SELECT        COUNT(*) AS Expr1
                          FROM            dbo.AnalyzePass AS Users_1) AS Total_check,
                             (SELECT        COUNT(*) AS Expr2
                               FROM            dbo.AnalyzePass AS Users_2
                               WHERE        (Valid = 1)) AS Total_valid_check,
                             (SELECT        COUNT(*) AS Expr3
                               FROM            dbo.AnalyzePass AS Users_3
                               WHERE        (Serie = 'fail')) AS Total_invalid_pass_serie,
                             (SELECT        COUNT(*) AS Expr4
                               FROM            dbo.AnalyzePass AS Users_4
                               WHERE        (Number = 'fail')) AS Total_invalid_pass_number,
                             (SELECT        COUNT(*) AS Expr5
                               FROM            dbo.AnalyzePass AS Users_5
                               WHERE        (SerieEqualRegion = 'fail')) AS Total_pass_region_not_match_serie,
                             (SELECT        COUNT(*) AS Expr6
                               FROM            dbo.AnalyzePass AS Users_6
                               WHERE        (Code = 'fail')) AS Total_code_not_match_unit_name,
                             (SELECT        COUNT(*) AS Expr7
                               FROM            dbo.AnalyzePass AS Users_7
                               WHERE        (PassIsOut = 'fail')) AS Total_pass_is_out
GO