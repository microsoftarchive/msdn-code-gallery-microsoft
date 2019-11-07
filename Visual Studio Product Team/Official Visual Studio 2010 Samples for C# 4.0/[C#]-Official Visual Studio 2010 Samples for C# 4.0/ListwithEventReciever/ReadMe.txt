

This sample creates a new SharePoint List(DonationList) and ListInstance(DonationInstance) with an EventReceiver(DonationDeleting) attached. 

To run the sample 

 - Change the Site URL property of the project to a valid SharePoint site on your development system (i.e. http://<MachineName> ).

Once the list is created you can access it from the quick launch menu of your SharePoint site. Create a new donation entry with an amount greater than 0.  The event receiver will prevent you from deleting any donation item with an amount greater than 0.

Note: If you run both VB and C# sample on the same machine, the second project being run will have name conflicts with the package and the list instance.  A dialog will be displayed prompting you to resolve the conflicts.
