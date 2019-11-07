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
Imports System.Text
Imports System.Collections.Generic
Imports System.Reflection
Imports System.ComponentModel
Imports System.Windows.Forms
Imports Microsoft.VsSDK.UnitTestLibrary
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
	''' <summary>
    ''' This is a test class for Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderOptions and is intended
    ''' to contain all Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProviderOptions Unit Tests.
    ''' </summary>
	<TestClass()> _
	Public Class SccProviderOptionsTest
		''' <summary>
        ''' A test for OnActivate (CancelEventArgs).
        ''' </summary>
		<TestMethod()> _
		Public Sub OnActivateTest()
            Dim target As New SccProviderOptions()

			Dim method As MethodInfo = GetType(SccProviderOptions).GetMethod("OnActivate", BindingFlags.NonPublic Or BindingFlags.Instance)
            Dim e As New CancelEventArgs()
			method.Invoke(target, New Object() { e })
		End Sub

		''' <summary>
        ''' A test for OnApply (PageApplyEventArgs).
        ''' </summary>
		<TestMethod()> _
		Public Sub OnApplyTest()
            Dim target As New SccProviderOptions()

            ' Create a basic service provider.
			Using serviceProvider As OleServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices()
                ' Mock the UIShell service to answer Cancel to the dialog invocation.
				Dim mockUIShell As BaseMock = MockUiShellProvider.GetShowMessageBoxCancel()
				serviceProvider.AddService(GetType(IVsUIShell), mockUIShell, True)

                ' Create an ISite wrapper over the service provider.
                Dim wrappedProvider As New SiteWrappedServiceProvider(serviceProvider)
				target.Site = wrappedProvider

				Dim shell As System.Reflection.Assembly = GetType(Microsoft.VisualStudio.Shell.DialogPage).Assembly
				Dim argtype As Type = shell.GetType("Microsoft.VisualStudio.Shell.DialogPage+PageApplyEventArgs", True)

				Dim method As MethodInfo = GetType(SccProviderOptions).GetMethod("OnApply", BindingFlags.NonPublic Or BindingFlags.Instance)
				Dim eventargs As Object = shell.CreateInstance(argtype.FullName)

				method.Invoke(target, New Object() { eventargs })
			End Using
		End Sub

		''' <summary>
        ''' A test for OnClosed (EventArgs).
        ''' </summary>
		<TestMethod()> _
		Public Sub OnClosedTest()
            Dim target As New SccProviderOptions()

			Dim method As MethodInfo = GetType(SccProviderOptions).GetMethod("OnClosed", BindingFlags.NonPublic Or BindingFlags.Instance)
            Dim e As New EventArgs()
			method.Invoke(target, New Object() { e })
		End Sub

		''' <summary>
        ''' A test for OnDeactivate (CancelEventArgs).
        ''' </summary>
		<TestMethod()> _
		Public Sub OnDeactivateTest()
            Dim target As New SccProviderOptions()

			Dim method As MethodInfo = GetType(SccProviderOptions).GetMethod("OnDeactivate", BindingFlags.NonPublic Or BindingFlags.Instance)
            Dim e As New CancelEventArgs()
			method.Invoke(target, New Object() { e })
		End Sub

		''' <summary>
        ''' A test for Window.
        ''' </summary>
		<TestMethod()> _
		Public Sub WindowTest()
            Dim target As New SccProviderOptions()

			Dim [property] As PropertyInfo = GetType(SccProviderOptions).GetProperty("Window", BindingFlags.NonPublic Or BindingFlags.Instance)
			Dim val As IWin32Window = TryCast([property].GetValue(target, Nothing), IWin32Window)
			Assert.IsNotNull(val, "The property page control was not created")
		End Sub
	End Class
End Namespace
