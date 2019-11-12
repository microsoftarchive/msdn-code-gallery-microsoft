using System;
using System.Collections.Generic;
using System.Data.SqlClient;
namespace CSNETCoreBulkInsert
{
    public class BulkCopyHelper
    {
        public string ConnectionString { get; set; }
        public List<Student> Students { get; set; }
        
        public BulkCopyHelper(string connStr)
        {
            Students = new List<Student>();
            ConnectionString = connStr;
        }
        
        public void BulkInsert(string tableName)
        {
            using(SqlConnection connection =new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                using (SqlBulkCopy bulkCopy =new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, trans))
                {
                    bulkCopy.BatchSize = 100;
                    bulkCopy.DestinationTableName = tableName;
                    try {
                        StudentDbDataReader studentDR = new StudentDbDataReader(Students);
                        bulkCopy.WriteToServer(studentDR);
                        trans.Commit();
                    }
                    catch(Exception ex)
                    {
                        trans.Rollback();
                        connection.Close();
                        Console.WriteLine(ex.Message);
                    }
                }
                connection.Close();

            }
        }

    }
}