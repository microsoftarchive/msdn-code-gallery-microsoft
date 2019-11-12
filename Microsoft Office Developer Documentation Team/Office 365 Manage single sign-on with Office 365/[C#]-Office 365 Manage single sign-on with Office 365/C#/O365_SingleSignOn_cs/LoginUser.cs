using System;
using System.Security;
using System.Windows.Forms;

namespace O365_SingleSignOn
{
    public partial class LoginUser : Form
    {
       
        public LoginUser()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, EventArgs e)
        {

            // Verify user credentials are empty or not.
            if (UserPrincipalName.Text == "" || UserPassword.Text == "")
            {
                MessageBox.Show("Username and Password cannot be empty", "Problem in login");
                this.DialogResult = DialogResult.None;
                return;
            }

            // Assign username to variable.
            UserCredential.UserName = UserPrincipalName.Text;

            // Create and assign secure password string.
            SecureString securePass = new SecureString();
            foreach (char secureChar in UserPassword.Text)
            {
                securePass.AppendChar(secureChar);
            }

            UserCredential.Password = securePass;

            // Close this dialog box and proceed to verify user credentials.
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

      
    }
}
