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
Imports Microsoft.VsSDK.UnitTestLibrary
Imports System.ComponentModel
Imports Microsoft.VisualStudio

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider.UnitTests
	Friend Class SiteWrappedServiceProvider
		Implements ISite
		Private _sp As OleServiceProvider

		Public Sub New(ByVal sp As OleServiceProvider)
			_sp = sp
		End Sub

        ' Support the ISite interface.
		Public Overridable ReadOnly Property Component() As IComponent Implements ISite.Component
			Get
				Return Nothing
			End Get
		End Property
		Public Overridable ReadOnly Property Container() As IContainer Implements ISite.Container
			Get
				Return Nothing
			End Get
		End Property
		Public Overridable ReadOnly Property DesignMode() As Boolean Implements ISite.DesignMode
			Get
				Return False
			End Get
		End Property
		Public Overridable Property Name() As String Implements ISite.Name
			Get
				Return "SiteWrappedServiceProvider"
			End Get
			Set(ByVal value As String)

			End Set
		End Property

        ' Support the IServiceProvider interface.
		Public Overridable Function GetService(ByVal serviceType As Type) As Object Implements System.IServiceProvider.GetService
            ' Query IUnknown from the service provider.
			Dim ppvObject As IntPtr = CType(0, IntPtr)
			Dim guidService As Guid = serviceType.GUID
			Dim guidIntf As Guid = VSConstants.IID_IUnknown
			Dim iResult As Integer = _sp.QueryService(guidService, guidIntf, ppvObject)
			If iResult <> VSConstants.S_OK Then
				Return Nothing
			End If

            ' Return the object to the caller.
			Return System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(ppvObject)
		End Function
	End Class
End Namespace
