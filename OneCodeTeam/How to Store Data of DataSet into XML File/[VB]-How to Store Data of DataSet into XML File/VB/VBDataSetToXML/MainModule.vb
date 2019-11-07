'**************************** Module Header ******************************\
' Module Name:  MainModule.vb
' Project:      VBDataSetToXML
' Copyright (c) Microsoft Corporation.
' 
' In this sample, we will demonstrate how to write data into XML file from 
' DataSet and read data into DataSet from XML.
' 1. We will create one dataset with two tables.
' 2. We will use two ways to export dataset into the XML files:WriteXml method 
' and GetXml method.
' 3. We will use two ways to import dataset from the XML files:ReadXml method 
' and InferXmlSchema method.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports System.IO
Imports System.Xml

Namespace VBDataSetToXML
    Friend Class MainModule
        Shared Sub Main(ByVal args() As String)
            ' Create the DataSet
            Dim school As New DataSet("MySchool")
            Dim course As DataTable = CreateCourse()
            Dim department As DataTable = CreateDepartment()
            school.Tables.Add(course)
            school.Tables.Add(department)

            ' Define the constraint between the tables.
            Dim courseDepartFK As New ForeignKeyConstraint("CourseDepartFK",
                                                           department.Columns("DepartmentID"),
                                                           course.Columns("DepartmentID"))
            courseDepartFK.DeleteRule = Rule.Cascade
            courseDepartFK.UpdateRule = Rule.Cascade
            courseDepartFK.AcceptRejectRule = AcceptRejectRule.None
            course.Constraints.Add(courseDepartFK)

            InsertDepartments(department)
            InsertCourses(course)

            ' Export the dataset to the XML file.
            Console.WriteLine("Data of the whole DataSet {0}", school.DataSetName)
            DataTableHelper.ShowDataSet(school)

            Dim xmlWithSchemaFileName As String = "WriterXMLWithSchema.xml"
            Dim xmlGetDataFileName As String = "GetXML.xml"

            ' Use two ways to export the dataset to the Xml file.
            DataTableHelper.WriteDataSetToXML(school, xmlWithSchemaFileName)
            DataTableHelper.GetXMLFromDataSet(school, xmlGetDataFileName)

            ' Import the dataset from the XML file.
            ' Use two ways to import the dataset from the Xml file.
            Console.WriteLine("Read Xml docuement into a new DataSet:")
            Dim newSchool As New DataSet("NewSchool")
            DataTableHelper.ReadXmlIntoDataSet(newSchool, xmlWithSchemaFileName)
            DataTableHelper.ShowDataSetSchema(newSchool)
            Console.WriteLine()

            Console.WriteLine("Infer a schema for a DataSet from an XML document:")
            InferDataSetSchemaFromXml()

            Console.WriteLine("Press any key to exit.")
            Console.ReadKey()
        End Sub

        Private Shared Function CreateCourse() As DataTable
            Dim course As New DataTable("Course")
            Dim cols() As DataColumn = {New DataColumn("CourseID", GetType(String)),
                                        New DataColumn("Year", GetType(Int32)),
                                        New DataColumn("Title", GetType(String)),
                                        New DataColumn("Credits", GetType(Int32)),
                                        New DataColumn("DepartmentID", GetType(Int32))}
            course.Columns.AddRange(cols)

            course.PrimaryKey = New DataColumn() {course.Columns("CourseID"), course.Columns("Year")}

            Return course
        End Function

        Private Shared Function CreateDepartment() As DataTable
            Dim department As New DataTable("Department")
            Dim cols() As DataColumn = {New DataColumn("DepartmentID", GetType(Int32)),
                                        New DataColumn("Name", GetType(String)),
                                        New DataColumn("Budget", GetType(Decimal)),
                                        New DataColumn("StartDate", GetType(Date)),
                                        New DataColumn("Administrator", GetType(Int32))}
            department.Columns.AddRange(cols)

            department.PrimaryKey = New DataColumn() {department.Columns("DepartmentID")}

            Return department
        End Function

        Private Shared Sub InsertDepartments(ByVal department As DataTable)
            Dim rows() As Object =
                {New Object() {1, "Engineering", 350000.0, New Date(2007, 9, 1), 2},
                 New Object() {2, "English", 120000.0, New Date(2007, 9, 1), 6},
                 New Object() {4, "Economics", 200000.0, New Date(2007, 9, 1), 4},
                 New Object() {7, "Mathematics", 250024.0, New Date(2007, 9, 1), 3}}

            For Each row As Object() In rows
                department.Rows.Add(row)
            Next row
        End Sub

        Private Shared Sub InsertCourses(ByVal course As DataTable)
            Dim rows() As Object =
                {New Object() {"C1045", 2012, "Calculus", 4, 7},
                 New Object() {"C1061", 2012, "Physics", 4, 1},
                 New Object() {"C2021", 2012, "Composition", 3, 2},
                 New Object() {"C2042", 2012, "Literature", 4, 2}}

            For Each row As Object() In rows
                course.Rows.Add(row)
            Next row
        End Sub

        ''' <summary>
        ''' Display the results of inferring schema from four types of XML structures
        ''' </summary>
        Private Shared Sub InferDataSetSchemaFromXml()
            Dim xmlFileNames() As String =
                {"XMLFiles\ElementsWithOnlyAttributes.xml",
                 "XMLFiles\ElementsWithAttributes.xml",
                 "XMLFiles\RepeatingElements.xml",
                 "XMLFiles\ElementsWithChildElements.xml"}

            For Each xmlFileName As String In xmlFileNames
                Console.WriteLine("Result of {0}", Path.GetFileNameWithoutExtension(xmlFileName))
                Dim newSchool As New DataSet()
                newSchool.InferXmlSchema(xmlFileName, Nothing)
                DataTableHelper.ShowDataSetSchema(newSchool)
                Console.WriteLine()
            Next xmlFileName
        End Sub
    End Class
End Namespace
