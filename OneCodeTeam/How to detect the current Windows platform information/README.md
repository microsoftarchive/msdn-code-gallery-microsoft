# How to detect the current Windows platform information
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- Windows Desktop App Development
## Topics
- Platform Detector
## Updated
- 09/21/2016
## Description

<h1><em><img id="154417" src="154417-8171.onecodesampletopbanner.png" alt=""></em></h1>
<h2>Proc&eacute;dure de d&eacute;tection des informations de plate-forme Windows actuelles</h2>
<h2>Introduction</h2>
<p>L'exemple illustre la fa&ccedil;on dont nous pouvons d&eacute;tecter les informations suivantes sur la plate-forme Windows actuelle&nbsp;:</p>
<ol>
<li>Nom du syst&egrave;me d'exploitation </li><li>Niveau de service pack install&eacute; sur l'ordinateur </li><li>Nom de l'ordinateur </li><li>Nombre de bits de l'ordinateur </li><li>Groupe de travail/Domaine </li></ol>
<p><a href="http://stackoverflow.com/questions/1953377/how-to-know-a-process-is-32-bit-or-64-bit-programmatically">http://stackoverflow.com/questions/1953377/how-to-know-a-process-is-32-bit-or-64-bit-programmatically</a>&nbsp;&nbsp;&nbsp;
<br>
<a href="https://social.msdn.microsoft.com/Forums/vstudio/fr-fr/24792cdc-2d8e-454b-9c68-31a19892ca53/how-to-check-whether-the-system-is-32-bit-or-64-bit-?forum=csharpgeneral">http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/24792cdc-2d8e-454b-9c68-31a19892ca53</a>&nbsp;
 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</p>
<h2>Ex&eacute;cution de l'exemple</h2>
<p>Appuyez sur Ctrl&#43;F5 pour ex&eacute;cuter l'exemple afin d'afficher le formulaire.<em><br>
</em></p>
<h2><img id="154418" src="154418-134213.png" alt=""></h2>
<h2>Utilisation du code</h2>
<p>D&eacute;clarez la structure suivante pour stocker les informations de syst&egrave;me d'exploitation.<strong>&nbsp;</strong><em>&nbsp;</em></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">struct</span>&nbsp;OSVERSIONINFO&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;dwOSVersionInfoSize;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;dwMajorVersion;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;dwMinorVersion;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;dwBuildNumber;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;dwPlatformId;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;[MarshalAs(UnmanagedType.ByValTStr,&nbsp;SizeConst&nbsp;=&nbsp;<span class="cs__number">128</span>)]&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;szCSDVersion;&nbsp;
}&nbsp;
&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;
<div class="endscriptcode">&nbsp;La fonction suivante remplit la structure OSVERSIONINFO et ajoute &eacute;galement une r&eacute;f&eacute;rence au System.Runtime.InteropServices.dll.</div>
<strong>&nbsp;</strong><em>&nbsp;</em></div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp">[DllImport(<span class="cs__string">&quot;kernel32.Dll&quot;</span>)]&nbsp;
<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">extern</span>&nbsp;<span class="cs__keyword">short</span>&nbsp;GetVersionEx(<span class="cs__keyword">ref</span>&nbsp;OSVERSIONINFO&nbsp;o);&nbsp;
<span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;GetServicePack()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;OSVERSIONINFO&nbsp;os&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;OSVERSIONINFO();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;os.dwOSVersionInfoSize&nbsp;=&nbsp;Marshal.SizeOf(<span class="cs__keyword">typeof</span>(OSVERSIONINFO));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;GetVersionEx(<span class="cs__keyword">ref</span>&nbsp;os);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(os.szCSDVersion&nbsp;==&nbsp;<span class="cs__string">&quot;&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;<span class="cs__string">&quot;No&nbsp;Service&nbsp;Pack&nbsp;Installed&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;os.szCSDVersion;&nbsp;
}&nbsp;
&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;&nbsp;
<div class="endscriptcode">&nbsp;La fonction suivante renvoie s'il s'agit d'une version de serveur du syst&egrave;me d'exploitation de l'ordinateur. Ajoutez &eacute;galement une r&eacute;f&eacute;rence au System.Management.dll.</div>
<strong>&nbsp;</strong><em>&nbsp;</em></div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">bool</span>&nbsp;IsServerVersion()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">using</span>&nbsp;(ManagementObjectSearcher&nbsp;searcher&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;ManagementObjectSearcher(<span class="cs__string">&quot;SELECT&nbsp;*&nbsp;FROM&nbsp;Win32_OperatingSystem&quot;</span>))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">foreach</span>&nbsp;(ManagementObject&nbsp;managementObject&nbsp;<span class="cs__keyword">in</span>&nbsp;searcher.Get())&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;ProductType&nbsp;sera&nbsp;l'un&nbsp;des&nbsp;&eacute;l&eacute;ments&nbsp;suivants&nbsp;:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;1:&nbsp;Station&nbsp;de&nbsp;travail</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;2:&nbsp;Contr&ocirc;leur&nbsp;de&nbsp;domaine</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;3:&nbsp;Serveur</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">uint</span>&nbsp;productType&nbsp;=&nbsp;(<span class="cs__keyword">uint</span>)managementObject.GetPropertyValue(<span class="cs__string">&quot;ProductType&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;productType&nbsp;!=&nbsp;<span class="cs__number">1</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;<span class="cs__keyword">false</span>;&nbsp;
}&nbsp;
&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;
<div class="endscriptcode">D&eacute;clarez la classe Interop suivante pour appeler GetSystemMetrics qui nous aidera &agrave; faire la distinction entre Windows Server&nbsp;2003 et Windows Server&nbsp;2003 R2</div>
<strong>&nbsp;</strong><em>&nbsp;</em></div>
<div class="endscriptcode"></div>
<div class="endscriptcode"></div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;partial&nbsp;<span class="cs__keyword">class</span>&nbsp;WindowsAPI&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;[DllImport(<span class="cs__string">&quot;user32.dll&quot;</span>)]&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">extern</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;GetSystemMetrics(<span class="cs__keyword">int</span>&nbsp;smIndex);&nbsp;
}&nbsp;
&nbsp;
</pre>
</div>
</div>
</div>
</div>
<div class="endscriptcode">
<p>Ajoutez le code suivant au gestionnaire d'&eacute;v&eacute;nements de chargement de formulaires (Form Load Event Handler) et ajoutez &eacute;galement une r&eacute;f&eacute;rence au System.DirectoryServices.dll.</p>
</div>
<div class="endscriptcode"></div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__com">//D&eacute;finir&nbsp;textBox5&nbsp;et&nbsp;textBox6&nbsp;selon&nbsp;le&nbsp;nombre&nbsp;de&nbsp;bits&nbsp;de&nbsp;l'ordinateur&nbsp;et&nbsp;du&nbsp;processus</span>&nbsp;
<span class="cs__keyword">if</span>&nbsp;(Environment.Is64BitOperatingSystem)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;textBox5.Text&nbsp;=&nbsp;<span class="cs__string">&quot;64-Bit&quot;</span>;&nbsp;
}&nbsp;
<span class="cs__keyword">else</span>&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;textBox5.Text&nbsp;=&nbsp;<span class="cs__string">&quot;32-Bit&quot;</span>;&nbsp;
}&nbsp;
&nbsp;
<span class="cs__keyword">if</span>&nbsp;(Environment.Is64BitProcess)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;textBox6.Text&nbsp;=&nbsp;<span class="cs__string">&quot;64-Bit&quot;</span>;&nbsp;
}&nbsp;
<span class="cs__keyword">else</span>&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;textBox6.Text&nbsp;=&nbsp;<span class="cs__string">&quot;32-Bit&quot;</span>;&nbsp;
}&nbsp;
&nbsp;
<span class="cs__com">//D&eacute;finir&nbsp;textbox1&nbsp;sur&nbsp;le&nbsp;nom&nbsp;du&nbsp;syst&egrave;me&nbsp;d'exploitation&nbsp;en&nbsp;v&eacute;rifiant&nbsp;la&nbsp;version&nbsp;du&nbsp;syst&egrave;me&nbsp;d'exploitation.&nbsp;</span>&nbsp;
Version&nbsp;vs&nbsp;=&nbsp;Environment.OSVersion.Version;&nbsp;
&nbsp;
<span class="cs__keyword">bool</span>&nbsp;isServer&nbsp;=&nbsp;IsServerVersion();&nbsp;
&nbsp;
<span class="cs__keyword">switch</span>&nbsp;(vs.Major)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">case</span>&nbsp;<span class="cs__number">3</span>:&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;textBox1.Text&nbsp;=&nbsp;<span class="cs__string">&quot;Windows&nbsp;NT&nbsp;3.51&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">break</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">case</span>&nbsp;<span class="cs__number">4</span>:&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;textBox1.Text&nbsp;=&nbsp;<span class="cs__string">&quot;Windows&nbsp;NT&nbsp;4.0&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">break</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">case</span>&nbsp;<span class="cs__number">5</span>:&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(vs.Minor&nbsp;==&nbsp;<span class="cs__number">0</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;textBox1.Text&nbsp;=&nbsp;<span class="cs__string">&quot;Windows&nbsp;2000&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;<span class="cs__keyword">if</span>&nbsp;(vs.Minor&nbsp;==&nbsp;<span class="cs__number">1</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;textBox1.Text&nbsp;=&nbsp;<span class="cs__string">&quot;Windows&nbsp;XP&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(isServer)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(WindowsAPI.GetSystemMetrics(<span class="cs__number">89</span>)&nbsp;==&nbsp;<span class="cs__number">0</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;textBox1.Text&nbsp;=&nbsp;<span class="cs__string">&quot;Windows&nbsp;Server&nbsp;2003&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;textBox1.Text&nbsp;=&nbsp;<span class="cs__string">&quot;Windows&nbsp;Server&nbsp;2003&nbsp;R2&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;textBox1.Text&nbsp;=&nbsp;<span class="cs__string">&quot;Windows&nbsp;XP&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">break</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">case</span>&nbsp;<span class="cs__number">6</span>:&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(vs.Minor&nbsp;==&nbsp;<span class="cs__number">0</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(isServer)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;textBox1.Text&nbsp;=&nbsp;<span class="cs__string">&quot;Windows&nbsp;Server&nbsp;2008&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;textBox1.Text&nbsp;=&nbsp;<span class="cs__string">&quot;Windows&nbsp;Vista&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;<span class="cs__keyword">if</span>&nbsp;(vs.Minor&nbsp;==&nbsp;<span class="cs__number">1</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(isServer)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;textBox1.Text&nbsp;=&nbsp;<span class="cs__string">&quot;Windows&nbsp;Server&nbsp;2008&nbsp;R2&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;textBox1.Text&nbsp;=&nbsp;<span class="cs__string">&quot;Windows&nbsp;7&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;<span class="cs__keyword">if</span>&nbsp;(vs.Minor&nbsp;==&nbsp;<span class="cs__number">2</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;textBox1.Text&nbsp;=&nbsp;<span class="cs__string">&quot;Windows&nbsp;8&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(isServer)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;textBox1.Text&nbsp;=&nbsp;<span class="cs__string">&quot;Windows&nbsp;Server&nbsp;2012&nbsp;R2&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;textBox1.Text&nbsp;=&nbsp;<span class="cs__string">&quot;Windows&nbsp;8.1&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">break</span>;&nbsp;
}&nbsp;&nbsp;
&nbsp;
<span class="cs__com">//D&eacute;finir&nbsp;textBox2&nbsp;sur&nbsp;le&nbsp;nom&nbsp;de&nbsp;l'ordinateur</span>&nbsp;
textBox2.Text&nbsp;=&nbsp;Environment.MachineName;&nbsp;
&nbsp;
<span class="cs__com">//D&eacute;finir&nbsp;textBox4&nbsp;sur&nbsp;le&nbsp;nom&nbsp;de&nbsp;domaine&nbsp;auquel&nbsp;l'ordinateur&nbsp;est&nbsp;connect&eacute;,&nbsp;sinon&nbsp;le&nbsp;d&eacute;finir&nbsp;sur&nbsp;Groupe&nbsp;de&nbsp;travail</span>&nbsp;
<span class="cs__keyword">try</span>&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;textBox4.Text&nbsp;=&nbsp;<a class="libraryLink" href="https://msdn.microsoft.com/fr-FR/library/System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain.aspx" target="_blank" title="Auto generated link to System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain">System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain</a>().Name;&nbsp;&nbsp;
}&nbsp;
<span class="cs__keyword">catch</span>&nbsp;(ActiveDirectoryObjectNotFoundException&nbsp;ex)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;textBox4.Text&nbsp;=&nbsp;<span class="cs__string">&quot;WORKGROUP&quot;</span>;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
}&nbsp;
&nbsp;
<span class="cs__com">//D&eacute;finir&nbsp;textBox3&nbsp;sur&nbsp;le&nbsp;niveau&nbsp;de&nbsp;service&nbsp;pack&nbsp;install&eacute;&nbsp;sur&nbsp;l'ordinateur</span>&nbsp;
textBox3.Text&nbsp;=&nbsp;GetServicePack();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
</pre>
</div>
</div>
</div>
</div>
</div>
</div>
<h2>Informations suppl&eacute;mentaires</h2>
<p>Proc&eacute;dure pour d&eacute;terminer le niveau du service pack du syst&egrave;me d'exploitation dans Visual C# .NET
<br>
<a href="http://support.microsoft.com/kb/304721">http://support.microsoft.com/kb/304721</a></p>
<p>MSDN&nbsp;: Propri&eacute;t&eacute;s d'environnement<br>
<a href="http://msdn.microsoft.com/en-us/library/System.Environment_properties(v=vs.110).aspx">http://msdn.microsoft.com/en-us/library/System.Environment_properties(v=vs.110).aspx</a></p>
<div class="endscriptcode">
<div class="endscriptcode">
<div class="endscriptcode"></div>
</div>
</div>
<p>&nbsp;</p>
<p><span style="color:#ffffff">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies, and
 reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from
</span></p>
<p><span><img id="154420" src="154420-image.png" alt=""></span></p>
<p><strong>&nbsp;</strong><em>&nbsp;</em></p>
<p>&nbsp;</p>
<div class="endscriptcode"></div>
<p>&nbsp;</p>
<p><em>&nbsp;</em><strong>&nbsp;</strong><em>&nbsp;</em><span style="text-decoration:underline">&nbsp;</span><span style="text-decoration:line-through">&nbsp;</span></p>
