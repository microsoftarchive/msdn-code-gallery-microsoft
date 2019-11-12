//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;


using nwind;
[assembly: CLSCompliant(true)]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: ComVisible(false)]

namespace LinqToNorthwind {
    class Program
    {
        public static string dbPath = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\..\Data\NORTHWND.MDF"));
        public static string sqlServerInstance = @".\SQLEXPRESS";
        public static string connString = "AttachDBFileName='" + dbPath + "';Server='" + sqlServerInstance + "';user instance=true;Integrated Security=SSPI; Pooling=false; Connection Timeout=60";

        [STAThread()]
        static void Main()
        {
            // The following assumes that:
            // 1. SQL Server 2005 Express is installed on your machine
            // 2. You install the Data Sample directory that contains Northwind.
            // Or, if you have installed Northwind, you will need to alter the connection string to
            // Northwind db = new Northwind("Server=.\\SQLExpress;Database=Northwind;Trusted_Connection=True");
            // You must edit the path to point to the mdf file on your machine
            // Northwind db = new Northwind("c:\\northwind\\northwnd.mdf");

            Northwind db = new Northwind(connString);
            db.Log = Console.Out;
            Samples.Sample15(db);
            Console.ReadLine();
        }
    }
}
