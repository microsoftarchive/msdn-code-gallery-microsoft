# User impersonation demo (CppImpersonateUser)
## Requires
- Visual Studio 2008
## License
- MS-LPL
## Technologies
- Windows SDK
## Topics
- Impersonation
## Updated
- 03/01/2012
## Description

<h1>User impersonation demo (<span class="SpellE">C<span style="">pp</span>ImpersonateUser</span>)<span style="">
</span></h1>
<h2>Introduction<span style=""> </span></h2>
<p class="MsoNormal"><span style="">Windows Impersonation is a powerful feature Windows uses frequently in its security model. In general Windows also uses impersonation in its client/server programming model. Impersonation lets a server to temporarily adopt
 the security profile of a client making a resource request. The server can then access resources on behalf of the client, and the OS carries out the access validations.
</span></p>
<p class="MsoNormal"><span style="">A server impersonates a client only within the thread that makes the impersonation request. Thread-control data structures contain an optional entry for an impersonation token. However, a thread's primary token, which represents
 the thread's real security credentials, is always accessible in the process's control structure.
</span></p>
<p class="MsoNormal"><span style="">After the server thread finishes its task, it reverts to its primary security profile. These forms of impersonation are convenient for carrying out specific actions at the request of a client and for ensuring that object
 accesses are audited correctly. </span></p>
<p class="MsoNormal"><span style="">In this code sample we use the <span class="SpellE">
<b style="">LogonUser</b></span> API and the <span class="SpellE"><b style="">ImpersonateLoggedOnUser</b></span> API to impersonate the user represented by the specified user token. Then display the current user via the
<span class="SpellE"><b style="">GetUserName</b></span> API to show user impersonation.
<span class="SpellE"><b style="">LogonUser</b></span> can only be used to log onto the local machine; it cannot log you onto a remote computer. The account that you use in the
<span class="SpellE"><b style="">LogonUser</b></span>() call must also be known to the local machine, either as a local account or as a domain account that is visible to the local computer. If
<span class="SpellE"><b style="">LogonUser</b></span> is successful, then it will give you an access token that specifies the credentials of the user account you chose.
</span></p>
<h2>Running the Sample:<span style=""> </span></h2>
<p class="MsoNormal"><span style=""></span></p>
<p class="MsoNormal">P<span style="">ress <b style="">F5</b> to run this application, you will see the instruction. Then type a valid local account or domain account, and then you will see
</span></p>
<p class="MsoNormal"><span style=""><img src="53384-image.png" alt="" width="501" height="154" align="middle">
</span><span style=""></span></p>
<h2>Using the Code<span style=""> </span></h2>
<p class="MsoListParagraph" style="margin-left:54.0pt"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Get the name of the user associated with the current thread.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
wprintf(L&quot;Before the impersonation ...\n&quot;);
DWORD nSize = ARRAYSIZE(szCurrentUserName);
if (!GetUserName(szCurrentUserName, &nSize))
{
    ReportError(L&quot;GetUserName&quot;);
    goto Cleanup;
}
wprintf(L&quot;The current user is %s\n\n&quot;, szCurrentUserName);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst" style="margin-left:54.0pt"><span style=""></span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:54.0pt"><span style=""></span></p>
<p class="MsoListParagraphCxSpLast" style="margin-left:54.0pt"><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Gather the credential information of the impersonated user.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
wprintf(L&quot;Enter the name of the impersonated user: &quot;);
fgetws(szUserName, ARRAYSIZE(szUserName), stdin);
pc = wcschr(szUserName, L'\n');
if (pc != NULL) *pc = L'\0';  // Remove the trailing L'\n'


wprintf(L&quot;Enter the domain name: &quot;);
fgetws(szDomain, ARRAYSIZE(szDomain), stdin);
pc = wcschr(szDomain, L'\n');
if (pc != NULL) *pc = L'\0';  // Remove the trailing L'\n'


wprintf(L&quot;Enter the password: &quot;);
fgetws(szPassword, ARRAYSIZE(szPassword), stdin);
pc = wcschr(szPassword, L'\n');
if (pc != NULL) *pc = L'\0';  // Remove the trailing L'\n'


// Attempt to log on the user.
if (!LogonUser(szUserName, szDomain, szPassword, 
    LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, &hToken))
{
    ReportError(L&quot;LogonUser&quot;);
    goto Cleanup;
}






 


3.      
Impersonate
the logged on user.





if (!ImpersonateLoggedOnUser(hToken))
{
    ReportError(L&quot;ImpersonateLoggedOnUser&quot;);
    goto Cleanup;
}


// The impersonation is successful.
fSucceeded = TRUE;
wprintf(L&quot;\nThe impersonation is successful\n&quot;);


// Print the name of the user associated with the current thread.
ZeroMemory(szCurrentUserName, sizeof(szCurrentUserName));
nSize = ARRAYSIZE(szCurrentUserName);
if (!GetUserName(szCurrentUserName, &nSize))
{
    ReportError(L&quot;GetUserName&quot;);
    goto Cleanup;
}
wprintf(L&quot;The current user is %s\n\n&quot;, szCurrentUserName);


// Work as the impersonated user.
// ...

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst" style="margin-left:54.0pt"><span style=""><span style="">&nbsp;</span>
</span></p>
<p class="MsoListParagraphCxSpLast" style="margin-left:54.0pt"><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Impersonate the logged on user. </span></p>
<p class="MsoListParagraphCxSpFirst" style="margin-left:54.0pt"><span style=""></span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-left:54.0pt"><span style=""></span></p>
<p class="MsoListParagraphCxSpLast" style="margin-left:54.0pt"><span style=""><span style="">4.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Undo impersonation. (<span class="SpellE"><b style="">WindowsImpersonationContext.Undo</b></span>)
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
Cleanup:


    // Clean up the buffer containing sensitive password.
    SecureZeroMemory(szPassword, sizeof(szPassword));


    // If the impersonation was successful, undo the impersonation.
    if (fSucceeded)
    {
        wprintf(L&quot;Undo the impersonation ...\n&quot;);
        if (!RevertToSelf())
        {
            ReportError(L&quot;RevertToSelf&quot;);
        }


        // Print the name of the user associated with the current thread.
        ZeroMemory(szCurrentUserName, sizeof(szCurrentUserName));
        nSize = ARRAYSIZE(szCurrentUserName);
        if (!GetUserName(szCurrentUserName, &nSize))
        {
            ReportError(L&quot;GetUserName&quot;);
        }
        wprintf(L&quot;The current user is %s\n\n&quot;, szCurrentUserName);
    }
    
    if (hToken)
    {
        CloseHandle(hToken);
        hToken = NULL;
    }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraph" style="margin-left:54.0pt"><span style=""></span></p>
<h2>More Information<span style=""> </span></h2>
<p class="MsoNormal"><span style=""><a href="http://blogs.msdn.com/shawnfa/archive/2005/03/24/401905.aspx">Safe Impersonation
<span class="GramE">With</span> Whidbey</a> </span></p>
<p class="MsoNormal"><span style=""><a href="http://blogs.msdn.com/shawnfa/archive/2005/03/21/400088.aspx">How to Impersonate</a>
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
