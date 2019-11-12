/****************************** Module Header ******************************\
* Module Name:  MainForm.cs
* Project:      CSWinFormSingleInstanceApp
* Copyright (c) Microsoft Corporation.
* 
* The  sample demonstrates how to achieve the goal that only 
* one instance of the application is allowed in Windows Forms application..
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace CSWinFormSingleInstanceApp
{
    public partial class MainForm : Form
    {
        #region fields
        LoginForm loginEntry = new LoginForm();
        string loginMsg = "";
        event EventHandler LoginMsgChanged;
        #endregion

        #region Properties
        public string LoginMsg
        {
            get { return loginMsg; }
            set
            {
                if (value != loginMsg)
                {
                    loginMsg = value;
                    OnLoginMsgChanged(EventArgs.Empty);
                }
            }
        }
        #endregion

        #region Constructor
        public MainForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(MainForm_Load);
            this.StartPosition = FormStartPosition.CenterScreen;
         
        }
        #endregion

        #region EventHandlers
        void MainForm_Load(object sender, EventArgs e)
        {
            //show Login dialog
            loginEntry.ShowDialog();
            //display welcome message
            DisplayWelcomeMessage();
            //bind Text of lableUserStatus to LoginMsg property.
            this.labelUserStatus.DataBindings.Add("Text", this, "LoginMsg");
        }
        private void buttonLogoff_Click(object sender, EventArgs e)
        {
            GlobleData.IsUserLoggedIn = false;
            LoginMsg = "User Successfully logged off";

            this.Visible = false;
            loginEntry = new LoginForm();
            if (loginEntry.ShowDialog() == DialogResult.OK)
            {
                if (this.Visible == false && GlobleData.IsUserLoggedIn)
                {
                    this.Visible = true;
                }
                LoginMsg = "User Successfully logged in";
            }
        }
        #endregion

        #region HelperMethods
        void DisplayWelcomeMessage()
        {
            string welcomeMsg = "Welcome to codeplex:" + GlobleData.UserName;
            this.labelWelcomeMsg.Text = welcomeMsg;        
            LoginMsg ="User Successfully logged in";
           
        }

        void OnLoginMsgChanged(EventArgs e)
        {
            if (LoginMsgChanged != null)
            {
                LoginMsgChanged(this, e);
            }
        }
        #endregion
    }
}
