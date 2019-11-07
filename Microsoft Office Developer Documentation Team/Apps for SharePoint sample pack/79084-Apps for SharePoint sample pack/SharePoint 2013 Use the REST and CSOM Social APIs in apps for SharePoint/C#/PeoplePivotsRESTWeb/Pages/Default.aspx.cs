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

namespace PeoplePivotsRESTWeb.Pages
{
    public partial class Default : System.Web.UI.Page
    {
        private enum SocialUserType
        {
            Follower,
            Followed
        }

        private enum SocialUserFilter
        {
            All,
            Week,
            Liked
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            /* Build the display when the app loads */
            if (!Page.IsPostBack)
            {
                try
                {

                    //Save the context token, access token, and web url in hidden fields
                    hdnContextToken.Value = TokenHelper.GetContextTokenFromRequest(Page.Request);
                    hdnHostWeb.Value = Page.Request["SPHostUrl"];

                    string remoteWebUrl = Request.Url.Authority;
                    SharePointContextToken spContextToken = TokenHelper.ReadAndValidateContextToken(hdnContextToken.Value, remoteWebUrl);

                    Uri hostWebUri = new Uri(hdnHostWeb.Value);
                    string hostWebAuthority = hostWebUri.Authority;
                    OAuth2AccessTokenResponse accessToken = TokenHelper.GetAccessToken(spContextToken, hostWebAuthority);
                    hdnAccessToken.Value = accessToken.AccessToken;

                    //bind the followers and followed for display
                    followersImages.DataSource = LoadSocialUsers(SocialUserType.Follower, SocialUserFilter.All);
                    followersImages.DataBind();
                    followedImages.DataSource = LoadSocialUsers(SocialUserType.Followed, SocialUserFilter.All);
                    followedImages.DataBind();

                }
                catch (Exception x)
                {
                    messages.Text = "Exception in Page_Load: " + x.Message;
                }

            }
        }

        protected void feedFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                try
                {
                    messages.Text = string.Empty;

                    SocialUserFilter filter = SocialUserFilter.All;
                    if (feedFilters.SelectedValue == "1") filter = SocialUserFilter.Week;
                    if (feedFilters.SelectedValue == "2") filter = SocialUserFilter.Liked;

                    //Apply the selected filter
                    followersImages.DataSource = LoadSocialUsers(SocialUserType.Follower, filter);
                    followersImages.DataBind();
                    followedImages.DataSource = LoadSocialUsers(SocialUserType.Followed, filter);
                    followedImages.DataBind();
                }
                catch (Exception x)
                {
                    messages.Text = "Exception in feedFilters_SelectedIndexChanged: " + x.Message;
                }
            }

        }

        private List<SocialUser> LoadSocialUsers(SocialUserType type, SocialUserFilter filter)
        {
            List<SocialUser> users = new List<SocialUser>();

            //Create the endpoint for the REST call
            string endpoint;
            if (type == SocialUserType.Followed)
                endpoint = hdnHostWeb.Value + "/_api/social.following/my/Followed(types=1)";
            else
                endpoint = hdnHostWeb.Value + "/_api/social.following/my/Followers";

            //Make the request
            XDocument responseDoc = GetDataREST(endpoint);

            //Parse the response
            XNamespace d = "http://schemas.microsoft.com/ado/2007/08/dataservices";
            XNamespace m = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
            XNamespace georss = "http://www.georss.org/georss";
            XNamespace gml = "http://www.opengis.net/gml";

            var followedUsers = from e in responseDoc.Root.Descendants(d + "element")
                                select new
                                {
                                    AccountName = e.Element(d + "AccountName").Value.Split('|')[2],
                                    Name = e.Element(d + "Name").Value,
                                    ImageUrl = e.Element(d + "ImageUri").Value
                                };

            //Build a collection of users
            foreach (var followedUser in followedUsers)
            {
                SocialUser user = new SocialUser();
                user.AccountName = followedUser.AccountName;
                user.Name = followedUser.Name;
                user.ImageUrl = followedUser.ImageUrl;
                users.Add(user);
            }

            //Filter the users
            return FilterFollowed(users, filter);

        }

        private List<SocialUser> FilterFollowed(List<SocialUser> users, SocialUserFilter filter)
        {
            List<SocialUser> includedUsers = new List<SocialUser>();

            foreach (SocialUser user in users)
            {
                bool include = false;
                List<SocialPost> posts = GetPosts(user.AccountName);

                switch (filter)
                {
                    case SocialUserFilter.All:
                        include = true;
                        break;

                    case SocialUserFilter.Week:
                        foreach (SocialPost post in posts)
                        {
                            if (post.CreatedDate.AddDays(7) >= DateTime.Today)
                            {
                                include = true;
                                break;
                            }
                        }
                        break;

                    case SocialUserFilter.Liked:
                        foreach (SocialPost post in posts)
                        {
                            if (post.LikedByMe)
                            {
                                include = true;
                                break;
                            }
                        }
                        break;

                }

                if (include)
                {
                    SocialUser includedUser = new SocialUser();
                    includedUser.AccountName = user.AccountName;
                    includedUser.ImageUrl = user.ImageUrl;
                    includedUser.Name = user.Name;
                    includedUsers.Add(user);
                }
            }

            return includedUsers;

        }

        protected void ShowFeedActivity(object source, RepeaterCommandEventArgs e)
        {
            ImageButton imageButton = e.Item.FindControl("followedImage") as ImageButton;
            if (imageButton == null)
                imageButton = e.Item.FindControl("followersImage") as ImageButton;

            userFeedName.Text = imageButton.ToolTip;
            List<SocialPost> posts = GetPosts(imageButton.AlternateText);
            feedGrid.DataSource = posts;
            feedGrid.DataBind();

        }

        private List<SocialPost> GetPosts(string accountName)
        {
            try
            {

                List<SocialPost> posts = new List<SocialPost>();

                //Make the request
                string endpoint = hdnHostWeb.Value + "/_api/social.feed/actor(item='" + accountName + "')/Feed";
                XDocument responseDoc = GetDataREST(endpoint);

                //Parse the response
                XNamespace d = "http://schemas.microsoft.com/ado/2007/08/dataservices";
                XNamespace m = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
                XNamespace georss = "http://www.georss.org/georss";
                XNamespace gml = "http://www.opengis.net/gml";

                var personalEntries = from e in responseDoc.Root.Descendants(d + "RootPost")
                                      select new
                                      {
                                          Body = e.Element(d + "Text").Value,
                                          CreatedDate = DateTime.Parse(e.Element(d + "CreatedTime").Value),
                                          LikedByMe = bool.Parse(e.Element(d + "LikerInfo").Element(d + "IncludesCurrentUser").Value)
                                      };

                //Build a collection of posts
                foreach (var personalEntry in personalEntries)
                {
                    SocialPost post = new SocialPost();
                    post.CreatedDate = personalEntry.CreatedDate;
                    post.Body = personalEntry.Body;
                    post.LikedByMe = personalEntry.LikedByMe;
                    posts.Add(post);
                }

                return posts;

            }
            catch
            {
                return new List<SocialPost>();
            }
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