' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
'
Imports System.Collections.Generic
Imports System.IO
Imports System.Xml
Imports System.Linq
Imports System.Xml.Linq

Module Module1

    Sub Main()
        SetDataPath()
        ' List all books by Serge and Peter with co-authored books repeated
        Dim doc = XDocument.Load(SetDataPath() & "bib.xml")

        Dim b1 = From b In doc...<book> _
                 Aggregate author In b.<author>.<first> _
                 Into AnyAuthorNamedSerge = Any(author.Value = "Serge") _
                 Where AnyAuthorNamedSerge = True

        Dim b2 = From b In doc...<book> _
                 Aggregate author In b.<author>.<first> _
                 Into AnyAuthorNamedSerge = Any(author.Value = "Peter") _
                 Where AnyAuthorNamedSerge = True

        Dim books = b1.Concat(b2)

        For Each b In books
            Console.WriteLine(b)
        Next

        Console.ReadLine()

    End Sub

    Function SetDataPath() As String
        Dim datapath = Environment.CommandLine
        Do While datapath.StartsWith("""")
            datapath = datapath.Substring(1, datapath.Length - 2)
        Loop
        Do While datapath.EndsWith("""") OrElse datapath.EndsWith(" ")
            datapath = datapath.Substring(0, datapath.Length - 2)
        Loop
        datapath = Path.GetDirectoryName(datapath)

        Return Path.Combine(datapath, "data\\")
    End Function
End Module
