# How to check the spelling in ASP.NET TextBox
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
## Topics
- Spell Checking
## Updated
- 09/22/2016
## Description

<h1><em><img id="154477" src="154477-8171.onecodesampletopbanner.png" alt="" width="696" height="58"></em></h1>
<h1><span><span>如何在 ASP.NET 文字方塊中檢查拼字 </span></span></h1>
<h2><span><span>簡介</span></span></h2>
<p><span><span>此專案示範如何 </span><span>檢查 </span><span>文字方塊</span><span> </span><span>中的拼字是否正確。 此範例程式碼會透過
</span><span></span><span>MS Word </span><span>拼字檢查</span><span>元件檢查使用者的輸入文字。</span></span></p>
<h2><span><span>建置範例</span></span></h2>
<p><span><span>請依照下列示範步驟執行。</span></span></p>
<p><span><span>步驟 1： 開啟專案。</span></span></p>
<p><span><span>步驟 2： 展開 CS/VBASPNETCheckSpellingWritten Web 應用程式，並按下 Ctrl&#43;F5 以顯示 Default.aspx。</span></span></p>
<p><span><span>步驟 3： 我們會在頁面上看到文字方塊控制項 </span><span>。 您可以按下 &quot;Check&quot; </span><span>按鈕以直接檢查文字方塊中
</span><span>拼字錯誤的</span><span>文字 </span><span>，或 </span><span>輸入</span><span>某些其他文字，</span><span>然後按一下 &ldquo;Check&rdquo;。</span></span></p>
<p><span><span>步驟 4： 此程式碼範例會顯示</span><span>文字的對話方塊，請</span><span> </span><span>從建議文字清單選擇正確的文字</span><span>或自行輸入文字</span><span>。</span></span></p>
<p><span><span>步驟 5： 如果一切順利，請關閉</span><span>對話方塊，您便會發現 </span><span>系統已修改您的文字。</span></span></p>
<p><span><span>步驟 6： 驗證完成。</span></span></p>
<h2><span><span>使用程式碼</span></span></h2>
<p><span><span>在按鈕的 Click 事件中，新增</span><span>下方的程式碼</span><span>。</span></span> <strong>
&nbsp;</strong><em>&nbsp;</em></p>
<p></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span><span class="hidden">csharp</span>
<pre class="hidden">' Prevent multiple checker window. 
               If applicationWord IsNot Nothing Then 
                   Return 
               End If 
 
               applicationWord = New Microsoft.Office.Interop.Word.Application() 
               Dim errors As Integer = 0 
               If tbInput.Text.Length &gt; 0 Then 
                   Dim template As Object = Missing.Value 
                   Dim newTemplate As Object = Missing.Value 
                   Dim documentType As Object = Missing.Value 
                   Dim visible As Object = True 
 
                   ' 定義 MS Word 文件，我們會使用此文件計算錯誤數量，並
                   ' 叫用文件的 CheckSpelling 方法。
                   Dim documentCheck As Microsoft.Office.Interop.Word._Document =  
                      applicationWord.Documents.Add(template, newTemplate, documentType, visible) 
                   applicationWord.Visible = False 
                   documentCheck.Words.First.InsertBefore(tbInput.Text) 
                   Dim spellErrorsColl As Microsoft.Office.Interop.Word.ProofreadingErrors = documentCheck.SpellingErrors 
                   errors = spellErrorsColl.Count 
 
                   Dim [optional] As Object = Missing.Value 
                   documentCheck.Activate() 
                   documentCheck.CheckSpelling([optional], [optional], [optional], [optional], [optional], [optional], _ 
                       [optional], [optional], [optional], [optional], [optional], [optional]) 
                   documentCheck.LanguageDetected = True 
 
 
                  ' 使用者關閉對話方塊時，便會顯示錯誤訊息。
                  If errors = 0 Then 
                      lbMessage.Text = &quot;No errors&quot; 
                  Else 
                      lbMessage.Text = &quot;Total errors num:&quot; &amp; errors 
                  End If 
 
                  ' 取代文字方塊的錯誤文字。
                  Dim first As Object = 0 
                  Dim last As Object = documentCheck.Characters.Count - 1 
                  tbInput.Text = documentCheck.Range(first, last).Text 
              End If 
 
              Dim saveChanges As Object = False 
              Dim originalFormat As Object = Missing.Value 
              Dim routeDocument As Object = Missing.Value 
              DirectCast(applicationWord, _Application).Quit(saveChanges, originalFormat, routeDocument) 
              applicationWord = Nothing 


 </pre>
<pre class="hidden">if (applicationWord != null) 
              {
                  return; 
              } 
              applicationWord = new Microsoft.Office.Interop.Word.Application(); 
              int errors = 0; 
              if (tbInput.Text.Length &gt; 0) 
              {     
                  object template = Missing.Value; 
                  object newTemplate = Missing.Value; 
                  object documentType = Missing.Value; 
                  object visible = true; 
                  // 定義 MS Word 文件，然後我們會使用此文件計算錯誤數量，並
                  // 叫用文件的 CheckSpelling 方法。
                  Microsoft.Office.Interop.Word._Document documentCheck = applicationWord.Documents.Add(ref template, 
                    ref newTemplate, ref documentType, ref visible); 
                  applicationWord.Visible = false; 
                  documentCheck.Words.First.InsertBefore(tbInput.Text); 
                  Microsoft.Office.Interop.Word.ProofreadingErrors spellErrorsColl = documentCheck.SpellingErrors; 
                  errors = spellErrorsColl.Count; 
                  object optional = Missing.Value; 
                  documentCheck.Activate(); 
                  documentCheck.CheckSpelling(ref optional, ref optional, ref optional, ref optional, ref optional, ref optional, 
                      ref optional, ref optional, ref optional, ref optional, ref optional, ref optional); 
                  documentCheck.LanguageDetected = true;  
                  // 使用者關閉對話方塊時，便會顯示錯誤訊息。
                  if (errors == 0) 
                  { 
                      lbMessage.Text = &quot;No errors&quot;; 
                  } 
                  else 
                  { 
                      lbMessage.Text = &quot;Total errors num:&quot; &#43; errors; 
                  } 
                  // 取代文字方塊中拼字錯誤的文字。
                  object first = 0; 
                  object last = documentCheck.Characters.Count - 1; 
                  tbInput.Text = documentCheck.Range(ref first, ref last).Text; 
              } 
              object saveChanges = false; 
              object originalFormat = Missing.Value; 
              object routeDocument = Missing.Value; 
              ((_Application)applicationWord).Quit(ref saveChanges, ref originalFormat, ref routeDocument);
              applicationWord = null;
</pre>
<div class="preview">
<pre class="vb"><span class="visualBasic__com">'&nbsp;Prevent&nbsp;multiple&nbsp;checker&nbsp;window.&nbsp;</span><span class="visualBasic__keyword">If</span>&nbsp;applicationWord&nbsp;<span class="visualBasic__keyword">IsNot</span><span class="visualBasic__keyword">Nothing</span><span class="visualBasic__keyword">Then</span><span class="visualBasic__keyword">Return</span><span class="visualBasic__keyword">End</span><span class="visualBasic__keyword">If</span>&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;applicationWord&nbsp;=&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;Microsoft.Office.Interop.Word.Application()&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;errors&nbsp;<span class="visualBasic__keyword">As</span><span class="visualBasic__keyword">Integer</span>&nbsp;=&nbsp;<span class="visualBasic__number">0</span><span class="visualBasic__keyword">If</span>&nbsp;tbInput.Text.Length&nbsp;&gt;&nbsp;<span class="visualBasic__number">0</span><span class="visualBasic__keyword">Then</span><span class="visualBasic__keyword">Dim</span>&nbsp;template&nbsp;<span class="visualBasic__keyword">As</span><span class="visualBasic__keyword">Object</span>&nbsp;=&nbsp;Missing.Value&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;newTemplate&nbsp;<span class="visualBasic__keyword">As</span><span class="visualBasic__keyword">Object</span>&nbsp;=&nbsp;Missing.Value&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;documentType&nbsp;<span class="visualBasic__keyword">As</span><span class="visualBasic__keyword">Object</span>&nbsp;=&nbsp;Missing.Value&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;visible&nbsp;<span class="visualBasic__keyword">As</span><span class="visualBasic__keyword">Object</span>&nbsp;=&nbsp;<span class="visualBasic__keyword">True</span><span class="visualBasic__com">'&nbsp;定義&nbsp;MS&nbsp;Word&nbsp;文件，我們會使用此文件計算錯誤數量，並</span><span class="visualBasic__com">'&nbsp;叫用文件的&nbsp;CheckSpelling&nbsp;方法。</span><span class="visualBasic__keyword">Dim</span>&nbsp;documentCheck&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Microsoft.Office.Interop.Word._Document&nbsp;=&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;applicationWord.Documents.Add(template,&nbsp;newTemplate,&nbsp;documentType,&nbsp;visible)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;applicationWord.Visible&nbsp;=&nbsp;<span class="visualBasic__keyword">False</span>&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;documentCheck.Words.First.InsertBefore(tbInput.Text)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;spellErrorsColl&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Microsoft.Office.Interop.Word.ProofreadingErrors&nbsp;=&nbsp;documentCheck.SpellingErrors&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;errors&nbsp;=&nbsp;spellErrorsColl.Count&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;[optional]&nbsp;<span class="visualBasic__keyword">As</span><span class="visualBasic__keyword">Object</span>&nbsp;=&nbsp;Missing.Value&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;documentCheck.Activate()&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;documentCheck.CheckSpelling([optional],&nbsp;[optional],&nbsp;[optional],&nbsp;[optional],&nbsp;[optional],&nbsp;[optional],&nbsp;_&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[optional],&nbsp;[optional],&nbsp;[optional],&nbsp;[optional],&nbsp;[optional],&nbsp;[optional])&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;documentCheck.LanguageDetected&nbsp;=&nbsp;<span class="visualBasic__keyword">True</span><span class="visualBasic__com">'&nbsp;使用者關閉對話方塊時，便會顯示錯誤訊息。</span><span class="visualBasic__keyword">If</span>&nbsp;errors&nbsp;=&nbsp;<span class="visualBasic__number">0</span><span class="visualBasic__keyword">Then</span>&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;lbMessage.Text&nbsp;=&nbsp;<span class="visualBasic__string">&quot;No&nbsp;errors&quot;</span><span class="visualBasic__keyword">Else</span>&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;lbMessage.Text&nbsp;=&nbsp;<span class="visualBasic__string">&quot;Total&nbsp;errors&nbsp;num:&quot;</span>&nbsp;&amp;&nbsp;errors&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span><span class="visualBasic__keyword">If</span><span class="visualBasic__com">'&nbsp;取代文字方塊的錯誤文字。</span><span class="visualBasic__keyword">Dim</span>&nbsp;first&nbsp;<span class="visualBasic__keyword">As</span><span class="visualBasic__keyword">Object</span>&nbsp;=&nbsp;<span class="visualBasic__number">0</span><span class="visualBasic__keyword">Dim</span>&nbsp;last&nbsp;<span class="visualBasic__keyword">As</span><span class="visualBasic__keyword">Object</span>&nbsp;=&nbsp;documentCheck.Characters.Count&nbsp;-&nbsp;<span class="visualBasic__number">1</span>&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;tbInput.Text&nbsp;=&nbsp;documentCheck.Range(first,&nbsp;last).Text&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span><span class="visualBasic__keyword">If</span><span class="visualBasic__keyword">Dim</span>&nbsp;saveChanges&nbsp;<span class="visualBasic__keyword">As</span><span class="visualBasic__keyword">Object</span>&nbsp;=&nbsp;<span class="visualBasic__keyword">False</span><span class="visualBasic__keyword">Dim</span>&nbsp;originalFormat&nbsp;<span class="visualBasic__keyword">As</span><span class="visualBasic__keyword">Object</span>&nbsp;=&nbsp;Missing.Value&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;routeDocument&nbsp;<span class="visualBasic__keyword">As</span><span class="visualBasic__keyword">Object</span>&nbsp;=&nbsp;Missing.Value&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">DirectCast</span>(applicationWord,&nbsp;_Application).Quit(saveChanges,&nbsp;originalFormat,&nbsp;routeDocument)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;applicationWord&nbsp;=&nbsp;<span class="visualBasic__keyword">Nothing</span></pre>
</div>
</div>
</div>
<p></p>
<p><span><span>其他相關資訊</span></span></p>
<p><span><span>MSDN： </span><a href="http://msdn.microsoft.com/en-us/library/microsoft.office.interop.word(office.11).aspx"></a><a title="自動產生連結至 Microsoft.Office.Interop.Word" class="libraryLink" href="http://msdn.microsoft.com/zh-tw/library/Microsoft.Office.Interop.Word.aspx">Microsoft.Office.Interop.Word</a></span>
 (英文)<span> </span></p>
<p><span><span>MSDN： </span><a href="http://msdn.microsoft.com/en-us/library/microsoft.office.interop.word.application(office.11).aspx"><span>應用程式 介面</span></a><span> (英文)</span></span></p>
<p><span><span>MSDN<span style="color:#ffffff">： </span></span><span style="color:#ffffff"><a href="http://msdn.microsoft.com/en-us/library/microsoft.office.interop.word.document(office.11).aspx">文件 介面</a> (英文)</span></span><span style="color:#ffffff">
<strong>&nbsp;</strong><em>&nbsp;</em></span></p>
<p><span style="color:#ffffff">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies, and
 reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</span></p>
<p><span style="color:#ffffff"><img id="154478" src="154478-bf9af16d-7669-4472-97db-a9ff04d1cd2dimage.png" alt="" width="341" height="57"><br>
</span></p>
<p><strong>&nbsp;</strong><em></em></p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p><em><br>
</em></p>
