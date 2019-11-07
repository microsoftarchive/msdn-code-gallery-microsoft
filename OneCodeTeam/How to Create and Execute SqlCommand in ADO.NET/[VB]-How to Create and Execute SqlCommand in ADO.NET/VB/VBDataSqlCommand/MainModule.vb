'**************************** Module Header ******************************\
' Module Name:  MainModule.vb
' Project:      VBDataSqlCommand
' Copyright (c) Microsoft Corporation.
' 
' We can create and execute different types of SqlCommand. 
' In this application, we will demonstrate how to create and execute SqlCommand:
' 1. Create different types of SqlCommand;
' 2. Execute SqlCommand in different ways;
' 3. Display the result.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports System.Data.SqlClient
Imports VBDataSqlCommand.My

Namespace VBDataSqlCommand
    Friend Class MainModule
        Shared Sub Main(ByVal args() As String)
            Dim connectionString As String = MySettings.Default.MyConnectionString

            CountCourses(connectionString, 2012)
            Console.WriteLine()

            Console.WriteLine("The following result lists the departments that started from 2007:")
            GetDepartments(connectionString, 2007)
            Console.WriteLine()

            Console.WriteLine("Add the credits when the credits of course is lower than 4.")
            AddCredits(connectionString, 4)
            Console.WriteLine()

            Console.WriteLine("Please press any key to exit...")
            Console.ReadKey()

        End Sub

        ''' <summary>
        ''' Count the courses of specified year.
        ''' </summary>
        Private Shared Sub CountCourses(ByVal connectionString As String, ByVal year As Int32)
            Dim commandText As String =
                "Select Count([CourseID]) FROM [MySchool].[dbo].[Course] Where Year=@Year"
            Dim parameterYear As New SqlParameter("@Year", SqlDbType.Int)
            parameterYear.Value = year

            Dim oValue As Object = SqlHelper.ExecuteScalar(connectionString, commandText,
                                                           CommandType.Text, parameterYear)
            Dim count As Int32
            If Int32.TryParse(oValue.ToString(), count) Then
                Console.WriteLine("There {0} {1} course{2} in {3}.", If(count > 1, "are", "is"), count,
                                  If(count > 1, "s", Nothing), year)
            End If
        End Sub

        ''' <summary>
        ''' Display the Departments that start from the specified year.
        ''' </summary>
        Private Shared Sub GetDepartments(ByVal connectionString As String, ByVal year As Int32)
            Dim commandText As String = "dbo.GetDepartmentsOfSpecifiedYear"

            ' Specify the year of StartDate
            Dim parameterYear As New SqlParameter("@Year", SqlDbType.Int)
            parameterYear.Value = year

            ' When the direction of parameter is set as Output, you can get the value after 
            ' executing the command.
            Dim parameterBudget As New SqlParameter("@BudgetSum", SqlDbType.Money)
            parameterBudget.Direction = ParameterDirection.Output

            Using reader As SqlDataReader = SqlHelper.ExecuteReader(connectionString, commandText,
                                                                    CommandType.StoredProcedure,
                                                                    parameterYear, parameterBudget)
                Console.WriteLine("{0,-20}{1,-20}{2,-20}{3,-20}", "Name", "Budget", "StartDate",
                                  "Administrator")
                Do While reader.Read()
                    Console.WriteLine("{0,-20}{1,-20:C}{2,-20:d}{3,-20}", reader("Name"),
                                      reader("Budget"), reader("StartDate"), reader("Administrator"))
                Loop
            End Using
            Console.WriteLine("{0,-20}{1,-20:C}", "Sum:", parameterBudget.Value)
        End Sub

        ''' <summary>
        ''' If credits of course is lower than the certain value, the method will add the credits.
        ''' </summary>
        Private Shared Sub AddCredits(ByVal connectionString As String, ByVal creditsLow As Int32)
            Dim commandText As String =
                "Update [MySchool].[dbo].[Course] Set Credits=Credits+1 Where Credits<@Credits"

            Dim parameterCredits As New SqlParameter("@Credits", creditsLow)

            Dim rows As Int32 = SqlHelper.ExecuteNonQuery(connectionString, commandText,
                                                          CommandType.Text, parameterCredits)

            Console.WriteLine("{0} row{1} {2} updated.", rows, If(rows > 1, "s", Nothing),
                              If(rows > 1, "are", "is"))
        End Sub
    End Class
End Namespace
