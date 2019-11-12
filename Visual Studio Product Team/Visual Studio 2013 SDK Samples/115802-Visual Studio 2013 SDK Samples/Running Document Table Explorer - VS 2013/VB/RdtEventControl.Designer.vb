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
Namespace MyCompany.RdtEventExplorer
	Public Partial Class RdtEventControl
		''' <summary> 
		''' Required designer variable.
		''' </summary>
		Private components As System.ComponentModel.IContainer = Nothing

		''' <summary> 
		''' Clean up any resources being used.
		''' </summary>
		Protected Overrides Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
                If components IsNot Nothing Then
                    components.Dispose()
                End If
			End If
			MyBase.Dispose(disposing)
		End Sub


		#Region "Component Designer generated code"
		''' <summary> 
		''' Required method for Designer support - do not modify 
		''' the contents of this method with the code editor.
		''' </summary>
		Private Sub InitializeComponent()
			Me.components = New System.ComponentModel.Container()
			Me.eventGrid = New System.Windows.Forms.DataGridView()
			Me.detailsColumn = New System.Windows.Forms.DataGridViewButtonColumn()
			Me.myControlBindingSource = New System.Windows.Forms.BindingSource(Me.components)
			CType(Me.eventGrid, System.ComponentModel.ISupportInitialize).BeginInit()
			CType(Me.myControlBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
			Me.SuspendLayout()
			' 
			' eventGrid
			' 
			Me.eventGrid.Anchor = (CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) Or System.Windows.Forms.AnchorStyles.Left) Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles))
			Me.eventGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
			Me.eventGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
			Me.eventGrid.Location = New System.Drawing.Point(0, 0)
			Me.eventGrid.Name = "eventGrid"
			Me.eventGrid.Size = New System.Drawing.Size(389, 255)
			Me.eventGrid.TabIndex = 1
            AddHandler Me.eventGrid.CellClick, New System.Windows.Forms.DataGridViewCellEventHandler(AddressOf Me.eventGrid_CellClick)
			' 
			' detailsColumn
			' 
			Me.detailsColumn.Name = "detailsColumn"
			' 
			' myControlBindingSource
			' 
			Me.myControlBindingSource.DataSource = GetType(MyCompany.RdtEventExplorer.RdtEventControl)
			' 
			' RDTEventControl
			' 
			Me.AutoScaleDimensions = New System.Drawing.SizeF(6F, 13F)
			Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
			Me.BackColor = System.Drawing.SystemColors.Window
			Me.Controls.Add(Me.eventGrid)
			Me.Name = "RDTEventControl"
			Me.Size = New System.Drawing.Size(389, 293)
			CType(Me.eventGrid, System.ComponentModel.ISupportInitialize).EndInit()
			CType(Me.myControlBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
			Me.ResumeLayout(False)

		End Sub
		#End Region

		Private myControlBindingSource As System.Windows.Forms.BindingSource
		Private WithEvents eventGrid As System.Windows.Forms.DataGridView
		Private detailsColumn As System.Windows.Forms.DataGridViewButtonColumn
	End Class
End Namespace
