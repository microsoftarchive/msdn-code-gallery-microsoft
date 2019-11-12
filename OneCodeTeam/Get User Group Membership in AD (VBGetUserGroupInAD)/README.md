# Get User Group Membership in AD (VBGetUserGroupInAD)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- ADSI
- Directory Services
## Topics
- ADSI
- Active Directory
## Updated
- 01/31/2012
## Description

<div class="WordSection1">
<h2><span style="font-size:14.0pt; line-height:115%">Get User Group Membership in AD (VBGetUserGroupInAD)</span></h2>
<h2>Introduction</h2>
<p class="MsoNormal">This sample application demonstrates how to perform a search on the user&rsquo;s group membership in Active Directory. This demonstrates the recursive looping method. Also it shows how to get the Object SID for the group</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">You can execute this sample by creating the exe via Visual Studio.</p>
<p class="MsoNormal">In order to execute the application, you must need to consider the followings:</p>
<p class="MsoListParagraphCxSpFirst" style="margin-bottom:.0001pt; text-indent:-.25in; line-height:normal; text-autospace:none">
<span style="font-family:Symbol"><span>&middot;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Please change the distingusihedName of the domain as per your domain environment. This you should find at the line# 31 in the file VBGetUserGroupInAD.VB. Currently it is using
<span style="font-size:9.5pt; font-family:Consolas; color:#a31515">DC=contoso,DC=com</span></p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span style="font-family:Symbol"><span>&middot;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Please pass the appropriate user&rsquo;s sAMAccountName as the second parameter, for which we are searching the group membership in AD</p>
<h2>Using the Code</h2>
<p class="MsoNormal">We are using <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.DirectoryServices.aspx" target="_blank" title="Auto generated link to System.DirectoryServices">System.DirectoryServices</a> namespace to perform a search on AD. We will be passing the distinguishedName of the domain with the username whose membership we would like to fetch.</p>
<p class="MsoNormal">Once we found the user, we will read the memberOF attribute&rsquo;s value. It is one of the possibilities that the group can be member of another group as well; in this case we would need to do a recursive looping.</p>
<h2>More Information</h2>
<p class="MsoNormal">For more information on <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.DirectoryServices.aspx" target="_blank" title="Auto generated link to System.DirectoryServices">System.DirectoryServices</a> Namespace
<a href="http://msdn.microsoft.com/en-us/library/system.directoryservices.aspx">http://msdn.microsoft.com/en-us/library/system.directoryservices.aspx</a> , DirectoryEntry Class
<a href="http://msdn.microsoft.com/en-us/library/system.directoryservices.directoryentry.aspx">
http://msdn.microsoft.com/en-us/library/system.directoryservices.directoryentry.aspx</a> &amp; DirectorySearcher Class
<a href="http://msdn.microsoft.com/en-us/library/system.directoryservices.directorysearcher.aspx">
http://msdn.microsoft.com/en-us/library/system.directoryservices.directorysearcher.aspx</a></p>
<p class="MsoNormal">For <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Security.Principal.SecurityIdentifier.aspx" target="_blank" title="Auto generated link to System.Security.Principal.SecurityIdentifier">System.Security.Principal.SecurityIdentifier</a> <a href="http://msdn.microsoft.com/en-us/library/system.security.principal.securityidentifier.aspx">
http://msdn.microsoft.com/en-us/library/system.security.principal.securityidentifier.aspx</a></p>
<p class="MsoNormal">&nbsp;</p>
<p class="MsoNormal"><span><br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt=""></a></div>
&nbsp;</span>
<p></p>
<p class="MsoNormal">&nbsp;</p>
</div>
