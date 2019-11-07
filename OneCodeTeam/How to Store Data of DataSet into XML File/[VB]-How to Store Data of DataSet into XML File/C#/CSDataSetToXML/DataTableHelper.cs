/****************************** Module Header ******************************\
Module Name:  DataTableHelper.cs
Project:      CSDataSetToXML
Copyright (c) Microsoft Corporation.

In this sample, we will demonstrate how to write data into XML file from 
DataSet and read data into DataSet from XML.
This file includes some helper methods.

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
using System.Text;
using System.Xml;

namespace CSDataSetToXML
{
    static class DataTableHelper
    {
        /// <summary>
        /// Use WriteXml method to export the dataset.
        /// </summary>
        public static void WriteDataSetToXML(DataSet dataset, String xmlFileName)
        {
            using (FileStream fsWriterStream = new FileStream(xmlFileName, FileMode.Create))
            {
                using (XmlTextWriter xmlWriter = new XmlTextWriter(fsWriterStream, Encoding.Unicode))
                {
                    dataset.WriteXml(xmlWriter, XmlWriteMode.WriteSchema);
                    Console.WriteLine("Write {0} to the File {1}.", dataset.DataSetName, xmlFileName);
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        /// Use GetXml method to get the XML data of the dataset and then export to the file.
        /// </summary>
        public static void GetXMLFromDataSet(DataSet dataset, String xmlFileName)
        {
            using (StreamWriter writer = new StreamWriter(xmlFileName))
            {
                writer.WriteLine(dataset.GetXml());
                Console.WriteLine("Get Xml data from {0} and write to the File {1}.", dataset.DataSetName, xmlFileName);
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Use ReadXml method to import the dataset from the dataset.
        /// </summary>
        public static void ReadXmlIntoDataSet(DataSet newDataSet, String xmlFileName)
        {
            using (FileStream fsReaderStream = new FileStream(xmlFileName, FileMode.Open))
            {
                using (XmlTextReader xmlReader = new XmlTextReader(fsReaderStream))
                {
                    newDataSet.ReadXml(xmlReader, XmlReadMode.ReadSchema);
                }
            }
        }

        /// <summary>
        /// Display the columns and value of DataSet.
        /// </summary>
        public static void ShowDataSet(DataSet dataset)
        {
            foreach (DataTable table in dataset.Tables)
            {
                Console.WriteLine("Table {0}:", table.TableName);
                ShowDataTable(table);
            }
        }

        /// <summary>
        /// Display the columns and value of DataTable.
        /// </summary>
        private static void ShowDataTable(DataTable table)
        {
            foreach (DataColumn col in table.Columns)
            {
                Console.Write("{0,-14}", col.ColumnName);
            }
            Console.WriteLine("{0,-14}", "");

            foreach (DataRow row in table.Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                {
                    foreach (DataColumn col in table.Columns)
                    {
                        if (col.DataType.Equals(typeof(DateTime)))
                        {
                            Console.Write("{0,-14:d}", row[col, DataRowVersion.Original]);
                        }
                        else if (col.DataType.Equals(typeof(Decimal)))
                        {
                            Console.Write("{0,-14:C}", row[col, DataRowVersion.Original]);
                        }
                        else
                        {
                            Console.Write("{0,-14}", row[col, DataRowVersion.Original]);
                        }
                    }
                }
                else
                {
                    foreach (DataColumn col in table.Columns)
                    {
                        if (col.DataType.Equals(typeof(DateTime)))
                        {
                            Console.Write("{0,-14:d}", row[col]);
                        }
                        else if (col.DataType.Equals(typeof(Decimal)))
                        {
                            Console.Write("{0,-14:C}", row[col]);
                        }
                        else
                        {
                            Console.Write("{0,-14}", row[col]);
                        }
                    }
                }
                Console.WriteLine("{0,-14}", "");
            }
        }

        /// <summary>
        /// Display the columns of DataSet.
        /// </summary>
        public static void ShowDataSetSchema(DataSet dataSet)
        {
            Console.WriteLine("{0} contains the following tables:", dataSet.DataSetName);
            foreach (DataTable table in dataSet.Tables)
            {
                Console.WriteLine("   Table {0} contains the following columns:", table.TableName);
                ShowDataTableSchema(table);
            }
        }

        /// <summary>
        /// Display the columns of DataTable
        /// </summary>
        private static void ShowDataTableSchema(DataTable table)
        {
            String columnString = "";
            foreach (DataColumn col in table.Columns)
            {
                columnString += col.ColumnName + "   ";
            }
            Console.WriteLine(columnString);
        }
    }
}
