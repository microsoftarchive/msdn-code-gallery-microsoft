Copyright (c) Microsoft Corporation.  All rights reserved.

Conversation Translator Sample 	

Uses the conversation namespace from the Lync Model API to intercept instant messages and provide translation using Bing Web Services.

Sample location:
64Bit Operating System: %PROGRAMFILES(X86)%\Microsoft Lync\SDK\Samples\ConversationTranslator
32Bit Operating System: %PROGRAMFILES%\Microsoft Lync\SDK\Samples\ConversationTranslator


Warnings:
- Requires internet access for the translation content service from Bing Translation
- Please ignore the Visual Studio warning about Web Services without Web Projects. 
Since this project uses an external Web Service, there are no issues.


Features
- Provides a sample architecture for registering for and handling asynchronous Lync SDK events in Silverlight
- Register for two Conversation related events: ParticipantAdded, InstantMessageReceived.
- Use the InstantMessageModality.BeginSendMessage() method and callback
- Uses the Bing Translator Web Service


Prerequisites (for compiling and running in Visual Studio)
- Visual Studio 2010 and Silverlight 4 SDK
- Microsoft Lync SDK

Prerequisites (for running installed sample on client machines)
- Microsoft Lync 


Running the sample
------------------
1. Preferably copy the Conversation Translator folder to a folder outside of Program Files.
2. Open ConversationTranslator.csproj
3. Have Lync running with exactly one conversation window.
	- For debugging purposes, the Conversation Translator will associate itself with the first element in the conversation collection
4. Hit F5 in Visual Studio.
5. Verification: the sample will be ready and working when the language combo boxes are filled with the languages


How to use the sample application
---------------------------------
1. Select your language in the Me: combo box.
2. Select your target audience's language in the Them: combo box.
3. Received messages will automatically be translated to your language
4. To send a message:
	a. Type the message in the input area and hit Enter or click the Translate button
	b. When the translation arrives, edit it if necessary
	c. After verifying the translator hit Enter again or click the Send button
5. You may use the Cancel button to return back to typing the original message


How to install the Conversation Translator into the conversation window extension area
--------------------------------------------------------------------------------------
1. Build the sample project in Visual Studio
2. Create and execute a registry file (.reg) with content similar to the one shown below, 
replacing InternalURL with the absolute location of your project's generated ConversationTranslatorTestPage.html file.

The values shown below are representative of the case in which the Conversation Translator folder is copied to C:\.
Note the fact that '\' is replaced with '/'.

[HKEY_CURRENT_USER\Software\Microsoft\Communicator\ContextPackages\{69380005-8401-414c-9a8b-dea88e3ffb71}]
"Name"="Conversation Translator"
"ExtensibilityApplicationType"=dword:00000000
"ExtensibilityWindowSize"=dword:00000001
"DefaultContextPackage"=dword:00000000
"InternalURL"="file:///C:/ConversationTranslator/Bin/Debug/ConversationTranslatorTestPage.html"

3. Within a conversation window, open the extension menu 
(double arrows on the right of the conversation buttons bar) and choose Conversation Translator.

Note: The Conversation Translator files may also be hosted on a network share or web site.
In this configuration, the files ConversationTranslatorTestPage.html and ConversationTranslator.xap 
should be placed in the same directory on the server.
The domain or network server needs to be added to the Internet Explorer Trusted Sites zone.

To add a network share server to the trusted site list, add the content below to the registry file 
created in step 2 (replace "myserver" with the appropriate machine name).

[HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains\myserver]
"file"=dword:00000002

To add a website to the trusted site list, add the content below to the registry file 
created in step 2 (replace "contoso.com\www" with the website domain name).

[HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains\contoso.com\www]
"http"=dword:00000002

Please refer to the SDK documentation for more details.
	
