USE StoredProceduresDemo
IF EXISTS (
           SELECT *
             FROM sysobjects
            WHERE Name = 'GetEmployees'
              AND TYPE = 'p')
  DROP PROCEDURE GetEmployees
GO
CREATE PROCEDURE GetEmployees AS
SELECT EmployeeID, FirstName, LastName, HireDate
  FROM Employees
GO
IF EXISTS (
           SELECT *
             FROM sysobjects
            WHERE Name = 'GetFirstNames'
              AND TYPE = 'p')

  DROP PROCEDURE GetFirstNames
GO
CREATE PROCEDURE GetFirstNames
AS
SELECT FirstName
  FROM Employees
 ORDER BY FirstName
GO
IF EXISTS (
           SELECT *
             FROM sysobjects
            WHERE Name = 'GetCountryNames'
              AND TYPE = 'p')
  DROP PROCEDURE GetCountryNames
GO
CREATE PROCEDURE GetCountryNames
AS
SELECT Country
  FROM Employees
 ORDER BY Country
GO
IF EXISTS (
           SELECT *
             FROM sysobjects
            WHERE Name = 'GetEmployeesByName'
              AND TYPE = 'p')
  DROP PROCEDURE GetEmployeesByName
GO
CREATE PROCEDURE GetEmployeesByName
@FirstName VarChar(40)
AS
SELECT EmployeeID
     , FirstName
     , LastName
     , HireDate
  FROM Employees
 WHERE FirstName = @FirstName
GO
IF EXISTS (
           SELECT *
             FROM StoredProceduresDemo.dbo.sysobjects
            WHERE Name = 'CountPeopleInCountry'
              AND TYPE = 'p')
  DROP PROCEDURE CountPeopleInCountry
GO
CREATE PROCEDURE CountPeopleInCountry
@Country NVarChar(40),
@CountInCountry int OUT
AS
SELECT @CountInCountry = COUNT(EmployeeID)
FROM Employees
WHERE Country = @Country
RETURN
(SELECT COUNT(EmployeeID)
   FROM Employees
  WHERE Country = @Country)
GO
