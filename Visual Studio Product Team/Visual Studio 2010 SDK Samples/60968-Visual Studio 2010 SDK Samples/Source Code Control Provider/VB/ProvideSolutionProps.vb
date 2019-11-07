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
Imports System.Collections.Generic
Imports System.Text
Imports System.Globalization
Imports Microsoft.VisualStudio.Shell

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider
	<AttributeUsage(AttributeTargets.Class, AllowMultiple := True, Inherited := True)> _
	Friend NotInheritable Class ProvideSolutionProps
		Inherits RegistrationAttribute
		Private _propName As String

		Public Sub New(ByVal propName As String)
			_propName = propName
		End Sub

		Public Overrides Sub Register(ByVal context As RegistrationContext)
			context.Log.WriteLine(String.Format(CultureInfo.InvariantCulture, "ProvideSolutionProps: ({0} = {1})", context.ComponentType.GUID.ToString("B"), PropName))

			Dim childKey As Key = Nothing

			Try
				childKey = context.CreateKey(String.Format(CultureInfo.InvariantCulture, "{0}\{1}", "SolutionPersistence", PropName))

				childKey.SetValue(String.Empty, context.ComponentType.GUID.ToString("B"))
			Finally
                If childKey IsNot Nothing Then
                    childKey.Close()
                End If
			End Try
		End Sub

		Public Overrides Sub Unregister(ByVal context As RegistrationContext)
			context.RemoveKey(String.Format(CultureInfo.InvariantCulture, "{0}\{1}", "SolutionPersistence", PropName))
		End Sub

		Public ReadOnly Property PropName() As String
			Get
				Return _propName
			End Get
		End Property
	End Class
End Namespace
