/****************************** Module Header ******************************\
Module Name:  User.cs
Project:      CSRESTfulWCFService
Copyright (c) Microsoft Corporation.
	 
User model class
	 
This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx.
All other rights reserved.
	 
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.ComponentModel.DataAnnotations;

namespace CSRESTfulWCFServiceASPNETClient.Models
{
    /// <summary>
    /// User model class
    /// </summary>
    public class User
    {
        #region Properties

        /// <summary>
        /// Id property
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name property
        /// </summary>
        [Required(ErrorMessage="Name can't be empty")]
        [StringLength(40,ErrorMessage="Must be under 40 characters")]
        public string Name { get; set; }

        /// <summary>
        /// Age property
        /// </summary>
        [Required(ErrorMessage="Age can't be empty")]
        [Range(0,120,ErrorMessage="Age must be between 0 and 120")]
        public int Age { get; set; }

        /// <summary>
        /// Sex property
        /// </summary>
        public Sex Sex { get; set; }

        /// <summary>
        /// Comments property
        /// </summary>
        public string Comments { get; set; }
        #endregion
    }

    /// <summary>
    /// Utilities Sex enum
    /// </summary>
    public enum Sex
    {
        Male = 0,
        Female = 1
    }
}