# Use JSOM to change a user's permissions on lists and libraries in SharePoint
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- SharePoint
- SharePoint 2013
## Topics
- JSON
- Permission
## Updated
- 09/22/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode"><img src="https://aka.ms/onecodesampletopbanner1" alt=""></a><strong>&nbsp;</strong><em></em></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:24pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><span style="font-weight:bold; font-size:14pt">How to use JSOM to change a user's permissions on lists and libraries in SharePoint (SPSChangeUserListPermission)</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Introduction</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">&nbsp;</span><span>This sample demonstrates how to use JavaScript Object Model to change user permission on list or library.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Running the Sample</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp; <span style="font-size:11pt">Open the project by double-click</span><span style="font-size:11pt">ing</span><span style="font-size:11pt"> the SPSChangeUserListPermission</span><span style="font-size:11pt">.</span><span style="font-size:11pt">sln.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp; <span style="font-size:11pt">Locate the CSSPSChangeUserListPermission folders under the Layouts folder.</span><span style="font-size:11pt"> Open the ChangePermissions.aspx page. In the
</span><span style="font-size:11pt">CustomPermission function, </span><span style="font-weight:bold">modify the</span><span style="font-weight:bold"> following variables to yours</span><span style="font-size:11pt">:</span><span style="font-size:11pt"> list
</span><span style="font-size:11pt">name (</span><span style="font-size:11pt">RatingList in this sample)</span><span style="font-size:11pt">, username (</span><span style="font-size:11pt">domain\username)</span><span style="font-size:11pt">
</span><span style="font-size:11pt">and the </span><span style="font-size:11pt">RoleType (</span><span style="font-weight:bold">SP.RoleType.contributor</span><span style="font-size:11pt"> in this sample)</span><span style="font-size:11pt">.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp; <span style="font-size:11pt">In the Solution Explorer window, right-click the project name and then click Deploy.
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp; <span style="font-size:11pt">View the ChangePermissions page in the web browser</span><span style="font-size:11pt">.</span><span style="font-size:11pt">
</span><span style="font-size:11pt">T</span><span style="font-size:11pt">he link resembles the following:</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-weight:bold; text-decoration:underline">Yoursite/_layouts/15/CSSPSChangeUserListPermission/changepermissions.aspx</span><span style="font-size:11pt">.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-weight:bold">B</span><span style="font-weight:bold">efore</span><span style="font-size:11pt"> running the changepermissions page, we will have
</span><span style="font-size:11pt">list permissions</span><span style="font-size:11pt"> as</span><span style="font-size:11pt">
</span><span style="font-size:11pt">shown </span><span style="font-size:11pt">below:</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt"><img src="119784-image.png" alt="" width="575" height="267" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-weight:bold">After</span><span style="font-size:11pt"> run</span><span style="font-size:11pt">ning</span><span style="font-size:11pt"> the changepermissions page,
</span><span style="font-size:11pt">we will get</span><span style="font-size:11pt"> list permissions</span><span style="font-size:11pt"> of new levels</span><span style="font-size:11pt">
</span><span style="font-size:11pt">as shown below</span><span style="font-size:11pt">:</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt"><img src="119785-image.png" alt="" width="501" height="165" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp; <span style="font-size:11pt">Validation</span><span style="font-size:11pt"> is</span><span style="font-size:11pt"> finished.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Using the Code</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span>&bull;&nbsp;</span><span>Start </span><span style="font-weight:bold">Visual Studio</span><span>.
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span>&bull;&nbsp;</span><span>On the </span><span style="font-weight:bold">File</span><span> menu, click
</span><span style="font-weight:bold">New</span><span>, and then click </span><span style="font-weight:bold">Project</span><span>.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span>&bull;&nbsp;</span><span>In </span><span>the Installed Templates section</span><span>
</span><span>of </span><span>the </span><span style="font-weight:bold">New</span><span>
</span><span style="font-weight:bold">Project</span><span> dialog box,</span><span> expand either Visual Basic or Visual C#, expand
</span><span style="font-weight:bold">SharePoint</span><span>, and then click </span>
<span style="font-weight:bold">Empty SharePoint Project</span><span>.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span>&bull;&nbsp;</span><span>Right-click the project to add the desired folder.
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span>&quot;</span><span style="font-weight:bold">Add</span><span>&quot;</span><span>
</span><span>&gt;&gt;</span><span>&quot;</span><span style="font-weight:bold">SharePoint Layouts Mapped Folder</span><span>&quot;.
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span>&bull;&nbsp;</span><span>Right-click the &quot;</span><span style="font-weight:bold">Layouts</span><span>&quot; folder then add a new folder named &quot;</span><span>ChangeUserListPermission</span><span>&quot;, then add a new Page named &quot;</span><span>ChangePermissions</span><span>.aspx&quot;.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span>&bull;&nbsp;</span><span>Add </span><span>references</span><span> to relevant JavaScript files to the page:</span><span>
</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">html</span>

<pre class="html" id="codePreview">&lt;script type=&quot;text/javascript&quot; src=&quot;jquery-1.9.1.min.js&quot;&gt;&lt;/script&gt;
&lt;script type=&quot;text/javascript&quot; src=&quot;/_layouts/15/sp.runtime.js&quot;&gt;&lt;/script&gt;
&lt;script type=&quot;text/javascript&quot; src=&quot;/_layouts/15/sp.js&quot;&gt;&lt;/script&gt;
&lt;% #if SOME_UNDEFINED_CONSTANT %&gt; 
&lt;script type=&quot;text/ecmascript&quot; src=&quot;/_layouts/SP.core.debug.js&quot; &gt;&lt;/script&gt;
&lt;script type=&quot;text/ecmascript&quot; src=&quot;/_layouts/SP.runtime.debug.js&quot; &gt;&lt;/script&gt;
&lt;script type=&quot;text/ecmascript&quot; src=&quot;/_layouts/SP.debug.js&quot; &gt;&lt;/script&gt;
&lt;% #endif %&gt;
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">&bull;&nbsp; <span style="font-size:11pt">And then use the following script to handle the operation:</span><span style="font-size:11pt">
</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>JavaScript</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">js</span>

<pre class="js" id="codePreview">&lt;script type=&quot;text/javascript&quot;&gt;
        $(document).ready(function () {
            // Call the function.
            CustomPermission();
        });
        // Web of current client context.
        var web;
        // The list\Library will be operation.
        var oList;
        // The user will be operation.
        var oUser;
        // The roles will be operation.
        var roles;
        function CustomPermission() {
            // Get current client context.
            var clientContext = new SP.ClientContext.get_current();
            // Get current web.
            this.web = clientContext.get_web();
            // Provide the existing list name.
            this.oList = this.web.get_lists().getByTitle('RatingList');
            // Break the inheritance. 
            this.oList.breakRoleInheritance(false, true);
            // Get the user(domain\username).
            this.oUser = this.web.ensureUser(&quot;seiyasu\\seiya&quot;);
            // Define the roles that will be operation.
            this.roles = SP.RoleDefinitionBindingCollection.newObject(clientContext);
            this.roles.add(web.get_roleDefinitions().getByType(SP.RoleType.contributor));
            // Register the role for the user.
            this.oList.get_roleAssignments().add(this.oUser, this.roles)
            clientContext.load(this.web);
            clientContext.load(this.oUser);
            clientContext.load(this.oList);
            //Make a query call to execute the above statements
            clientContext.executeQueryAsync(Function.createDelegate(this, this.onQuerySuccess),
                Function.createDelegate(this, this.onQueryFailure));
        }
      
        function onQuerySuccess() {
            $('#message').text('Updated');
        }
        function onQueryFailure(sender, args) {
            alert('Request failed. ' &#43; args.get_message() &#43; '\n' &#43; args.get_stackTrace());
        }
&lt;/script&gt; 
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span>&bull;&nbsp;</span><span style="font-size:11pt">Now</span><span style="font-size:11pt">
</span><span style="font-size:11pt">y</span><a name="_GoBack"></a><span style="font-size:11pt">ou can build and debug it.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">More Information</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">SharePoint 2013 .NET Server, CSOM, JSOM, and REST API index</span><span style="font-size:11pt">
<br>
</span><a href="http://msdn.microsoft.com/en-us/library/office/dn268594(v=office.15).aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://msdn.microsoft.com/en-us/library/office/dn268594(v=office.15).aspx</span></a><span style="font-size:11pt">
<br>
</span><span style="font-size:11pt">How to: Add and Remove Mapped Folders</span><span style="font-size:11pt">
<br>
</span><a href="http://msdn.microsoft.com/en-us/library/ee231521.aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://msdn.microsoft.com/en-us/library/ee231521.aspx</span></a><span style="font-size:11pt">
<br>
</span><span style="font-size:11pt">SP.ClientContext Class</span><span style="font-size:11pt">
<br>
</span><a href="http://msdn.microsoft.com/en-us/library/office/ff408569(v=office.14).aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://msdn.microsoft.com/en-us/library/office/ff408569(v=office.14).aspx</span></a><span style="font-size:11pt">
<br>
</span><span style="font-size:11pt">SPRoleType enumeration</span><span style="font-size:11pt">
<br>
</span><a href="http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.sproletype.aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://msdn.microsoft.com/en-us/library/microsoft.sharepoint.sproletype.aspx</span></a><span style="font-size:11pt">
<br>
</span><span style="font-size:11pt">SP.RoleDefinitionBindingCollection Class</span><span style="font-size:11pt">
<br>
</span><a href="http://msdn.microsoft.com/en-us/library/office/ff410029(v=office.14).aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://msdn.microsoft.com/en-us/library/office/ff410029(v=office.14).aspx</span></a><span style="font-size:11pt">
<br>
</span><span style="font-size:11pt">How to: Work with Roles Using JavaScript</span><span style="font-size:11pt">
<br>
</span><a href="http://msdn.microsoft.com/en-us/library/office/hh185014(v=office.14).aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://msdn.microsoft.com/en-us/library/office/hh185014(v=office.14).aspx</span></a><span style="font-size:11pt">
<br>
</span><span style="font-size:11pt">SPRoleDefinitionCollection class</span><span style="font-size:11pt">
<br>
</span><a href="http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.sproledefinitioncollection(v=office.15).aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.sproledefinitioncollection(v=office.15).aspx</span></a><span style="font-size:11pt">
<br>
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt"><br>
</span></span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
