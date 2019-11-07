/***************************** Module Header ******************************\
* Module Name:	TableDataSource.cs
* Project:		CSAzureDeleteWithoutRetrieving
* Copyright (c) Microsoft Corporation.
* 
* This sample demonstrates how to reduce the request to Azure storage service.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\**************************************************************************/

using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace CSAzureDeleteWithoutRetrieving.DataSource
{
    public static class TableDataSource
    {
        public static readonly StorageCredentials cred = new StorageCredentials(
       "{Storage-account}",
       "{Storage-account-key }");

        private static string tableName = "CSAzureDeleteWithoutRetrievingSampleTable";
        private static IRetryPolicy conflictRetryPolicy = new ConflictRetryPolicy(TimeSpan.FromSeconds(5), 10);
        private static CloudTableClient client = new  CloudTableClient( new Uri(string.Format("http://{0}.table.core.windows.net", cred.AccountName)), cred) 
            {
                RetryPolicy = conflictRetryPolicy 
            };
        

        public static async Task<ObservableCollection<DynamicTableEntity>> GetTableEntities()
        {
            var table = client.GetTableReference(tableName);
            TableQuery query = new TableQuery();

            var results = await table.ExecuteQuerySegmentedAsync(query, null);
            ObservableCollection<DynamicTableEntity> itemList = new ObservableCollection<DynamicTableEntity>();
            foreach (var item in results)
            {
                itemList.Add((DynamicTableEntity)item);
            }
            return itemList;
        }

        public static async Task<bool> DeleteTable()
        {
            try
            {
                var table = client.GetTableReference(tableName);
                TableRequestOptions requestOptions = new TableRequestOptions()
                {
                    RetryPolicy = conflictRetryPolicy,
                };

                await table.DeleteIfExistsAsync(requestOptions, null);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
            
        }
        public static async Task<ObservableCollection<DynamicTableEntity>> CreateSampleData()
        {
            var table = client.GetTableReference(tableName);
            try
            {                
                TableRequestOptions requestOptions=new TableRequestOptions()
                {
                    RetryPolicy = conflictRetryPolicy,
                };
                if (await table.CreateIfNotExistsAsync(requestOptions,null))
                {
                    TableBatchOperation bacthOperation = new TableBatchOperation();
                    ObservableCollection<DynamicTableEntity> entityList = SampleData;
                    foreach (var entity in entityList)
                    {
                        bacthOperation.Insert(entity);
                    }
                    await table.ExecuteBatchAsync(bacthOperation);
                    return entityList;
                }
                else
                {
                    return await (GetTableEntities());
                }        
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static ObservableCollection<DynamicTableEntity> SampleData
        {
            get
            {
                ObservableCollection<DynamicTableEntity> entityList = new ObservableCollection<DynamicTableEntity>();
                DynamicTableEntity entity;
                for (int i = 0; i < 10; i++)
                {
                    Dictionary<string, EntityProperty> dic = new Dictionary<string, EntityProperty>();
                    dic.Add("Time", EntityProperty.GeneratePropertyForDateTimeOffset(new DateTime(2001, 1, 1)));
                    dic.Add("ToDoItem", EntityProperty.GeneratePropertyForString("Do the case" + i));

                    entity = new DynamicTableEntity("Partition1", "Row" + i, DateTime.Now.ToString(), dic);
                    entityList.Add(entity);
                }
                return entityList;
            }
        }

        public static async Task<bool> DeleteEntity(DynamicTableEntity entity)
        {
            try
            {
                var table = client.GetTableReference(tableName);

                TableOperation deleteOperation = TableOperation.Delete(entity);
                var result = await table.ExecuteAsync(deleteOperation);

                return true;
            }
            catch (Exception e)
            {
                // In windows store apps, StorageException is an internal class.
                // You can't convert Exception to StorageException, so you should use 
                // RequestResult.TranslateFromExceptionMessage(e.Message) to get the HttpStatusCode.
                var result = RequestResult.TranslateFromExceptionMessage(e.Message);

                // We treat 404 as it has already been deleted.
                if (result.HttpStatusCode == 404)
                {
                    return true;
                }
                else
                {
                    return false;
                    // throw new WebException(result.HttpStatusMessage);
                }
            }

        }

        public static async Task<bool> DeleteEntities(ObservableCollection<DynamicTableEntity> entities)
        {
            if (entities.Count>0)
            {
                TableBatchOperation bacthOperation = new TableBatchOperation();

                try
                {
                    var table = client.GetTableReference(tableName);


                    foreach (var entity in entities)
                    {
                        bacthOperation.Delete(entity);
                    }
                    await table.ExecuteBatchAsync(bacthOperation);
                    return true;

                }
                catch (Exception e)
                {
                    var result = RequestResult.TranslateFromExceptionMessage(e.Message);
                    return false;
                   
                }
            }
            else
            { 
                return true;
            }  
        }
    }
}
