

To enable this sample to run on Microsoft Office SharePoint Server (MOSS) 2007 you must make the following changes to the workflow project:

 - Change the Debugging Site URL property to "http://localhost/Docs/", or another valid SharePoint site on your development system.

 - Change the Target Library or List to which you are associating the workflow to "Documents", or another list on the site specified as the Debugging Site URL.


These properties can be set in the SharePoint Workflow wizard. To open the SharePoint Workflow wizard, right-click the project node in Solution Explorer and select “SharePoint Debug Settings.”  The Debugging Site URL setting is located on the first page of the wizard, and the Target Library or List setting is located on the second page.

