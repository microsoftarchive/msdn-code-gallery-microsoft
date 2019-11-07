# VB app invokes PowerShell script (VBPowerShell)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Powershell
## Topics
- Powershell
## Updated
- 03/01/2012
## Description

<h1><span style="">VB</span> app invokes PowerShell script (<span class="SpellE"><span style="">VB</span>PowerShell</span>)<span style="">
</span></h1>
<h2>Introduction</h2>
<p class="MsoNormal"><br>
This sample indicates how to call <span class="SpellE"><b style="">Powershell</b></span> from VB.NET language. It first<span style="">
</span>creates a <span class="SpellE"><b style="">Runspace</b></span> object in
<span class="SpellE"><b style="">System.Management.Automation</b></span> namespace. Then it creates a
<b style="">Pipeline</b> from <span class="SpellE"><b style="">Runspace</b></span>. The
<b style="">Pipeline</b> is used to host a line of<span style=""> </span>commands which are supposed to be executed. The example call
<b style="">Get-Process </b>command to get all processes whose name start with &quot;<span style="">V</span>&quot;<span style="">.
</span></p>
<h2><span style="">Building</span> the Sample<span style=""> </span></h2>
<p class="MsoNormal"><span style="">To create this project, we first need to install PowerShell. We can find the download link from the following KB article:
<a href="http://support.microsoft.com/kb/968929">http://support.microsoft.com/kb/968929</a>
</span></p>
<h2>Running the Sample<span style=""> </span></h2>
<p class="MsoNormal"><span style="">Press F5 to run this application, and you will see following result.
</span></p>
<p class="MsoNormal"><span style=""><img src="53323-image.png" alt="" width="458" height="239" align="middle">
</span><span style=""></span></p>
<h2>Using the Code<span style=""> </span></h2>
<p class="MsoListParagraph" style=""><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Create a <span class="SpellE"><b style="">RunSpace</b></span> to host the
<span class="SpellE"><b style="">Powershell</b></span> script environment using
<span class="SpellE"><b style="">RunspaceFactory.CreateRunSpace</b></span>. </span>
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>
<pre class="hidden">
Dim runSpace As Runspace = RunspaceFactory.CreateRunspace()
runSpace.Open()

</pre>
<pre id="codePreview" class="vb">
Dim runSpace As Runspace = RunspaceFactory.CreateRunspace()
runSpace.Open()

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span style=""></span></p>
<p class="MsoListParagraphCxSpLast" style=""><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Create a <b style="">Pipeline</b> to host commands to be executed using
<span class="SpellE"><b style="">Runspace.CreatePipeline</b></span>. </span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>
<pre class="hidden">
Dim pipeLine As Pipeline = runSpace.CreatePipeline()

</pre>
<pre id="codePreview" class="vb">
Dim pipeLine As Pipeline = runSpace.CreatePipeline()

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span style=""></span></p>
<p class="MsoListParagraphCxSpLast" style=""><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Create a <b style="">Command</b> object by passing the command to the constructor.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>
<pre class="hidden">
Dim getProcessCStarted As New Command(&quot;Get-Process&quot;)


' Add parameters to the Command. 
getProcessCStarted.Parameters.Add(&quot;name&quot;, &quot;V*&quot;)

</pre>
<pre id="codePreview" class="vb">
Dim getProcessCStarted As New Command(&quot;Get-Process&quot;)


' Add parameters to the Command. 
getProcessCStarted.Parameters.Add(&quot;name&quot;, &quot;V*&quot;)

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span style=""></span></p>
<p class="MsoListParagraphCxSpLast" style=""><span style=""><span style="">4.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Add the commands to the <b style="">Pipeline</b>.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>
<pre class="hidden">
pipeLine.Commands.Add(getProcessCStarted)

</pre>
<pre id="codePreview" class="vb">
pipeLine.Commands.Add(getProcessCStarted)

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span style=""></span></p>
<p class="MsoListParagraphCxSpLast" style=""><span style=""><span style="">5.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Run all commands in the current pipeline by calling
<span class="SpellE"><b style="">Pipeline.Invoke</b></span>. It returns a <span class="SpellE">
<b style="">System.Collections.ObjectModel.Collection</b></span> object.<span style="">&nbsp;
</span>In this example, the executed script is &quot;Get-Process -name V*&quot;. </span>
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>
<pre class="hidden">
Dim vNameProcesses As Collection(Of PSObject) = pipeLine.Invoke()
Dim psObject As PSObject
For Each psObject In vNameProcesses
    Dim process As Process = TryCast(psObject.BaseObject, Process)
    Console.WriteLine(&quot;Process Name: {0}&quot;, process.ProcessName)
Next

</pre>
<pre id="codePreview" class="vb">
Dim vNameProcesses As Collection(Of PSObject) = pipeLine.Invoke()
Dim psObject As PSObject
For Each psObject In vNameProcesses
    Dim process As Process = TryCast(psObject.BaseObject, Process)
    Console.WriteLine(&quot;Process Name: {0}&quot;, process.ProcessName)
Next

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraph"><span style=""></span></p>
<h2>More Information<span style=""> </span></h2>
<p class="MsoNormal"><span style=""><a href="http://msdn.microsoft.com/en-us/library/system.management.automation.runspaces.runspace(VS.85).aspx">MSDN: Runspace Class</a></span><span style="">
</span></p>
<p class="MsoNormal"><span style=""><a href="http://msdn.microsoft.com/en-us/library/ms569128(VS.85).aspx">MSDN: Pipeline.Invoke Method ()</a></span><span style="">
</span></p>
<p class="MsoNormal"><span style=""><a href="http://social.msdn.microsoft.com/Forums/en-AU/csharpgeneral/thread/faa70c95-6191-4f64-bb5a-5b67b8453237">How to call Powershell Script function from C# ?</a>
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
