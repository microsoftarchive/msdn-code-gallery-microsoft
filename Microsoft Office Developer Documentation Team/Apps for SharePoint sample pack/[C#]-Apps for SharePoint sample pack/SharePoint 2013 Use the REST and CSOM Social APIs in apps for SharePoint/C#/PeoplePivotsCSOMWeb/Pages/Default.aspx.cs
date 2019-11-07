using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using Microsoft.SharePoint.Client.Social;

namespace PeoplePivotsCSOMWeb.Pages
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

            using (ClientContext ctx = TokenHelper.GetClientContextWithContextToken(hdnHostWeb.Value, hdnContextToken.Value, Request.Url.Authority))
            {
                //Get social users
                ClientResult<SocialActor[]> socialActors = null;
                SocialFollowingManager followingManager = new SocialFollowingManager(ctx);
                ctx.Load(followingManager);

                if (type == SocialUserType.Follower)
                    socialActors = followingManager.GetFollowers();
                if (type == SocialUserType.Followed)
                    socialActors = followingManager.GetFollowed(SocialActorTypes.Users);

                ctx.ExecuteQuery();

                //Build a collection of users
                foreach (var socialActor in socialActors.Value)
                {
                    SocialUser user = new SocialUser();
                    user.AccountName = socialActor.AccountName;
                    user.Name = socialActor.Name;
                    user.ImageUrl = socialActor.ImageUri;
                    users.Add(user);
                }
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
            List<SocialPost> posts = new List<SocialPost>();

            using (ClientContext ctx = TokenHelper.GetClientContextWithContextToken(hdnHostWeb.Value, hdnContextToken.Value, Request.Url.Authority))
            {
                try
                {
                    //Get posts
                    SocialFeedManager feedManager = new SocialFeedManager(ctx);
                    ctx.Load(feedManager);

                    SocialFeedOptions feedOptions = new SocialFeedOptions();
                    feedOptions.MaxThreadCount = 50;
                    feedOptions.SortOrder = SocialFeedSortOrder.ByCreatedTime;
                    ClientResult<SocialFeed> feedData = feedManager.GetFeedFor(accountName, feedOptions);
                    ctx.ExecuteQuery();

                    //Build a collection of posts
                    foreach (SocialThread thread in feedData.Value.Threads)
                    {
                        SocialPost post = new SocialPost();
                        post.CreatedDate = thread.RootPost.CreatedTime;
                        post.Body = thread.RootPost.Text;
                        post.LikedByMe = thread.RootPost.LikerInfo.IncludesCurrentUser;
                        posts.Add(post);
                    }

                    return posts;
                }
                catch
                {
                    return new List<SocialPost>();
                }

            }
        }

    }
}