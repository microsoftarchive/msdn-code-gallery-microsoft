/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Samples.VisualStudio.CodeSweep.Scanner;
using System.IO;
using Microsoft.Build.Framework;
using System.ComponentModel;
using Microsoft.Samples.VisualStudio.CodeSweep.BuildTask.Properties;
using System.Globalization;

namespace Microsoft.Samples.VisualStudio.CodeSweep.BuildTask
{
    /// <summary>
    /// MSBuild task which performs a CodeSweep scan across items in a project.
    /// </summary>
    [Description("CodeSweepTaskEntry")]
    public class ScannerTask : Task
    {
        List<string> _duplicateTerms = new List<string>();

        /// <summary>
        /// Performs a scan over all files in the project.
        /// </summary>
        /// <returns>False if any violations are found, true otherwise.</returns>
        /// <remarks>
        /// If any violations are found, a message will be sent to standard output.  If this task
        /// is running the VS IDE with the CodeSweep package loaded, the message will also be
        /// placed in the task list.
        /// </remarks>
        public override bool Execute()
        {
            _ignoreInstances = new List<IIgnoreInstance>(Factory.DeserializeIgnoreInstances(_ignoreInstancesString, Path.GetDirectoryName(_project)));

            IMultiFileScanResult result = Scanner.Factory.GetScanner().Scan(GetFilesToScan(), GetTermTables(), OutputScanResults);

            if (_signalErrorIfTermsFound)
            {
                return result.PassedScan == result.Attempted;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the list of term table files, expressed as paths relative to the project
        /// folder, delimited by semicolons.
        /// </summary>
        [Required]
        public string TermTables
        {
            set { _termTables = (value != null) ? value.Split(';') : null; }
            get { return (_termTables != null) ? Utilities.Concatenate(_termTables, ";") : null; }
        }

        /// <summary>
        /// Gets or sets the list of files to scan.
        /// </summary>
        [Required]
        public ITaskItem[] FilesToScan
        {
            set { _filesToScan = value; }
            get { return _filesToScan; }
        }

        /// <summary>
        /// Gets or sets the full path of the project file being built.
        /// </summary>
        [Required]
        public string Project
        {
            get { return _project; }
            set { _project = value; }
        }

        string _ignoreInstancesString;

        /// <summary>
        /// Gets or sets the list of "ignore instances".
        /// </summary>
        public string IgnoreInstances
        {
            get { return _ignoreInstancesString; }
            set { _ignoreInstancesString = value; }
        }

        bool _signalErrorIfTermsFound = false;

        /// <summary>
        /// Controls whether the task will indicate an error when it finds a search term.
        /// </summary>
        public bool SignalErrorIfTermsFound
        {
            get { return _signalErrorIfTermsFound; }
            set { _signalErrorIfTermsFound = value; }
        }

        #region Private Members

        string[] _termTables;
        ITaskItem[] _filesToScan;
        string _project;
        List<IIgnoreInstance> _ignoreInstances;

        private void OutputScanResults(IScanResult result)
        {
            if (result.Scanned)
            {
                if (!result.Passed)
                {
                    foreach (IScanHit hit in result.Results)
                    {
                        IIgnoreInstance thisIgnoreInstance = Factory.GetIgnoreInstance(result.FilePath, hit.LineText, hit.Term.Text, hit.Column);

                        if (!_ignoreInstances.Contains(thisIgnoreInstance))
                        {
                            if (hit.Warning != null)
                            {
                                if (null == _duplicateTerms.Find(
                                    delegate(string item)
                                    {
                                        return String.Compare(item, hit.Term.Text, StringComparison.OrdinalIgnoreCase) == 0;
                                    }))
                                {
                                    Log.LogWarning(hit.Warning);
                                    _duplicateTerms.Add(hit.Term.Text);
                                }
                            }

                            string outputText;

                            if (hit.Term.RecommendedTerm != null && hit.Term.RecommendedTerm.Length > 0)
                            {
                                outputText = String.Format(CultureInfo.CurrentUICulture, Resources.HitFormatWithReplacement, result.FilePath, hit.Line + 1, hit.Column + 1, hit.Term.Text, hit.Term.Severity, hit.Term.Class, hit.Term.Comment, hit.Term.RecommendedTerm);
                            }
                            else
                            {
                                outputText = String.Format(CultureInfo.CurrentUICulture, Resources.HitFormat, result.FilePath, hit.Line + 1, hit.Column + 1, hit.Term.Text, hit.Term.Severity, hit.Term.Class, hit.Term.Comment);
                            }

                            if (HostObject != null && HostObject is IScannerHost)
                            {
                                // We're piping the results to the task list, so we don't want to use
                                // LogWarning, which would create an entry in the error list.
                                Log.LogMessage(MessageImportance.High, outputText);
                            }
                            else
                            {
                                Log.LogWarning(outputText);
                            }
                        }
                    }
                }
            }
            else
            {
                Log.LogWarning(String.Format(CultureInfo.CurrentUICulture, Resources.FileNotScannedError, result.FilePath));
            }

            if (HostObject != null && HostObject is IScannerHost)
            {
                IScannerHost scannerHost = HostObject as IScannerHost;
                scannerHost.AddResult(result, _project);
            }
        }

        private IEnumerable<ITermTable> GetTermTables()
        {
            List<ITermTable> result = new List<ITermTable>();

            foreach (string file in _termTables)
            {
                try
                {
                    result.Add(Scanner.Factory.GetTermTable(file));
                }
                catch (Exception ex)
                {
                    if (ex is ArgumentException || ex is System.Xml.XmlException)
                    {
                        Log.LogWarning(string.Format(CultureInfo.CurrentUICulture, Resources.TermTableLoadFailed, file));
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return result;
        }

        private IEnumerable<string> GetFilesToScan()
        {
            foreach (ITaskItem item in _filesToScan)
            {
                yield return item.ItemSpec;
            }
        }

        #endregion
    }
}
