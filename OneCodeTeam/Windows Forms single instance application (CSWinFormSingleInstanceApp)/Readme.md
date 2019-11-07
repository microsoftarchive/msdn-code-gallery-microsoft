# Windows Forms single instance application (CSWinFormSingleInstanceApp)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Windows Forms
## Topics
- Singleton
## Updated
- 07/22/2012
## Description

<h1><span style="font-family:新宋体">WINDOWS FORMS APPLICATION</span> (<span style="font-family:新宋体">CSWinFormSingleInstanceApp</span>)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The SingleInstanceApp sample demonstrates how to achieve the goal that only one instance of the application is allowed in Windows Forms application. In this sample, we are taking advantage of VB.NET Framework since it has built-in support
 for single instance application.<span style="">&nbsp; </span></p>
<h2>Running the Sample</h2>
<p class="MsoNormal"><span style=""><img src="61554-image.png" alt="" width="300" height="300" align="middle">
</span></p>
<p class="MsoNormal"><span style=""><img src="61555-image.png" alt="" width="300" height="300" align="middle">
</span></p>
<h2>Using the Code</h2>
<p class="MsoListParagraphCxSpFirst" style="text-indent:5.0pt"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Create a LoginForm which will be used to authenticate the user's indentity.
</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:5.0pt"><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Create a MainForm which the user will be redirect to after the user has been authenticated.
</p>
<p class="MsoListParagraphCxSpLast" style="text-indent:5.0pt"><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Create a class inheriting from WindowsFormsApplicationBase, and name<span style="">
</span>it &quot;SingleInstanceApp&quot;.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
//We need to add Microsoft.VisualBasic reference to use
//WindowsFormsApplicationBase type.
class SingleInstanceApp : WindowsFormsApplicationBase 
{
    public SingleInstanceApp()
    {
    }
    public SingleInstanceApp(Form f)
    {
        //set IsSingleInstance property to true to make the application 
        base.IsSingleInstance = true;
        //set MainForm of the application.
        this.MainForm = f;
    }
}

</pre>
<pre id="codePreview" class="csharp">
//We need to add Microsoft.VisualBasic reference to use
//WindowsFormsApplicationBase type.
class SingleInstanceApp : WindowsFormsApplicationBase 
{
    public SingleInstanceApp()
    {
    }
    public SingleInstanceApp(Form f)
    {
        //set IsSingleInstance property to true to make the application 
        base.IsSingleInstance = true;
        //set MainForm of the application.
        this.MainForm = f;
    }
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraph" style="text-indent:5.0pt"><span style=""><span style="">4.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Create a class <span class="GramE">&quot; SingleInstanceAppStarter</span> &quot; which will be specialized used to start the MessageLoop.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
public class SingleInstanceAppStarter
{
    static SingleInstanceApp app = null;


    //Construct SingleInstanceApp object, and invoke its run method
    public static void Start(Form f, StartupNextInstanceEventHandler handler)
    {
        if (app == null && f != null)
            app = new SingleInstanceApp(f);


        //wire up StartupNextInstance event handler
        app.StartupNextInstance &#43;= handler;
        app.Run(Environment.GetCommandLineArgs());
    }
}

</pre>
<pre id="codePreview" class="csharp">
public class SingleInstanceAppStarter
{
    static SingleInstanceApp app = null;


    //Construct SingleInstanceApp object, and invoke its run method
    public static void Start(Form f, StartupNextInstanceEventHandler handler)
    {
        if (app == null && f != null)
            app = new SingleInstanceApp(f);


        //wire up StartupNextInstance event handler
        app.StartupNextInstance &#43;= handler;
        app.Run(Environment.GetCommandLineArgs());
    }
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraph" style="text-indent:5.0pt"><span style=""><span style="">5.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>If the user has not loggined in the application, and the user tries to start another instance, the LoginForm will be activated.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
//The handler when attemping to start another instance of this application
//We can customize the logic here for which form to activate in different 
//conditions. Like in this sample, we will be selctively activate the LoginForm
//or MainForm based on the login state of the user.
static void StartNewInstance(object sender, StartupNextInstanceEventArgs e)
{
    FormCollection forms = Application.OpenForms;
    if (forms[&quot;LoginForm&quot;] != null && forms[&quot;LoginForm&quot;].WindowState== FormWindowState.Minimized)
    {
        forms[&quot;LoginForm&quot;].WindowState = FormWindowState.Normal;
        forms[&quot;LoginForm&quot;].Activate();
    }
    else if (forms[&quot;LoginForm&quot;] == null && GlobleData.IsUserLoggedIn == false)
    {
        LoginForm f = new LoginForm();
        f.ShowDialog();
    }
}

</pre>
<pre id="codePreview" class="csharp">
//The handler when attemping to start another instance of this application
//We can customize the logic here for which form to activate in different 
//conditions. Like in this sample, we will be selctively activate the LoginForm
//or MainForm based on the login state of the user.
static void StartNewInstance(object sender, StartupNextInstanceEventArgs e)
{
    FormCollection forms = Application.OpenForms;
    if (forms[&quot;LoginForm&quot;] != null && forms[&quot;LoginForm&quot;].WindowState== FormWindowState.Minimized)
    {
        forms[&quot;LoginForm&quot;].WindowState = FormWindowState.Normal;
        forms[&quot;LoginForm&quot;].Activate();
    }
    else if (forms[&quot;LoginForm&quot;] == null && GlobleData.IsUserLoggedIn == false)
    {
        LoginForm f = new LoginForm();
        f.ShowDialog();
    }
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraph" style="text-indent:5.0pt"><span style=""><span style="">6.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>If the user <span class="SpellE">loggined</span> in the <span class="GramE">
application,</span> and the user tries to start another instance, the MainForm will be activated.<span style="">&nbsp;&nbsp;&nbsp;
</span></p>
<h2>More Information </h2>
<p class="MsoListParagraph" style="text-indent:5.0pt"><span style="font-family:Symbol"><span style="">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/magazine/cc163741.aspx">NET MATTERS: Single-Instance apps</a></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
