'**************************** Module Header ******************************\
' Module Name:  SqlHelper.vb
' Project:      VBDataSqlCommand
' Copyright (c) Microsoft Corporation.
' 
' We can create and execute different types of SqlCommand. In this application, 
' we will demonstrate how to create and execute SqlCommand.
' The file contains some methods that set the connection, command and exectute 
' the command.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports System.Data.SqlClient
Imports System.Threading.Tasks

Namespace VBDataSqlCommand
    Friend NotInheritable Class SqlHelper
        ''' <summary>
        ''' Set the connection, command, and then execute the command with non query.
        ''' </summary>
        Private Sub New()
        End Sub
        Public Shared Function ExecuteNonQuery(ByVal connectionString As String, ByVal commandText As String,
                                               ByVal commandType As CommandType, ByVal ParamArray parameters() As SqlParameter) As Int32
            Using conn As New SqlConnection(connectionString)
                Using cmd As New SqlCommand(commandText, conn)
                    ' There're three command types: StoredProcedure, Text, TableDirect. The TableDirect 
                    ' type is only for OLE DB.  
                    cmd.CommandType = commandType
                    cmd.Parameters.AddRange(parameters)

                    conn.Open()
                    Return cmd.ExecuteNonQuery()
                End Using
            End Using
        End Function

        ''' <summary>
        ''' Set the connection, command, and then execute the command and only return one value.
        ''' </summary>
        Public Shared Function ExecuteScalar(ByVal connectionString As String, ByVal commandText As String,
                                             ByVal commandType As CommandType, ByVal ParamArray parameters() As SqlParameter) As Object
            Using conn As New SqlConnection(connectionString)
                Using cmd As New SqlCommand(commandText, conn)
                    cmd.CommandType = commandType
                    cmd.Parameters.AddRange(parameters)

                    conn.Open()
                    Return cmd.ExecuteScalar()
                End Using
            End Using
        End Function

        ''' <summary>
        ''' Set the connection, command, and then execute the command with query and return the reader.
        ''' </summary>
        Public Shared Function ExecuteReader(ByVal connectionString As String, ByVal commandText As String,
                                             ByVal commandType As CommandType, ByVal ParamArray parameters() As SqlParameter) As SqlDataReader
            Dim conn As New SqlConnection(connectionString)

            Using cmd As New SqlCommand(commandText, conn)
                cmd.CommandType = commandType
                cmd.Parameters.AddRange(parameters)

                conn.Open()
                ' When using CommandBehavior.CloseConnection, the connection will be closed when the 
                ' IDataReader is closed.
                Dim reader As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)

                Return reader
            End Using
        End Function
    End Class
End Namespace
