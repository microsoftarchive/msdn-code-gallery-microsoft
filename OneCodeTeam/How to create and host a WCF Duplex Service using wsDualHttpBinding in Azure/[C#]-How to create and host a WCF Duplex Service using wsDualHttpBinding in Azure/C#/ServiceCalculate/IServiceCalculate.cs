/****************************** Module Header ******************************\
* Module Name:  IServiceCalculate.cs
* Project:	    ServiceCalculate
* Copyright (c) Microsoft Corporation.
* 
* WSDualHttpBinding supports duplex services. A duplex service is a service that uses duplex message patterns.
* These patterns provide the ability for a service to communicate back to the client via a callback.
*  
* This interface defines the contracts of the WCF service.
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
    [ServiceContract(CallbackContract = typeof(IServiceCalulateCallBack), SessionMode = SessionMode.Required)]
    public interface IServiceCalculate
    {
        /// <summary>
        /// Gets a value dbeOneParameter plus dbeTwoParameter when the mehtod finishes waiting for an underlying response message.
        /// </summary>
        /// <param name="dbeOneParameter"></param>
        /// <param name="dbeTwoParameter"></param>
        /// <param name="strSymbol"></param>
        [OperationContract]
        void SumByInputValue(double dbeOneParameter, double dbeTwoParameter, string strSymbol);

        /// <summary>
        /// Gets a value dbeOneParameter plus dbeTwoParameter without the method finishing waiting for an underlying response message.
        /// </summary>
        /// <param name="dbeOneParameter"></param>
        /// <param name="dbeTwoParameter"></param>
        /// <param name="strSymbol"></param>
        [OperationContract(IsOneWay = true)]
        void SumByInputValueOneway(double dbeOneParameter, double dbeTwoParameter, string strSymbol);
    }

    public interface IServiceCalulateCallBack
    {
        /// <summary>
        /// The value will be displayed according to the format value chosen when the method finishes waiting for an underlying response message.
        /// </summary>
        /// <param name="strSymbol"></param>
        /// <param name="dbeSumValue"></param>
        [OperationContract]
        void DisplayResultByOption(string strSymbol, double dbeSumValue);
        
         /// <summary>
         /// The value will be displayed according to the format value chosen without the method finishing waiting for an underlying response message. 
         /// </summary>
         /// <param name="strSymbol"></param>
         /// <param name="dbeSumValue"></param>
        [OperationContract(IsOneWay=true)]
        void DisPlayResultByOptionOneWay(string strSymbol, double dbeSumValue);
    }
}
