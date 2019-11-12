/****************************** Module Header ******************************\
 * Module Name:  CalculatorService.cs
 * Project:      CustomMessageHeaderService
 * Copyright (c) Microsoft Corporation.
 * 
 *  This class is the Service contract realization of a simple Calculator.
 *  It also deals with the UserInfo object instance from IncomingMessageHeaders.
 *
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using CustomMessageHeader;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace CustomMessageHeaderService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CalculatorService" in both code and config file together.
    public class CalculatorService : ICalculatorService
    {
        // This instance is used to store the received SOAP Header information.
        UserInfo _incomingUserInfo;

        // It's used to store the received HTTP Header information.
        string strIncomingHTTPHeader;
        public CalculatorService()
        {
            int intHeaderIndex = OperationContext.Current.IncomingMessageHeaders.FindHeader("UserInfo", "http://tempuri.org");
            if (intHeaderIndex != -1)
            {
                _incomingUserInfo = OperationContext.Current.IncomingMessageHeaders.GetHeader<UserInfo>(intHeaderIndex);
            }
            strIncomingHTTPHeader = ((HttpRequestMessageProperty)OperationContext.Current.IncomingMessageProperties["httpRequest"]).Headers["MyHttpHeader"];
            
        }
        
        /// <summary>
        /// Add the two number and output the IncomingMessageHeaders infomation.
        /// </summary>
        /// <param name="n1">number 1</param>
        /// <param name="n2">number 2</param>
        /// <returns></returns>
        public double Add(double n1, double n2)
        {
            if(_incomingUserInfo != null) 
            {
                Console.WriteLine("Incoming User Info: FirstName:{0}, LastName:{1}, Age:{2}", _incomingUserInfo.FirstName, _incomingUserInfo.LastName, _incomingUserInfo.Age);
            } 
            else 
            {
                Console.WriteLine("There was no incoming user information");
            }
            
            Console.WriteLine("Incoming HTTP Header Value: {0}", (strIncomingHTTPHeader!=null) ? strIncomingHTTPHeader : "null");
            return n1 + n2;
        }

        public double Subtract(double n1, double n2)
        {
            return n1 - n2;
        }

        public double Multiply(double n1, double n2)
        {
            return n1 * n2;
        }

        public double Divide(double n1, double n2)
        {
            return n1 / n2;
        }
    }
}
