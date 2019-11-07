/****************************** Module Header ******************************\
 * Module Name: CustomExceptions.cs
 * Project: CSTaskScheduler
 * Copyright (c) Microsoft Corporation. 
 * 
 * Represents an exception when task path is not valid.
 * 
 * This source is subject to the Microsoft Public License. 
 * See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL 
 * All other rights reserved. 
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\****************************************************************************/

namespace CSTaskScheduler.Exceptions
{
    using System;

    /// <summary>
    /// Represents an exception when task path is not valid
    /// </summary>
    [Serializable]
    public class InvalidTaskPathException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the InvalidTaskPathException class
        /// </summary>
        public InvalidTaskPathException() : base(Constants.InvalidTaskPathError) 
        {
        }

        /// <summary>
        /// Initializes a new instance of the InvalidTaskPathException class
        /// </summary>
        /// <param name="message">Custom Message</param>
        public InvalidTaskPathException(string message) : base(message) 
        {
        }

        /// <summary>
        /// Initializes a new instance of the InvalidTaskPathException class
        /// </summary>
        /// <param name="message">Custom Message</param>
        /// <param name="innerException">Any Exception details</param>
        public InvalidTaskPathException(string message, Exception innerException) : base(message, innerException) 
        { 
        }
    }

    /// <summary>
    /// Represents an exception when task name is not valid
    /// </summary>
    [Serializable]
    public class InvalidTaskNameException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the InvalidTaskNameException class
        /// </summary>
        public InvalidTaskNameException() : base(Constants.InvalidTaskNameError) 
        {
        }

        /// <summary>
        /// Initializes a new instance of the InvalidTaskNameException class
        /// </summary>
        /// <param name="message">Custom Message</param>
        public InvalidTaskNameException(string message) : base(message) 
        {
        }

        /// <summary>
        /// Initializes a new instance of the InvalidTaskNameException class
        /// </summary>
        /// <param name="message">Custom Message</param>
        /// <param name="innerException">Any Exception details</param>
        public InvalidTaskNameException(string message, Exception innerException) : base(message, innerException) 
        { 
        }
    }

    /// <summary>
    /// Represents an exception when new task to be created, doesn't have any action configured
    /// </summary>
    [Serializable]
    public class NoActionSpeciifedException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the NoActionSpeciifedException class
        /// </summary>
        public NoActionSpeciifedException() : base(Constants.NoActionSpecifiedError) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the NoActionSpeciifedException class
        /// </summary>
        /// <param name="message">Custom Message</param>
        public NoActionSpeciifedException(string message) : base(message) 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the NoActionSpeciifedException class
        /// </summary>
        /// <param name="message">Custom Message</param>
        /// <param name="innerException">Any Exception details</param>
        public NoActionSpeciifedException(string message, Exception innerException) : base(message, innerException) 
        {
        }
    }
}
