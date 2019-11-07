/****************************** Module Header ******************************\
 * Module Name:  ZipHelper.cs
 * Project:      ZipHelperWinRT
 * Copyright (c) Microsoft Corporation.
 * 
 * This Windows Runtime component uses ZipArchive to unzip the zip file.
 * We wrap the unzip function in WinRT component, so it can be consumed by C++/JS client
 * apps.
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.IO.Compression;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.IO;
using Windows.Foundation;

namespace ZipHelperWinRT
{
    public sealed class ZipHelper
    {
        /// <summary>
        /// Unzips the specified zip file to the specified destination folder.
        /// </summary>
        /// <param name="zipFile">The zip file</param>
        /// <param name="destinationFolder">The destination folder</param>
        /// <returns></returns>
        public static IAsyncAction UnZipFileAsync(StorageFile zipFile, StorageFolder destinationFolder)
        {
            return UnZipFileHelper(zipFile, destinationFolder).AsAsyncAction();
        }
        #region private helper functions
        private static async Task UnZipFileHelper(StorageFile zipFile, StorageFolder destinationFolder)
        {
            if (zipFile == null || destinationFolder == null ||
                !Path.GetExtension(zipFile.Name).Equals(".zip", StringComparison.OrdinalIgnoreCase)
                )
            {
                throw new ArgumentException("Invalid argument...");
            }

            Stream zipMemoryStream = await zipFile.OpenStreamForReadAsync();

            // Create zip archive to access compressed files in memory stream
            using (ZipArchive zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Read))
            {
                // Unzip compressed file iteratively.
                foreach (ZipArchiveEntry entry in zipArchive.Entries)
                {
                    await UnzipZipArchiveEntryAsync(entry, entry.FullName, destinationFolder);
                }
            }
        }

        /// <summary>
        /// It checks if the specified path contains directory.
        /// </summary>
        /// <param name="entryPath">The specified path</param>
        /// <returns></returns>
        private static bool IfPathContainDirectory(string entryPath)
        {
            if (string.IsNullOrEmpty(entryPath))
            {
                return false;
            }

            return entryPath.Contains("/");
        }

        /// <summary>
        /// It checks if the specified folder exists.
        /// </summary>
        /// <param name="storageFolder">The container folder</param>
        /// <param name="subFolderName">The sub folder name</param>
        /// <returns></returns>
        private static async Task<bool> IfFolderExistsAsync(StorageFolder storageFolder, string subFolderName)
        {
            try
            {
                IStorageItem item = await storageFolder.GetItemAsync(subFolderName);
                return (item != null);
            }
            catch
            {
                // Should never get here
                return false;
            }
        }

        /// <summary>
        /// Unzips ZipArchiveEntry asynchronously.
        /// </summary>
        /// <param name="entry">The entry which needs to be unzipped</param>
        /// <param name="filePath">The entry's full name</param>
        /// <param name="unzipFolder">The unzip folder</param>
        /// <returns></returns>
        private static async Task UnzipZipArchiveEntryAsync(ZipArchiveEntry entry, string filePath, StorageFolder unzipFolder)
        {
            if (IfPathContainDirectory(filePath))
            {
                // Create sub folder
                string subFolderName = Path.GetDirectoryName(filePath);

                bool isSubFolderExist = await IfFolderExistsAsync(unzipFolder, subFolderName);

                StorageFolder subFolder;

                if (!isSubFolderExist)
                {
                    // Create the sub folder.
                    subFolder =
                        await unzipFolder.CreateFolderAsync(subFolderName, CreationCollisionOption.ReplaceExisting);
                }
                else
                {
                    // Just get the folder.
                    subFolder =
                        await unzipFolder.GetFolderAsync(subFolderName);
                }

                // All sub folders have been created yet. Just pass the file name to the Unzip function.
                string newFilePath = Path.GetFileName(filePath);

                if (!string.IsNullOrEmpty(newFilePath))
                {
                    // Unzip file iteratively.
                    await UnzipZipArchiveEntryAsync(entry, newFilePath, subFolder);
                }
            }
            else
            {

                // Read uncompressed contents
                using (Stream entryStream = entry.Open())
                {
                    byte[] buffer = new byte[entry.Length];
                    entryStream.Read(buffer, 0, buffer.Length);

                    // Create a file to store the contents
                    StorageFile uncompressedFile = await unzipFolder.CreateFileAsync
                    (entry.Name, CreationCollisionOption.ReplaceExisting);

                    // Store the contents
                    using (IRandomAccessStream uncompressedFileStream =
                    await uncompressedFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        using (Stream outstream = uncompressedFileStream.AsStreamForWrite())
                        {
                            outstream.Write(buffer, 0, buffer.Length);
                            outstream.Flush();
                        }
                    }
                }
            }
        }
        #endregion
    }
}

