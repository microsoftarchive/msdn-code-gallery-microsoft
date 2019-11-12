/****************************** Module Header ******************************\
Module Name:  Program.cs
Project:      CSDataSqlCommand
Copyright (c) Microsoft Corporation.

We can create and execute different types of SqlCommand. 
In this application, we will demonstrate how to create and execute SqlCommand:

1. Create different types of SqlCommand;
2. Execute SqlCommand in different ways;
3. Display the result.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Data;
using System.Data.SqlClient;
using CSDataSqlCommand.Properties;

namespace CSDataSqlCommand
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings settings = new Settings();
            String connectionString = settings.MyConnectionString;

            CountCourses(connectionString, 2012);
            Console.WriteLine();

            Console.WriteLine("The following result lists the departments that started from 2007:");
            GetDepartments(connectionString, 2007);
            Console.WriteLine();

            Console.WriteLine("Add the credits when the credits of course is lower than 4.");
            AddCredits(connectionString, 4);
            Console.WriteLine();

            Console.WriteLine("Please press any key to exit...");
            Console.ReadKey();

        }

        /// <summary>
        /// Count the courses of specified year.
        /// </summary>
        static void CountCourses(String connectionString, Int32 year)
        {
            String commandText = "Select Count([CourseID]) FROM [MySchool].[dbo].[Course] Where Year=@Year";
            SqlParameter parameterYear = new SqlParameter("@Year", SqlDbType.Int);
            parameterYear.Value = year;

            Object oValue = SqlHelper.ExecuteScalar(connectionString, commandText, CommandType.Text,
                parameterYear);
            Int32 count;
            if (Int32.TryParse(oValue.ToString(), out count))
            {
                Console.WriteLine("There {0} {1} course{2} in {3}.", count > 1 ? "are" : "is",
                    count, count > 1 ? "s" : null, year);
            }
        }

        /// <summary>
        /// Display the Departments that start from the specified year.
        /// </summary>
        static void GetDepartments(String connectionString, Int32 year)
        {
            String commandText = "dbo.GetDepartmentsOfSpecifiedYear";

            // Specify the year of StartDate
            SqlParameter parameterYear = new SqlParameter("@Year", SqlDbType.Int);
            parameterYear.Value = year;

            // When the direction of parameter is set as Output, you can get the value after 
            // executing the command.
            SqlParameter parameterBudget = new SqlParameter("@BudgetSum", SqlDbType.Money);
            parameterBudget.Direction = ParameterDirection.Output;

            using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, commandText,
                CommandType.StoredProcedure, parameterYear, parameterBudget))
            {
                Console.WriteLine("{0,-20}{1,-20}{2,-20}{3,-20}", "Name", "Budget", "StartDate",
                    "Administrator");
                while (reader.Read())
                {
                    Console.WriteLine("{0,-20}{1,-20:C}{2,-20:d}{3,-20}", reader["Name"],
                        reader["Budget"], reader["StartDate"], reader["Administrator"]);
                }
            }
            Console.WriteLine("{0,-20}{1,-20:C}", "Sum:", parameterBudget.Value);
        }

        /// <summary>
        /// If credits of course is lower than the certain value, the method will add the credits.
        /// </summary>
        static void AddCredits(String connectionString, Int32 creditsLow)
        {
            String commandText = "Update [MySchool].[dbo].[Course] Set Credits=Credits+1 Where Credits<@Credits";

            SqlParameter parameterCredits = new SqlParameter("@Credits", creditsLow);

            Int32 rows = SqlHelper.ExecuteNonQuery(connectionString, commandText, CommandType.Text, parameterCredits);

            Console.WriteLine("{0} row{1} {2} updated.", rows, rows > 1 ? "s" : null,
                rows > 1 ? "are" : "is");
        }
    }
}
