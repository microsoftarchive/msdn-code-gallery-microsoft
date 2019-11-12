'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports System.Reflection

Namespace Microsoft.Samples.VisualStudio.CodeSweep
    ''' <summary>
    ''' Global constants.
    ''' </summary>
    Public Class Globals
        ''' <summary>
        ''' Gets the full path of the CodeSweep folder under the current user's ApplicationData
        ''' folder where per-user files are stored.
        ''' </summary>
        Private Sub New()
        End Sub
        Public Shared ReadOnly Property AppDataFolder() As String
            Get
                Return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft\CodeSweep")
            End Get
        End Property

        ''' <summary>
        ''' The file name (no path) of the allowed extensions list.
        ''' </summary>
        Public Const AllowedExtensionsFileName As String = "extensions.xml"

        ''' <summary>
        ''' Gets the full path of the allowed extensions list.
        ''' </summary>
        Public Shared ReadOnly Property AllowedExtensionsPath() As String
            Get
                Return Path.Combine(AppDataFolder, AllowedExtensionsFileName)
            End Get
        End Property

        ''' <summary>
        ''' The file name (no path) of the default term table.
        ''' </summary>
        Public Const DefaultTermTableFileName As String = "sample_term_table.xml"

        ''' <summary>
        ''' Gets the full path of the default term table.
        ''' </summary>
        Public Shared ReadOnly Property DefaultTermTablePath() As String
            Get
                Return Path.Combine(AppDataFolder, DefaultTermTableFileName)
            End Get
        End Property

        Public Shared ReadOnly Property DefaultTermTableInstallPath() As String
            Get
                Dim expected As String = Path.Combine(InstallationFolder, DefaultTermTableFileName)

                If File.Exists(expected) Then
                    Return expected
                Else
                    ' Maybe this is not the installed product, but rather the VS SDK project.
                    ' Look for the dll two levels up from our current folder.
                    Return Utilities.AbsolutePathFromRelative(Path.Combine("..\..", DefaultTermTableFileName), InstallationFolder)
                End If
            End Get
        End Property

        Public Shared ReadOnly Property AllowedExtensionsInstallPath() As String
            Get
                Dim expected As String = Path.Combine(InstallationFolder, AllowedExtensionsFileName)

                If File.Exists(expected) Then
                    Return expected
                Else
                    ' Maybe this is not the installed product, but rather the VS SDK project.
                    ' Look for the dll two levels up from our current folder.
                    Return Utilities.AbsolutePathFromRelative(Path.Combine("..\..", AllowedExtensionsFileName), InstallationFolder)
                End If
            End Get
        End Property

#Region "Private Members"

        Private Shared ReadOnly Property InstallationFolder() As String
            Get
                Return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetLoadedModules()(0).FullyQualifiedName)
            End Get
        End Property

#End Region ' Private Members
    End Class
End Namespace
