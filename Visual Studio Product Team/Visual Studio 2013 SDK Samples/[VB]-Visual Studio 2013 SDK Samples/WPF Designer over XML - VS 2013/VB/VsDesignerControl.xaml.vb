'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports System.Text

Namespace Microsoft.VsTemplateDesigner
	''' <summary>
	''' Interaction logic for VsDesignerControl.xaml
	''' </summary>
	Partial Public Class VsDesignerControl
        Inherits System.Windows.Controls.UserControl

		Public Sub New()
			InitializeComponent()
		End Sub

		Public Sub New(ByVal viewModel As IViewModel)
			DataContext = viewModel
			InitializeComponent()
			' wait until we're initialized to handle events
			AddHandler viewModel.ViewModelChanged, AddressOf ViewModelChanged
		End Sub

		Friend Sub DoIdle()
			' only call the view model DoIdle if this control has focus
			' otherwise, we should skip and this will be called again
			' once focus is regained
			Dim viewModel As IViewModel = TryCast(DataContext, IViewModel)
			If viewModel IsNot Nothing AndAlso Me.IsKeyboardFocusWithin Then
				viewModel.DoIdle()
			End If
		End Sub

		Private Sub ViewModelChanged(ByVal sender As Object, ByVal e As EventArgs)
			' this gets called when the view model is updated because the Xml Document was updated
			' since we don't get individual PropertyChanged events, just re-set the DataContext
			Dim viewModel As IViewModel = TryCast(DataContext, IViewModel)
			DataContext = Nothing ' first, set to null so that we see the change and rebind
			DataContext = viewModel
		End Sub

		Private Sub treeContent_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Dim treeView = TryCast(sender, System.Windows.Controls.TreeView)
			If treeView IsNot Nothing Then
				' make sure that any top-level items that contain other items are expanded
				For Each item As Object In treeView.Items
					Dim treeItem As TreeViewItem = TryCast(treeView.ItemContainerGenerator.ContainerFromItem(item), TreeViewItem)
					treeItem.IsExpanded = True
				Next item
			End If
		End Sub

		Private Sub treeContent_SelectedItemChanged(ByVal sender As Object, ByVal e As RoutedPropertyChangedEventArgs(Of Object))
			Dim viewModel = TryCast(DataContext, IViewModel)
            Dim treeView = TryCast(sender, System.Windows.Controls.TreeView)
			If (viewModel IsNot Nothing) AndAlso (treeView IsNot Nothing) Then
				' pass Selection events along to the view model so that the Properties window is updated
				viewModel.OnSelectChanged(treeView.SelectedItem)
			End If
		End Sub

		Private Sub cbLocation_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
			Dim viewModel = TryCast(DataContext, IViewModel)
            Dim comboBox = TryCast(sender, System.Windows.Controls.ComboBox)
			If Not viewModel.IsLocationFieldSpecified Then
				' don't show selection in combobox if there was no data in file
				comboBox.SelectedIndex = -1
			End If
		End Sub
	End Class
End Namespace
