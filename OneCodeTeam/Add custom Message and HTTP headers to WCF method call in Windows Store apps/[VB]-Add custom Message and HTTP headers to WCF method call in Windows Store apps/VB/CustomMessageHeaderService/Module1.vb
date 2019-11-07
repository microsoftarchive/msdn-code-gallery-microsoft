'****************************** Module Header ******************************\
' Module Name:  Module1.vb
' Project:      CustomMessageHeaderService
' Copyright (c) Microsoft Corporation.
' 
' This is a server end application listening on a port.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/
Imports System.ServiceModel.Description
Imports System.ServiceModel


Module Module1
    ' WCF service's address.
    Dim strBaseAddress As String = "http://localhost:8001"

    Sub Main()
        Dim baseAddress As New Uri(strBaseAddress)
        ' Create a ServiceHost instance with a specific service address.
        Dim localHost As New ServiceHost(GetType(CalculatorService), baseAddress)

        Try
            ' Release the metadata.
            Dim _serviceMetadataBehavior As New ServiceMetadataBehavior()
            _serviceMetadataBehavior.HttpGetEnabled = True
            localHost.Description.Behaviors.Add(_serviceMetadataBehavior)
            localHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexHttpBinding(), "mex")

            ' Specify the type of the service contract ICalculator and the binding type WSHttpBinding.
            localHost.AddServiceEndpoint(GetType(ICalculatorService), New BasicHttpBinding(), "")

            ' Start the Service.
            localHost.Open()
            Console.WriteLine("Service listening on {0}", baseAddress.AbsoluteUri)
            Console.ReadLine()
            localHost.Close()
        Catch oEx As Exception
            Console.WriteLine("Exception: {0}", oEx.Message)
        End Try
    End Sub

End Module
