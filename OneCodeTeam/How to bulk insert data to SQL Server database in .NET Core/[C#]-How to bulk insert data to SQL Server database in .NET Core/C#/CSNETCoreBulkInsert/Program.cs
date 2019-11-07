using System;
using System.Collections.Generic;
using System.Data;
namespace CSNETCoreBulkInsert
{
    class Program
    {
        static void Main(string[] args)
        {
            // get sample data
            List<Student> students = GetSampleData();
            // new helper
            BulkCopyHelper helper = new BulkCopyHelper("Server=.;Database=BulkInsertTestDB;Trusted_Connection=True;");
            // add sample data to insert
            helper.Students.AddRange(students);
            Console.WriteLine("Start.");
            // invoke bulk insert method
            helper.BulkInsert("dbo.student");
            Console.WriteLine("Finished. Press any key to continue...");
            Console.ReadKey();
        }

        static private List<Student> GetSampleData()
        {
            List<Student> students = new List<Student>();
            students.Add(new Student("Jakey", 22));
            students.Add(new Student("July", 23));
            students.Add(new Student("Angel", 22));
            students.Add(new Student("Windy", 25));
            students.Add(new Student("Anny", 24));
            students.Add(new Student("Eric", 21));
            return students;
        }

    }
}