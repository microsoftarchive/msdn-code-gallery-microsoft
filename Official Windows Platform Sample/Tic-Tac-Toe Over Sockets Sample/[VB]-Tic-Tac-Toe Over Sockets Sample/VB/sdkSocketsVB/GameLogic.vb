'
'   Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
'   Use of this sample source code is subject to the terms of the Microsoft license 
'   agreement under which you licensed this sample source code and is provided AS-IS.
'   If you did not accept the terms of the license agreement, you are not authorized 
'   to use this sample source code.  For the terms of the license, please see the 
'   license agreement between you and Microsoft.
'  
'   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
'  
'
Public NotInheritable Class GameLogic
    Private Shared _rnd As Random
    Private Sub New()
    End Sub
    Public Shared Function Play(ByVal incoming As String) As String
        If Not String.IsNullOrWhiteSpace(incoming) Then
            incoming = incoming.Trim(vbNullChar.ToCharArray())
            Dim pairs() As String = incoming.Split("|"c)

            Dim playAs As String = pairs(0)
            Dim list As New List(Of String)()
            For i = 1 To pairs.Length - 1
                Dim pair() As String = pairs(i).Split("*"c)

                If String.IsNullOrWhiteSpace(pair(1)) Then
                    list.Add(pair(0))
                End If

            Next i

            If _rnd Is Nothing Then
                _rnd = New Random(Date.Now.Millisecond)
            End If


            Return If(list.Count > 0, list(_rnd.Next(0, list.Count)), String.Empty)
        End If

        Return String.Empty


    End Function
End Class
