Simple Ink C++ Sample
==============================================================================
 Demonstrates how to use inking API surface, most importantly how to render
 live ink and how to render ink strokes off-line using D2D. Also demonstrates
 selecting and erasing ink, and event handlers such as copy/paste, save/load,
 select all, delete, change drawing attributes, change recognizer, recognize.
 On start, the application shows white screen. The user can then ink using
 using pen or mouse (ink color is changing with every stroke).

Files
==============================================================================
 DirectXBase.cpp
 DirectXBase.h
 DirectXSample.h
 Package.appxmanifest
 pch.cpp
 pch.h
 ReadMe.txt
 simpleInk.cpp
 simpleInk.h
 simpleInk.sln
 simpleInk.vcxproj
 images\microsoft-sdk.png
 images\placeholder-sdk.png
 images\smallTile-sdk.png
 images\splash-sdk.png
 images\squareTile-sdk.png
 images\storeLogo-sdk.png
 images\tile-sdk.png
 images\windows-sdk.png

To build the sample using the command prompt
==============================================================================
 1. Copy sample directory outside Program Files folder.
 2. Open the Command Prompt window and navigate to the copied sample directory.
 3. Type msbuild simpleInk.sln. The application will be built in the default
    \Debug or \Release directory.

To build and start the sample using Visual Studio
==============================================================================
 1. Copy sample directory outside Program Files folder.
 2. Open File Explorer and navigate to the copied sample directory.
 3. Double-click the icon for the simpleInk.sln file to open the file in
     Visual Studio.
 4. In the Build menu, select Build Solution.
 5. In the Debug menu, select Start Debugging (or Start Without Debugging)

Usage
==============================================================================
 To ink, use pen or mouse with left button pressed.
 To erase ink strokes, use the back of the pen, or pen with secondary button
  pressed, or mouse with right button pressed.
 To select strokes, draw a lasso around the ink strokes using the pen with
  barrel button pressed or CTRL + mouse with left button pressed.
 Backspace --> Delete ink
 Space     --> Recognize ink
 Ctrl+a    --> Select all
 Ctrl+c    --> Copy to clipboard
 Ctrl+d    --> Change drawing attributes
 Ctrl+o    --> Load ink from file
 Ctrl+r    --> Change recognizer
 Ctrl+s    --> Save ink to file
 Ctrl+t    --> Copy text to clipboard (press Space to recognize ink before)
 Ctrl+v    --> Paste from clipboard
