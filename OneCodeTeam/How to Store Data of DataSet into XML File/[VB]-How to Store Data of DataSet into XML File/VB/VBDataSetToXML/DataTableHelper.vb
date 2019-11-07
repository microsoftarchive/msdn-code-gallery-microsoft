'**************************** Module Header ******************************\
' Module Name:  DataTableHelper.vb
' Project:      VBDataSetToXML
' Copyright (c) Microsoft Corporation.
' 
' In this sample, we will demonstrate how to write data into XML file from 
' DataSet and read data into DataSet from XML.
' This file includes some helper methods.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************?

Imports System.IO
Imports System.Text
Imports System.Xml

Namespace VBDataSetToXML
    Friend NotInheritable Class DataTableHelper
        ''' <summary>
        ''' Use WriteXml method to export the dataset.
        ''' </summary>
        Private Sub New()
        End Sub
        Public Shared Sub WriteDataSetToXML(ByVal dataset As DataSet, ByVal xmlFileName As String)
            Using fsWriterStream As New FileStream(xmlFileName, FileMode.Create)
                Using xmlWriter As New XmlTextWriter(fsWriterStream, Encoding.Unicode)
                    dataset.WriteXml(xmlWriter, XmlWriteMode.WriteSchema)
                    Console.WriteLine("Write {0} to the File {1}.", dataset.DataSetName, xmlFileName)
                    Console.WriteLine()
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' Use GetXml method to get the XML data of the dataset and then export to the file.
        ''' </summary>
        Public Shared Sub GetXMLFromDataSet(ByVal dataset As DataSet, ByVal xmlFileName As String)
            Using writer As New StreamWriter(xmlFileName)
                writer.WriteLine(dataset.GetXml())
                Console.WriteLine("Get Xml data from {0} and write to the File {1}.",
                                  dataset.DataSetName, xmlFileName)
                Console.WriteLine()
            End Using
        End Sub

        ''' <summary>
        ''' Use ReadXml method to import the dataset from the dataset.
        ''' </summary>
        Public Shared Sub ReadXmlIntoDataSet(ByVal newDataSet As DataSet, ByVal xmlFileName As String)
            Using fsReaderStream As New FileStream(xmlFileName, FileMode.Open)
                Using xmlReader As New XmlTextReader(fsReaderStream)
                    newDataSet.ReadXml(xmlReader, XmlReadMode.ReadSchema)
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' Display the columns and value of DataSet.
        ''' </summary>
        Public Shared Sub ShowDataSet(ByVal dataset As DataSet)
            For Each table As DataTable In dataset.Tables
                Console.WriteLine("Table {0}:", table.TableName)
                ShowDataTable(table)
            Next table
        End Sub

        ''' <summary>
        ''' Display the columns and value of DataTable.
        ''' </summary>
        Private Shared Sub ShowDataTable(ByVal table As DataTable)
            For Each col As DataColumn In table.Columns
                Console.Write("{0,-14}", col.ColumnName)
            Next col
            Console.WriteLine("{0,-14}", "")

            For Each row As DataRow In table.Rows
                If row.RowState = DataRowState.Deleted Then
                    For Each col As DataColumn In table.Columns
                        If col.DataType.Equals(GetType(Date)) Then
                            Console.Write("{0,-14:d}", row(col, DataRowVersion.Original))
                        ElseIf col.DataType.Equals(GetType(Decimal)) Then
                            Console.Write("{0,-14:C}", row(col, DataRowVersion.Original))
                        Else
                            Console.Write("{0,-14}", row(col, DataRowVersion.Original))
                        End If
                    Next col
                Else
                    For Each col As DataColumn In table.Columns
                        If col.DataType.Equals(GetType(Date)) Then
                            Console.Write("{0,-14:d}", row(col))
                        ElseIf col.DataType.Equals(GetType(Decimal)) Then
                            Console.Write("{0,-14:C}", row(col))
                        Else
                            Console.Write("{0,-14}", row(col))
                        End If
                    Next col
                End If
                Console.WriteLine("{0,-14}", "")
            Next row
        End Sub

        ''' <summary>
        ''' Display the columns of DataSet.
        ''' </summary>
        Public Shared Sub ShowDataSetSchema(ByVal dataSet As DataSet)
            Console.WriteLine("{0} contains the following tables:", dataSet.DataSetName)
            For Each table As DataTable In dataSet.Tables
                Console.WriteLine("   Table {0} contains the following columns:", table.TableName)
                ShowDataTableSchema(table)
            Next table
        End Sub

        ''' <summary>
        ''' Display the columns of DataTable
        ''' </summary>
        Private Shared Sub ShowDataTableSchema(ByVal table As DataTable)
            Dim columnString As String = ""
            For Each col As DataColumn In table.Columns
                columnString &= col.ColumnName & "   "
            Next col
            Console.WriteLine(columnString)
        End Sub
    End Class
End Namespace
