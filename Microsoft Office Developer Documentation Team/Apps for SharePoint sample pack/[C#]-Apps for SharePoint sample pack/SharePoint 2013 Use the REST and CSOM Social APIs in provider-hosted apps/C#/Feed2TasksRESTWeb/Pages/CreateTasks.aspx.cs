using System;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.IdentityModel.S2S.Protocols.OAuth2;

namespace Feed2TasksRESTWeb.Pages
{
    public partial class CreateTasks : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                try
                {

                    /* Save the context token, access token, and web url in hidden fields
                     * These will be needed by subsequent REST calls */
                    hdnContextToken.Value = TokenHelper.GetContextTokenFromRequest(Page.Request);
                    hdnHostWeb.Value = Page.Request["SPHostUrl"];

                    string remoteWebUrl = Request.Url.Authority;
                    SharePointContextToken spContextToken = TokenHelper.ReadAndValidateContextToken(hdnContextToken.Value, remoteWebUrl);

                    Uri hostWebUri = new Uri(hdnHostWeb.Value);
                    string hostWebAuthority = hostWebUri.Authority;
                    OAuth2AccessTokenResponse accessToken = TokenHelper.GetAccessToken(spContextToken, hostWebAuthority);
                    hdnAccessToken.Value = accessToken.AccessToken;

                    //Get the current user and save in hidden fields
                    string endpoint = hdnHostWeb.Value + "/_api/web/currentuser";
                    XNamespace d = "http://schemas.microsoft.com/ado/2007/08/dataservices";
                    XDocument responseDoc = GetDataREST(endpoint);

                    hdnDisplayName.Value = responseDoc.Descendants(d + "Title").First().Value;
                    hdnUserId.Value = responseDoc.Descendants(d + "Id").First().Value;


                    //Show assignment candidates
                    assignmentPosts.DataSource = GetAssignmentCandidates();
                    assignmentPosts.DataBind();
                }
                catch (Exception x)
                {
                    messages.Text = x.Message;
                }
            }

        }

        private List<AssignmentPost> GetAssignmentCandidates()
        {
            try
            {
                /* Get all posts where the current user is mentioned
                 * and are tagged with #Assignment. These are candidates
                 * to become tasks for the current user */

                List<AssignmentPost> posts = new List<AssignmentPost>();

                //Make the request
                string endpoint = hdnHostWeb.Value + "/_api/social.feed/my/MentionFeed";
                XDocument responseDoc = GetDataREST(endpoint);

                //Parse the response
                XNamespace d = "http://schemas.microsoft.com/ado/2007/08/dataservices";

                var assignments = from e in responseDoc.Root.Descendants(d + "Threads").First().Elements(d + "element")
                                  select new
                                  {
                                      Body = e.Element(d + "PostReference").Element(d + "Post").Element(d + "Text").Value,
                                      CreatedDate = DateTime.Parse(e.Element(d + "RootPost").Element(d + "CreatedTime").Value),
                                      Requester = e.Element(d + "Actors").Elements(d + "element").ElementAt(int.Parse(e.Element(d + "PostReference").Element(d + "Post").Element(d + "AuthorIndex").Value)).Element(d + "Name").Value
                                  };


                //Build a collection of assignment candidates
                foreach (var assignment in assignments)
                {
                    if (assignment.Body.Contains("#Assignment"))
                    {
                        AssignmentPost post = new AssignmentPost();
                        post.CreatedDate = assignment.CreatedDate;
                        post.Body = assignment.Body
                            .Replace("#Assignment", string.Empty)
                            .Replace("@" + hdnDisplayName.Value,string.Empty);
                        post.Requester = assignment.Requester;
                        posts.Add(post);
                    }
                }

                return posts;

            }
            catch (Exception x)
            {
                messages.Text = x.Message;
                return new List<AssignmentPost>();
            }
        }

        protected void assignmentPosts_SelectedIndexChanged(object sender, EventArgs e)
        {
            messages.Text = string.Empty;

            try
            {
                /* Create a new task from the selected post */

                CreateNewTask("New Task from " + assignmentPosts.SelectedItem.Cells[3].Text,
                                hdnUserId.Value,
                                assignmentPosts.SelectedItem.Cells[2].Text);

                //confirm
                messages.Text = "New task assigned!";
            }
            catch (Exception x)
            {
                messages.Text = x.Message;
            }
        }

        private string GetFormDigest()
        {
            /* Retrieve a form digest, which is required for creating a new list item */

            string endpoint = hdnHostWeb.Value + "/_api/contextinfo";
            HttpWebRequest digestRequest = (HttpWebRequest)HttpWebRequest.Create(endpoint);
            digestRequest.Headers.Add("Authorization", "Bearer " + hdnAccessToken.Value);
            digestRequest.Method = "POST";
            digestRequest.ContentLength = 0;

            HttpWebResponse digestResponse = (HttpWebResponse)digestRequest.GetResponse();
            XDocument responseDoc = XDocument.Load(digestResponse.GetResponseStream());

            XNamespace d = "http://schemas.microsoft.com/ado/2007/08/dataservices";
            string formDigest = responseDoc.Descendants(d + "FormDigestValue").First().Value;

            return formDigest;
        }

        private XDocument CreateNewTask(string Title, string UserId, string Body)
        {
            /* Post a new task item to the list */

            string endpoint = hdnHostWeb.Value + "/_api/web/lists/getByTitle('Tasks')/items";

            //Message to post
            string itemXML = String.Format(@"
                   <entry xmlns='http://www.w3.org/2005/Atom'
                    xmlns:d='http://schemas.microsoft.com/ado/2007/08/dataservices'
                    xmlns:m='http://schemas.microsoft.com/ado/2007/08/dataservices/metadata'>
                     <category term='SP.Data.TasksListItem'
                      scheme='http://schemas.microsoft.com/ado/2007/08/dataservices/scheme' />
                       <content type='application/xml'>
                         <m:properties>
                           <d:Title>{0}</d:Title>
                           <d:StartDate>{1}</d:StartDate>
                           <d:AssignedToId><d:element m:type='Edm.Int32'>{2}</d:element></d:AssignedToId>
                           <d:Body>{3}</d:Body>
                        </m:properties>
                      </content>
                    </entry>",
                         Title,
                         String.Format("{0}T{1}Z",DateTime.Now.ToString("yyyy-MM-dd"),DateTime.Now.ToString("HH:mm:ss")),
                         UserId,
                         Body);

            //Post it
            HttpWebRequest restRequest = (HttpWebRequest)HttpWebRequest.Create(endpoint);
            restRequest.Headers.Add("Authorization", "Bearer " + hdnAccessToken.Value);
            restRequest.Credentials = CredentialCache.DefaultCredentials;
            restRequest.Method = "POST";
            restRequest.Headers["X-RequestDigest"] = GetFormDigest();
            restRequest.Accept = "application/atom+xml";
            restRequest.ContentType = "application/atom+xml";
            restRequest.ContentLength = itemXML.Length;
            StreamWriter sw = new StreamWriter(restRequest.GetRequestStream());
            sw.Write(itemXML);
            sw.Flush();

            //Get response
            HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();
            StreamReader restStream = new StreamReader(restResponse.GetResponseStream());
            XDocument responseDoc = XDocument.Load(restStream);

            return responseDoc;

        }

        private XDocument GetDataREST(string uri)
        {
            /* Make a RESTful call and return an XDocument */

            //RESTful GET Request
            HttpWebRequest followedRequest = (HttpWebRequest)HttpWebRequest.Create(uri);

            //Attach the Access token
            followedRequest.Headers.Add("Authorization", "Bearer " + hdnAccessToken.Value);

            //Get Response document
            HttpWebResponse followedResponse = (HttpWebResponse)followedRequest.GetResponse();
            StreamReader followedStream = new StreamReader(followedResponse.GetResponseStream());
            XDocument responseDoc = XDocument.Load(followedStream);

            //Return XDocument
            return responseDoc;
        }

    }
}