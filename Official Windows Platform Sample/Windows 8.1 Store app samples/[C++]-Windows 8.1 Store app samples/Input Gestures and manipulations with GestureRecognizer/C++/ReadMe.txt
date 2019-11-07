Gesture Recognizer Sample
=========================
Demonstrates how to use GestureRecognizer to recognize touch, pen and mouse gestures in a CoreWindow and D2D based Windows application.
GestureRecognizer enables recognition of gestures, manipulations (pan, zoom, rotation) and provides inertia calculations for realistic modeling of touch, pen and mouse interactions.
The sample doesn't rely on any UI framework, but instead it uses ApplicationObject to obtain a CoreWindow. CoreWindow provides pointer events for the application, which are passed to GestureRecognizer as input. The application draws using D2D.

On start, the application shows four square shapes in different colors. The user can manipulate objects - move them around, scale, rotate. Tap on the object will change its color. Right tap on the object will restore the original transform. Hold on the object will display object's current transform. Background can be manipulated as well - moved, scaled and rotated. Hold on the background will display currently used device information. Right tap on the background will restore its original transform. Gestures and manipulations can be performed using touch, pen or mouse.

Sample Language Implementations
===============================
C++

Files
=====
GestureRecognizerSample.sln
GestureRecognizerSample.vcxproj
Package.appxmanifest
BackgroundObject.cpp
DrawingObject.cpp
GestureRecognizerSample.cpp
ManipulatableObject.cpp
DirectXBase.cpp
pch.cpp
BackgroundObject.h
DrawingObject.h
GestureRecognizerSample.h
ManipulatableObject.h
DirectXBase.h
DirectXSample.h
pch.h
ReadMe.txt
images\microsoft-sdk.png
images\placeholder-sdk.png
images\smallTile-sdk.png
images\splash-sdk.png
images\squareTile-sdk.png
images\storeLogo-sdk.png
images\tile-sdk.png
images\windows-sdk.png

To build the sample using the command prompt
============================================
     1. Copy sample directory outside Program Files folder.
     2. Open the Command Prompt window and navigate to the copied sample directory.
     3. Type msbuild GestureRecognizerSample.sln. The application will be built in the default \Debug or \Release directory.

To build the sample using Visual Studio
=======================================
     1. Copy sample directory outside Program Files folder.
     2. Open File Explorer and navigate to the copied sample directory.
     3. Double-click the icon for the GestureRecognizerSample.sln file to open the file in Visual Studio.
     4. In the Build menu, select Build Solution. The application will be built in the default \Debug or \Release directory.

To run the sample
=================
     1. Navigate to the GestureRecognizerSample tile on the Start screen.
     2. Tap on the tile to start the application.
