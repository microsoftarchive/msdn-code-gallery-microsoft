==============================================================================
 ASP.NET APPLICATION : VBASPNETSearchEngine Project Overview
==============================================================================

//////////////////////////////////////////////////////////////////////////////
Summary:

This sample shows how to implement a simple search engine in an ASP.NET web site.

//////////////////////////////////////////////////////////////////////////////
Demo the Sample:

Open Default.aspx page, input one or more keywords into the text box. 
Click the submit button.

//////////////////////////////////////////////////////////////////////////////
Code Logical:

1. Create the database.
   a. Create a SQL Server database named "MyDatabase.mdf" within App_Data folder.
   b. Create a Table named "Articles" in the database.

      ID       bigint (Primary Key)
      Title    nvarchar(50)
      Content  varchar(MAX)

   c. Input some sample records to the Table.

2. Data Access.
   a. Create a class named "Article" represents a record.
   b. Create a class named "DataAccess" to access database. This class contains 
      public methods GetArticle(), GetAll() and Search(). Within Search() method,
      the key code is generating a complex Sql command which is used to search database.

        ' Generate a complex Sql command.
        Dim sqlBuilder As New StringBuilder()
        sqlBuilder.Append("select * from [Articles] where ")
        For Each item As String In keywords
            sqlBuilder.AppendFormat("([Title] like '%{0}%' or [Content] like '%{0}%') and ", item)
        Next

        ' Remove unnecessary string " and " at the end of the command.
        Dim sql As String = sqlBuilder.ToString(0, sqlBuilder.Length - 5)

3. Search Page.
   The key controls of this page is TextBox control named "txtKeyWords" which 
   is used to input keywords, and Repeater control which is used to display
   result.
   And there is a JavaScript function which is used to hightlight keywords
   in the result.

        for (var i = 0; i < keywords.length; i++)
        {
            var a = new RegExp(keywords[i], "igm");
            container.innerHTML = container.innerHTML.replace(a, "<span style='background:#FF0;'>$0</span>");
        }

4. The Detail Page.
   This page receives a parameter from Query String named "id", and then call 
   DataAccess class to retrieve a individual record from database to show in the page.
   

//////////////////////////////////////////////////////////////////////////////
References:

SQL Server and ADO.NET
http://msdn.microsoft.com/en-us/library/kb9s9ks0.aspx

Connecting to a Data Source (ADO.NET)
http://msdn.microsoft.com/en-us/library/32c5dh3b.aspx

LIKE (Transact-SQL)
http://msdn.microsoft.com/en-us/library/ms179859.aspx

Repeater Web Server Control Overview
http://msdn.microsoft.com/en-us/library/x8f2zez5.aspx

How to: Pass Values Between ASP.NET Web Pages
http://msdn.microsoft.com/en-us/library/6c3yckfw.aspx