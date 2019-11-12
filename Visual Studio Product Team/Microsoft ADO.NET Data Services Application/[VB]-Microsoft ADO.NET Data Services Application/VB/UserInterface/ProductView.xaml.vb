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
Imports System.Windows.Shapes
Imports AdventureWorksService
Imports UserInterface.Gateways

Namespace UserInterface
	''' <summary>
	''' Interaction logic for ProductView.xaml
	''' </summary>
	Partial Public Class ProductView
		Inherits Window


		Private gateway As ProductGateway


	''' <summary>
		''' Lauches the entry form on startup
		''' </summary>
		Public Sub New(ByVal gateway As ProductGateway)
			InitializeComponent()
			Me.gateway = gateway
		End Sub

		''' <summary>
		''' If true then ProductView window is being used to create/add a new product.
		''' If false then ProductView window is being used to edit an existing product.
		''' </summary>
		Private _FormCreateMode As Boolean = True

		Private Property FormCreateMode() As Boolean
			Get
				Return _FormCreateMode
			End Get
			Set(ByVal value As Boolean)
				_FormCreateMode = value
			End Set
		End Property

		''' <summary>
		''' Product instance being edited or created.
		''' </summary>
		Private Property product() As Product


		Public Sub UpdateProduct(ByVal product As Product)
			Me.product = gateway.GetProducts(product.Name, product.ProductCategory)(0)
			FormCreateMode = False
			Me.Title = "Edit " & product.Name
		End Sub


		Private Sub BtnCancel_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
			Me.Close()
		End Sub

		Private Sub Window_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
			BindCategories()
			If FormCreateMode Then
				product = New Product()
			End If
			BindProduct()
		End Sub

		''' <summary>
		''' Bind properties of the product instance being updated/created to the corresponding TextBox.
		''' </summary>
		Private Sub BindProduct()
			txtProductNumber.DataContext = product
			txtName.DataContext = product
			txtListPrice.DataContext = product
			txtColor.DataContext = product
			CategoryComboBoxProductDetail.DataContext = product
			txtModifiedDate.DataContext = product
			txtSellStartDate.DataContext = product
			txtStandardCost.DataContext = product
		End Sub

		''' <summary>
		''' Query for list of Categories and bind to ComboBox
		''' </summary>
		Private Sub BindCategories()
			CategoryComboBoxProductDetail.ItemsSource = gateway.GetCategories()
			CategoryComboBoxProductDetail.SelectedIndex = 0
		End Sub


		Private Sub BtnSave_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
			If FormCreateMode Then
				product.ProductCategory = CType(CategoryComboBoxProductDetail.SelectedItem, ProductCategory)
				gateway.AddProduct(product)
			Else
				product.ProductCategory = CType(CategoryComboBoxProductDetail.SelectedItem, ProductCategory)
				gateway.UpdateProduct(product)
			End If
			Me.Close()
		End Sub
	End Class
End Namespace
