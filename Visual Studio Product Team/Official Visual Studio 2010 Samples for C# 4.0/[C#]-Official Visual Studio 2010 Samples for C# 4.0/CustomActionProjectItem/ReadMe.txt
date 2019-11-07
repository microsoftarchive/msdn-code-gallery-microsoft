


This is a sample of a Visual Studio 2010 extension for use with the new SharePoint project templates.  It takes advantage of the extensibility APIs that are part of the new SharePoint development tools within Visual Studio 2010.  The extension is for a SharePoint custom action.  In order to use this sample as intended, you will need to have the Visual Studio 2010 SDK installed.  This can be download from here - http://go.microsoft.com/fwlink/?LinkId=164562.

The solution contains two projects:
- A class library project that implements the extension.
- A VSIX project that packages the extension into a VSIX file. This file is used to install the extension.

To run the sample, open the Visual Basic or C# version of the CustomActionProjectItem solution in Visual Studio 2010 and press F5. The following actions occur:
- The extension is built from the ProjectItemDefinition project.
- The VSIX package is created from the CustomActionProjectItem project.
- An experimental instance of Visual Studio is launched in which the VSIX package is installed.

In the experimental instance of Visual Studio you can test out the custom action by doing the following:
- Create a new Empty SharePoint Project. This project template is available when you select the 2010 node under the SharePoint node in the New Project dialog. Use the same programming language (Visual Basic or C#) as the version of the CustomActionProjectItem project you opened.
- In the SharePoint Customization Wizard, type the site URL of a SharePoint site on your local machine and click "Finish".
- After the project is created, right-click the project node in Solution Explorer and select "Add" | "New Item...".
- In the Add New Item dialog, click the "SharePoint" node, select the "Custom Action" item, and then click "Add".

The custom action is added to your project. You can try the following tasks:
- In Solution Explorer, right-click the CustomAction node and click the “View Custom Action Designer” menu item. This shortcut menu item is added by the extension.
- In Solution Explorer, select the CustomAction node and then open the Properties window. Verify that a custom property nameed "Custom Action Property" appears in the Properties sindow. This property is added by the extension. 
- Edit the Elements.xml file under the CustomAction project item and press F5 to deploy it to the local site that you specified when you created the project.



