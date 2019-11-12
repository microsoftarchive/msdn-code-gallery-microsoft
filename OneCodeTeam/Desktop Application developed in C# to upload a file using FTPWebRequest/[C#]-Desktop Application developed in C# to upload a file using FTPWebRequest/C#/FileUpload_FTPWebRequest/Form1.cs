
//+++++++++++++++DISCLAIMER+++++++++++++++++++++++++++++
//--------------------------------------------------------------------------------- 
//The sample scripts are not supported under any Microsoft standard support program or service. The sample scripts are provided AS IS without warranty  
//of any kind. Microsoft further disclaims all implied warranties including, without limitation, any implied warranties of merchantability or of fitness for 
//a particular purpose. The entire risk arising out of the use or performance of the sample scripts and documentation remains with you. In no event shall 
//Microsoft, its authors, or anyone else involved in the creation, production, or delivery of the scripts be liable for any damages whatsoever (including, 
//without limitation, damages for loss of business profits, business interruption, loss of business information, or other pecuniary loss) arising out of the use 
//of or inability to use the sample scripts or documentation, even if Microsoft has been advised of the possibility of such damages 
//---------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace FileUpload_FTPWebRequest
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            //Disable the Upload and Stop button untill the file is selected for uploading
            base.OnLoad(e);
            button1.Enabled = false;
          
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (!File.Exists(this.txtFilename.Text))
                MessageBox.Show("Please select a valid file for upload");
            else
            {
              
                String sourcefilepath = this.txtFilename.Text;   
                String fileName= Path.GetFileName(this.txtFilename.Text);
               
                String ftpusername = "admin";
                String ftppassword = "P@$$W0rd";

                //Using the System.NET FTPWebRequest class
                FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create("ftp://localhost/httpdocs/" + DocumentDirectory.Text + "/" + fileName);

                //Enabling the Passive mode FTP with SSL
                ftp.UsePassive = true;
                ftp.EnableSsl = true;
                ftp.Method = WebRequestMethods.Ftp.ListDirectory;

                //Validating the remote server certificate
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

                ftp.Credentials = new NetworkCredential(ftpusername, ftppassword);

                ftp.KeepAlive = true;
                ftp.UseBinary = true;
                ftp.Method = WebRequestMethods.Ftp.UploadFile;

                //Read the File to be uploaded to the server
                FileStream fs = File.OpenRead(sourcefilepath);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();

                Stream ftpstream = ftp.GetRequestStream();
                ftpstream.Write(buffer, 0, buffer.Length);
                ftpstream.Close();
                
                progressBar1.Value =100;
            }
        }

        public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        //List of Radio Button to select the folder where the file needs to be uploaded
        public void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            DocumentDirectory.Text = "ProcedureDocuments";
        }

        public void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            DocumentDirectory.Text = "PlanningDocuments";
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                //Browse the file to be uploaded
                string defaultPath= @"C:\traces\";
                openFileDialog1.FileName = this.txtFilename.Text;
                openFileDialog1.Filter = "All Files (*.pdf)|*.pdf";
                openFileDialog1.InitialDirectory = defaultPath;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    //update the TextBox with selected File's FileName property
                    this.txtFilename.Text = openFileDialog1.FileName;
                }
                this.button1.Enabled = this.txtFilename.Text.Length > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error selecting file" + Environment.NewLine + ex.Message);
            }

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            DocumentDirectory.Text = "BulletinBoardDocuments";
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            DocumentDirectory.Text = "GoverningDocuments";
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            DocumentDirectory.Text = "FinancialDocuments";
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            DocumentDirectory.Text = "MeetingDocuments";
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            DocumentDirectory.Text = "LegalDocuments";
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            DocumentDirectory.Text = "ContractDocuments";
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            DocumentDirectory.Text = "InvoiceDocuments";
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            DocumentDirectory.Text = "Directories";
        }

    }
}
