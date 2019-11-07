# Display and select current rating
## Requires
- Visual Studio 2008
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
## Topics
- AJAX
- Rating
## Updated
- 06/05/2013
## Description

<h1>Ajax Rating control-display the current rating and allow user to select the current rating(CSASPNETRatingControlSelectCurrentValue)</h1>
<h2>Introduction </h2>
<p class="MsoNormal">This sample will demonstrate you how to solve the problem that using the Ajax Rating control to select the currently selected option. Because the OnChanged event doesn't trigger resulted in the user cannot select the currently selected
 items. The sample will load a list of books, when the user clicks the link in one of the records, we can see the rating corresponds to the current record books. When the user clicks on the current rating, the database will use the current rating to insert
 a new record.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Please follow these steps below.</p>
<p class="MsoNormal">Step 1:&nbsp;Open the <span style="">CSASPNETRatingControlSelectCurrentValue</span>.sln. Expand the
<a name="OLE_LINK1"><span style="">CSASPNETRatingControlSelectCurrentValue</span>
</a>web application and press Ctrl &#43; F5 to show the Default.aspx. </p>
<p class="MsoNormal"><b style="">[Note]</b> <b style="">You may need to install the AjaxControlToolkit,</b>
<b style="">You can download it here: <a href="http://ajaxcontroltoolkit.codeplex.com/">
http://ajaxcontroltoolkit.codeplex.com/</a> </b></p>
<p class="MsoNormal"><span style=""><img src="83609-image.png" alt="" width="568" height="358" align="middle">
</span></p>
<p class="MsoNormal">Step 2: Click on a link of the record, then the current rating of Ajax Rating control will change to which item you select. Assumptions to select the third:</p>
<p class="MsoNormal"><span style=""><img src="83610-image.png" alt="" width="450" height="319" align="middle">
</span></p>
<p class="MsoNormal">Step 3: Click on the current rating of Ajax Rating control, it will insert a new record to the database and re-bind.<br>
<span style=""><img src="83611-image.png" alt="" width="669" height="399" align="middle">
</span></p>
<p class="MsoNormal">Step 4:<span style="">&nbsp;&nbsp; </span>Validation finished.</p>
<h2>Using the Code</h2>
<p class="MsoNormal" style="">Code Logical: <span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></p>
<p class="MsoNormal">Step 1. Create a C# &quot;ASP.NET Empty Web Application&quot; in Visual Studio 2008. Name it as &quot;<span style="">CSASPNETRatingControlSelectCurrentValue</span>&quot;.
</p>
<p class="MsoNormal">Step 2.<span style="">&nbsp; </span>Add a DataBase under the App_Data file and rename it to &quot;Books&quot;. Add a table in the database and named it &quot;BookInfo&quot;. Table field is defined as follows<span style="">:
</span></p>
<p class="MsoNormal"><span style=""><img src="83612-image.png" alt="" width="419" height="179" align="middle">
</span><br>
Then we need to add some records in this table.</p>
<p class="MsoNormal">Step 3.<span style="">&nbsp; </span>Add a GridView Control, a Button Control and an Ajax Rating control in the page then rename the GridView to &quot;gdvBooks&quot;. Write the style sheet file for the Rating Control and then name it &quot;Rate.css&quot;:</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">html</span>
<pre class="hidden">
.ratingStar
{
    font-size: 0pt;
    width: 13px;
    height: 12px;
    margin: 0px;
    padding: 0px;
    cursor: pointer;
    display: block;
    background-repeat: no-repeat;
}
.filledRatingStar
{
    background-image: url(Images/FilledStar.png);
}
.emptyRatingStar
{
    background-image: url(Images/EmptyStar.png);
}
.savedRatingStar
{
    background-image: url(Images/SavedStar.png);
}



</pre>
<pre id="codePreview" class="html">
.ratingStar
{
    font-size: 0pt;
    width: 13px;
    height: 12px;
    margin: 0px;
    padding: 0px;
    cursor: pointer;
    display: block;
    background-repeat: no-repeat;
}
.filledRatingStar
{
    background-image: url(Images/FilledStar.png);
}
.emptyRatingStar
{
    background-image: url(Images/EmptyStar.png);
}
.savedRatingStar
{
    background-image: url(Images/SavedStar.png);
}



</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><br>
Write the javascript file for the Rating Control and name it &quot;Rate.js&quot;. The script will be used to trigger button event when click rating Control.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>JavaScript</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">js</span>
<pre class="hidden">
Sys.Extended.UI.RatingBehavior.prototype._onStarClick = function(item) {
    if (this._readOnly) {
        return;
    }
    this.set_Rating(this._currentRating);
    __doPostBack('btnSubmit', '');


}; 

</pre>
<pre id="codePreview" class="js">
Sys.Extended.UI.RatingBehavior.prototype._onStarClick = function(item) {
    if (this._readOnly) {
        return;
    }
    this.set_Rating(this._currentRating);
    __doPostBack('btnSubmit', '');


}; 

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><br>
Step 4.<span style="">&nbsp; </span>Bind data to GridView. The Code will be shown as below:</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
/// &lt;summary&gt;
        /// Bind data to gdvBooks.
        /// &lt;/summary&gt;
        private void BindData()
        {
            SqlDataAdapter sda = new SqlDataAdapter(&quot;select * from bookInfo&quot;, conn);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            gdvBooks.DataSource = ds;
            gdvBooks.DataBind();
        }

</pre>
<pre id="codePreview" class="csharp">
/// &lt;summary&gt;
        /// Bind data to gdvBooks.
        /// &lt;/summary&gt;
        private void BindData()
        {
            SqlDataAdapter sda = new SqlDataAdapter(&quot;select * from bookInfo&quot;, conn);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            gdvBooks.DataSource = ds;
            gdvBooks.DataBind();
        }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><br>
Step 5. Paid to control the rate of the currently selected item. The code will be shown as below:</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">html</span>
<pre class="hidden">
&lt;asp:GridView ID=&quot;gdvBooks&quot; runat=&quot;server&quot; AutoGenerateColumns=&quot;False&quot; DataKeyNames=&quot;Id&quot;
           OnRowCommand=&quot;gdvBooks_RowCommand&quot;&gt;
           &lt;Columns&gt;
              ...
               &lt;asp:TemplateField HeaderText=&quot;Rate&quot;&gt;
                   &lt;ItemTemplate&gt;
                       &lt;asp:LinkButton ID=&quot;LinkbtnSubmit&quot; runat=&quot;server&quot; CommandName=&quot;RateDetail&quot; CommandArgument='&lt;%# Eval(&quot;Rate&quot;) %&gt;'&gt;RateDetail&lt;/asp:LinkButton&gt;
                   &lt;/ItemTemplate&gt;
               &lt;/asp:TemplateField&gt;
           &lt;/Columns&gt;
       &lt;/asp:GridView&gt;

</pre>
<pre id="codePreview" class="html">
&lt;asp:GridView ID=&quot;gdvBooks&quot; runat=&quot;server&quot; AutoGenerateColumns=&quot;False&quot; DataKeyNames=&quot;Id&quot;
           OnRowCommand=&quot;gdvBooks_RowCommand&quot;&gt;
           &lt;Columns&gt;
              ...
               &lt;asp:TemplateField HeaderText=&quot;Rate&quot;&gt;
                   &lt;ItemTemplate&gt;
                       &lt;asp:LinkButton ID=&quot;LinkbtnSubmit&quot; runat=&quot;server&quot; CommandName=&quot;RateDetail&quot; CommandArgument='&lt;%# Eval(&quot;Rate&quot;) %&gt;'&gt;RateDetail&lt;/asp:LinkButton&gt;
                   &lt;/ItemTemplate&gt;
               &lt;/asp:TemplateField&gt;
           &lt;/Columns&gt;
       &lt;/asp:GridView&gt;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
/// &lt;summary&gt;
        /// Paid to control the rate of the currently selected item.
        /// &lt;/summary&gt;
        /// &lt;param name=&quot;sender&quot;&gt;&lt;/param&gt;
        /// &lt;param name=&quot;e&quot;&gt;&lt;/param&gt;
        protected void gdvBooks_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == &quot;RateDetail&quot;)
            {
                LinkButton lb = (LinkButton)e.CommandSource;//Get control according to CommandSource


                string s = lb.CommandArgument;
                Rating1.CurrentRating = Convert.ToInt32(lb.CommandArgument);
            }
        }

</pre>
<pre id="codePreview" class="csharp">
/// &lt;summary&gt;
        /// Paid to control the rate of the currently selected item.
        /// &lt;/summary&gt;
        /// &lt;param name=&quot;sender&quot;&gt;&lt;/param&gt;
        /// &lt;param name=&quot;e&quot;&gt;&lt;/param&gt;
        protected void gdvBooks_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == &quot;RateDetail&quot;)
            {
                LinkButton lb = (LinkButton)e.CommandSource;//Get control according to CommandSource


                string s = lb.CommandArgument;
                Rating1.CurrentRating = Convert.ToInt32(lb.CommandArgument);
            }
        }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><br>
<span style="">&nbsp;</span>Step 6. In the Click event of the button, get the value of the selected item and insert a new record to database by using the value, the code will be shown as below:</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
//Store the value of the item is selected and used to insert the database record
            int intRate = 0;


            switch (Rating1.CurrentRating)
            {
                case 1:
                    intRate = 1;
                    break;
                case 2:
                    intRate = 2;
                    break;
                case 3:
                    intRate = 3;
                    break;
                case 4:
                    intRate = 4;
                    break;
                case 5:
                    intRate = 5;
                    break;
            }


            try
            {
                //Insert a new record.
                insertdataintosql(&quot;test1&quot;, &quot;Microsoft&quot;, intRate);
            }
            catch (DataException ee)
            {
                lbResponse.Text = ee.Message;
                lbResponse.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                //Bind data.
                BindData();
            }

</pre>
<pre id="codePreview" class="csharp">
//Store the value of the item is selected and used to insert the database record
            int intRate = 0;


            switch (Rating1.CurrentRating)
            {
                case 1:
                    intRate = 1;
                    break;
                case 2:
                    intRate = 2;
                    break;
                case 3:
                    intRate = 3;
                    break;
                case 4:
                    intRate = 4;
                    break;
                case 5:
                    intRate = 5;
                    break;
            }


            try
            {
                //Insert a new record.
                insertdataintosql(&quot;test1&quot;, &quot;Microsoft&quot;, intRate);
            }
            catch (DataException ee)
            {
                lbResponse.Text = ee.Message;
                lbResponse.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                //Bind data.
                BindData();
            }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
/// &lt;summary&gt;
        /// The function use to insert data to sql.
        /// &lt;/summary&gt;
        /// &lt;param name=&quot;name&quot;&gt;name&lt;/param&gt;
        /// &lt;param name=&quot;Author&quot;&gt;Author&lt;/param&gt;
        /// &lt;param name=&quot;Rate&quot;&gt;Rate&lt;/param&gt;
        public void insertdataintosql(string name, string Author, int Rate)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = &quot;insert into bookInfo(name,Author,Rate) values(@name,@Author,@Rate)&quot;;
            cmd.Parameters.Add(&quot;@name&quot;, SqlDbType.NVarChar).Value = name;
            cmd.Parameters.Add(&quot;@Author&quot;, SqlDbType.NVarChar).Value = Author;
            cmd.Parameters.Add(&quot;@Rate&quot;, SqlDbType.Int).Value = Rate;
            cmd.CommandType = CommandType.Text;
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

</pre>
<pre id="codePreview" class="csharp">
/// &lt;summary&gt;
        /// The function use to insert data to sql.
        /// &lt;/summary&gt;
        /// &lt;param name=&quot;name&quot;&gt;name&lt;/param&gt;
        /// &lt;param name=&quot;Author&quot;&gt;Author&lt;/param&gt;
        /// &lt;param name=&quot;Rate&quot;&gt;Rate&lt;/param&gt;
        public void insertdataintosql(string name, string Author, int Rate)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = &quot;insert into bookInfo(name,Author,Rate) values(@name,@Author,@Rate)&quot;;
            cmd.Parameters.Add(&quot;@name&quot;, SqlDbType.NVarChar).Value = name;
            cmd.Parameters.Add(&quot;@Author&quot;, SqlDbType.NVarChar).Value = Author;
            cmd.Parameters.Add(&quot;@Rate&quot;, SqlDbType.Int).Value = Rate;
            cmd.CommandType = CommandType.Text;
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><br>
Step 7. Build the application and you can debug it.</p>
<h2>More Information</h2>
<p class="MsoNormal">AjaxControlToolkitSampleSite<br>
<a href="http://www.asp.net/ajaxLibrary/AjaxControlToolkitSampleSite/Rating/Rating.aspx">http://www.asp.net/ajaxLibrary/AjaxControlToolkitSampleSite/Rating/Rating.aspx</a></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
