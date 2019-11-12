/***************************** Module Header ******************************\
* Module Name:	ConflictRetryPolicy.cs
* Project:		CSAzureDeleteWithoutRetrieving
* Copyright (c) Microsoft Corporation.
* 
* This sample demonstrates how to reduce the request to Azure storage service.
*
* This class implements IRetryPolicy. It will handle the conflict error when 
* Azure Data Center has transient fault, and retry the request later.
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
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System;

namespace CSAzureDeleteWithoutRetrieving
{

    public class ConflictRetryPolicy:IRetryPolicy
    {
        int maxRetryAttemps = 10;
        TimeSpan defaultRetryInterval = TimeSpan.FromSeconds(5);

        public ConflictRetryPolicy(TimeSpan deltaBackoff, int retryAttempts)
        {
            maxRetryAttemps = retryAttempts;
            defaultRetryInterval = deltaBackoff;
        }

        public IRetryPolicy CreateInstance()
        {
            return new ConflictRetryPolicy(TimeSpan.FromSeconds(5), 10);
        }

        public bool ShouldRetry(int currentRetryCount, int statusCode, Exception lastException, out TimeSpan retryInterval, OperationContext operationContext)
        {
            retryInterval = defaultRetryInterval;
            if (currentRetryCount >= maxRetryAttemps)
            {
                return false;
            }
            if (statusCode == 409)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
