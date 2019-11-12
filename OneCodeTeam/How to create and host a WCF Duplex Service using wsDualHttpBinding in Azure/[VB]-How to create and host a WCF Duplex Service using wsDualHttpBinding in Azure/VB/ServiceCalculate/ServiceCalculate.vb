'/****************************** Module Header ******************************\
'* Module Name:  ServiceCalculate.vb
'* Project:	    ServiceCalculate
'* Copyright (c) Microsoft Corporation.
'* 
'* WSDualHttpBinding supports duplex services. A duplex service is a service that uses duplex message patterns.
'* These patterns provide the ability for a service to communicate back to the client via a callback.
'*  
'* This class implements ServiceCalculate.IServiceCalculate interface. 
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

<ServiceBehavior(InstanceContextMode:=InstanceContextMode.PerSession, ConcurrencyMode:=ConcurrencyMode.Reentrant)>
Public Class ServiceCalculate
    Implements IServiceCalculate
    Public Sub SumByInputValue(ByVal dbeOneParameter As Double, ByVal dbeTwoParameter As Double, ByVal strSymbol As String) Implements IServiceCalculate.SumByInputValue
        Dim dbeSum As Double = 0
        Try
            dbeSum = dbeOneParameter + dbeTwoParameter
        Catch

        End Try

        Dim ctx As OperationContext = OperationContext.Current
        Dim callBack As IServiceCalulateCallBack = ctx.GetCallbackChannel(Of IServiceCalulateCallBack)()
        callBack.DisplayResultByOption(strSymbol, dbeSum)
    End Sub


    Public Sub SumByInputValueOneway(ByVal dbeOneParameter As Double, ByVal dbeTwoParameter As Double, ByVal strSymbol As String) Implements IServiceCalculate.SumByInputValueOneway
        Dim dbeSum As Double = 0
        Try
            dbeSum = dbeOneParameter + dbeTwoParameter
        Catch
        End Try

        Dim ctx As OperationContext = OperationContext.Current
        Dim callBack As IServiceCalulateCallBack = ctx.GetCallbackChannel(Of IServiceCalulateCallBack)()
        callBack.DisPlayResultByOptionOneWay(strSymbol, dbeSum)
    End Sub
End Class