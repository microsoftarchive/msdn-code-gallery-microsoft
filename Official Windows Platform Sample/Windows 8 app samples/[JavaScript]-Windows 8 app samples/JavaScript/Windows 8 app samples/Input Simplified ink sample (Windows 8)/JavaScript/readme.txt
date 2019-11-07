Simple Ink JS Sample
==============================================================================
 Demonstrates how to use inking API surface, most importantly how to render
 live ink and how to render ink strokes off-line. Also demonstrates selecting
 and erasing ink, and event handlers such as copy/paste, save/load, select
 all, delete, change drawing attributes, change recognizer, recognize.
 On start, the application shows white screen. The user can then ink using
 using pen or mouse (ink color is changing with every stroke).

Files
==============================================================================
 default.html
 Package.appxmanifest
 simpleInk.js
 simpleInk.jsproj
 simpleInk.sln
 css\simpleInk.css 
 images\microsoft-sdk.png
 images\placeholder-sdk.png
 images\smallTile-sdk.png
 images\splash-sdk.png
 images\squareTile-sdk.png
 images\storeLogo-sdk.png
 images\tile-sdk.png
 images\windows-sdk.png

To start the sample using Visual Studio
==============================================================================
 1. Copy sample directory outside Program Files folder.
 2. Open File Explorer and navigate to the copied sample directory.
 3. Double-click the icon for the simpleInk.sln file to open the file in
     Visual Studio.
 4. In the Debug menu, select Start Debugging (or Start Without Debugging)

Usage
==============================================================================
 Use pen and no buttons, or mouse with left button pressed to ink
 Use eraser, or pen with secondary button pressed, or mouse with right button
  pressed to erase ink (by scribbling across the ink you want to erase)
 Use pen with barrel button pressed, or Ctrl + mouse with left button pressed
  to select ink (by drawing a contour around the ink you want to select)
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
