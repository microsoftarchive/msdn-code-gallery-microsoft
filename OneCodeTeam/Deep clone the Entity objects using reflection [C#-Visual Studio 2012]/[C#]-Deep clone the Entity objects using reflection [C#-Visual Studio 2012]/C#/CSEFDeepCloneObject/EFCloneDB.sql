CREATE DATABASE EFCloneDB;
GO

USE EFCloneDB;
GO

CREATE TABLE [dbo].[Employee](
	[EmpId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[FirstName] [varchar](50) NULL,
	[LastName] [varchar](50) NULL,
	[Age] [float] NULL,
	[Sex] [varchar](10) NULL,
	) 

GO

CREATE TABLE [dbo].[EmpAddress](
	[AddressId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[EmpId] [int] NULL REFERENCES [dbo].[Employee]([EmpId]),
	[AddressLine] [varchar](150) NULL,
	[City] [varchar](150) NULL,
	[State] [varchar](150) NULL,
	)
GO

CREATE TABLE [dbo].[BasicSalesInfo](
    [SalInfoId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[EmpId] [int] NULL REFERENCES [dbo].[Employee]([EMPID]),
	[Total] [float] NULL,
    )
GO

CREATE TABLE [dbo].[DetailSalesInfo](
    [DetailSalInfoId] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[BasicSalInfoId] [int] NULL REFERENCES [dbo].[BasicSalesInfo]([SalInfoId]),
    [Sale] [float] NULL,
	[Year] [date] NULL,
    )