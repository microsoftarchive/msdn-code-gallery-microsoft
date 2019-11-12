' Copyright (c) Microsoft Corporation. All rights reserved.
Option Strict On

Public Class ImageAttributes
    Public Src As String
    Public Alt As String
    Public Height As String
    Public Width As String

    Sub New(ByVal strSrc As String, ByVal strAlt As String, ByVal strHeight As String, ByVal strWidth As String)
        Src = strSrc
        Width = strWidth
        Height = strHeight
        Alt = strAlt
    End Sub
End Class