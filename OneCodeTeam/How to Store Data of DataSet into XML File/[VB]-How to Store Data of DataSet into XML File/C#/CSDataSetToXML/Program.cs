/****************************** Module Header ******************************\
Module Name:  Program.cs
Project:      CSDataSetToXML
Copyright (c) Microsoft Corporation.

In this sample, we will demonstrate how to write data into XML file from 
DataSet and read data into DataSet from XML.
1. We will create one dataset with two tables.
2. We will use two ways to export dataset into the XML files:WriteXml method 
and GetXml method.
3. We will use two ways to import dataset from the XML files:ReadXml method 
and InferXmlSchema method.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Data;
using System.IO;
using System.Xml;

namespace CSDataSetToXML
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Create the DataSet
            DataSet school = new DataSet("MySchool");
            DataTable course = CreateCourse();
            DataTable department = CreateDepartment();
            school.Tables.Add(course);
            school.Tables.Add(department);

            // Define the constraint between the tables.
            ForeignKeyConstraint courseDepartFK =
                new ForeignKeyConstraint("CourseDepartFK",
                    department.Columns["DepartmentID"],
                    course.Columns["DepartmentID"]);
            courseDepartFK.DeleteRule = Rule.Cascade;
            courseDepartFK.UpdateRule = Rule.Cascade;
            courseDepartFK.AcceptRejectRule = AcceptRejectRule.None;
            course.Constraints.Add(courseDepartFK);

            InsertDepartments(department);
            InsertCourses(course); 
            #endregion

            #region Export the dataset to the XML file.
            Console.WriteLine("Data of the whole DataSet {0}", school.DataSetName);
            DataTableHelper.ShowDataSet(school);

            String xmlWithSchemaFileName = "WriterXMLWithSchema.xml";
            String xmlGetDataFileName = "GetXML.xml";

            // Use two ways to export the dataset to the Xml file.
            DataTableHelper.WriteDataSetToXML(school, xmlWithSchemaFileName);
            DataTableHelper.GetXMLFromDataSet(school, xmlGetDataFileName); 
            #endregion

            #region Import the dataset from the XML file.
            // Use two ways to import the dataset from the Xml file.
            Console.WriteLine("Read Xml document into a new DataSet:");
            DataSet newSchool = new DataSet("NewSchool");
            DataTableHelper.ReadXmlIntoDataSet(newSchool, xmlWithSchemaFileName);
            DataTableHelper.ShowDataSetSchema(newSchool);
            Console.WriteLine();

            Console.WriteLine("Infer a schema for a DataSet from an XML document:");
            InferDataSetSchemaFromXml(); 
            #endregion

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        static DataTable CreateCourse()
        {
            DataTable course = new DataTable("Course");
            DataColumn[] cols ={
                              new DataColumn("CourseID",typeof(String)),
                              new DataColumn("Year",typeof(Int32)),
                              new DataColumn("Title",typeof(String)),
                              new DataColumn("Credits",typeof(Int32)),
                              new DataColumn("DepartmentID",typeof(Int32))};
            course.Columns.AddRange(cols);

            course.PrimaryKey = new DataColumn[] { course.Columns["CourseID"], course.Columns["Year"] };

            return course;
        }

        static DataTable CreateDepartment()
        {
            DataTable department = new DataTable("Department");
            DataColumn[] cols = { 
                                new DataColumn("DepartmentID", typeof(Int32)),
                                new DataColumn("Name",typeof(String)),
                                new DataColumn("Budget",typeof(Decimal)),
                                new DataColumn("StartDate",typeof(DateTime)),
                                new DataColumn("Administrator",typeof(Int32))};
            department.Columns.AddRange(cols);

            department.PrimaryKey = new DataColumn[] { department.Columns["DepartmentID"] };

            return department;
        }

        static void InsertDepartments(DataTable department)
        {
            Object[] rows = { 
                            new Object[]{1,"Engineering",350000.00,new DateTime(2007,9,1),2},
                            new Object[]{2,"English",120000.00,new DateTime(2007,9,1),6},
                            new Object[]{4,"Economics",200000.00,new DateTime(2007,9,1),4},
                            new Object[]{7,"Mathematics",250024.00,new DateTime(2007,9,1),3}};

            foreach (Object[] row in rows)
            {
                department.Rows.Add(row);
            }
        }

        static void InsertCourses(DataTable course)
        {
            Object[] rows ={
                               new Object[]{"C1045",2012,"Calculus",4,7},
                               new Object[]{"C1061",2012,"Physics",4,1},
                               new Object[]{"C2021",2012,"Composition",3,2},
                               new Object[]{"C2042",2012,"Literature",4,2}};

            foreach (Object[] row in rows)
            {
                course.Rows.Add(row);
            }
        }

        /// <summary>
        /// Display the results of inferring schema from four types of XML structures
        /// </summary>
        private static void InferDataSetSchemaFromXml()
        {
            String[] xmlFileNames = { 
                                    @"XMLFiles\ElementsWithOnlyAttributes.xml", 
                                    @"XMLFiles\ElementsWithAttributes.xml",
                                    @"XMLFiles\RepeatingElements.xml", 
                                    @"XMLFiles\ElementsWithChildElements.xml" };

            foreach (String xmlFileName in xmlFileNames)
            {
                Console.WriteLine("Result of {0}", Path.GetFileNameWithoutExtension(xmlFileName));
                DataSet newSchool = new DataSet();
                newSchool.InferXmlSchema(xmlFileName,null);
                DataTableHelper.ShowDataSetSchema(newSchool);
                Console.WriteLine();
           }
        }
    }
}
