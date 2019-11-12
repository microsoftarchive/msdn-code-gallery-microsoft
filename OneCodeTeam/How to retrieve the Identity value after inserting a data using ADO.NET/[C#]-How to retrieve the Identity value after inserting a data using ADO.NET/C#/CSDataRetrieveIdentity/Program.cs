/****************************** Module Header ******************************\
Module Name:  Program.cs
Project:      CSDataRetrieveIdentity
Copyright (c) Microsoft Corporation.

We often set the column as identity when the values in the column must be unique. 
And sometimes we need the identity value of new data. In this application, we 
will demonstrate how to retrieve the identity values:
1. Create a stored procedure to insert data and return identity value;
2. Execute a command to insert the new data and display the result;
3. Use SqlDataAdapter to insert new data and display the result.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using CSDataRetrieveIdentity.Properties;

namespace CSDataRetrieveIdentity
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings settings = new Settings();
            String SqlDbConnectionString = settings.SqlDbConnectionString;

            InsertPerson(SqlDbConnectionString, "Janice", "Galvin");
            Console.WriteLine();

            InsertPersonInAdapter(SqlDbConnectionString, "Peter", "Krebs");
            Console.WriteLine();

            String oledbConnectionString = settings.OlelDbConnectionString;
            InsertPersonInJet4Database(oledbConnectionString, "Janice", "Galvin");
            Console.WriteLine();

            Console.WriteLine("Please press any key to exit.....");
            Console.ReadKey();
        }

        /// <summary>
        /// Using stored procedure to insert a new row and retrieve the identity value
        /// </summary>
        static void InsertPerson(String connectionString, String firstName, String lastName)
        {
            String commandText = "dbo.InsertPerson";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@FirstName", firstName));
                    cmd.Parameters.Add(new SqlParameter("@LastName", lastName));
                    SqlParameter personId = new SqlParameter("@PersonID", SqlDbType.Int);
                    personId.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(personId);

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    Console.WriteLine("Person Id of new person:{0}", personId.Value);
                }
            }
        }

        /// <summary>
        /// Using stored procedure in adapter to insert new rows and update the identity value.
        /// </summary>
        static void InsertPersonInAdapter(String connectionString, String firstName, String lastName)
        {
            String commandText = "dbo.InsertPerson";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter mySchool =
                    new SqlDataAdapter("Select PersonID,FirstName,LastName from [dbo].[Person]", conn);

                mySchool.InsertCommand = new SqlCommand(commandText, conn);
                mySchool.InsertCommand.CommandType = CommandType.StoredProcedure;

                mySchool.InsertCommand.Parameters.Add(
                    new SqlParameter("@FirstName", SqlDbType.NVarChar, 50, "FirstName"));
                mySchool.InsertCommand.Parameters.Add(
                    new SqlParameter("@LastName", SqlDbType.NVarChar, 50, "LastName"));

                SqlParameter personId = mySchool.InsertCommand.Parameters.Add(
                    new SqlParameter("@PersonID", SqlDbType.Int, 0, "PersonID"));
                personId.Direction = ParameterDirection.Output;

                DataTable persons = new DataTable();
                mySchool.Fill(persons);

                DataRow newPerson = persons.NewRow();
                newPerson["FirstName"] = firstName;
                newPerson["LastName"] = lastName;
                persons.Rows.Add(newPerson);

                mySchool.Update(persons);
                Console.WriteLine("Show all persons:");
                ShowDataTable(persons, 14);
            }
        }

        /// <summary>
        /// For a Jet 4.0 database, we need use the sigle statement and event handler to insert new rows 
        /// and retrieve the identity value.
        /// </summary>
        static void InsertPersonInJet4Database(String connectionString, String firstName, String lastName)
        {
            String commandText = "Insert into Person(FirstName,LastName) Values(?,?)";
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                OleDbDataAdapter mySchool =
                    new OleDbDataAdapter("Select PersonID,FirstName,LastName from Person", conn);

                #region Create Insert Command
                mySchool.InsertCommand = new OleDbCommand(commandText, conn);
                mySchool.InsertCommand.CommandType = CommandType.Text;

                mySchool.InsertCommand.Parameters.Add(
                    new OleDbParameter("@FirstName", OleDbType.VarChar, 50, "FirstName"));
                mySchool.InsertCommand.Parameters.Add(
                    new OleDbParameter("@LastName", OleDbType.VarChar, 50, "LastName"));
                mySchool.InsertCommand.UpdatedRowSource = UpdateRowSource.Both;
                #endregion

                DataTable persons = CreatePersonsTable();

                mySchool.Fill(persons);

                DataRow newPerson = persons.NewRow();
                newPerson["FirstName"] = firstName;
                newPerson["LastName"] = lastName;
                persons.Rows.Add(newPerson);

                DataTable dataChanges = persons.GetChanges();

                mySchool.RowUpdated += OnRowUpdated;

                mySchool.Update(dataChanges);

                Console.WriteLine("Data before merging:");
                ShowDataTable(persons, 14);
                Console.WriteLine();

                persons.Merge(dataChanges);
                persons.AcceptChanges();

                Console.WriteLine("Data after merging");
                ShowDataTable(persons, 14);
            }
        }

        static void OnRowUpdated(object sender, OleDbRowUpdatedEventArgs e)
        {
            if (e.StatementType == StatementType.Insert)
            {
                // Retrieve the identity value
                OleDbCommand cmdNewId = new OleDbCommand("Select @@IDENTITY", e.Command.Connection);
                e.Row["PersonID"] = (Int32)cmdNewId.ExecuteScalar();

                // After the status is changed, the original values in the row are preserved. And the 
                // Merge method will be called to merge the new identity value into the original DataTable.
                e.Status = UpdateStatus.SkipCurrentRow;
            }
        }

        /// <summary>
        /// Create the Persons table before filling.
        /// </summary>
        private static DataTable CreatePersonsTable()
        {
            DataTable persons = new DataTable();

            DataColumn personId = new DataColumn();
            personId.DataType = Type.GetType("System.Int32");
            personId.ColumnName = "PersonID";
            personId.AutoIncrement = true;
            personId.AutoIncrementSeed = 0;
            personId.AutoIncrementStep = -1;
            persons.Columns.Add(personId);

            DataColumn firstName = new DataColumn();
            firstName.DataType = Type.GetType("System.String");
            firstName.ColumnName = "FirstName";
            persons.Columns.Add(firstName);

            DataColumn lastName = new DataColumn();
            lastName.DataType = Type.GetType("System.String");
            lastName.ColumnName = "LastName";
            persons.Columns.Add(lastName);

            DataColumn[] pkey = { personId };
            persons.PrimaryKey = pkey;

            return persons;
        }

        private static void ShowDataTable(DataTable table, Int32 length)
        {
            foreach (DataColumn col in table.Columns)
            {
                Console.Write("{0,-" + length + "}", col.ColumnName);
            }
            Console.WriteLine();

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn col in table.Columns)
                {
                    if (col.DataType.Equals(typeof(DateTime)))
                    {
                        Console.Write("{0,-" + length + ":d}", row[col]);
                    }
                    else if (col.DataType.Equals(typeof(Decimal)))
                    {
                        Console.Write("{0,-" + length + ":C}", row[col]);
                    }
                    else
                    {
                        Console.Write("{0,-" + length + "}", row[col]);
                    }
                }

                Console.WriteLine();
            }
        }
    }
}
