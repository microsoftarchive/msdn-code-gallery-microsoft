/****************************** Module Header ******************************\
Module Name:  Default.aspx.cs
Project:      CSSQLAzureStoreImages_WebRole
Copyright (c) Microsoft Corporation.

This sample demonstrates how to store images in Windows Azure SQL Server.
Sometimes the developers need to store the files in the Windows Azure. In 
this sample, we introduce two ways to implement this function:
1. Store the image data in SQL Azure. It's easy to search and manage the images.
2. Store the image in the Blob and store the Uri of the Blob in SQL Azure. 
The space of Blob is cheaper. If we can store the image in the Blob and store 
the information of image in SQL Azure, it's also easy to manage the images.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Configuration;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.StorageClient.Protocol;

namespace CSSQLAzureStoreImages_WebRole
{
    public partial class _Default : System.Web.UI.Page
    {
        public ImagesContext imagesDb = new ImagesContext();
        // Store the SelectedValue of the imageLocation
        public String selectedValue = null;

        private const String BLOB = "BLOB";
        private const String SQL = "SQL";

        /// <summary>
        /// Load the Page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    // Make sure that eh container of Blob exists.
                    this.EnsureContainerExists();
                }
                else
                {
                    // Store the SelectedValue of the imageLocation to the variable.
                    this.selectedValue = imageLocation.SelectedValue;
                }

                // Get the images.
                this.RefreshGallery();
            }
            catch (System.Net.WebException we)
            {
                this.status.Text = "Network error: " + we.Message;
            }
            catch (StorageException se)
            {
                Console.WriteLine("Storage service error: " + se.Message);
            }

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (RoleEnvironment.IsAvailable)
            {
                ListItem[] source = new[]{
                    new ListItem{Text="Azure Blob Service",Value=BLOB},
                    new ListItem{Text="Azure SQL Database",Value=SQL}};
                this.imageLocation.Items.Clear();
                this.imageLocation.Items.AddRange(source);

                this.imageLocation.SelectedValue = this.selectedValue;
            }
        }

        protected void Upload_Click(object sender, EventArgs e)
        {
            if (this.imageFile.HasFile)
            {
                this.status.Text = "Inserted [" + this.imageFile.FileName + "] - Content Type [" + this.imageFile.PostedFile.ContentType + "] - Length [" + this.imageFile.PostedFile.ContentLength + "]";

                this.SaveImage(
                  this.imageName.Text,
                  this.imageDescription.Text,
                  this.imageFile.FileName,
                  this.imageFile.PostedFile.ContentType,
                  this.imageFile.FileBytes);

                this.RefreshGallery();
            }
            else
            {
                this.status.Text = "No image file";
            }

        }

        /// <summary>
        /// Set the uri of images.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                Image img = e.Item.FindControl("img") as Image;

                // According to the choose of location, we set the uri of image.
                if (String.IsNullOrEmpty(this.selectedValue) || this.selectedValue.Equals(BLOB))
                {
                    ImageInBlob image = e.Item.DataItem as ImageInBlob;
                    img.ImageUrl = image.BlobUri;
                }
                else if (this.selectedValue.Equals(SQL))
                {
                    ImageInSQLAzure image = e.Item.DataItem as ImageInSQLAzure;
                    img.ImageUrl = "GetImage.ashx?ImageId=" + image.ImageId;
                }

            }

        }

        /// <summary>
        /// Delete an image by ImageId.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnDeleteImage(object sender, CommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "Delete")
                {
                    Int32 imageId = Int32.Parse(e.CommandArgument as String);

                    if (String.IsNullOrEmpty(this.selectedValue) || this.selectedValue.Equals(BLOB))
                    {
                        ImageInBlob deletedImage = (from i in imagesDb.BlobImages
                                                    where i.ImageId == imageId
                                                    select i).FirstOrDefault();
                        if (deletedImage != null)
                        {
                            // Delete an blob by uri.
                            var blob = this.GetContainer().GetBlobReference(deletedImage.BlobUri);
                            blob.DeleteIfExists();

                            imagesDb.BlobImages.Remove(deletedImage);
                            imagesDb.SaveChanges();
                        }
                    }
                    else if (this.selectedValue.Equals(SQL))
                    {
                        ImagesTable deletedImage = (from i in imagesDb.ImagesTable
                                                    where i.ImageId == imageId
                                                    select i).FirstOrDefault();

                        if (deletedImage != null)
                        {
                            ImageInSQLAzure deletedImageInfo = deletedImage.ImageInfo;

                            imagesDb.SQLAzureImages.Remove(deletedImageInfo);
                            imagesDb.ImagesTable.Remove(deletedImage);
                            imagesDb.SaveChanges();
                        }
                    }
                }
            }
            catch (StorageClientException se)
            {
                this.status.Text = "Storage client error: " + se.Message;
            }
            catch (Exception)
            {
            }

            this.RefreshGallery();

        }

        /// <summary>
        /// Copy an image by ImageId
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnCopyImage(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "Copy")
            {
                Int32 imageId = Int32.Parse(e.CommandArgument as String);
                if (String.IsNullOrEmpty(this.selectedValue) || this.selectedValue.Equals(BLOB))
                {
                    ImageInBlob copiedImage = (from i in imagesDb.BlobImages
                                               where i.ImageId == imageId
                                               select i).FirstOrDefault();
                    if (copiedImage != null)
                    {
                        var srcBlob = this.GetContainer().GetBlobReference(copiedImage.BlobUri);

                        String newImageName = "Copy of \"" + copiedImage.ImageName + "\"";
                        var newBlob = this.GetContainer().GetBlobReference(Guid.NewGuid().ToString());

                        // Copy content from source blob
                        newBlob.CopyFromBlob(srcBlob);

                        // Copy the info of image.
                        ImageInBlob newImage = new ImageInBlob();
                        newImage.FileName = copiedImage.FileName;
                        newImage.ImageName = newImageName;
                        newImage.Description = copiedImage.Description;
                        newImage.BlobUri = newBlob.Uri.ToString();

                        imagesDb.BlobImages.Add(newImage);
                        imagesDb.SaveChanges();

                        this.RefreshGallery();
                    }
                }
                else if (this.selectedValue.Equals(SQL))
                {
                    ImagesTable copiedImage = (from i in imagesDb.ImagesTable
                                               where i.ImageId == imageId
                                               select i).FirstOrDefault();

                    if (copiedImage != null)
                    {
                        ImageInSQLAzure copiedImageInfo = copiedImage.ImageInfo;
                        ImagesTable newImage = new ImagesTable();
                        ImageInSQLAzure newImageInfo = new ImageInSQLAzure();

                        // Copy the info of image.
                        newImageInfo.FileName = copiedImageInfo.FileName;
                        newImageInfo.ImageName = "Copy of \"" + copiedImageInfo.ImageName + "\"";
                        newImageInfo.Description = copiedImageInfo.Description;

                        newImage.ImageData = copiedImage.ImageData;
                        newImage.ImageInfo = newImageInfo;

                        imagesDb.SQLAzureImages.Add(newImageInfo);
                        imagesDb.ImagesTable.Add(newImage);
                        imagesDb.SaveChanges();

                        this.RefreshGallery();
                    }
                }
            }

        }

        /// <summary>
        /// Make sure that the Container exits. If it's not, create one.
        /// </summary>
        private void EnsureContainerExists()
        {
            var container = this.GetContainer();
            container.CreateIfNotExist();

            var permissions = container.GetPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Container;
            container.SetPermissions(permissions);
        }

        private CloudBlobContainer GetContainer()
        {
            // Get a handle on account, create a blob service client and get container proxy
            var account = CloudStorageAccount.FromConfigurationSetting("StorageConnectionString");
            var client = account.CreateCloudBlobClient();

            return client.GetContainerReference(RoleEnvironment.GetConfigurationSettingValue("StorageName"));
        }


        /// <summary>
        /// Get the images by the choose of locations
        /// </summary>
        private void RefreshGallery()
        {
            if (String.IsNullOrEmpty(this.selectedValue) || this.selectedValue.Equals(BLOB))
            {
                this.images.DataSource = imagesDb.BlobImages.ToList();
            }
            else if (this.selectedValue.Equals(SQL))
            {
                this.images.DataSource = imagesDb.SQLAzureImages.ToList();
            }

            this.images.DataBind();
        }

        /// <summary>
        /// Save the images to the location
        /// </summary>
        /// <param name="name">It's the name that user inputs.</param>
        /// <param name="description">It's the description that user inputs.</param>
        /// <param name="fileName">It's the name of the file.</param>
        /// <param name="contentType">It's the type of the file.</param>
        /// <param name="data">It's the content of the file</param>
        private void SaveImage(string name, string description, string fileName, string contentType, byte[] data)
        {
            // Store the images to the Blob and SQL Azure by the choose of the location.
            if (this.selectedValue.Equals(BLOB))
            {
                name = string.IsNullOrEmpty(name) ? "unknown" : name;

                var blob = this.GetContainer().GetBlobReference(name);

                blob.Properties.ContentType = contentType;

                ImageInBlob newImage = new ImageInBlob();
                newImage.FileName = fileName;
                newImage.ImageName = name;
                newImage.Description = string.IsNullOrEmpty(description) ? "unknown" : description;

                blob.UploadByteArray(data);
                newImage.BlobUri = blob.Uri.ToString();

                imagesDb.BlobImages.Add(newImage);
                imagesDb.SaveChanges();
            }
            else if (this.selectedValue.Equals(SQL))
            {
                ImageInSQLAzure newImageInfo = new ImageInSQLAzure();
                ImagesTable newImage = new ImagesTable();

                newImageInfo.FileName = fileName;
                newImageInfo.ImageName = string.IsNullOrEmpty(name) ? "unknown" : name;
                newImageInfo.Description = string.IsNullOrEmpty(description) ? "unknown" : description;

                newImage.ImageInfo = newImageInfo;
                newImage.ImageData = data;

                imagesDb.SQLAzureImages.Add(newImageInfo);
                imagesDb.ImagesTable.Add(newImage);
                imagesDb.SaveChanges();
            }
        }
    }
}
