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

namespace O365_ManageUsers
{
    public partial class ManageUsers : Form
    {

        public ManageUsers()
        {
            InitializeComponent();
        }

        private void ManageUsers_Load(object sender, EventArgs e)
        {
            try
            {

                // Open Login box to validate user.
                LoginUser loginUser = new LoginUser();

                if (loginUser.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;

                    // Getting user list within the domain.
                    Collection<PSObject> results = ExcutePowershellCommands();

                    if (results != null && results.Count > 0)
                    {
                        FillUserList(results);
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

        private void AddUser_Click(object sender, EventArgs e)
        {
            try
            {

                // Opens 'Add User' page as a model dialog.
                UserOperations userOperations = new UserOperations(UserOperations.UserOpertaionType.Save);

                if (userOperations.ShowDialog() == DialogResult.OK) // If user added successfully.
                {
                    Cursor.Current = Cursors.WaitCursor;
                    // Update user list with newly added user.

                    Collection<PSObject> results = ExcutePowershellCommands();

                    if (results != null && results.Count > 0)
                    {
                        FillUserList(results);
                    }

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
        /// This grid event perform 'Update User' operation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {

                if (e.RowIndex != -1 && e.ColumnIndex == UserList.Columns["colEdit"].Index)
                {
                    string userPrincipalName = UserList[UserList.Columns["colUserPrincipalName"].Index, e.RowIndex].Value.ToString();

                    // Opens 'Update User' page as a model dialog.
                    UserOperations userOperations = new UserOperations(UserOperations.UserOpertaionType.Update, userPrincipalName);

                    if (userOperations.ShowDialog() == DialogResult.OK) // If user updated successfully.
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        // Update user list with updated user details.
                        Collection<PSObject> results = ExcutePowershellCommands();

                        if (results != null && results.Count > 0)
                        {
                            FillUserList(results);
                        }

                        Cursor.Current = Cursors.Default;
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Fill user list data into grid view.
        /// </summary>
        /// <param name="results"></param>
        private void FillUserList(Collection<PSObject> results)
        {
            try
            {
                DataTable userListTemp = CreateUserTable();
                
                //Iterate through each user item.
                foreach (PSObject itemUser in results)
                {
                    DataRow newUser = userListTemp.NewRow();
                    if (itemUser.Properties["DisplayName"] != null && itemUser.Properties["DisplayName"].Value !=null)
                        newUser["DisplayName"] = itemUser.Properties["DisplayName"].Value.ToString();
                    if (itemUser.Properties["FirstName"] != null && itemUser.Properties["FirstName"].Value != null)
                        newUser["FirstName"] = itemUser.Properties["FirstName"].Value.ToString();
                    if (itemUser.Properties["LastName"] != null && itemUser.Properties["LastName"].Value != null)
                        newUser["LastName"] = itemUser.Properties["LastName"].Value.ToString();
                    if (itemUser.Properties["UserPrincipalName"] != null && itemUser.Properties["UserPrincipalName"].Value != null)
                        newUser["UserPrincipalName"] = itemUser.Properties["UserPrincipalName"].Value.ToString();
                    if (itemUser.Properties["Department"] != null && itemUser.Properties["Department"].Value != null)
                        newUser["Department"] = itemUser.Properties["Department"].Value.ToString();
                    if (itemUser.Properties["Country"] != null && itemUser.Properties["Country"].Value != null)
                        newUser["Country"] = itemUser.Properties["Country"].Value.ToString();
                    userListTemp.Rows.Add(newUser);
                }

                // Bind data list to grid view.
                UserList.DataSource = userListTemp;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Execute Get-MsolUser command and returns resulted data.
        /// </summary>
        /// <returns></returns>
        private Collection<PSObject> ExcutePowershellCommands()
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
                Command getUserCommand = new Command("Get-MsolUser");
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
                        if (error.Count > 0 && com == connectCommand)
                        {
                            MessageBox.Show(error[0].ToString(), "Problem in login");
                            this.Close();
                            return null;
                        }
                        if (error.Count > 0 && com == getUserCommand)
                        {
                            MessageBox.Show(error[0].ToString(), "Problem in getting users");
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

        // Create temp table to store user data returned from powershell command.
        private DataTable CreateUserTable()
        {
            DataTable userListTemp = new DataTable();
            userListTemp.Columns.Add("DisplayName");
            userListTemp.Columns.Add("FirstName");
            userListTemp.Columns.Add("LastName");
            userListTemp.Columns.Add("UserPrincipalName");
            userListTemp.Columns.Add("Department");
            userListTemp.Columns.Add("Country");
            return userListTemp;
        }
    }
}
