# How to Store Data of DataSet into XML File
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- ADO.NET
- Data Access
- .NET Development
## Topics
- XML
- DataSet
- XML Schema
## Updated
- 09/21/2016
## Description

<p><em><img id="154794" src="154794-8171.onecodesampletopbanner.png" alt="" width="696" height="58"></em></p>
<h1>DataSet のデータを XML ファイルに&#26684;納する方法 (CSDataSetToXML)</h1>
<h2>はじめに</h2>
<p>このサンプルでは、DataSet から XML ファイルにデータを書き込む方法と、XML から DataSet にデータを読み取る方法について説明します。</p>
<p>1. 2 つのテーブルを持つ 1 つのデータ セットを作成します。</p>
<p>2. 次の 2 つの方法でデータ セットを XML ファイルにエクスポートします。WriteXml を使用する方法と、GetXml メソッドを使用する方法です。</p>
<p>3. 次の 2 つの方法で XML ファイルからデータ セットをインポートします。ReadXml を使用する方法と、InferXmlSchema メソッドを使用する方法です。</p>
<h2>サンプルの実行</h2>
<p>F5 キーを押してサンプルを実行すると、次のような結果になります。</p>
<p>まず、2 つのテーブルを含む 1 つのデータ セットを作成します。<img id="154796" src="154796-image.png" alt=""></p>
<p>次に、2 とおりの方法でデータ セットを XML ファイルにエクスポートします。</p>
<p>a. WriteXml メソッドを使用してデータ セットをエクスポートします。<img id="154797" src="154797-5674a109-43e8-4fea-aaec-019715466589image.png" alt="" width="643" height="23"></p>
<p>b. GetXml メソッドを使用してデータ セットをエクスポートします。<img id="154798" src="154798-0ed385fe-8ff4-41f9-992a-9784d5961968image.png" alt="" width="640" height="24"></p>
<p>次に、XML ファイルからデータ セットをインポートします。</p>
<p>a. ReadXml を使用してデータ セットをインポートします。</p>
<p>前に WriteXml メソッドで作成した XML ファイルを ReadXml メソッドを使用して読み取ります。XML ファイルからスキーマも読み取られるため、元のデータ セットと同じスキーマになっていることが分かります。<img id="154799" src="154799-5e46ae88-ec68-4cd1-b3d1-9b96e178ff21image.png" alt="" width="641" height="89"></p>
<p>b. InferXmlSchema メソッドを使用してスキーマを推論します。</p>
<p>4 種類の XML 構造を表示します。</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;1. 属性のみを持つ要素</p>
<p>XML 文書は次のとおりです。</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">xml</span>
<pre class="hidden">&lt;MySchool&gt;
  &lt;Course CourseID=&quot;C1045&quot; Year=&quot;2012&quot;  Title=&quot;Calculus&quot; Credits=&quot;4&quot; DepartmentID=&quot;7&quot; /&gt;
  &lt;Course CourseID=&quot;C1061&quot; Year=&quot;2012&quot;  Title=&quot;Physics&quot; Credits=&quot;4&quot; DepartmentID=&quot;1&quot; /&gt;
  &lt;Department DepartmentID=&quot;1&quot; Name=&quot;Engineering&quot; Budget=&quot;350000&quot; StartDate=&quot;2007-09-01T00:00:00&#43;08:00&quot; Administrator=&quot;2&quot; /&gt;
  &lt;Department DepartmentID=&quot;7&quot; Name=&quot;Mathematics&quot; Budget=&quot;250024&quot; StartDate=&quot;2007-09-01T00:00:00&#43;08:00&quot; Administrator=&quot;3&quot; /&gt;
&lt;/MySchool&gt;</pre>
<div class="preview">
<pre class="xml"><span class="xml__tag_start">&lt;MySchool</span><span class="xml__tag_start">&gt;&nbsp;
</span><span class="xml__tag_start">&lt;Course</span><span class="xml__attr_name">CourseID</span>=<span class="xml__attr_value">&quot;C1045&quot;</span><span class="xml__attr_name">Year</span>=<span class="xml__attr_value">&quot;2012&quot;</span><span class="xml__attr_name">Title</span>=<span class="xml__attr_value">&quot;Calculus&quot;</span><span class="xml__attr_name">Credits</span>=<span class="xml__attr_value">&quot;4&quot;</span><span class="xml__attr_name">DepartmentID</span>=<span class="xml__attr_value">&quot;7&quot;</span><span class="xml__tag_start">/&gt;</span><span class="xml__tag_start">&lt;Course</span><span class="xml__attr_name">CourseID</span>=<span class="xml__attr_value">&quot;C1061&quot;</span><span class="xml__attr_name">Year</span>=<span class="xml__attr_value">&quot;2012&quot;</span><span class="xml__attr_name">Title</span>=<span class="xml__attr_value">&quot;Physics&quot;</span><span class="xml__attr_name">Credits</span>=<span class="xml__attr_value">&quot;4&quot;</span><span class="xml__attr_name">DepartmentID</span>=<span class="xml__attr_value">&quot;1&quot;</span><span class="xml__tag_start">/&gt;</span><span class="xml__tag_start">&lt;Department</span><span class="xml__attr_name">DepartmentID</span>=<span class="xml__attr_value">&quot;1&quot;</span><span class="xml__attr_name">Name</span>=<span class="xml__attr_value">&quot;Engineering&quot;</span><span class="xml__attr_name">Budget</span>=<span class="xml__attr_value">&quot;350000&quot;</span><span class="xml__attr_name">StartDate</span>=<span class="xml__attr_value">&quot;2007-09-01T00:00:00&#43;08:00&quot;</span><span class="xml__attr_name">Administrator</span>=<span class="xml__attr_value">&quot;2&quot;</span><span class="xml__tag_start">/&gt;</span><span class="xml__tag_start">&lt;Department</span><span class="xml__attr_name">DepartmentID</span>=<span class="xml__attr_value">&quot;7&quot;</span><span class="xml__attr_name">Name</span>=<span class="xml__attr_value">&quot;Mathematics&quot;</span><span class="xml__attr_name">Budget</span>=<span class="xml__attr_value">&quot;250024&quot;</span><span class="xml__attr_name">StartDate</span>=<span class="xml__attr_value">&quot;2007-09-01T00:00:00&#43;08:00&quot;</span><span class="xml__attr_name">Administrator</span>=<span class="xml__attr_value">&quot;3&quot;</span><span class="xml__tag_start">/&gt;</span><span class="xml__tag_end">&lt;/MySchool&gt;</span></pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;上記の XML 文書から推論される結果は次のとおりです。</div>
<div class="endscriptcode"><img id="154835" src="154835-ced1215f-71a4-4d76-8153-4264a9be4107image.png" alt="" width="640" height="74"></div>
<p>&nbsp;</p>
<p>ルート要素の名前がデータ セットの名前になり、要素の名前がテーブルの名前になり、属性の名前が列の名前になることが分かります。</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;2. 属性および要素テキストを持つ要素。</p>
<p>XML 文書は次のとおりです。</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">xml</span>
<pre class="hidden">&lt;MySchool&gt;
  &lt;Course CourseID=&quot;C1045&quot; Year=&quot;2012&quot;  Title=&quot;Calculus&quot; Credits=&quot;4&quot; DepartmentID=&quot;7&quot;&gt;New&lt;/Course&gt;
  &lt;Course CourseID=&quot;C1061&quot; Year=&quot;2012&quot;  Title=&quot;Physics&quot; Credits=&quot;4&quot; DepartmentID=&quot;1&quot; /&gt;
  &lt;Department DepartmentID=&quot;1&quot; Name=&quot;Engineering&quot; Budget=&quot;350000&quot; StartDate=&quot;2007-09-01T00:00:00&#43;08:00&quot; Administrator=&quot;2&quot; /&gt;
  &lt;Department DepartmentID=&quot;7&quot; Name=&quot;Mathematics&quot; Budget=&quot;250024&quot; StartDate=&quot;2007-09-01T00:00:00&#43;08:00&quot; Administrator=&quot;3&quot;&gt;Cancelled&lt;/Department&gt;
&lt;/MySchool&gt;
</pre>
<div class="preview">
<pre class="xml"><span class="xml__tag_start">&lt;MySchool</span><span class="xml__tag_start">&gt;&nbsp;
</span><span class="xml__tag_start">&lt;Course</span><span class="xml__attr_name">CourseID</span>=<span class="xml__attr_value">&quot;C1045&quot;</span><span class="xml__attr_name">Year</span>=<span class="xml__attr_value">&quot;2012&quot;</span><span class="xml__attr_name">Title</span>=<span class="xml__attr_value">&quot;Calculus&quot;</span><span class="xml__attr_name">Credits</span>=<span class="xml__attr_value">&quot;4&quot;</span><span class="xml__attr_name">DepartmentID</span>=<span class="xml__attr_value">&quot;7&quot;</span><span class="xml__tag_start">&gt;</span>New<span class="xml__tag_end">&lt;/Course&gt;</span><span class="xml__tag_start">&lt;Course</span><span class="xml__attr_name">CourseID</span>=<span class="xml__attr_value">&quot;C1061&quot;</span><span class="xml__attr_name">Year</span>=<span class="xml__attr_value">&quot;2012&quot;</span><span class="xml__attr_name">Title</span>=<span class="xml__attr_value">&quot;Physics&quot;</span><span class="xml__attr_name">Credits</span>=<span class="xml__attr_value">&quot;4&quot;</span><span class="xml__attr_name">DepartmentID</span>=<span class="xml__attr_value">&quot;1&quot;</span><span class="xml__tag_start">/&gt;</span><span class="xml__tag_start">&lt;Department</span><span class="xml__attr_name">DepartmentID</span>=<span class="xml__attr_value">&quot;1&quot;</span><span class="xml__attr_name">Name</span>=<span class="xml__attr_value">&quot;Engineering&quot;</span><span class="xml__attr_name">Budget</span>=<span class="xml__attr_value">&quot;350000&quot;</span><span class="xml__attr_name">StartDate</span>=<span class="xml__attr_value">&quot;2007-09-01T00:00:00&#43;08:00&quot;</span><span class="xml__attr_name">Administrator</span>=<span class="xml__attr_value">&quot;2&quot;</span><span class="xml__tag_start">/&gt;</span><span class="xml__tag_start">&lt;Department</span><span class="xml__attr_name">DepartmentID</span>=<span class="xml__attr_value">&quot;7&quot;</span><span class="xml__attr_name">Name</span>=<span class="xml__attr_value">&quot;Mathematics&quot;</span><span class="xml__attr_name">Budget</span>=<span class="xml__attr_value">&quot;250024&quot;</span><span class="xml__attr_name">StartDate</span>=<span class="xml__attr_value">&quot;2007-09-01T00:00:00&#43;08:00&quot;</span><span class="xml__attr_name">Administrator</span>=<span class="xml__attr_value">&quot;3&quot;</span><span class="xml__tag_start">&gt;</span>Cancelled<span class="xml__tag_end">&lt;/Department&gt;</span><span class="xml__tag_end">&lt;/MySchool&gt;</span></pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<p>上記の XML 文書から推論される結果は次のとおりです。</p>
<p><img id="154801" src="154801-1db86f01-a38b-4e59-a35c-f83027084b45image.png" alt="" width="637" height="88"></p>
<p>1 番目の種類と 2 番目の種類の違いは、2 番目の種類の XML 文書に要素テキストが含まれている点のみです。結果の違いは、次に挙げる新しい列のみであることが分かります。 &quot;Course_Text&quot;, &quot;Department_Text&quot;.</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;3. 要素の繰り返し</p>
<p>XML 文書は次のとおりです。 <strong>&nbsp;</strong><em>&nbsp;</em></p>
<p>&nbsp;</p>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">xml</span>
<pre class="hidden">&lt;MySchool&gt;
  &lt;Course&gt;C1045&lt;/Course&gt;
  &lt;Course&gt;C1061&lt;/Course&gt;
  &lt;Department&gt;Engineering&lt;/Department&gt; 
  &lt;Department&gt;Mathematics&lt;/Department&gt;
&lt;/MySchool&gt;
</pre>
<div class="preview">
<pre class="xml"><span class="xml__tag_start">&lt;MySchool</span><span class="xml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;<span class="xml__tag_start">&lt;Course</span><span class="xml__tag_start">&gt;</span>C1045<span class="xml__tag_end">&lt;/Course&gt;</span>&nbsp;
&nbsp;&nbsp;<span class="xml__tag_start">&lt;Course</span><span class="xml__tag_start">&gt;</span>C1061<span class="xml__tag_end">&lt;/Course&gt;</span>&nbsp;
&nbsp;&nbsp;<span class="xml__tag_start">&lt;Department</span><span class="xml__tag_start">&gt;</span>Engineering<span class="xml__tag_end">&lt;/Department&gt;</span>&nbsp;&nbsp;
&nbsp;&nbsp;<span class="xml__tag_start">&lt;Department</span><span class="xml__tag_start">&gt;</span>Mathematics<span class="xml__tag_end">&lt;/Department&gt;</span>&nbsp;
<span class="xml__tag_end">&lt;/MySchool&gt;</span>&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
上記の XML 文書から推論される結果は次のとおりです。<strong>&nbsp;</strong><em>&nbsp;</em></div>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p><img id="154802" src="154802-59f949fe-c26b-4793-89c3-a1953b222734image.png" alt="" width="637" height="81"></p>
<p>要素の反復は 1 つの表と推論されることが分かります。</p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;4. 子要素を持つ要素</p>
<p>XML 文書は次のとおりです。</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">xml</span>
<pre class="hidden">&lt;MySchool&gt;
  &lt;Course&gt;
    &lt;CourseID&gt;C1045&lt;/CourseID&gt;
    &lt;Year&gt;2012&lt;/Year&gt;
    &lt;Title&gt;Calculus&lt;/Title&gt;
    &lt;Credits&gt;4&lt;/Credits&gt;
    &lt;DepartmentID&gt;7&lt;/DepartmentID&gt;
  &lt;/Course&gt;
  &lt;Course&gt;
    &lt;CourseID&gt;C1061&lt;/CourseID&gt;
    &lt;Year&gt;2012&lt;/Year&gt;
    &lt;Title&gt;Physics&lt;/Title&gt;
    &lt;Credits&gt;4&lt;/Credits&gt;
    &lt;DepartmentID&gt;1&lt;/DepartmentID&gt;
  &lt;/Course&gt;
  .................................
  &lt;Department&gt;
    &lt;DepartmentID&gt;1&lt;/DepartmentID&gt;
    &lt;Name&gt;Engineering&lt;/Name&gt;
    &lt;Budget&gt;350000&lt;/Budget&gt;
    &lt;StartDate&gt;2007-09-01T00:00:00&#43;08:00&lt;/StartDate&gt;
    &lt;Administrator&gt;2&lt;/Administrator&gt;
  &lt;/Department&gt;
  &lt;Department&gt;
    &lt;DepartmentID&gt;2&lt;/DepartmentID&gt;
    &lt;Name&gt;English&lt;/Name&gt;
    &lt;Budget&gt;120000&lt;/Budget&gt;
    &lt;StartDate&gt;2007-09-01T00:00:00&#43;08:00&lt;/StartDate&gt;
    &lt;Administrator&gt;6&lt;/Administrator&gt;
  &lt;/Department&gt;
  .................................
&lt;/MySchool&gt;
</pre>
<div class="preview">
<pre class="xml"><span class="xml__tag_start">&lt;MySchool</span><span class="xml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;<span class="xml__tag_start">&lt;Course</span><span class="xml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;CourseID</span><span class="xml__tag_start">&gt;</span>C1045<span class="xml__tag_end">&lt;/CourseID&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;Year</span><span class="xml__tag_start">&gt;</span>2012<span class="xml__tag_end">&lt;/Year&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;Title</span><span class="xml__tag_start">&gt;</span>Calculus<span class="xml__tag_end">&lt;/Title&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;Credits</span><span class="xml__tag_start">&gt;</span>4<span class="xml__tag_end">&lt;/Credits&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;DepartmentID</span><span class="xml__tag_start">&gt;</span>7<span class="xml__tag_end">&lt;/DepartmentID&gt;</span>&nbsp;
&nbsp;&nbsp;<span class="xml__tag_end">&lt;/Course&gt;</span>&nbsp;
&nbsp;&nbsp;<span class="xml__tag_start">&lt;Course</span><span class="xml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;CourseID</span><span class="xml__tag_start">&gt;</span>C1061<span class="xml__tag_end">&lt;/CourseID&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;Year</span><span class="xml__tag_start">&gt;</span>2012<span class="xml__tag_end">&lt;/Year&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;Title</span><span class="xml__tag_start">&gt;</span>Physics<span class="xml__tag_end">&lt;/Title&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;Credits</span><span class="xml__tag_start">&gt;</span>4<span class="xml__tag_end">&lt;/Credits&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;DepartmentID</span><span class="xml__tag_start">&gt;</span>1<span class="xml__tag_end">&lt;/DepartmentID&gt;</span>&nbsp;
&nbsp;&nbsp;<span class="xml__tag_end">&lt;/Course&gt;</span>&nbsp;
&nbsp;&nbsp;.................................&nbsp;
&nbsp;&nbsp;<span class="xml__tag_start">&lt;Department</span><span class="xml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;DepartmentID</span><span class="xml__tag_start">&gt;</span>1<span class="xml__tag_end">&lt;/DepartmentID&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;Name</span><span class="xml__tag_start">&gt;</span>Engineering<span class="xml__tag_end">&lt;/Name&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;Budget</span><span class="xml__tag_start">&gt;</span>350000<span class="xml__tag_end">&lt;/Budget&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;StartDate</span><span class="xml__tag_start">&gt;</span>2007-09-01T00:00:00&#43;08:00<span class="xml__tag_end">&lt;/StartDate&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;Administrator</span><span class="xml__tag_start">&gt;</span>2<span class="xml__tag_end">&lt;/Administrator&gt;</span>&nbsp;
&nbsp;&nbsp;<span class="xml__tag_end">&lt;/Department&gt;</span>&nbsp;
&nbsp;&nbsp;<span class="xml__tag_start">&lt;Department</span><span class="xml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;DepartmentID</span><span class="xml__tag_start">&gt;</span>2<span class="xml__tag_end">&lt;/DepartmentID&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;Name</span><span class="xml__tag_start">&gt;</span>English<span class="xml__tag_end">&lt;/Name&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;Budget</span><span class="xml__tag_start">&gt;</span>120000<span class="xml__tag_end">&lt;/Budget&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;StartDate</span><span class="xml__tag_start">&gt;</span>2007-09-01T00:00:00&#43;08:00<span class="xml__tag_end">&lt;/StartDate&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;Administrator</span><span class="xml__tag_start">&gt;</span>6<span class="xml__tag_end">&lt;/Administrator&gt;</span>&nbsp;
&nbsp;&nbsp;<span class="xml__tag_end">&lt;/Department&gt;</span>&nbsp;
&nbsp;&nbsp;.................................&nbsp;
<span class="xml__tag_end">&lt;/MySchool&gt;</span>&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p><strong>&nbsp;</strong><em>&nbsp;</em></p>
<p>&nbsp;</p>
<p>上記の XML 文書から推論される結果は次のとおりです。 <img id="154803" src="154803-5e46ae88-ec68-4cd1-b3d1-9b96e178ff21image.png" alt="" width="641" height="89"></p>
<p>上記の XML 文書は GetXml メソッドで前に作成した文書と同じであり、元のデータ セットと同じ構造を取得できました。このタイプの構造では、ルート要素の名前がデータ セットの名前になり、2 番目のレベルの要素がテーブルの名前になり、3 番目のレベルの要素が属性の名前になることが分かります。<strong>&nbsp;</strong><em>&nbsp;</em></p>
<h2>コードの使用</h2>
<p>1. 2 とおりの方法でデータ セットを XML ファイルにエクスポートします。</p>
<p>a. WriteXml メソッドを使用してデータ セットをエクスポートします。</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">using (FileStream fsWriterStream = new FileStream(xmlFileName, FileMode.Create))
{
    using (XmlTextWriter xmlWriter = new XmlTextWriter(fsWriterStream, Encoding.Unicode))
    {
        dataset.WriteXml(xmlWriter, XmlWriteMode.WriteSchema);
        Console.WriteLine(&quot;Write {0} to the File {1}.&quot;, dataset.DataSetName, xmlFileName);
        Console.WriteLine();
    }
}
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">using</span>&nbsp;(FileStream&nbsp;fsWriterStream&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;FileStream(xmlFileName,&nbsp;FileMode.Create))&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">using</span>&nbsp;(XmlTextWriter&nbsp;xmlWriter&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;XmlTextWriter(fsWriterStream,&nbsp;Encoding.Unicode))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;dataset.WriteXml(xmlWriter,&nbsp;XmlWriteMode.WriteSchema);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="cs__string">&quot;Write&nbsp;{0}&nbsp;to&nbsp;the&nbsp;File&nbsp;{1}.&quot;</span>,&nbsp;dataset.DataSetName,&nbsp;xmlFileName);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;b. GetXml メソッドを使用してデータ セットをエクスポートします。</div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">using (StreamWriter writer = new StreamWriter(xmlFileName))
{
    writer.WriteLine(dataset.GetXml());
    Console.WriteLine(&quot;Get Xml data from {0} and write to the File {1}.&quot;, dataset.DataSetName, xmlFileName);
    Console.WriteLine();
}
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">using</span>&nbsp;(StreamWriter&nbsp;writer&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;StreamWriter(xmlFileName))&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;writer.WriteLine(dataset.GetXml());&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="cs__string">&quot;Get&nbsp;Xml&nbsp;data&nbsp;from&nbsp;{0}&nbsp;and&nbsp;write&nbsp;to&nbsp;the&nbsp;File&nbsp;{1}.&quot;</span>,&nbsp;dataset.DataSetName,&nbsp;xmlFileName);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine();&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
</div>
<p>&nbsp;</p>
<p>2. 2 とおりの方法でデータ セットを XML ファイルからインポートします。</p>
<p>a. ReadXml メソッドを使用してデータ セットをインポートします。</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">using (FileStream fsReaderStream = new FileStream(xmlFileName, FileMode.Open))
{
    using (XmlTextReader xmlReader = new XmlTextReader(fsReaderStream))
    {
        newDataSet.ReadXml(xmlReader, XmlReadMode.ReadSchema);
    }
}

</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">using</span>&nbsp;(FileStream&nbsp;fsReaderStream&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;FileStream(xmlFileName,&nbsp;FileMode.Open))&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">using</span>&nbsp;(XmlTextReader&nbsp;xmlReader&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;XmlTextReader(fsReaderStream))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;newDataSet.ReadXml(xmlReader,&nbsp;XmlReadMode.ReadSchema);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;b. InferXmlSchema メソッドを使用してデータ セットをインポートします。<strong>&nbsp;</strong><em>&nbsp;</em></div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">String[] xmlFileNames = { 
                        @&quot;XMLFiles\ElementsWithOnlyAttributes.xml&quot;, 
                        @&quot;XMLFiles\ElementsWithAttributes.xml&quot;,
                        @&quot;XMLFiles\RepeatingElements.xml&quot;, 
                        @&quot;XMLFiles\ElementsWithChildElements.xml&quot; };


foreach (String xmlFileName in xmlFileNames)
{
    Console.WriteLine(&quot;Result of {0}&quot;, Path.GetFileNameWithoutExtension(xmlFileName));
    DataSet newSchool = new DataSet();
    newSchool.InferXmlSchema(xmlFileName,null);
    DataTableHelper.ShowDataSetSchema(newSchool);
    Console.WriteLine();
           }
</pre>
<div class="preview">
<pre class="csharp">String[]&nbsp;xmlFileNames&nbsp;=&nbsp;{&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;@<span class="cs__string">&quot;XMLFiles\ElementsWithOnlyAttributes.xml&quot;</span>,&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;@<span class="cs__string">&quot;XMLFiles\ElementsWithAttributes.xml&quot;</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;@<span class="cs__string">&quot;XMLFiles\RepeatingElements.xml&quot;</span>,&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;@<span class="cs__string">&quot;XMLFiles\ElementsWithChildElements.xml&quot;</span>&nbsp;};&nbsp;
&nbsp;
&nbsp;
<span class="cs__keyword">foreach</span>&nbsp;(String&nbsp;xmlFileName&nbsp;<span class="cs__keyword">in</span>&nbsp;xmlFileNames)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="cs__string">&quot;Result&nbsp;of&nbsp;{0}&quot;</span>,&nbsp;Path.GetFileNameWithoutExtension(xmlFileName));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;DataSet&nbsp;newSchool&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DataSet();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;newSchool.InferXmlSchema(xmlFileName,<span class="cs__keyword">null</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;DataTableHelper.ShowDataSetSchema(newSchool);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
</div>
<p><span class="MsoHyperlink"><a href="http://msdn.microsoft.com/ja-jp/library/system.data.dataset.getxml.aspx"><span class="SpellE">DataSet.GetXml</span> メソッド</a></span></p>
<p class="MsoNormal"><span class="MsoHyperlink"><a href="http://msdn.microsoft.com/ja-jp/library/d6swf149.aspx"><span class="SpellE">DataSet.ReadXml</span> メソッド (<span class="SpellE">XmlReader</span>)</a></span></p>
<p class="MsoNormal"><span class="MsoHyperlink"><a href="http://msdn.microsoft.com/ja-jp/library/cat50f7f.aspx">テーブルの推論</a></span></p>
<p class="MsoNormal"><span class="MsoHyperlink"><a href="http://msdn.microsoft.com/ja-jp/library/fx29c3yd(VS.110).aspx">XML からの
<span class="SpellE">DataSet</span> の読み込み</a></span><strong>&nbsp;</strong><em>&nbsp;</em></p>
<p>&nbsp;</p>
<p><img id="154804" src="154804-a2aa0602-8919-4a19-9dcf-bdbc504c20dfimage.png" alt=""></p>
<p>&nbsp;</p>
