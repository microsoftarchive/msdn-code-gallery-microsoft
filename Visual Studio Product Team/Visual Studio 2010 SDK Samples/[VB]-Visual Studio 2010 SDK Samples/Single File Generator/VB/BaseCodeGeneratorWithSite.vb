'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports System.CodeDom.Compiler
Imports System.Runtime.InteropServices
Imports EnvDTE
Imports EnvDTE80
Imports VSLangProj
Imports VSOLE = Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.Designer.Interfaces

Namespace Microsoft.Samples.VisualStudio.GeneratorSample
	''' <summary>
	''' Base code generator with site implementation
	''' </summary>
	Public MustInherit Class BaseCodeGeneratorWithSite
		Inherits BaseCodeGenerator
		Implements VSOLE.IObjectWithSite
		Private site As Object = Nothing

        Private codeDomProvider_local As CodeDomProvider = Nothing

        Private serviceProvider_local As ServiceProvider = Nothing

#Region "IObjectWithSite Members"

        ''' <summary>
        ''' GetSite method of IOleObjectWithSite
        ''' </summary>
        ''' <param name="riid">interface to get</param>
        ''' <param name="ppvSite">IntPtr in which to stuff return value</param>
        Private Sub GetSite(ByRef riid As Guid, <System.Runtime.InteropServices.Out()> ByRef ppvSite As IntPtr) Implements VSOLE.IObjectWithSite.GetSite
            If site Is Nothing Then
                Throw New COMException("object is not sited", VSConstants.E_FAIL)
            End If

            Dim pUnknownPointer As IntPtr = Marshal.GetIUnknownForObject(site)
            Dim intPointer As IntPtr = IntPtr.Zero
            Marshal.QueryInterface(pUnknownPointer, riid, intPointer)

            If intPointer = IntPtr.Zero Then
                Throw New COMException("site does not support requested interface", VSConstants.E_NOINTERFACE)
            End If

            ppvSite = intPointer
        End Sub

        ''' <summary>
        ''' SetSite method of IOleObjectWithSite
        ''' </summary>
        ''' <param name="pUnkSite">site for this object to use</param>
        Private Sub SetSite(ByVal pUnkSite As Object) Implements VSOLE.IObjectWithSite.SetSite
            site = pUnkSite
            codeDomProvider_local = Nothing
            serviceProvider_local = Nothing
        End Sub

#End Region

        ''' <summary>
        ''' Demand-creates a ServiceProvider
        ''' </summary>
        Private ReadOnly Property SiteServiceProvider() As ServiceProvider
            Get
                If serviceProvider_local Is Nothing Then
                    serviceProvider_local = New ServiceProvider(TryCast(site, VSOLE.IServiceProvider))
                    Debug.Assert(serviceProvider_local IsNot Nothing, "Unable to get ServiceProvider from site object.")
                End If
                Return serviceProvider_local
            End Get
        End Property

        ''' <summary>
        ''' Method to get a service by its GUID
        ''' </summary>
        ''' <param name="serviceGuid">GUID of service to retrieve</param>
        ''' <returns>An object that implements the requested service</returns>
        Protected Function GetService(ByVal serviceGuid As Guid) As Object
            Return SiteServiceProvider.GetService(serviceGuid)
        End Function

        ''' <summary>
        ''' Method to get a service by its Type
        ''' </summary>
        ''' <param name="serviceType">Type of service to retrieve</param>
        ''' <returns>An object that implements the requested service</returns>
        Protected Function GetService(ByVal serviceType As Type) As Object
            Return SiteServiceProvider.GetService(serviceType)
        End Function

        ''' <summary>
        ''' Returns a CodeDomProvider object for the language of the project containing
        ''' the project item the generator was called on
        ''' </summary>
        ''' <returns>A CodeDomProvider object</returns>
        Protected Overridable Function GetCodeProvider() As CodeDomProvider
            If codeDomProvider_local Is Nothing Then
                'Query for IVSMDCodeDomProvider/SVSMDCodeDomProvider for this project type
                Dim provider As IVSMDCodeDomProvider = TryCast(GetService(GetType(SVSMDCodeDomProvider)), 
                    IVSMDCodeDomProvider)
                If provider IsNot Nothing Then
                    codeDomProvider_local = TryCast(provider.CodeDomProvider, CodeDomProvider)
                Else
                    'In the case where no language specific CodeDom is available, fall back to C#
                    codeDomProvider_local = CodeDomProvider.CreateProvider("C#")
                End If
            End If
            Return codeDomProvider_local
        End Function

		''' <summary>
		''' Gets the default extension of the output file from the CodeDomProvider
		''' </summary>
		''' <returns></returns>
		Protected Overrides Function GetDefaultExtension() As String
			Dim codeDom As CodeDomProvider = GetCodeProvider()
			Debug.Assert(codeDom IsNot Nothing, "CodeDomProvider is NULL.")
            Dim extension = codeDom.FileExtension
			If extension IsNot Nothing AndAlso extension.Length > 0 Then
				extension = "." & extension.TrimStart(".".ToCharArray())
			End If
			Return extension
		End Function

		''' <summary>
		''' Returns the EnvDTE.ProjectItem object that corresponds to the project item the code 
		''' generator was called on
		''' </summary>
		''' <returns>The EnvDTE.ProjectItem of the project item the code generator was called on</returns>
		Protected Function GetProjectItem() As ProjectItem
            Dim p = GetService(GetType(ProjectItem))
			Debug.Assert(p IsNot Nothing, "Unable to get Project Item.")
			Return CType(p, ProjectItem)
		End Function

		''' <summary>
		''' Returns the EnvDTE.Project object of the project containing the project item the code 
		''' generator was called on
		''' </summary>
		''' <returns>
		''' The EnvDTE.Project object of the project containing the project item the code generator was called on
		''' </returns>
		Protected Function GetProject() As Project
			Return GetProjectItem().ContainingProject
		End Function

		''' <summary>
		''' Returns the VSLangProj.VSProjectItem object that corresponds to the project item the code 
		''' generator was called on
		''' </summary>
		''' <returns>The VSLangProj.VSProjectItem of the project item the code generator was called on</returns>
		Protected Function GetVSProjectItem() As VSProjectItem
			Return CType(GetProjectItem().Object, VSProjectItem)
		End Function

		''' <summary>
		''' Returns the VSLangProj.VSProject object of the project containing the project item the code 
		''' generator was called on
		''' </summary>
		''' <returns>
		''' The VSLangProj.VSProject object of the project containing the project item 
		''' the code generator was called on
		''' </returns>
		Protected Function GetVSProject() As VSProject
			Return CType(GetProject().Object, VSProject)
		End Function
	End Class
End Namespace