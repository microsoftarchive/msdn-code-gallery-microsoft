To compile this sample project:

1. Install the Bing Maps SDK for Windows 8.1 Store apps.
2. Get a Bing maps developer key. Edit Geofencing4SqSample\Mainpage.xaml as XML.
   Replace YOUR_BING_MAPS_KEY with your Bing maps developer key.
3. Reserve your app name in the Windows Store developer dashboard.
4. Edit Geofencing4SqSample\Package.appxmanifest and replace the Package display name 
   (Packaging tab) with your app name.
5. Get your app SID from the Windows Store developer dashboard. This is generated from
   your package display name. Edit Geofencing4SqSample\Constants.cs and update the AppSid
   constant with your app's SID.
6. Register your app at developer.foursquare.com. Set your redirect url to 
   ms-app://YOUR_APP_SID.
7. Edit Geofencing4SqSample\Constants.cs to replace YOUR_FOURSQUARE_CLIENT_ID with the
   Client Id that Foursquare has provisioned for your app.
   Repeat this step for Geofencing4SqSample\BackgroundTasks\Constants.cs.
8. Select an architecture and build your project.  "AnyCPU" is not supported by the Bing
   Maps SDK.