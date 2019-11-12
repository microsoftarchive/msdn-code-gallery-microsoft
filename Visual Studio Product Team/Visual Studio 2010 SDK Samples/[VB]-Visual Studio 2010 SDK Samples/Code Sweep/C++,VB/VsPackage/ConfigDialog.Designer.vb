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
Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    Partial Public Class ConfigDialog
        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.IContainer = Nothing

        ''' <summary>
        ''' Clean up any resources being used.
        ''' </summary>
        ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso (components IsNot Nothing) Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

#Region "Windows Form Designer generated code"

        ''' <summary>
        ''' Required method for Designer support - do not modify
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Dim resources As New System.ComponentModel.ComponentResourceManager(GetType(ConfigDialog))
            Me.label1 = New System.Windows.Forms.Label()
            Me._termTableListBox = New System.Windows.Forms.ListBox()
            Me._addButton = New System.Windows.Forms.Button()
            Me._removeButton = New System.Windows.Forms.Button()
            Me._closeButton = New System.Windows.Forms.Button()
            Me._scanButton = New System.Windows.Forms.Button()
            Me._autoScanCheckBox = New System.Windows.Forms.CheckBox()
            Me._toolTip = New System.Windows.Forms.ToolTip(Me.components)
            Me.SuspendLayout()
            ' 
            ' label1
            ' 
            resources.ApplyResources(Me.label1, "label1")
            Me.label1.Name = "label1"
            ' 
            ' _termTableListBox
            ' 
            resources.ApplyResources(Me._termTableListBox, "_termTableListBox")
            Me._termTableListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me._termTableListBox.FormattingEnabled = True
            Me._termTableListBox.Name = "_termTableListBox"
            Me._termTableListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
            AddHandler Me._termTableListBox.ControlRemoved, New System.Windows.Forms.ControlEventHandler(AddressOf Me._termTableListBox_ControlRemoved)
            AddHandler Me._termTableListBox.SelectedIndexChanged, New System.EventHandler(AddressOf Me._termTableListBox_SelectedIndexChanged)
            AddHandler Me._termTableListBox.ControlAdded, New System.Windows.Forms.ControlEventHandler(AddressOf Me._termTableListBox_ControlAdded)
            ' 
            ' _addButton
            ' 
            resources.ApplyResources(Me._addButton, "_addButton")
            Me._addButton.Name = "_addButton"
            AddHandler Me._addButton.Click, New System.EventHandler(AddressOf Me._addButton_Click)
            ' 
            ' _removeButton
            ' 
            resources.ApplyResources(Me._removeButton, "_removeButton")
            Me._removeButton.Name = "_removeButton"
            AddHandler Me._removeButton.Click, New System.EventHandler(AddressOf Me._removeButton_Click)
            ' 
            ' _closeButton
            ' 
            resources.ApplyResources(Me._closeButton, "_closeButton")
            Me._closeButton.Name = "_closeButton"
            AddHandler Me._closeButton.Click, New System.EventHandler(AddressOf Me._closeButton_Click)
            ' 
            ' _scanButton
            ' 
            resources.ApplyResources(Me._scanButton, "_scanButton")
            Me._scanButton.Name = "_scanButton"
            AddHandler Me._scanButton.Click, New System.EventHandler(AddressOf Me._scanButton_Click)
            ' 
            ' _autoScanCheckBox
            ' 
            resources.ApplyResources(Me._autoScanCheckBox, "_autoScanCheckBox")
            Me._autoScanCheckBox.Name = "_autoScanCheckBox"
            AddHandler Me._autoScanCheckBox.CheckedChanged, New System.EventHandler(AddressOf Me._autoScanCheckBox_CheckedChanged)
            ' 
            ' ConfigDialog
            ' 
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me._autoScanCheckBox)
            Me.Controls.Add(Me._scanButton)
            Me.Controls.Add(Me._closeButton)
            Me.Controls.Add(Me._removeButton)
            Me.Controls.Add(Me._addButton)
            Me.Controls.Add(Me._termTableListBox)
            Me.Controls.Add(Me.label1)
            Me.KeyPreview = True
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "ConfigDialog"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
            AddHandler Me.KeyPress, New System.Windows.Forms.KeyPressEventHandler(AddressOf Me.ConfigDialog_KeyPress)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

#End Region

        Private label1 As System.Windows.Forms.Label
        Private WithEvents _termTableListBox As System.Windows.Forms.ListBox
        Private WithEvents _addButton As System.Windows.Forms.Button
        Private WithEvents _removeButton As System.Windows.Forms.Button
        Private WithEvents _closeButton As System.Windows.Forms.Button
        Private WithEvents _scanButton As System.Windows.Forms.Button
        Private WithEvents _autoScanCheckBox As System.Windows.Forms.CheckBox
        Private _toolTip As System.Windows.Forms.ToolTip
    End Class
End Namespace