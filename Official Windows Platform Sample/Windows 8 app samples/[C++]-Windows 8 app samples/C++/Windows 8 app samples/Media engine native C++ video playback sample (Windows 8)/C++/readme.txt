Overview
--------

This sample contains native C++ implementation of Media Engine playback App using MSDK.

- The App allows user to browse and load media file from the local folders.
- It plays back the media content full screen using Media Engine.
- It does not support seeking.

This sample requires Windows 8 or later.
  
  
To use this sample:
-------------------

1. Build the sample.
2. Create AppX package and deploy.
3. Launch the App. 
4. Browse to the desired media content location, select and open the file for playback.
5. To load another file for playback use either right mouse click or press 'o' key.
6. To Pause/Play use middle mouse click or press 'space' key.

 
Classes:
--------

MediaEngineNotify: Implements the callback for Media Engine event notification.

MEPlayer: Main class that implements file picker and does the Media Engine setup using ICoreWindow, execution and cleanup for Playback.

VideoView: Class that activates the playback window and manages Window events.

FrameworkView: Class that implements Windows::ApplicationModel::Core::IFrameworkView.


THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.

Copyright (c) Microsoft Corporation. All rights reserved.