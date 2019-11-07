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
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.Samples.VisualStudio.Services.Interfaces

Namespace Microsoft.Samples.VisualStudio.Services
	''' <summary>
	''' This is the class that implements the global service. All it needs to do is to implement 
	''' the interfaces exposed by this service (in this case IMyGlobalService).
	''' This class also needs to implement the SMyGlobalService interface in order to notify the 
	''' package that it is actually implementing this service.
	''' </summary>
	Public Class MyGlobalService
		Implements IMyGlobalService, SMyGlobalService
		' Store in this variable the service provider that will be used to query for other services.
		Private serviceProvider As IServiceProvider
		Public Sub New(ByVal sp As IServiceProvider)
			Trace.WriteLine("Constructing a new instance of MyGlobalService")
			serviceProvider = sp
		End Sub

		#Region "IMyGlobalService Members"
		''' <summary>
		''' Implementation of the function that does not access the local service.
		''' </summary>
		<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId := "Microsoft.Samples.VisualStudio.Services.HelperFunctions.WriteOnOutputWindow(System.IServiceProvider,System.String)")> _
		Public Sub GlobalServiceFunction() Implements IMyGlobalService.GlobalServiceFunction
            Dim outputText As String = " ======================================" & Microsoft.VisualBasic.Constants.vbLf & Microsoft.VisualBasic.Constants.vbTab & "GlobalServiceFunction called." & Microsoft.VisualBasic.Constants.vbLf & " ======================================" & Microsoft.VisualBasic.Constants.vbLf
			HelperFunctions.WriteOnOutputWindow(serviceProvider, outputText)
		End Sub

		''' <summary>
		''' Implementation of the function that will call a method of the local service.
		''' Notice that this class will access the local service using as service provider the one
		''' implemented by ServicesPackage.
		''' </summary>
		Public Function CallLocalService() As Integer Implements IMyGlobalService.CallLocalService
			' Query the service provider for the local service.
			' This object is supposed to be build by ServicesPackage and it pass its service provider
			' to the constructor, so the local service should be found.
			Dim localService As IMyLocalService = TryCast(serviceProvider.GetService(GetType(SMyLocalService)), IMyLocalService)
            If localService Is Nothing Then
                ' The local service was not found; write a message on the debug output and exit.
                Trace.WriteLine("Can not get the local service from the global one.")
                Return -1
            End If

			' Now call the method of the local service. This will write a message on the output window.
			Return localService.LocalServiceFunction()
		End Function
		#End Region
	End Class
End Namespace
