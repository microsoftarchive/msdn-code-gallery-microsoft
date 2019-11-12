/****************************** Module Header ******************************\
Module Name:  AutoUpdate.cs
Project:      CSDataSqlNotificationRequest
Copyright (c) Microsoft Corporation.

After executing a command on database to get data, the data may be changed by
the other clients. If the application needs the latest data, it needs a notification 
from the server. In this application, we will demonstrate how to execute the 
SqlCommand with a SqlNotificationRequest:
1. Set and execute the SqlCommand with a SqlNotificationRequest;
2. Begin to monitor the queue in SqlServer;
3. Refresh the data.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Security;
using CSDataSqlNotificationRequest.Properties;
using System.Data;

namespace CSDataSqlNotificationRequest
{
    public partial class AutoUpdate : Form
    {
        private SqlNotificationRequestRegister notification = null;

        private SqlConnection conn = null;
        private SqlCommand command = null;
        // StudentGradeChangeMessages is the name of the Service Broker service that has
        // been defined.
        private const String serviceName = "StudentGradeChangeNotifications";
        private Int32 timeOut = 30;
        private DataSet dataToWatch = null;
        private const String tableName = "Grade";

        private Boolean isStopped = true;
        private Int32 enrollmentID = -1;
        private Int32 rowId = -1;
        private Int32 colId = -1;

        private bool CanRequestNotifications()
        {
            // In order to use the callback feature of the
            // SqlDependency, the application must have
            // the SqlClientPermission permission.
            try
            {
                SqlClientPermission perm =
                    new SqlClientPermission(
                    PermissionState.Unrestricted);

                perm.Demand();

                return true;
            }
            catch (SecurityException se)
            {
                MessageBox.Show(se.Message, "Permission Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public AutoUpdate()
        {
            InitializeComponent();
        }

        private void AutoUpdate_Load(object sender, EventArgs e)
        {
            btnGetData.Enabled = CanRequestNotifications();
        }

        /// <summary>
        /// Get the data and begin to monitor the queue
        /// </summary>
        private void GetData_Click(object sender, EventArgs e)
        {
            String connectionString = Settings.Default.MyConnectionString;

            if (conn == null)
            {
                conn = new SqlConnection(connectionString);
            }

            if (command == null)
            {
                command = new SqlCommand(GetSelectSQL(), conn);
            }

            if (dataToWatch == null)
            {
                dataToWatch = new DataSet();
            }

            // Create a SqlNotificationRequest
            notification = new SqlNotificationRequestRegister(GetListenerSQL(),
                serviceName, timeOut, connectionString);
            notification.OnChanged += NotificationOnChanged;

            ChangeButtonState();

            GetData();
        }

        /// <summary>
        /// When changed the data, the method will be invoked.
        /// </summary>
        void NotificationOnChanged(object sender, EventArgs e)
        {
            // If InvokeRequired returns True, the code
            // is executing on a worker thread.
            if (this.InvokeRequired)
            {
                // Create a delegate to perform the thread switch.
                this.BeginInvoke(new Action(GetData), null);

                return;
            }

            GetData();
        }

        /// <summary>
        /// Display the data in the DataGridView
        /// </summary>
        void GetData()
        {
            if (notification == null || isStopped)
            {
                return;
            }

            dataToWatch.Clear();

            command.Notification = null;
            command.Notification = notification.NotificationRequest;

            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                adapter.Fill(dataToWatch, tableName);

                dgvWatch.DataSource = dataToWatch;
                dgvWatch.DataMember = tableName;
                dgvWatch.Columns["EnrollmentID"].Visible = false;

                if (rowId > 0 && rowId <= dgvWatch.RowCount)
                {
                    dgvWatch.Rows[rowId].Cells[colId].Selected = true;
                    dgvWatch.Rows[0].Cells[1].Selected = false;
                }
            }

            // Monitor the queue again
            notification.StartSqlNotification();
        }

        /// <summary>
        /// Update the new grade.
        /// </summary>
        private void Update_Click(object sender, EventArgs e)
        {
            Decimal grade = -1;
            if (!Decimal.TryParse(txtGrade.Text, out grade))
            {
                MessageBox.Show("Please input the right new grade!");
                return;
            }

            if (grade > 0)
            {
                String updateSqlSting = GetUpdateSQL();

                using (SqlCommand cmd = new SqlCommand(updateSqlSting, conn))
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }

                    cmd.Parameters.AddWithValue("@Grade", grade);
                    cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);

                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }
        }

        private void dgvWatch_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataToWatch == null || this.btnGetData.Enabled)
            {
                return;
            }

            DataTable table = dataToWatch.Tables[tableName];
            rowId = e.RowIndex;
            colId = e.ColumnIndex;

            if (e.RowIndex >= 0 && e.RowIndex < table.Rows.Count)
            {
                DataRow row = table.Rows[e.RowIndex];

                Int32 id = -1;
                if (Int32.TryParse(row["EnrollmentID"].ToString(), out id))
                {
                    enrollmentID = id;
                }
                else
                {
                    return;
                }

                txtName.Text = row["Name"].ToString();
                txtCourse.Text = row["Course"].ToString();
                txtGrade.Text = row["Grade"].ToString();

                btnUpdate.Enabled = true;
                txtGrade.Enabled = true;
            }
        }

        /// <summary>
        /// Stop SqlNotificationRequest.
        /// </summary>
        private void StopSqlNotificationRequest(object sender, EventArgs e)
        {
            if (notification != null)
            {
                notification.StopSqlNotification();
                command.Dispose();
            }

            btnUpdate.Enabled = false;
            ChangeButtonState();
        }

        private void ChangeButtonState()
        {
            this.btnGetData.Enabled = this.btnStop.Enabled;
            this.btnStop.Enabled = !this.btnStop.Enabled;
            isStopped = !isStopped;
            txtGrade.Enabled = btnUpdate.Enabled;
        }

        private String GetSelectSQL()
        {
            return @"Select sg.EnrollmentID, pe.FirstName +' '+pe.LastName as Name,co.Title as Course,sg.Grade
                     From dbo.StudentGrade as sg join dbo.Person as pe on sg.StudentID=pe.PersonID
                     join dbo.Course as co on sg.CourseID=co.CourseID 
                     order by Name";
        }

        private String GetUpdateSQL()
        {
            return @"update [dbo].[StudentGrade] set Grade=@Grade where EnrollmentID=@EnrollmentID";
        }

        private String GetListenerSQL()
        {
            // StudentGradeChangeMessages is the name of the Service Broker queue that has
            // been defined.
            return "WAITFOR (RECEIVE * FROM StudentGradeChangeMessages);";
        }
    }
}
