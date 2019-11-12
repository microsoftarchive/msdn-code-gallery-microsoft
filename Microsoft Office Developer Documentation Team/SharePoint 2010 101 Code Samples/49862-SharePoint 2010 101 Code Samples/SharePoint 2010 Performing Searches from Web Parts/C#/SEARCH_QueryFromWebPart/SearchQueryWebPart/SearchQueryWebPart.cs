using System;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.Office.Server.Search.Administration;
using Microsoft.Office.Server.Search.Query;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace SEARCH_QueryFromWebPart.SearchQueryWebPart
{
    /// <summary>
    /// This example Web Part shows how to call the search Query Object Model from a
    /// Web Part. 
    /// </summary>
    /// <remarks>
    /// This code works whether you use a SharePoint Search service application or a
    /// FAST Search service application. However, make sure that your site is connected
    /// to one or the other and that the service application has crawled the content
    /// in your site. Otherwise you will get zero results.
    /// </remarks>
    [ToolboxItemAttribute(false)]
    public class SearchQueryWebPart : WebPart
    {

        //Controls
        Label searchTermsLabel;
        TextBox searchTermsTextbox;
        Button searchButton;
        Label resultsLabel;
        DataGrid resultsGrid;

        protected override void CreateChildControls()
        {
            this.Controls.Clear();
            //Create the searchTermsLabel
            searchTermsLabel = new Label();
            searchTermsLabel.Text = "Search Terms:";
            this.Controls.Add(searchTermsLabel);
            //Create the searchTearmsTextbox
            searchTermsTextbox = new TextBox();
            this.Controls.Add(searchTermsTextbox);
            //Create the searchButton and add a click handler
            searchButton = new Button();
            searchButton.Text = "Search";
            searchButton.Click += new EventHandler(searchButton_Click);
            this.Controls.Add(searchButton);
            //Create the resultsLabel
            resultsLabel = new Label();
            this.Controls.Add(resultsLabel);
            //Create the datagrid
            resultsGrid = new DataGrid();
            this.Controls.Add(resultsGrid);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //Render all the controls
            searchTermsLabel.RenderControl(writer);
            searchTermsTextbox.RenderControl(writer);
            searchButton.RenderControl(writer);
            writer.Write("<br />");
            resultsLabel.RenderControl(writer);
            writer.Write("<br />");
            resultsGrid.RenderControl(writer);
        }

        void searchButton_Click(object sender, EventArgs e)
        {
            //Only search if there are search terms
            if (searchTermsTextbox.Text != string.Empty)
            {
                //First, we must connect to the service application proxy
                SearchServiceApplicationProxy proxy = (SearchServiceApplicationProxy)SearchServiceApplicationProxy.GetProxy(SPServiceContext.GetContext(SPContext.Current.Site));

                //Create and configure a Keyword Query object
                KeywordQuery query = new KeywordQuery(proxy);
                query.ResultsProvider = SearchProvider.Default;
                query.QueryText = searchTermsTextbox.Text;
                //Relevant Results are the main search results
                query.ResultTypes = ResultType.RelevantResults;
                //Now we can execute the query
                ResultTableCollection searchResults = query.Execute();

                if (searchResults.Exists(ResultType.RelevantResults))
                {
                    //There are relevant results. We need them in a Data Table
                    //so we can bind them to the Data Grid for display
                    ResultTable resultTable = searchResults[ResultType.RelevantResults];
                    //Tell the user how many results we got
                    resultsLabel.Text = String.Format("There are {0} results", resultTable.TotalRows.ToString());
                    //Set up and load the data table
                    DataTable resultDataTable = new DataTable();
                    resultDataTable.TableName = "Result";
                    resultDataTable.Load(resultTable, LoadOption.OverwriteChanges);

                    //Bind the datatable to the Data Grid
                    resultsGrid.DataSource = resultDataTable;
                    resultsGrid.DataBind();
                }
                else
                {
                    //Search executed but found nothing
                    resultsLabel.Text = "There were no relevant results. Try other search terms";
                }

            }
            else
            {
                //No search terms in the textbox
                resultsLabel.Text = "Please enter at least one search term";
            }
        }
    }
}
