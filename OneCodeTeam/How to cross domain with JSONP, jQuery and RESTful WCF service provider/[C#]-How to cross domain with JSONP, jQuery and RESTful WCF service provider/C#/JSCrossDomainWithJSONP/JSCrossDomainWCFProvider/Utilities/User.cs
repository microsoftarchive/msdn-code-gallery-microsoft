/****************************** Module Header ******************************\
Module Name:  User.cs
Project:      JSCrossDomainWCFProvider
Copyright (c) Microsoft Corporation.
	 
Utility class to provide data for UserService.svc
	 
This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx.
All other rights reserved.
	 
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace JSCrossDomainWCFProvider.Utilities
{
    /// <summary>
    /// Utilities User class
    /// </summary>
    [DataContract(Namespace = "http://rest-server/datacontract/user")]
    internal class User
    {
        #region Fields

        private static User userObj;
        private List<User> lUser;

        #endregion

        #region Properties

        /// <summary>
        /// Id property
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Name property
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Age property
        /// </summary>
        [DataMember]
        public int Age { get; set; }

        /// <summary>
        /// Sex property
        /// </summary>
        [DataMember]
        public Sex Sex { get; set; }

        /// <summary>
        /// Comments property
        /// </summary>
        [DataMember]
        public string Comments { get; set; }

        /// <summary>
        /// User object property
        /// </summary>
        public static User UserObject
        {
            get
            {
                if (userObj == null)
                {
                    userObj = new User();
                }

                return userObj;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Fill data and get all users
        /// </summary>
        /// <returns>Return a user list</returns>
        internal List<User> GetAllUsers()
        {
            if (lUser == null)
            {
                lUser = new List<User>();

                lUser.Add(new User
                {
                    Id = 1,
                    Name = "Jason",
                    Age = 25,
                    Sex = Sex.Male,
                    Comments = "Jason is a boy!"
                });

                lUser.Add(new User
                {
                    Id = 2,
                    Name = "Susan",
                    Age = 25,
                    Sex = Sex.Female,
                    Comments = "Susan is a girl!"
                });

                lUser.Add(new User
                {
                    Id = 3,
                    Name = "Nancy",
                    Age = 18,
                    Sex = Sex.Female,
                    Comments = "Nancy is a girl!"
                });
            }

            return lUser;
        }

        #endregion
    }

    /// <summary>
    /// Utilities Sex enum
    /// </summary>
    internal enum Sex
    {
        Male = 0,
        Female = 1
    }
}