using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using EnvDTE;
using EnvDTE80;

namespace ProjectLauncher
{
    public partial class LaunchForm : Form
    {
        DTE2 _dte;
        string _filePath;
        public LaunchForm(DTE2 dte)
        {
            _dte = dte;
            InitializeComponent();
        }

        public string FilePath
        {
            get { return this._filePath; }
        }

        private void btnLaunch_Click(object sender, EventArgs e)
        {
            // for the type of projects the sample is interested in, the project output will be the primary output of the
            // active output group
            int selectedIndex = this.cmbProjects.SelectedIndex + 1;
            Project selectedProject = _dte.Solution.Projects.Item(selectedIndex);

            object[] objFileNames = selectedProject.ConfigurationManager.ActiveConfiguration.OutputGroups.Item(1).FileURLs as object[];

            string fileName = objFileNames[0] as string;
            System.Uri fileUri = new System.Uri(fileName);

            _filePath = fileUri.LocalPath;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void LaunchForm_Load(object sender, EventArgs e)
        {
            if (_dte.Solution.Projects.Count == 0)
            {
                // If the misc files project is the only project, display an error.
                MessageBox.Show("A project must be opened");
                this.Close();
                return;
            }

            // Add all the projects except the misc files project to the combobox.
            for (int i = 1; i <= _dte.Solution.Projects.Count; i++)
            {
                Project proj = _dte.Solution.Projects.Item(i);
                this.cmbProjects.Items.Add(proj.Name);
            }

            this.cmbProjects.SelectedIndex = 0;
        }
    }
}
