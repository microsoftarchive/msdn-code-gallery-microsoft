/****************************** Module Header ******************************\
* Module Name:    Article.cs
* Project:        CSASPNETSearchEngine
* Copyright (c) Microsoft Corporation
*
* This class reprensents a record in database.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
\*****************************************************************************/


namespace CSASPNETSearchEngine
{
    /// <summary>
    /// This class reprensents a record in database.
    /// </summary>
    public class Article
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}