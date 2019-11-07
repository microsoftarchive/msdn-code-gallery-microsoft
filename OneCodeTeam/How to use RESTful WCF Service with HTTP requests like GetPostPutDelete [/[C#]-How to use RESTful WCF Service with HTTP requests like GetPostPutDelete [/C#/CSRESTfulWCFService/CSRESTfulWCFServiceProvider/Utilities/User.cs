/****************************** Module Header ******************************\
Module Name:  User.cs
Project:      CSRESTfulWCFServiceProvider
Copyright (c) Microsoft Corporation.
	 
Utility class to provide data for UserService.svc
	 
This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx.
All other rights reserved.
	 
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CSRESTfulWCFServiceProvider.Utilities
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

        /// <summary>
        /// User list property
        /// </summary>
        public List<User> LUser
        {
            get
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
        }

        #endregion

        #region Methods

        /// <summary>
        /// Fill data and get all users
        /// </summary>
        /// <returns>Return a user list</returns>
        internal List<User> GetAllUsers()
        {
            return LUser;
        }

        /// <summary>
        /// Create a user
        /// </summary>
        /// <param name="user">User object</param>
        internal void CreateUser(User user)
        {
            LUser.Add(user);
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="user">User object</param>
        internal void UpdateUser(User user)
        {
            var original = LUser.Find(u => u.Id == user.Id);

            if (original == null)
                throw new Exception(string.Format("User {0} does not exist!", user.Name));

            original.Name = user.Name;
            original.Sex = user.Sex;
            original.Age = user.Age;
            original.Comments = user.Comments;
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="id">User Id</param>
        internal void DeleteUser(string id)
        {
            if (!LUser.Exists(u => u.Id.ToString() == id))
                throw new Exception("Special user does not exist!");

            LUser.Remove(LUser.Find(u => u.Id.ToString() == id));
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