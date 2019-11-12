/****************************** Module Header ******************************\
* Module Name:  FTPFileSystem.cs
* Project:	    CSFTPUpload
* Copyright (c) Microsoft Corporation.
* 
* The class FTPFileSystem represents a file on the remote FTP server. When run
* the FTP LIST protocol method to get a detailed listing of the files on an 
* FTP server, the server will response many records of information. Each record
* represents a file. Depended on the FTP Directory Listing Style of the server,
* the record is like 
* 1. MSDOS
*    1.1. Directory
*         12-13-10  12:41PM  <DIR>  Folder A
*    1.2. File
*         12-13-10  12:41PM  [Size] File B  
*         
*   NOTE: The date segment is like "12-13-10" instead of "12-13-2010" if Four-digit
*         years is not checked in IIS.
*        
* 2. UNIX
*    2.1. Directory
*         drwxrwxrwx 1 owner group 0 Dec 1 12:00 Folder A
*    2.2. File
*         -rwxrwxrwx 1 owner group [Size] Dec 1 12:00 File B
* 
*    NOTE: The date segment does not contains year.
* 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Text.RegularExpressions;
using System.Text;

namespace CSFTPUpload
{

    public class FTPFileSystem
    {
        /// <summary>
        /// The original record string.
        /// </summary>
        public string OriginalRecordString { get; set; }

        /// <summary>
        /// MSDOS or UNIX.
        /// </summary>
        public FTPDirectoryListingStyle DirectoryListingStyle { get; set; }

        /// <summary>
        /// The server Path.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// The name of this FTPFileSystem instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Specify whether this FTPFileSystem instance is a directory.
        /// </summary>
        public bool IsDirectory { get; set; }

        /// <summary>
        /// The last modified time of this FTPFileSystem instance.
        /// </summary>
        public DateTime ModifiedTime { get; set; }

        /// <summary>
        /// The size of this FTPFileSystem instance if it is not a directory.
        /// </summary>
        public int Size { get; set; }

        private FTPFileSystem() { }

        /// <summary>
        /// Override the method ToString() to display a more friendly message.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0}\t{1}\t\t{2}",
                this.ModifiedTime.ToString("yyyy-MM-dd HH:mm"),
                this.IsDirectory ? "<DIR>" : this.Size.ToString(),
                this.Name);
        }

        /// <summary>
        /// Find out the FTP Directory Listing Style from the recordString.
        /// </summary>
        public static FTPDirectoryListingStyle GetDirectoryListingStyle(string recordString)
        {
            Regex regex = new System.Text.RegularExpressions.Regex(@"^[d-]([r-][w-][x-]){3}$");

            string header = recordString.Substring(0, 10);

            // If the style is UNIX, then the header is like "drwxrwxrwx".
            if (regex.IsMatch(header))
            {
                return FTPDirectoryListingStyle.UNIX;
            }
            else
            {
                return FTPDirectoryListingStyle.MSDOS;
            }
        }

        /// <summary>
        /// Get an FTPFileSystem from the recordString. 
        /// </summary>
        public static FTPFileSystem ParseRecordString(Uri baseUrl, string recordString, FTPDirectoryListingStyle type)
        {
            FTPFileSystem fileSystem = null;

            if (type == FTPDirectoryListingStyle.UNIX)
            {
                fileSystem = ParseUNIXRecordString(recordString);
            }
            else
            {
                fileSystem = ParseMSDOSRecordString(recordString);
            }

            // Add "/" to the url if it is a directory
            fileSystem.Url = new Uri(baseUrl, fileSystem.Name + (fileSystem.IsDirectory ? "/" : string.Empty));

            return fileSystem;
        }

        /// <summary>
        /// The recordString is like
        /// Directory: drwxrwxrwx   1 owner    group               0 Dec 13 11:25 Folder A
        /// File:      -rwxrwxrwx   1 owner    group               1024 Dec 13 11:25 File B
        /// NOTE: The date segment does not contains year.
        /// </summary>
        static FTPFileSystem ParseUNIXRecordString(string recordString)
        {
            FTPFileSystem fileSystem = new FTPFileSystem();

            fileSystem.OriginalRecordString = recordString.Trim();
            fileSystem.DirectoryListingStyle = FTPDirectoryListingStyle.UNIX;

            // The segments is like "drwxrwxrwx", "",  "", "1", "owner", "", "", "", 
            // "group", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", 
            // "0", "Dec", "13", "11:25", "Folder", "A".
            string[] segments = fileSystem.OriginalRecordString.Split(' ');

            int index = 0;

            // The permission segment is like "drwxrwxrwx".
            string permissionsegment = segments[index];

            // If the property start with 'd', then it means a directory.
            fileSystem.IsDirectory = permissionsegment[0] == 'd';

            // Skip the empty segments.
            while (segments[++index] == string.Empty) { }

            // Skip the directories segment.

            // Skip the empty segments.
            while (segments[++index] == string.Empty) { }

            // Skip the owner segment.

            // Skip the empty segments.
            while (segments[++index] == string.Empty) { }

            // Skip the group segment.

            // Skip the empty segments.
            while (segments[++index] == string.Empty) { }

            // If this fileSystem is a file, then the size is larger than 0. 
            fileSystem.Size = int.Parse(segments[index]);

            // Skip the empty segments.
            while (segments[++index] == string.Empty) { }

            // The month segment.
            string monthsegment = segments[index];

            // Skip the empty segments.
            while (segments[++index] == string.Empty) { }

            // The day segment.
            string daysegment = segments[index];

            // Skip the empty segments.
            while (segments[++index] == string.Empty) { }

            // The time segment.
            string timesegment = segments[index];

            fileSystem.ModifiedTime = DateTime.Parse(string.Format("{0} {1} {2} ",
                timesegment, monthsegment, daysegment));

            // Skip the empty segments.
            while (segments[++index] == string.Empty) { }

            // Calculate the index of the file name part in the original string.
            int filenameIndex = 0;

            for (int i = 0; i < index; i++)
            {
                // "" represents ' ' in the original string.
                if (segments[i] == string.Empty)
                {
                    filenameIndex += 1;
                }
                else
                {
                    filenameIndex += segments[i].Length + 1;
                }
            }
            // The file name may include many segments because the name can contain ' '.          
            fileSystem.Name = fileSystem.OriginalRecordString.Substring(filenameIndex).Trim();

            return fileSystem;
        }

        /// <summary>
        /// 12-13-10  12:41PM       <DIR>          Folder A
        /// </summary>
        /// <param name="recordString"></param>
        /// <returns></returns>
        static FTPFileSystem ParseMSDOSRecordString(string recordString)
        {
            FTPFileSystem fileSystem = new FTPFileSystem();

            fileSystem.OriginalRecordString = recordString.Trim();
            fileSystem.DirectoryListingStyle = FTPDirectoryListingStyle.MSDOS;

            // The segments is like "12-13-10",  "", "12:41PM", "", "","", "",
            // "", "", "<DIR>", "", "", "", "", "", "", "", "", "", "Folder", "A".
            string[] segments = fileSystem.OriginalRecordString.Split(' ');

            int index = 0;

            // The date segment is like "12-13-10" instead of "12-13-2010" if Four-digit years
            // is not checked in IIS.
            string dateSegment = segments[index];
            string[] dateSegments = dateSegment.Split(new char[] { '-' },
                StringSplitOptions.RemoveEmptyEntries);

            int month = int.Parse(dateSegments[0]);
            int day = int.Parse(dateSegments[1]);
            int year = int.Parse(dateSegments[2]);

            // If year >=50 and year <100, then  it means the year 19**
            if (year >= 50 && year < 100)
            {
                year += 1900;
            }

            // If year <50, then it means the year 20**
            else if (year < 50)
            {
                year += 2000;
            }

            // Skip the empty segments.
            while (segments[++index] == string.Empty) { }

            // The time segment.
            string timesegment = segments[index];

            fileSystem.ModifiedTime = DateTime.Parse(string.Format("{0}-{1}-{2} {3}",
                year, month, day, timesegment));

            // Skip the empty segments.
            while (segments[++index] == string.Empty) { }

            // The size or directory segment.
            // If this segment is "<DIR>", then it means a directory, else it means the
            // file size.
            string sizeOrDirSegment = segments[index];

            fileSystem.IsDirectory = sizeOrDirSegment.Equals("<DIR>",
                StringComparison.OrdinalIgnoreCase);

            // If this fileSystem is a file, then the size is larger than 0. 
            if (!fileSystem.IsDirectory)
            {
                fileSystem.Size = int.Parse(sizeOrDirSegment);
            }

            // Skip the empty segments.
            while (segments[++index] == string.Empty) { }

            // Calculate the index of the file name part in the original string.
            int filenameIndex = 0;

            for (int i = 0; i < index; i++)
            {
                // "" represents ' ' in the original string.
                if (segments[i] == string.Empty)
                {
                    filenameIndex += 1;
                }
                else
                {
                    filenameIndex += segments[i].Length + 1;
                }
            }
            // The file name may include many segments because the name can contain ' '.          
            fileSystem.Name = fileSystem.OriginalRecordString.Substring(filenameIndex).Trim();

            return fileSystem;
        }
    }
}
