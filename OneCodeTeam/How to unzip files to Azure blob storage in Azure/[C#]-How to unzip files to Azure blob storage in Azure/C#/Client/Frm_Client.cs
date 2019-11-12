/****************************** Module Header ******************************\
*Module Name: Form1.cs
*Project:     Client
*Copyright (c) Microsoft Corporation.
*
*This is the client side programe. It's used to invoke the WCF service on 
*Azure workrole. 
* 
*This source is subject to the Microsoft Public License.
*See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
*All other rights reserved.
*
*THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
*EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
*WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Frm_Client : Form
    {
        ServiceReference1.UnZipWcfServiceClient client = new ServiceReference1.UnZipWcfServiceClient();
        public Frm_Client()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BindingContainer();
        }


        /// <summary>
        ///  Lists all Containers and add them to ComboBox
        /// </summary>
        private void BindingContainer()
        {
            try
            {
                int intIndex = cbx_Container.SelectedIndex;
                string strSelectText = cbx_Container.Text;
                cbx_Container.Items.Clear();
                List<string> lstContainer = client.GetAllContainer().ToList();
                if (lstContainer != null)
                {
                    for (int i = 0; i < lstContainer.Count(); i++)
                    {
                        cbx_Container.Items.Add(lstContainer[i]);
                    }
                    if (cbx_Container.Items.Count > 0 && intIndex>=0)
                    {
                        cbx_Container.Text = strSelectText;
                    }
                    else if (cbx_Container.Items.Count > 0)
                    {
                        cbx_Container.SelectedIndex = 0;
                    }
                    cbx_Container_SelectedIndexChanged(null,null);    
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          
        }

        List<string> lstUploadFile = new List<string>();
        /// <summary>
        /// Select zip file you want to upload to blob 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SelectFile_Click(object sender, EventArgs e)
        {
            string filePath;   
            string fileName;  
            string fileExtension;   
            string[] strExtension = new string[] { ".zip"};
            lstUploadFile.Clear();
            txt_FileName.Text = "";
            try
            {
                OpenFileDialog myOpenFileDialog = new OpenFileDialog();
                //InitialDirectory  
                myOpenFileDialog.InitialDirectory = "d:\\";
                myOpenFileDialog.Filter = "zip files (*.zip)|*.zip|All files (*.*)|*.*"; 
                myOpenFileDialog.FilterIndex = 1;
                myOpenFileDialog.RestoreDirectory = true;
                myOpenFileDialog.Title = "Choose Zip File";
                myOpenFileDialog.Multiselect = true;
                
                if (myOpenFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string[] strArrPath = myOpenFileDialog.FileNames;
                      
                    for (int i = 0; i < strArrPath.Length;i++ )
                    {
                        filePath = strArrPath[i];
                        fileName = Path.GetFileName(filePath);
                        fileExtension = Path.GetExtension(filePath);
                        txt_FileName.Text += "\"" + fileName + "\"" + " "; 
                        if (!strExtension.Contains(fileExtension))
                        {
                            MessageBox.Show(fileName+ " is not Zip file");
                            txt_FileName.Text = "";
                            break;
                        }
                        if(!lstUploadFile.Contains(filePath))
                        {
                            lstUploadFile.Add(filePath);
                        }                       
                    }
                }  
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }  
        }

        /// <summary>
        /// Select the container of which you want to view all blobs 
        /// </summary>
        /// <param name="sender"></param> 
        /// <param name="e"></param>
        private void cbx_Container_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string strContainerName = cbx_Container.SelectedItem.ToString();
                BindingBlob(strContainerName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// Lists all blobs and add them to flowLayoutPanel1
        /// </summary>
        /// <param name="strContainerName"></param>
        private void BindingBlob(string strContainerName)
        {
            try
            {
                flowLayoutPanel1.Controls.Clear();
                if (!string.IsNullOrEmpty(strContainerName))
                {
                    List<string> lstBolb = client.GetAllBlob(strContainerName).ToList();
                    if (lstBolb != null)
                    {
                        for (int i = 0; i < lstBolb.Count(); i++)
                        {
                            Label label = new Label();
                            label.Name = "lbl_" + i.ToString();
                            label.Text = lstBolb[i];
                            label.Margin = new System.Windows.Forms.Padding(3);
                            label.Padding = new System.Windows.Forms.Padding(3);
                            label.AutoSize = true;
                            flowLayoutPanel1.Controls.Add(label);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Unzips file and uploads to blob
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_UnZipAndUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckContainerName())
                {
                    if (lstUploadFile.Count>0)
                    {
                        bool ret = false;
                        for (int i = 0; i < lstUploadFile.Count; i++)
                        {
                            if(i!=0&&ret==false)
                            {
                                MessageBox.Show("Unziping file and uploading to blob storage failed!");
                                break;
                            }
                            ret = client.UnZipFiles(lstUploadFile[i], txt_ContainerName.Text);
                        }
                      
                        if(ret)
                        {
                            MessageBox.Show("Successfully unzip file and upload to blob storage!");
                        }
                        else
                        {
                            MessageBox.Show("Unziping file and uploading to blob storage failed!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Select the zip file you want to unzip and upload!");
                    }
                }
                else
                {
                    MessageBox.Show("Container name is error, please check it!");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            // Lists all Containers and add them to ComboBox
            BindingContainer();
        }


        /// <summary>
        /// Checks container name  
        /// </summary>
        /// <returns></returns>
        private bool CheckContainerName()
        {
            bool retFlag = false;
            string strConName=txt_ContainerName.Text.Trim();
            if(strConName.Length>=3&&strConName.Length<=63)
            {
                Regex regex = new Regex("^[a-z0-9]([a-z0-9]|(?:(-)(?!\\2)))*$");
                retFlag = regex.IsMatch(strConName);
            }
            return retFlag;
        }
 
    }
}
