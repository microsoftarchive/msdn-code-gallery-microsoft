# Upload/edit image in ASP.NET (CSASPNETImageEditUpload)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- ASP.NET
## Topics
- Image
- FormView
## Updated
- 02/03/2012
## Description

<h2><span style="font-family:courier new,courier; font-size:medium">ASP.NET APPLICATION : CSASPNETFormViewUpload Project Overview</span></h2>
<h3><span style="font-family:courier new,courier; font-size:small">Use:</span></h3>
<p><span style="font-family:courier new,courier; font-size:small">This CSASPNETFormViewUpload sample demonstrates how to display and upload
</span><br>
<span style="font-family:courier new,courier; font-size:small">images in an ASP.NET FormView control and how to implement Insert, Edit,
</span><br>
<span style="font-family:courier new,courier; font-size:small">Update, Delete and Paging functions in the control.</span></p>
<p><span style="font-family:courier new,courier; font-size:small">This project includes two pages: Default and Image.</span></p>
<p><span style="font-family:courier new,courier; font-size:small">Default populates a FormView control with data from a SQL Server database
</span><br>
<span style="font-family:courier new,courier; font-size:small">and provides UI for data manipulation.</span></p>
<p><span style="font-family:courier new,courier; font-size:small">Image is used to retrieve the image from a SQL Server database and display
</span><br>
<span style="font-family:courier new,courier; font-size:small">it in the Web page.</span></p>
<h3><br>
<span style="font-family:courier new,courier; font-size:small">Creation:</span></h3>
<p><span style="font-family:courier new,courier; font-size:small">Step1. Create a C# ASP.NET Web Application named CSASPNETFormViewUpload in
</span><br>
<span style="font-family:courier new,courier; font-size:small">Visual Studio 2008 / Visual Web Developer.</span></p>
<p><br>
<span style="font-family:courier new,courier; font-size:small">Step2. Drag a FormView control into the Default page.</span></p>
<p><span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; (1) Rename the FormView to fvPerson.</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; (2) In the Source view, copy and paste the markup of following three
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; templates from the sample:</span></p>
<p><span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; ItemTemplate: render the particular record displayed in the FormView.</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; EditItemTemplate: customize the editing interface for the FormView.</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; InsertItemTemplate: customize the inserting interface for the FormView.</span></p>
<p><span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; Related references:</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; ASP.NET: Using the FormView's Templates&nbsp;</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; MSDN: FormView Class</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; MSDN: Image Class</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">Step3. Copy the following methods from the sample,and paste them in the
</span><br>
<span style="font-family:courier new,courier; font-size:small">code-behind of Default page:</span></p>
<p><span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; Page_Load Method:</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; Initialize underlying objects, when the Page is accessed
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; for the first time.</span></p>
<p><span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; BindFormView Method:</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; Bind the FormView control with data from a SQL Server table.</span></p>
<p><span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; Related reference:</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; MSDN: using Statement (C# Reference)</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">Step4. Navigate to the Property panel of fvPerson and then switch to Event.
</span><br>
<span style="font-family:courier new,courier; font-size:small">Double-click on the following events to generate the Event handlers.
</span><br>
<span style="font-family:courier new,courier; font-size:small">After that, fill the generated methods with the sample code.</span></p>
<p><span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; (1)&nbsp;ModeChanging Event: Occurs when the FormView control switches
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; between edit, insert, and read-only mode, but before the mode changes.</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; In this event, we need to switch FormView control to the new mode and
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; then rebind the FormView control to show data in new mode.&nbsp;</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; ////////////////////////////////////////////////////////////////</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; fvPerson.ChangeMode(e.NewMode);</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; BindFormView();</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; ////////////////////////////////////////////////////////////////</span></p>
<p><span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; Related reference:</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; MSDN: FormView.ModeChanging&nbsp;</span></p>
<p><span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; (2)&nbsp;PageIndexChanging Event: Occurs when a pager button within the
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; control is clicked.</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; In order to show data in the new page, we need to set the index of new
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; page and then rebind the FormView control. &nbsp;</span></p>
<p><span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; ////////////////////////////////////////////////////////////////&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; fvPerson.PageIndex = e.NewPageIndex;</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; BindFormView();</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; ////////////////////////////////////////////////////////////////</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; Related reference:&nbsp;</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; MSDN: FormView.PageIndexChanging Event</span></p>
<p><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; (3)&nbsp;ItemInserting Event: Occurs when an Insert button within a FormView
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; control is clicked, but before the insert operation.</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; After clicking Insert button, we need to get the first name, last name
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; and specified image file (bytes) from the &nbsp;InsertItemTemplate of the
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; FormView control.</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; ////////////////////////////////////////////////////////////////</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; string strLastName = ((TextBox)fvPerson.Row.FindControl(&quot;tbLastName&quot;)).Text;</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; string strFirstName = ((TextBox)fvPerson.Row.FindControl(&quot;tbFirstName&quot;)).Text;</span></p>
<p><span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; cmd.Parameters.Add(&quot;@LastName&quot;, SqlDbType.NVarChar, 50).Value = strLastName;</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; cmd.Parameters.Add(&quot;@FirstName&quot;, SqlDbType.NVarChar, 50).Value = strFirstName;</span></p>
<p><span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; FileUpload uploadPicture = (FileUpload)fvPerson.FindControl(&quot;uploadPicture&quot;);</span></p>
<p><span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; if (uploadPicture.HasFile)</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; {</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; cmd.Parameters.Add(&quot;@Picture&quot;, SqlDbType.VarBinary).Value = uploadPicture.FileBytes;</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; }</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; else</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; {</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; cmd.Parameters.Add(&quot;@Picture&quot;, SqlDbType.VarBinary).Value = DBNull.Value;</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; }</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; ////////////////////////////////////////////////////////////////</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></p>
<p><span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; Related reference:&nbsp;</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; MSDN: FormView.ItemInserting Event&nbsp;</span></p>
<p><span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; (4)&nbsp;ItemUpdating Event: Occurs when an Update button within a FormView
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; control is clicked, but before the update operation.</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; After clicking Update button, we need to get and pass the person ID,
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; first name, last name and specified image file (bytes) from the
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; EditItemTemplate of the FormView control.</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; ////////////////////////////////////////////////////////////////
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; string strPersonID = ((Label)fvPerson.Row.FindControl(&quot;lblPersonID&quot;)).Text;&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; ////////////////////////////////////////////////////////////////</span></p>
<p><span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; Related reference:&nbsp;</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; MSDN: FormView.ItemUpdating Event</span></p>
<p><span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; (5)&nbsp;ItemDeletingEvent: Occurs when a Delete button within a FormView
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; control is clicked, but before the delete operation.</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; We get the PersonID from the ItemTemplate of the FormView control and
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; then delete the person record in the database based on the PersonID.</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; ////////////////////////////////////////////////////////////////</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; string strPersonID = ((Label)fvPerson.Row.FindControl(&quot;lblPersonID&quot;)).Text;</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; ////////////////////////////////////////////////////////////////</span></p>
<p><span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; Related reference:&nbsp;</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; MSDN: FormView.ItemDeleting Event</span></p>
<p><br>
<span style="font-family:courier new,courier; font-size:small">Step5. Create a new Web page named Image in the project. Copy the Page_Load
</span><br>
<span style="font-family:courier new,courier; font-size:small">method from the sample, and paste it in code-behind of Image page:</span></p>
<p><span style="font-family:courier new,courier; font-size:small">In this page, we retrieve the image data from the database, convert it to a
</span><br>
<span style="font-family:courier new,courier; font-size:small">bytes array and then write the array to the HTTP output stream
</span><br>
<span style="font-family:courier new,courier; font-size:small">to display the image.</span></p>
<p><span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; ////////////////////////////////////////////////////////////////
</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; Byte[] bytes = (byte[])cmd.ExecuteScalar();</span></p>
<p><span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; if (bytes != null)</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; {</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Response.ContentType = &quot;image/jpeg&quot;;</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Response.BinaryWrite(bytes);</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Response.End();</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; }</span><br>
<span style="font-family:courier new,courier; font-size:small">&nbsp;&nbsp;&nbsp; ////////////////////////////////////////////////////////////////</span></p>
<p><span style="font-family:courier new,courier; font-size:small">Related references:</span></p>
<p><span style="font-family:courier new,courier; font-size:small">MSDN: Request Object</span><br>
<span style="font-family:courier new,courier; font-size:small">MSDN: Response Object</span></p>
<h3><br>
<span style="font-family:courier new,courier; font-size:small">References:</span></h3>
<p><span style="font-family:courier new,courier; font-size:small">ASP.NET: Using the FormView's Templates</span><br>
<span style="font-family:courier new,courier; font-size:small"><a href="http://www.asp.net/learn/data-access/tutorial-14-cs.aspx">http://www.asp.net/learn/data-access/tutorial-14-cs.aspx</a></span></p>
<p><span style="font-family:courier new,courier; font-size:small">MSDN: Image Class</span><br>
<span style="font-family:courier new,courier; font-size:small"><a href="http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.image.aspx">http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.image.aspx</a></span></p>
<p><span style="font-family:courier new,courier; font-size:small">MSDN: FormView Class</span><br>
<span style="font-family:courier new,courier; font-size:small"><a href="http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.formview.aspx">http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.formview.aspx</a></span></p>
<p><span style="font-family:courier new,courier; font-size:small">MSDN: using Statement (C# Reference)</span><br>
<span style="font-family:courier new,courier; font-size:small"><a href="http://msdn.microsoft.com/en-us/library/yh598w02.aspx">http://msdn.microsoft.com/en-us/library/yh598w02.aspx</a></span></p>
<p><span style="font-family:courier new,courier; font-size:small">MSDN: FormView.ModeChanging</span><br>
<span style="font-family:courier new,courier; font-size:small"><a href="http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.formview.modechanging.aspx">http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.formview.modechanging.aspx</a></span></p>
<p><span style="font-family:courier new,courier; font-size:small">MSDN: FormView.PageIndexChanging Event</span><br>
<span style="font-family:courier new,courier; font-size:small"><a href="http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.formview.pageindexchanging.aspx">http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.formview.pageindexchanging.aspx</a></span></p>
<p><span style="font-family:courier new,courier; font-size:small">MSDN: FormView.ItemInserting Event</span><br>
<span style="font-family:courier new,courier; font-size:small"><a href="http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.formview.iteminserting.aspx">http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.formview.iteminserting.aspx</a></span></p>
<p><span style="font-family:courier new,courier; font-size:small">MSDN: FormView.ItemUpdating Event</span><br>
<span style="font-family:courier new,courier; font-size:small"><a href="http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.formview.itemupdating.aspx">http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.formview.itemupdating.aspx</a></span></p>
<p><span style="font-family:courier new,courier; font-size:small">MSDN: FormView.ItemDeleting Event</span><br>
<span style="font-family:courier new,courier; font-size:small"><a href="http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.formview.itemdeleting.aspx">http://msdn.microsoft.com/en-us/library/system.web.ui.webcontrols.formview.itemdeleting.aspx</a></span></p>
<p><span style="font-family:courier new,courier; font-size:small">MSDN: Request Object</span><br>
<span style="font-family:courier new,courier; font-size:small"><a href="http://msdn.microsoft.com/en-us/library/ms524948.aspx">http://msdn.microsoft.com/en-us/library/ms524948.aspx</a></span></p>
<p><span style="font-family:courier new,courier; font-size:small">MSDN: Response Object</span><br>
<span style="font-family:courier new,courier; font-size:small"><a href="http://msdn.microsoft.com/en-us/library/ms525405.aspx">http://msdn.microsoft.com/en-us/library/ms525405.aspx</a></span></p>
<p><br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt=""></a></div>
