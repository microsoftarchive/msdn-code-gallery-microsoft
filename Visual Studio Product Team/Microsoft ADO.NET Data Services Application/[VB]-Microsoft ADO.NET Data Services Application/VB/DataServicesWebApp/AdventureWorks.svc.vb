' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Data.Services
Imports System.Linq
Imports System.ServiceModel.Web
Imports System.Web

Namespace DataServicesWebApp
	Public Class AdventureWorks
		Inherits DataService(Of AdventureWorksLTEntities)
		''' <summary>
		''' This method is called only once to initialize service-wide policies.
		''' </summary>
		Public Shared Sub InitializeService(ByVal config As IDataServiceConfiguration)
			' TODO: set rules to indicate which entity sets and service operations are visible, updatable, etc.
			' Examples:
			' config.SetEntitySetAccessRule("MyEntityset", EntitySetRights.AllRead);
			' config.SetServiceOperationAccessRule("MyServiceOperation", ServiceOperationRights.All);

			' For testing purposes use "*" to indicate all entity sets/service operations.
			' "*" should NOT be used in production systems.
			' This Sample only exposes the entity sets needed by the application we are building.
			' This Sample uses EntitySetRight.All which allows both Read and Write access to the Entity Set.
			config.SetEntitySetAccessRule("Products", EntitySetRights.All)
			config.SetEntitySetAccessRule("ProductCategories", EntitySetRights.All)
			config.SetEntitySetAccessRule("ProductDescriptions", EntitySetRights.All)
			config.SetEntitySetAccessRule("ProductModelProductDescriptions", EntitySetRights.All)
			config.SetEntitySetAccessRule("ProductModels", EntitySetRights.All)
		End Sub
	End Class
End Namespace
