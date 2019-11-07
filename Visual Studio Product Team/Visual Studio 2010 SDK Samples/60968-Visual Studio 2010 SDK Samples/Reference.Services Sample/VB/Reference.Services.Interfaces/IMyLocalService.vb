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
	''' This is the interface implemented by the local service.
	''' Notice that we have to define this interface as COM visible so that 
	''' it will be possible to query for it from the native version of IServiceProvider.
	''' </summary>
	<Guid("14ba15f0-cce1-4208-8532-bbe52e610acb"), ComVisible(True)> _
	Public Interface IMyLocalService
		Function LocalServiceFunction() As Integer
	End Interface

	''' <summary>
	''' This interface is used to define the Type or Guid that identifies the service.
	''' It is not strictly required because our service will implement only one interface,
	''' but in case of services that implement multiple interfaces it is good practice to define
	''' a different type to identify the service itself.
	''' </summary>
	<Guid("f2ed44e5-31a4-4aac-9f83-fc20e63ce64c")> _
	Public Interface SMyLocalService
	End Interface
End Namespace
