/****************************** Module Header ******************************\
Module Name:  Program.cs
Project:      CSEFStoreXmlFiles
Copyright (c) Microsoft Corporation.

This sample demonstrates how to import/export the XML into/from database using 
Code First in EF.
We implement two ways in the sample:
1. Using LinqToXml to import/export the XML files;
2. Using the XmlColumn to store the Xml files.

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
using System.Text;
using System.Data.Entity;
using System.Xml.Linq;

namespace CSEFStoreXmlFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<MySchoolContext>());

            XDocument document = XDocument.Load("Courses-2012.xml");

            ImportCourses(document);
            Console.WriteLine("Imported the Courses-2012.xml file by LinqToXml");
            Console.WriteLine();

            ExportCourses();
            Console.WriteLine("Exported the CoursesByLinqToXml.xml by LinqToXml");
            Console.WriteLine();

            ImportYearCourses(document);
            Console.WriteLine("Imported the Courses-2012.xml file into the XmlColumn");
            Console.WriteLine();

            ExportYearCourses();
            Console.WriteLine("Exported the CoursesFromXmlColumn.xml from XmlColumn");
            Console.WriteLine();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Use LinqToXml to import information in the xml file into the database
        /// </summary>
        /// <param name="document">the Xml file that we import into the database</param>
        static void ImportCourses(XDocument document)
        {
            using (MySchoolContext school = new MySchoolContext())
            {
                // Get the Course information from the Xml document
                IEnumerable<Course> courses = 
                    from c in document.Descendants("Course")
                    select new Course
                    {
                        CourseID = c.Element("CourseId") == null ? 
                        Guid.NewGuid().ToString() : c.Element("CourseId").Value,
                        Title = c.Element("Title") == null ? null : c.Element("Title").Value,
                        Credits = c.Element("Credits") == null ? -1 : Int32.Parse(c.Element("Credits").Value) ,
                        Department = c.Element("Department") == null ? null : c.Element("Department").Value
                    };

                foreach (Course course in courses)
                {
                    school.Courses.Add(course);
                }

                school.SaveChanges();
            }
        }

        /// <summary>
        /// Use LinqToXml to export the information into a Xml file from the database 
        /// </summary>
        static void ExportCourses()
        {
            using (MySchoolContext school = new MySchoolContext())
            {
                XNamespace ns = "http://CSEFStoreXmlFiles";
                XElement coursesXml = new XElement(ns + "Courses",
                    from c in school.Courses.Take(5).AsEnumerable()
                    select new XElement(ns + "Course",
                        c.CourseID == null ? null : new XElement(ns + "CourseID", c.CourseID),
                        c.Title == null ? null : new XElement(ns + "Title", c.Title),
                        c.Credits == null ? null : new XElement(ns + "Credits", c.Credits),
                        c.Department == null ? null : new XElement(ns + "Department", c.Department)));

                coursesXml.Save("CoursesByLinqToXml.xml");
            }
        }

        /// <summary>
        /// Use XmlColumn to store the Xml file in the database
        /// </summary>
        /// <param name="document">the Xml file we need to store in the datase</param>
        static void ImportYearCourses(XDocument document)
        {
            using (MySchoolContext school = new MySchoolContext())
            {
                // Set the value of Courses property with the Xml document to store the Xml file.
                YearCourse yearCourse = new YearCourse { Year=2012,Courses=document};

                school.YearCourses.Add(yearCourse);

                school.SaveChanges();
            }
        }

        /// <summary>
        /// Export the Xml file from the XmlColumn of the database
        /// </summary>
        static void ExportYearCourses()
        {
            using (MySchoolContext school = new MySchoolContext())
            {
                IQueryable<YearCourse> coursesList = from yc in school.YearCourses
                                                select yc;

                foreach (YearCourse courses in coursesList)
                {
                    // Set the Xml file name
                    String fileName = new StringBuilder().AppendFormat
                        ("CoursesFromXmlColumn-{0}.xml", courses.Year).ToString();
                    courses.Courses.Save(fileName);

                    // After get the Xml document from the XmlColumn, we can use the LinqToXml 
                    // to get the information of the Course.
                    IEnumerable<Course> courseList = 
                        from c in courses.Courses.Descendants("Course")
                        select new Course
                        {
                            CourseID = c.Element("CourseId") == null ? 
                            Guid.NewGuid().ToString() : c.Element("CourseId").Value,
                            Title = c.Element("Title") == null ? null : c.Element("Title").Value,
                            Credits = c.Element("Credits") == null ? -1 : Int32.Parse(c.Element("Credits").Value),
                            Department = c.Element("Department") == null ? null : c.Element("Department").Value
                        };

                    Console.WriteLine("{0}'s Courses:",courses.Year);
                    foreach (Course course in courseList)
                    {
                        Console.WriteLine("CourseID:{0,-10} Title:{1,-15} Credits:{2,-5} Department:{3}",
                            course.CourseID,course.Title,course.Credits,course.Department);
                    }
                }
            }
        }
    }
}
