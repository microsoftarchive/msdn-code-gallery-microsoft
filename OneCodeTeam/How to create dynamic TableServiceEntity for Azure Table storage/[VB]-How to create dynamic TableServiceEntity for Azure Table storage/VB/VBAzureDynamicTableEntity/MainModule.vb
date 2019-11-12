'***************************** Module Header ******************************\
' Module Name:	MainModule.vb
' Project:		VBAzureDynamicTableEntity
' Copyright (c) Microsoft Corporation.
' 
' This sample shows how to define properties at the run time which will be 
' added to the table when inserting the entities.
' Windows Azure table has flexible schema, so we needn't to define an entity 
' class to serialize the entity.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports Microsoft.WindowsAzure.Storage.Table
Imports Microsoft.WindowsAzure.Storage

Module MainModule

    Sub Main()

        Dim connectionString = "DefaultEndpointsProtocol=https;AccountName=[Storage Account];AccountKey=[Primary Access Key ]"
        Dim storageAccount As CloudStorageAccount = CloudStorageAccount.Parse(connectionString)

        Try
            Dim client = storageAccount.CreateCloudTableClient()
            Dim table = client.GetTableReference("VBShortMessages")
            table.CreateIfNotExists()

            ' dynamic keyword in vb.net does not exist, it's identical to Object is used in VB.net with Option strict off.
            Dim entity As Object = New DynamicObjectTableEntity("default", DateTime.Now.ToShortTimeString())

            entity.Name = "pete"
            entity.Message = "Hello"
            table.Execute(TableOperation.Insert(entity))
            Console.WriteLine("insert successfully!")
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

        Console.Read()
    End Sub

End Module
