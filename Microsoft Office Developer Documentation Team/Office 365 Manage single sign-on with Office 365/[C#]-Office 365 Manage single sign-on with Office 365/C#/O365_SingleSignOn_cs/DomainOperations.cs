using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Windows.Forms;

namespace O365_SingleSignOn
{
    public partial class DomainOperations : Form
    {
        public DomainOperations()
        {
            InitializeComponent();
        }

        private void DomainOperations_Load(object sender, EventArgs e)
        {
            DomainType.SelectedIndex = 0;
        }

        private void AddDomain_Click(object sender, EventArgs e)
        {
            try
            {
                // Check for required parameters.
                if (CheckRequiredFields())
                {
                    // Initialize domain object and fill with values.
                    Cursor.Current = Cursors.WaitCursor;
                    O365Domain domainEntry = new O365Domain();
                    domainEntry.DomainName = FullDomainName.Text.Trim();
                    domainEntry.DomainType = DomainType.SelectedItem.ToString().Trim();

                    AddDomain(domainEntry);
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
        /// Perform add domain operation using command New-MsolDomain / New-MsolFederatedDomain.
        /// </summary>
        /// <param name="userEntry"></param>
        private void AddDomain(O365Domain domainEntry)
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
                connectCommand.Parameters.Add((new CommandParameter("Credential", credential)));
                // Create command to add office 365 domain.
                Command domainCommand = null;

                if (domainEntry.DomainType == "Standard(Managed) Domain")
                {
                    domainCommand = new Command("New-MsolDomain");
                    domainCommand.Parameters.Add((new CommandParameter("Name", domainEntry.DomainName)));
                }
                else if (domainEntry.DomainType == "Federated(SSO) Domain")
                {
                    domainCommand = new Command("New-MsolFederatedDomain");
                    domainCommand.Parameters.Add((new CommandParameter("DomainName", domainEntry.DomainName)));
                }
                using (Runspace psRunSpace = RunspaceFactory.CreateRunspace(initialSession))
                {
                    // Open runspace.
                    psRunSpace.Open();

                    //Iterate through each command and executes it.
                    foreach (var com in new Command[] { connectCommand, domainCommand })
                    {
                        var pipe = psRunSpace.CreatePipeline();
                        pipe.Commands.Add(com);
                        // Execute command and generate results and errors (if any).
                        Collection<PSObject> results = pipe.Invoke();
                        var error = pipe.Error.ReadToEnd();


                        if (error.Count > 0 && com == domainCommand)
                        {
                            MessageBox.Show(error[0].ToString());
                            this.DialogResult = DialogResult.None;
                            return;
                        }
                        else if (results.Count >= 0 && com == domainCommand)
                        {
                            if (domainEntry.DomainType == "Standard(Managed) Domain")
                            {
                                MessageBox.Show("DomainName : " + results[0].Properties["Name"].Value.ToString() + "\r\n" +
                              "Status : " + results[0].Properties["Status"].Value.ToString() + "\r\n" +
                              "Authentication: " + results[0].Properties["Authentication"].Value.ToString(),
                              "Domain added successfully.");
                            }
                            else if (domainEntry.DomainType == "Federated(SSO) Domain")
                            {
                                MessageBox.Show("Federated Domain added successfully.");
                            }

                            this.DialogResult = DialogResult.OK;
                        }

                    }


                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Validate required parameters.
        /// </summary>
        /// <returns></returns>
        private bool CheckRequiredFields()
        {
            string validationMessage = "";
            if (FullDomainName.Text == "")
            {
                validationMessage += "\r\n DomainName";
            }

            if (validationMessage != "")
            {
                validationMessage = "You must provide required field(s) : " + validationMessage;
                MessageBox.Show(validationMessage, "Required Parameters");
                this.DialogResult = DialogResult.None;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    // This class is designed to define domain object values.
    public class O365Domain
    {
        private string domainName = "";
        public string DomainName
        {
            get { return domainName; }
            set { domainName = value; }
        }

        private string domainType = "";
        public string DomainType
        {
            get { return domainType; }
            set { domainType = value; }
        }

    }
}
