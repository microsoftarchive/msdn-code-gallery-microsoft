'****************************** Module Header ******************************\
' Module Name:  WCFService.vb
' Project:      JSONWCFService
' Copyright (c) Microsoft Corporation.
' 
' This is Json WCF Service. Windows Store App will call this service to get result. 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

' NOTE: You can use the "Rename" command on the context menu to change the class name "Service1" in code, svc and config file together.
' NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.vb at the Solution Explorer and start debugging.
Public Class WCFService
    Implements IWCFService

    Public Function GetDataUsingDataContract(Name As String, Age As String) As String Implements IWCFService.GetDataUsingDataContract
        Return "Your input is: " & "Name: " & Name & "  Age: " & Age
    End Function
End Class

