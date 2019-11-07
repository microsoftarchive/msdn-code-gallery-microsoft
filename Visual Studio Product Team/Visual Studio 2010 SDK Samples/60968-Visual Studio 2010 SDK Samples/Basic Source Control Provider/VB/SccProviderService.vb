'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************

' SccProviderService.vb : Implementation of Sample Source Control Provider Service
'


Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell.Interop

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.BasicSccProvider
	<Guid(GuidStrings.GuidSccProviderService)> _
	Public Class SccProviderService
		Implements IVsSccProvider
		Private _active As Boolean = False
		Private _sccProvider As BasicSccProvider = Nothing

		Public Sub New(ByVal sccProvider As BasicSccProvider)
			_sccProvider = sccProvider
		End Sub

		''' <summary>
		''' Returns whether this source control provider is the active scc provider.
		''' </summary>
		Public ReadOnly Property Active() As Boolean
			Get
				Return _active
			End Get
		End Property

		'--------------------------------------------------------------------------------
		' IVsSccProvider specific interfaces
		'--------------------------------------------------------------------------------

		' Called by the scc manager when the provider is activated. 
        ' Make visible and enable if necessary scc related menu commands.
		Public Function SetActive() As Integer Implements IVsSccProvider.SetActive
			Trace.WriteLine(String.Format(CultureInfo.CurrentUICulture, "Provider set active"))

			_active = True
			_sccProvider.OnActiveStateChange()

			Return VSConstants.S_OK
		End Function

		' Called by the scc manager when the provider is deactivated. 
        ' Hides and disable scc related menu commands.
		Public Function SetInactive() As Integer Implements IVsSccProvider.SetInactive
			Trace.WriteLine(String.Format(CultureInfo.CurrentUICulture, "Provider set inactive"))

			_active = False
			_sccProvider.OnActiveStateChange()

			Return VSConstants.S_OK
		End Function

		Public Function AnyItemsUnderSourceControl(<System.Runtime.InteropServices.Out()> ByRef pfResult As Integer) As Integer Implements IVsSccProvider.AnyItemsUnderSourceControl
			pfResult = 0
			Return VSConstants.S_OK
		End Function
	End Class
End Namespace