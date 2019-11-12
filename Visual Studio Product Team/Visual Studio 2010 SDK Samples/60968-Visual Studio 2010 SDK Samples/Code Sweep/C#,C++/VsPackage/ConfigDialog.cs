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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;
using Microsoft.Samples.VisualStudio.CodeSweep.VSPackage.Properties;
using System.Globalization;

namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
{
    public partial class ConfigDialog : Form, IConfigurationDialog
    {
        /// <summary>
        /// Creates a new dialog object.
        /// </summary>
        public ConfigDialog()
        {
            InitializeComponent();

            _termTableListBox.DrawItem += new DrawItemEventHandler(_termTableListBox_DrawItem);
        }

        /// <summary>
        /// Sets the service provider used by this object to get VS services.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown if <c>provider</c> is null.</exception>
        public IServiceProvider ServiceProvider
        {
            set
            {
                _serviceProvider = value;
            }

            get
            {
                return _serviceProvider;
            }
        }

        #region IConfigurationDialog Members

        /// <summary>
        /// Shows the dialog using the configuration of the specified project(s).
        /// </summary>
        /// <param name="projects">The projects whose configuration will be used to populate the dialog, and which will be modified when the user changes the settings in the dialog.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <c>projects</c> is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <c>projects</c> is empty or contains null entries.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if <c>SetServiceProvider</c> has not been called.</exception>
        /// <remarks>
        /// The dialog is opened in a modal state; this method does not return until the dialog is
        /// closed.
        /// If the specified projects do not have identical configurations, a warning dialog will
        /// be displayed and the dialog will not be shown.
        /// When the dialog is opened, a new build task will be created in each project if one does
        /// not already exist.
        /// The UI behavior is as follows:
        ///     The Add button is always enabled.  Clicking it opens an "Open File" common dialog.
        ///     If the dialog is canceled, there is no change to the state.  If a file is opened,
        ///     the file's full path is shown in the list box and is appended to the "TermTables"
        ///     property of each project's scanner task.  If the full path is too wide to be
        ///     shown in the list box, it will be abbreviated by substitution of ellipses.
        /// 
        ///     The Remove button is enabled if and only if one or more items are selected in the
        ///     list box.  Clicking it deletes the selected items (and removes them from the
        ///     "TermTables" property of each project's scanner task).
        /// 
        ///     The Scan Now button is enabled if and only if the list box contains one
        ///     or more valid term tables.  Pressing it closes the dialog and begins a background
        ///     scan using the BackgroundScanner class.
        /// 
        ///     The checkbox to scan automatically with every build is checked if and only if the
        ///     RunCodeSweepAfterBuild property is set in the project(s).
        /// 
        ///     The Close button closes the dialog, retaining any changes the user has made.  The
        ///     "X" button in the title bar has the same effect.
        /// </remarks>
        public void Invoke(IList<IVsProject> projects)
        {
            if (projects == null)
            {
                throw new ArgumentNullException("projects");
            }

            if (projects.Count == 0)
            {
                throw new ArgumentException("The list cannot be empty.", "projects");
            }

            base.Font = GetVSFont();

            CreateDefaultConfigurationsIfNecessary(projects);

            if (!IdenticallyConfigured(projects))
            {
                ShowInconsistentConfigurationError();
                return;
            }

            _projects = projects;
            RefreshAllControls();
            ShowDialog();
        }

        #endregion

        #region Private Members

        class ListEntry
        {
            readonly string _fullPath;
            readonly bool _isValid;

            public ListEntry(string fullPath, bool isValid)
            {
                _fullPath = fullPath;
                _isValid = isValid;
            }

            public string FullPath
            {
                get { return _fullPath; }
            }

            public bool IsValid
            {
                get { return _isValid; }
            }

            public void DrawAbbreviatedPath(Font font, Color color, Rectangle bounds, Graphics graphics)
            {
                StringFormat format = new StringFormat();
                format.Trimming = StringTrimming.EllipsisPath;

                Brush brush = new SolidBrush(color);
                graphics.DrawString(_fullPath, font, brush, bounds, format);

                brush.Dispose();
                format.Dispose();
            }
        }

        IList<IVsProject> _projects;
        IServiceProvider _serviceProvider;

        private void RefreshEnabledStates()
        {
            IProjectConfigurationStore store = Factory.GetProjectConfigurationStore(_projects[0]);

            _removeButton.Enabled = _termTableListBox.SelectedItems.Count > 0;
            _autoScanCheckBox.Checked = store.RunWithBuild;
            _scanButton.Enabled = ValidTermTableCount > 0;

            if (!AllAreMSBuildProjects(_projects))
            {
                _autoScanCheckBox.Enabled = false;
                _toolTip.SetToolTip(_autoScanCheckBox, Resources.NonMSBuildCheckboxTip);
            }
        }

        private static bool AllAreMSBuildProjects(IList<IVsProject> projects)
        {
            foreach (IVsProject project in projects)
            {
                if (!ProjectUtilities.IsMSBuildProject(project))
                {
                    return false;
                }
            }

            return true;
        }

        private void RefreshAllControls()
        {
            IProjectConfigurationStore store = Factory.GetProjectConfigurationStore(_projects[0]);

            _termTableListBox.Items.Clear();
            foreach (string tableFile in store.TermTableFiles)
            {
                AddTermTableWithAbsolutePath(tableFile);
            }

            RefreshEnabledStates();
        }

        private int ValidTermTableCount
        {
            get
            {
                int count = 0;

                foreach (object fileObj in _termTableListBox.Items)
                {
                    ListEntry entry = (ListEntry)fileObj;

                    if (entry.IsValid)
                    {
                        ++count;
                    }
                }

                return count;
            }
        }

        private void _autoScanCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            foreach (IVsProject project in _projects)
            {
                Factory.GetProjectConfigurationStore(project).RunWithBuild = _autoScanCheckBox.Checked;
            }
        }

        private void _closeButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void _addButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = Resources.AddTableDialogCaption;

            dialog.DefaultExt = ".xml";
            dialog.Multiselect = true;
            dialog.RestoreDirectory = true;

            dialog.Filter = Resources.FileOpenDlgFilters;
            dialog.FilterIndex = 0;

            dialog.FileOk +=
                delegate(object sender2, CancelEventArgs e2)
                {
                    // Try to instantiate each term table file, to validate it.
                    foreach (string file in dialog.FileNames)
                    {
                        try
                        {
                            CodeSweep.Scanner.Factory.GetTermTable(file);
                        }
                        catch
                        {
                            MessageBox.Show(Resources.InvalidTermTableError, Resources.InvalidTermTableCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            e2.Cancel = true;
                            break;
                        }
                    }
                };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in dialog.FileNames)
                {
                    AddTermTableWithAbsolutePath(file);
                }

                RefreshEnabledStates();
            }
        }

        private void AddTermTableWithAbsolutePath(string file)
        {
            foreach (IVsProject project in _projects)
            {
                if (!Factory.GetProjectConfigurationStore(project).TermTableFiles.Contains(file))
                {
                    Factory.GetProjectConfigurationStore(project).TermTableFiles.Add(file);
                }
            }

            bool isValid = true;
            try
            {
                CodeSweep.Scanner.Factory.GetTermTable(file);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is System.Xml.XmlException)
                {
                    isValid = false;
                }
                else
                {
                    throw;
                }
            }

            _termTableListBox.Items.Add(new ListEntry(file, isValid));
        }

        private void _removeButton_Click(object sender, EventArgs e)
        {
            while (_termTableListBox.SelectedItems.Count > 0)
            {
                object item = _termTableListBox.SelectedItems[0];

                foreach (IVsProject project in _projects)
                {
                    foreach (string existingTable in Factory.GetProjectConfigurationStore(project).TermTableFiles)
                    {
                        if (String.Compare(existingTable, ((ListEntry)item).FullPath, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            Factory.GetProjectConfigurationStore(project).TermTableFiles.Remove(existingTable);
                            break;
                        }
                    }
                }

                _termTableListBox.Items.Remove(item);
            }

            RefreshEnabledStates();
        }

        private void _termTableListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _removeButton.Enabled = _termTableListBox.SelectedItems.Count > 0;
        }

        private void _termTableListBox_ControlAdded(object sender, ControlEventArgs e)
        {
            _scanButton.Enabled = ValidTermTableCount > 0;
        }

        private void _termTableListBox_ControlRemoved(object sender, ControlEventArgs e)
        {
            _scanButton.Enabled = ValidTermTableCount > 0;
        }

        private void _scanButton_Click(object sender, EventArgs e)
        {
            Factory.GetBackgroundScanner().StopIfRunning(true);
            Factory.GetBackgroundScanner().Start(_projects);
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void ConfigDialog_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)27)
            {
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        void _termTableListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (_termTableListBox.Items.Count > 0 && e.Index >= 0)
            {
                ListEntry entry = (ListEntry)_termTableListBox.Items[e.Index];

                if (entry != null)
                {
                    Color color;

                    if (entry.IsValid)
                    {
                        color = e.ForeColor;
                    }
                    else
                    {
                        color = Color.FromKnownColor(KnownColor.GrayText);
                    }
                    entry.DrawAbbreviatedPath(e.Font, color, e.Bounds, e.Graphics);
                }
            }
        }

        private static void CreateDefaultConfigurationsIfNecessary(IEnumerable<IVsProject> projects)
        {
            foreach (IVsProject project in projects)
            {
                IProjectConfigurationStore store = Factory.GetProjectConfigurationStore(project);

                if (!store.HasConfiguration)
                {
                    store.CreateDefaultConfiguration();
                }
            }
        }

        private static void ShowInconsistentConfigurationError()
        {
            MessageBox.Show(Resources.InconsistentConfigurationError, Resources.InconsistentConfigurationCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static bool IdenticallyConfigured(IList<IVsProject> projects)
        {
            if (projects == null)
            {
                throw new ArgumentNullException("projects");
            }

            if (projects.Count == 0)
            {
                throw new ArgumentException("The list cannot be empty.", "projects");
            }

            IProjectConfigurationStore firstStore = Factory.GetProjectConfigurationStore(projects[0]);

            for (int i = 1; i < projects.Count; ++i)
            {
                IProjectConfigurationStore thisStore = Factory.GetProjectConfigurationStore(projects[i]);

                if (firstStore.RunWithBuild != thisStore.RunWithBuild ||
                    !Utilities.UnorderedCollectionsAreEqual(firstStore.TermTableFiles, thisStore.TermTableFiles))
                {
                    return false;
                }
            }

            return true;
        }

        Font GetVSFont()
        {
            IUIHostLocale hostLocale = _serviceProvider.GetService(typeof(IUIHostLocale)) as IUIHostLocale;

            if (hostLocale != null)
            {
                UIDLGLOGFONT[] dlgFont = new UIDLGLOGFONT[] { new UIDLGLOGFONT() };
                hostLocale.GetDialogFont(dlgFont);
                return Font.FromLogFont(dlgFont[0]);
            }
            else
            {
                return base.Font;
            }
        }

        #endregion
    }
}