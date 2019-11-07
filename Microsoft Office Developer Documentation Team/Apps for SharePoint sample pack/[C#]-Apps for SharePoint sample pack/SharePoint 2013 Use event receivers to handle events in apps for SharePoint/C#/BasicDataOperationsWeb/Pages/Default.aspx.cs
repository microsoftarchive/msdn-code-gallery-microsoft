using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;
using Microsoft.IdentityModel.S2S.Tokens;
using System.Net;
using System.IO;

namespace BasicDataOperationsWeb.Pages
{
    public partial class Default : System.Web.UI.Page
    {
        SharePointContextToken contextToken;
        string accessToken;
        Uri sharepointUrl;

        protected void Page_Load(object sender, EventArgs e)
        {
            TokenHelper.TrustAllCertificates();
            string contextTokenString = TokenHelper.GetContextTokenFromRequest(Request);

            if (contextTokenString != null)
            {
                contextToken =
                    TokenHelper.ReadAndValidateContextToken(contextTokenString, Request.Url.Authority);

                sharepointUrl = new Uri(Request.QueryString["SPAppWebUrl"]);
                accessToken =
                    TokenHelper.GetAccessToken(contextToken, sharepointUrl.Authority).AccessToken;
                AddListButton.CommandArgument = accessToken;
                RefreshListButton.CommandArgument = accessToken;
                RetrieveListButton.CommandArgument = accessToken;
                AddItemButton.CommandArgument = accessToken;
                DeleteListButton.CommandArgument = accessToken;
                ChangeListTitleButton.CommandArgument = accessToken;
                RetrieveLists(accessToken);

            }
            else if (!IsPostBack)
            {
                Response.Write("Could not find a context token.");
            }
        }

        //This method retrieves all of the lists on the host Web.
        private void RetrieveLists(string accessToken)
        {
            if (IsPostBack)
            {
                sharepointUrl = new Uri(Request.QueryString["SPAppWebUrl"]);
            }

            AddItemButton.Visible = false;
            AddListItemBox.Visible = false;
            RetrieveListNameBox.Enabled = true;
            DeleteListButton.Visible = false;
            ChangeListTitleButton.Visible = false;
            ChangeListTitleBox.Visible = false;
            ListTable.Rows[0].Cells[1].Text = "List ID";

            //Execute a request for all of the site's lists.
            ClientContext clientContext =
            TokenHelper.GetClientContextWithAccessToken(sharepointUrl.ToString(), accessToken);
            Web web = clientContext.Web;
            ListCollection lists = web.Lists;
            clientContext.Load<ListCollection>(lists);
            clientContext.ExecuteQuery();

            foreach (List list in lists)
            {
                TableRow tableRow = new TableRow();
                TableCell tableCell1 = new TableCell();
                tableCell1.Controls.Add(new LiteralControl(list.Title));
                LiteralControl idClick = new LiteralControl();
                //Use Javascript to populate the RetrieveListNameBox control with the list id.
                string clickScript = "<a onclick=\"document.getElementById(\'RetrieveListNameBox\').value = '" + list.Id.ToString() + "';\" href=\"#\">" + list.Id.ToString() + "</a>";

                idClick.Text = clickScript;
                TableCell tableCell2 = new TableCell();
                tableCell2.Controls.Add(idClick);
                tableRow.Cells.Add(tableCell1);
                tableRow.Cells.Add(tableCell2);
                ListTable.Rows.Add(tableRow);
            }
        }

        //This method retrieves all items from a specified list.
        private void RetrieveListItems(string accessToken, Guid listId)
        {
            if (IsPostBack)
            {
                sharepointUrl = new Uri(Request.QueryString["SPAppWebUrl"]);
            }

            //Adjust the visibility of controls on the page in light of the list-specific context.
            AddItemButton.Visible = true;
            AddListItemBox.Visible = true;
            RetrieveListNameBox.Enabled = false;
            DeleteListButton.Visible = true;
            ChangeListTitleButton.Visible = true;
            ChangeListTitleBox.Visible = true;
            ListTable.Rows[0].Cells[1].Text = "List Items";

            //Execute a request to get the first 100 of the list's items.
            ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(sharepointUrl.ToString(), accessToken);
            Web web = clientContext.Web;
            ListCollection lists = web.Lists;
            List selectedList = lists.GetById(listId);

            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = "<View><RowLimit>100</RowLimit></View>";

            //Use the fully qualified name to disambiguate the ListItemCollection type.
            Microsoft.SharePoint.Client.ListItemCollection listItems = selectedList.GetItems(camlQuery);
            clientContext.Load<ListCollection>(lists);
            clientContext.Load<List>(selectedList);
            clientContext.Load<Microsoft.SharePoint.Client.ListItemCollection>(listItems);

            clientContext.ExecuteQuery();

            TableRow tableRow = new TableRow();
            TableCell tableCell1 = new TableCell();
            tableCell1.Controls.Add(new LiteralControl(selectedList.Title));
            TableCell tableCell2 = new TableCell();

            foreach (Microsoft.SharePoint.Client.ListItem item in listItems)
            {
                tableCell2.Text += item.FieldValues["Title"] + "<br>";
            }

            tableRow.Cells.Add(tableCell1);
            tableRow.Cells.Add(tableCell2);
            ListTable.Rows.Add(tableRow);
        }


        //This method adds a list with the specified title.
        private void AddList(string accessToken, string newListName)
        {
            if (IsPostBack)
            {
                sharepointUrl = new Uri(Request.QueryString["SPAppWebUrl"]);
            }

            //Execute a request to add a list that has the user-supplied name.
            ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(sharepointUrl.ToString(), accessToken);
            Web web = clientContext.Web;
            ListCollection lists = web.Lists;
            ListCreationInformation listCreationInfo = new ListCreationInformation();
            listCreationInfo.Title = newListName;
            listCreationInfo.TemplateType = (int)ListTemplateType.GenericList;
            lists.Add(listCreationInfo);
            clientContext.Load<ListCollection>(lists);
            try
            {
                clientContext.ExecuteQuery();
            }
            catch (Exception e)
            {
                AddListNameBox.Text = e.Message;
            }
            RetrieveLists(accessToken);
        }

        //This method adds a list item to the specified list.
        private void AddListItem(string accessToken, Guid listId, string newItemName)
        {
            if (IsPostBack)
            {
                sharepointUrl = new Uri(Request.QueryString["SPAppWebUrl"]);
            }

            //Execute a request to add a list item.
            ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(sharepointUrl.ToString(), accessToken);
            Web web = clientContext.Web;
            ListCollection lists = web.Lists;
            List selectedList = lists.GetById(listId);
            clientContext.Load<ListCollection>(lists);
            clientContext.Load<List>(selectedList);
            ListItemCreationInformation listItemCreationInfo = new ListItemCreationInformation();
            var listItem = selectedList.AddItem(listItemCreationInfo);
            listItem["Title"] = newItemName;
            listItem.Update();
            clientContext.ExecuteQuery();
            RetrieveListItems(accessToken, listId);
        }

        private void ChangeListTitle(string accessToken, Guid listId, string newListTitle)
        {
            if (IsPostBack)
            {
                sharepointUrl = new Uri(Request.QueryString["SPAppWebUrl"]);
            }

            //Execute a request to change the title of the specified list.
            ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(sharepointUrl.ToString(), accessToken);
            Web web = clientContext.Web;
            ListCollection lists = web.Lists;
            List selectedList = lists.GetById(listId);
            clientContext.Load<ListCollection>(lists);
            clientContext.Load<List>(selectedList);
            selectedList.Title = newListTitle;
            selectedList.Update();
            clientContext.ExecuteQuery();
            RetrieveListItems(accessToken, listId);
        }

        private void DeleteList(string accessToken, Guid listId)
        {
            if (IsPostBack)
            {
                sharepointUrl = new Uri(Request.QueryString["SPAppWebUrl"]);
            }

            //Execute a request to delete the specified list.
            ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(sharepointUrl.ToString(), accessToken);
            Web web = clientContext.Web;
            ListCollection lists = web.Lists;
            List selectedList = lists.GetById(listId);
            clientContext.Load<ListCollection>(lists);
            clientContext.Load<List>(selectedList);
            selectedList.DeleteObject();
            clientContext.ExecuteQuery();
            RetrieveListNameBox.Text = "";
            RetrieveLists(accessToken);
        }

        protected void AddList_Click(object sender, EventArgs e)
        {

            string commandAccessToken = ((Button)sender).CommandArgument;
            if (AddListNameBox.Text != "")
            {
                AddList(commandAccessToken, AddListNameBox.Text);
            }
            else
            {
                AddListNameBox.Text = "Enter a list title";
            }
        }

        protected void RefreshList_Click(object sender, EventArgs e)
        {

            string commandAccessToken = ((Button)sender).CommandArgument;
            RetrieveLists(commandAccessToken);
        }

        protected void RetrieveListButton_Click(object sender, EventArgs e)
        {
            string commandAccessToken = ((Button)sender).CommandArgument;

            Guid listId = new Guid();
            if (Guid.TryParse(RetrieveListNameBox.Text, out listId))
            {
                RetrieveListItems(commandAccessToken, listId);
            }
            else
            {
                RetrieveListNameBox.Text = "Enter a List GUID";
            }
        }

        protected void AddItemButton_Click(object sender, EventArgs e)
        {
            string commandAccessToken = ((Button)sender).CommandArgument;
            Guid listId = new Guid(RetrieveListNameBox.Text);
            if (AddListItemBox.Text != "")
            {
                AddListItem(commandAccessToken, listId, AddListItemBox.Text);
            }
            else
            {
                AddListItemBox.Text = "Enter an item title";
            }
        }

        protected void DeleteListButton_Click(object sender, EventArgs e)
        {
            string commandAccessToken = ((Button)sender).CommandArgument;
            Guid listId = new Guid(RetrieveListNameBox.Text);
            DeleteList(commandAccessToken, listId);
        }

        protected void ChangeListTitleButton_Click(object sender, EventArgs e)
        {
            string commandAccessToken = ((Button)sender).CommandArgument;
            Guid listId = new Guid(RetrieveListNameBox.Text);
            if (ChangeListTitleBox.Text != null)
            {
                ChangeListTitle(commandAccessToken, listId, ChangeListTitleBox.Text);
            }
            else
            {
                ChangeListTitleBox.Text = "Enter a new list title";
            }
        }
    }
}