// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SampleSupport;
using SampleQueries;
using System.IO;
using DataSetSampleQueries;

// See the ReadMe.html for additional information
namespace SampleQueries
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            List<SampleHarness> harnesses = new List<SampleHarness>();

            // LinqSamples:
            LinqSamples linqHarness = new LinqSamples();
            harnesses.Add(linqHarness);

            // Linq To SQL Samples:
            LinqToSqlSamples linqToSqlHarness = new LinqToSqlSamples();
            harnesses.Add(linqToSqlHarness);

            // LinqToXmlSamples:
            LinqToXmlSamples linqToXmlHarness = new LinqToXmlSamples();
            harnesses.Add(linqToXmlHarness);

            // DataSetLinqSamples:
            DataSetLinqSamples dsLinqSamples = new DataSetLinqSamples();
            harnesses.Add(dsLinqSamples);
            
            // XQueryUseCases:
            XQueryUseCases xqueryHarness = new XQueryUseCases();
            harnesses.Add(xqueryHarness);

            if (args.Length >= 1 && args[0] == "/runall") {
                foreach (SampleHarness harness in harnesses)
                {
                    harness.RunAllSamples();
                }
            }
            else {
                Application.EnableVisualStyles();
                
                using (SampleForm form = new SampleForm("LINQ Project Sample Query Explorer", harnesses))
                {
                    form.ShowDialog();
                }
            }
        }
    }
}