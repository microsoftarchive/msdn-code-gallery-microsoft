/***************************** Module Header ******************************\
* Module Name:	Program.cs
* Project:		CSAzureDynamicTableEntity
* Copyright (c) Microsoft Corporation.
* 
* This sample shows how to define properties at the run time which will be 
* added to the table when inserting the entities.
* Windows Azure table has flexible schema, so we needn't to define a entity 
* class to serialize the entity.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\**************************************************************************/

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace CSAzureDynamicTableEntity
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=[Storage Account];AccountKey=[Primary Access Key ]";
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
           
            try
            {
                var client = storageAccount.CreateCloudTableClient();
                var table = client.GetTableReference("ShortMessages");
                table.CreateIfNotExists();
                dynamic entity = new DynamicObjectTableEntity("default", DateTime.Now.ToShortTimeString());

                entity.Name = "pete";
                entity.Message = "Hello";
                table.Execute(TableOperation.Insert(entity));
                Console.WriteLine("insert successfully!");                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();    
        }
      
    }
}
