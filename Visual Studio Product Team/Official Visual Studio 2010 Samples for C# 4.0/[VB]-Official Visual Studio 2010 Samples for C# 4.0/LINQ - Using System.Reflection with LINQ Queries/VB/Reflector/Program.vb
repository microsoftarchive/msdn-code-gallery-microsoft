' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
'
'Copyright (C) Microsoft Corporation.  All rights reserved.

Imports System.IO
Imports System.Xml
Imports System.Reflection

' The sample illustrates queries over components from System.Reflection and
' System.Xml.XLinq. The by-product is an HTML document outlining the 
' public APIs for a given assembly, in this case System.Xml.Linq.dll.


Friend Module Program

    Const HtmlFile As String = "System.Xml.Linq.html"

    Public Sub Main()

        ' Get Path and Name of assembly to reflect
        Dim assembly As Assembly = assembly.GetAssembly(GetType(XDocument))
        Dim AssemblyFile As String = assembly.CodeBase

        ' reflect over the assembly
        Dim reflector As New Reflector()
        reflector.Reflect(AssemblyFile)

        ' generate the HTML document
        Dim settings As New XmlWriterSettings()
        settings.OmitXmlDeclaration = True
        settings.Indent = True
        Dim writer As XmlWriter = XmlWriter.Create(HtmlFile, settings)
        reflector.Transform(writer)
        writer.Close()

        ' display the HTML document
        Dim fileInfo As New FileInfo(HtmlFile)
        If fileInfo.Exists Then Process.Start("iexplore.exe", fileInfo.FullName)
    End Sub
End Module