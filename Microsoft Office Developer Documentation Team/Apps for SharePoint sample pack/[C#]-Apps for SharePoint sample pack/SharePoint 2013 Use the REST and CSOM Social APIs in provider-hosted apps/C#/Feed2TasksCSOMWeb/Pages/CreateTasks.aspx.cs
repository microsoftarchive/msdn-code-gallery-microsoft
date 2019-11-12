using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.UserProfiles;
using Microsoft.SharePoint.Client.Social;

namespace Feed2TasksCSOMWeb.Pages
{
    public partial class CreateTasks : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                /* Save the Context Token and Host Web URL in hidden fields
                 * These will be needed to create client contexts in subsequent calls */
                hdnContextToken.Value = TokenHelper.GetContextTokenFromRequest(Page.Request);
                hdnHostWeb.Value = Page.Request["SPHostUrl"];

                List<AssignmentPost> posts = new List<AssignmentPost>();

                /* Create a collection of posts that mention the current user
                 * and are tagged with #Assignment. These posts are candidates
                 * to be turned into tasks for the current user */

                using (ClientContext ctx = TokenHelper.GetClientContextWithContextToken(hdnHostWeb.Value, hdnContextToken.Value, Request.Url.Authority))
                {
                    try
                    {
                        //Get current user
                        ctx.Load(ctx.Web, w => w.CurrentUser);
                        ctx.ExecuteQuery();

                        //Get posts that mention the current user
                        SocialFeedManager feedManager = new SocialFeedManager(ctx);
                        ctx.Load(feedManager);

                        SocialFeedOptions feedOptions = new SocialFeedOptions();
                        feedOptions.MaxThreadCount = 50;
                        feedOptions.SortOrder = SocialFeedSortOrder.ByCreatedTime;
                        ClientResult<SocialFeed> feedData = feedManager.GetMentions(false, feedOptions);
                        ctx.ExecuteQuery();

                        //Build a collection of posts tagged with #Assignment
                        foreach (SocialThread thread in feedData.Value.Threads)
                        {
                            if (thread.PostReference.Post.Text.Contains("#Assignment"))
                            {
                                AssignmentPost post = new AssignmentPost();
                                post.CreatedDate = thread.RootPost.CreatedTime;
                                post.Body = thread.PostReference.Post.Text
                                    .Replace("#Assignment", string.Empty)
                                    .Replace("@" + ctx.Web.CurrentUser.Title, string.Empty);
                                post.Requester = thread.Actors[thread.PostReference.Post.AuthorIndex].Name;
                                posts.Add(post);
                            }
                        }

                        //Bind these posts for display
                        assignmentPosts.DataSource = posts;
                        assignmentPosts.DataBind();
                    }
                    catch (Exception x)
                    {
                        messages.Text = x.Message;
                    }
                }
            }
        }

        protected void assignmentPosts_SelectedIndexChanged(object sender, EventArgs e)
        {
            /* Create a task from an item selected in the grid */

            using (ClientContext ctx = TokenHelper.GetClientContextWithContextToken(hdnHostWeb.Value, hdnContextToken.Value, Request.Url.Authority))
            {
                try
                {
                    messages.Text = string.Empty;

                    //Get current user
                    ctx.Load(ctx.Web, w => w.CurrentUser);
                    ctx.ExecuteQuery();

                    //Get the tasks list
                    List tasks = ctx.Web.Lists.GetByTitle("Tasks");
                    ctx.Load(tasks);

                    //Create task item
                    ListItemCreationInformation listItemCI = new ListItemCreationInformation();
                    Microsoft.SharePoint.Client.ListItem task = tasks.AddItem(listItemCI);
                    task["Title"] = "New Task from " + assignmentPosts.SelectedItem.Cells[3].Text;
                    task["StartDate"] = DateTime.Now;
                    task["AssignedTo"] = ctx.Web.CurrentUser;
                    task["Body"] = assignmentPosts.SelectedItem.Cells[2].Text;
                    task.Update();
                    ctx.ExecuteQuery();

                    //Confirm
                    messages.Text = "New task assigned!";
                   
                }
                catch (Exception x)
                {
                    messages.Text = x.Message;
                }
            }

        }
    }
}
