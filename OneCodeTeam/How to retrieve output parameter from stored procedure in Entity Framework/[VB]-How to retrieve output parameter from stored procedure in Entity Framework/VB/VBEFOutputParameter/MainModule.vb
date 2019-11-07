'****************************** Module Header ******************************\
' Module Name:    MainModule.vb
' Project:        VBEFOutputParameter
' Copyright (c) Microsoft Corporation
'
' This sample demonstrates how to use ObjectParameter instance to get the value 
' of output parameter.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************/

Imports System.Data.Objects

Module MainModule

    Sub Main()
        Dim name As String
        Dim description As String

        Do
            Console.WriteLine("Please input a name: ")
            name = Console.ReadLine()

            While String.IsNullOrEmpty(name)
                Console.WriteLine("Name can't be empty, please input again:")
                name = Console.ReadLine()
            End While

            Console.WriteLine()
            Console.WriteLine("Please input the description: ")
            description = Console.ReadLine()
            Console.WriteLine()

            Using context As New EFDemoDBEntities()
                ' Create an ObjectParameter instance to retrieve output parameter from stored procedure
                Dim Output As New ObjectParameter("ID", GetType(Int32))
                context.InsertPerson(name, description, Output)

                Console.Write("ID: {0}" & vbLf, Output.Value)
                Console.WriteLine("Press any key to continue, press 'Q' to exit." & vbLf)
            End Using
        Loop While Console.ReadKey(True).Key <> ConsoleKey.Q

    End Sub

End Module
