/****************************** Module Header ******************************\
 * Module Name:  FTPFileDataSource.cs
 * Project:	     CSWindowsStoreAppFTPDownloader
 * Copyright (c) Microsoft Corporation.
 * 
 * The datasource of the UI which supports group. 
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using CSWindowsStoreAppFTPDownloader.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CSWindowsStoreAppFTPDownloader.DataModel
{
    class FTPFileDataSource : BindableBase
    {
        private ObservableCollection<SampleDataGroup> allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this.allGroups; }
        }

        /// <summary>
        /// Group all FTP items.
        /// 1. Directory Group
        /// 2. File Group.
        /// </summary>
        /// <param name="fileList"></param>
        public FTPFileDataSource( IEnumerable<FTP.FTPFileSystem> fileList)
        {
            SampleDataGroup dirGroup = new SampleDataGroup(
                "Directory Group",// + Guid.NewGuid(),
                "Directories",
                "Sub directories in this folder",
                "Assets/Folder.png",
                "all sub directories in this folder");

            SampleDataGroup fileGroup = new SampleDataGroup(
                "File Group",// + Guid.NewGuid(),
                "Files",
                "Files in this folder",
                "Assets/File.png",
                "all files in this folder");

            this.allGroups.Add(dirGroup);
            this.allGroups.Add(fileGroup);


            foreach (var item in fileList)
            {
                if (item.IsDirectory)
                {
                    var subDir = new SampleDataItem(
                        item.Url.ToString(),
                        item.Name,
                        string.Empty,
                        "Assets/Folder.png",
                        string.Format("{0}", item.ModifiedTime),
                        item,
                        dirGroup);

                    subDir.SetImage("ms-appx:///Assets/Folder.png");

                    dirGroup.Items.Add(subDir);
                }
                else
                {
                    var file = new SampleDataItem(
                        item.Url.ToString(),
                        item.Name,
                        string.Format("{0}KB", Math.Ceiling((double)item.Size / 1024)),
                        "Assers/File.png",
                        string.Format("{0}KB", item.ModifiedTime),
                        item,
                        fileGroup);

                    file.SetImage("ms-appx:///Assets/File.png");
                    fileGroup.Items.Add(file);
                }

            }
        }


    }
}
