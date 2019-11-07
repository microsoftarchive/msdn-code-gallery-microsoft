'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************


Imports Microsoft.VisualBasic
Imports System
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace ComboBox.UnitTest
	Friend Class Utilities
		Public Delegate Sub ThrowingFunction()

		Private Sub New()
		End Sub
		Public Shared Function HasFunctionThrown(Of ExceptionType As Exception)(ByVal [function] As ThrowingFunction) As Boolean
			Dim hasThrown As Boolean = False
			Try
				[function]()
			Catch e1 As ExceptionType
				hasThrown = True
			Catch ti As System.Reflection.TargetInvocationException
				hasThrown = (Not Nothing Is TryCast(ti.InnerException, ExceptionType))
				If (Not hasThrown) Then
					Throw
				End If
			End Try

			Return hasThrown
		End Function

		Public Shared Sub SameArray(Of Element_T)(ByVal expected As Element_T(), ByVal actual As Element_T())
			' If one array is null, then also the other must be null.
			If (Nothing Is expected) OrElse (Nothing Is actual) Then
				Assert.IsNull(expected)
				Assert.IsNull(actual)
				Return
			End If

			' The arrays are equal only if they contain the same number of elements.
			Assert.AreEqual(Of Integer)(expected.Length, actual.Length)

            ' Now check that all the elements are the same.
            For i As Integer = 0 To expected.Length - 1
                Assert.AreEqual(Of Element_T)(expected(i), actual(i))
            Next i

        End Sub
	End Class
End Namespace
