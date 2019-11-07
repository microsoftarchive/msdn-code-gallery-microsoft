# How to create <ul> elements dynamically in ASP.NET
## Requires
- Visual Studio 2013
## License
- MIT
## Technologies
- ASP.NET
- .NET
- Web App Development
## Topics
- Dynamically
- ul
## Updated
- 09/22/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode"><img src=":-onecodesampletopbanner1" alt=""></a><strong>&nbsp;</strong><em>&nbsp;</em></div>
<h2 class="MsoNormal">Introduction</h2>
<p class="MsoNormal">UL is not a server control in ASP.NET. Dynamically creating UL is a very common requirement in ASP.NET forum. This article and the attached code samples demonstrate how to dynamically create UL elements and LI elements.</p>
<h2 class="MsoNormal">Running the sample</h2>
<p class="MsoNormal">You can directly run this sample with Ctrl&#43;F5.</p>
<p class="MsoNormal">After the application starts, you can see the default.aspx page as shown below:</p>
<p class="MsoNormal"><span><img src="156776-image.png" alt="" width="500" height="293" align="middle">
</span></p>
<p class="MsoNormal">Click the &quot;Create a new UL&quot; button and then hover the mouse on the created item, you will see some buttons. Try to click those buttons you see, you will get something like this:</p>
<p class="MsoNormal"><span><img src="156777-image.png" alt="" width="621" height="307" align="middle">
</span></p>
<p class="MsoNormal">You can click the &quot;save it&quot; button to get the tree's value in the code-behind file.</p>
<h2 class="MsoNormal">Using the code</h2>
<p class="MsoNormal"><span class="auto-style11">The code sample provides the following reusable functions for dynamically creating UL in ASP.NET.</span></p>
<h3><strong>How to dynamically create UL/LI elements?</strong></h3>
<p class="MsoNormal">&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>JavaScript</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">js</span>
<pre class="hidden">//Create a UL element and attach a Li Element in order to let this UL viewable.
        function createUL(event) {
  var ul = jQuery('&lt;ul&gt;', { text: &quot;&gt;&quot; });
  ul.append(jQuery('&lt;button&gt;', {text: 'Add Li', class: 'btn', onclick: 'addLi(event);return false;' }));
  ul.append(jQuery('&lt;button&gt;', { text: 'Add sub ul', class: 'btn', onclick: 'createUL(event);return false;' }));
  ul.append(jQuery('&lt;button&gt;', { text: 'Delete ul', class: 'btn', onclick: 'deleteUL(event);return false;' }));
 
  ul.append(newLi());
  $(ul).hover(function () {
      $(ul).children(&quot;button&quot;).show();
  },
  function () {
      { $(ul).children(&quot;button&quot;).hide(); }
  }
  );
  $(event.target).parent().append(ul);
        }
 
        //return a new li element with many events
        function newLi(liValue) {
  var li;
  if (liValue) {
      li = jQuery(
          '&lt;li&gt;',
         {
             text: liValue,
             id: clientID&#43;&#43;
         });
  }
  else {
      li = jQuery('&lt;li&gt;',
         {
             text: 'new value',
             id: clientID&#43;&#43;
         });
  }
  
  $(li).click(function () {
      var div = jQuery('&lt;div&gt;');
      var textbox = jQuery(&quot;&lt;input type='text'&gt;&quot;);
      $(textbox).val(li.text());
      $(textbox).attr(&quot;id&quot;, clientID&#43;&#43;);
      $(textbox).css(&quot;Width&quot;, div.css(&quot;Width&quot;));
      $(textbox).keypress(function (e) {
          if (e.charCode == 13|| e.keyCode==13) {
              if ($(textbox).val().length == 0)
                  deleteLi(textbox);
              else
                  textbox.replaceWith(newLi($(textbox).val()));
              return false;
          }
      });
 
      $(textbox).mouseleave(function () {
          $(document).mousedown(function (event) {
              if ($(event.target).attr(&quot;id&quot;) != $(textbox).attr(&quot;id&quot;)) {
                  if ($(textbox).val().length == 0) 
                      deleteLi(textbox);
                  else
                      textbox.replaceWith(newLi($(textbox).val()));
              }
          });
      });
 
     
 
      $(div).append(textbox);
      $(this).replaceWith(div).val($(this).text());
      
  });
 
  return li;
        }
 
        //Delete li 
        //target is a textbox
        function deleteLi(target) {
  var ul = $(target).parent().parent();
  $(target).parent().remove();
  if ($(ul).find($('li')).length==0){
      ul.remove();
  }
        }
        
        function addLi(event) {
  $(event.target).parent().append(newLi());
        }
 
        function deleteUL(event) {
  $(event.target).parent().remove();
        }
</pre>
<div class="preview">
<pre class="js"><span class="js__sl_comment">//Create&nbsp;a&nbsp;UL&nbsp;element&nbsp;and&nbsp;attach&nbsp;a&nbsp;Li&nbsp;Element&nbsp;in&nbsp;order&nbsp;to&nbsp;let&nbsp;this&nbsp;UL&nbsp;viewable.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__operator">function</span>&nbsp;createUL(event)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;ul&nbsp;=&nbsp;jQuery(<span class="js__string">'&lt;ul&gt;'</span>,&nbsp;<span class="js__brace">{</span>&nbsp;text:&nbsp;<span class="js__string">&quot;&gt;&quot;</span>&nbsp;<span class="js__brace">}</span>);&nbsp;
&nbsp;&nbsp;ul.append(jQuery(<span class="js__string">'&lt;button&gt;'</span>,&nbsp;<span class="js__brace">{</span>text:&nbsp;<span class="js__string">'Add&nbsp;Li'</span>,&nbsp;class:&nbsp;<span class="js__string">'btn'</span>,&nbsp;onclick:&nbsp;<span class="js__string">'addLi(event);return&nbsp;false;'</span>&nbsp;<span class="js__brace">}</span>));&nbsp;
&nbsp;&nbsp;ul.append(jQuery(<span class="js__string">'&lt;button&gt;'</span>,&nbsp;<span class="js__brace">{</span>&nbsp;text:&nbsp;<span class="js__string">'Add&nbsp;sub&nbsp;ul'</span>,&nbsp;class:&nbsp;<span class="js__string">'btn'</span>,&nbsp;onclick:&nbsp;<span class="js__string">'createUL(event);return&nbsp;false;'</span>&nbsp;<span class="js__brace">}</span>));&nbsp;
&nbsp;&nbsp;ul.append(jQuery(<span class="js__string">'&lt;button&gt;'</span>,&nbsp;<span class="js__brace">{</span>&nbsp;text:&nbsp;<span class="js__string">'Delete&nbsp;ul'</span>,&nbsp;class:&nbsp;<span class="js__string">'btn'</span>,&nbsp;onclick:&nbsp;<span class="js__string">'deleteUL(event);return&nbsp;false;'</span>&nbsp;<span class="js__brace">}</span>));&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;ul.append(newLi());&nbsp;
&nbsp;&nbsp;$(ul).hover(<span class="js__operator">function</span>&nbsp;()&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;$(ul).children(<span class="js__string">&quot;button&quot;</span>).show();&nbsp;
&nbsp;&nbsp;<span class="js__brace">}</span>,&nbsp;
&nbsp;&nbsp;<span class="js__operator">function</span>&nbsp;()&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;$(ul).children(<span class="js__string">&quot;button&quot;</span>).hide();&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;);&nbsp;
&nbsp;&nbsp;$(event.target).parent().append(ul);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//return&nbsp;a&nbsp;new&nbsp;li&nbsp;element&nbsp;with&nbsp;many&nbsp;events</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__operator">function</span>&nbsp;newLi(liValue)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;li;&nbsp;
&nbsp;&nbsp;<span class="js__statement">if</span>&nbsp;(liValue)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;li&nbsp;=&nbsp;jQuery(&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__string">'&lt;li&gt;'</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;text:&nbsp;liValue,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;id:&nbsp;clientID&#43;&#43;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>);&nbsp;
&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;<span class="js__statement">else</span>&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;li&nbsp;=&nbsp;jQuery(<span class="js__string">'&lt;li&gt;'</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;text:&nbsp;<span class="js__string">'new&nbsp;value'</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;id:&nbsp;clientID&#43;&#43;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>);&nbsp;
&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;$(li).click(<span class="js__operator">function</span>&nbsp;()&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;div&nbsp;=&nbsp;jQuery(<span class="js__string">'&lt;div&gt;'</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;textbox&nbsp;=&nbsp;jQuery(<span class="js__string">&quot;&lt;input&nbsp;type='text'&gt;&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;$(textbox).val(li.text());&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;$(textbox).attr(<span class="js__string">&quot;id&quot;</span>,&nbsp;clientID&#43;&#43;);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;$(textbox).css(<span class="js__string">&quot;Width&quot;</span>,&nbsp;div.css(<span class="js__string">&quot;Width&quot;</span>));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;$(textbox).keypress(<span class="js__operator">function</span>&nbsp;(e)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">if</span>&nbsp;(e.charCode&nbsp;==&nbsp;<span class="js__num">13</span>||&nbsp;e.keyCode==<span class="js__num">13</span>)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">if</span>&nbsp;($(textbox).val().length&nbsp;==&nbsp;<span class="js__num">0</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;deleteLi(textbox);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;textbox.replaceWith(newLi($(textbox).val()));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">return</span>&nbsp;false;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;$(textbox).mouseleave(<span class="js__operator">function</span>&nbsp;()&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;$(document).mousedown(<span class="js__operator">function</span>&nbsp;(event)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">if</span>&nbsp;($(event.target).attr(<span class="js__string">&quot;id&quot;</span>)&nbsp;!=&nbsp;$(textbox).attr(<span class="js__string">&quot;id&quot;</span>))&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">if</span>&nbsp;($(textbox).val().length&nbsp;==&nbsp;<span class="js__num">0</span>)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;deleteLi(textbox);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;textbox.replaceWith(newLi($(textbox).val()));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;$(div).append(textbox);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;$(<span class="js__operator">this</span>).replaceWith(div).val($(<span class="js__operator">this</span>).text());&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;<span class="js__brace">}</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;<span class="js__statement">return</span>&nbsp;li;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//Delete&nbsp;li&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//target&nbsp;is&nbsp;a&nbsp;textbox</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__operator">function</span>&nbsp;deleteLi(target)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;ul&nbsp;=&nbsp;$(target).parent().parent();&nbsp;
&nbsp;&nbsp;$(target).parent().remove();&nbsp;
&nbsp;&nbsp;<span class="js__statement">if</span>&nbsp;($(ul).find($(<span class="js__string">'li'</span>)).length==<span class="js__num">0</span>)<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ul.remove();&nbsp;
&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__operator">function</span>&nbsp;addLi(event)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;$(event.target).parent().append(newLi());&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__operator">function</span>&nbsp;deleteUL(event)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;$(event.target).parent().remove();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p>&nbsp;</p>
<h2 class="MsoNormal">More Information</h2>
<p class="MsoNormal"><span style="font-family:Symbol"><span>&bull;<span style="font:7pt/normal &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/bb412179.aspx" style="text-indent:-0.25in">http://msdn.microsoft.com/en-us/library/bb412179.aspx</a></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
