using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.ObjectModel;
using System.Security;

namespace O365_ManageLicenses
{
    public partial class ManageLicense : Form
    {

        public ManageLicense()
        {
            InitializeComponent();
        }

        // Form load event method.
        private void ManageLicense_Load(object sender, EventArgs e)
        {
            try
            {
                // Open Login box to validate user.
                LoginUser loginUser = new LoginUser();

                if (loginUser.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;

                    // Getting O365 license list within the domain.
                    Collection<PSObject> results = ExcutePowershellCommands();

                    if (results != null && results.Count > 0)
                    {
                        FillLicenseList(results);
                    }
                    Cursor.Current = Cursors.Default;
                }
                else
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Fill O365 license list data into grid view.
        /// </summary>
        /// <param name="results"></param>
        private void FillLicenseList(Collection<PSObject> results)
        {
            try
            {
                DataTable licenseList = CreateLicenseTable();

                // Iterate through collection of O365 license objects.
                foreach (PSObject itemLicense in results)
                {
                    DataRow licenseRowData = licenseList.NewRow();
                    if (itemLicense.Properties["SkuId"] != null && itemLicense.Properties["SkuId"].Value != null)
                        licenseRowData["SkuId"] = itemLicense.Properties["SkuId"].Value.ToString();
                    if (itemLicense.Properties["AccountSkuId"] != null && itemLicense.Properties["AccountSkuId"].Value != null)
                        licenseRowData["AccountSkuId"] = itemLicense.Properties["AccountSkuId"].Value.ToString();
                    licenseList.Rows.Add(licenseRowData);
                }

                // Bind data list to grid view.
                UserLicenseList.DataSource = licenseList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Assign a license to O365 user.
        private void AssignLicense_Click(object sender, EventArgs e)
        {
            try
            {
                // Opens 'Manage license' page as a model dialog.
                LicenseOperations licenseOp = new LicenseOperations(LicenseOperations.LicenseOpertaionType.Assign);
                licenseOp.ShowDialog();
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
        }

        //Remove a license to O365 user.
        private void RemoveLicense_Click(object sender, EventArgs e)
        {
            try
            {
                // Opens 'Manage license' page as a model dialog.
                LicenseOperations licenseOp = new LicenseOperations(LicenseOperations.LicenseOpertaionType.Remove);
                licenseOp.ShowDialog();
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Execute Get-MsolAccountSku command and returns resulted data.
        /// </summary>
        /// <returns></returns>
        private Collection<PSObject> ExcutePowershellCommands()
        {
            try
            {
                Collection<PSObject> licenseList = null;
                // Create Initial Session State for runspace.
                InitialSessionState initialSession = InitialSessionState.CreateDefault();
                initialSession.ImportPSModule(new[] { "MSOnline" });
                // Create credential object.
                PSCredential credential = new PSCredential(UserCredential.UserName, UserCredential.Password);
                // Create command to connect office 365.
                Command connectCommand = new Command("Connect-MsolService");
                connectCommand.Parameters.Add((new CommandParameter("Credential", credential)));
                // Create command to get office 365 users.
                Command getLicenseCommand = new Command("Get-MsolAccountSku");
                using (Runspace psRunSpace = RunspaceFactory.CreateRunspace(initialSession))
                {
                    // Open runspace.
                    psRunSpace.Open();

                    //Iterate through each command and executes it.
                    foreach (var com in new Command[] { connectCommand, getLicenseCommand })
                    {
                        var pipe = psRunSpace.CreatePipeline();
                        pipe.Commands.Add(com);
                        // Execute command and generate results and errors (if any).
                        Collection<PSObject> results = pipe.Invoke();
                        var error = pipe.Error.ReadToEnd();
                        if (error.Count > 0 && com == connectCommand)
                        {
                            MessageBox.Show(error[0].ToString(), "Problem in login");
                            this.Close();
                            return null;
                        }
                        if (error.Count > 0 && com == getLicenseCommand)
                        {
                            MessageBox.Show(error[0].ToString(), "Problem in getting licenses");
                            this.Close();
                            return null;
                        }
                        else
                        {
                            licenseList = results;
                        }
                    }
                    // Close the runspace.
                    psRunSpace.Close();
                }
                return licenseList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Create temp table to store O365 license data returned from powershell command.
        private DataTable CreateLicenseTable()
        {
            DataTable licenseList = new DataTable();
            licenseList.Columns.Add("SkuId");
            licenseList.Columns.Add("AccountSkuId");
            return licenseList;
        }
    }
}
