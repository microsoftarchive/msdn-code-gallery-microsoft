Simple Ink C++ Sample
==============================================================================
 Demonstrates how to use inking API surface, most importantly how to render
 live ink and how to render ink strokes off-line using XAML. Also demonstrates
 selecting and erasing ink, and event handlers such as move, recognize, save/load,
 cut/copy/paste, and change drawing attributes.

Files
==============================================================================
 App.xaml
 App.xaml.cs
 Constants.cs
 Files.txt
 Helpers.cs
 MainPage.xaml
 MainPage.xaml.cs
 Package.appxmanifest
 ReadMe.txt
 Scenario1.xaml
 Scenario1.xaml.cs
 SimpleInk.csproj
 SimpleInk.sln
 XamlInkRenderer.cs
 Assets\microsoft-sdk.png
 Assets\placeholder-sdk.png
 Assets\smalltile-sdk.png
 Assets\splash-sdk.png
 Assets\squaretile-sdk.png
 Assets\storelogo-sdk.png
 Assets\tile-sdk.png
 Assets\windows-sdk.png
 Common\LayoutAwarePage.cs
 Common\StandardStyles.xaml
 Properties\AssemblyInfo.cs
 Sample-Utils\SampleTemplateStyles.xaml
 Sample-Utils\SuspensionManager.cs

Usage
==============================================================================
 To ink, use pen or mouse with left button pressed.
 To erase ink strokes, use the back of the pen, or pen with secondary button
  pressed, or mouse with right button pressed.
 To select strokes, draw a lasso around the ink strokes using the pen with
  barrel button pressed or CTRL + mouse with left button pressed.
 To move selected ink use your finger.
 To load/save, cut/copy/paste, change properties, and reco ink, use the
  appropriate UI elements.
 
