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
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio.Shell

Namespace Microsoft.Samples.VisualStudio.IDE.EditorWithToolbox
	''' <summary>
	''' This class implements Visual studio package that is registered within Visual Studio IDE.
	''' The EditorPackage class uses a number of registration attribute to specify integration parameters.
	''' </summary>
	''' <remarks>
	''' <para>A description of the different attributes used here is given below:</para>
	''' <para>PackageRegistration: Used to determine if the package registration tool should look for additional 
	'''                      attributes. We currently specify that the package commands are described in a 
	'''                      managed package and not in a separate satellite UI binary.</para>
	''' <para>ProvideMenuResource: Provides information about menu resources. 
	'''     We specify ResourceId=1000 and version=1.</para>
	''' <para>ProvideEditorLogicalView: Indicates that our editor supports LOGVIEWID_Designer logical view and 
	'''     registers EditorFactory class to manage this view.</para>
	''' <para>ProvideEditorExtension: With this attribute we register our AddNewItem Templates 
	'''     for specified project types.</para>
	''' </remarks>
	' We register our AddNewItem Templates the Miscellaneous Files Project:
	' We register that our editor supports LOGVIEWID_Designer logical view
    <PackageRegistration(UseManagedResourcesOnly:=True), InstalledProductRegistration("#100", "#102", "10.0", IconResourceID:=400), ProvideEditorExtension(GetType(EditorFactory), ".tbx", 32, ProjectGuid:="{A2FE74E1-B743-11d0-AE1A-00A0C90FFFC3}", TemplateDir:="Templates", NameResourceID:=106), ProvideEditorLogicalView(GetType(EditorFactory), "{7651a703-06e5-11d1-8ebd-00a0c90f26ea}"), Guid(GuidStrings.GuidClientPackage)> _
    Public Class EditorPackage
        Inherits Package
        Implements IDisposable
#Region "Fields"

        Private editorFactory As EditorFactory

#End Region

#Region "Constructors"
        ''' <summary>
        ''' Initializes new instance of the EditorPackage.
        ''' </summary>
        Public Sub New()
        End Sub

#End Region

#Region "Methods"
        ''' <summary>
        ''' Create EditorPackage context.
        ''' </summary>
        Protected Overrides Sub Initialize()
            'Create Editor Factory.
            MyBase.Initialize()
            editorFactory = New EditorFactory()
            MyBase.RegisterEditorFactory(editorFactory)
        End Sub

#Region "IDisposable Pattern"
        ''' <summary>
        ''' Releases the resources used by the Package object.
        ''' </summary>
        Public Overloads Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
        End Sub

        ''' <summary>
        ''' Releases the resources used by the Package object.
        ''' </summary>
        ''' <param name="disposing">This parameter determines whether the method has been called directly or indirectly by a user's code.</param>
        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                Debug.WriteLine(String.Format(CultureInfo.CurrentCulture, "Entering Dispose() of: {0}", Me.ToString()))
                If disposing Then
                    If editorFactory IsNot Nothing Then
                        editorFactory.Dispose()
                        editorFactory = Nothing
                    End If
                    GC.SuppressFinalize(Me)
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub
#End Region

#End Region
    End Class
End Namespace
