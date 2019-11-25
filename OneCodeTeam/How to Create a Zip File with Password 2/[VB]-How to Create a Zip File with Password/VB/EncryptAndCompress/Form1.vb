Imports System
Imports System.Windows.Forms
Imports System.Security.Cryptography
Imports System.IO
Imports System.IO.Packaging
Imports System.Diagnostics


Public Class Form1

    Private Sub btnBrowse_Click(sender As System.Object, e As System.EventArgs) Handles btnBrowse.Click
        Dim browseFile As New OpenFileDialog
        browseFile.Multiselect = False
        browseFile.Filter = "Text Files (*.txt)|*.txt"
        browseFile.ShowDialog()
        If Not [String].IsNullOrEmpty(browseFile.FileName) Then
            tbFilePath.Text = browseFile.FileName
        End If
    End Sub

    Private Sub btnCompress_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCompress.Click
        Dim originalData As String = String.Empty
        Dim encryptedData() As Byte = Nothing
        Dim sourceFileDirectory As String = String.Empty
        Dim sourceFileName As String = String.Empty
        Dim encryptedFileName As String = String.Empty
        If (tbFilePath.Text = String.Empty) Then
            MessageBox.Show("Please choose a file to encrypt and zip.", "Encrypt Compress", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Else
            Try
                ' Reading all the data from the source file.
                originalData = File.ReadAllText(tbFilePath.Text)
                ' Creating a new instance of the AesCryptoServiceProvider class.
                ' This generates a new key and initialization vector (IV).
                Dim myAes As AesCryptoServiceProvider = New AesCryptoServiceProvider
                ' Encrypt the string to an array of bytes.
                encryptedData = EncryptStringToBytes_Aes(originalData, myAes.Key, myAes.IV)
                sourceFileDirectory = Path.GetDirectoryName(tbFilePath.Text)
                sourceFileName = Path.GetFileNameWithoutExtension(tbFilePath.Text)

                File.WriteAllText((sourceFileDirectory + ("\" + (sourceFileName + "_encrypted.txt"))), Convert.ToBase64String(encryptedData))

                If File.Exists((sourceFileDirectory + "\Output.zip")) Then
                    File.Delete((sourceFileDirectory + "\Output.zip"))
                End If

                Dim zipPackage As Package = Package.Open(((sourceFileDirectory + "\Output.zip")), IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite)

                encryptedFileName = Path.GetFileName((sourceFileDirectory + ("\" _
                                + (sourceFileName + "_encrypted.txt"))))
                Dim zipPartUri As Uri = PackUriHelper.CreatePartUri(New Uri(encryptedFileName, UriKind.Relative))
                Dim zipPackagePart As PackagePart = zipPackage.CreatePart(zipPartUri, "", CompressionOption.Normal)
                Dim sourceFileStream As FileStream = New FileStream((sourceFileDirectory + ("\" _
                                + (sourceFileName + "_encrypted.txt"))), FileMode.Open, FileAccess.Read)
                
                Dim destinationFileStream As Stream = zipPackagePart.GetStream

                'Dim contentType As String = Net.Mime.MediaTypeNames.Application.Zip
                Dim zipContent As Byte() = File.ReadAllBytes((sourceFileDirectory + ("\" + (sourceFileName + "_encrypted.txt"))))
                zipPackagePart.GetStream().Write(zipContent, 0, zipContent.Length)

                zipPackage.Close()
                sourceFileStream.Close()

                MessageBox.Show((sourceFileName + ".txt is encrypted and zipped successfully."), "Encrypt Compress", MessageBoxButtons.OK, MessageBoxIcon.Information)
                File.Delete((sourceFileDirectory + ("\" + (sourceFileName + "_encrypted.txt"))))
                Process.Start(sourceFileDirectory)

            Catch exception As Exception
                MessageBox.Show(exception.Message, "Encrypt Compress", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        End If

    End Sub

    Private Shared Function EncryptStringToBytes_Aes(ByVal plainText As String, ByVal key() As Byte, ByVal IV() As Byte) As Byte()
        ' Checking the arguments.
        If (plainText.Length = 0) Then
            Throw New ArgumentNullException("Source file size is ZERO.")
        End If

        If ((key Is Nothing) _
                    OrElse (key.Length = 0)) Then
            Throw New ArgumentNullException("Symmetric key is null.")
        End If

        If ((IV Is Nothing) _
                    OrElse (IV.Length = 0)) Then
            Throw New ArgumentNullException("Initilization Vector is null.")
        End If

        Dim encrypted() As Byte

        Dim aesAlg As AesCryptoServiceProvider = New AesCryptoServiceProvider
        aesAlg.Key = key
        aesAlg.IV = IV

        Dim encryptor As ICryptoTransform = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV)

        Dim msEncrypt As MemoryStream = New MemoryStream
        Dim csEncrypt As CryptoStream = New CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)
        Dim swEncrypt As StreamWriter = New StreamWriter(csEncrypt)
        ' Write all data to the stream.
        swEncrypt.Write(plainText)
        encrypted = msEncrypt.ToArray

        Return encrypted
    End Function

    Private Sub CopyStream(ByVal source As Stream, ByVal target As Stream)
        Const bufferSize As Integer = 4096
        Dim buffer() As Byte = New Byte((bufferSize) - 1) {}
        Dim bytesRead As Integer = 0

        While (source.Read(buffer, 0, bufferSize) > 0)
            target.Write(buffer, 0, bytesRead)

        End While

    End Sub


End Class

