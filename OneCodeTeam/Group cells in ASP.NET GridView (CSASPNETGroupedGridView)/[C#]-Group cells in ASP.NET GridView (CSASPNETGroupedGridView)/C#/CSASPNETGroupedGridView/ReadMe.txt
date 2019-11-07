=============================================================================
     ASP.NET APPLICATION : CSASPNETGroupedGridView Project Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

The code sample shows how to group cells in GridView with the same value.


/////////////////////////////////////////////////////////////////////////////
Demo:

Open the sample project and run tha web application.  In Default.aspx, you 
will see two GridViews binding the same data (Product Name, Category, Weight).  
The first GridView shows each row of data as usual.  The second GridView 
groups Product Name and Category, and shows the merged cells. 


/////////////////////////////////////////////////////////////////////////////
Implementation:

Step1. Open your Visual Studio 2000 to create a Visual C# Web application by 
choosing "File" -> "New" -> "Project...", expand the "Visual C#" tag and 
select "Web", then choose "ASP.NET Web Application".  You can name it as 
"CSASPNETGroupedGridView".

Step2. In Default.aspx, add two GridViews. 

        <table>
            <tr>
                <td style="padding:20px">
                    <h3>
                        General GridView</h3>
                    <asp:GridView ID="generalGridView" runat="server" AllowPaging="True" 
                        OnPageIndexChanged="generalGridView_PageIndexChanged"
                        OnPageIndexChanging="generalGridView_PageIndexChanging">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <RowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                    </asp:GridView>
                </td>
                <td style="padding:20px">
                    <h3>
                        Grouped GridView</h3>
                    <asp:GridView ID="groupedGridView" runat="server" AllowPaging="True" 
                        OnPageIndexChanged="groupedGridView_PageIndexChanged"
                        OnPageIndexChanging="groupedGridView_PageIndexChanging">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <RowStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                    </asp:GridView>
                </td>
            </tr>
        </table>

Step3. Reference the jQuery library and add the following reusable Javascript 
function for grouping GridView cells. 

    <script type="text/javascript" src="http://ajax.microsoft.com/ajax/jquery/jquery-1.4.4.min.js"></script>
    <script type="text/javascript">

        // This js function will automatially combine the GridView cells
        // gridviewId: The id of the gridview.
        function GroupGridViewCells(gridviewId) {

            // Get the number of the rows
            var rowNum = $(gridviewId + " tr").length;

            // Get the number of the columns;
            var colNum = $(gridviewId + " tr:eq(0)>th").length;

            // Get the current cell
            var cell = null;

            // Get the previous cell
            var previouscell = null;

            // Begin to loop from the second row to the end
            for (var col = 0; col < colNum; ++col) {

                for (var row = 1; row < rowNum; ++row) {

                    cell = $(gridviewId + " tr:eq(" + row + ")>td:eq(" + col + ")").first();

                    // We haven't the previous row to compare with, so keep this first
                    if (row == 1) {
                        previouscell = $(gridviewId + " tr:eq(" + row + ")>td:eq(" + col + ")").first();
                        previouscell.attr("rowspan", "1");
                    }
                    else {

                        // If the current value has the same value as the previous one,
                        // the previous one's rowspan should be increased by 1, and you should
                        // hide the current cell.
                        if (cell.html() == previouscell.html()) {

                            previouscell.attr("rowspan", parseInt(previouscell.attr("rowspan") + 1));
                            cell.css("display", "none");
                        }
                        else {
                            // If the current cell's value doesn't equal to the previous one
                            // we should restart keep the cell for future comparation.
                            previouscell = $(gridviewId + " tr:eq(" + row + ")>td:eq(" + col + ")");
                            previouscell.attr("rowspan", "1");
                            previouscontent = previouscell.html();
                        }
                    }
                }
            }
        }

Step4. Call the GroupGridViewCells function to group cells in the second 
GridView control.

        // Call the js function like this:
        $(function () {
            GroupGridViewCells("#groupedGridView");
        })

Step5. Switch to the code-behind and the following code to bind the same 
sorted test data to the GridViews.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindSortedTestData(generalGridView);
                BindSortedTestData(groupedGridView);
            }
        }

        /// <summary>
        /// Bind sorted test data to the given GridView control.
        /// </summary>
        /// <param name="gridView">the GridView control</param>
        private void BindSortedTestData(GridView gridView)
        {
            const string TestDataViewStateId = "TestData";
            DataTable dt = ViewState[TestDataViewStateId] as DataTable;

            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add("Product Name", typeof(string));
                dt.Columns.Add("Category", typeof(int));
                dt.Columns.Add("Weight", typeof(double));
                Random r = new Random(DateTime.Now.Millisecond);

                for (int i = 1; i <= 50; i++)
                {
                    // Adding ProductId, Category, and random price.
                    dt.Rows.Add(
                        "Product" + r.Next(1, 5), 
                        r.Next(1, 5), 
                        Math.Round(r.NextDouble() * 100 + 50, 2)
                        );
                }

                ViewState[TestDataViewStateId] = dt;
            }

            // Sort by ProductName
            dt.DefaultView.Sort = "Product Name,Category";
            
            gridView.DataSource = dt;
            gridView.DataBind();
        }


/////////////////////////////////////////////////////////////////////////////
Reference：

http://www.codeproject.com/KB/webforms/MergeGridViewCells.aspx 
(Containing a server-side solution for grouping cells in GridView)


/////////////////////////////////////////////////////////////////////////////