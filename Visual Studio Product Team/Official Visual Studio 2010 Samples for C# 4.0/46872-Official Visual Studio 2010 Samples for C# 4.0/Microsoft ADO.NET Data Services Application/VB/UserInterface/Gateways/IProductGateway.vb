' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports AdventureWorksService


Namespace UserInterface.Gateways
	Public Interface IProductGateway
		Function GetProducts(ByVal productName As String, ByVal category As ProductCategory) As IList(Of Product)
		Function GetCategories() As IList(Of ProductCategory)
		Function DeleteProduct(ByVal product As Product) As String
		Sub UpdateProduct(ByVal product As Product)
		Sub AddProduct(ByVal product As Product)
	End Interface
End Namespace
