'***************************** Module Header ******************************\
' Module Name:	IRESTAPI.svc.vb
' Project:		WCFRESTService
' Copyright (c) Microsoft Corporation.
' 
' This sample will show you how to create HTTPWebReqeust, and get HTTPWebResponse.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

' NOTE: You can use the "Rename" command on the context menu to change the class name "RESTAPI" in code, svc and config file together.
' NOTE: In order to launch WCF Test Client for testing this service, please select RESTAPI.svc or RESTAPI.svc.vb at the Solution Explorer and start debugging.
Public Class RESTAPI
    Implements IRESTAPI

    Public Function GetUserNameByID(id As String) As String Implements IRESTAPI.GetUserNameByID
        Try
            Dim userID As Integer = Integer.Parse(id)
            If userID > 0 AndAlso userID < 5 Then
                Dim userDataList = (New UserDataList()).getUserDataList()
                Dim query = From data In userDataList Where data.ID = id Select data.UserName
                Return query.FirstOrDefault()
            Else
                Return "Can't find the user with id: " & id
            End If
        Catch e As Exception
            Throw New Exception(String.Format("Can't convert {0} to int", id))
        End Try
    End Function

    Public Function GetUserNameByPostID(rData As UserData) As UserData Implements IRESTAPI.GetUserNameByPostID
        Try
            Dim userID As Integer = Integer.Parse(rData.ID)
            If userID > 0 AndAlso userID < 5 Then
                Dim userDataList = (New UserDataList()).getUserDataList()
                Dim query = From data In userDataList Where data.ID = rData.ID Select data
                Return New UserData() With { _
                     .UserName = query.FirstOrDefault().UserName _
                }
            Else
                Return Nothing
            End If
        Catch
            Throw New Exception(String.Format("Can't convert {0} to int", rData.ID))
        End Try


    End Function
End Class
