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
Imports System.Runtime.InteropServices

Namespace Microsoft.Samples.VisualStudio.Services.Interfaces
	''' <summary>
	''' This is the interface that will be implemented by the global service exposed
	''' by the package defined in Reference.Services.
	''' Notice that we have to define this interface as COM visible so that 
	''' it will be possible to query for it from the native version of IServiceProvider.
	''' </summary>
	<Guid("5436ecaa-c6ad-4ffa-ae05-f78b946e1258"), ComVisible(True)> _
	Public Interface IMyGlobalService
		Sub GlobalServiceFunction()
		Function CallLocalService() As Integer
	End Interface

	''' <summary>
	''' The goal of this interface is actually just to define a Type (or Guid from the native
	''' client's point of view) that will be used to identify the service.
	''' In theory, we could use the interface defined above, but it is a good practice to always
	''' define a new type as the service's identifier because a service can expose different interfaces.
	''' </summary>
	<Guid("aa5eb4f7-b327-4d5a-acda-6db3193545e1")> _
	Public Interface SMyGlobalService
	End Interface
End Namespace
