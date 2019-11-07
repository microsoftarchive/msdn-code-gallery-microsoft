/****************************** Module Header ******************************\
Module Name:  Program.cs
Project:      CSEFPivotOperation
Copyright (c) Microsoft Corporation.

This sample demonstrates how to implement the Pivot and Unpivot operation in 
Entity Framework.
In this sample, we use two classes to store the data from the database by EF,
and then display the data in Pivot/Unpivot format.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace CSEFPivotOperation
{
    class Program
    {
        static void Main(string[] args)
        {
            PivotInEF();
            Console.WriteLine();

            UnpivotInEF();
            Console.WriteLine();

            Console.WriteLine("Press any key to exit.....");
            Console.ReadKey();
        }

        /// <summary>
        /// Execute the Pivot operation in EF.
        /// </summary>
        static void PivotInEF()
        {
            List<PivotRow<Person, String, Decimal>> studentGrade = null;
            List<String> courses = null;

            using (MySchoolEntities school = new MySchoolEntities())
            {
                // Get the data from the database.
                studentGrade = (from sg in school.StudentGrades
                                group sg by sg.StudentID into sgGroup
                                select new PivotRow<Person, String, Decimal>
                                {
                                    ObjectId = sgGroup.Select(g => g.Person).FirstOrDefault(),
                                    Attributes = sgGroup.Select(g => g.Course.Title),
                                    Values = sgGroup.Select(g => g.Grade)
                                }
                    ).ToList();

                // Get the list of attributes.
                courses = school.Courses.Select(c => c.Title).ToList();
            }

            // Get the Pivot table.
            using (DataTable pivotTable = PivotRow<Person, String, Decimal>.GetPivotTable(courses, studentGrade))
            {
                Console.WriteLine("It's the result of Pivot in EF:");
                foreach (DataColumn col in pivotTable.Columns)
                {
                    Console.Write("{0,-15}", col.ColumnName);
                }
                Console.WriteLine();

                foreach (DataRow row in pivotTable.Rows)
                {
                    Person p = (Person)row[0];
                    Console.Write("{0,-15}", p.FirstName + " " + p.LastName);

                    for (int i = 1; i < pivotTable.Columns.Count; i++)
                    {
                        Console.Write("{0,-15}", row[i].GetType().Equals(typeof(DBNull)) ? "NULL" : row[i]);
                    }
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// Execute the Upivot operation in EF.
        /// </summary>
        static void UnpivotInEF()
        {
            // set the function list of attributes.
            Dictionary<String, Func<Person, DateTime?>> attrFuncList = new Dictionary<string, Func<Person, DateTime?>>();
            attrFuncList["HireDate"] = p => p.HireDate;
            attrFuncList["EnrollmentDate"] = p => p.EnrollmentDate;

            // Store the Unpivot result.
            IEnumerable<UnpivotRow<String, String, DateTime>> result = null;

            using (MySchoolEntities school = new MySchoolEntities())
            {
                // Get the data from databasy by EF.
                var persons = (from person in school.People
                               select person).ToList();

                // Get the query result of every attribute.
                foreach (String key in attrFuncList.Keys)
                {
                    String k = key;
                    IEnumerable<UnpivotRow<String, String, DateTime>> query =
                        (from person in persons
                         where attrFuncList[k](person) != null
                         select new UnpivotRow<String, String, DateTime>
                         {
                             ObjectId = person.FirstName + " " + person.LastName,
                             Attribute = k,
                             // Get the value of a certain attribute.
                             Value = (DateTime)attrFuncList[k](person)
                         });

                    // Concat the results.
                    result = result == null ? query : result.Concat(query);
                }
            }

            Console.WriteLine("It's the result of Unpivot In EF:");
            Console.WriteLine("{0,-15}{1,-15}{2,-15}", "ObjectId", "AttributeName", "Value");
            foreach (var row in result.ToList())
            {
                Console.WriteLine("{0,-15}{1,-15}{2,-15}",
                    row.ObjectId, row.Attribute, row.Value.ToShortDateString());
            }
        }
    }
}
