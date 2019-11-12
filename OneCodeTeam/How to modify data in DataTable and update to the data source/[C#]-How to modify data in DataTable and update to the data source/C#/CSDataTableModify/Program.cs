/****************************** Module Header ******************************\
Module Name:  Program.cs
Project:      CSDataTableModify
Copyright (c) Microsoft Corporation.

We have several ways to modify the data in DataTable. In this application, 
we will demonstrate how to use different ways to modify data in DataTable 
and update to the source.
1. We use SqlDataAdapter to fill the DataTables.
2. We set  DataTable Constraints in DataTables.
4. We use DataTable Edits to modify data.
3. We use DataRow.Delete Method and DataRowCollection.Remove Method to delete 
the rows, and then compare them.
5. We use SqlDataAdapter to update the datasource.

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
using CSDataTableModify.Properties;

namespace CSDataTableModify
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings settings = new Settings();

            #region Get Data
            String selectString = 
             @"Select [CourseID],[Year],[Title],[Credits],[DepartmentID] From [dbo].[Course];
               Select [DepartmentID],[Name],[Budget],[StartDate],[Administrator] From [dbo].[Department] ";

            DataSet dataSet = new DataSet();
            DataTable course = dataSet.Tables.Add("Course");
            DataTable department = dataSet.Tables.Add("Department");

            Console.WriteLine("Get data from database:");
            GetDataTables(settings.MySchoolConnectionString, selectString, dataSet, course, department);
            Console.WriteLine();
            #endregion

            #region Use DataTable Edits to edit the data
            String updateString = 
                    @"Update [dbo].[Course] Set [Credits]=@Credits Where [CourseID]=@CourseID;";

            course.ColumnChanged += OnColumnChanged;
            
            // Set the Credits of first row is negative value, and set the Credits of second row is plus.
            ChangeCredits(course, course.Rows[0], -1);
            ChangeCredits(course,course.Rows[1], 11);

            UpdateDataTables(settings.MySchoolConnectionString, updateString, dataSet, "Course", 
                new SqlParameter("@CourseID", SqlDbType.NVarChar, 10, "CourseID"), 
                new SqlParameter("@Credits", SqlDbType.Int, 4, "Credits"));
            Console.WriteLine("Only the Credits of second row is changed.");
            ShowDataTable(course);
            Console.WriteLine();
            #endregion

            #region Delete and Remove from DataTable
            // Create the foreign key constraint, and set the DeleteRule with Cascade.
            ForeignKeyConstraint courseDepartFK = 
                new ForeignKeyConstraint("CourseDepartFK", 
                    department.Columns["DepartmentID"], 
                    course.Columns["DepartmentID"]);
            courseDepartFK.DeleteRule = Rule.Cascade;
            courseDepartFK.UpdateRule = Rule.Cascade;
            courseDepartFK.AcceptRejectRule = AcceptRejectRule.None;
            course.Constraints.Add(courseDepartFK);

            String deleteString = @"Delete From [dbo].[Course] Where [CourseID]=@CourseID;";

            department.Rows[0].Delete();
            Console.WriteLine("If One row in Department table is deleted, the related rows "+
                "in Course table will also be deleted.");
            Console.WriteLine("Department DataTable:");
            ShowDataTable(department);
            Console.WriteLine();
            Console.WriteLine("Course DataTable:");
            ShowDataTable(course);
            Console.WriteLine();
            // Update the delete operation
            DeleteDataTables(settings.MySchoolConnectionString, deleteString, dataSet, "Course",  
                new SqlParameter("@CourseID", SqlDbType.NVarChar, 10, "CourseID"));
            Console.WriteLine("After delete operation:");
            Console.WriteLine("Course DataTable:");
            ShowDataTable(course);
            Console.WriteLine();

            course.Rows.RemoveAt(0);
            Console.WriteLine("Now we remove one row from Course:");
            ShowDataTable(course);
            DeleteDataTables(settings.MySchoolConnectionString, deleteString, dataSet, "Course",  
                new SqlParameter("@CourseID", SqlDbType.NVarChar, 10, "CourseID")); 
            #endregion

            Console.WriteLine("Please press any key to exit......");
            Console.ReadLine();
        }

        /// <summary>
        /// Use SqlDataAdapter to get data.
        /// </summary>
        private static void GetDataTables(String connectionString,String selectString,
            DataSet dataSet,params DataTable[] tables)
        {
            using (SqlDataAdapter adapter = new SqlDataAdapter())
            {               
                adapter.SelectCommand = new SqlCommand(selectString);
                adapter.SelectCommand.Connection = new SqlConnection(connectionString);

                adapter.Fill(0, 0,tables);

                foreach (DataTable table in dataSet.Tables)
                {
                    Console.WriteLine("Data in {0}:",table.TableName);
                    ShowDataTable(table);
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// Use SqlDataAdapter to update the updata operation.
        /// </summary>
        private static void UpdateDataTables(String connectionString, String updateString, 
            DataSet dataSet, String tableName, params SqlParameter[] parameters)
        {
            using (SqlDataAdapter adapter = new SqlDataAdapter())
            {
                adapter.UpdateCommand = new SqlCommand(updateString);
                adapter.UpdateCommand.Parameters.AddRange(parameters);
                adapter.UpdateCommand.Connection = new SqlConnection(connectionString);

                adapter.Update(dataSet, tableName);
            }
        }

        /// <summary>
        /// Use SqlDataAdapter to update delete operation.
        /// </summary>
        private static void DeleteDataTables(String connectionString,String deleteString,
            DataSet dataSet,String tableName,params SqlParameter[] parameters)
        {
            using (SqlDataAdapter adapter = new SqlDataAdapter())
            {
                adapter.DeleteCommand = new SqlCommand(deleteString);
                adapter.DeleteCommand.Parameters.AddRange(parameters);
                adapter.DeleteCommand.Connection = new SqlConnection(connectionString);

                adapter.Update(dataSet,tableName);
            }
        }

        /// <summary>
        /// Use DataTable Edits to modify the data.
        /// </summary>
        private static void ChangeCredits(DataTable table,DataRow row,Int32 credits)
        {
            row.BeginEdit();
            Console.WriteLine("We change row {0}",table.Rows.IndexOf(row));
            row["Credits"] = credits;
            row.EndEdit();
        }

        /// <summary>
        /// The method will be invoked when the value in DataTable is changed.
        /// </summary>
        private static void OnColumnChanged(Object sender, DataColumnChangeEventArgs args)
        {
            Int32 credits = 0;
            // If Credits is changed and the value is negative, we'll cancel the edit.
            if ((args.Column.ColumnName == "Credits")&&
                (!Int32.TryParse(args.ProposedValue.ToString(),out credits)||credits<0))
                {
                    Console.WriteLine("The value of Credits is invalid. Edit canceled.");
                    args.Row.CancelEdit();
                }
        }

        /// <summary>
        /// Display the column and value of DataTable.
        /// </summary>
        private static void ShowDataTable(DataTable table)
        {
            foreach (DataColumn col in table.Columns)
            {
                Console.Write("{0,-14}", col.ColumnName);
            }
            Console.WriteLine("{0,-14}", "RowState");

            foreach (DataRow row in table.Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                {
                    foreach (DataColumn col in table.Columns)
                    {
                        if (col.DataType.Equals(typeof(DateTime)))
                        {
                            Console.Write("{0,-14:d}", row[col,DataRowVersion.Original]);
                        }
                        else if (col.DataType.Equals(typeof(Decimal)))
                        {
                            Console.Write("{0,-14:C}", row[col, DataRowVersion.Original]);
                        }
                        else
                        {
                            Console.Write("{0,-14}", row[col, DataRowVersion.Original]);
                        }
                    }
                }
                else
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
                }
                Console.WriteLine("{0,-14}", row.RowState);
            }
        }
    }
}
