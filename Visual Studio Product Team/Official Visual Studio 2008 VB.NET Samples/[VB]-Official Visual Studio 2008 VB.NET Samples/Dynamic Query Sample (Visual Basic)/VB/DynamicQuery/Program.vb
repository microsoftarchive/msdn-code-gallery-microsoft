Imports nwind
Imports System.IO
Imports System.Windows.Forms
Imports System.Linq.Dynamic
Imports System.Linq.Expressions

Namespace Dynamic
    Module Program

        Sub Main()
            ' For this sample to work, you need an active database server or SqlExpress.
            ' Here is a connection to the Data sample project that ships with Microsoft Visual Studio 2008.
            Dim dpPath = Path.GetFullPath(Path.Combine(Application.StartupPath, "..\..\Data\NORTHWND.MDF"))
            Dim sqlServerInstance = ".\SQLEXPRESS"
            Dim connString = "AttachDBFileName='" & dpPath & "';Server='" & sqlServerInstance & "';user instance=true;Integrated Security=SSPI;"

            ' Here is an alternate connect string that you can modify for your own purposes.
            ' string connString = "server=test;database=northwind;user id=test;password=test";

            Dim db As New Northwind(connString)
            db.Log = Console.Out

            Dim query _
              = db.Customers.Where("City == @0 and Orders.Count >= @1", "London", 10). _
                OrderBy("CompanyName"). _
                Select("New(CompanyName as Name, Phone)")
            Console.WriteLine(query)
            Console.ReadLine()
        End Sub

    End Module
End Namespace
