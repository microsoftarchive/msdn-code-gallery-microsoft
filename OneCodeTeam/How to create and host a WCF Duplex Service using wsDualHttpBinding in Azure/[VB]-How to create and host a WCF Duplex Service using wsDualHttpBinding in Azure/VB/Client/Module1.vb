
'/****************************** Module Header ******************************\
'* Module Name: Program.vb
'* Project:     Client
'* Copyright (c) Microsoft Corporation.
'* 
'* 
'* WSDualHttpBinding supports duplex services. A duplex service is a service that uses duplex message patterns.
'* These patterns provide the ability for a service to communicate back to the client via a callback.
'*  
'* This is the client side programe. It's used to invoke the WCF service on Azure workrole. 
'*  
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/


Imports System.Threading
Imports System.ServiceModel

Module Module1
    Sub Main(ByVal args() As String)

        Dim p As New Program()
        p.CallWCF()

    End Sub

    Public Class Program
        Implements CalculatorServiceClient.IServiceCalculateCallback
        Private Shared strSymbol As String = "{0:C4}"

        Sub CallWCF()
            Dim instance As New InstanceContext(New Program())
            Dim client As New CalculatorServiceClient.ServiceCalculateClient(instance)

            Try
                Dim binding As WSDualHttpBinding = TryCast(client.Endpoint.Binding, WSDualHttpBinding)
                binding.ClientBaseAddress = New Uri("http://localhost:8081/client")

                Dim dbeOneParameter As Double = 0
                Dim dbeTwoParameter As Double = 0

                Console.WriteLine("Input the first parameter")
                inputParameter(dbeOneParameter)

                Console.WriteLine("Input the second parameter")
                inputParameter(dbeTwoParameter)

                Console.WriteLine()
                Console.ForegroundColor = ConsoleColor.DarkGreen
                Console.WriteLine("Starting to call WCF Service method SumByInputValue.")
                client.SumByInputValue(dbeOneParameter, dbeTwoParameter, strSymbol)
                Console.WriteLine("Calling WCF Service method SumByInputValue finished.")

                Console.WriteLine()
                Console.ResetColor()

                Console.ForegroundColor = ConsoleColor.DarkRed
                Console.WriteLine("Starting to call WCF Service method SumByInputValueOneway.")
                client.SumByInputValueOneway(dbeOneParameter, dbeTwoParameter, strSymbol)
                Console.WriteLine("Calling WCF Service method SumByInputValueOneway finished.")

            Catch ex As Exception

            End Try

            Console.ReadLine()
            Console.ResetColor()
        End Sub

        Shared Sub inputParameter(<System.Runtime.InteropServices.Out()> ByRef dbeParameter As Double)
            If Double.TryParse(Console.ReadLine(), dbeParameter) Then

            Else
                Console.WriteLine(" Input the parameter is wrong! Please input the parameter again. ")
                dbeParameter = 0
                inputParameter(dbeParameter)

            End If
        End Sub

        Public Sub DisplayResultByOption(ByVal strSymbol As String, ByVal dbeSumValue As Double) Implements CalculatorServiceClient.IServiceCalculateCallback.DisplayResultByOption
            Console.Write("Call WCFCallback method DisplayResultByOption:")
            Console.WriteLine(String.Format(strSymbol, dbeSumValue) + ".")
        End Sub

        Public Sub DisPlayResultByOptionOneWay(ByVal strSymbol As String, ByVal dbeSumValue As Double) Implements CalculatorServiceClient.IServiceCalculateCallback.DisPlayResultByOptionOneWay
            Console.Write("Call WCFCallback method DisPlayResultByOptionOneWay:")
            Console.WriteLine(String.Format(strSymbol, dbeSumValue) + ".")
        End Sub


    End Class

End Module
