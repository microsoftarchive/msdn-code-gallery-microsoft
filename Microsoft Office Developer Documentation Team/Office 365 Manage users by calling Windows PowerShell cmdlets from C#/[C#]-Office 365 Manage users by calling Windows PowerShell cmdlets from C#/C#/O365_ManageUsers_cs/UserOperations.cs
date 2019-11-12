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

namespace O365_ManageUsers
{
    public partial class UserOperations : Form
    {
        private UserOpertaionType opType;
        private string userPrincipalName;

        public enum UserOpertaionType
        {
            Save = 1,
            Update = 2
        };

        // Initialize parameter values and update display text according to operation.
        public UserOperations(UserOpertaionType opType, string userPrincipalName = "")
        {
            this.opType = opType;
            this.userPrincipalName = userPrincipalName;
            InitializeComponent();

            if (opType == UserOpertaionType.Save)
            {
                this.Text = SaveUser.Text = "Add User";
                Header.Text = "Please enter below details to create new user";
            }
            else if (opType == UserOpertaionType.Update)
            {
                this.Text = SaveUser.Text = "Update User";
                Header.Text = "Update below user details";
                UserPrincipalName.Enabled = false;

                // Fill user details on controls.
                FillUserDetails();
            }
        }

        private void SaveUser_Click(object sender, EventArgs e)
        {
            try
            {
                // Check for required parameters.
                if (CheckRequiredFields())
                {
                    // Initialize user object and fill with values.
                    Cursor.Current = Cursors.WaitCursor;
                    O365User userEntry = new O365User();
                    userEntry.DisplayName = DisplayName.Text;
                    userEntry.FirstName = FirstName.Text;
                    userEntry.LastName = LastName.Text;
                    userEntry.UserPrincipalName = UserPrincipalName.Text;
                    userEntry.Department = Department.Text;
                    userEntry.Country = Country.Text;
                    userEntry.UsageLocation = UsageLocation.Text;

                    // Choose appropriate operation.
                    UserOperation(userEntry);
                    Cursor.Current = Cursors.Default;
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Validate required parameters.
        /// </summary>
        /// <returns></returns>
        private bool CheckRequiredFields()
        {
            string validationMessage = "";
            if (UserPrincipalName.Text.Trim() == "")
            {
                validationMessage += "\r\n UserPrincipalName";
            }
            if (DisplayName.Text.Trim() == "")
            {
                validationMessage += "\r\n DisplayName";
            }
            if (UsageLocation.Text.Trim() == "")
            {
                validationMessage += "\r\n UsageLocation";
            }
            if (validationMessage != "")
            {
                validationMessage = "You must provide required field(s) : " + validationMessage;
                MessageBox.Show(validationMessage,"Required Parameters");
                this.DialogResult = DialogResult.None;
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Fill user details by powershell command Get-MsolUser.
        /// </summary>
        private void FillUserDetails()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                // Create Initial Session State for runspace.
                InitialSessionState initialSession = InitialSessionState.CreateDefault();
                initialSession.ImportPSModule(new[] { "MSOnline" });
                // Create credential object.
                PSCredential credential = new PSCredential(UserCredential.UserName, UserCredential.Password);
                // Create command to connect office 365.
                Command connectCommand = new Command("Connect-MsolService");
                connectCommand.Parameters.Add((new CommandParameter("Credential", credential)));
                // Create command to get office 365 user.
                Command getUserCommand = new Command("Get-MsolUser");
                getUserCommand.Parameters.Add((new CommandParameter("UserPrincipalName", userPrincipalName)));
                using (Runspace psRunSpace = RunspaceFactory.CreateRunspace(initialSession))
                {
                    // Open runspace.
                    psRunSpace.Open();
                    //Iterate through each command and executes it.
                    foreach (var com in new Command[] { connectCommand, getUserCommand })
                    {
                        var pipe = psRunSpace.CreatePipeline();
                        pipe.Commands.Add(com);
                        // Execute command and generate results and errors (if any).
                        Collection<PSObject> results = pipe.Invoke();
                        var error = pipe.Error.ReadToEnd();

                        if (error.Count > 0 && com == getUserCommand)
                        {
                            MessageBox.Show("Unable to get user with UserPrincipalName : " + userPrincipalName);
                            this.DialogResult = DialogResult.Abort;
                            return;
                        }
                        else if (results.Count > 0 && com == getUserCommand)
                        {

                            // Fill user details on the form controls.
                            foreach (PSObject itemUser in results)
                            {
                                if (itemUser.Properties["DisplayName"] != null && itemUser.Properties["DisplayName"].Value != null)
                                    DisplayName.Text = itemUser.Properties["DisplayName"].Value.ToString();
                                if (itemUser.Properties["FirstName"] != null && itemUser.Properties["FirstName"].Value != null)
                                    FirstName.Text = itemUser.Properties["FirstName"].Value.ToString();
                                if (itemUser.Properties["LastName"] != null && itemUser.Properties["LastName"].Value != null)
                                    LastName.Text = itemUser.Properties["LastName"].Value.ToString();
                                if (itemUser.Properties["UserPrincipalName"] != null && itemUser.Properties["UserPrincipalName"].Value != null)
                                    UserPrincipalName.Text = itemUser.Properties["UserPrincipalName"].Value.ToString();
                                if (itemUser.Properties["Department"] != null && itemUser.Properties["Department"].Value != null)
                                    Department.Text = itemUser.Properties["Department"].Value.ToString();
                                if (itemUser.Properties["Country"] != null && itemUser.Properties["Country"].Value != null)
                                    Country.Text = itemUser.Properties["Country"].Value.ToString();
                                if (itemUser.Properties["UsageLocation"] != null && itemUser.Properties["UsageLocation"].Value != null)
                                    UsageLocation.Text = itemUser.Properties["UsageLocation"].Value.ToString();
                            }
                        }
                    }
                    // Close the runspace.
                    psRunSpace.Close();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Unable to get user with UserPrincipalName : " + userPrincipalName);
                this.DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// Perform add/update user operation using command New-MsolUser / Set-MsolUser.
        /// </summary>
        /// <param name="userEntry"></param>
        private void UserOperation(O365User userEntry)
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
                Command userCommand = null;
                if (opType == UserOpertaionType.Save)
                {
                    userCommand = new Command("New-MsolUser");
                }
                else if (opType == UserOpertaionType.Update)
                {
                    userCommand = new Command("Set-MsolUser");
                }
                userCommand.Parameters.Add((new CommandParameter("UserPrincipalName", userEntry.UserPrincipalName)));
                userCommand.Parameters.Add((new CommandParameter("DisplayName", userEntry.DisplayName)));
                userCommand.Parameters.Add((new CommandParameter("FirstName", userEntry.FirstName)));
                userCommand.Parameters.Add((new CommandParameter("LastName", userEntry.LastName)));
                userCommand.Parameters.Add((new CommandParameter("Country", userEntry.Country)));
                userCommand.Parameters.Add((new CommandParameter("Department", userEntry.Department)));
                userCommand.Parameters.Add((new CommandParameter("UsageLocation", userEntry.UsageLocation)));
                using (Runspace psRunSpace = RunspaceFactory.CreateRunspace(initialSession))
                {
                    // Open runspace.
                    psRunSpace.Open();

                    //Iterate through each command and executes it.
                    foreach (var com in new Command[] { connectCommand, userCommand })
                    {
                        var pipe = psRunSpace.CreatePipeline();
                        pipe.Commands.Add(com);
                        // Execute command and generate results and errors (if any).
                        Collection<PSObject> results = pipe.Invoke();
                        var error = pipe.Error.ReadToEnd();
                        if (error.Count > 0 && com == userCommand)
                        {
                            MessageBox.Show(error[0].ToString());
                            this.DialogResult = DialogResult.None;
                            return;
                        }
                        else if (results.Count >= 0 && com == userCommand)
                        {
                            // If user added successfully then display some information of newly added user.
                            if (opType == UserOpertaionType.Save)
                            {
                                MessageBox.Show("UserPrincipalName : " + results[0].Properties["UserPrincipalName"].Value.ToString()+"\r\n"+
                                "Default Password : " + results[0].Properties["Password"].Value.ToString(), 
                                "User added successfully.");
                            }
                            else if (opType == UserOpertaionType.Update)
                            {
                                MessageBox.Show("User details updated successfully.");
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

    // This class is designed to define user object values.
    public class O365User
    {
        private string firstName = "";
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        private string lastName = "";
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        private string displayName = "";
        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        private string userPrincipalName = "";
        public string UserPrincipalName
        {
            get { return userPrincipalName; }
            set { userPrincipalName = value; }
        }

        private string department = "";
        public string Department
        {
            get { return department; }
            set { department = value; }
        }

        private string country = "";
        public string Country
        {
            get { return country; }
            set { country = value; }
        }

        private string usageLocation = "";
        public string UsageLocation
        {
            get { return usageLocation; }
            set { usageLocation = value; }
        }
    }
}
