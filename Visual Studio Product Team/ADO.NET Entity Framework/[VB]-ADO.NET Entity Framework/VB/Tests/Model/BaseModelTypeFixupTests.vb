' Copyright (c) Microsoft Corporation.  All rights reserved.


Imports Microsoft.VisualBasic
Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace Tests.Model

    ''' <summary>
    ''' Tests the fixup behavior of Pure POCO versions of objects in the model
    ''' </summary>
    <TestClass>
    Public Class BaseModelTypeFixupTests
        Inherits FixupTestsBase
        ''' <summary>
        ''' Returns an instance of T created from the default constructor
        ''' </summary>
        ''' <typeparam name="T">The type to be created</typeparam>
        ''' <returns>A new instance of type T</returns>
        Protected Overrides Function CreateObject(Of T As Class)() As T
            Return Activator.CreateInstance(Of T)()
        End Function
    End Class
End Namespace
