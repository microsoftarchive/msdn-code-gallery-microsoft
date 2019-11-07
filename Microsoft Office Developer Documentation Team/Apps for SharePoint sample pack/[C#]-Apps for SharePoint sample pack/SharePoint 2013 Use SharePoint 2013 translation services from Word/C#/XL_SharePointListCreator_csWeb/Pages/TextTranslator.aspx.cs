using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// using statments that make our code below easier to read/write
using Microsoft.SharePoint.Client;
using Microsoft.Office.Client.TranslationServices;

// using statements for Office OpenXml
// NOTE: This project includes a reference to the OpenXml SDK 2.5
// You will need to download the SDK from: http://www.microsoft.com/en-us/download/details.aspx?id=30425
// Also note that you must add a reference to WindowsBase for the packaging to work.
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

// using statement that enables us to use FileStream objects easily
using System.IO;


namespace WD_SharePointTranslation_csWeb.Pages
{
    public partial class TextTranslator : System.Web.UI.Page
    {

        // Variables used in events and to control the actual page content
        public string connectedSiteUrl = string.Empty;
        public string accessToken = string.Empty;
        public string refreshToken = string.Empty;
        public bool success = false;
        public Exception exception = null;
        public string targetLibrary = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Read the data that has been posted
            connectedSiteUrl = Request.Form["connectedSiteUrl"];
            accessToken = Request.Form["accessToken"];
            refreshToken = Request.Form["refreshToken"];
            
            // Note that the following form fields were set dynamically by JavaScript
            // to represent the data from the document to be translated and the target language
            string documentContent = string.Empty;
            string targetLanguage = string.Empty;
            documentContent = Request.Form["documentContent"];
            targetLanguage = Request.Form["documentLanguage"];
            try
            {
                using (ClientContext context = TokenHelper.GetClientContextWithAccessToken(connectedSiteUrl, accessToken))
                {
                    // Use standard CSOM and OOXML approaches for creating a file
                    Web thisWeb = context.Web;
                    List docLib = thisWeb.Lists.GetByTitle("Documents");
                    Folder rootFolder = docLib.RootFolder;
                    context.Load(thisWeb);
                    context.Load(docLib);
                    context.Load(rootFolder);
                    context.ExecuteQuery();
                    FileStream fs = null;
                    try
                    {

                        // We'll build a Word Document by using OOXML 
                        // Note that we'll first create it in a folder in this Web app. 
                        using (WordprocessingDocument wordDocument =
                            WordprocessingDocument.Create(Server.MapPath("~/TempOOXML/SourceDocument.docx"),
                            WordprocessingDocumentType.Document))
                        {

                            // Add a main document part. 
                            MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                            // Create the document structure.
                            mainPart.Document = new Document();
                            Body body = mainPart.Document.AppendChild(new Body());

                            // Create a paragraph based on the text that was posted 
                            // in Request.Form["documentContent"]
                            Paragraph para = body.AppendChild(new Paragraph());
                            Run run = para.AppendChild(new Run());
                            run.AppendChild(new Text(documentContent));

                        }

                        // At this stage, the local file has been created in the folder of this Web project
                        // so we'll now read it and create a new file in SharePoint, based on this local file.
                        byte[] documentBytes;
                        fs = System.IO.File.OpenRead(Server.MapPath("~/TempOOXML/SourceDocument.docx"));
                        documentBytes = new byte[fs.Length];
                        fs.Read(documentBytes, 0, Convert.ToInt32(fs.Length));


                        // At this stage, the file contents of the OOXML document has been read into the byte array 
                        // so we can use that as the content of a new file in SharePoint. 
                        Microsoft.SharePoint.Client.FileCreationInformation ooxmlFile
                            = new Microsoft.SharePoint.Client.FileCreationInformation();
                        ooxmlFile.Overwrite = true;
                        ooxmlFile.Url = thisWeb.Url 
                            + rootFolder.ServerRelativeUrl 
                            + "/SharePointSourceDocument.docx";
                        ooxmlFile.Content = documentBytes;
                        Microsoft.SharePoint.Client.File newFile = rootFolder.Files.Add(ooxmlFile);
                        context.Load(newFile);
                        context.ExecuteQuery();
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        // Tell the user what went wrong. These variables will be used
                        // to report the error to the user in the TextTranslator.aspx page.
                        success = false;
                        exception = ex;
                        
                    }
                    finally
                    {
                        // Clean up our filestream object
                        fs.Close();
                    }


                    // Do the actual translation work. Note that we use a synchronous translation
                    // approach here, but you could also use the TranslationJob object to
                    // perform an asynchronous translation.
                    if (success)
                    {
                        try
                        {
                            SyncTranslator job = new SyncTranslator(context, targetLanguage);
                            job.OutputSaveBehavior = SaveBehavior.AlwaysOverwrite;
                            job.Translate(
                                thisWeb.Url + rootFolder.ServerRelativeUrl + "/SharePointSourceDocument.docx",
                                thisWeb.Url + rootFolder.ServerRelativeUrl + "/" + targetLanguage + "_Document.docx");
                            context.ExecuteQuery();
                            targetLibrary = thisWeb.Url + rootFolder.ServerRelativeUrl;
                        }
                        catch (Exception ex)
                        {
                            // Tell the user what went wrong. These variables will be used
                            // to report the error to the user in the TextTranslator.aspx page.
                            success = false;
                            exception = ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Tell the user what went wrong. These variables will be used
                // to report the error to the user in the TextTranslator.aspx page.
                success = false;
                exception = ex;
            }
        }

    }
}