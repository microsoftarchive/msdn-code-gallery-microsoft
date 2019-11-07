/****************************** Module Header ******************************\
Module Name:  ObjectUndoChanges.cs
Project:      CSEFUndoChanges
Copyright (c) Microsoft Corporation.

This sample demonstrates how to undo the changes in Entity Framework.
This file demonstrates how to undo the changes in different levels using ObjectContext

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Linq;
using ObjMySchool;

namespace CSEFUndoChanges
{
    public static class ObjectUndoChanges
    {
        /// <summary>
        /// This method demonstrates how to undo the changes in Context level using ObjectContext.
        /// </summary>
        public static void UndoChangesInContext()
        {
            using (ObjectMySchool school = new ObjectMySchool())
            {
                ObjectCourse course = school.ObjectCourses.FirstOrDefault();
                if (course != null)
                {
                    Console.WriteLine("Before changes:");
                    course.ShowObjectCourse();

                    Console.WriteLine("After changes:");
                    // Change the course and the related department
                    course.Title += "-Modified";
                    course.Department.Name += "-Modified";
                    course.ShowObjectCourse();

                    Console.WriteLine("After Undo ObjectContext");
                    // Undo the whole Context
                    school.UndoObjectContext();
                    course.ShowObjectCourse();
                    
                }
            }
        }

        /// <summary>
        /// This method demonstrates how to undo the changes in Entities level using ObjectContext.
        /// </summary>
        public static void UndoChangesInEntities()
        {
            using (ObjectMySchool school = new ObjectMySchool())
            {
                ObjectDepartment department = school.ObjectDepartments.FirstOrDefault();
                IQueryable<ObjectCourse> courses = from c in school.ObjectCourses
                                               where c.DepartmentID == department.DepartmentID
                                               select c;

                Console.WriteLine("Before changes:");
                foreach (ObjectCourse course in courses)
                {
                    course.ShowObjectCourse();
                }

                // Change the department and the related course.
                Console.WriteLine("After changes:");
                department.Name += "-Modified";
                foreach (ObjectCourse course in courses)
                {
                    course.Title += "-Modified";
                    course.ShowObjectCourse();
                }

                Console.WriteLine("After Undo Course Entities:");
                // Undo the ObjectCourse type Entities. We will see the changes of the department 
                // are not undone.
                school.UndoObjectEntities<ObjectCourse>(school.ObjectCourses);
                foreach (ObjectCourse course in courses)
                {
                    course.ShowObjectCourse();
                }
            }
        }

        /// <summary>
        /// This method demonstrates how to undo the changes in Entity level using ObjectContext.
        /// </summary>
        public static void UndoChangesInEnity()
        {
            using (ObjectMySchool school = new ObjectMySchool())
            {
                ObjectDepartment department = school.ObjectDepartments.FirstOrDefault();
                IQueryable<ObjectCourse> courses = from c in school.ObjectCourses
                                                   where c.DepartmentID == department.DepartmentID
                                                   select c;

                Console.WriteLine("Before changes:");
                foreach (ObjectCourse course in courses)
                {
                    course.ShowObjectCourse();
                }

                // Change the course.
                Console.WriteLine("After changes:");
                foreach (ObjectCourse course in courses)
                {
                    course.Title += "-Modified";
                    course.ShowObjectCourse();
                }

                Console.WriteLine("After Undo the First Course Entity:");
                // Undo one course. You can see only the changes of the first course are undone.
                school.UndoObjectEntity(courses.FirstOrDefault());
                foreach (ObjectCourse course in courses)
                {
                    course.ShowObjectCourse();
                }
            }
        }

        /// <summary>
        /// This method demonstrates how to undo the changes in Property level using ObjectContext.
        /// </summary>
        public static void UndoChangesInProperty()
        {
            using (ObjectMySchool school = new ObjectMySchool())
            {
                ObjectCourse course = school.ObjectCourses.FirstOrDefault();
                ObjectDepartment department=school.ObjectDepartments.FirstOrDefault();
                if (course != null)
                {
                    Console.WriteLine("Before changes:");
                    course.ShowObjectCourse();

                    Console.WriteLine("After changes:");
                    // Change the course Properties.
                    course.Title += "-Modified";
                    course.Department = department;
                    course.ShowObjectCourse();

                    Console.WriteLine("After Undo Course Entity's Title Property:");
                    // Undo the change in the Entity Property level. UndoObjectEntityProperty 
                    // method will undo the Title property of the course, but the change of the 
                    // Department Property will not be undone.
                    school.UndoObjectEntityProperty(course, "Title");
                    course.ShowObjectCourse();
                }
            }
        }

        /// <summary>
        /// This method shows the information of the course and the related department.
        /// </summary>
        /// <param name="course">show the information in the course</param>
        private static void ShowObjectCourse(this ObjectCourse course)
        {
            if (course != null)
            {
                Console.WriteLine("CourseID:{0,-5} CourseName:{1,-15} DepartmentName:{2}", course.CourseID, course.Title, course.Department.Name);
            }
        }
    }
}
