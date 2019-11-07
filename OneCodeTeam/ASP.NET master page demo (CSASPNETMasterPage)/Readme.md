# ASP.NET master page demo (CSASPNETMasterPage)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- ASP.NET
## Topics
- Master Page
## Updated
- 03/04/2012
## Description

<h1>ASP.NET master page demo (<span class="SpellE">CSASPNETMasterPage</span>)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The project illustrates how to use Master Page. ASP.NET defines two new specialized types of pages: Master Page and Content Page. A Master page is a page template. Like an ordinary ASP.NET web page, it can contain any combination of HTML.
 In addition, Master Page includes a special control called ContentPlaceHolder which works as a holder of Content Page. On the other hand, each Content Page references a single Master Page and acquires its content. Both Master Page and Content Page work together
 can allow developers easier to build websites with a standard appearance.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Please follow these demonstration steps below.</p>
<p class="MsoNormal">Step 1:&nbsp;Open the CSASPNETMasterPage.sln. Expand the <span class="SpellE">
CSASPNETMasterPage</span> web application and press Ctrl &#43; F5 to show the ContentPage.aspx.
</p>
<p class="MsoNormal">Step 2: We can find a little complex web page in browser, the page includes a master page and a content page, we can input some text in content page��s TextBox control and it will display in master page, the content page is embedded in
 master page. </p>
<p class="MsoNormal"><span style=""><img src="53621-image.png" alt="" width="529" height="527" align="middle">
</span></p>
<p class="MsoNormal">Step 3: Click gotoNestedContentPage link, we can see the content page is changed. There is another master page as follows, and this master page includes two content pages, you can change the page by clicking the links in left.</p>
<p class="MsoNormal"><span style=""><img src="53622-image.png" alt="" width="529" height="527" align="middle">
</span></p>
<p class="MsoNormal">Step 4: Validation finished.</p>
<h2>Using the Code</h2>
<p class="MsoNormal">Code Logical : </p>
<p class="MsoNormal">Step 1. Create a C# &quot;ASP.NET Empty Web Application&quot; in Visual Studio 2010. Name it as ��<span class="SpellE">CSASPNETMasterPage</span>&quot;.<span style="">&nbsp;
</span>Create three web form pages and two master pages, the ��Master.master�� page is the main master page, it includes all content pages and child master pages inside, the ��ContentPage.aspx�� page is the content page with some server controls, the ��NestedMaster.master��,
 ��NestedContentPageA.aspx��, ��NestedContentPageB.aspx�� pages are nested in Master.master page.</p>
<p class="MsoNormal">Step 2. The Master page includes a ContentPlaceHolder control for nesting in content page; the two Hyperlinks will redirect you to different content pages with ContentPage.aspx and NestedMasterPage.master.
</p>
<h3>The following code showing the Master.master page HTML markup and use ContentPlaceHolder to embed content page:</h3>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">html</span>
<pre class="hidden">
&lt;html xmlns=&quot;http://www.w3.org/1999/xhtml&quot;&gt;
&lt;head runat=&quot;server&quot;&gt;
    &lt;title&gt;&lt;/title&gt;
&lt;/head&gt;
&lt;body style=&quot;background-color:#F7F6F3&quot;&gt;
    &lt;form id=&quot;form1&quot; runat=&quot;server&quot;&gt;
    <div>
        <h2>Master Page</h2>
        <p>
            &lt;asp:Label ID=&quot;lbHello&quot; runat=&quot;server&quot;&gt;&lt;/asp:Label&gt;
        </p>
        &lt;asp:HyperLink ID=&quot;linkToContentPage&quot; runat=&quot;server&quot; 
                       NavigateUrl=&quot;~/ContentPage.aspx&quot; 
                       Text=&quot;gotoContentPage&quot;&gt;&lt;/asp:HyperLink&gt;&nbsp;


        &lt;asp:HyperLink ID=&quot;linkToNestedContentPage&quot; runat=&quot;server&quot; 
                       NavigateUrl=&quot;~/NestedContentPageA.aspx&quot;
                       Text=&quot;gotoNestedContentPage&quot;&gt;&lt;/asp:HyperLink&gt;
        <hr>
        &lt;%--Content Page Begin--%&gt;
        &lt;asp:ContentPlaceHolder ID=&quot;MainContentHolder&quot; runat=&quot;server&quot;&gt;
        &lt;/asp:ContentPlaceHolder&gt;
        &lt;%--Content Page End--%&gt;
      
    </div>
    &lt;/form&gt;
&lt;/body&gt;
&lt;/html&gt;

</pre>
<pre id="codePreview" class="html">
&lt;html xmlns=&quot;http://www.w3.org/1999/xhtml&quot;&gt;
&lt;head runat=&quot;server&quot;&gt;
    &lt;title&gt;&lt;/title&gt;
&lt;/head&gt;
&lt;body style=&quot;background-color:#F7F6F3&quot;&gt;
    &lt;form id=&quot;form1&quot; runat=&quot;server&quot;&gt;
    <div>
        <h2>Master Page</h2>
        <p>
            &lt;asp:Label ID=&quot;lbHello&quot; runat=&quot;server&quot;&gt;&lt;/asp:Label&gt;
        </p>
        &lt;asp:HyperLink ID=&quot;linkToContentPage&quot; runat=&quot;server&quot; 
                       NavigateUrl=&quot;~/ContentPage.aspx&quot; 
                       Text=&quot;gotoContentPage&quot;&gt;&lt;/asp:HyperLink&gt;&nbsp;


        &lt;asp:HyperLink ID=&quot;linkToNestedContentPage&quot; runat=&quot;server&quot; 
                       NavigateUrl=&quot;~/NestedContentPageA.aspx&quot;
                       Text=&quot;gotoNestedContentPage&quot;&gt;&lt;/asp:HyperLink&gt;
        <hr>
        &lt;%--Content Page Begin--%&gt;
        &lt;asp:ContentPlaceHolder ID=&quot;MainContentHolder&quot; runat=&quot;server&quot;&gt;
        &lt;/asp:ContentPlaceHolder&gt;
        &lt;%--Content Page End--%&gt;
      
    </div>
    &lt;/form&gt;
&lt;/body&gt;
&lt;/html&gt;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"></p>
<p class="MsoNormal"><span class="GramE">Step 3.</span> The <span class="SpellE">
NestedMaster</span> page is similar to Master page, you need to change Hyperlink��s NavigateUrl property and write different message in content pages. The another thing we need to handle is that we have to pass the massage in ContentPage to Master page, here
 we use <span class="GramE">Master.FindControl(</span>&quot;lbHello&quot;) method to find master page��s controls.</p>
<h3>The following code is used to find master page��s control and assign value to Label control.</h3>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
protected void Button1_Click(object sender, EventArgs e)
{
    Label lbMasterPageHello = Master.FindControl(&quot;lbHello&quot;) as Label;


    if (lbMasterPageHello != null)
    {
        lbMasterPageHello.Text = &quot;Hello, &quot; &#43; txtName.Text &#43; &quot;!&quot;;
    }
}

</pre>
<pre id="codePreview" class="csharp">
protected void Button1_Click(object sender, EventArgs e)
{
    Label lbMasterPageHello = Master.FindControl(&quot;lbHello&quot;) as Label;


    if (lbMasterPageHello != null)
    {
        lbMasterPageHello.Text = &quot;Hello, &quot; &#43; txtName.Text &#43; &quot;!&quot;;
    }
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"></p>
<p class="MsoNormal">Step 4. Build the application and you can debug it.</p>
<p class="MsoNormal"></p>
<h2>More Information</h2>
<p class="MsoListParagraphCxSpFirst" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/wtxbf3hh.aspx">ASP.NET Master Pages Overview</a></p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/x2b3ktt7.aspx">Nested ASP.NET Master Pages</a></p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style=""><a href="http://msdn.microsoft.com/en-us/library/fft2ye18.aspx">Create Content Pages for an ASP.NET Master Page</a>
</span></p>
<p class="MsoListParagraphCxSpLast" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style=""><a href="http://msdn.microsoft.com/en-us/library/xxwa0ff0.aspx">How to: Reference ASP.NET Master Page Content</a>
</span></p>
<p class="MsoNormal"></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
