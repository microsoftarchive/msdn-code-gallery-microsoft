using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Windows.Forms;

namespace O365_SingleSignOn
{
    public partial class DomainsList : Form
    {
        public DomainsList()
        {
            InitializeComponent();
        }

        private void DomainsList_Load(object sender, EventArgs e)
        {
            try
            {

                // Open Login box to validate user.
                LoginUser loginUser = new LoginUser();

                if (loginUser.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;

                    // Getting domains list.
                    Collection<PSObject> results = ExecutePowershellCommands();

                    if (results != null && results.Count > 0)
                    {
                        FillDomainList(results);
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
        /// Fill domain list data into grid view.
        /// </summary>
        /// <param name="results"></param>
        private void FillDomainList(Collection<PSObject> results)
        {
            try
            {
                DataTable domainListTemp = CreateDomainTable();

                // Iterate through collection of domain objects.
                foreach (PSObject itemUser in results)
                {
                    DataRow newDomain = domainListTemp.NewRow();

                    if (itemUser.Properties["Name"] != null && itemUser.Properties["Name"].Value != null)
                        newDomain["DomainName"] = itemUser.Properties["Name"].Value.ToString();
                    if (itemUser.Properties["Status"] != null && itemUser.Properties["Status"].Value != null)
                        newDomain["Status"] = itemUser.Properties["Status"].Value.ToString();
                    if (itemUser.Properties["Authentication"] != null && itemUser.Properties["Authentication"].Value != null)
                        newDomain["Authentication"] = itemUser.Properties["Authentication"].Value.ToString();

                    domainListTemp.Rows.Add(newDomain);
                }

                // Bind data list to grid view.
                DomainList.DataSource = domainListTemp;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Create temp table to store domain data returned from powershell command.
        private DataTable CreateDomainTable()
        {
            DataTable domainListTemp = new DataTable();
            domainListTemp.Columns.Add("DomainName");
            domainListTemp.Columns.Add("Status");
            domainListTemp.Columns.Add("Authentication");
            return domainListTemp;
        }

        /// <summary>
        /// Execute Get-MsolDomain command and returns resulted data.
        /// </summary>
        /// <returns></returns>
        private Collection<PSObject> ExecutePowershellCommands()
        {

            try
            {
                Collection<PSObject> domainList = null;
                // Create Initial Session State for runspace.
                InitialSessionState initialSession = InitialSessionState.CreateDefault();
                initialSession.ImportPSModule(new[] { "MSOnline" });
                // Create credential object.
                PSCredential credential = new PSCredential(UserCredential.UserName, UserCredential.Password);
                // Create command to connect office 365.
                Command connectCommand = new Command("Connect-MsolService");
                connectCommand.Parameters.Add((new CommandParameter("Credential", credential)));
                // Create command to get office 365 domain.
                Command getDomainCommand = new Command("Get-MsolDomain");
                using (Runspace psRunSpace = RunspaceFactory.CreateRunspace(initialSession))
                {
                    // Open runspace.
                    psRunSpace.Open();

                    //Iterate through each command and executes it.
                    foreach (var com in new Command[] { connectCommand, getDomainCommand })
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
                        if (error.Count > 0 && com == getDomainCommand)
                        {
                            MessageBox.Show(error[0].ToString(), "Problem in getting domains");
                            this.Close();
                            return null;
                        }
                        else
                        {
                            domainList = results;
                        }
                    }

                }
                return domainList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Call click event of button create domain.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateDomain_Click(object sender, EventArgs e)
        {
            try
            {

                // Opens 'Create Domain' page as a model dialog.
                DomainOperations domainOperations = new DomainOperations();

                if (domainOperations.ShowDialog() == DialogResult.OK) // If domain added successfully.
                {
                    Cursor.Current = Cursors.WaitCursor;
                    // Update domain list with newly added domain.

                    Collection<PSObject> results = ExecutePowershellCommands();

                    if (results != null && results.Count > 0)
                    {
                        FillDomainList(results);
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
    }
}
