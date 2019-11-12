using System;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.IO.Packaging;
using System.Diagnostics;

namespace EncryptCompress
{
    /// <summary>
    /// This class comprises of functions which can encrypt and compress text files.
    /// </summary>
    public partial class EncryptCompressForm : Form
    {
        public EncryptCompressForm()
        {
            InitializeComponent();           
        }          

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            // Select the text file to be encrypted and zipped.
            OpenFileDialog selectFile = new OpenFileDialog();
            selectFile.Filter = "Text Files (*.txt)|*.txt";
            selectFile.ShowDialog();
            if (selectFile.FileName != string.Empty)
            {
                tbFilePath.Text = selectFile.FileName;
            }
        }

        private void btnCompress_Click(object sender, EventArgs e)
        {
            string originalData = string.Empty;
            byte[] encryptedData = null;
            string sourceFileDirectory = string.Empty;
            string sourceFileName = string.Empty;
            string encryptedFileName = string.Empty;

            if (tbFilePath.Text == string.Empty)
            {
                MessageBox.Show("Please choose a file to encrypt and zip.", 
                    "Encrypt Compress", MessageBoxButtons.OK, 
                    MessageBoxIcon.Exclamation);
            }
            else
            {
                try
                {
                    // Reading all the data from the source file.
                    originalData = File.ReadAllText(tbFilePath.Text);
                    // Creating a new instance of the AesCryptoServiceProvider class.
                    // This generates a new key and initialization vector (IV).
                    using (AesCryptoServiceProvider myAes = 
                        new AesCryptoServiceProvider())
                    {
                        // Encrypt the string to an array of bytes.
                        encryptedData = EncryptStringToBytes_Aes(originalData, 
                                        myAes.Key, myAes.IV);
                        sourceFileDirectory = Path.GetDirectoryName(tbFilePath.Text);
                        sourceFileName = Path.GetFileNameWithoutExtension(
                                        tbFilePath.Text);
                        File.WriteAllText(sourceFileDirectory +"\\"+ sourceFileName + 
                            "_encrypted.txt", Convert.ToBase64String(encryptedData));
                    }

                    // Creating the package. (If the package file already exists, 
                    // FileMode.OpenOrCreate will automatically open if the file
                    // exists otherwise a new file will be created. The 'using'
                    // statement insures that 'package' is closed and disposed when
                    // it goes out of scope.)
                    if (File.Exists(sourceFileDirectory + "\\Output.zip"))
                    {
                        File.Delete(sourceFileDirectory + "\\Output.zip");
                    }
                    using (Package zipPackage = ZipPackage.Open(sourceFileDirectory + 
                                                "\\Output.zip", FileMode.OpenOrCreate))
                    {
                        encryptedFileName = Path.GetFileName(sourceFileDirectory + "\\" +
                                            sourceFileName + "_encrypted.txt");
                        Uri zipPartUri = PackUriHelper.CreatePartUri(
                                        new Uri(encryptedFileName, UriKind.Relative));
                        PackagePart zipPackagePart = zipPackage.CreatePart(zipPartUri, 
                                                        "", CompressionOption.Normal);
                        using (FileStream sourceFileStream = new FileStream(
                            sourceFileDirectory + "\\" + sourceFileName + 
                            "_encrypted.txt", FileMode.Open, FileAccess.Read))
                        {
                            using (Stream destinationFileStream = 
                                zipPackagePart.GetStream())
                            {
                                CopyStream(sourceFileStream, destinationFileStream);
                            }
                        }
                        MessageBox.Show(sourceFileName + 
                            ".txt is encrypted and zipped successfully.", 
                            "Encrypt Compress", MessageBoxButtons.OK, 
                            MessageBoxIcon.Information);
                        File.Delete(sourceFileDirectory + "\\" + sourceFileName +
                            "_encrypted.txt");
                        Process.Start(sourceFileDirectory);
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Encrypt Compress", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// This function encrypts the input text to an array of bytes.
        /// </summary>
        /// <param name="plainText">Human readable input text</param>
        /// <param name="key">The symmetric key that is used for encryption</param>
        /// <param name="IV">A random initialization vector</param>
        /// <returns></returns>
        private static byte[] EncryptStringToBytes_Aes(string plainText, 
                                                    byte[] key, byte[] IV)
        {
            // Checking the arguments.
            if (plainText.Length == 0)
                throw new ArgumentNullException("Source file size is zero.");
            if (key == null || key.Length == 0)
                throw new ArgumentNullException("Symmetric key is null.");
            if (IV == null || IV.Length == 0)
                throw new ArgumentNullException("Initilization Vector is null.");
            byte[] encrypted;
            // Creating an AesCryptoServiceProvider object with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = key;
                aesAlg.IV = IV;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(
                                                aesAlg.Key, aesAlg.IV);
                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, 
                                            encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
        
        /// <summary>
        /// This function copies one stream data to another.
        /// </summary>
        /// <param name="source">Input stream</param>
        /// <param name="target">Output stream</param>
        private static void CopyStream(Stream source, Stream target)
        {
            const int bufferSize = 0x1000;
            byte[] buffer = new byte[bufferSize];
            int bytesRead = 0;
            while ((bytesRead = source.Read(buffer, 0, bufferSize)) > 0)
            {
                target.Write(buffer, 0, bytesRead);
            }
        }
    }
}