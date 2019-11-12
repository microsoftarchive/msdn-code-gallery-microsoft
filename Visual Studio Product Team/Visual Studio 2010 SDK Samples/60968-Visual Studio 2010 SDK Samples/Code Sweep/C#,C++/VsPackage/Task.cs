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
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Diagnostics;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Runtime.InteropServices;
using Microsoft.Samples.VisualStudio.CodeSweep.VSPackage.Properties;

namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
{
    class Task : IVsTaskItem, IVsTaskItem3
    {
        public enum TaskFields
        {
            Priority,
            PriorityNumber,
            Term,
            Class,
            Replacement,
            Comment,
            File,
            Line,
            Project
        }

        public Task(string term, int severity, string termClass, string comment, string replacement, string filePath, int line, int column, string projectFile, string lineText, TaskProvider provider, IServiceProvider serviceProvider)
        {
            _term = term;
            _severity = severity;
            _class = termClass;
            _comment = ParseLinks(comment);
            _file = filePath;
            _line = line;
            _column = column;
            _provider = provider;
            _replacement = replacement;
            _serviceProvider = serviceProvider;
            _projectFile = projectFile;
            _lineText = lineText;
        }

        public string ProjectFile
        {
            get { return _projectFile; }
        }

        public bool Ignored
        {
            get
            {
                if (_projectFile != null && _projectFile.Length > 0)
                {
                    IVsProject project = ProjectUtilities.GetProjectByFileName(_projectFile);
                    if (project != null)
                    {
                        BuildTask.IIgnoreInstance ignoreMe = BuildTask.Factory.GetIgnoreInstance(_file, _lineText, _term, _column);
                        foreach (BuildTask.IIgnoreInstance instance in Factory.GetProjectConfigurationStore(project).IgnoreInstances)
                        {
                            if (instance.CompareTo(ignoreMe) == 0)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            set
            {
                if (value != Ignored)
                {
                    if (_projectFile != null && _projectFile.Length > 0)
                    {
                        IVsProject project = ProjectUtilities.GetProjectByFileName(_projectFile);
                        if (project != null)
                        {
                            IProjectConfigurationStore store = Factory.GetProjectConfigurationStore(project);
                            BuildTask.IIgnoreInstance ignoreMe = BuildTask.Factory.GetIgnoreInstance(_file, _lineText, _term, _column);

                            if (value)
                            {
                                store.IgnoreInstances.Add(ignoreMe);
                            }
                            else
                            {
                                foreach (BuildTask.IIgnoreInstance instance in store.IgnoreInstances)
                                {
                                    if (instance.CompareTo(ignoreMe) == 0)
                                    {
                                        store.IgnoreInstances.Remove(instance);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #region IVsTaskItem Members

        public int CanDelete(out int pfCanDelete)
        {
            pfCanDelete = 0;
            return VSConstants.S_OK;
        }

        public int Category(VSTASKCATEGORY[] pCat)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int Column(out int piCol)
        {
            piCol = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int Document(out string pbstrMkDocument)
        {
            pbstrMkDocument = "";
            return VSConstants.E_NOTIMPL;
        }

        public int HasHelp(out int pfHasHelp)
        {
            pfHasHelp = 0;
            return VSConstants.S_OK;
        }

        public int ImageListIndex(out int pIndex)
        {
            pIndex = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int IsReadOnly(VSTASKFIELD field, out int pfReadOnly)
        {
            pfReadOnly = 1;
            return VSConstants.S_OK;
        }

        public int Line(out int piLine)
        {
            piLine = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int NavigateTo()
        {
            int hr = VSConstants.S_OK;
            IVsUIShellOpenDocument openDoc = _serviceProvider.GetService(typeof(SVsUIShellOpenDocument)) as IVsUIShellOpenDocument;

            Microsoft.VisualStudio.OLE.Interop.IServiceProvider sp = null;
            IVsUIHierarchy hierarchy = null;
            uint itemID = 0;
            IVsWindowFrame frame = null;
            Guid viewGuid = VSConstants.LOGVIEWID_TextView;

            hr = openDoc.OpenDocumentViaProject(_file, ref viewGuid, out sp, out hierarchy, out itemID, out frame);
            Debug.Assert(hr == VSConstants.S_OK, "OpenDocumentViaProject did not return S_OK.");

            hr = frame.Show();
            Debug.Assert(hr == VSConstants.S_OK, "Show did not return S_OK.");

            IntPtr viewPtr = IntPtr.Zero;
            Guid textLinesGuid = typeof(IVsTextLines).GUID;
            hr = frame.QueryViewInterface(ref textLinesGuid, out viewPtr);
            Debug.Assert(hr == VSConstants.S_OK, "QueryViewInterface did not return S_OK.");

            IVsTextLines textLines = Marshal.GetUniqueObjectForIUnknown(viewPtr) as IVsTextLines;

            IVsTextManager textMgr = _serviceProvider.GetService(typeof(SVsTextManager)) as IVsTextManager;
            IVsTextView textView = null;
            hr = textMgr.GetActiveView(0, textLines, out textView);
            Debug.Assert(hr == VSConstants.S_OK, "QueryViewInterface did not return S_OK.");

            if (textView != null)
            {
                if (_line >= 0)
                {
                    textView.SetCaretPos(_line, Math.Max(_column, 0));
                }
            }

            return VSConstants.S_OK;
        }

        public int NavigateToHelp()
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnDeleteTask()
        {
            return VSConstants.E_NOTIMPL;
        }

        public int OnFilterTask(int fVisible)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int SubcategoryIndex(out int pIndex)
        {
            pIndex = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int get_Checked(out int pfChecked)
        {
            pfChecked = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int get_Priority(VSTASKPRIORITY[] ptpPriority)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int get_Text(out string pbstrName)
        {
            pbstrName = "";
            return VSConstants.E_NOTIMPL;
        }

        public int put_Checked(int fChecked)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int put_Priority(VSTASKPRIORITY tpPriority)
        {
            return VSConstants.E_NOTIMPL;
        }

        public int put_Text(string bstrName)
        {
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region IVsTaskItem3 Members

        public int GetColumnValue(int iField, out uint ptvtType, out uint ptvfFlags, out object pvarValue, out string pbstrAccessibilityName)
        {
            ptvfFlags = 0;
            pbstrAccessibilityName = "";

            switch ((TaskFields)iField)
            {
                case TaskFields.Class:
                    ptvtType = (uint)__VSTASKVALUETYPE.TVT_TEXT;
                    pvarValue = _class;
                    break;
                case TaskFields.Comment:
                    ptvtType = (uint)__VSTASKVALUETYPE.TVT_LINKTEXT;
                    pvarValue = _comment;
                    break;
                case TaskFields.File:
                    ptvtType = (uint)__VSTASKVALUETYPE.TVT_TEXT;
                    ptvfFlags = (uint)__VSTASKVALUEFLAGS.TVF_FILENAME;
                    pvarValue = _file;
                    break;
                case TaskFields.Line:
                    if (_line == -1)
                    {
                        ptvtType = (uint)__VSTASKVALUETYPE.TVT_NULL;
                        pvarValue = null;
                    }
                    else
                    {
                        ptvtType = (uint)__VSTASKVALUETYPE.TVT_BASE10;
                        pvarValue = _line + 1; // Display as one-based coordinate.
                    }
                    break;
                case TaskFields.Priority:
                    ptvfFlags = (uint)__VSTASKVALUEFLAGS.TVF_HORZ_CENTER;
                    ptvtType = (uint)__VSTASKVALUETYPE.TVT_IMAGE;
                    pvarValue = TaskProvider.GetImageIndexForSeverity(_severity);
                    if (_severity <= 1)
                    {
                        pbstrAccessibilityName = Resources.HighPriority;
                    }
                    else if (_severity == 2)
                    {
                        pbstrAccessibilityName = Resources.MediumPriority;
                    }
                    else
                    {
                        pbstrAccessibilityName = Resources.LowPriority;
                    }
                    break;
                case TaskFields.PriorityNumber:
                    ptvtType = (uint)__VSTASKVALUETYPE.TVT_BASE10;
                    pvarValue = _severity;
                    break;
                case TaskFields.Project:
                    ptvtType = (uint)__VSTASKVALUETYPE.TVT_TEXT;
                    if (_projectFile != null && _projectFile.Length > 0)
                    {
                        pvarValue = ProjectUtilities.GetUniqueProjectNameFromFile(_projectFile);
                    }
                    else
                    {
                        pvarValue = "";
                    }
                    break;
                case TaskFields.Replacement:
                    ptvtType = (uint)__VSTASKVALUETYPE.TVT_TEXT;
                    pvarValue = _replacement;
                    break;
                case TaskFields.Term:
                    ptvtType = (uint)__VSTASKVALUETYPE.TVT_TEXT;
                    pvarValue = _term;
                    break;
                default:
                    ptvtType = (uint)__VSTASKVALUETYPE.TVT_NULL;
                    pvarValue = null;
                    return VSConstants.E_INVALIDARG;
            }

            if (Ignored)
            {
                ptvfFlags |= (uint)__VSTASKVALUEFLAGS.TVF_STRIKETHROUGH;
            }

            return VSConstants.S_OK;
        }

        public int GetDefaultEditField(out int piField)
        {
            piField = -1;
            return VSConstants.E_NOTIMPL;
        }

        public int GetEnumCount(int iField, out int pnValues)
        {
            pnValues = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int GetEnumValue(int iField, int iValue, out object pvarValue, out string pbstrAccessibilityName)
        {
            pvarValue = null;
            pbstrAccessibilityName = "";
            return VSConstants.E_NOTIMPL;
        }

        public int GetNavigationStatusText(out string pbstrText)
        {
            pbstrText = _comment;
            return VSConstants.S_OK;
        }

        public int GetSurrogateProviderGuid(out Guid pguidProvider)
        {
            pguidProvider = Guid.Empty;
            return VSConstants.E_NOTIMPL;
        }

        public int GetTaskName(out string pbstrName)
        {
            pbstrName = _term;
            return VSConstants.S_OK;
        }

        public int GetTaskProvider(out IVsTaskProvider3 ppProvider)
        {
            ppProvider = _provider;
            return VSConstants.S_OK;
        }

        public int GetTipText(int iField, out string pbstrTipText)
        {
            pbstrTipText = "";
            return VSConstants.E_NOTIMPL;
        }

        public int IsDirty(out int pfDirty)
        {
            pfDirty = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int OnLinkClicked(int iField, int iLinkIndex)
        {
            TextFinder startFinder =
                delegate(string text, int startIndex)
                {
                    return text.IndexOf("@", startIndex);
                };

            TextFinder endFinder =
                delegate(string text, int startIndex)
                {
                    return text.IndexOf("@", startIndex + 1);
                };

            Span span = FindNthSpan(_comment, iLinkIndex, startFinder, endFinder);

            if (span != null)
            {
                IVsWebBrowsingService browser = _serviceProvider.GetService(typeof(SVsWebBrowsingService)) as IVsWebBrowsingService;
                IVsWindowFrame frame = null;

                int hr = browser.Navigate(_comment.Substring(span.Start + 1, span.Length - 2), 0, out frame);
                Debug.Assert(hr == VSConstants.S_OK, "Navigate did not return S_OK.");
                return VSConstants.S_OK;
            }
            else
            {
                Debug.Assert(false, "Invalid link index sent to OnLinkClicked.");
                return VSConstants.E_INVALIDARG;
            }
        }

        public int SetColumnValue(int iField, ref object pvarValue)
        {
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region Private Members

        readonly string _term;
        readonly int _severity;
        readonly string _class;
        readonly string _comment;
        readonly string _file;
        readonly int _line;
        readonly int _column;
        readonly TaskProvider _provider;
        readonly string _replacement;
        readonly IServiceProvider _serviceProvider;
        readonly string _projectFile;
        readonly string _lineText;

        class Span
        {
            public Span(int start, int length)
            {
                _start = start;
                _length = length;
            }

            readonly int _start;

            public int Start
            {
                get { return _start; }
            }

            readonly int _length;

            public int Length
            {
                get { return _length; }
            }
        }

        delegate int TextFinder(string text, int startIndex);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startFinder"></param>
        /// <param name="endFinder"></param>
        /// <returns></returns>
        /// <remarks>
        /// <c>startFinder(text, startIndex)</c> is used to find the beginning of the next span.
        /// If it returns a positive index, <c>endFinder(text, startFinder(text, startIndex))</c>
        /// is used to find the end of the span.  <c>endFinder</c> should return the index of the
        /// last character in the span (not the first character after the span).
        /// </remarks>
        private static Span FindSpan(string text, int startIndex, TextFinder startFinder, TextFinder endFinder)
        {
            int start = startFinder(text, startIndex);

            if (start >= 0)
            {
                int end = endFinder(text, start);
                return new Span(start, end - start + 1);
            }
            else
            {
                return new Span(-1, -1);
            }
        }

        private static IEnumerable<Span> FindSpans(string text, TextFinder startFinder, TextFinder endFinder)
        {
            int index = 0;

            for (Span span = FindSpan(text, index, startFinder, endFinder); span.Start >= 0; span = FindSpan(text, index, startFinder, endFinder))
            {
                index = span.Start + span.Length;
                yield return span;
            }
        }

        private static Span FindNthSpan(string text, int spanIndex, TextFinder startFinder, TextFinder endFinder)
        {
            foreach (Span span in FindSpans(text, startFinder, endFinder))
            {
                if (spanIndex == 0)
                {
                    return span;
                }
                else
                {
                    --spanIndex;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds http links in the given text and surrounds them with '@' so they will be treated
        /// as links by the task list.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <remarks>
        /// In VS 2005, the task list has a bug which prevents links from being displayed properly
        /// unless they begin somewhere on the first line of a task cell.
        /// </remarks>
        private static string ParseLinks(string text)
        {
            TextFinder startFinder =
                delegate(string text2, int startIndex)
                {
                    return text2.IndexOf("http://", startIndex);
                };

            TextFinder endFinder =
                delegate(string text2, int startIndex)
                {
                    if (startIndex > 0 && text2[startIndex - 1] == '"')
                    {
                        // This is a quoted URL, so look for the end-quote.
                        int endQuote = text2.IndexOf('"', startIndex);
                        if (endQuote >= 0)
                        {
                            return endQuote;
                        }
                        else
                        {
                            return text2.Length - 1;
                        }
                    }
                    else
                    {
                        // It's not a quoted URL, so just end it on the next whitespace character.
                        int nextWhitespace = text2.IndexOfAny(new char[] { ' ', '\t', '\n', '\r' }, startIndex);
                        if (nextWhitespace == -1)
                        {
                            nextWhitespace = text2.Length;
                        }

                        // If the character before the whitespace is punctuation (as in a URL
                        // which ends with a period that isn't actually part of it), don't
                        // include the punctuation character.
                        if (Char.IsPunctuation(text2[nextWhitespace - 1]))
                        {
                            --nextWhitespace;
                        }
                        return nextWhitespace - 1;
                    }
                };

            StringBuilder result = new StringBuilder();
            int previousSpanEnd = 0;

            foreach (Span linkSpan in FindSpans(text, startFinder, endFinder))
            {
                if (linkSpan.Start > previousSpanEnd)
                {
                    result.Append(text.Substring(previousSpanEnd, linkSpan.Start - (previousSpanEnd)));
                }
                result.Append('@');
                result.Append(text.Substring(linkSpan.Start, linkSpan.Length));
                result.Append('@');
                previousSpanEnd = linkSpan.Start + linkSpan.Length;
            }

            if (previousSpanEnd < text.Length)
            {
                result.Append(text.Substring(previousSpanEnd));
            }

            return result.ToString();
        }

        #endregion
    }
}
