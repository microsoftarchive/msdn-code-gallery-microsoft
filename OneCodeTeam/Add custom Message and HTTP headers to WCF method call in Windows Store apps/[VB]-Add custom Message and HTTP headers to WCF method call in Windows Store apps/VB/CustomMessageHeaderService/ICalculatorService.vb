'****************************** Module Header ******************************\
' Module Name:  ICalculatorService.vb
' Project:      CustomMessageHeaderService
' Copyright (c) Microsoft Corporation.
' 
' This is the Service contract interface of a simple Calculator.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/
Imports System.ServiceModel


' NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICalculatorService" in both code and config file together.
<ServiceContract> _
Public Interface ICalculatorService
    <OperationContract> _
    Function Add(n1 As Double, n2 As Double) As Double
End Interface
