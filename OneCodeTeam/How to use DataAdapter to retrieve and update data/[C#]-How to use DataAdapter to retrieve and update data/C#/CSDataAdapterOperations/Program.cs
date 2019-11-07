/****************************** Module Header ******************************\
Module Name:  Program.cs
Project:      CSDataAdapterOperations
Copyright (c) Microsoft Corporation.

We can use DataAdapter to retrieve and update the data, and sometimes 
the features of DataAdapter make some specific operations easier. In this 
sample, we will demonstrate how to use DataAdapter to retrieve and update 
the data:
1. Retrieve Data
a. Use DataAdapter.AcceptChangesDuringFill Property to clone the data in 
database.
 If the property is set as false, AcceptChanges is not called when filling 
the table, and the newly added rows are treated as inserted rows. So we can 
use these rows to insert the new rows into the database.

b. Use DataAdapter.TableMappings Property to define the mapping between the 
source table and DataTable.

c. Use DataAdapter.FillLoadOption Property to determine how the adapter fills 
the DataTable from the DbDataReader.
When we create a DataTable, we can only write the data from database to the 
current version or the original version by setting the property as the 
LoadOption.Upsert or the LoadOption.PreserveChanges.

2. Update table
Use DbDataAdapter.UpdateBatchSize Property to perform batch operations.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using CSDataAdapterOperations.Properties;

namespace CSDataAdapterOperations
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings settings = new Settings();

            #region Copy the data from the database.
            // Get the table Department and Course from the database.
            String selectString = @"SELECT [DepartmentID],[Name],[Budget],[StartDate],[Administrator]
                                     FROM [MySchool].[dbo].[Department];

                                   SELECT [CourseID],@Year as [Year],Max([Title]) as [Title],
                                   Max([Credits]) as [Credits],Max([DepartmentID]) as [DepartmentID]
                                   FROM [MySchool].[dbo].[Course]
                                   Group by [CourseID]";

            DataSet mySchool = new DataSet();

            SqlCommand selectCommand = new SqlCommand(selectString);
            SqlParameter parameter = selectCommand.Parameters.Add("@Year", SqlDbType.SmallInt, 2);
            parameter.Value = new Random(DateTime.Now.Millisecond).Next(9999);

            // Use DataTableMapping to map the source tables and the destination tables.
            DataTableMapping[] tableMappings = {new DataTableMapping("Table", "Department"),
                                              new DataTableMapping("Table1", "Course")};
            CopyData(mySchool, settings.MySchoolConnectionString, selectCommand, tableMappings);

            Console.WriteLine("The following tables are from the database.");
            foreach (DataTable table in mySchool.Tables)
            {
                Console.WriteLine(table.TableName);
                ShowDataTable(table);
            } 
            #endregion

            #region Roll back the changes
            DataTable department = mySchool.Tables["Department"];
            DataTable course = mySchool.Tables["Course"];

            department.Rows[0]["Name"] = "New" + department.Rows[0][1];
            course.Rows[0]["Title"] = "New" + course.Rows[0]["Title"];
            course.Rows[0]["Credits"] = 10;

            Console.WriteLine("After we changed the tables:");
            foreach (DataTable table in mySchool.Tables)
            {
                Console.WriteLine(table.TableName);
                ShowDataTable(table);
            }

            department.RejectChanges();
            Console.WriteLine("After use the RejectChanges method in Department table to roll back the changes:");
            ShowDataTable(department);

            DataColumn[] primaryColumns = { course.Columns["CourseID"] };
            DataColumn[] resetColumns = { course.Columns["Title"] };
            ResetCourse(course, settings.MySchoolConnectionString, primaryColumns, resetColumns);
            Console.WriteLine("After use the ResetCourse method in Course table to roll back the changes:");
            ShowDataTable(course); 
            #endregion

            #region Batch update the table.
            String insertString = @"Insert into [MySchool].[dbo].[Course]([CourseID],[Year],[Title],
                                   [Credits],[DepartmentID]) 
             values (@CourseID,@Year,@Title,@Credits,@DepartmentID)";
            SqlCommand insertCommand = new SqlCommand(insertString);
            insertCommand.Parameters.Add("@CourseID", SqlDbType.NVarChar, 10, "CourseID");
            insertCommand.Parameters.Add("@Year", SqlDbType.SmallInt, 2, "Year");
            insertCommand.Parameters.Add("@Title", SqlDbType.NVarChar, 100, "Title");
            insertCommand.Parameters.Add("@Credits", SqlDbType.Int, 4, "Credits");
            insertCommand.Parameters.Add("@DepartmentID", SqlDbType.Int, 4, "DepartmentID");

            const Int32 batchSize = 10;
            BatchInsertUpdate(course, settings.MySchoolConnectionString, insertCommand, batchSize); 
            #endregion

            Console.WriteLine();
            Console.WriteLine("Press any key to exit.....");
            Console.ReadKey();
        }

        /// <summary>
        /// Copy the Data from the database.
        /// </summary>
        private static void CopyData(DataSet dataSet,String connectionString,
            SqlCommand selectCommand,DataTableMapping[] tableMappings)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                selectCommand.Connection = connection;

                connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(selectCommand))
                {
                    adapter.TableMappings.AddRange(tableMappings);
                    // If set the AcceptChangesDuringFill as the false, AcceptChanges will not be 
                    // called on a DataRow after it is added to the DataTable during any of the 
                    // Fill operations.
                    adapter.AcceptChangesDuringFill = false;

                    adapter.Fill(dataSet);
                }
            }
        }
                
        /// <summary>
        /// Roll back only one column or serveral columns data of the Course table by calling 
        /// ResetDataTable method.
        /// </summary>
        /// <param name="table">the Course table</param>
        /// <param name="connectionString">the connection string</param>
        /// <param name="primaryColumns">the primary columns of table</param>
        /// <param name="resetColumns">the columns whose data need to be reset</param>
        private static void ResetCourse(DataTable table, String connectionString,
            DataColumn[] primaryColumns,DataColumn[] resetColumns)
        {
            table.PrimaryKey = primaryColumns;

            // Build the query string
            String primaryCols=String.Join(",",primaryColumns.Select(col => col.ColumnName));
            String resetCols = String.Join(",",
                resetColumns.Select(col=>"Max("+col.ColumnName+") as "+col.ColumnName));

            String selectString = String.Format("Select {0},{1} from Course Group by {0}",
                primaryCols,resetCols) ;

            SqlCommand selectCommand = new SqlCommand(selectString);

            ResetDataTable(table, connectionString, selectCommand);
        }

        /// <summary>
        /// RejectChanges will roll back all changes that have been made to the table since it was loaded, 
        /// or the last time AcceptChanges was called. If we want to copy all the data from the database, 
        /// we may lose all the data after call the RejectChanges method. The ResetDataTable method can 
        /// just roll back one or more columns of data.
        /// </summary>
        private static void ResetDataTable(DataTable table, String connectionString,
            SqlCommand selectCommand)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                selectCommand.Connection = connection;

                connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(selectCommand))
                {
                    // The incoming values for this row will be written to the current version of each 
                    // column. The original version of each column's data will not be changed.
                    adapter.FillLoadOption = LoadOption.Upsert;

                    adapter.Fill(table);
                }
            }
        }

        private static void BatchInsertUpdate(DataTable table, String connectionString, 
            SqlCommand insertCommand, Int32 batchSize)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                insertCommand.Connection = connection;
                // When setting UpdateBatchSize to a value other than 1, all the commands 
                // associated with the SqlDataAdapter have to have their UpdatedRowSource 
                // property set to None or OutputParameters. An exception is thrown otherwise.
                insertCommand.UpdatedRowSource = UpdateRowSource.None;

                connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter())
                {
                    adapter.InsertCommand = insertCommand;
                    // Gets or sets the number of rows that are processed in each round-trip to the server.
                    // Setting it to 1 disables batch updates, as rows are sent one at a time.
                    adapter.UpdateBatchSize = batchSize;

                    adapter.Update(table);

                    Console.WriteLine("Successfully to update the table.");
                }
            }
        }

        private static void ShowDataTable(DataTable table)
        {
            foreach (DataColumn col in table.Columns)
            {
                Console.Write("{0,-14}", col.ColumnName);
            }
            Console.WriteLine("{0,-14}", "RowState");

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn col in table.Columns)
                {
                    if (col.DataType.Equals(typeof(DateTime)))
                    {
                        Console.Write("{0,-14:d}", row[col]);
                    }
                    else if (col.DataType.Equals(typeof(Decimal)))
                    {
                        Console.Write("{0,-14:C}", row[col]);
                    }
                    else
                    {
                        Console.Write("{0,-14}", row[col]);
                    }
                }
                Console.WriteLine("{0,-14}",row.RowState);
            }
        }
    }
}
