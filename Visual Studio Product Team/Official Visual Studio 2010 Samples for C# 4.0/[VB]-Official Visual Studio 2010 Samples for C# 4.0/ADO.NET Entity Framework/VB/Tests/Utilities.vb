' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)



Imports Microsoft.VisualBasic
Imports System
Imports System.Globalization
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace Tests

    ''' <summary>
    ''' Static helper methods for unit testing
    ''' </summary>
    Public NotInheritable Class Utilities
        ''' <summary>
        ''' Verifies that the supplied Action throws an ArgumentNullException
        ''' Performs an Assert.Fail is the exception is not thrown
        ''' </summary>
        ''' <param name="call">The action that should throw</param>
        ''' <param name="parameter">The parameter that should be identified in the exception</param>
        ''' <param name="method">Method name for logging purposes</param>
        Private Sub New()
        End Sub
        Public Shared Sub CheckNullArgumentException(ByVal [call] As Action, ByVal parameter As String, ByVal method As String)
            If [call] Is Nothing Then
                Throw New ArgumentNullException("call")
            End If

            If parameter Is Nothing Then
                Throw New ArgumentNullException("parameter")
            End If

            If method Is Nothing Then
                Throw New ArgumentNullException("method")
            End If

            Try
                [call]()
                Assert.Fail(String.Format(CultureInfo.InvariantCulture, "Supplying null '{0}' argument to '{1}' did not throw.", parameter, method))
            Catch ex As ArgumentNullException
                Assert.AreEqual(parameter, ex.ParamName, String.Format(CultureInfo.InvariantCulture, "'ArgumentNullException.ParamName' wrong when supplying null '{0}' argument to '{1}'.", parameter, method))
            End Try
        End Sub
    End Class
End Namespace
