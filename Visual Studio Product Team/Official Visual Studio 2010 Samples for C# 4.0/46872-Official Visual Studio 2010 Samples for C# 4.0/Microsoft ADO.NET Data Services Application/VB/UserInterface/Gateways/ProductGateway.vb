' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Linq
Imports System.Configuration
Imports AdventureWorksService
Imports System.Data.Services.Client

Namespace UserInterface.Gateways

	Public Class ProductGateway
		Implements IProductGateway

		''' <summary>
		''' DataServiceContext object representing the runtime context for the data service.
		''' </summary>
		Private context As AdventureWorksLTEntities

		''' <summary>
		''' URI representing the service entry point
		''' </summary>
		Private serviceUri As Uri

		''' <summary>
		''' Initialize DataServiceContext
		''' </summary>
		Public Sub New()
			serviceUri = New Uri("http://localhost:50000/AdventureWorks.svc")
			context = New AdventureWorksLTEntities(serviceUri)
			context.MergeOption = MergeOption.OverwriteChanges
		End Sub

		''' <summary>
		''' If no product name is specified return all products with the specified categoryId, otherwise return only products with the specified categoryId and product name.
		''' </summary>
	 ''' <param name="productName">The product name used to query Products</param>
		''' <param name="category">The category used to query Products</param>
		Public Function GetProducts(ByVal productName As String, ByVal category As ProductCategory) As IList(Of Product) Implements IProductGateway.GetProducts
			Dim query As IEnumerable(Of Product)

			Dim categoryId As Integer = category.ProductCategoryID
			If Not String.IsNullOrEmpty(productName) Then
				query = From p In context.Products.Expand("ProductCategory")
				        Where p.ProductCategory.ProductCategoryID = categoryId AndAlso p.Name = productName
				        Select p
			Else
				query = From p In context.Products.Expand("ProductCategory")
				        Where p.ProductCategory.ProductCategoryID = categoryId
				        Select p
			End If

			Try
				Dim productSet As List(Of Product) = query.ToList()
				Return productSet
			Catch e1 As Exception
				Return Nothing
			End Try
		End Function

		''' <summary>
		''' Return all Product Categories
		''' </summary>
		Public Function GetCategories() As IList(Of ProductCategory) Implements IProductGateway.GetCategories
			Return context.ProductCategories.ToList()
		End Function

		''' <summary>
		''' Try to delete the specified product, if product cannot be deleted, return error message "Product Cannot be Deleted"
		''' </summary>
	''' <param name="product">The product to be deleted</param>
		Public Function DeleteProduct(ByVal product As Product) As String Implements IProductGateway.DeleteProduct
			context.DeleteObject(product)

			Try
				context.SaveChanges()
			Catch e1 As DataServiceRequestException
				Return "Product Cannot be Deleted"
			End Try
			Return Nothing
		End Function

	 ''' <summary>
		''' This method assumes that all fields have been changed and updates the entire entity, including the association to ProductCategory.
		''' Changes are sent to the server using SaveChangesOptions.Batch so that all operations are sent in a single HTTP request.
	''' </summary>
	''' <param name="product">The product to be Updated</param>
		Public Sub UpdateProduct(ByVal product As Product) Implements IProductGateway.UpdateProduct
			Dim newCategory As ProductCategory = product.ProductCategory
			context.SetLink(product, "ProductCategory", newCategory)
			context.UpdateObject(product)
			context.SaveChanges(SaveChangesOptions.Batch)
		End Sub

	 ''' <summary>
		''' Add new product object to the DataServiceContext, and associat the object with an existing ProductCategory.
		''' Changes are sent to the server using SaveChangesOptions.Batch so that all operations are sent in a single HTTP request.
	''' </summary>
	''' <param name="product">The product to be Added</param>
		Public Sub AddProduct(ByVal product As Product) Implements IProductGateway.AddProduct
			product.rowguid = Guid.NewGuid()
			context.AddObject("Products", product)
			context.SetLink(product, "ProductCategory", product.ProductCategory)
			context.SaveChanges(SaveChangesOptions.Batch)
		End Sub

	End Class
End Namespace
