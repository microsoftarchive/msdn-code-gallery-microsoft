/****************************** Module Header ******************************\
Module Name:  DbUndoChanges.cs
Project:      CSEFUndoChanges
Copyright (c) Microsoft Corporation.

This sample demonstrates how to undo the changes in Entity Framework.
This file demonstrates how to undo the changes in different levels using DbContext

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Linq;
using DBMySchool;

namespace CSEFUndoChanges
{
    public static class DbUndoChanges
    {
        /// <summary>
        /// This method demonstrates how to undo the changes in Context level using DbContext.
        /// </summary>
        public static void UndoChangesInContext()
        {
            using (DbMySchool school = new DbMySchool())
            {
                DbCourse course = school.DbCourses.FirstOrDefault();
                if (course != null)
                {
                    Console.WriteLine("Before changes:");
                    course.ShowDbCourse();

                    Console.WriteLine("After changes:");
                    // Change the course and the related department.
                    course.Title += "-Modified";
                    course.Department.Name += "-Modified";
                    course.ShowDbCourse();

                    Console.WriteLine("After Undo DbContext:");
                    // Undo the whole Context.
                    school.UndoDbContext();
                    course.ShowDbCourse();
                }
            }
        }

        /// <summary>
        /// This method demonstrates how to undo the changes in Entities level using DbContext.
        /// </summary>
        public static void UndoChangesInEntities()
        {
            using (DbMySchool school = new DbMySchool())
            {
                DbDepartment department = school.DbDepartments.FirstOrDefault();
                IQueryable<DbCourse> courses = from c in school.DbCourses
                                               where c.DepartmentID == department.DepartmentID
                                               select c;

                Console.WriteLine("Before changes:");
                foreach (DbCourse course in courses)
                {
                    course.ShowDbCourse();
                }

                // Change the department and the related courses.
                Console.WriteLine("After changes:");
                department.Name += "-Modified";
                foreach (DbCourse course in courses)
                {
                    course.Title += "-Modified";
                    course.ShowDbCourse();
                }

                Console.WriteLine("After Undo Course Entities:");
                // Undo the DbCourse type Entities. We will see the changes of the department 
                // are not undone.
                school.UndoDbEntities<DbCourse>();
                foreach (DbCourse course in courses)
                {
                    course.ShowDbCourse();
                }
            }
        }

        /// <summary>
        /// This method demonstrates how to undo the changes in Entity level using DbContext.
        /// </summary>
        public static void UndoChangesInEntity()
        {
            using (DbMySchool school = new DbMySchool())
            {
                DbDepartment department = school.DbDepartments.FirstOrDefault();
                IQueryable<DbCourse> courses = from c in school.DbCourses
                                               where c.DepartmentID == department.DepartmentID
                                               select c;

                Console.WriteLine("Before changes:");
                foreach (DbCourse course in courses)
                {
                    course.ShowDbCourse();
                }

                // Change the courses.
                Console.WriteLine("After changes:");
                foreach (DbCourse course in courses)
                {
                    course.Title += "-Modified";
                    course.ShowDbCourse();
                }

                Console.WriteLine("After Undo the First Course Entity:");
                // Undo one course. You can see only the changes of the first course are undone.
                school.UndoDbEntity(courses.FirstOrDefault());
                foreach (DbCourse course in courses)
                {
                    course.ShowDbCourse();
                }
            }
        }


        /// <summary>
        /// This method demonstrates how to undo the changes in Property level using DbContext.
        /// </summary>
        public static void UndoChangesInProperty()
        {
            using (DbMySchool school = new DbMySchool())
            {
                DbCourse course = school.DbCourses.FirstOrDefault();
                DbDepartment department = school.DbDepartments.FirstOrDefault();
                if (course != null)
                {
                    Console.WriteLine("Before changes:");
                    course.ShowDbCourse();

                    Console.WriteLine("After changes:");
                    // Change the course Properties.
                    course.Title += "-Modified";
                    course.Department = department;
                    course.ShowDbCourse();

                    Console.WriteLine("After Undo Course Entity's Title Property:");
                    // Undo the change in the Entity Property level. UndoDbEntityProperty 
                    // method will undo the Title property of the course, but the change of the 
                    // Department Property will not be undone.
                    school.UndoDbEntityProperty(course,"Title");
                    course.ShowDbCourse();
                }
            }
        }

        /// <summary>
        /// This method shows the information of the course and the related department.
        /// </summary>
        /// <param name="course">show the information in the course</param>
        private static void ShowDbCourse(this DbCourse course)
        {
            if (course != null)
            {
                Console.WriteLine("CourseID:{0,-5} CourseName:{1,-15} DepartmentName:{2}", course.CourseID, course.Title, course.Department.Name);
            }
        }
            
    }
}
