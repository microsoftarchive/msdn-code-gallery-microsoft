/****************************** Module Header ******************************\
Module Name:  Program.cs
Project:      CSEFBetweenOperator
Copyright (c) Microsoft Corporation.

This sample demonstrates how to implement the Between operation in Entity 
Framework.
In this sample, we use two ways to implement the Between operation in Enitity
Framework:
1. Use the Entity SQL;
2. Use the extension method and expression tree.

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
using System.Data.Objects;

namespace CSEFBetweenOperator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Use the Entity SQL to implement the Between operation
            IEnumerable<Course> courses = GetCoursesByEntitySQL();
            ShowCourses("Get the Courses by Entity SQL", courses);
            Console.WriteLine();

            // Use the extension method to implement the Between operation
            courses = GetCoursesByExtension();
            ShowCourses("Get the Courses by extension method", courses);
            Console.WriteLine();

            Console.WriteLine("Press any key to exit.....");
            Console.ReadKey();
        }


        private static void ShowCourses(String showString, IEnumerable<Course> courses)
        {
            Console.WriteLine(showString);
            foreach (Course course in courses)
            {
                Console.WriteLine("CourseID:{0,-10} CourseTitle:{1,-15} Department:{2,-10}",
                    course.CourseID, course.Title, course.DepartmentID);
            }
        }

        /// <summary>
        /// Use the Entity SQL to implement the Between operation
        /// </summary>
        /// <returns></returns>
        private static List<Course> GetCoursesByEntitySQL()
        {
            using (School school = new School())
            {
                return school.Courses.Where(
                "it.DepartmentID between @lowerbound And @highbound",
                new ObjectParameter("lowerbound", 1),
                new ObjectParameter("highbound", 5)).ToList();
            }
        }

        /// <summary>
        /// Use the extension method to implement the Between operation
        /// </summary>
        /// <returns></returns>
        private static List<Course> GetCoursesByExtension()
        {
            using (School school = new School())
            {
                return school.Courses.Between(c => c.CourseID, "C1050", "C3141").ToList();
            }
        }
    }
}
