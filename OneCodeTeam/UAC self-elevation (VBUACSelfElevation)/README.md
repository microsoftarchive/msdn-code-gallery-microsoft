# UAC self-elevation (VBUACSelfElevation)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Windows SDK
## Topics
- Security
- UAC
- Integrity Level
## Updated
- 03/01/2012
## Description

<h1>UAC self-elevation (<span class="SpellE"><span style="">VB</span>UACSelfElevation</span>)
<span style=""></span></h1>
<h2>Introduction</h2>
<p class="MsoNormal"><span style="">User Account Control (UAC) is a new security component in Windows Vista and newer operating systems. With UAC fully enabled, interactive administrators normally run with least user privileges. This example demonstrates
 how to check the privilege level of the current process, and how to self-elevate the process by giving explicit consent with the Consent UI.
</span><span style=""></span></p>
<h2>Running the Sample<span style=""> </span></h2>
<p class="MsoNormal"><span style="">You must run this sample on Windows Vista or newer operating systems.
</span></p>
<p class="MsoListParagraphCxSpFirst" style=""><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Turn on UAC in the Control Panel. </span></p>
<p class="MsoListParagraphCxSpMiddle"><span style="">See <a href="http://windows.microsoft.com/en-US/windows-vista/Turn-User-Account-Control-on-or-off">
Turn User Account Control on or off</a> </span></p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Press F5 to run this application, and you will see following window if you are an administrator of this machine.
</span></p>
<p class="MsoListParagraphCxSpMiddle"><span style=""><img src="53313-image.png" alt="" width="260" height="196" align="middle">
</span><span style=""></span></p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Click the Self-elevate button, and UAC will show a dialog to let you confirm the self-elevate operation. Click OK and you will see following window.
</span></p>
<p class="MsoListParagraphCxSpMiddle"><span style=""><img src="53314-image.png" alt="" width="260" height="196" align="middle">
</span><span style=""></span></p>
<p class="MsoListParagraphCxSpLast"><span style="">If you are not an administrator of the machine, you have to type username and password which represent an administrator.
</span></p>
<h2>Using the Code<span style=""> </span></h2>
<p class="MsoListParagraphCxSpFirst" style=""><b style=""><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span></b><b style=""><span style="">Check and display the current process's &quot;run as administrator&quot; status, elevation information and integrity level when the application initializes the main dialog.
</span></b></p>
<p class="MsoListParagraphCxSpLast"><span style="">Create the following four helper functions:
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">
''' &lt;summary&gt;
''' The function checks whether the primary access token of the process belongs 
''' to user account that is a member of the local Administrators group, even if 
''' it currently is not elevated.
''' &lt;/summary&gt;
''' &lt;returns&gt;
''' Returns True if the primary access token of the process belongs to user 
''' account that is a member of the local Administrators group. Returns False 
''' if the token does not.
''' &lt;/returns&gt;
''' &lt;exception cref=&quot;System.ComponentModel.Win32Exception&quot;&gt;
''' When any native Windows API call fails, the function throws a Win32Exception 
''' with the last error code.
''' &lt;/exception&gt;
Friend Function IsUserInAdminGroup() As Boolean


''' &lt;summary&gt;
''' The function checks whether the current process is run as administrator.
''' In other words, it dictates whether the primary access token of the 
''' process belongs to user account that is a member of the local 
''' Administrators group and it is elevated.
''' &lt;/summary&gt;
''' &lt;returns&gt;
''' Returns True if the primary access token of the process belongs to user 
''' account that is a member of the local Administrators group and it is 
''' elevated. Returns False if the token does not.
''' &lt;/returns&gt;
Friend Function IsRunAsAdmin() As Boolean


''' &lt;summary&gt;
''' The function gets the elevation information of the current process. It 
''' dictates whether the process is elevated or not. Token elevation is only 
''' available on Windows Vista and newer operating systems, thus 
''' IsProcessElevated throws a C&#43;&#43; exception if it is called on systems prior 
''' to Windows Vista. It is not appropriate to use this function to determine 
''' whether a process is run as administartor.
''' &lt;/summary&gt;
''' &lt;returns&gt;
''' Returns True if the process is elevated. Returns False if it is not.
''' &lt;/returns&gt;
''' &lt;exception cref=&quot;System.ComponentModel.Win32Exception&quot;&gt;
''' When any native Windows API call fails, the function throws a Win32Exception
''' with the last error code.
''' &lt;/exception&gt;
''' &lt;remarks&gt;
''' TOKEN_INFORMATION_CLASS provides TokenElevationType to check the elevation 
''' type (TokenElevationTypeDefault / TokenElevationTypeLimited / 
''' TokenElevationTypeFull) of the process. It is different from TokenElevation 
''' in that, when UAC is turned off, elevation type always returns 
''' TokenElevationTypeDefault even though the process is elevated (Integrity 
''' Level == High). In other words, it is not safe to say if the process is 
''' elevated based on elevation type. Instead, we should use TokenElevation.
''' &lt;/remarks&gt;
Friend Function IsProcessElevated() As Boolean


''' &lt;summary&gt;
''' The function gets the integrity level of the current process. Integrity 
''' level is only available on Windows Vista and newer operating systems, thus 
''' GetProcessIntegrityLevel throws a C&#43;&#43; exception if it is called on systems 
''' prior to Windows Vista.
''' &lt;/summary&gt;
''' &lt;returns&gt;
''' Returns the integrity level of the current process. It is usually one of 
''' these values:
''' 
'''    SECURITY_MANDATORY_UNTRUSTED_RID - means untrusted level. It is used by 
'''    processes started by the Anonymous group. Blocks most write access.
'''    (SID: S-1-16-0x0)
'''    
'''    SECURITY_MANDATORY_LOW_RID - means low integrity level. It is used by 
'''    Protected Mode Internet Explorer. Blocks write acess to most objects 
'''    (such as files and registry keys) on the system. (SID: S-1-16-0x1000)
''' 
'''    SECURITY_MANDATORY_MEDIUM_RID - means medium integrity level. It is used 
'''    by normal applications being launched while UAC is enabled. 
'''    (SID: S-1-16-0x2000)
'''    
'''    SECURITY_MANDATORY_HIGH_RID - means high integrity level. It is used by 
'''    administrative applications launched through elevation when UAC is 
'''    enabled, or normal applications if UAC is disabled and the user is an 
'''    administrator. (SID: S-1-16-0x3000)
'''    
'''    SECURITY_MANDATORY_SYSTEM_RID - means system integrity level. It is used 
'''    by services and other system-level applications (such as Wininit, 
'''    Winlogon, Smss, etc.)  (SID: S-1-16-0x4000)
''' 
''' &lt;/returns&gt;
''' &lt;exception cref=&quot;System.ComponentModel.Win32Exception&quot;&gt;
''' When any native Windows API call fails, the function throws a Win32Exception 
''' with the last error code.
''' &lt;/exception&gt;
Friend Function GetProcessIntegrityLevel() As Integer

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span style=""></span></p>
<p class="MsoListParagraphCxSpMiddle"><span style="">Some of the methods need to P/Invoke some native Windows APIs. The P/Invoke signatures are defined in
<span class="SpellE">NativeMethod.vb</span>. </span></p>
<p class="MsoListParagraphCxSpLast"><span style="">In the constructor of the main form, check and display the &quot;run as administrator&quot; status, the elevation information, and the integrity level of the current process.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">
' Get and display whether the primary access token of the process belongs 
' to user account that is a member of the local Administrators group even 
' if it currently is not elevated (IsUserInAdminGroup).
Try
    Dim fInAdminGroup As Boolean = Me.IsUserInAdminGroup
    Me.lbInAdminGroup.Text = fInAdminGroup.ToString
Catch ex As Exception
    Me.lbInAdminGroup.Text = &quot;N/A&quot;
    MessageBox.Show(ex.Message, &quot;An error occurred in IsUserInAdminGroup&quot;, _
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
End Try


' Get and display whether the process is run as administrator or not 
' (IsRunAsAdmin).
Try
    Dim fIsRunAsAdmin As Boolean = Me.IsRunAsAdmin
    Me.lbIsRunAsAdmin.Text = fIsRunAsAdmin.ToString
Catch ex As Exception
    Me.lbIsRunAsAdmin.Text = &quot;N/A&quot;
    MessageBox.Show(ex.Message, &quot;An error occurred in IsRunAsAdmin&quot;, _
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
End Try


' Get and display the process elevation information (IsProcessElevated) 
' and integrity level (GetProcessIntegrityLevel). The information is not 
' available on operating systems prior to Windows Vista.
If (Environment.OSVersion.Version.Major &gt;= 6) Then
    ' Running Windows Vista or later (major version &gt;= 6). 


    Try
        ' Get and display the process elevation information.
        Dim fIsElevated As Boolean = Me.IsProcessElevated
        Me.lbIsElevated.Text = fIsElevated.ToString


        ' Update the Self-elevate button to show the UAC shield icon on 
        ' the UI if the process is not elevated.
        Me.btnElevate.FlatStyle = FlatStyle.System
        NativeMethod.SendMessage(Me.btnElevate.Handle, NativeMethod.BCM_SETSHIELD, _
                                 0, IIf(fIsElevated, IntPtr.Zero, New IntPtr(1)))
    Catch ex As Exception
        Me.lbIsElevated.Text = &quot;N/A&quot;
        MessageBox.Show(ex.Message, &quot;An error occurred in IsProcessElevated&quot;, _
                        MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Try


    Try
        ' Get and display the process integrity level.
        Dim IL As Integer = Me.GetProcessIntegrityLevel
        Select Case IL
            Case NativeMethod.SECURITY_MANDATORY_UNTRUSTED_RID
                Me.lbIntegrityLevel.Text = &quot;Untrusted&quot;
            Case NativeMethod.SECURITY_MANDATORY_LOW_RID
                Me.lbIntegrityLevel.Text = &quot;Low&quot;
            Case NativeMethod.SECURITY_MANDATORY_MEDIUM_RID
                Me.lbIntegrityLevel.Text = &quot;Medium&quot;
            Case NativeMethod.SECURITY_MANDATORY_HIGH_RID
                Me.lbIntegrityLevel.Text = &quot;High&quot;
            Case NativeMethod.SECURITY_MANDATORY_SYSTEM_RID
                Me.lbIntegrityLevel.Text = &quot;System&quot;
            Case Else
                Me.lbIntegrityLevel.Text = &quot;Unknown&quot;
        End Select
    Catch ex As Exception
        Me.lbIntegrityLevel.Text = &quot;N/A&quot;
        MessageBox.Show(ex.Message, &quot;An error occurred in GetProcessIntegrityLevel!&quot;, _
                        MessageBoxButtons.OK, MessageBoxIcon.Hand)
    End Try


Else
    Me.lbIsElevated.Text = &quot;N/A&quot;
    Me.lbIntegrityLevel.Text = &quot;N/A&quot;
End If

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span style=""></span></p>
<p class="MsoListParagraphCxSpMiddle"><span style=""></span></p>
<p class="MsoListParagraphCxSpMiddle"><span style=""></span></p>
<p class="MsoListParagraphCxSpLast" style=""><b style=""><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span></b><b style=""><span style="">Handle the click event of the Self-elevate button. When user clicks the button, elevate the process by restarting itself with
<span class="SpellE">ProcessStartInfo.UseShellExecute</span> = true and <span class="SpellE">
ProcessStartInfo.Verb</span> = &quot;<span class="SpellE">runas</span>&quot; if the process is not run as administrator.
</span></b></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">
Private Sub btnElevate_Click(ByVal sender As <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Object.aspx" target="_blank" title="Auto generated link to System.Object">System.Object</a>, ByVal e As <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.EventArgs.aspx" target="_blank" title="Auto generated link to System.EventArgs">System.EventArgs</a>) _
Handles btnElevate.Click
    ' Elevate the process if it is not run as administrator.
    If (Not Me.IsRunAsAdmin) Then
        ' Launch itself as administrator
        Dim proc As New ProcessStartInfo
        proc.UseShellExecute = True
        proc.WorkingDirectory = Environment.CurrentDirectory
        proc.FileName = Application.ExecutablePath
        proc.Verb = &quot;runas&quot;


        Try
            Process.Start(proc)
        Catch
            ' The user refused the elevation.
            ' Do nothing and return directly ...
            Return
        End Try


        Application.Exit()  ' Quit itself
    Else
        MessageBox.Show(&quot;The process is running as administrator&quot;, &quot;UAC&quot;)
    End If
End Sub

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><b style=""><span style=""></span></b></p>
<p class="MsoListParagraphCxSpMiddle" style=""><b style=""><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span></b><b style=""><span style="">Automatically elevate the process when it's started up.
</span></b></p>
<p class="MsoListParagraphCxSpMiddle"><span style="">If your application always requires administrative privileges, such as during an installation step, the operating system can automatically prompt the user for privileges elevation each time your application
 is invoked. </span></p>
<p class="MsoListParagraphCxSpLast"><span style="">If a specific kind of resource (RT_MANIFEST) is found embedded within the application executable, the system looks for the &lt;<span class="SpellE">trustInfo</span>&gt; section and parses its contents.
 Here is an example of this section in the manifest file: </span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">xml</span>

<pre id="codePreview" class="xml">
&lt;trustInfo xmlns=&quot;urn:schemas-microsoft-com:asm.v2&quot;&gt;
   &lt;security&gt;
      &lt;requestedPrivileges&gt;
         &lt;requestedExecutionLevel
            level=&quot;requireAdministrator&quot;
         /&gt;
      &lt;/requestedPrivileges&gt;
   &lt;/security&gt;
&lt;/trustInfo&gt;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span style="">Three different values are possible for the level attribute
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:58.5pt"><span style=""><span style="">a)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span class="SpellE"><span style="">requireAdministrator</span></span><span style="">
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:58.5pt"><span style="">The application must be started with Administrator privileges; it won't run otherwise.
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:58.5pt"><span style=""><span style="">b)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style=""><span style="">&nbsp;</span><span class="SpellE">highestAvailable</span><span style="">&nbsp;&nbsp;
</span></span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:58.5pt"><span style="">The application is started with the highest possible privileges.<span style="">&nbsp;
</span>If the user is logged on with an Administrator account, an elevation prompt<span style="">&nbsp;&nbsp;
</span>appears. If the user is a Standard User, the application is started<span style="">&nbsp;&nbsp;
</span>(without any elevation prompt) with these standard privileges. </span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:58.5pt"><span style=""><span style="">c)<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style=""><span style="">&nbsp;</span><span class="SpellE">asInvoker</span><span style="">&nbsp;&nbsp;
</span></span></p>
<p class="MsoListParagraphCxSpLast" style="margin-left:58.5pt"><span style="">The application is started with the same privileges as the calling application.
</span></p>
<p class="MsoNormal"><span style=""></span></p>
<p class="MsoNormal" style="margin-left:35.45pt"><span style="">To configure the elevation level in this Visual C# Windows Forms project, open the project's properties turn to the Security tab, check the checkbox &quot;Enable
<span class="SpellE">ClickOnce</span> Security Settings&quot;, check &quot;This is a
<span class="SpellE">fulltrust</span> application&quot; and close the application
<span class="SpellE">Properies</span> page. This creates an <span class="SpellE">
app.manifest</span> file and configures the project to embed the manifest. You can open the &quot;<span class="SpellE">app.manifest</span>&quot; file from Solution Explorer by expanding the
<span class="SpellE">Properies</span> folder. The file has the following content by default.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">xml</span>

<pre id="codePreview" class="xml">
&lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
&lt;asmv1:assembly manifestVersion=&quot;1.0&quot; xmlns=&quot;urn:schemas-microsoft-com:asm.v1&quot; 
xmlns:asmv1=&quot;urn:schemas-microsoft-com:asm.v1&quot; xmlns:asmv2=&quot;urn:schemas-microsoft-com:asm.v2&quot; 
xmlns:xsi=&quot;http://www.w3.org/2001/XMLSchema-instance&quot;&gt;
  &lt;assemblyIdentity version=&quot;1.0.0.0&quot; name=&quot;MyApplication.app&quot; /&gt;
  &lt;trustInfo xmlns=&quot;urn:schemas-microsoft-com:asm.v2&quot;&gt;
    &lt;security&gt;
      &lt;requestedPrivileges xmlns=&quot;urn:schemas-microsoft-com:asm.v3&quot;&gt;
        &lt;!-- UAC Manifest Options
            If you want to change the Windows User Account Control level replace the 
            requestedExecutionLevel node with one of the following.


        &lt;requestedExecutionLevel  level=&quot;asInvoker&quot; uiAccess=&quot;false&quot; /&gt;
        &lt;requestedExecutionLevel  level=&quot;requireAdministrator&quot; uiAccess=&quot;false&quot; /&gt;
        &lt;requestedExecutionLevel  level=&quot;highestAvailable&quot; uiAccess=&quot;false&quot; /&gt;


            If you want to utilize File and Registry Virtualization for backward 
            compatibility then delete the requestedExecutionLevel node.
        --&gt;
        &lt;requestedExecutionLevel level=&quot;asInvoker&quot; uiAccess=&quot;false&quot; /&gt;
      &lt;/requestedPrivileges&gt;
      &lt;applicationRequestMinimum&gt;
        &lt;PermissionSet class=&quot;System.Security.PermissionSet&quot; version=&quot;1&quot; 
        Unrestricted=&quot;true&quot; ID=&quot;Custom&quot; SameSite=&quot;site&quot; /&gt;
        &lt;defaultAssemblyRequest permissionSetReference=&quot;Custom&quot; /&gt;
      &lt;/applicationRequestMinimum&gt;
    &lt;/security&gt;
  &lt;/trustInfo&gt;
&lt;/asmv1:assembly&gt;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-left:35.45pt"><span style="">Here we are focusing on the line:
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">xml</span>

<pre id="codePreview" class="xml">
&lt;requestedExecutionLevel level=&quot;asInvoker&quot; uiAccess=&quot;false&quot; /&gt;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-left:35.45pt"><span style="">You can change it to be
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">xml</span>

<pre id="codePreview" class="xml">
&lt;requestedExecutionLevel level=&quot;requireAdministrator&quot; uiAccess=&quot;false&quot; /&gt;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-left:35.45pt"><span class="GramE"><span style="">to</span></span><span style=""> require the application always be started with Administrator privileges.
</span></p>
<h2>More Information<span style=""> </span></h2>
<p class="MsoNormal"><span style=""><a href="http://msdn.microsoft.com/en-us/library/aa511445.aspx">MSDN: User Account Control</a></span><span style="">
</span></p>
<p class="MsoNormal"><span style=""><a href="http://msdn.microsoft.com/en-us/library/bb530410.aspx">MSDN: Windows Vista Application Development Requirements for User Account Control Compatibility</a>
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
