# How to inject javascript code into WebBrowser in Windows Forms applications
## Requires
- 
## License
- Apache License, Version 2.0
## Technologies
- Windows Forms
- Internet Explorer
- .NET Development
- Internet Explorer Development
## Topics
- WebBrowser
- code snippets
- inject code
## Updated
- 09/22/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src=":-onecodesampletopbanner1" alt="">
</a></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:24pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><span style="font-weight:bold; font-size:14pt">Windows Forms 응용 프로그램에서 WebBrowser 컨트롤에 JavaScript 코드를 삽입하는 방법
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">소개</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">이 코드 조각 프로젝트에서는 Windows Forms 응용 프로그램에서 System.Windows.Forms.Webbrowser 컨트롤에 JavaScript 코드를 삽입하는 방법을 보여 줍니다.
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">코드 사용</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">1단계. Visual Studio에서 Windows Forms 응용 프로그램을 만든 다음 WebBrowser 컨트롤을 기본 폼에 끌어 놓고
</span><span>컨트롤의 URL 속성을 설정합니다.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">2단계. 아래와 같은 단계에 따라 필요한 참조를 추가합니다</span><span style="font-size:11pt">.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">프로젝트를 마우스 오른쪽 단추로 클릭하고 </span>
<span style="font-weight:bold">참조 추가...</span><span style="font-size:11pt"> -&gt;
</span><span style="font-weight:bold">COM </span><span style="font-size:11pt">-&gt;
</span><span style="font-weight:bold">형식 라이브러리</span><span style="font-size:11pt">를 선택한 다음 &quot;</span><span style="font-weight:bold">Microsoft HTML 개체 라이브러리</span><span>&quot;를 선택합니다.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span>3단계. 네임스페이스를 추가합니다. </span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>VB</span><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span><span class="hidden">cplusplus</span>
<pre class="hidden">using mshtml;
</pre>
<pre class="hidden">Imports mshtml
</pre>
<pre class="hidden">using namespace MSHTML;
</pre>
<pre class="csharp" id="codePreview">using mshtml;
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span>&nbsp;</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span>4단계. 다음 코드 조각에서는 WebBrowser 컨트롤에 JavaScript 코드를 삽입하는 방법을 보여 줍니다.</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>VB</span><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span><span class="hidden">cplusplus</span>
<pre class="hidden">private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
{
    HtmlElement headElement = webBrowser1.Document.GetElementsByTagName(&quot;head&quot;)[0];
    HtmlElement scriptElement = webBrowser1.Document.CreateElement(&quot;script&quot;);
    IHTMLScriptElement element = (IHTMLScriptElement)scriptElement.DomElement;
    element.text = &quot;function sayHello() { alert('hello') }&quot;;
    headElement.AppendChild(scriptElement);
    webBrowser1.Document.InvokeScript(&quot;sayHello&quot;);
}
</pre>
<pre class="hidden">Private Sub WebBrowser1_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted
    Dim headElement As HtmlElement = WebBrowser1.Document.GetElementsByTagName(&quot;head&quot;)(0)
    Dim scriptElement As HtmlElement = WebBrowser1.Document.CreateElement(&quot;script&quot;)
    Dim element As IHTMLScriptElement = DirectCast(scriptElement.DomElement, IHTMLScriptElement)
    element.text = &quot;function sayHello() { alert('hello') }&quot;
    headElement.AppendChild(scriptElement)
    WebBrowser1.Document.InvokeScript(&quot;sayHello&quot;)
End Sub
</pre>
<pre class="hidden">System::Void MyForm::webBrowser1_DocumentCompleted(System::Object^  sender, System::Windows::Forms::WebBrowserDocumentCompletedEventArgs^  e)
{
 HtmlElement^ headElement = webBrowser1-&gt;Document-&gt;GetElementsByTagName(&quot;head&quot;)[0];
 HtmlElement^ scriptElement = webBrowser1-&gt;Document-&gt;CreateElement(&quot;script&quot;);
 IHTMLScriptElement^ element = (IHTMLScriptElement^)scriptElement-&gt;DomElement;
 element-&gt;text = &quot;function sayHello() { alert('hello') }&quot;;
 headElement-&gt;AppendChild(scriptElement);
 webBrowser1-&gt;Document-&gt;InvokeScript(&quot;sayHello&quot;);
}
</pre>
<pre class="csharp" id="codePreview">private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
{
    HtmlElement headElement = webBrowser1.Document.GetElementsByTagName(&quot;head&quot;)[0];
    HtmlElement scriptElement = webBrowser1.Document.CreateElement(&quot;script&quot;);
    IHTMLScriptElement element = (IHTMLScriptElement)scriptElement.DomElement;
    element.text = &quot;function sayHello() { alert('hello') }&quot;;
    headElement.AppendChild(scriptElement);
    webBrowser1.Document.InvokeScript(&quot;sayHello&quot;);
}
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span>5단계. 응용 프로그램을 빌드하고 실행할 수 있습니다. 아무 문제가 없다면 환영 메시지가 표시됩니다.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt"><img src="117487-image.png" alt="" width="222" height="169" align="middle">
</span><a name="_GoBack"></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">추가 정보</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">WebBrowser 컨트롤(영문)</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a href="http://msdn.microsoft.com/ko-kr/library/system.windows.forms.webbrowser(v=vs.110).ASPX" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://msdn.microsoft.com/ko-kr/library/system.windows.forms.webbrowser(v=vs.110).ASPX</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">WebBrowser.Document 속성(영문)</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a href="http://msdn.microsoft.com/ko-kr/library/system.windows.forms.webbrowser.document(v=vs.110).aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://msdn.microsoft.com/ko-kr/library/system.windows.forms.webbrowser.document(v=vs.110).aspx</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">HtmlDocument 클래스(영문)</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a href="http://msdn.microsoft.com/ko-kr/library/system.windows.forms.htmldocument(v=vs.110).aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://msdn.microsoft.com/ko-kr/library/system.windows.forms.htmldocument(v=vs.110).aspx</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span>HtmlDocument.InvokeScript 메서드(영문)</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a href="http://msdn.microsoft.com/ko-kr/library/system.windows.forms.htmldocument.invokescript(v=vs.110).aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://msdn.microsoft.com/ko-kr/library/system.windows.forms.htmldocument.invokescript(v=vs.110).aspx</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework는 개발자들의 실제 문제와 요구에 따라 구성된 중앙 집중식 무료 코드 샘플 라이브러리입니다. 모든 Microsoft 개발 기술에 대한 고객 중심 코드 샘플을 제공하고 일반 프로그래밍 작업 진행에서 개발자의 수고를 더는 것을 목표로 삼고 있습니다. Microsoft 팀은 MSDN 포럼, 소셜 미디어 및 다양한 DEV 커뮤니티에서
 개발자들이 겪는 문제를 확인합니다. 개발자들이 자주 문의하는 프로그래밍 작업을 기반&#51004;로 코드 샘플을 작성하며 개발자들이 짧은 샘플 게시 주기로 이러한 샘플을 다운로드할 수 있습니다. 또한 무료 코드 샘플 요청 서비스를 제공합니다. 이는 Microsoft 개발자 커뮤니티가 Microsoft에서 코드 샘플을 직접 구할 수 있는 사전 대응적인 방식입니다.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
