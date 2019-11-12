using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SP_ProviderApp_WAC_csWeb.Pages
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // The following code gets the client context that represents the host web.
            var contextToken = TokenHelper.GetContextTokenFromRequest(Page.Request);

            // Because this is a provider-hosted app, SharePoint will pass in the host Url in the querystring.
            // Therefore, we'll retrieve it so that we can use it in GetClientContextWithContextToken method call
            var hostWeb = Page.Request["SPHostUrl"];

            // Then we'll build our context, exactly as implemented in the Visual Studio template for provider-hosted apps
            using (var clientContext = TokenHelper.GetClientContextWithContextToken(hostWeb, contextToken, Request.Url.Authority))
            {
                // Now we will use some pretty standard CSOM operations to enumerate the 
                // document libraries in the host web...
                Microsoft.SharePoint.Client.Web hostedWeb = clientContext.Web;
                Microsoft.SharePoint.Client.ListCollection libs = hostedWeb.Lists;
                clientContext.Load(hostedWeb);
                clientContext.Load(libs);
                clientContext.ExecuteQuery();
                var foundFiles = false;
                foreach (Microsoft.SharePoint.Client.List lib in libs)
                {
                    if (lib.BaseType == Microsoft.SharePoint.Client.BaseType.DocumentLibrary)
                    {
                        // ... and for each document library we'll enumerate all the Office files that
                        // may exist in the root folder of each library.
                        Microsoft.SharePoint.Client.Folder folder = lib.RootFolder;
                        Microsoft.SharePoint.Client.FileCollection files = folder.Files;
                        clientContext.Load(folder);
                        clientContext.Load(files);
                        clientContext.ExecuteQuery();
                        foreach (Microsoft.SharePoint.Client.File file in files)
                        {
                            if ((file.ServerRelativeUrl.ToLower().EndsWith(".docx"))
                                || (file.ServerRelativeUrl.ToLower().EndsWith(".xlsx"))
                                || (file.ServerRelativeUrl.ToLower().EndsWith(".pptx"))
                            )
                            {
                                // We know that we have at least one file, so we'll set the foundFiles variable to true
                                foundFiles = true;
                                // Then, for each Office file, we'll build a tile in the UI and set its style to an
                                // appropriate style that we have defined in point8020metro.css.
                                Panel fileItem = new Panel();
                                if (file.ServerRelativeUrl.ToLower().EndsWith(".docx"))
                                {
                                    fileItem.CssClass = "tile tileWord fl";
                                }
                                if (file.ServerRelativeUrl.ToLower().EndsWith(".xlsx"))
                                {
                                    fileItem.CssClass = "tile tileExcel fl";
                                }
                                if (file.ServerRelativeUrl.ToLower().EndsWith(".pptx"))
                                {
                                    fileItem.CssClass = "tile tilePowerPoint fl";
                                }
                                // Then we'll add text to the tile to represent the name of the file
                                fileItem.Controls.Add(new LiteralControl(file.Name));

                                // And now we'll add a custom-styled link for opening the file in 'View' mode
                                // in the Office Web Access Companion
                                HyperLink fileView = new HyperLink();
                                fileView.CssClass = "tileBodyView";
                                fileView.Text = "";
                                fileView.ToolTip = "View in browser";
                                fileView.Target = "_blank";
                                fileView.Width = new Unit(125);
                                fileView.NavigateUrl = hostedWeb.Url + "/_layouts/15/WopiFrame.aspx?sourcedoc=" + file.ServerRelativeUrl + "&action=view&source=" + hostedWeb.Url + file.ServerRelativeUrl;

                                // And finally we'll add a custom-styled link for opening the file in 'Edit' mode
                                // in the Office Web Access Companion
                                HyperLink fileEdit = new HyperLink();
                                fileEdit.CssClass = "tileBodyEdit";
                                fileEdit.Text = "";
                                fileEdit.ToolTip = "Edit in browser";
                                fileEdit.Target = "_blank";
                                fileEdit.Width = new Unit(125);
                                fileEdit.NavigateUrl = hostedWeb.Url + "/_layouts/15/WopiFrame.aspx?sourcedoc=" + file.ServerRelativeUrl + "&action=edit&source=" + hostedWeb.Url + file.ServerRelativeUrl;

                                fileItem.Controls.Add(new LiteralControl("<br/>"));
                                fileItem.Controls.Add(fileView);
                                fileItem.Controls.Add(new LiteralControl("<br/>"));
                                fileItem.Controls.Add(fileEdit);
                                FileList.Controls.Add(fileItem);
                            }
                            
                            
                        }
                    }
                }
                SiteTitle.Text = "Office Web Access: " + hostedWeb.Title;
                // If no videos have been found, build a red tile to inform the user
                if (!foundFiles)
                {
                    LiteralControl noItems = new LiteralControl("<div id='" + Guid.NewGuid()
                                    + "' class='tile tileRed fl'>There are no Office files in the parent Web</div>");
                    FileList.Controls.Add(noItems);
                }
            }
        }
    }
}