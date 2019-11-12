using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace O365_ManageLicenses
{
    public partial class LicenseOperations : Form
    {
        private LicenseOpertaionType opType;
        private string userPrincipalName;

        public enum LicenseOpertaionType
        {
            Assign = 1,
            Remove = 2
        };

        // Initialize parameter values and update display text according to operation.
        public LicenseOperations(LicenseOpertaionType opType, string userPrincipalName = "")
        {
            this.opType = opType;
            this.userPrincipalName = userPrincipalName;

            InitializeComponent();
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                // Fill user dropdown list.
                FillUserList();
                // Fill domain licenses dropdown list.
                FillLicenseList();

                if (opType == LicenseOpertaionType.Assign)
                {
                    this.Text = ManageLicenseOperations.Text = "Assign License";
                    HeaderText.Text = "Please select below details to assign license to user";
                }
                else if (opType == LicenseOpertaionType.Remove)
                {
                    this.Text = ManageLicenseOperations.Text = "Remove License";
                    HeaderText.Text = "Please select below details to remove license of a user";
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
        }

        // Function to manage license.(assign and remove license)
        private void ManageLicenseOperations_Click(object sender, EventArgs e)
        {
            try
            {
                // Initialize license object and fill with values.
                Cursor.Current = Cursors.WaitCursor;
                AssignLicense assignLicense = new AssignLicense();
                assignLicense.LicenseName = SelectLicense.SelectedValue.ToString();
                assignLicense.UserPrincipalName = SelectUser.SelectedValue.ToString();

                // Choose appropriate operation.
                ManageLicense(assignLicense);
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
        }

        // Fill license drop-down using power shell command result object.
        private void FillLicenseList()
        {
            try
            {
                Collection<PSObject> results = ExcutePowershellCommands("Get-MsolAccountSku");

                if (results != null && results.Count > 0)
                {
                    List<string> LicenseList = new List<string>();

                    // Iterate through collection of user objects.
                    foreach (PSObject itemUser in results)
                    {
                        if (itemUser.Properties["AccountSkuId"] != null && itemUser.Properties["AccountSkuId"].Value != null)
                            LicenseList.Add(itemUser.Properties["AccountSkuId"].Value.ToString());
                    }

                    // Bind data list to grid view.
                    SelectLicense.DataSource = LicenseList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Fill user drop-down using powershell command result object.
        private void FillUserList()
        {
            try
            {
                Collection<PSObject> results = ExcutePowershellCommands("Get-MsolUser");

                if (results != null && results.Count > 0)
                {
                    List<string> UserList = new List<string>();

                    // Iterate through collection of user objects.
                    foreach (PSObject itemUser in results)
                    {
                        if (itemUser.Properties["UserPrincipalName"] != null && itemUser.Properties["UserPrincipalName"].Value != null)
                            UserList.Add(itemUser.Properties["UserPrincipalName"].Value.ToString());
                    }

                    // Bind data list to grid view.
                    SelectUser.DataSource = UserList;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Execute powershell command and returns resulted data.
        /// </summary>
        /// <returns></returns>
        public Collection<PSObject> ExcutePowershellCommands(string strCommandText)
        {
            try
            {
                Collection<PSObject> userList = null;
                // Create Initial Session State for runspace.
                InitialSessionState initialSession = InitialSessionState.CreateDefault();
                initialSession.ImportPSModule(new[] { "MSOnline" });
                // Create credential object.
                PSCredential credential = new PSCredential(UserCredential.UserName, UserCredential.Password);
                // Create command to connect office 365.
                Command connectCommand = new Command("Connect-MsolService");
                connectCommand.Parameters.Add((new CommandParameter("Credential", credential)));
                // Create command to get office 365 users.
                Command getCommand = new Command(strCommandText);
                using (Runspace psRunSpace = RunspaceFactory.CreateRunspace(initialSession))
                {
                    // Open runspace.
                    psRunSpace.Open();

                    // Iterate through each command and executes it.
                    foreach (var com in new Command[] { connectCommand, getCommand })
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
                        if (error.Count > 0 && com == getCommand)
                        {
                            MessageBox.Show(error[0].ToString(), "Problem in getting data");
                            this.Close();
                            return null;
                        }
                        else
                        {
                            userList = results;
                        }
                    }
                    // Close the runspace.
                    psRunSpace.Close();
                }
                return userList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Add/Remove license using powershell command.
        private void ManageLicense(AssignLicense assignLicense)
        {
            try
            {
                // Create Initial Session State for runspace.
                InitialSessionState initialSession = InitialSessionState.CreateDefault();
                initialSession.ImportPSModule(new[] { "MSOnline" });
                // Create credential object.
                PSCredential credential = new PSCredential(UserCredential.UserName, UserCredential.Password);
                // Create command to connect office 365.
                Command connectCommand = new Command("Connect-MsolService");
                // Create command to add/update office 365 user.
                string manageLicenseSwitch="";
                if (opType == LicenseOpertaionType.Assign)
                {
                    manageLicenseSwitch = "AddLicenses";
                }
                else if (opType == LicenseOpertaionType.Remove)
                {
                    manageLicenseSwitch = "RemoveLicenses";
                }
                Command licenseCommand = new Command("Set-MsolUserLicense");
                licenseCommand.Parameters.Add((new CommandParameter("UserPrincipalName", assignLicense.UserPrincipalName)));
                licenseCommand.Parameters.Add((new CommandParameter(manageLicenseSwitch, assignLicense.LicenseName)));
                using (Runspace psRunSpace = RunspaceFactory.CreateRunspace(initialSession))
                {
                    // Open runspace.
                    psRunSpace.Open();

                    // Iterate through each command and executes it.
                    foreach (var com in new Command[] { connectCommand, licenseCommand })
                    {
                        var pipe = psRunSpace.CreatePipeline();
                        pipe.Commands.Add(com);
                        // Execute command and generate results and errors (if any).
                        Collection<PSObject> results = pipe.Invoke();
                        var error = pipe.Error.ReadToEnd();
                        if (error.Count > 0 && com == licenseCommand)
                        {
                            MessageBox.Show(error[0].ToString());
                            this.DialogResult = DialogResult.None;
                            return;
                        }
                        else if (results.Count >= 0 && com == licenseCommand)
                        {
                            if (opType == LicenseOpertaionType.Assign)
                            {
                                MessageBox.Show("License assigned successfully.");
                            }
                            else if (opType == LicenseOpertaionType.Remove)
                            {
                                MessageBox.Show("License removed successfully.");
                            }
                            this.DialogResult = DialogResult.OK;
                        }
                    }
                    // Close the runspace.
                    psRunSpace.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class AssignLicense
    {
        private string userPrincipalName = "";
        public string UserPrincipalName
        {
            get { return userPrincipalName; }
            set { userPrincipalName = value; }
        }

        private string licenseName = "";
        public string LicenseName
        {
            get { return licenseName; }
            set { licenseName = value; }
        }
    }
}
