Before running the test cases in ManualTests folder, please follow the steps below to setup the test environment.

1. Download docker installer from docker website and then install docker.
   It's better to have WSL 2 ready in local machine.

Open Windows Terminal

2. Run the following command to pull SQL Server docker image.
   >docker pull mcr.microsoft.com/mssql/server
3. Run the following command to run SQL Server in a docker image. Note: password must meet SQL Server password policy requirements.
   >docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=1qaz@WSX3edc" -p 1433:1433 --name sql_server_demo -d mcr.microsoft.com/mssql/server
4. Download Northwind script by the following command,
   >curl https://raw.githubusercontent.com/Microsoft/sql-server-samples/master/samples/databases/northwind-pubs/instnwnd.sql > instnwnd.sql
5. Copy the script to the docker image by 
   >docker cp instnwnd.sql sql_server_demo:/
6. Connect to SQL Server by 
   >docker exec -it sql_server_demo /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P 1qaz@WSX3edc
7. Execute the Northwind database script by 
   > USE master 
   > GO 
   > CREATE DATABASE Northwind 
   > GO 
   > USE Northwind 
   > GO 
   > :r instnwnd.sql 
   > GO
   > exit

8. Open Azure Data Studio to connect to SQL Server. Note: Server=localhost,1433, user name=sa ,and password is shown above. Trust server certificate must be true.
9. Create the following stored procedure to Northwind database
CREATE PROCEDURE InsertEmployee @LastName NVARCHAR(20),
    @FirstName NVARCHAR(10),
    @Title NVARCHAR(30),
    @TitleOfCourtesy NVARCHAR(25),
    @BirthDate DATETIME,
    @HireDate DATETIME,
    @Address NVARCHAR(60),
    @City NVARCHAR(15),
    @Region NVARCHAR(15),
    @PostalCode NVARCHAR(10),
    @Country NVARCHAR(15),
    @HomePhone NVARCHAR(24),
    @Extension NVARCHAR(4),
    @Notes TEXT,
    @ReportsTo INT,
    @PhotoPath NVARCHAR(255)
AS
BEGIN
    INSERT INTO Employees (
        LastName,
        FirstName,
        Title,
        TitleOfCourtesy,
        BirthDate,
        HireDate,
        Address,
        City,
        Region,
        PostalCode,
        Country,
        HomePhone,
        Extension,
        Notes,
        ReportsTo,
        PhotoPath
        )
    VALUES (
        @LastName,
        @FirstName,
        @Title,
        @TitleOfCourtesy,
        @BirthDate,
        @HireDate,
        @Address,
        @City,
        @Region,
        @PostalCode,
        @Country,
        @HomePhone,
        @Extension,
        @Notes,
        @ReportsTo,
        @PhotoPath
        )
END
GO

CREATE PROCEDURE GetEmployeesByFirstName @FirstName NVARCHAR(50)
AS
BEGIN
    SELECT EmployeeID,
        FirstName,
        LastName,
        Title,
        TitleOfCourtesy,
        BirthDate,
        HireDate,
        Address,
        City,
        Region,
        PostalCode,
        Country,
        HomePhone,
        Extension,
        Notes,
        ReportsTo,
        PhotoPath
    FROM Employees
    WHERE FirstName LIKE @FirstName + '%';-- Use the LIKE operator for partial matches
END
GO 

CREATE PROCEDURE GetAllEmployees
AS
BEGIN
    SELECT EmployeeID,
        FirstName,
        LastName,
        Title,
        BirthDate,
        HireDate,
        Address,
        City,
        Region,
        PostalCode,
        Country,
        HomePhone,
        Extension,
        Notes,
        ReportsTo,
        PhotoPath
    FROM Employees
END
GO

CREATE PROCEDURE GetAllOrders
AS
BEGIN
    SELECT OrderID,
        CustomerID,
        EmployeeID,
        OrderDate,
        RequiredDate,
        ShippedDate,
        ShipVia,
        Freight,
        ShipName,
        ShipAddress,
        ShipCity,
        ShipRegion,
        ShipPostalCode,
        ShipCountry
    FROM Orders;
END
GO

CREATE PROCEDURE UpdateEmployeeOneExtension
AS
UPDATE Employees
SET Extension = '1234'
WHERE EmployeeID = 1
GO
