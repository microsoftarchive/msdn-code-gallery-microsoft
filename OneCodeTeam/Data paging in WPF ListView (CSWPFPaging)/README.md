# Data paging in WPF ListView (CSWPFPaging)
## Requires
- Visual Studio 2008
## License
- MS-LPL
## Technologies
- WPF
## Topics
- Controls
- Paging
## Updated
- 03/01/2012
## Description

<h1><span style="">Data paging in WPF ListView (<span class="SpellE">CSWPFPaging</span>)
</span></h1>
<h2>Introduction</h2>
<p class="MsoNormal"><br>
The sample demonstrates how to page data in WPF.<span style=""> </span></p>
<h2>Running the Sample<span style=""> </span></h2>
<p class="MsoNormal"><span style="">Press F5 to run this application, and press the
<b style="">First</b>, <b style="">Previous</b>, <b style="">Next</b> or <b style="">
Last</b> buttons, you will see that the <b style="">ListView</b> shows different page of items.
</span></p>
<p class="MsoNormal"><span style=""><img src="53241-image.png" alt="" width="402" height="391" align="middle">
<img src="53242-image.png" alt="" width="402" height="391" align="middle">
</span><span style=""></span></p>
<h2>Using the Code<span style=""> </span></h2>
<p class="MsoListParagraphCxSpFirst" style="margin-left:54.0pt"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Create a Customer class with properties of ID, Name, Age, Country, etc.
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:54.0pt"><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Define a ListView with columns binding to each properties of the Customer object;
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:54.0pt"><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Drag 4 buttons on to the <span class="SpellE">
<b style="">MainWindow</b></span>, which are for displaying <b style="">First, Previous, Next, Last
</b>page. </span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:54.0pt"><span style=""><span style="">4.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Construct an <span class="SpellE"><b style="">ObservableCollection</b></span> collection of Customer objects.
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:54.0pt"><span style=""><span style="">5.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Create a <span class="SpellE"><b style="">CollectionViewSource</b></span> object and set source to the customer list.
</span></p>
<p class="MsoListParagraphCxSpLast" style="margin-left:54.0pt"><span style=""><span style="">6.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Handle the <span class="SpellE"><b style="">CollectionViewSource</b>.<b style="">Filter</b></span> event to show data only in the current page.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
void view_Filter(object sender, FilterEventArgs e)
{
    int index = customers.IndexOf((Customer)e.Item);


    if (index &gt;= itemPerPage * currentPageIndex && index &lt; 
        itemPerPage * (currentPageIndex &#43; 1))
    {
        e.Accepted = true;
    }
    else
    {
        e.Accepted = false;
    }
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraph" style="margin-left:54.0pt"><span style=""><span style="">7.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Binding the <span class="SpellE"><b style="">CollectionViewSource</b></span> object to the ListView.
</span></p>
<h2>More Information<span style=""> </span></h2>
<p class="MsoNormal"><span style=""><a href="http://msdn.microsoft.com/en-us/library/ms668604.aspx">ObservableCollection&lt;T&gt; Class</a></span><span style="">
</span></p>
<p class="MsoNormal"><span style=""><a href="http://msdn.microsoft.com/en-us/library/system.windows.data.collectionviewsource.aspx">CollectionViewSource Class</a>
</span></p>
<p class="MsoNormal"><span style=""><a href="http://msdn.microsoft.com/en-us/library/system.windows.controls.listview.aspx">ListView Class</a>
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
