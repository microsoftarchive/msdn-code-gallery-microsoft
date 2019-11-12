Imports System
Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Windows
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.Shell

''' <summary>
''' This class implements the tool window exposed by this package and hosts a user control.
'''
''' In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
''' usually implemented by the package implementer.
'''
''' This class derives from the ToolWindowPane class provided from the MPF in order to use its 
''' implementation of the IVsUIElementPane interface.
''' </summary>
<Guid("1bcb49dc-47f9-4eba-8d7d-b2baefe89076")> _
Public Class BuildProgressToolWindow
    Inherits ToolWindowPane

    Private progressBar As ProgressBarControl
    Private enableEffects As Boolean = False

    ''' <summary>
    ''' Standard constructor for the tool window.
    ''' </summary>
    Public Sub New()
        MyBase.New(Nothing)
        ' Set the window title reading it from the resources.
        Me.Caption = Resources.ToolWindowTitle
        ' Set the image that will appear on the tab of the window frame
        ' when docked with an other window
        ' The resource ID correspond to the one defined in the resx file
        ' while the Index is the offset in the bitmap strip. Each image in
        ' the strip being 16x16.
        Me.BitmapResourceID = 301
        Me.BitmapIndex = 1

        'This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
        'we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
        'the object returned by the Content property.
        progressBar = New ProgressBarControl()
        Me.Content = progressBar
    End Sub

    ' Enable/disable animation effects on the progress bar
    Public Property EffectsEnabled As Boolean
        Get
            Return enableEffects
        End Get
        Set(ByVal value As Boolean)
            enableEffects = value
            progressBar.AnimateColor = enableEffects
        End Set
    End Property

    ' Set the progress bar value
    Public Property Progress As Double
        Get
            Return progressBar.Value
        End Get
        Set(ByVal value As Double)
            progressBar.Value = value
        End Set
    End Property

    ' Set the progress bar text
    Public Property BarText As String
        Get
            Return progressBar.Text
        End Get
        Set(ByVal value As String)
            progressBar.Text = value
        End Set
    End Property
End Class
