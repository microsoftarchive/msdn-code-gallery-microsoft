using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;

namespace MyFileConnector
{
    public class MyFile
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime LastModified { get; set; }
        public Byte[] SecurityDescriptor { get; set; }
        public string Extension { get; set; }
        public String ContentType { get; set; }
    }

    public class MyFolder
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime LastModified { get; set; }
        public Byte[] SecurityDescriptor { get; set; }
    }
    
    public class MyFileProxy : IDisposable
    {
        private string path;

        public void connect(string folderpath)
        {
            this.path = folderpath;
        }

        [Browsable(true)]
        public MyFolder[] GetFolders(string parentFolderPath)
        {
            List<MyFolder> myFolders = new List<MyFolder>();
            foreach (string dirpath in Directory.GetDirectories(parentFolderPath))
            {
                MyFolder myfolder = new MyFolder();
                myfolder.Path = dirpath;
                DirectoryInfo di = new DirectoryInfo(dirpath);
                myfolder.Name = di.Name;
                myfolder.LastModified = di.LastWriteTimeUtc;
                DirectorySecurity sec = di.GetAccessControl();
                myfolder.SecurityDescriptor = new Byte[sec.GetSecurityDescriptorBinaryForm().Length];
                myfolder.SecurityDescriptor = sec.GetSecurityDescriptorBinaryForm();

                myFolders.Add(myfolder);
            }
            return myFolders.ToArray();
        }

        [Browsable(true)]
        public MyFolder GetSpecifiedFolder(string folderpath)
        {
            if (!Directory.Exists(folderpath))
            {
                throw new Microsoft.BusinessData.Runtime.ObjectNotFoundException(String.Format(
                    "No folder exists at the path {0}",
                    folderpath));
            }

            MyFolder myfolder = new MyFolder();
            myfolder.Path = folderpath;
            DirectoryInfo di = new DirectoryInfo(folderpath);
            myfolder.Name = di.Name;
            myfolder.LastModified = di.LastWriteTimeUtc;
            DirectorySecurity sec = di.GetAccessControl();
            myfolder.SecurityDescriptor = new Byte[sec.GetSecurityDescriptorBinaryForm().Length];
            myfolder.SecurityDescriptor = sec.GetSecurityDescriptorBinaryForm();
            return myfolder;
        }

        [Browsable(true)]
        public MyFile[] GetFiles(string folderpath)
        {
            List<MyFile> myfiles = new List<MyFile>();

            foreach (string filepath in Directory.GetFiles(folderpath))
            {
                MyFile myfile = new MyFile();
                myfile.Path = filepath;
                FileInfo fi = new FileInfo(filepath);
                myfile.Extension = fi.Extension.TrimStart(new char[] { '.' });

                if (myfile.Extension.Equals("txt", StringComparison.OrdinalIgnoreCase))
                {
                    myfile.ContentType = "text/plain";
                }
                else
                {
                    myfile.ContentType = "unknown/unknown";
                }

                myfile.LastModified = fi.LastWriteTimeUtc;
                myfile.Name = fi.Name;
                FileSecurity sec = fi.GetAccessControl();
                myfile.SecurityDescriptor = new Byte[sec.GetSecurityDescriptorBinaryForm().Length];
                myfile.SecurityDescriptor = sec.GetSecurityDescriptorBinaryForm();
                myfiles.Add(myfile);
            }

            return myfiles.ToArray();
        }

        [Browsable(true)]
        public MyFile GetSpecifiedFile(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new Microsoft.BusinessData.Runtime.ObjectNotFoundException(String.Format(
                    "No file exists at the path {0}",
                    filepath));
            }

            FileInfo fi = new FileInfo(filepath);
            MyFile myfile = new MyFile();
            myfile.Path = filepath;
            myfile.Extension = fi.Extension.TrimStart(new char[] {'.'});

            if (myfile.Extension.Equals("txt", StringComparison.OrdinalIgnoreCase))
            {
                myfile.ContentType = "text/plain";
            }
            else
            {
                myfile.ContentType = "unknown/unknown";
            }

            myfile.LastModified = fi.LastWriteTimeUtc;
            myfile.Name = fi.Name;
            FileSecurity sec = fi.GetAccessControl();
            myfile.SecurityDescriptor = new Byte[sec.GetSecurityDescriptorBinaryForm().Length];
            myfile.SecurityDescriptor = sec.GetSecurityDescriptorBinaryForm();
            return myfile;
        }

        [Browsable(true)]
        public FileStream GetFileStream(string filepath)
        {
            return new FileStream(filepath, FileMode.Open, FileAccess.Read);
        }

        #region IDisposable Members

        public void Dispose() { }

        #endregion
    }
}
