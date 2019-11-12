/************************************* Module Header **************************************\
 * Module Name:  Program.cs
 * Project:      CSUseADO
 * Copyright (c) Microsoft Corporation.
 *
 * The CSUseADO sample demonstrates the Microsoft ActiveX Data Objects(ADO) technology to 
 * access databases using Visual C#. It shows the basic structure of connecting to a data 
 * source, issuing SQL commands, using the Recordset object and performing the cleanup.  
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\******************************************************************************************/

using System;

class Program
{
    static void Main(string[] args)
    {
        ADODB.Connection conn = null;
        ADODB.Recordset rs = null;



        try
        {
            ////////////////////////////////////////////////////////////////////////////////
            // Connect to the data source.
            // 

            Console.WriteLine("Connecting to the database ...");

            // Get the connection string 
            string connStr = string.Format("Provider=SQLOLEDB;Data Source={0};Initial Catalog={1};Integrated Security=SSPI",
                ".\\sqlexpress", "SQLServer2005DB");

            // Open the connection
            conn = new ADODB.Connection();
            conn.Open(connStr, null, null, 0);


            ////////////////////////////////////////////////////////////////////////////////
            // Build and Execute an ADO Command.
            // It can be a SQL statement (SELECT/UPDATE/INSERT/DELETE), or a stored 
            // procedure call. Here is the sample of an INSERT command.
            // 

            Console.WriteLine("Inserting a record to the CountryRegion table...");

            // 1. Create a command object
            ADODB.Command cmdInsert = new ADODB.Command();

            // 2. Assign the connection to the command
            cmdInsert.ActiveConnection = conn;

            // 3. Set the command text
            // SQL statement or the name of the stored procedure 
            cmdInsert.CommandText = "INSERT INTO CountryRegion(CountryRegionCode, Name, ModifiedDate)"
                + " VALUES (?, ?, ?)";

            // 4. Set the command type
            // ADODB.CommandTypeEnum.adCmdText for oridinary SQL statements; 
            // ADODB.CommandTypeEnum.adCmdStoredProc for stored procedures.
            cmdInsert.CommandType = ADODB.CommandTypeEnum.adCmdText;

            // 5. Append the parameters

            // Append the parameter for CountryRegionCode (nvarchar(20)
            ADODB.Parameter paramCode = cmdInsert.CreateParameter(
                "CountryRegionCode",                        // Parameter name
                ADODB.DataTypeEnum.adVarChar,               // Parameter type (nvarchar(20))
                ADODB.ParameterDirectionEnum.adParamInput,  // Parameter direction
                20,                                         // Max size of value in bytes
                "ZZ"+DateTime.Now.Millisecond);             // Parameter value
            cmdInsert.Parameters.Append(paramCode);

            // Append the parameter for Name (nvarchar(200))
            ADODB.Parameter paramName = cmdInsert.CreateParameter(
                "Name",                                     // Parameter name
                ADODB.DataTypeEnum.adVarChar,               // Parameter type (nvarchar(200))
                ADODB.ParameterDirectionEnum.adParamInput,  // Parameter direction
                200,                                        // Max size of value in bytes
                "Test Region Name");                        // Parameter value
            cmdInsert.Parameters.Append(paramName);

            // Append the parameter for ModifiedDate (datetime)
            ADODB.Parameter paramModifiedDate = cmdInsert.CreateParameter(
                "ModifiedDate",                             // Parameter name
                ADODB.DataTypeEnum.adDate,                  // Parameter type (datetime)
                ADODB.ParameterDirectionEnum.adParamInput,  // Parameter direction
                -1,                                         // Max size (ignored for datetime)
                DateTime.Now);                              // Parameter value
            cmdInsert.Parameters.Append(paramModifiedDate);

          
            // 6. Execute the command
            object nRecordsAffected = Type.Missing;
            object oParams = Type.Missing;
            cmdInsert.Execute(out nRecordsAffected, ref oParams,
                (int)ADODB.ExecuteOptionEnum.adExecuteNoRecords);


            ////////////////////////////////////////////////////////////////////////////////
            // Use the Recordset Object.
            // http://msdn.microsoft.com/en-us/library/ms681510.aspx
            // Recordset represents the entire set of records from a base table or the  
            // results of an executed command. At any time, the Recordset object refers to  
            // only a single record within the set as the current record.
            // 

            Console.WriteLine("Enumerating the records in the CountryRegion table");

            // 1. Create a Recordset object
            rs = new ADODB.Recordset();

            // 2. Open the Recordset object
            string strSelectCmd = "SELECT * FROM CountryRegion"; // WHERE ...
            rs.Open(strSelectCmd,                                // SQL statement / table,view name / 
                                                                 // stored procedure call / file name
                conn,                                            // Connection / connection string
                ADODB.CursorTypeEnum.adOpenForwardOnly,          // Cursor type. (forward-only cursor)
                ADODB.LockTypeEnum.adLockOptimistic,	         // Lock type. (locking records only 
                                                                 // when you call the Update method.
                (int)ADODB.CommandTypeEnum.adCmdText);	         // Evaluate the first parameter as
                                                                 // a SQL command or stored procedure.

            // 3. Enumerate the records by moving the cursor forward

            // Move to the first record in the Recordset
            rs.MoveFirst(); 
            while (!rs.EOF)
            {
                // When dumping a SQL-Nullable field in the table, need to test it for DBNull.Value.
                string code = (rs.Fields["CountryRegionCode"].Value == DBNull.Value) ?
                    "(DBNull)" : rs.Fields["CountryRegionCode"].Value.ToString();

                string name = (rs.Fields["Name"].Value == DBNull.Value) ?
                    "(DBNull)" : rs.Fields["Name"].Value.ToString();

                DateTime modifiedDate = (rs.Fields["ModifiedDate"].Value == DBNull.Value) ?
                    DateTime.MinValue : (DateTime)rs.Fields["ModifiedDate"].Value;

                Console.WriteLine(" {2} \t{0}\t{1}", code, name, modifiedDate.ToString("yyyy-MM-dd"));

                // Move to the next record
                rs.MoveNext();   
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("The application throws the error: {0}", ex.Message);
            if (ex.InnerException != null)
                Console.WriteLine("Description: {0}", ex.InnerException.Message);
        }
        finally
        {
            ////////////////////////////////////////////////////////////////////////////////
            // Clean up objects before exit.
            // 

            Console.WriteLine("Closing the connections ...");

            // Close the record set if it is open
            if (rs != null && rs.State == (int)ADODB.ObjectStateEnum.adStateOpen)
                rs.Close();

            // Close the connection to the database if it is open
            if (conn != null && conn.State == (int)ADODB.ObjectStateEnum.adStateOpen)
                conn.Close();
        }
    }
}

