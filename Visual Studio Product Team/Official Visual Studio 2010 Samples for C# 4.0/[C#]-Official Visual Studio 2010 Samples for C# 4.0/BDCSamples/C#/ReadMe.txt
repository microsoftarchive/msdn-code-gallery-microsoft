

*************ReadMe****************

Apply to both VB and C# sample projects.

These BDC samples connect to Northwind database as external data source. Please read below notes carefully and make sure you have the Northwind database connection before you run the sample project.


**************************
Prepare Northwind DB
**************************

If you already have a Northwind Database, just open Setting.setting file in each project and update the NORTHWNDConnectionString with your own connection string.

Otherwise, please follow these steps:

1. Following steps assume you already have SQL Express installed on your machine which comes free with Visual Studio and SharePoint Server. If you haven't installed, please visit http://www.microsoft.com/express/sql/download/ and install SQL Express first.

2. Create a "SampleNorthwind" DB. To do so, open VS. Go to View->Server Explorer. Right click on the Data Connections node in Server Explorer window, and select Create New SQL Server Database.

3. In the prompt dialog, enter "localhost\sqlexpress" as Server Name, and give the new database name "SampleNorthwind".

   * If you're using the SQL Express that comes with SharePoint Server, please replace "localhost\sqlexpress" with "localhost\sharepoint".

4. Start a Command Prompt.


5. In the Command Prompt, type in following and press enter: 

	sqlcmd -S localhost\sqlexpress -d samplenorthwind -i <Path of CreateSampleNorthwindDB.sql file>

   
   * CreateSampleNorthwindDB.sql is located in the same folder as this ReadMe file. It will create the Customer table schema and data for you.


*************************
Deploy BDC Model
*************************

Now you can open the sample projects. You'll need to specify Site URL of the project first. To do this, click on the project node and in the Properties window set Site URL to "http://localhost". Now press F5 to debug or Build->Deploy to deploy the BDC model.

* Please note that if you want to play with both sample projects, it's recommended that you create a second Database table to connect the second project.

Currently both sample projects are connected to the same DB you just created, so it's possible that at runtime only one project gets to connect to the      database correctly. To avoid any data conflict, it's strongly recommended to create a second table. To do that, you can follow the above      steps again except use a different table name. When you have done this, just open the Setting.setting file in the project and change the NORTHWNDConnectionString to connect to the new table.


***************************************
Create External List on SharePoint site
***************************************

Once you've successfully deployed the BDC model, now you can create an External List on the SharePoint site to manipulate data. 

To do so, 

1. Go to the homepage of your SharePoint site. Typically Http://localhost.
2. On the top left corner of the site, select Site Actions -> More Options. 
3. On the Create page, click on External List.
4. On the New page, give a name to the list. on Data Source Configuration section, click on Select External Content Type button. On the prompt External Content Type Picker, select the type you just deployed - BdcSampleCSharp.BdcModel1.Customer or BdcSampleVB.Customer.

   *Note: If you cannot see any types in the External Content Type picker, this is probably because you're not granted with the permission to access BDC Metadata store. To grant the permission, go to SharePoint Central Admin page, click on Manage Service applications, then click on Business Data Connectivity Service. On the View External Content Types page, click on Set Metadata Store Permissions in the ribbon. Then add your user name on the Set Metadata store Permissions dialog and grant permission.

5. Click OK on the New page.

Now you should be able to see a list of Customers on the SharePoint page, and be able to Create/Update/Delete the customer information. The changes will be made in the database real-time.
