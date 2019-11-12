
'/****************************** Module Header ******************************\
'* Module Name:  IServiceCalculate.vb
'* Project:	    ServiceCalculate
'* Copyright (c) Microsoft Corporation.
'* 
'* WSDualHttpBinding supports duplex services. A duplex service is a service that uses duplex message patterns.
'* These patterns provide the ability for a service to communicate back to the client via a callback.
'*  
'* This interface defines the contracts of the WCF service.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/


Imports System.ServiceModel

<ServiceContract(CallbackContract:=GetType(IServiceCalulateCallBack), SessionMode:=SessionMode.Required)>
Public Interface IServiceCalculate
    ''' <summary>
    ''' Gets a value dbeOneParameter plus dbeTwoParameter when the mehtod finishes waiting for an underlying response message.
    ''' </summary>
    ''' <param name="dbeOneParameter"></param>
    ''' <param name="dbeTwoParameter"></param>
    ''' <param name="strSymbol"></param>
    ''' <remarks></remarks>
    <OperationContract>
    Sub SumByInputValue(ByVal dbeOneParameter As Double, ByVal dbeTwoParameter As Double, ByVal strSymbol As String)

    ''' <summary>
    ''' Gets a value dbeOneParameter plus dbeTwoParameter without the method finishing waiting for an underlying response message.
    ''' </summary>
    ''' <param name="dbeOneParameter"></param>
    ''' <param name="dbeTwoParameter"></param>
    ''' <param name="strSymbol"></param>
    ''' <remarks></remarks>
    <OperationContract(IsOneWay:=True)>
    Sub SumByInputValueOneway(ByVal dbeOneParameter As Double, ByVal dbeTwoParameter As Double, ByVal strSymbol As String)
End Interface

Public Interface IServiceCalulateCallBack

    ''' <summary>
    ''' The value will be displayed according to the format value chosen when the method finishes waiting for an underlying response message.
    ''' </summary>
    ''' <param name="strSymbol"></param>
    ''' <param name="dbeSumValue"></param>
    ''' <remarks></remarks>
    <OperationContract>
    Sub DisplayResultByOption(ByVal strSymbol As String, ByVal dbeSumValue As Double)

    ''' <summary>
    ''' The value will be displayed according to the format value chosen without the method finishing waiting for an underlying response message. 
    ''' </summary>
    ''' <param name="strSymbol"></param>
    ''' <param name="dbeSumValue"></param>
    ''' <remarks></remarks>
    <OperationContract(IsOneWay:=True)>
    Sub DisPlayResultByOptionOneWay(ByVal strSymbol As String, ByVal dbeSumValue As Double)
End Interface