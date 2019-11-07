To setup the sample:
Make sure you have installed the Windows Azure Mobile Services Nuget package from - http://go.microsoft.com/fwlink/?LinkID=320478&clcid=0x409

To setup the Azure Mobile service backend:
1.	If you do not have an Windows Azure subscription already, then sign up for a free trial . This will enable you to host 10 mobile services for free. 
2.	Install the command line tools for Windows Azure. 
3.	Download the credentials needed to talk to Windows Azure. You need to do this once to manage all subsequent commands to Mobile Services. 
To do this:
	o	Download the Windows Azure management credentials by entering the command azure account download. This will open up a web page to login to the Management Portal for Windows Azure. Once you log in, it will generate and prompt you to download a publishsettings file for your Windows Azure subscription. Save this file to a location on your machine. 
	o	Import the publishsettings file from the saved location. This will configure your command line client to manage all your Windows Azure services from the command line. To do this, enter the command:
		azure account import [SaveLocation] 
4.	Create a Windows Azure Mobile Service by entering the command:
	azure mobile create [AzureMobileServiceName] [sqlAdminUsername] [sqlAdminPassword] 
5.	Setup the tables for your Windows Azure Mobile Service by entering the following commands:
		azure mobile table create [AzureMobileServiceName] devices --permissions insert=user,update=admin,delete=admin,read=admin --permissions insert=user,update=user,delete=admin,read=user 
		azure mobile table create [AzureMobileServiceName] invites --permissions insert=user,update=user,delete=admin,read=user 
		azure mobile table create [AzureMobileServiceName] items --permissions insert=user,update=admin,delete=user,read=user 
		azure mobile table create [AzureMobileServiceName] listMembers --permissions insert=user,update=admin,delete=user,read=user 
		azure mobile table create [AzureMobileServiceName] profiles --permissions insert=user,update=admin,delete=admin,read=user 
		azure mobile table create [AzureMobileServiceName] settings --permissions insert=admin,update=admin,delete=admin,read=application 
6.	Upload the scripts to your Windows Azure Mobile Service which will set up the database. In the command line, change to the Scripts directory the under sample installation location. Run the following commands to upload this scripts:
		azure mobile script upload [AzureMobileServiceName] table/devices.insert.js 
		azure mobile script upload [AzureMobileServiceName] table/invites.insert.js 
		azure mobile script upload [AzureMobileServiceName] table/invites.read.js 
		azure mobile script upload [AzureMobileServiceName] table/invites.update.js 
		azure mobile script upload [AzureMobileServiceName] table/items.delete.js 
		azure mobile script upload [AzureMobileServiceName] table/items.insert.js 
		azure mobile script upload [AzureMobileServiceName] table/items.read.js 
		azure mobile script upload [AzureMobileServiceName] table/listMembers.insert.js 
		azure mobile script upload [AzureMobileServiceName] table/listmembers.delete.js 
		azure mobile script upload [AzureMobileServiceName] table/listMembers.read.js 
		azure mobile script upload [AzureMobileServiceName] table/profiles.insert.js 
7.	Register your app to receive push notifications. 
8.	In Microsoft Visual Studio, open the sample solution and right click the doto project and then select Store -> Associate App with the Store from the context menu. Complete the wizard by logging in and then select the app you previously registered in the store. 
9.	Collect information from the Live Connect Developer Center.
	o	Go to the Live Connect Developer Center and select your app in the My Applications list. 
	o	Once in your app, click Edit Settings. 
	o	Under API Settings, make a note of the Client ID, Client secret and the Package security identifier (SID) values. 
	o	Set the Redirect Domain to be the URI of your mobile service. 
10.	Configure your Windows Azure Mobile Service with the Client secret and Package security identifier values obtained in the previous step:
		azure mobile config set [AzureMobileServiceName] microsoftAccountClientId [ClientId] 
		azure mobile config set [AzureMobileServiceName] microsoftAccountClientSecret [ClientSecret] 
		azure mobile config set [AzureMobileServiceName] microsoftAccountPackageSID [PackageSID] 
11.	Get the ApplicationUrl and ApplicationKey for your Windows Azure Mobile Service:
		azure mobile show [AzureMobileServiceName] 
12.	Update sample code so that the sample can talk to Mobile Services.
	o	Open up the App.xaml.cs file. 
	o	Replace the mobile-service-url and mobile-service-key with the ApplicationUrl and ApplicationKey retrieved in the previous step. Now your Windows store app is all set to talk to your newly created Mobile Services. 
