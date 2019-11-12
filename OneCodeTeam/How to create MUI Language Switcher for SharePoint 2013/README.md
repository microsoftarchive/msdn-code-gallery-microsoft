# How to create MUI Language Switcher for SharePoint 2013
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- SharePoint
- SharePoint 2013
- SharePoint Development
## Topics
- SharePoint
- MUI Language Switcher
## Updated
- 09/22/2016
## Description

<h1><em><img id="154629" src="154629-8171.onecodesampletopbanner.png" alt=""></em></h1>
<h1>Jak vytvořit přep&iacute;nač jazyka MUI pro službu SharePoint 2013 (CSSharePointLangSwitcher)</h1>
<h2>&Uacute;vod</h2>
<p>Tento projekt ukazuje, jak lze vytvořit přep&iacute;nač jazyka <strong>MUI (Multilingual User Interface)</strong> pomoc&iacute; modulu HTTP. V&nbsp;metodě
<strong>PreRequestHandlerExecute</strong> na&scaron;eho vlastn&iacute;ho modulu jsme zkontrolujeme předvolby jazyka dan&eacute;ho uživatele a pot&eacute; jazyky přid&aacute;me do hlavičky ž&aacute;dosti
<strong>Accept-Language</strong> .</p>
<h2>Spu&scaron;těn&iacute; uk&aacute;zky</h2>
<p>Proveďte n&aacute;sleduj&iacute;c&iacute; kroky.</p>
<p>Krok 1: Otevřete soubor CSSharePointLangSwitcher.sln a položku &quot;<strong>Adresa URL webu</strong>&quot; nastavte na svůj vlastn&iacute; web.</p>
<p>Krok 2: <strong>Nasaďte</strong> projekt.</p>
<p>Krok 3: Zobrazte v&nbsp;prohl&iacute;žeči str&aacute;nku sample.aspx.&nbsp; Na str&aacute;nce uvid&iacute;te ovl&aacute;dac&iacute; prvek DropDownList a přep&iacute;nač.&nbsp;<strong>&nbsp;</strong><em>&nbsp;</em></p>
<h1><img id="154630" src="154630-image.png" alt="" width="558" height="242"></h1>
<p><span><span>Ze seznamu DropDownList vyberte jazyk a pot&eacute; klikněte na tlač&iacute;tko &quot;<strong>Change Language</strong>&quot; . Zobraz&iacute; se n&aacute;sleduj&iacute;c&iacute; str&aacute;nka:</span><strong></strong><em></em></span></p>
<p><img id="154631" src="154631-11d46b09-9db5-4f28-af4f-af1c608213a4image.png" alt=""></p>
<p>Krok 4: Ověřen&iacute; je hotov&eacute;.</p>
<h2>Použit&iacute; k&oacute;du</h2>
<p>Logika k&oacute;du: &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;</p>
<p>Krok 1: Vytvořte v&nbsp;jazyce C# &quot;<strong>pr&aacute;zdn&yacute;</strong><strong></strong><strong>projekt SharePoint</strong>&quot; v&nbsp;aplikaci Visual Studio a dejte mu n&aacute;zev &quot;CSSharePointLangSwitcher&quot;.</p>
<p>Krok 2: Přidejte do projektu <strong>str&aacute;nku aplikace</strong> a přejmenujte jej na Sample.aspx.</p>
<p>K&oacute;d html souboru Sample.aspx bude n&aacute;sleduj&iacute;c&iacute;:<strong></strong><em></em></p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">编辑脚本</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">html</span>

<div class="preview">
<pre class="html"><span class="html__tag_start">&lt;%@&nbsp;Assembly</span>&nbsp;<span class="html__attr_name">Name</span>=<span class="html__attr_value">&quot;$SharePoint.Project.AssemblyFullName$&quot;</span>&nbsp;<span class="html__tag_start">%&gt;</span>&nbsp;
<span class="html__tag_start">&lt;%@&nbsp;Import</span>&nbsp;<span class="html__attr_name">Namespace</span>=<span class="html__attr_value">&quot;Microsoft.SharePoint.ApplicationPages&quot;</span>&nbsp;<span class="html__tag_start">%&gt;</span>&nbsp;
<span class="html__tag_start">&lt;%@&nbsp;Register</span>&nbsp;<span class="html__attr_name">Tagprefix</span>=<span class="html__attr_value">&quot;SharePoint&quot;</span>&nbsp;<span class="html__attr_name">Namespace</span>=<span class="html__attr_value">&quot;Microsoft.SharePoint.WebControls&quot;</span>&nbsp;<span class="html__attr_name">Assembly</span>=<span class="html__attr_value">&quot;<a class="libraryLink" href="https://msdn.microsoft.com/cs-CZ/library/Microsoft.SharePoint.aspx" target="_blank" title="Auto generated link to Microsoft.SharePoint">Microsoft.SharePoint</a>,&nbsp;Version=15.0.0.0,&nbsp;Culture=neutral,&nbsp;PublicKeyToken=71e9bce111e9429c&quot;</span>&nbsp;<span class="html__tag_start">%&gt;</span>&nbsp;
<span class="html__tag_start">&lt;%@&nbsp;Register</span>&nbsp;<span class="html__attr_name">Tagprefix</span>=<span class="html__attr_value">&quot;Utilities&quot;</span>&nbsp;<span class="html__attr_name">Namespace</span>=<span class="html__attr_value">&quot;Microsoft.SharePoint.Utilities&quot;</span>&nbsp;<span class="html__attr_name">Assembly</span>=<span class="html__attr_value">&quot;<a class="libraryLink" href="https://msdn.microsoft.com/cs-CZ/library/Microsoft.SharePoint.aspx" target="_blank" title="Auto generated link to Microsoft.SharePoint">Microsoft.SharePoint</a>,&nbsp;Version=15.0.0.0,&nbsp;Culture=neutral,&nbsp;PublicKeyToken=71e9bce111e9429c&quot;</span>&nbsp;<span class="html__tag_start">%&gt;</span>&nbsp;
<span class="html__tag_start">&lt;%@&nbsp;Register</span>&nbsp;<span class="html__attr_name">Tagprefix</span>=<span class="html__attr_value">&quot;asp&quot;</span>&nbsp;<span class="html__attr_name">Namespace</span>=<span class="html__attr_value">&quot;System.Web.UI&quot;</span>&nbsp;<span class="html__attr_name">Assembly</span>=<span class="html__attr_value">&quot;System.Web.Extensions,&nbsp;Version=4.0.0.0,&nbsp;Culture=neutral,&nbsp;PublicKeyToken=31bf3856ad364e35&quot;</span>&nbsp;<span class="html__tag_start">%&gt;</span>&nbsp;
<span class="html__tag_start">&lt;%@&nbsp;Import</span>&nbsp;<span class="html__attr_name">Namespace</span>=<span class="html__attr_value">&quot;Microsoft.SharePoint&quot;</span>&nbsp;<span class="html__tag_start">%&gt;</span>&nbsp;
<span class="html__tag_start">&lt;%@&nbsp;Assembly</span>&nbsp;<span class="html__attr_name">Name</span>=<span class="html__attr_value">&quot;<a class="libraryLink" href="https://msdn.microsoft.com/cs-CZ/library/Microsoft.Web.CommandUI.aspx" target="_blank" title="Auto generated link to Microsoft.Web.CommandUI">Microsoft.Web.CommandUI</a>,&nbsp;Version=15.0.0.0,&nbsp;Culture=neutral,&nbsp;PublicKeyToken=71e9bce111e9429c&quot;</span>&nbsp;<span class="html__tag_start">%&gt;</span>&nbsp;
<span class="html__tag_start">&lt;%@&nbsp;Page</span>&nbsp;<span class="html__attr_name">Language</span>=<span class="html__attr_value">&quot;C#&quot;</span>&nbsp;<span class="html__attr_name">AutoEventWireup</span>=<span class="html__attr_value">&quot;true&quot;</span>&nbsp;<span class="html__attr_name">CodeBehind</span>=<span class="html__attr_value">&quot;Sample.aspx.cs&quot;</span>&nbsp;<span class="html__attr_name">Inherits</span>=<span class="html__attr_value">&quot;CSSharePointLangSwitcher.Layouts.LangSwitcher.Sample&quot;</span>&nbsp;<span class="html__attr_name">DynamicMasterPageFile</span>=<span class="html__attr_value">&quot;~masterurl/default.master&quot;</span>&nbsp;<span class="html__tag_start">%&gt;</span>&nbsp;
&nbsp;
&nbsp;
<span class="html__tag_start">&lt;asp</span>:Content&nbsp;<span class="html__attr_name">ID</span>=<span class="html__attr_value">&quot;PageHead&quot;</span>&nbsp;<span class="html__attr_name">ContentPlaceHolderID</span>=<span class="html__attr_value">&quot;PlaceHolderAdditionalPageHead&quot;</span>&nbsp;<span class="html__attr_name">runat</span>=<span class="html__attr_value">&quot;server&quot;</span><span class="html__tag_start">&gt;&nbsp;
</span>&nbsp;
&nbsp;
<span class="html__tag_end">&lt;/asp:Content&gt;</span>&nbsp;
&nbsp;
&nbsp;
&nbsp;
&nbsp;
<span class="html__tag_start">&lt;asp</span>:Content&nbsp;<span class="html__attr_name">ID</span>=<span class="html__attr_value">&quot;Main&quot;</span>&nbsp;<span class="html__attr_name">ContentPlaceHolderID</span>=<span class="html__attr_value">&quot;PlaceHolderMain&quot;</span>&nbsp;<span class="html__attr_name">runat</span>=<span class="html__attr_value">&quot;server&quot;</span><span class="html__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="html__tag_start">&lt;asp</span>:DropDownList&nbsp;<span class="html__attr_name">ID</span>=<span class="html__attr_value">&quot;ddlLanguages&quot;</span>&nbsp;<span class="html__attr_name">runat</span>=<span class="html__attr_value">&quot;server&quot;</span>&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="html__attr_name">DataTextField</span>=<span class="html__attr_value">&quot;DisplayName&quot;</span>&nbsp;<span class="html__attr_name">DataValueField</span>=<span class="html__attr_value">&quot;LanguageTag&quot;</span>&nbsp;<span class="html__tag_start">&gt;&nbsp;</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="html__tag_end">&lt;/asp:DropDownList&gt;</span>&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="html__tag_start">&lt;asp</span>:Button&nbsp;<span class="html__attr_name">runat</span>=<span class="html__attr_value">&quot;server&quot;</span>&nbsp;<span class="html__attr_name">ID</span>=<span class="html__attr_value">&quot;btnSave&quot;</span>&nbsp;<span class="html__attr_name">Text</span>=<span class="html__attr_value">&quot;Change&nbsp;Language&quot;</span>&nbsp;<span class="html__attr_name">OnClick</span>=<span class="html__attr_value">&quot;btnSave_Click&quot;</span><span class="html__tag_start">/&gt;</span>&nbsp;
<span class="html__tag_end">&lt;/asp:Content&gt;</span>&nbsp;
&nbsp;
&nbsp;
<span class="html__tag_start">&lt;asp</span>:Content&nbsp;<span class="html__attr_name">ID</span>=<span class="html__attr_value">&quot;PageTitle&quot;</span>&nbsp;<span class="html__attr_name">ContentPlaceHolderID</span>=<span class="html__attr_value">&quot;PlaceHolderPageTitle&quot;</span>&nbsp;<span class="html__attr_name">runat</span>=<span class="html__attr_value">&quot;server&quot;</span><span class="html__tag_start">&gt;&nbsp;
</span>Application&nbsp;Page&nbsp;
<span class="html__tag_end">&lt;/asp:Content&gt;</span>&nbsp;
&nbsp;
&nbsp;
<span class="html__tag_start">&lt;asp</span>:Content&nbsp;<span class="html__attr_name">ID</span>=<span class="html__attr_value">&quot;PageTitleInTitleArea&quot;</span>&nbsp;<span class="html__attr_name">ContentPlaceHolderID</span>=<span class="html__attr_value">&quot;PlaceHolderPageTitleInTitleArea&quot;</span>&nbsp;<span class="html__attr_name">runat</span>=<span class="html__attr_value">&quot;server&quot;</span>&nbsp;<span class="html__tag_start">&gt;&nbsp;
</span><span class="html__tag_end">&lt;/asp:Content&gt;</span>&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;<span lang="EN">K&oacute;d na pozad&iacute; souboru Sample.aspx bude n&aacute;sleduj&iacute;c&iacute;</span><span lang="ZH-CN">：</span></div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;partial&nbsp;<span class="cs__keyword">class</span>&nbsp;Sample&nbsp;:&nbsp;LayoutsPageBase&nbsp;
&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;K&oacute;d&nbsp;aktu&aacute;lně&nbsp;vybran&eacute;ho&nbsp;jazyka&nbsp;v&nbsp;souborech&nbsp;cookies.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;strKeyName&nbsp;=&nbsp;<span class="cs__string">&quot;LangSwitcher_Setting&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">protected</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Page_Load(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;EventArgs&nbsp;e)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(!IsPostBack)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Z&iacute;sk&aacute;n&iacute;&nbsp;seznamu&nbsp;nainstalovan&yacute;ch&nbsp;jazyků&nbsp;a&nbsp;nav&aacute;z&aacute;n&iacute;&nbsp;na&nbsp;ovl&aacute;dac&iacute;&nbsp;prvek&nbsp;DropDownList.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SPLanguageCollection&nbsp;languages&nbsp;=&nbsp;SPLanguageSettings.GetGlobalInstalledLanguages(<span class="cs__number">15</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ddlLanguages.DataSource&nbsp;=&nbsp;languages;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ddlLanguages.DataBind();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Přid&aacute;n&iacute;&nbsp;položky&nbsp;na&nbsp;zač&aacute;tek&nbsp;seznamu&nbsp;DropDownList&nbsp;a&nbsp;v&yacute;choz&iacute;&nbsp;nastaven&iacute;&nbsp;jako&nbsp;vybran&eacute;.&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ddlLanguages.Items.Insert(<span class="cs__number">0</span>,&nbsp;<span class="cs__string">&quot;NotSelected&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ddlLanguages.SelectedIndex&nbsp;=&nbsp;<span class="cs__number">0</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;Uložen&iacute;&nbsp;aktu&aacute;lně&nbsp;vybran&eacute;ho&nbsp;jazyka&nbsp;do&nbsp;souboru&nbsp;cookie.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;/summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&lt;param&nbsp;name=&quot;sender&quot;&gt;&lt;/param&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&lt;param&nbsp;name=&quot;e&quot;&gt;&lt;/param&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">protected</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;btnSave_Click(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;EventArgs&nbsp;e)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(ddlLanguages.SelectedIndex&nbsp;&gt;&nbsp;<span class="cs__number">0</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Vybran&yacute;&nbsp;jazyk.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;strLanguage&nbsp;=&nbsp;ddlLanguages.SelectedValue;&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Nastaven&iacute;&nbsp;souborů&nbsp;cookies.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;HttpCookie&nbsp;acookie&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;HttpCookie(strKeyName);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;acookie.Value&nbsp;=&nbsp;strLanguage;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;acookie.Expires&nbsp;=&nbsp;DateTime.MaxValue;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Response.Cookies.Add(acookie);&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Response.Redirect(Request.RawUrl);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;
<p>Krok 3: Přidejte modul <strong>HttpModule</strong> a dejte mu n&aacute;zev &quot;<strong>HTTPSwitcherModule</strong>&quot;. Zaregistrujte vlastn&iacute; obslužnou rutinu ud&aacute;lost&iacute; v&nbsp;ud&aacute;losti
<strong>Init</strong>.</p>
<strong></strong><em></em></div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">编辑脚本</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Init(HttpApplication&nbsp;context)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;以下是如何处理&nbsp;Request&nbsp;事件并为其提供</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;自定义日志记录实现的示例</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;context.PreRequestHandlerExecute&nbsp;&#43;=&nbsp;context_PreRequestHandlerExecute;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;K&oacute;d př&iacute;kazu <strong>context_PreRequestHandlerExecute</strong> vypad&aacute; přibližně takto:<strong></strong><em></em></div>
</div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__com">///&nbsp;&lt;summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;Za&nbsp;předpokladu,&nbsp;že&nbsp;vybran&yacute;&nbsp;jazyk&nbsp;je&nbsp;uložen&nbsp;v&nbsp;souboru&nbsp;cookie.&nbsp;Za&nbsp;prv&eacute;&nbsp;z&iacute;skejte&nbsp;vybran&yacute;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;jazyk&nbsp;ze&nbsp;souboru&nbsp;cookie.&nbsp;Pak&nbsp;přidejte&nbsp;vybran&yacute;&nbsp;jazyk&nbsp;do&nbsp;hlavičky&nbsp;ž&aacute;dosti.&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;Nakonec&nbsp;vybran&yacute;&nbsp;jazyk&nbsp;použijte&nbsp;pro&nbsp;aktu&aacute;ln&iacute;&nbsp;jazykovou&nbsp;verzi.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;/summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;sender&quot;&gt;&lt;/param&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;e&quot;&gt;&lt;/param&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">void</span>&nbsp;context_PreRequestHandlerExecute(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;EventArgs&nbsp;e)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Z&iacute;sk&aacute;n&iacute;&nbsp;aktu&aacute;ln&iacute;&nbsp;aplikace.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;HttpApplication&nbsp;httpApp&nbsp;=&nbsp;sender&nbsp;<span class="cs__keyword">as</span>&nbsp;HttpApplication;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Z&iacute;sk&aacute;n&iacute;&nbsp;v&scaron;ech&nbsp;informac&iacute;&nbsp;specifick&yacute;ch&nbsp;pro&nbsp;HTTP&nbsp;o&nbsp;aktu&aacute;ln&iacute;&nbsp;ž&aacute;dosti&nbsp;HTTP.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;HttpContext&nbsp;context&nbsp;=&nbsp;httpApp.Context;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Aktu&aacute;ln&iacute;&nbsp;jazyk.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;strLanguage&nbsp;=&nbsp;<span class="cs__keyword">string</span>.Empty;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;K&oacute;d&nbsp;aktu&aacute;lně&nbsp;vybran&eacute;ho&nbsp;jazyka&nbsp;v&nbsp;souborech&nbsp;cookies.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;strKeyName&nbsp;=&nbsp;<span class="cs__string">&quot;LangSwitcher_Setting&quot;</span>;&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Nastaven&iacute;&nbsp;aktu&aacute;ln&iacute;ho&nbsp;jazyka.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(httpApp.Request.Cookies[strKeyName]&nbsp;!=&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;strLanguage&nbsp;=&nbsp;httpApp.Request.Cookies[strKeyName].Value;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;strLanguage&nbsp;=&nbsp;<span class="cs__string">&quot;en-us&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;lang&nbsp;=&nbsp;context.Request.Headers[<span class="cs__string">&quot;Accept-Language&quot;</span>];&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(lang&nbsp;!=&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(!lang.Contains(strLanguage))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;context.Request.Headers[<span class="cs__string">&quot;Accept-Language&quot;</span>]&nbsp;=&nbsp;strLanguage&nbsp;&#43;&nbsp;<span class="cs__string">&quot;,&quot;</span>&nbsp;&#43;&nbsp;context.Request.Headers[<span class="cs__string">&quot;Accept-Language&quot;</span>];&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;culture&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;<a class="libraryLink" href="https://msdn.microsoft.com/cs-CZ/library/System.Globalization.CultureInfo.aspx" target="_blank" title="Auto generated link to System.Globalization.CultureInfo">System.Globalization.CultureInfo</a>(strLanguage);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Použit&iacute;&nbsp;jazykov&eacute;&nbsp;verze.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SPUtility.SetThreadCulture(culture,&nbsp;culture);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">catch</span>&nbsp;(Exception&nbsp;ex)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a class="libraryLink" href="https://msdn.microsoft.com/cs-CZ/library/System.Diagnostics.Debug.WriteLine.aspx" target="_blank" title="Auto generated link to System.Diagnostics.Debug.WriteLine">System.Diagnostics.Debug.WriteLine</a>(ex.Message);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<div class="endscriptcode">V tomto př&iacute;kladu použ&iacute;v&aacute;me tř&iacute;du
<strong>SPWebConfigModification</strong> k&nbsp;přid&aacute;n&iacute; modulu do souboru
<strong>Web.Config</strong>. Lze to prov&eacute;st tak&eacute; ručně.
<p>Krok 4: Můžete prov&eacute;st laděn&iacute; a testov&aacute;n&iacute;.</p>
<h2>Dal&scaron;&iacute; informace</h2>
<p>Rozhran&iacute; HttpModule<br>
<a href="http://msdn.microsoft.com/cs-cz/library/system.web.ihttpmodule.aspx">http://msdn.microsoft.com/cs-cz/library/system.web.ihttpmodule.aspx</a></p>
<p>Tř&iacute;da SPWebConfigModification</p>
<p><a href="http://msdn.microsoft.com/cs-cz/library/office/microsoft.sharepoint.administration.spwebconfigmodification(v=office.15).aspx">http://msdn.microsoft.com/cs-cz/library/office/microsoft.sharepoint.administration.spwebconfigmodification(v=office.15).aspx</a></p>
<p>Ud&aacute;lost HttpApplication.PreRequestHandlerExecute</p>
<p><a href="http://msdn.microsoft.com/cs-cz/library/system.web.httpapplication.prerequesthandlerexecute(v=vs.110).aspx">http://msdn.microsoft.com/cs-cz/library/system.web.httpapplication.prerequesthandlerexecute(v=vs.110).aspx</a></p>
<p>Metoda SPUtility.SetThreadCulture</p>
<p><a href="http://msdn.microsoft.com/cs-cz/library/office/microsoft.sharepoint.utilities.sputility.setthreadculture(v=office.15).aspx">http://msdn.microsoft.com/cs-cz/library/office/microsoft.sharepoint.utilities.sputility.setthreadculture(v=office.15).aspx</a><strong></strong><em></em></p>
<img id="154632" src="154632-442eee23-e501-470e-98c0-0325e0d1f95bimage.png" alt=""><em>&nbsp;</em><strong>&nbsp;</strong><em>&nbsp;</em></div>
