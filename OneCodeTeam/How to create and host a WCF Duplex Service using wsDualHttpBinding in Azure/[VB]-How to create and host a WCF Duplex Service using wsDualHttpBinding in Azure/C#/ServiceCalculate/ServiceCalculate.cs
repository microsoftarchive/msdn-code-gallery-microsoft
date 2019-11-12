/****************************** Module Header ******************************\
* Module Name:  ServiceCalculate.cs
* Project:	    ServiceCalculate
* Copyright (c) Microsoft Corporation.
* 
* WSDualHttpBinding supports duplex services. A duplex service is a service that uses duplex message patterns.
* These patterns provide the ability for a service to communicate back to the client via a callback.
*  
* This class implements ServiceCalculate.IServiceCalculate interface. 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
namespace ServiceCalculate
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession,ConcurrencyMode=ConcurrencyMode.Reentrant)]
   public class ServiceCalculate:IServiceCalculate
    {
        public void SumByInputValue(double dbeOneParameter, double dbeTwoParameter, string strSymbol)
        {
            double dbeSum = 0;
            try
            {
                dbeSum = dbeOneParameter + dbeTwoParameter;
            }
            catch
            { 
            
            }
            OperationContext ctx = OperationContext.Current;
            IServiceCalulateCallBack callBack = ctx.GetCallbackChannel<IServiceCalulateCallBack>();
            callBack.DisplayResultByOption(strSymbol, dbeSum);
        }


        public void SumByInputValueOneway(double dbeOneParameter, double dbeTwoParameter, string strSymbol)
        {
            double dbeSum = 0;
            try
            {
                dbeSum = dbeOneParameter + dbeTwoParameter;
            }
            catch
            {
            }

            OperationContext ctx = OperationContext.Current;
            IServiceCalulateCallBack callBack = ctx.GetCallbackChannel<IServiceCalulateCallBack>();
            callBack.DisPlayResultByOptionOneWay(strSymbol, dbeSum);
        }
    }
}
