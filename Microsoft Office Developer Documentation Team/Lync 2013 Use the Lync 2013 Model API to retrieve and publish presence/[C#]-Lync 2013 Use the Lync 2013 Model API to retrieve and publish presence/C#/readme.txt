Copyright (c) Microsoft Corporation.  All rights reserved.

Presence Publication Sample

This sample uses the Lync Model API to retrieve and publish information of the Self contact (currently signed in user).
It also shows how to sign in to Lync using the credentials of the active user..

Sample location:
64Bit Operating System: %PROGRAMFILES(X86)%\Microsoft Lync\SDK\Samples\PresencePublication
32Bit Operating System: %PROGRAMFILES%\Microsoft Lync\SDK\Samples\ContactInformation

Features
- Get information of a Lync contact such as the name, photo, current availability and personal note.
- Publish information of the Self contact, such as availability and personal note.
- Sign in to Lync using the credentials of the user currently logged in to the machine.
- Sign out from Lync.
- Handle Lync events to respond to changes in the client state and changes in the contact information.

Warnings:
- The sample runs in Visual Studio 2008 but the project file included is for Visual Studio 2010.

Prerequisites (for compiling and running in Visual Studio)
- .Net Framework 4.0 or above.
- Visual Studio 2010 or above.
- Microsoft Lync SDK

Prerequisites (for running installed sample on client machines)
- Microsoft Lync must be installed and running.

Running the sample
------------------
1. Open PresencePublication.csproj
2. Hit F5 in Visual Studio