using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


// using statements for Office OpenXml
// NOTE: This project includes a reference to the OpenXml SDK 2.5
// You will need to download the SDK from: http://www.microsoft.com/en-us/download/details.aspx?id=30425
// Also note that you must add a reference to WindowsBase for the packaging to work.
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

// using statement that enables us to use FileStream objects easily
using System.IO;


namespace SP_Autohosted_OOXML_csWeb.Pages
{
    public partial class Default : System.Web.UI.Page
    {
        // Variables that we will use in our CSOM code
        static Microsoft.SharePoint.Client.ClientContext clientContext;
        static Microsoft.SharePoint.Client.Web hostingWeb;
        static Microsoft.SharePoint.Client.List targetLibrary;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // The following code gets the client context that represents the host web.
                string contextToken = TokenHelper.GetContextTokenFromRequest(Page.Request);

                // Because this is an Autohosted App, SharePoint will pass in the host Url in the querystring.
                // Therefore, we'll retrieve it so that we can use it in GetClientContextWithContextToken method call
                string hostWeb = Page.Request["SPHostUrl"];

                // Then we'll build our context, exactly as implemented in the Visual Studio template for Autohosted apps
                clientContext = TokenHelper.GetClientContextWithContextToken(hostWeb, contextToken, Request.Url.Authority);
                
                    // Now we will use some pretty standard CSOM operations to enumerate the 
                    // document libraries in the host web...
                    hostingWeb = clientContext.Web;
                    Microsoft.SharePoint.Client.ListCollection libs = hostingWeb.Lists;
                    clientContext.Load(hostingWeb);
                    clientContext.Load(libs);
                    clientContext.ExecuteQuery();
                    bool foundLibrary = false;
                    foreach (Microsoft.SharePoint.Client.List lib in libs)
                    {
                        if (lib.BaseType == Microsoft.SharePoint.Client.BaseType.DocumentLibrary)
                        {
                            // We know that we have at least one library,
                            // so we'll set the foundLibrary variable to true...
                            foundLibrary = true;
                            // ... and add the library title to the dropdown list on the page
                            OutputLibrary.Items.Add(lib.Title);
                            CreateDocumentLink.CssClass = "tile tileOrange";
                            CreateDocumentLink.Text = "Click here to create a document\nin the selected library";
                            CreateDocumentLink.Enabled = true;
                        }
                    }
                    SiteTitle.Text = "Office Open XML (OOXML) Document Creator: " + hostingWeb.Title;
                    // If no libraries have been found, inform the user
                    if (!foundLibrary)
                    {
                        CreateDocumentLink.CssClass = "tile tileRed";
                        CreateDocumentLink.Text = "There are no libraries in the host Web.";
                        CreateDocumentLink.Enabled = false;
                    }
                
            }
        }

        protected void CreateDocumentLink_Click(object sender, EventArgs e)
        {
            FileStream fs = null;
            try
            {

                // When the user has selected a library, they will be allowed to click the button
                // The first thing we'll do is get the target library and its root folder.
                targetLibrary = hostingWeb.Lists.GetByTitle(OutputLibrary.SelectedItem.Text);
                Microsoft.SharePoint.Client.Folder destintationFolder = targetLibrary.RootFolder;
                clientContext.Load(destintationFolder);
                clientContext.ExecuteQuery();

                // Then we'll build a Word Document by using OOXML 
                // Note that we'll first create it in a folder in this Web app. 
                using (WordprocessingDocument wordDocument =
                    WordprocessingDocument.Create(Server.MapPath("~/SampleOOXML/LocalOOXMLDocument.docx"),
                    WordprocessingDocumentType.Document))
                {

                    // Add a main document part. 
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                    // Create the document structure.
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    // Create a paragraph.
                    Paragraph para = body.AppendChild(new Paragraph());
                    Run run = para.AppendChild(new Run());
                    run.AppendChild(new Text("Here's some text in a paragraph"));

                    // Create a table.
                    DocumentFormat.OpenXml.Wordprocessing.Table table
                        = new DocumentFormat.OpenXml.Wordprocessing.Table();

                    // Create some table border settings.
                    TableProperties borderProperties = new TableProperties(
                        new TableBorders(
                        new TopBorder
                        {
                            Val = new EnumValue<BorderValues>(BorderValues.DashDotStroked),
                            Size = 12
                        },
                        new BottomBorder
                        {
                            Val = new EnumValue<BorderValues>(BorderValues.DashDotStroked),
                            Size = 12
                        },
                        new LeftBorder
                        {
                            Val = new EnumValue<BorderValues>(BorderValues.DashDotStroked),
                            Size = 12
                        },
                        new RightBorder
                        {
                            Val = new EnumValue<BorderValues>(BorderValues.DashDotStroked),
                            Size = 12
                        },
                        new InsideHorizontalBorder
                        {
                            Val = new EnumValue<BorderValues>(BorderValues.Single),
                            Size = 12
                        },
                        new InsideVerticalBorder
                        {
                            Val = new EnumValue<BorderValues>(BorderValues.Single),
                            Size = 12
                        }));

                    // Add the table border settings to the table.
                    table.AppendChild<TableProperties>(borderProperties);

                    // Create a table row and add two cells with some text
                    var tr = new DocumentFormat.OpenXml.Wordprocessing.TableRow();
                    var tc1 = new DocumentFormat.OpenXml.Wordprocessing.TableCell();
                    tc1.Append(new Paragraph(new Run(new Text("Here's some text in table cell #1"))));
                    var tc2 = new DocumentFormat.OpenXml.Wordprocessing.TableCell();
                    tc2.Append(new Paragraph(new Run(new Text("Here's some text in table cell #2"))));
                    tr.Append(tc1);
                    tr.Append(tc2);

                    // Add the row to the table, and the table to the body of the document.
                    table.Append(tr);
                    body.Append(table);
                }

                // At this stage, the local file has been created in the folder of this Web project
                // so we'll now read it and create a new file in SharePoint, based on this local file.
                byte[] documentBytes;
                fs = File.OpenRead(Server.MapPath("~/SampleOOXML/LocalOOXMLDocument.docx"));
                documentBytes = new byte[fs.Length];
                fs.Read(documentBytes, 0, Convert.ToInt32(fs.Length));
                

                // At this stage, the file contents of the OOXML document has been read into the byte array 
                // so we can use that as the content of a new file in SharePoint. 
                Microsoft.SharePoint.Client.FileCreationInformation ooxmlFile
                    = new Microsoft.SharePoint.Client.FileCreationInformation();
                ooxmlFile.Overwrite = true;
                ooxmlFile.Url = hostingWeb.Url
                    + destintationFolder.ServerRelativeUrl
                    + "/SharePointOOXMLDocument.docx";
                ooxmlFile.Content = documentBytes;
                Microsoft.SharePoint.Client.File newFile = targetLibrary.RootFolder.Files.Add(ooxmlFile);
                clientContext.Load(newFile);
                clientContext.ExecuteQuery();

                // Let the user navigate to the document library where the file has been created
                string targetUrl = hostingWeb.Url + destintationFolder.ServerRelativeUrl;
                DocumentLink.Text = "Document has been created in SharePoint! Click here to view the library";
                DocumentLink.Visible = true;
                DocumentLink.NavigateUrl = targetUrl;
            }
            catch (Exception ex)
            {
                // Tell the user what went wrong
                DocumentLink.Text = "An error has occurred: " + ex.Message;
                DocumentLink.Visible = true;
                DocumentLink.NavigateUrl = "";
            }
            finally
            {
                // Clean up our filestream object
                fs.Close();
            }
        }
    }
}