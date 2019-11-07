/****************************** Module Header ******************************\
Module Name:  UserService.svc.cs
Project:      JSCrossDomainWCFProvider
Copyright (c) Microsoft Corporation.
	 
WCF Service class to provide operations
	 
This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx.
All other rights reserved.
	 
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Collections.Generic;
using JSCrossDomainWCFProvider.Utilities;

namespace JSCrossDomainWCFProvider
{
    /// <summary>
    /// WCF Service calss to provide operations
    /// </summary>
    internal class UserService : IUserService
    {
        #region Methods

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>Return user list</returns>
        public List<User> GetAllUsers()
        {
            return User.UserObject.GetAllUsers();
        }

        #endregion
    }
}
