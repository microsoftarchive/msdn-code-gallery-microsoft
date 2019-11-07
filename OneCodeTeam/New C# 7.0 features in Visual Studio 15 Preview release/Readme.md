# New C# 7.0 features in Visual Studio 15 Preview release
## Requires
- Visual Studio 2017
## License
- MIT
## Technologies
- C#
- .NET
- Languages
## Topics
- C#
- c#7.0
- 7.0
- c#70
- Out variables
- Pattern matching
- Tuples
- Deconstruction
- Local functions
- Digit separator
- Ref returns and locals
- C# 7.0
## Updated
- 09/26/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img alt="" src=":-8171.onecodesampletopbanner.png">
</a></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><span style="font-weight:bold; font-size:14pt">New C# 7.0 features in Visual Studio 15 Preview release</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Introduction
</span></span></p>
<p><span style="font-size:small">A number of new <a href="https://blogs.msdn.microsoft.com/dotnet/2016/08/24/whats-new-in-csharp-7-0/">
C# 7.0</a> features were introduced with the <a href="https://www.visualstudio.com/en-us/downloads/visual-studio-next-downloads-vs.aspx">
Visual Studio 15 preview</a> release on August 24, 2016.&nbsp;These features are showcased in this centralized VS 15 preview sample solution.&nbsp;</span></p>
<p><span style="font-size:small">This sample demonstrates 7 new C# 7.0 features:</span></p>
<p><span style="font-size:small"><span style="white-space:pre">&nbsp;</span>&bull;&nbsp;Out variables</span></p>
<p><span style="font-size:small"><span style="white-space:pre">&nbsp;</span>&bull;&nbsp;Pattern matching</span></p>
<p><span style="font-size:small"><span style="white-space:pre">&nbsp;</span>&bull;&nbsp;Tuples</span></p>
<p><span style="font-size:small"><span style="white-space:pre">&nbsp;</span>&bull;&nbsp;Deconstruction</span></p>
<p><span style="font-size:small"><span style="white-space:pre">&nbsp;</span>&bull;&nbsp;Local functions</span></p>
<p><span style="font-size:small"><span style="white-space:pre">&nbsp;</span>&bull;&nbsp;Digit separator</span></p>
<p><span style="font-size:small"><span style="white-space:pre">&nbsp;</span>&bull;&nbsp;Ref returns and locals</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Sample prerequisites</span></span></p>
<p><span style="font-size:small">This sample solution needs Visual Studio 15 to run. Download the latest
<a href="https://www.visualstudio.com/en-us/downloads/visual-studio-next-downloads-vs.aspx">
Visual Studio 15 preview</a><span style="text-decoration:underline"> to begin</span>.</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Running the sample</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span></span><span style="font-size:small">Open Visual Studio 15, click
<strong style="font-size:10px">File &gt; Open &gt; Project</strong><strong style="font-size:10px">/Solution</strong> in menu, and then, find the file &ldquo;<strong style="font-size:10px">CS70Sample.sln</strong>&rdquo; and double-click on it to open this Solution.</span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:small"><br>
</span></p>
<div>
<div>
<div></div>
</div>
</div>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img alt="" src="159001-image.png" width="575" height="235" align="middle"></span></span></p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><br>
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;<span>Right-click Solution
<strong>&lsquo;CS70Sample&rsquo;</strong> in Solution Explorer, and then click &ldquo;<strong>Restore NuGet Packages</strong>&rdquo;</span></span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal"><span><br>
</span></span></span></p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img alt="" src="159002-image.png" width="575" height="410" align="middle"></span></span></p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><br>
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span></span><span style="font-size:small">Then choose a branch of this sample you interested to read or debug it as you wish.</span></p>
<div>
<div>
<div></div>
</div>
</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Using the code</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:small"><span style="font-weight:bold">Out variables</span></span></p>
<p><span style="font-size:small">C# 7.0 has the ability to declare an out variable at the point which it is passed as an out argument.</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">class OutVariablesSample
{
	public static void Run()
	{
		// declare out variables in function call, and use those variables in the same scope
		if (GetXY(out int i, out int j)) {
			Console.WriteLine($&quot;x: {i}, y: {j}&quot;);
		}
	}

	static bool GetXY(out int x, out int y)
	{
		x = 0;
		y = 1;
		return true;
	}
}
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">class</span>&nbsp;OutVariablesSample&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Run()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;declare&nbsp;out&nbsp;variables&nbsp;in&nbsp;function&nbsp;call,&nbsp;and&nbsp;use&nbsp;those&nbsp;variables&nbsp;in&nbsp;the&nbsp;same&nbsp;scope</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(GetXY(<span class="cs__keyword">out</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;i,&nbsp;<span class="cs__keyword">out</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;j))&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine($<span class="cs__string">&quot;x:&nbsp;{i},&nbsp;y:&nbsp;{j}&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">bool</span>&nbsp;GetXY(<span class="cs__keyword">out</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;x,&nbsp;<span class="cs__keyword">out</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;y)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;x&nbsp;=&nbsp;<span class="cs__number">0</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;y&nbsp;=&nbsp;<span class="cs__number">1</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;<span class="cs__keyword">true</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode"></div>
<div class="endscriptcode"><span style="font-size:small; font-weight:bold">Pattern matching</span></div>
<p><span style="font-size:small">C# 7.0 introduces &ldquo;pattern matching&rdquo; abstract to determine the value for &ldquo;Type, value&rdquo; and then extract it if the test passes.</span></p>
<p><span style="font-size:small">In the sample, we will demonstrate 3 different kinds of patterns</span></p>
<p><span style="font-size:small"><span style="white-space:pre">&nbsp;</span>&bull;&nbsp;Type pattern</span></p>
<p><span style="font-size:small"><span style="white-space:pre">&nbsp;</span>&bull;&nbsp;Determine the type of the value and then assign the value to the new type identifier if the test passes.</span></p>
<p><span style="font-size:small"><span style="white-space:pre">&nbsp;</span>&bull;&nbsp;Constant pattern</span></p>
<p><span style="font-size:small"><span style="white-space:pre">&nbsp;</span>&bull;&nbsp;Determine the value and test whether it is equal to the value to be tested.</span></p>
<p><span style="font-size:small"><span style="white-space:pre">&nbsp;</span>&bull;&nbsp;var pattern</span></p>
<p><span style="font-size:small"><span style="white-space:pre">&nbsp;</span>&bull;&nbsp;Create a new identifier and assign the original value to it.</span></p>
<p>&nbsp;</p>
<p><span style="font-size:small">There are 2 approaches to apply the patterns listed above:</span></p>
<p><span style="font-size:small"><span style="white-space:pre">&nbsp;</span>&bull;&nbsp;Is expression</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">static void IsExpressionWithPattern()
{
	var content = &quot;Hello pattern&quot;;

	// 1. type pattern
	if (content is string str) {
		Console.WriteLine($&quot;Is expression with type pattern, value is \&quot;{str}\&quot;&quot;);
	}
	// 2. constant pattern
	if (content is &quot;Hello pattern&quot;) {
		Console.WriteLine($&quot;Is expression with constant pattern, constant check successfully&quot;);
	}
	// 3. var pattern
	if (content is var j) {
		Console.WriteLine($&quot;Is expression with var pattern, variable identifier change to \&quot;j\&quot; and value is \&quot;{j}\&quot;&quot;);
	}
}
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;IsExpressionWithPattern()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;content&nbsp;=&nbsp;<span class="cs__string">&quot;Hello&nbsp;pattern&quot;</span>;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;1.&nbsp;type&nbsp;pattern</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(content&nbsp;<span class="cs__keyword">is</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;str)&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine($<span class="cs__string">&quot;Is&nbsp;expression&nbsp;with&nbsp;type&nbsp;pattern,&nbsp;value&nbsp;is&nbsp;\&quot;{str}\&quot;&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;2.&nbsp;constant&nbsp;pattern</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(content&nbsp;<span class="cs__keyword">is</span>&nbsp;<span class="cs__string">&quot;Hello&nbsp;pattern&quot;</span>)&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine($<span class="cs__string">&quot;Is&nbsp;expression&nbsp;with&nbsp;constant&nbsp;pattern,&nbsp;constant&nbsp;check&nbsp;successfully&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;3.&nbsp;var&nbsp;pattern</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(content&nbsp;<span class="cs__keyword">is</span>&nbsp;var&nbsp;j)&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine($<span class="cs__string">&quot;Is&nbsp;expression&nbsp;with&nbsp;var&nbsp;pattern,&nbsp;variable&nbsp;identifier&nbsp;change&nbsp;to&nbsp;\&quot;j\&quot;&nbsp;and&nbsp;value&nbsp;is&nbsp;\&quot;{j}\&quot;&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Switch statement</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">static void SwitchStatementsWithPattern()
{
	var content = &quot;Hello pattern&quot;;

	// 1. type pattern
	switch (content) {
		case string str: {
				Console.WriteLine($&quot;Switch expression with type pattern, value is \&quot;{str}\&quot;&quot;);
				break;
			}
		case null: {
				break;
			}
	}
	// 2. constant pattern
	switch (content) {
		case &quot;Hello pattern&quot;: {
				Console.WriteLine($&quot;Switch expression with constant pattern, constant check successfully&quot;);
				break;
			}
		case null: {
				break;
			}
	}
	// 3. var pattern
	switch (content) {
		case var j: {
				Console.WriteLine($&quot;Switch expression with var pattern, variable identifier change to \&quot;j\&quot; and value is \&quot;{j}\&quot;&quot;);
				break;
			}
	}
}
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;SwitchStatementsWithPattern()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;content&nbsp;=&nbsp;<span class="cs__string">&quot;Hello&nbsp;pattern&quot;</span>;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;1.&nbsp;type&nbsp;pattern</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">switch</span>&nbsp;(content)&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">case</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;str:&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine($<span class="cs__string">&quot;Switch&nbsp;expression&nbsp;with&nbsp;type&nbsp;pattern,&nbsp;value&nbsp;is&nbsp;\&quot;{str}\&quot;&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">break</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">case</span>&nbsp;<span class="cs__keyword">null</span>:&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">break</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;2.&nbsp;constant&nbsp;pattern</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">switch</span>&nbsp;(content)&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">case</span>&nbsp;<span class="cs__string">&quot;Hello&nbsp;pattern&quot;</span>:&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine($<span class="cs__string">&quot;Switch&nbsp;expression&nbsp;with&nbsp;constant&nbsp;pattern,&nbsp;constant&nbsp;check&nbsp;successfully&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">break</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">case</span>&nbsp;<span class="cs__keyword">null</span>:&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">break</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;3.&nbsp;var&nbsp;pattern</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">switch</span>&nbsp;(content)&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">case</span>&nbsp;var&nbsp;j:&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine($<span class="cs__string">&quot;Switch&nbsp;expression&nbsp;with&nbsp;var&nbsp;pattern,&nbsp;variable&nbsp;identifier&nbsp;change&nbsp;to&nbsp;\&quot;j\&quot;&nbsp;and&nbsp;value&nbsp;is&nbsp;\&quot;{j}\&quot;&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">break</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;</pre>
</div>
</div>
</div>
<p><span style="font-size:small; font-weight:bold">Tuples</span></p>
<p>C# 7.0 provides an easy solution to returning the tuple from a function instead of using the long template tuple class type.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">static (string FirstName, string MiddleName, string LastName) SplitFullName(string fullName)
{
	var names = fullName.Split(' ', '\t');
	switch(names.Length) {
		case 2: {
				return (FirstName: names[0], MiddleName: string.Empty, LastName: names[1]);
			}
		case 3: {
				return (FirstName: names[0], MiddleName: names[1], LastName: names[2]);
			}
		default: {
				return (FirstName: fullName, MiddleName: string.Empty, LastName: string.Empty);
			}
	}
}
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">static</span>&nbsp;(<span class="cs__keyword">string</span>&nbsp;FirstName,&nbsp;<span class="cs__keyword">string</span>&nbsp;MiddleName,&nbsp;<span class="cs__keyword">string</span>&nbsp;LastName)&nbsp;SplitFullName(<span class="cs__keyword">string</span>&nbsp;fullName)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;names&nbsp;=&nbsp;fullName.Split(<span class="cs__string">'&nbsp;'</span>,&nbsp;<span class="cs__string">'\t'</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">switch</span>(names.Length)&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">case</span>&nbsp;<span class="cs__number">2</span>:&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;(FirstName:&nbsp;names[<span class="cs__number">0</span>],&nbsp;MiddleName:&nbsp;<span class="cs__keyword">string</span>.Empty,&nbsp;LastName:&nbsp;names[<span class="cs__number">1</span>]);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">case</span>&nbsp;<span class="cs__number">3</span>:&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;(FirstName:&nbsp;names[<span class="cs__number">0</span>],&nbsp;MiddleName:&nbsp;names[<span class="cs__number">1</span>],&nbsp;LastName:&nbsp;names[<span class="cs__number">2</span>]);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">default</span>:&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;(FirstName:&nbsp;fullName,&nbsp;MiddleName:&nbsp;<span class="cs__keyword">string</span>.Empty,&nbsp;LastName:&nbsp;<span class="cs__keyword">string</span>.Empty);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">
<p><span style="font-size:small"><strong>Note: </strong>Currently the tuple feature depends on the &ldquo;System.ValueTuple&rdquo; package. You can search for it on nuget.org by checking the &ldquo;Include prerelease&rdquo; option.&nbsp;</span></p>
</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:small"><span style="font-weight:bold">Deconstruction</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>C# 7.0 can split t</span><span>he </span><span>t</span><span>uple value
</span><span>in</span><span>to individual variables</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">// deconstruct for function tuple return value
(string a, string b, string c) = SplitFullName(&quot;Black Big Smith&quot;);
Console.WriteLine($&quot;Deconstruct on \&quot;SplitFullName\&quot; return value is a: {a}, b: {b}, c: {c}&quot;);

/// &lt;summary&gt;
/// 
/// &lt;/summary&gt;
/// &lt;param name=&quot;fullName&quot;&gt;&lt;/param&gt;
/// &lt;returns&gt;Return value is named tuple&lt;/returns&gt;
static (string FirstName, string MiddleName, string LastName) SplitFullName(string fullName)
{
	var names = fullName.Split(' ', '\t');
	switch (names.Length) {
		case 2: {
				return (FirstName: names[0], MiddleName: string.Empty, LastName: names[1]);
			}
		case 3: {
				return (FirstName: names[0], MiddleName: names[1], LastName: names[2]);
			}
		default: {
				return (FirstName: fullName, MiddleName: string.Empty, LastName: string.Empty);
			}
	}
}
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__com">//&nbsp;deconstruct&nbsp;for&nbsp;function&nbsp;tuple&nbsp;return&nbsp;value</span>&nbsp;
(<span class="cs__keyword">string</span>&nbsp;a,&nbsp;<span class="cs__keyword">string</span>&nbsp;b,&nbsp;<span class="cs__keyword">string</span>&nbsp;c)&nbsp;=&nbsp;SplitFullName(<span class="cs__string">&quot;Black&nbsp;Big&nbsp;Smith&quot;</span>);&nbsp;
Console.WriteLine($<span class="cs__string">&quot;Deconstruct&nbsp;on&nbsp;\&quot;SplitFullName\&quot;&nbsp;return&nbsp;value&nbsp;is&nbsp;a:&nbsp;{a},&nbsp;b:&nbsp;{b},&nbsp;c:&nbsp;{c}&quot;</span>);&nbsp;
&nbsp;
<span class="cs__com">///&nbsp;&lt;summary&gt;</span>&nbsp;
<span class="cs__com">///&nbsp;</span>&nbsp;
<span class="cs__com">///&nbsp;&lt;/summary&gt;</span>&nbsp;
<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;fullName&quot;&gt;&lt;/param&gt;</span>&nbsp;
<span class="cs__com">///&nbsp;&lt;returns&gt;Return&nbsp;value&nbsp;is&nbsp;named&nbsp;tuple&lt;/returns&gt;</span>&nbsp;
<span class="cs__keyword">static</span>&nbsp;(<span class="cs__keyword">string</span>&nbsp;FirstName,&nbsp;<span class="cs__keyword">string</span>&nbsp;MiddleName,&nbsp;<span class="cs__keyword">string</span>&nbsp;LastName)&nbsp;SplitFullName(<span class="cs__keyword">string</span>&nbsp;fullName)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;names&nbsp;=&nbsp;fullName.Split(<span class="cs__string">'&nbsp;'</span>,&nbsp;<span class="cs__string">'\t'</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">switch</span>&nbsp;(names.Length)&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">case</span>&nbsp;<span class="cs__number">2</span>:&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;(FirstName:&nbsp;names[<span class="cs__number">0</span>],&nbsp;MiddleName:&nbsp;<span class="cs__keyword">string</span>.Empty,&nbsp;LastName:&nbsp;names[<span class="cs__number">1</span>]);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">case</span>&nbsp;<span class="cs__number">3</span>:&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;(FirstName:&nbsp;names[<span class="cs__number">0</span>],&nbsp;MiddleName:&nbsp;names[<span class="cs__number">1</span>],&nbsp;LastName:&nbsp;names[<span class="cs__number">2</span>]);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">default</span>:&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;(FirstName:&nbsp;fullName,&nbsp;MiddleName:&nbsp;<span class="cs__keyword">string</span>.Empty,&nbsp;LastName:&nbsp;<span class="cs__keyword">string</span>.Empty);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:10pt">You can also deconstruct a class instance if the class has
</span><span style="font-size:10pt">an </span><span style="font-size:10pt">instant method or
</span><span style="font-size:10pt">an </span><span style="font-size:10pt">extension method</span><span style="font-size:10pt">
</span><span style="font-size:10pt">in</span><span style="font-size:10pt"> the form:
</span></div>
<div class="endscriptcode"><span style="font-size:10pt">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public void Deconstruct(out T1 x1, out T2 x2, .., out Tn &nbsp;&nbsp;xn)
{
 &hellip;
}</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Deconstruct(<span class="cs__keyword">out</span>&nbsp;T1&nbsp;x1,&nbsp;<span class="cs__keyword">out</span>&nbsp;T2&nbsp;x2,&nbsp;..,&nbsp;<span class="cs__keyword">out</span>&nbsp;Tn&nbsp;&nbsp;&nbsp;xn)&nbsp;
{&nbsp;
&nbsp;&hellip;&nbsp;
}</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
</span></div>
<div class="endscriptcode">&nbsp;</div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">// deconstruct for class
var point = new My3DPoint(2, 3, 5);
(int x, int y, int z) = point;
Console.WriteLine($&quot;Deconstruct on \&quot;My3DPoint\&quot; instance. x: {x}, y: {y}, z: {z}&quot;);

class My3DPoint
{
	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }

	public My3DPoint(int x, int y, int z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	/// &lt;summary&gt;
	/// Only class declaring &quot;Deconstruct&quot; method will support deconstruct
	/// &lt;/summary&gt;
	/// &lt;param name=&quot;x&quot;&gt;&lt;/param&gt;
	/// &lt;param name=&quot;y&quot;&gt;&lt;/param&gt;
	/// &lt;param name=&quot;z&quot;&gt;&lt;/param&gt;
	public void Deconstruct(out int x, out int y, out int z)
	{
		x = X;
		y = Y;
		z = Z;
	}
}
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__com">//&nbsp;deconstruct&nbsp;for&nbsp;class</span>&nbsp;
var&nbsp;point&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;My3DPoint(<span class="cs__number">2</span>,&nbsp;<span class="cs__number">3</span>,&nbsp;<span class="cs__number">5</span>);&nbsp;
(<span class="cs__keyword">int</span>&nbsp;x,&nbsp;<span class="cs__keyword">int</span>&nbsp;y,&nbsp;<span class="cs__keyword">int</span>&nbsp;z)&nbsp;=&nbsp;point;&nbsp;
Console.WriteLine($<span class="cs__string">&quot;Deconstruct&nbsp;on&nbsp;\&quot;My3DPoint\&quot;&nbsp;instance.&nbsp;x:&nbsp;{x},&nbsp;y:&nbsp;{y},&nbsp;z:&nbsp;{z}&quot;</span>);&nbsp;
&nbsp;
<span class="cs__keyword">class</span>&nbsp;My3DPoint&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;X&nbsp;{&nbsp;<span class="cs__keyword">get</span>;&nbsp;<span class="cs__keyword">set</span>;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;Y&nbsp;{&nbsp;<span class="cs__keyword">get</span>;&nbsp;<span class="cs__keyword">set</span>;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;Z&nbsp;{&nbsp;<span class="cs__keyword">get</span>;&nbsp;<span class="cs__keyword">set</span>;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;My3DPoint(<span class="cs__keyword">int</span>&nbsp;x,&nbsp;<span class="cs__keyword">int</span>&nbsp;y,&nbsp;<span class="cs__keyword">int</span>&nbsp;z)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;X&nbsp;=&nbsp;x;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Y&nbsp;=&nbsp;y;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Z&nbsp;=&nbsp;z;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;Only&nbsp;class&nbsp;declaring&nbsp;&quot;Deconstruct&quot;&nbsp;method&nbsp;will&nbsp;support&nbsp;deconstruct</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;/summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;x&quot;&gt;&lt;/param&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;y&quot;&gt;&lt;/param&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;z&quot;&gt;&lt;/param&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Deconstruct(<span class="cs__keyword">out</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;x,&nbsp;<span class="cs__keyword">out</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;y,&nbsp;<span class="cs__keyword">out</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;z)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;x&nbsp;=&nbsp;X;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;y&nbsp;=&nbsp;Y;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;z&nbsp;=&nbsp;Z;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode"></div>
<div class="endscriptcode"><span style="font-size:small; font-weight:bold">Local functions</span></div>
<p><span style="font-size:10pt">Now you can define </span><span style="font-size:10pt">a
</span><span style="font-size:10pt">function inside </span><span style="font-size:10pt">&nbsp;</span><span style="font-size:10pt">the</span><span style="font-size:10pt"> function like
</span><span style="font-size:10pt">javascript</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">class LocalFunctionsSample
{
	public static void Run()
	{
		void print(string content)
		{
			Console.WriteLine($&quot;Call the local function to print \&quot;{content}\&quot;&quot;);
		}

		print(&quot;Hello Local Function&quot;);
	}
}
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">class</span>&nbsp;LocalFunctionsSample&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Run()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">void</span>&nbsp;print(<span class="cs__keyword">string</span>&nbsp;content)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine($<span class="cs__string">&quot;Call&nbsp;the&nbsp;local&nbsp;function&nbsp;to&nbsp;print&nbsp;\&quot;{content}\&quot;&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;print(<span class="cs__string">&quot;Hello&nbsp;Local&nbsp;Function&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode"></div>
<div class="endscriptcode"><span style="font-size:12pt; font-weight:bold">Digit separator</span></div>
<p><span style="font-size:10pt">C# 7.0 allows _ as </span><span style="font-size:10pt">a
</span><span style="font-size:10pt">digit separator, it makes </span><span style="font-size:10pt">the
</span><span style="font-size:10pt">code clear</span><span style="font-size:10pt">er</span><span style="font-size:10pt"> if the number is
</span><span style="font-size:10pt">too</span><span style="font-size:10pt"> long to read</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">class DigitSeperatorSample
{
	public static void Run()
	{
		// integer
		int i = 1000;
		int seperatedInteger = 1_000;
		Console.WriteLine($&quot;Seperate integer: {i == seperatedInteger}&quot;);

		// float
		float f = 1.234f;
		float seperatedFloat = 1.2_34f;
		Console.WriteLine($&quot;Seperate float: {f == seperatedFloat}&quot;);

		// hex
		var x = 0xFF00AA;
		var seperatedX = 0xFF_00_AA;
		Console.WriteLine($&quot;Seperate hex: {x == seperatedX}&quot;);

		// binary
		var b = 0b100010100001;
		var seperatedB = 0b1000_1010_0001;
		Console.WriteLine($&quot;Seperate binary: {b == seperatedB}&quot;);
	}
}
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">class</span>&nbsp;DigitSeperatorSample&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Run()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;integer</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">int</span>&nbsp;i&nbsp;=&nbsp;<span class="cs__number">1000</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">int</span>&nbsp;seperatedInteger&nbsp;=&nbsp;1_000;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine($<span class="cs__string">&quot;Seperate&nbsp;integer:&nbsp;{i&nbsp;==&nbsp;seperatedInteger}&quot;</span>);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;float</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">float</span>&nbsp;f&nbsp;=&nbsp;<span class="cs__number">1</span>.234f;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">float</span>&nbsp;seperatedFloat&nbsp;=&nbsp;<span class="cs__number">1</span>.2_34f;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine($<span class="cs__string">&quot;Seperate&nbsp;float:&nbsp;{f&nbsp;==&nbsp;seperatedFloat}&quot;</span>);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;hex</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;x&nbsp;=&nbsp;0xFF00AA;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;seperatedX&nbsp;=&nbsp;0xFF_00_AA;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine($<span class="cs__string">&quot;Seperate&nbsp;hex:&nbsp;{x&nbsp;==&nbsp;seperatedX}&quot;</span>);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;binary</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;b&nbsp;=&nbsp;0b100010100001;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;seperatedB&nbsp;=&nbsp;0b1000_1010_0001;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine($<span class="cs__string">&quot;Seperate&nbsp;binary:&nbsp;{b&nbsp;==&nbsp;seperatedB}&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode"></div>
<div class="endscriptcode"><span style="font-size:12pt; font-weight:bold">Ref returns and locals</span></div>
<p><span style="font-size:10pt">C# 7.0 can now return a ref type value.</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">// the return value is ref type
static ref int Search(int targetNumber, int[] array)
{
	for (int i = 0; i &lt; array.Length; &#43;&#43;i) {
		if (array[i] == targetNumber)
			return ref array[i];
	}
	throw new Exception(&quot;Not found&quot;);
}
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__com">//&nbsp;the&nbsp;return&nbsp;value&nbsp;is&nbsp;ref&nbsp;type</span>&nbsp;
<span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">ref</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;Search(<span class="cs__keyword">int</span>&nbsp;targetNumber,&nbsp;<span class="cs__keyword">int</span>[]&nbsp;array)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">for</span>&nbsp;(<span class="cs__keyword">int</span>&nbsp;i&nbsp;=&nbsp;<span class="cs__number">0</span>;&nbsp;i&nbsp;&lt;&nbsp;array.Length;&nbsp;&#43;&#43;i)&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(array[i]&nbsp;==&nbsp;targetNumber)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;<span class="cs__keyword">ref</span>&nbsp;array[i];&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">throw</span>&nbsp;<span class="cs__keyword">new</span>&nbsp;Exception(<span class="cs__string">&quot;Not&nbsp;found&quot;</span>);&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode"></div>
<div class="endscriptcode"><span style="font-size:12pt; font-weight:bold">More information</span></div>
<p>&nbsp;</p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>VS 15 download:
</span><a href="https://www.visualstudio.com/en-us/downloads/visual-studio-next-downloads-vs.aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">https://www.visualstudio.com/en-us/downloads/visual-studio-next-downloads-vs.aspx</span></a></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>C# 7.0 first-hand look:
</span><a href="https://blogs.msdn.microsoft.com/dotnet/2016/08/24/whats-new-in-csharp-7-0/" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">https://blogs.msdn.microsoft.com/dotnet/2016/08/24/whats-new-in-csharp-7-0/</span></a><span>
</span></span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
