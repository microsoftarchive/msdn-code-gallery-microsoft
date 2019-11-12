using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
// using statements added for convenience when using
// CSOM objects
using SP = Microsoft.SharePoint.Client;
using SPSocial = Microsoft.SharePoint.Client.Social;


namespace SP_Claims_SPO_Social_csWeb.Pages
{
    public partial class Default : System.Web.UI.Page
    {
        // Variables we will use in various functions
        static SP.ClientContext clientContext;
        static List<string> postIds = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            // The following code gets the client context by using TokenHelper.
            if (!Page.IsPostBack)
            {
                SPSocial.SocialFeedManager feedMngr;
                string contextToken;
                string hostWeb;

                // The following code handles authentication/authorization against our SPO tenant
                // because this page is not running in SharePoint. The important thing for us is to
                // be able to get the ClientContext object.
                // NOTE: The TokenHelper class is the one provided by the Visual Studio template for 
                // autohosted Apps, so we haven't modified it's implementation in any way.
                HttpRequest req;
                req = Page.Request;
                contextToken = TokenHelper.GetContextTokenFromRequest(req);
                hostWeb = Page.Request["SPHostUrl"];
                clientContext = TokenHelper.GetClientContextWithContextToken(hostWeb, contextToken, Request.Url.Authority);
                
                // Now we are into the useful bits that enable us to work with feeds, posts, replies, and so on.
                feedMngr = new SPSocial.SocialFeedManager(clientContext);
                clientContext.Load(feedMngr);
                clientContext.ExecuteQuery();
                
                // Call our own function (see below) to render the three most recent posts as tiles.
                LoadPosts();
            }
        }

        // This function contains the logic for rendereing the three most recent posts
        private void LoadPosts()
        {
            try
            {
                // The SocialFeedManager class is what will enable us to work with feeds, posts, replies, and so on.
                SPSocial.SocialFeedManager feedMngr;
                feedMngr = new SPSocial.SocialFeedManager(clientContext);

                // Clear any current tiles from the UI.
                TimelinePanel.Controls.Clear();
                
                // psotIds is a List variable that will contain the IDs for up to three of the most resent posts.
                // It's used if the user chooses to 'Reply All', so we need to clear it here and then repopulate it 
                // everytime we refresh the UI.
                postIds.Clear();

                // Create an SocialFeedOptions object that we will use when we retrieve a feed.
                // Note that we are only retrieving the three most recent posts, simply so that
                // they will fit in our UI.
                SPSocial.SocialFeedOptions feedOptions = new SPSocial.SocialFeedOptions();
                feedOptions.MaxThreadCount = 3;
                feedOptions.SortOrder = SPSocial.SocialFeedSortOrder.ByCreatedTime;

                // Now we can use the SocialFeedOptions to get the timeline feed for the current user 
                SP.ClientResult<SPSocial.SocialFeed> timelineFeed = feedMngr.GetFeed(SPSocial.SocialFeedType.Timeline, feedOptions);


                // Load the feedMngr object and execute the previously-batched statements.
                
                clientContext.Load(feedMngr);
                clientContext.ExecuteQuery();

                // The value of the timelineFeed object is a ClientResult<SocialFeed>
                // so the first thing we'll do is cast it as an actual SocialFeed object
                SPSocial.SocialFeed feed = (SPSocial.SocialFeed)(timelineFeed.Value);
                
                // Then we'll iterate through the thread in the feed and obtain useful properties
                // for display in our tiles.
                foreach (SPSocial.SocialThread thread in feed.Threads)
                {
                    SPSocial.SocialActor[] actors = thread.Actors;
                    SPSocial.SocialPost post = thread.RootPost;
                    string authorName = actors[post.AuthorIndex].Name;
                    string postContent = post.Text;
                    int totalReplies = thread.TotalReplyCount;
                    Panel threadTile = new Panel();
                    threadTile.CssClass = "tile tileOrange fl";
                    threadTile.Controls.Add(new LiteralControl(authorName
                        + "<br/>("
                        + totalReplies.ToString()
                        + " replies)"));
                    Panel threadBody = new Panel();
                    threadBody.CssClass = "tileBody";
                    threadBody.Controls.Add(new LiteralControl(postContent));
                    threadTile.Controls.Add(threadBody);
                    
                    // Show the two most recent replies in the tile, if there are any.
                    // Note: By default, only the two most recent replies are returned. 
                    // To get all replies, you could call the SocialFeedManager.GetFullThread
                    // method before you load it and execute it.
                    if (totalReplies > 0)
                    {
                        SPSocial.SocialPost[] replies = thread.Replies;
                        foreach (SPSocial.SocialPost reply in replies)
                        {
                            Panel threadReply = new Panel();
                            threadReply.CssClass = "tileBody noPad";
                            threadReply.Style.Add("margin-left", "20px");
                            threadReply.Controls.Add(new LiteralControl(reply.Text));
                            threadTile.Controls.Add(threadReply);
                        }
                    }
                    TimelinePanel.Controls.Add(threadTile);

                    // Add the post IDs to the List object. This will be used if the user chooses to 'Reply All'
                    postIds.Add(post.Id);
                   
                }
            }
            catch (Exception ex)
            {
                errLabel.Text = ex.Message;
            }
        }

       
        // This function handles the click of the [Reply All] button.
        // Effectively it takes the text from the text box, and uses that
        // to reply to the posts with IDs that are contained in the postIds
        // list. That is, the three most recent posts as currently displayed
        // as tiles in the UI.
        protected void ReplyNow_Click(object sender, EventArgs e)
        {
            if (ReplyText.Text == string.Empty)
            { 
                LoadPosts();
                errLabel.Text = "Please enter some text";
                return;
            }
            try
            {
                SPSocial.SocialFeedManager feedMngr;
                feedMngr = new SPSocial.SocialFeedManager(clientContext);
                foreach (string postId in postIds)
                {
                    var postCreationData = new SPSocial.SocialPostCreationData();
                    postCreationData.ContentText = ReplyText.Text;
                    // Publish the reply. Note that the postId is the identifier of the post that we
                    // want to reply to.
                    feedMngr.CreatePost(postId, postCreationData);
                    clientContext.ExecuteQuery();
                }
            }
            finally
            {
                errLabel.Text = "";
                // Update the tiles so that our replies are rendered for each post.
                LoadPosts();
                ReplyText.Text = "";
            }
        }

        // This function handles the click of the [Post New] button.
        // Effectively it takes the text from the text box, and uses that
        // to create a new post. This is similar to the [Reply All] button,
        // with one key difference --- in the CreatePost method we pass in 
        // null as we are not replying to an existing post. This effectively
        // instructs SharePoint to create a new post.
        protected void PostNew_Click(object sender, EventArgs e)
        {
            if (ReplyText.Text == string.Empty)
            {
                LoadPosts();
                errLabel.Text = "Please enter some text";
                return;
            }
            try
            {
                SPSocial.SocialFeedManager feedMngr;
                feedMngr = new SPSocial.SocialFeedManager(clientContext);
                var postCreationData = new SPSocial.SocialPostCreationData();
                postCreationData.ContentText = ReplyText.Text;
                // Publish the new post. This is where we'll use null so that a new post is created.
                feedMngr.CreatePost(null, postCreationData);
                clientContext.ExecuteQuery();

            }
            finally
            {   
                errLabel.Text = "";
                // Update the tiles so that our newest post is rendered.
                LoadPosts();
                ReplyText.Text = "";
            }
        }

    }
}