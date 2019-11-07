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
Imports System.Diagnostics
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.Samples.VisualStudio.Services.Interfaces

Namespace Microsoft.Samples.VisualStudio.Services
	''' <summary>
	''' This is the class that implements the local service. It implements IMyLocalService
	''' because this is the interface that we want to use, but it also implements the empty
	''' interface SMyLocalService in order to notify the service creator that it actually
	''' implements this service.
	''' </summary>
	Public Class MyLocalService
		Implements IMyLocalService, SMyLocalService
        ' Store a reference to the service provider that will be used to access the shell's services.
		Private provider As IServiceProvider
		''' <summary>
		''' Public constructor of this service. This will use a reference to a service provider to
		''' access the services provided by the shell.
		''' </summary>
		Public Sub New(ByVal sp As IServiceProvider)
			Trace.WriteLine("Constructing a new instance of MyLocalService")
			provider = sp
		End Sub
		#Region "IMyLocalService Members"
		<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId := "Microsoft.Samples.VisualStudio.Services.HelperFunctions.WriteOnOutputWindow(System.IServiceProvider,System.String)")> _
		Public Function LocalServiceFunction() As Integer Implements IMyLocalService.LocalServiceFunction
            Dim outputText As String = " ======================================" & Microsoft.VisualBasic.Constants.vbLf & Microsoft.VisualBasic.Constants.vbTab & "LocalServiceFunction called." & Microsoft.VisualBasic.Constants.vbLf & " ======================================" & Microsoft.VisualBasic.Constants.vbLf
			HelperFunctions.WriteOnOutputWindow(provider, outputText)
			Return 0
		End Function
		#End Region
	End Class
End Namespace
