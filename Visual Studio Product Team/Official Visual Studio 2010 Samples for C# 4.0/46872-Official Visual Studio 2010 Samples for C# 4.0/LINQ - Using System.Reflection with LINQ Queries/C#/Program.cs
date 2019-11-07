// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Xml.Linq;

// See the ReadMe.html for additional information
namespace Samples
{
    public static class Program
    {
        const string HtmlFile = "System.Xml.Linq.html";
        
        public static void Main()
        {
            // Get Path and Name of assembly to reflect
            XDocument attr = new XDocument();
            Assembly assembly = Assembly.GetAssembly(attr.GetType());
            String AssemblyFile = assembly.CodeBase;

            // reflect over the assembly
            Reflector reflector = new Reflector();
            reflector.Reflect(AssemblyFile);
            
            // generate the HTML document
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(HtmlFile, settings);
            reflector.Transform(writer);
            writer.Close();
       
            // display the HTML document
            FileInfo fileInfo = new FileInfo(HtmlFile);
            if (fileInfo.Exists) Process.Start("iexplore.exe", fileInfo.FullName);
        }
    }
}