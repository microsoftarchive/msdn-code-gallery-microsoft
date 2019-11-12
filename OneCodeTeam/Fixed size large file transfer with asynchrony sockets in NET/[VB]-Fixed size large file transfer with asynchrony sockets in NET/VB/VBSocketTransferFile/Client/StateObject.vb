'******************************** Module Header **********************************\
' Module Name:	StateObject.vb
' Project:		Client
' Copyright (c) Microsoft Corporation.
' 
' This sample demonstrates how to implement fixed size large file transfer with asynchrony sockets in NET.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*********************************************************************************/


Imports System.Collections.Generic
Imports System.Linq
Imports System.Net.Sockets
Imports System.Text

Namespace Client
    Class StateObject
        Public workSocket As Socket = Nothing

        Public Const BufferSize As Integer = 5242880

        Public buffer As Byte() = New Byte(BufferSize - 1) {}

        Public connected As Boolean = False
    End Class
End Namespace

