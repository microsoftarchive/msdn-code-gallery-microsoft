using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management.Automation;
using System.Security;
using System.Text;
using System.Windows.Forms;
using Microsoft.Lync.Model;

namespace O365_Dashboard
{
    public partial class ShowDashboard : Form
    {
        LyncClient lyncClient;
        ContactManager contactMgr;
        private decimal usedSize, totalSize;

        public ShowDashboard()
        {
            InitializeComponent();

            try
            {
                lyncClient = LyncClient.GetClient();
                contactMgr = lyncClient.ContactManager;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowDashboard_Load(object sender, EventArgs e)
        {
            SignInAndGetAvailability();
        }

        /// <summary>
        /// Draw progress bar like rectangle to show mailbox usage.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OutlookSection_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (totalSize == 0)
                    return;
                
                // Default width of main rectangle.
                int totalWidth = 380;
                Graphics g = MailboxUsageGroup.CreateGraphics();
                SolidBrush brushMain = new SolidBrush(Color.LightGray);
                SolidBrush brushInner = new SolidBrush(Color.Green);

                // Calculate usage percentage.
                decimal usedPerc = (Convert.ToDecimal(usedSize) * 100) / totalSize;
                int usedWidth = Convert.ToInt32((totalWidth * usedPerc) / 100);

                // Draw main rectangle with light gray brush and progress rectangle with green brush.
                g.FillRectangle(brushMain, SummaryInfo.Left, SummaryInfo.Top + 30, totalWidth, 30);
                g.FillRectangle(brushInner, SummaryInfo.Left, SummaryInfo.Top + 30, usedWidth, 30);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DashboardMainTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (DashboardMainTab.SelectedIndex == 1)
            {
                FillEmailCountGraph();
                FillEmailBoxSizeProgress();
            }
            else
            {
                SignInAndGetAvailability();
            }

            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Get details of ProhibitSendQuota and TotalItemSize for given mailbox.
        /// Uses System.Management.Automation to run powershell commands.
        /// </summary>
        private void FillEmailBoxSizeProgress()
        {
            try
            {
                OutlookHelper outlookHelper = new OutlookHelper();

                // Create secure password string.
                SecureString securePass = new SecureString();

                foreach (char secureChar in ConfigurationManager.AppSettings["Password"].ToString())
                {
                    securePass.AppendChar(secureChar);
                }

                PSCredential credential = new PSCredential(ConfigurationManager.AppSettings["UserID"].ToString(), securePass);

                // Get allocated and used quota.
                string[] resultData = outlookHelper.GetMailBoxSizeDetails(credential,
                ConfigurationManager.AppSettings["LiveIDConnectionUri"].ToString(), ConfigurationManager.AppSettings["SchemaUri"].ToString());
                string allocatedQuota = resultData[0].Substring(resultData[0].IndexOf("(") + 1);
                allocatedQuota = allocatedQuota.Substring(0, allocatedQuota.IndexOf("bytes") - 1).Trim();

                string usedQuota = resultData[1].Substring(resultData[1].IndexOf("(") + 1);
                usedQuota = usedQuota.Substring(0, usedQuota.IndexOf("bytes") - 1).Trim();
                usedSize = Convert.ToDecimal(usedQuota);
                totalSize = Convert.ToDecimal(allocatedQuota);

                UsedSizeInfo.Text = (Convert.ToDecimal(usedSize) / 1024 / 1024).ToString("0.00") + " MB";
                TotalSizeInfo.Text = (Convert.ToDecimal(totalSize) / 1024 / 1024 / 1024).ToString("0.00") + " GB";

                if (totalSize > 0)
                {
                    SummaryInfo.Text = "Using " + ((Convert.ToDecimal(usedSize) * 100) / totalSize).ToString("0.00") + " % of total size.";
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Get count of read and unread emails in inbox only.
        /// Uses Microsoft.Exchange.WebServices to get these details.
        /// </summary>
        private void FillEmailCountGraph()
        {
            try
            {
                OutlookHelper outlookHelper = new OutlookHelper(true);

                int[] mailCount = outlookHelper.GetMailCountDetails();

                int readEmailCount = mailCount[0];
                int unReadEmailCount = mailCount[1];

                // Create array for y axis.
                int[] yValues = { readEmailCount, unReadEmailCount };

                // Bind data to chart series.
                MailCountChart.Series["srsMailCount"].Points.DataBindY(yValues);

                // Set label to show for each data point.
                MailCountChart.Series["srsMailCount"].Points[0].AxisLabel = mailCount[0].ToString();
                MailCountChart.Series["srsMailCount"].Points[1].AxisLabel = mailCount[1].ToString();

                // Set legend text.
                MailCountChart.Series["srsMailCount"].Points[0].LegendText = "Read Emails";
                MailCountChart.Series["srsMailCount"].Points[1].LegendText = "Unread Emails";
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Sign in to Lync Client if not signed in.
        /// </summary>
        public void SignInAndGetAvailability()
        {
            try
            {
                // Get Lync Client and Contact Manager.
                lyncClient = LyncClient.GetClient();

                // Register for the client state changed event 
                lyncClient.StateChanged += lyncClient_StateChanged;

                //If lync client is Uninitialized then first initialize it.
                if (lyncClient.State == ClientState.Uninitialized)
                {
                    lyncClient.BeginInitialize((ar) =>
                    {
                        lyncClient.EndInitialize(ar);
                    },
                    null);
                }
                // If lync client is signed out then let sign in user first.
                else if (lyncClient.State == ClientState.SignedOut)
                {
                    SignUserIntoLync();
                }
                // If user already signed in then get user list with their presence status.
                else if (lyncClient.State == ClientState.SignedIn)
                {
                    BindOnlineUsers();
                }
            }
            catch (ClientNotFoundException)
            {
                MessageBox.Show("Lync is not running on this computer");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Handles lync client state change events such as uninitialized->signed out and
        /// Signed out -> signed in.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lyncClient_StateChanged(object sender, ClientStateChangedEventArgs e)
        {
            if (e.OldState == ClientState.Uninitialized && e.NewState == ClientState.SignedOut)
            {
                SignUserIntoLync();
            }
            if (e.NewState == ClientState.SignedIn)
            {
                BindOnlineUsers();
            }
        }

        /// <summary>
        /// Starts the asynchronous sign in process
        /// </summary>
        private void SignUserIntoLync()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["UserID"].ToString()) &&
                !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Password"].ToString()))
            {
                lyncClient.BeginSignIn(
                    ConfigurationManager.AppSettings["UserID"].ToString(),
                    ConfigurationManager.AppSettings["UserID"].ToString(),
                    ConfigurationManager.AppSettings["Password"].ToString(),
                    (ar) => 
                    {
                        lyncClient.EndSignIn(ar);
                    }
                    , null);
            }
        }

        private delegate void BindOnlineUsersDelegate();

        /// <summary>
        /// Get list of online users and set datasource to gridview.
        /// </summary>
        private void BindOnlineUsers()
        {
            try
            {
                LyncHelper lyncHelper = new LyncHelper();
                List<O365User> userList = lyncHelper.GetListOfOnlineUsers(contactMgr);

                if (UserList.InvokeRequired)
                {
                    BindOnlineUsersDelegate invoke = new BindOnlineUsersDelegate(BindOnlineUsers);
                    this.Invoke(invoke);
                }
                else
                {
                    UserList.DataSource = userList;
                    UserList.ClearSelection();

                    foreach (DataGridViewRow row in this.UserList.Rows)
                    {
                        row.Height = 40;
                        DataGridViewCell cell = UserList.Rows[row.Index].Cells[UserList.Columns["colColorStatus"].Index];

                        if (UserList.Rows[row.Index].Cells[UserList.Columns["colStatus"].Index].Value.ToString().Trim() == "Available")
                        {
                            cell.Style.BackColor = Color.LimeGreen;
                        }
                        else if (UserList.Rows[row.Index].Cells[UserList.Columns["colStatus"].Index].Value.ToString().Trim() == "Busy")
                        {
                            cell.Style.BackColor = Color.Red;
                        }
                        else if (UserList.Rows[row.Index].Cells[UserList.Columns["colStatus"].Index].Value.ToString().Trim() == "Do not disturb")
                        {
                            cell.Style.BackColor = Color.DarkRed;
                        }
                        else if (UserList.Rows[row.Index].Cells[UserList.Columns["colStatus"].Index].Value.ToString().Trim() == "Be right back"
                            || UserList.Rows[row.Index].Cells[UserList.Columns["colStatus"].Index].Value.ToString().Trim() == "Off work"
                            || UserList.Rows[row.Index].Cells[UserList.Columns["colStatus"].Index].Value.ToString().Trim() == "Away")
                        {
                            cell.Style.BackColor = Color.Yellow;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception on binding to on-line users " + ex.Message);
            }
        }
    }
}
