# How to add checkbox and dropdownlist column in ASP.NET MVC WebGrid
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
- .NET
- Web App Development
## Topics
- ASP.NET
## Updated
- 09/21/2016
## Description

<h1><img id="154151" alt="" src="154151-8171.onecodesampletopbanner.png" width="696" height="58"></h1>
<h1>將核取方塊加入至 MVC Web &#26684;線 (CSASPNETWebGrid) 的每一列中</h1>
<h2>簡介</h2>
<p>這個範例將示範如何將核取方塊加入至 MVC Web &#26684;線中。您可以在程式碼範例中找到下列問題的答案：</p>
<ul>
<li>如何擴充 WebGrid 協助程式以提供較佳的核取方塊支援？ </li><li>如何將核取方塊加入至 Web &#26684;線？ </li></ul>
<p>&nbsp;</p>
<h2>執行範例</h2>
<p>您可以直接執行這個範例。</p>
<p>執行專案之後，您可以看到如下的結果：<strong>&nbsp;</strong><em>&nbsp;</em></p>
<h1><img id="154152" alt="" src="154152-image.png" width="1041" height="674"></h1>
<h2>使用程式碼</h2>
<p>程式碼範例為 WebGrid 提供下列可重複使用的函式。</p>
<h3>如何擴充 WebGrid 協助程式以提供較佳的核取方塊支援？<strong>&nbsp;</strong><em></em></h3>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">编辑脚本</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">namespace System.Web.Helpers
{
    public static class WebGridExtension
    {
        public static IHtmlString GetHtmlWithSelectAllCheckBox(this WebGrid webGrid, string tableStyle = null,
            string headerStyle = null, string footerStyle = null, string rowStyle = null,
            string alternatingRowStyle = null, string selectedRowStyle = null,
            string caption = null, bool displayHeader = true, bool fillEmptyRows = false,
            string emptyRowCellValue = null, IEnumerable&lt;WebGridColumn&gt; columns = null,
            IEnumerable&lt;string&gt; exclusions = null, WebGridPagerModes mode = WebGridPagerModes.All,
            string firstText = null, string previousText = null, string nextText = null,
            string lastText = null, int numericLinksCount = 5, object htmlAttributes = null,
            string checkBoxValue = &quot;ID&quot;)
        {
            var newColumn = webGrid.Column(header: &quot;{}&quot;,
                format: item =&gt; new HelperResult(writer =&gt;
                {
                    writer.Write(&quot;&lt;input class=\&quot;singleCheckBox\&quot; name=\&quot;selectedRows\&quot; value=\&quot;&quot;
                    &#43; item.Value.GetType().GetProperty(checkBoxValue).GetValue(item.Value, null).ToString()
                    &#43; &quot;\&quot; type=\&quot;checkbox\&quot; /&gt;&quot;
                    );
                }));
            var newColumns = columns.ToList();
            newColumns.Insert(0, newColumn);
            var script = @&quot;&lt;script&gt;
                if (typeof jQuery == 'undefined')
                {
                    document.write(
                        unescape(
                        &quot;&quot;%3Cscript src='http://ajax.aspnetcdn.com/ajax/jquery/jquery-1.9.0.min.js'%3E%3C/script%3E&quot;&quot;
                        )
                     );
                }
                (function(){
                    window.setTimeout(function() { initializeCheckBoxes();  }, 1000);
                    function initializeCheckBoxes(){   
                        $(function () {
                            $('#allCheckBox').live('click',function () {
                                var isChecked = $(this).attr('checked');                       
                                $('.singleCheckBox').attr('checked', isChecked  ? true: false);
                                $('.singleCheckBox').closest('tr').addClass(isChecked  ? 'selected-row': 'not-selected-row');
                                $('.singleCheckBox').closest('tr').removeClass(isChecked  ? 'not-selected-row': 'selected-row');
                            });
                            $('.singleCheckBox').live('click',function () {
                                var isChecked = $(this).attr('checked');
                                $(this).closest('tr').addClass(isChecked  ? 'selected-row': 'not-selected-row');
                                $(this).closest('tr').removeClass(isChecked  ? 'not-selected-row': 'selected-row');
                                if(isChecked &amp;&amp; $('.singleCheckBox').length == $('.selected-row').length)
                                     $('#allCheckBox').attr('checked',true);
                                else
                                    $('#allCheckBox').attr('checked',false);
                            });
                        });
                    }
                })();
            &lt;/script&gt;&quot;;
            var html = webGrid.GetHtml(tableStyle, headerStyle, footerStyle, rowStyle,
                                          alternatingRowStyle, selectedRowStyle, caption,
                                          displayHeader, fillEmptyRows, emptyRowCellValue,
                                          newColumns, exclusions, mode, firstText,
                                          previousText, nextText, lastText,
                                          numericLinksCount, htmlAttributes
                                          );

            return MvcHtmlString.Create(html.ToString().Replace(&quot;{}&quot;,
                                        &quot;&lt;input type='checkbox' id='allCheckBox'/&gt;&quot;) &#43; script);
        }  
    }
}
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">namespace</span>&nbsp;System.Web.Helpers&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">class</span>&nbsp;WebGridExtension&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;IHtmlString&nbsp;GetHtmlWithSelectAllCheckBox(<span class="cs__keyword">this</span>&nbsp;WebGrid&nbsp;webGrid,&nbsp;<span class="cs__keyword">string</span>&nbsp;tableStyle&nbsp;=&nbsp;<span class="cs__keyword">null</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;headerStyle&nbsp;=&nbsp;<span class="cs__keyword">null</span>,&nbsp;<span class="cs__keyword">string</span>&nbsp;footerStyle&nbsp;=&nbsp;<span class="cs__keyword">null</span>,&nbsp;<span class="cs__keyword">string</span>&nbsp;rowStyle&nbsp;=&nbsp;<span class="cs__keyword">null</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;alternatingRowStyle&nbsp;=&nbsp;<span class="cs__keyword">null</span>,&nbsp;<span class="cs__keyword">string</span>&nbsp;selectedRowStyle&nbsp;=&nbsp;<span class="cs__keyword">null</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;caption&nbsp;=&nbsp;<span class="cs__keyword">null</span>,&nbsp;<span class="cs__keyword">bool</span>&nbsp;displayHeader&nbsp;=&nbsp;<span class="cs__keyword">true</span>,&nbsp;<span class="cs__keyword">bool</span>&nbsp;fillEmptyRows&nbsp;=&nbsp;<span class="cs__keyword">false</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;emptyRowCellValue&nbsp;=&nbsp;<span class="cs__keyword">null</span>,&nbsp;IEnumerable&lt;WebGridColumn&gt;&nbsp;columns&nbsp;=&nbsp;<span class="cs__keyword">null</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;IEnumerable&lt;<span class="cs__keyword">string</span>&gt;&nbsp;exclusions&nbsp;=&nbsp;<span class="cs__keyword">null</span>,&nbsp;WebGridPagerModes&nbsp;mode&nbsp;=&nbsp;WebGridPagerModes.All,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;firstText&nbsp;=&nbsp;<span class="cs__keyword">null</span>,&nbsp;<span class="cs__keyword">string</span>&nbsp;previousText&nbsp;=&nbsp;<span class="cs__keyword">null</span>,&nbsp;<span class="cs__keyword">string</span>&nbsp;nextText&nbsp;=&nbsp;<span class="cs__keyword">null</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;lastText&nbsp;=&nbsp;<span class="cs__keyword">null</span>,&nbsp;<span class="cs__keyword">int</span>&nbsp;numericLinksCount&nbsp;=&nbsp;<span class="cs__number">5</span>,&nbsp;<span class="cs__keyword">object</span>&nbsp;htmlAttributes&nbsp;=&nbsp;<span class="cs__keyword">null</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;checkBoxValue&nbsp;=&nbsp;<span class="cs__string">&quot;ID&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;newColumn&nbsp;=&nbsp;webGrid.Column(header:&nbsp;<span class="cs__string">&quot;{}&quot;</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;format:&nbsp;item&nbsp;=&gt;&nbsp;<span class="cs__keyword">new</span>&nbsp;HelperResult(writer&nbsp;=&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;writer.Write(<span class="cs__string">&quot;&lt;input&nbsp;class=\&quot;singleCheckBox\&quot;&nbsp;name=\&quot;selectedRows\&quot;&nbsp;value=\&quot;&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&#43;&nbsp;item.Value.GetType().GetProperty(checkBoxValue).GetValue(item.Value,&nbsp;<span class="cs__keyword">null</span>).ToString()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&#43;&nbsp;<span class="cs__string">&quot;\&quot;&nbsp;type=\&quot;checkbox\&quot;&nbsp;/&gt;&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;newColumns&nbsp;=&nbsp;columns.ToList();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;newColumns.Insert(<span class="cs__number">0</span>,&nbsp;newColumn);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;script&nbsp;=&nbsp;@&quot;&lt;script&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(<span class="cs__keyword">typeof</span>&nbsp;jQuery&nbsp;==&nbsp;<span class="cs__string">'undefined'</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;document.write(&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;unescape(&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;&quot;</span>%3Cscript&nbsp;src=<span class="cs__string">'http://ajax.aspnetcdn.com/ajax/jquery/jquery-1.9.0.min.js'</span>%3E%3C/script%3E<span class="cs__string">&quot;&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;(function(){&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;window.setTimeout(function()&nbsp;{&nbsp;initializeCheckBoxes();&nbsp;&nbsp;},&nbsp;<span class="cs__number">1000</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;function&nbsp;initializeCheckBoxes(){&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;$(function&nbsp;()&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;$(<span class="cs__string">'#allCheckBox'</span>).live(<span class="cs__string">'click'</span>,function&nbsp;()&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;isChecked&nbsp;=&nbsp;$(<span class="cs__keyword">this</span>).attr(<span class="cs__string">'checked'</span>);&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;$(<span class="cs__string">'.singleCheckBox'</span>).attr(<span class="cs__string">'checked'</span>,&nbsp;isChecked&nbsp;&nbsp;?&nbsp;<span class="cs__keyword">true</span>:&nbsp;<span class="cs__keyword">false</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;$(<span class="cs__string">'.singleCheckBox'</span>).closest(<span class="cs__string">'tr'</span>).addClass(isChecked&nbsp;&nbsp;?&nbsp;<span class="cs__string">'selected-row'</span>:&nbsp;<span class="cs__string">'not-selected-row'</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;$(<span class="cs__string">'.singleCheckBox'</span>).closest(<span class="cs__string">'tr'</span>).removeClass(isChecked&nbsp;&nbsp;?&nbsp;<span class="cs__string">'not-selected-row'</span>:&nbsp;<span class="cs__string">'selected-row'</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;});&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;$(<span class="cs__string">'.singleCheckBox'</span>).live(<span class="cs__string">'click'</span>,function&nbsp;()&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;isChecked&nbsp;=&nbsp;$(<span class="cs__keyword">this</span>).attr(<span class="cs__string">'checked'</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;$(<span class="cs__keyword">this</span>).closest(<span class="cs__string">'tr'</span>).addClass(isChecked&nbsp;&nbsp;?&nbsp;<span class="cs__string">'selected-row'</span>:&nbsp;<span class="cs__string">'not-selected-row'</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;$(<span class="cs__keyword">this</span>).closest(<span class="cs__string">'tr'</span>).removeClass(isChecked&nbsp;&nbsp;?&nbsp;<span class="cs__string">'not-selected-row'</span>:&nbsp;<span class="cs__string">'selected-row'</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>(isChecked&nbsp;&amp;&amp;&nbsp;$(<span class="cs__string">'.singleCheckBox'</span>).length&nbsp;==&nbsp;$(<span class="cs__string">'.selected-row'</span>).length)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;$(<span class="cs__string">'#allCheckBox'</span>).attr(<span class="cs__string">'checked'</span>,<span class="cs__keyword">true</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;$(<span class="cs__string">'#allCheckBox'</span>).attr(<span class="cs__string">'checked'</span>,<span class="cs__keyword">false</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;});&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;});&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;})();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;/script&gt;&quot;;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;html&nbsp;=&nbsp;webGrid.GetHtml(tableStyle,&nbsp;headerStyle,&nbsp;footerStyle,&nbsp;rowStyle,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;alternatingRowStyle,&nbsp;selectedRowStyle,&nbsp;caption,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;displayHeader,&nbsp;fillEmptyRows,&nbsp;emptyRowCellValue,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;newColumns,&nbsp;exclusions,&nbsp;mode,&nbsp;firstText,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;previousText,&nbsp;nextText,&nbsp;lastText,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;numericLinksCount,&nbsp;htmlAttributes&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;MvcHtmlString.Create(html.ToString().Replace(<span class="cs__string">&quot;{}&quot;</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;&lt;input&nbsp;type='checkbox'&nbsp;id='allCheckBox'/&gt;&quot;</span>)&nbsp;&#43;&nbsp;script);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<h1><span lang="EN-US">如何將核取方塊加入至 Web &#26684;線</span></h1>
<p><span lang="EN-US"></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">编辑脚本</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">html</span>
<pre class="hidden">@model IEnumerable&lt;CSASPNETWebGrid.Models.Person&gt;
@{
    ViewBag.Title = &quot;Home Page&quot;; 
    var grid = new WebGrid(source: Model);
}

@section featured {
    &lt;section class=&quot;featured&quot;&gt;
        
        &lt;div class=&quot;content-wrapper&quot;&gt;
            &lt;hgroup class=&quot;title&quot;&gt;
                &lt;h1&gt;@ViewBag.Title.&lt;/h1&gt;
                &lt;h2&gt;@ViewBag.Message&lt;/h2&gt;
            &lt;/hgroup&gt;
          
               &lt;fieldset&gt;
                   &lt;legend&gt;Person&lt;/legend&gt;
                   @grid.GetHtmlWithSelectAllCheckBox(
                   tableStyle: &quot;grid&quot;, 
                   columns: grid.Columns(
                   grid.Column(columnName: &quot;Name&quot;),
                   grid.Column(columnName: &quot;Email&quot;),
                   grid.Column(columnName: &quot;Address&quot;)
                   ))
                   &lt;p&gt;
                       &lt;input type=&quot;submit&quot; value=&quot;Submit&quot; /&gt;
                   &lt;/p&gt;
               &lt;/fieldset&gt;
           
        &lt;/div&gt;
    &lt;/section&gt;
}


 
</pre>
<div class="preview">
<pre class="html">@model&nbsp;IEnumerable<span class="html__tag_start">&lt;CSASPNETWebGrid</span>.Models.Person<span class="html__tag_start">&gt;&nbsp;
</span>@{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ViewBag.Title&nbsp;=&nbsp;&quot;Home&nbsp;Page&quot;;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;grid&nbsp;=&nbsp;new&nbsp;WebGrid(source:&nbsp;Model);&nbsp;
}&nbsp;
&nbsp;
@section&nbsp;featured&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="html__tag_start">&lt;section</span><span class="html__attr_name">class</span>=<span class="html__attr_value">&quot;featured&quot;</span><span class="html__tag_start">&gt;&nbsp;
</span><span class="html__tag_start">&lt;div</span><span class="html__attr_name">class</span>=<span class="html__attr_value">&quot;content-wrapper&quot;</span><span class="html__tag_start">&gt;&nbsp;
</span><span class="html__tag_start">&lt;hgroup</span><span class="html__attr_name">class</span>=<span class="html__attr_value">&quot;title&quot;</span><span class="html__tag_start">&gt;&nbsp;
</span><span class="html__tag_start">&lt;h1</span><span class="html__tag_start">&gt;@</span>ViewBag.Title.<span class="html__tag_end">&lt;/h1&gt;</span><span class="html__tag_start">&lt;h2</span><span class="html__tag_start">&gt;@</span>ViewBag.Message<span class="html__tag_end">&lt;/h2&gt;</span><span class="html__tag_end">&lt;/hgroup&gt;</span><span class="html__tag_start">&lt;fieldset</span><span class="html__tag_start">&gt;&nbsp;
</span><span class="html__tag_start">&lt;legend</span><span class="html__tag_start">&gt;</span>Person<span class="html__tag_end">&lt;/legend&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;@grid.GetHtmlWithSelectAllCheckBox(&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;tableStyle:&nbsp;&quot;grid&quot;,&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;columns:&nbsp;grid.Columns(&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;grid.Column(columnName:&nbsp;&quot;Name&quot;),&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;grid.Column(columnName:&nbsp;&quot;Email&quot;),&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;grid.Column(columnName:&nbsp;&quot;Address&quot;)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="html__tag_start">&lt;p</span><span class="html__tag_start">&gt;&nbsp;
</span><span class="html__tag_start">&lt;input</span><span class="html__attr_name">type</span>=<span class="html__attr_value">&quot;submit&quot;</span><span class="html__attr_name">value</span>=<span class="html__attr_value">&quot;Submit&quot;</span><span class="html__tag_start">/&gt;</span><span class="html__tag_end">&lt;/p&gt;</span><span class="html__tag_end">&lt;/fieldset&gt;</span><span class="html__tag_end">&lt;/div&gt;</span><span class="html__tag_end">&lt;/section&gt;</span>&nbsp;
}&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;
</pre>
</div>
</div>
</div>
</span>
<p></p>
<p><span style="color:#ffffff">Microsoft All-In-One Code Framework 是為克服開發人員所實際遇到的困難和需求，所因應而生的免費且集中式程式碼範例程式庫。目標是要為所有的 Microsoft 開發技術提供客戶導向的程式碼範例，並減少開發人員花費在解決一般程式設計工作上的心力。我們的團隊透過 MSDN 論壇、社交媒體及各種開發社群傾聽程式開發人員的難題。我們根據開發人員的常見程式設計工作問題來撰寫程式碼，並透過快速的範例發佈週期讓開發人員下載程式碼。此外，我們也提供免費的程式碼範例要求服務。這是一種主動方式，可讓我們的開發人員社群直接從
 Microsoft 取得程式碼範例。</span></p>
<p><span style="color:#ffffff">Microsoft All-In-One Code Framework 是為克服開發人員所實際遇到的困難和需求，所因應而生的免費且集中式程式碼範例程式庫。目標是要為所有的 Microsoft 開發技術提供客戶導向的程式碼範例，並減少開發人員花費在解決一般程式設計工作上的心力。我們的團隊透過 MSDN 論壇、社交媒體及各種開發社群傾聽程式開發人員的難題。我們根據開發人員的常見程式設計工作問題來撰寫程式碼，並透過快速的範例發佈週期讓開發人員下載程式碼。此外，我們也提供免費的程式碼範例要求服務。這是一種主動方式，可讓我們的開發人員社群直接從
 Microsoft 取得程式碼範例。</span></p>
<p><span style="color:#ffffff">Microsoft All-In-One Code Framework 是為克服開發人員所實際遇到的困難和需求，所因應而生的免費且集中式程式碼範例程式庫。目標是要為所有的 Microsoft 開發技術提供客戶導向的程式碼範例，並減少開發人員花費在解決一般程式設計工作上的心力。我們的團隊透過 MSDN 論壇、社交媒體及各種開發社群傾聽程式開發人員的難題。我們根據開發人員的常見程式設計工作問題來撰寫程式碼，並透過快速的範例發佈週期讓開發人員下載程式碼。此外，我們也提供免費的程式碼範例要求服務。這是一種主動方式，可讓我們的開發人員社群直接從
 Microsoft 取得程式碼範例。<em>&nbsp;</em></span></p>
<p><span style="color:#ffffff"><strong></strong><em></em></span></p>
<p><span style="color:#ffffff"><em>&nbsp;</em></span></p>
<p><strong></strong><em></em></p>
