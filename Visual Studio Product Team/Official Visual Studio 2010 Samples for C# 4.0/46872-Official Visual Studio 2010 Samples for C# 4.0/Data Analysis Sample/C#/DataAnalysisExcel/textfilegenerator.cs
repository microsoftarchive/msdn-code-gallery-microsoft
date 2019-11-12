// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)

using System;
using System.Text;
using System.Data;
using System.IO;

namespace DataAnalysisExcel
{
    /// <summary>
    /// This class constructs a tab-delimited Unicode text file
    /// containing data from the Sales data table. First, a random named 
    /// folder is created under the current user's temp folder. 
    /// Next, a file named data.txt is created in that folder. Another file,
    /// named schema.ini is created in that folder with settings for PivotTable
    /// creation. This file is discovered automatically by the PivotTable by virtue
    /// of sharing the data file's folder.
    /// </summary>
    internal class TextFileGenerator : IDisposable
    {
        /// <summary>
        /// Full path to the created temp file.
        /// </summary>
        private string fullPath;

        /// <summary>
        /// Full path to the created toplevel folder.
        /// </summary>
        private string rootPath;

        /// <summary>
        /// Field to indicate that all files and directories created by this
        /// object have been deleted, e.g. DeleteCreatedFiles() has been called.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// The full path to the data.txt file.
        /// </summary>
        /// <value>The full path to the data.txt file.</value>
        internal string FullPath
        {
            get
            {
                return fullPath;
            }
        }

        /// <summary>
        /// Constructor. Creates the temporary folder, data.txt and schema.ini files.
        /// </summary>
        /// <param name="dt">The Sales table.</param>
        internal TextFileGenerator(DataTable dt)
        {
            string directoryName;
            string rootName;
            
            GenerateSecureTempFolder(out directoryName, out rootName);

            this.rootPath = rootName;
            this.fullPath = Path.Combine(directoryName, "data.txt");

            Encoding textEncoding;

            textEncoding = Encoding.Unicode;

            System.IO.Directory.CreateDirectory(directoryName);
            using (StreamWriter writer = new StreamWriter(this.fullPath, false, textEncoding, 512))
            {
                int remaining = dt.Columns.Count;

                foreach (DataColumn column in dt.Columns)
                {
                    writer.Write(QuoteString(column.ColumnName));

                    if (--remaining != 0)
                    {
                        writer.Write('\t');
                    }
                }

                writer.Write("\r\n");
                foreach (DataRow row in dt.Rows)
                {
                    int remainingItems = row.ItemArray.Length;

                    foreach (object item in row.ItemArray)
                    {
                        writer.Write(QuoteString(item.ToString()));

                        if (--remainingItems != 0)
                        {
                            writer.Write('\t');
                        }
                    }
                    writer.Write("\r\n");
                }
            }

            CreateSchemaIni();
        }

        ~TextFileGenerator()
        {
            InternalDispose();
        }

        /// <summary>
        /// Creates a schema.ini file for PivotTable configuration.
        /// </summary>
        private void CreateSchemaIni()
        {
            string contentsFormat = @"[{0}]
ColNameHeader=True
Format=TabDelimited
MaxScanRows=0
CharacterSet=Unicode
Col1=Date Char Width 255
Col2=Flavor Char Width 255
Col3=Inventory Integer
Col4=Sold Integer
Col5=Estimated Float
Col6=Recommendation Char Width 255
Col7=Profit Float
";
            string fileName = Path.Combine(Path.GetDirectoryName(this.fullPath), "schema.ini");

            using (StreamWriter writer = new StreamWriter(fileName, false, Encoding.Default, contentsFormat.Length + this.fullPath.Length))
            {
                writer.Write(contentsFormat, Path.GetFileName(this.fullPath));
            }
        }

        /// <summary>
        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            InternalDispose();

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        private void InternalDispose()
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // Call the appropriate methods to clean up 
                // unmanaged resources here.
                // If disposing is false, 
                // only the following code is executed.
                DeleteCreatedFiles();
            }
            disposed = true;
        }

        /// Deletes the created folder and its contents.
        /// </summary>
        private void DeleteCreatedFiles()
        {
            if (this.rootPath != null)
            {
                System.IO.Directory.Delete(rootPath, true);
                this.rootPath = null;
            }
        }

        /// <summary>
        /// Helper method for generating a secure directory structure
        /// file names.
        /// </summary>
        /// <param name="createdFolder">
        /// A random, secure path.
        /// </param>
        /// <param name="createdRoot">
        /// The toplevel path created. 
        /// </param>
        private static void GenerateSecureTempFolder(out string createdFolder, out string createdRoot)
        {
            string directoryName = Path.Combine(Path.GetTempPath(), GenerateRandomName());
            createdRoot = directoryName;
            directoryName = Path.Combine(directoryName, GenerateRandomName());
            createdFolder = directoryName;
        }

        /// <summary>
        /// Helper method for generating random 
        /// file names.
        /// </summary>
        /// <returns>A random, secure directory name.</returns>
        private static string GenerateRandomName()
        {
            byte[] data = new byte[9];
            StringBuilder randomString;

            // Retrieve some random bytes.
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(data);

            // Convert bytes to a string. This will generate an 12 character string.
            randomString = new StringBuilder(System.Convert.ToBase64String(data));

            // Convert to an 8.3 file name format
            randomString[8] = '.';

            // Replace any illegal file name characters.
            randomString = randomString.Replace('/', '-');
            randomString = randomString.Replace('+', '_');

            // Return the string.
            return randomString.ToString();
        }

        /// <summary>
        /// Puts a string inside quotes ("). Any quotes inside the string are doubled.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string QuoteString(string s)
        {
            StringBuilder sb = new StringBuilder("\"", s.Length + 2);

            sb.Append(s.Replace("\"", "\"\""));
            sb.Append('"');
            return sb.ToString();
        }
    }
}
