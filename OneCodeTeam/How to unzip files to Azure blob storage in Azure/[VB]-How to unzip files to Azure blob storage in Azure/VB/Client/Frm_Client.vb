'/****************************** Module Header ******************************\
'Module Name: Form1.vb
'Project:     Client
'Copyright (c) Microsoft Corporation.
'
'This is the client side programe. It's used to invoke the WCF service on 
'Azure workrole. 
' 
'This source is subject to the Microsoft Public License.
'See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
'All other rights reserved.
'
'THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/

Imports System.Text.RegularExpressions
Imports System.IO

Public Class Frm_Client

    Dim client As New ServiceReference1.UnZipWcfServiceClient()
 
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        BindingContainer()
    End Sub

    Dim lstUploadFile As New List(Of String)()

    ''' <summary>
    ''' Select zip file you want to upload to blob 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btn_SelectFile_Click(sender As Object, e As EventArgs) Handles btn_SelectFile.Click
        Dim filePath As String
        Dim fileName As String
        Dim fileExtension As String
        Dim strExtension() As String = {".zip"}
        lstUploadFile.Clear()
        txt_FileName.Text = ""
        Try
            Dim myOpenFileDialog As New OpenFileDialog()
            'InitialDirectory  
            myOpenFileDialog.InitialDirectory = "d:\"
            myOpenFileDialog.Filter = "zip files (*.zip)|*.zip|All files (*.*)|*.*"
            myOpenFileDialog.FilterIndex = 1
            myOpenFileDialog.RestoreDirectory = True
            myOpenFileDialog.Title = "Choose Zip File"
            myOpenFileDialog.Multiselect = True

            If myOpenFileDialog.ShowDialog() = DialogResult.OK Then
                Dim strArrPath() As String = myOpenFileDialog.FileNames

                For i As Integer = 0 To strArrPath.Length - 1
                    filePath = strArrPath(i)
                    fileName = Path.GetFileName(filePath)
                    fileExtension = Path.GetExtension(filePath)
                    txt_FileName.Text &= """" & fileName & """" & " "
                    If Not strExtension.Contains(fileExtension) Then
                        MessageBox.Show(fileName & " is not Zip file")
                        txt_FileName.Text = ""
                        Exit For
                    End If
                    If Not lstUploadFile.Contains(filePath) Then
                        lstUploadFile.Add(filePath)
                    End If
                Next i
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btn_UnZipAndUpload_Click(sender As Object, e As EventArgs) Handles btn_UnZipAndUpload.Click
        Try
            If CheckContainerName() Then
                If lstUploadFile.Count > 0 Then
                    Dim ret As Boolean = False
                    For i As Integer = 0 To lstUploadFile.Count - 1
                        If i <> 0 AndAlso ret = False Then
                            MessageBox.Show("Unziping file and uploading to blob storage failed!")
                            Exit For
                        End If
                        ret = client.UnZipFiles(lstUploadFile(i), txt_ContainerName.Text)
                    Next i

                    If ret Then
                        MessageBox.Show("Successfully unzip file and upload to blob storage!")
                    Else
                        MessageBox.Show("Unziping file and uploading to blob storage failed!")
                    End If
                Else
                    MessageBox.Show("Select the zip file you want to unzip and upload!")
                End If
            Else
                MessageBox.Show("Container name is error, please check it!")
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
        ' Lists all Containers and add them to ComboBox
        BindingContainer()
    End Sub

    ''' <summary>
    ''' Select the container of which you want to view all blobs 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cbx_Container_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbx_Container.SelectedIndexChanged
        Try
            Dim strContainerName As String = cbx_Container.SelectedItem.ToString()
            BindingBlob(strContainerName)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub


    ''' <summary>
    '''  Lists all Containers and add them to ComboBox
    ''' </summary>
    Private Sub BindingContainer()
        Try
            Dim intIndex As Integer = cbx_Container.SelectedIndex              
            Dim strSelectText As String = cbx_Container.Text
            cbx_Container.Items.Clear()
            Dim lstContainer As List(Of String) = client.GetAllContainer().ToList()
            If lstContainer IsNot Nothing Then
                For i As Integer = 0 To lstContainer.Count() - 1
                    cbx_Container.Items.Add(lstContainer(i))
                Next i
                If cbx_Container.Items.Count > 0 AndAlso intIndex >= 0 Then
                    cbx_Container.Text = strSelectText
                ElseIf cbx_Container.Items.Count > 0 Then
                    cbx_Container.SelectedIndex = 0
                End If
                cbx_Container_SelectedIndexChanged(Nothing, Nothing)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Lists all blobs and add them to flowLayoutPanel1
    ''' </summary>
    ''' <param name="strContainerName"></param>
    Private Sub BindingBlob(ByVal strContainerName As String)
        Try
            flowLayoutPanel1.Controls.Clear()
            If Not String.IsNullOrEmpty(strContainerName) Then
                Dim lstBolb As List(Of String) = client.GetAllBlob(strContainerName).ToList()
                If lstBolb IsNot Nothing Then
                    For i As Integer = 0 To lstBolb.Count() - 1
                        Dim label As New Label()
                        label.Name = "lbl_" & i.ToString()
                        label.Text = lstBolb(i)
                        label.Margin = New System.Windows.Forms.Padding(3)
                        label.Padding = New System.Windows.Forms.Padding(3)
                        label.AutoSize = True
                        flowLayoutPanel1.Controls.Add(label)
                    Next i
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Checks container name  
    ''' </summary>
    ''' <returns></returns>
    Private Function CheckContainerName() As Boolean
        Dim retFlag As Boolean = False
        Dim strConName As String = txt_ContainerName.Text.Trim()
        If strConName.Length >= 3 AndAlso strConName.Length <= 63 Then
            Dim regex As New Regex("^[a-z0-9]([a-z0-9]|(?:(-)(?!\2)))*$")
            retFlag = regex.IsMatch(strConName)
        End If
        Return retFlag
    End Function

End Class
