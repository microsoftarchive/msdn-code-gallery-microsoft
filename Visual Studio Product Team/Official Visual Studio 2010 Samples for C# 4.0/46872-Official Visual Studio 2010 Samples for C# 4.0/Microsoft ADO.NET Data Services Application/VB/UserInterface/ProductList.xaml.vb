' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports UserInterface.Gateways
Imports AdventureWorksService


Namespace UserInterface
	''' <summary>
	''' Interaction logic for ProductList.xaml
	''' </summary>
	Partial Public Class ProductList
		Inherits Window

		Private gateway As ProductGateway


	''' <summary>
		''' Lauches the entry form on startup
		''' </summary>
		Public Sub New()
			InitializeComponent()
			gateway = New ProductGateway()
			AddHandler ProductsListView.MouseDoubleClick, AddressOf ProductsListView_MouseDoubleClick
		End Sub

	''' <summary>
		''' Bind results of gateway.GetCategories() to the Product Category combo box at the top of the form.
		''' </summary>
		Private Sub BindCategories()
			CategoryComboBox.ItemsSource = gateway.GetCategories()
			CategoryComboBox.SelectedIndex = 0
		End Sub

	''' <summary>
		''' Binds results of gateway.GetProducts(string ProductName, ProductCategory p) to the ListView control.
		''' </summary>
		Private Sub BindProducts()
			If CategoryComboBox.SelectedIndex > -1 Then
				ProductsListView.ItemsSource = gateway.GetProducts(NameTextBox.Text, TryCast(CategoryComboBox.SelectedItem, ProductCategory))
			End If
		End Sub

		Private Sub Window_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
			BindCategories()
		End Sub

		Private Sub ProductsListView_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
			Dim p As Product = TryCast(ProductsListView.SelectedItem, Product)
			Dim window As New ProductView(gateway)
			AddHandler window.Closed, AddressOf window_Closed
			window.UpdateProduct(p)
			window.Show()
		End Sub

	''' <summary>
		''' Call BindProducts() when Search button is clicked.
		''' </summary>
		Private Sub btnSearch_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
			BindProducts()
		End Sub

		Private Sub btnNewProduct_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
			Dim window As New ProductView(gateway)
			AddHandler window.Closed, AddressOf window_Closed
			window.Show()
		End Sub

	''' <summary>
		''' Call gateway.DeleteProduct() to initiate delete of selected product, if product cannot be deleted gateway.DeleteProduct 
	''' does not return null and response is shown to user via MessageBox.
		''' </summary>
		Private Sub btnDeleteProduct_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
			Dim p As Product = TryCast(ProductsListView.SelectedItem, Product)
			If p IsNot Nothing Then
				Dim returned As String = gateway.DeleteProduct(p)
				If returned IsNot Nothing Then
					MessageBox.Show(returned)
				End If
				BindProducts()
			End If
		End Sub

	''' <summary>
		''' Refresh List when new category is selected
		''' </summary>
		Private Sub CategoryComboBox_SelectionChanged(ByVal sender As Object, ByVal e As SelectionChangedEventArgs)
			BindProducts()
		End Sub

		Private Sub window_Closed(ByVal sender As Object, ByVal e As EventArgs)
			BindCategories()
		End Sub


	End Class
End Namespace
