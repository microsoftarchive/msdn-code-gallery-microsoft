/****************************** Module Header ******************************\
* Module Name:    Article.cs
* Project:        CSASPNETSearchEngine
* Copyright (c) Microsoft Corporation
*
* This class reprensents a record in database.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
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