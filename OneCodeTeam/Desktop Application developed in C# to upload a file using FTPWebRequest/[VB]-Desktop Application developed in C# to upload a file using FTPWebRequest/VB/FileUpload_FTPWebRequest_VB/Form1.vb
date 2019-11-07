
'+++++++++++++++DISCLAIMER+++++++++++++++++++++++++++++
'--------------------------------------------------------------------------------- 
'The sample scripts are Not supported under any Microsoft standard support program Or service. The sample scripts are provided As Is without warranty  
'Of any kind. Microsoft further disclaims all implied warranties including, without limitation, any implied warranties Of merchantability Or Of fitness For 
'a particular purpose. The entire risk arising out Of the use Or performance Of the sample scripts And documentation remains With you. In no Event shall 
'Microsoft, its authors, Or anyone Else involved In the creation, production, Or delivery Of the scripts be liable For any damages whatsoever (including, 
'without limitation, damages For loss Of business profits, business interruption, loss Of business information, Or other pecuniary loss) arising out Of the use 
'Of Or inability To use the sample scripts Or documentation, even If Microsoft has been advised Of the possibility Of such damages 
'---------------------------------------------------------------------------------



Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports System.Net
Imports System.IO

Public Class Form1


    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Try
            'Browse the file to be uploaded
            Dim defaultPath As String = "C:\traces\"
            OpenFileDialog1.FileName = Me.txtFilename.Text
            OpenFileDialog1.Filter = "All Files (*.pdf)|*.pdf"
            OpenFileDialog1.InitialDirectory = defaultPath
            If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
                'update the TextBox with selected File's FileName property
                Me.txtFilename.Text = OpenFileDialog1.FileName
            End If
            Me.Button1.Enabled = Me.txtFilename.Text.Length > 0
        Catch ex As Exception
            MessageBox.Show("Error selecting file" + Environment.NewLine + ex.Message)
        End Try

    End Sub

    Private Sub button1_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If Not File.Exists(Me.txtFilename.Text) Then
            MessageBox.Show("Please select a valid file for upload")
        Else

            Dim sourcefilepath As [String] = Me.txtFilename.Text
            Dim fileName As [String] = Path.GetFileName(Me.txtFilename.Text)

            Dim ftpusername As [String] = "Admin"
            Dim ftppassword As [String] = "Pa$$w0rd"

            'Using the System.NET FTPWebRequest class 

            Dim ftp As FtpWebRequest = DirectCast(FtpWebRequest.Create("ftp://localhost/httpdocs/" + DocumentDirectory.Text + "/" + fileName), FtpWebRequest)

            'Enabling the Passive mode FTP with SSL

            ftp.UsePassive = True
            ftp.EnableSsl = True
            ftp.Method = WebRequestMethods.Ftp.ListDirectory

            'Validating the remote server certificate
            ServicePointManager.ServerCertificateValidationCallback = New System.Net.Security.RemoteCertificateValidationCallback(AddressOf AcceptAllCertifications)

            ftp.Credentials = New NetworkCredential(ftpusername, ftppassword)

            ftp.KeepAlive = True
            ftp.UseBinary = True
            ftp.Method = WebRequestMethods.Ftp.UploadFile

            'Read the File to be uploaded to the server
            Dim fs As FileStream = File.OpenRead(sourcefilepath)
            Dim buffer As Byte() = New Byte(fs.Length - 1) {}
            fs.Read(buffer, 0, buffer.Length)
            fs.Close()

            Dim ftpstream As Stream = ftp.GetRequestStream()
            ftpstream.Write(buffer, 0, buffer.Length)
            ftpstream.Close()


            progressBar1.Value = 100
        End If
    End Sub

    Public Function AcceptAllCertifications(sender As Object, certification As System.Security.Cryptography.X509Certificates.X509Certificate, chain As System.Security.Cryptography.X509Certificates.X509Chain, sslPolicyErrors As System.Net.Security.SslPolicyErrors) As Boolean
        Return True
    End Function

    'List of Radio Button to select the folder where the file needs to be uploaded
    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles ProcedureDocuments.CheckedChanged
        DocumentDirectory.Text = "ProcedureDocuments"
    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles PlanningDocuments.CheckedChanged
        DocumentDirectory.Text = "PlanningDocuments"
    End Sub

    Private Sub RadioButton3_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton3.CheckedChanged
        DocumentDirectory.Text = "BulletinBoardDocuments"
    End Sub

    Private Sub RadioButton4_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton4.CheckedChanged
        DocumentDirectory.Text = "GoverningDocuments"
    End Sub

    Private Sub RadioButton5_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton5.CheckedChanged
        DocumentDirectory.Text = "FinancialDocuments"
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub RadioButton7_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton7.CheckedChanged
        DocumentDirectory.Text = "MeetingDocuments"
    End Sub

    Private Sub RadioButton9_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton9.CheckedChanged
        DocumentDirectory.Text = "InvoiceDocuments"
    End Sub

    Private Sub RadioButton8_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton8.CheckedChanged
        DocumentDirectory.Text = "ContractDocuments"
    End Sub

    Private Sub RadioButton10_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton10.CheckedChanged
        DocumentDirectory.Text = "Directories"
    End Sub
End Class
