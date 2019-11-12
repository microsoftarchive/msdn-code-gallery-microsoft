# How to use OCR in VB in Universal Windows Platform apps
## Requires
- Visual Studio 2017
## License
- Apache License, Version 2.0
## Technologies
- OCR
- Universal Windows App Development
- Universal Windows Platform
- UWP
## Topics
- vb
- OCR
- UWP
## Updated
- 06/21/2017
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="-onecodesampletopbanner1" alt="">
</a></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><a name="OLE_LINK1"></a><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">How</span><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">
</span><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">to</span><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">
</span><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">use OCR in VB</span><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt"> in Universal Windows Platform apps</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>T</span><span>his sample demonstrates how</span><span> to </span><span>use OCR in VB</span><span> in Universal Windows Platform apps</span><span>.</span><span>
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="font-weight:bold; font-size:12pt">Sample prerequisites</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Visual Studio 2017 or above [</span><a href="https://www.visualstudio.com/" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">Visual
 Studio Home Page</span></a><span>]</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Building the sample</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:12pt"><span style="font-size:10pt; font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span style="font-size:10pt">Open the sample solution &ldquo;VB</span><span style="font-size:10pt">UWPOCR</span><span style="font-size:10pt">.sln&rdquo;
 using Visual Studio</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:12pt"><span style="font-size:10pt; font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span style="font-size:10pt">Right click the project &ldquo;VB</span><span style="font-size:10pt">UWPOCR</span><span style="font-size:10pt">&rdquo;
 and select Restore Packages</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:12pt"><span style="font-size:10pt; font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span style="font-size:10pt">Press
</span><span style="font-weight:bold; font-size:10pt">F6</span><span style="font-size:10pt"> Key or select
</span><span style="font-weight:bold; font-size:10pt">Build -&gt; Build Solution</span><span style="font-size:10pt"> from the menu to build the sample.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Running the sample</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:12pt"><span style="font-size:10pt; font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span style="font-size:10pt">Open the sample solution using Visual Studio,
</span><span style="font-size:10pt">select </span><span style="font-weight:bold; font-size:10pt">Local Machine</span><span style="font-size:10pt"> in the tool bar
</span><span style="font-size:10pt">then press F5 Key or select <strong>Debug -&gt; Start Debugging
</strong>from the menu.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="174426-image.png" alt="" width="575" height="407" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Click on the right button below the image.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="174427-image.png" alt="" width="575" height="407" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Click on the middle button below the image to upload user image.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="174428-image.png" alt="" width="575" height="454" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="174429-image.png" alt="" width="575" height="306" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Click on the right button to see the extracted result.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Click on the second menu item on the left, you can capture image by camera to get the result.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="174430-image.png" alt="" width="575" height="379" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="font-weight:bold; font-size:12pt">Using the code</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>OcrFileImage</span><span>:</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>

<div class="preview">
<pre class="vb"><span class="visualBasic__keyword">Private</span>&nbsp;Async&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;ExtractButton_Tapped(sender&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Object</span>,&nbsp;e&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;TappedRoutedEventArgs)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ClearResults()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Check&nbsp;if&nbsp;OcrEngine&nbsp;supports&nbsp;image&nbsp;resoulution.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;(bitmap.PixelWidth&nbsp;&gt;&nbsp;OcrEngine.MaxImageDimension&nbsp;<span class="visualBasic__keyword">Or</span>&nbsp;bitmap.PixelHeight&nbsp;&gt;&nbsp;OcrEngine.MaxImageDimension)&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rootPage.NotifyUser(&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">String</span>.Format(<span class="visualBasic__string">&quot;Bitmap&nbsp;dimensions&nbsp;({0}x{1})&nbsp;are&nbsp;too&nbsp;big&nbsp;for&nbsp;OCR.&quot;</span>,&nbsp;bitmap.PixelWidth,&nbsp;bitmap.PixelHeight)&nbsp;&#43;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__string">&quot;Max&nbsp;image&nbsp;dimension&nbsp;is&nbsp;&quot;</span>&nbsp;&#43;&nbsp;OcrEngine.MaxImageDimension&nbsp;&#43;&nbsp;<span class="visualBasic__string">&quot;.&quot;</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;NotifyType.ErrorMessage)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Return</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;myOcrEngine&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;OcrEngine&nbsp;=&nbsp;<span class="visualBasic__keyword">Nothing</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;(UserLanguageToggle.IsOn)&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Try&nbsp;to&nbsp;create&nbsp;OcrEngine&nbsp;for&nbsp;first&nbsp;supported&nbsp;language&nbsp;from&nbsp;UserProfile.GlobalizationPreferences.Languages&nbsp;list.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;If&nbsp;none&nbsp;of&nbsp;the&nbsp;languages&nbsp;are&nbsp;available&nbsp;on&nbsp;device,&nbsp;method&nbsp;returns&nbsp;null.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;myOcrEngine&nbsp;=&nbsp;OcrEngine.TryCreateFromUserProfileLanguages()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Try&nbsp;to&nbsp;create&nbsp;OcrEngine&nbsp;for&nbsp;specified&nbsp;language.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;If&nbsp;language&nbsp;Is&nbsp;Not&nbsp;supported&nbsp;on&nbsp;device,&nbsp;method&nbsp;returns&nbsp;null.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;myOcrEngine&nbsp;=&nbsp;OcrEngine.TryCreateFromLanguage(LanguageList.SelectedValue)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;(myOcrEngine&nbsp;<span class="visualBasic__keyword">IsNot</span>&nbsp;<span class="visualBasic__keyword">Nothing</span>)&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Recognize&nbsp;text&nbsp;from&nbsp;image.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;myOcrResult&nbsp;=&nbsp;Await&nbsp;myOcrEngine.RecognizeAsync(bitmap)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Display&nbsp;recognized&nbsp;text.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ExtractedTextBox.Text&nbsp;=&nbsp;myOcrResult.Text&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;(myOcrResult.TextAngle&nbsp;<span class="visualBasic__keyword">IsNot</span>&nbsp;<span class="visualBasic__keyword">Nothing</span>)&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;If&nbsp;text&nbsp;Is&nbsp;detected&nbsp;under&nbsp;some&nbsp;angle&nbsp;in&nbsp;this&nbsp;sample&nbsp;scenario&nbsp;we&nbsp;want&nbsp;to</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;overlay&nbsp;word&nbsp;boxes&nbsp;over&nbsp;original&nbsp;image,&nbsp;so&nbsp;we&nbsp;rotate&nbsp;overlay&nbsp;boxes.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;transform&nbsp;=&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;RotateTransform()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;transform.Angle&nbsp;=&nbsp;myOcrResult.TextAngle&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;transform.CenterX&nbsp;=&nbsp;PreviewImage.ActualWidth&nbsp;/&nbsp;<span class="visualBasic__number">2</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;transform.CenterY&nbsp;=&nbsp;PreviewImage.ActualHeight&nbsp;/&nbsp;<span class="visualBasic__number">2</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;TextOverlay.RenderTransform&nbsp;=&nbsp;transform&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Create&nbsp;overlay&nbsp;boxes&nbsp;over&nbsp;recognized&nbsp;words.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;<span class="visualBasic__keyword">Each</span>&nbsp;line&nbsp;<span class="visualBasic__keyword">In</span>&nbsp;myOcrResult.Lines&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;lineRect&nbsp;=&nbsp;Rect.Empty&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;<span class="visualBasic__keyword">Each</span>&nbsp;word&nbsp;<span class="visualBasic__keyword">In</span>&nbsp;line.Words&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;lineRect.Union(word.BoundingRect)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Determine&nbsp;if&nbsp;line&nbsp;Is&nbsp;horizontal&nbsp;Or&nbsp;vertical.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Vertical&nbsp;lines&nbsp;are&nbsp;supported&nbsp;only&nbsp;in&nbsp;Chinese&nbsp;Traditional&nbsp;And&nbsp;Japanese&nbsp;languages.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;isVerticalLine&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Boolean</span>&nbsp;=&nbsp;lineRect.Height&nbsp;&gt;&nbsp;lineRect.Width&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">For</span>&nbsp;<span class="visualBasic__keyword">Each</span>&nbsp;word&nbsp;<span class="visualBasic__keyword">In</span>&nbsp;line.Words&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;wordBoxOverlay&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;WordOverlay(word)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Keep&nbsp;references&nbsp;to&nbsp;word&nbsp;boxes.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;wordBoxes.Add(wordBoxOverlay)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Define&nbsp;overlay&nbsp;style.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;overlay&nbsp;=&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;Border()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;(isVerticalLine)&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;overlay.Style&nbsp;=&nbsp;Resources.Item(<span class="visualBasic__string">&quot;HighlightedWordBoxVerticalLine&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;overlay.Style&nbsp;=&nbsp;Resources.Item(<span class="visualBasic__string">&quot;HighlightedWordBoxHorizontalLine&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Bind&nbsp;word&nbsp;boxes&nbsp;to&nbsp;UI.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;overlay.SetBinding(Border.MarginProperty,&nbsp;wordBoxOverlay.CreateWordPositionBinding())&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;overlay.SetBinding(Border.WidthProperty,&nbsp;wordBoxOverlay.CreateWordWidthBinding())&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;overlay.SetBinding(Border.HeightProperty,&nbsp;wordBoxOverlay.CreateWordHeightBinding())&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Put&nbsp;the&nbsp;filled&nbsp;textblock&nbsp;in&nbsp;the&nbsp;results&nbsp;grid.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;TextOverlay.Children.Add(overlay)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Next</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Rescale&nbsp;word&nbsp;boxes&nbsp;to&nbsp;match&nbsp;current&nbsp;UI&nbsp;size.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UpdateWordBoxTransform()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rootPage.NotifyUser(&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__string">&quot;Image&nbsp;is&nbsp;OCRed&nbsp;for&nbsp;&quot;</span>&nbsp;&#43;&nbsp;myOcrEngine.RecognizerLanguage.DisplayName&nbsp;&#43;&nbsp;<span class="visualBasic__string">&quot;&nbsp;language.&quot;</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;NotifyType.StatusMessage)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rootPage.NotifyUser(<span class="visualBasic__string">&quot;Selected&nbsp;language&nbsp;is&nbsp;not&nbsp;available.&quot;</span>,&nbsp;NotifyType.ErrorMessage)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;</pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="font-weight:bold; font-size:12pt">More</span><span style="font-weight:bold; font-size:12pt">
</span><span style="font-weight:bold; font-size:12pt">information</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><a href="https://docs.microsoft.com/en-us/uwp/api/windows.media.ocr.ocrengine" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">OcrEngine Class</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><a href="https://github.com/Microsoft/Windows-universal-samples/tree/master/Samples/OCR" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">C&#43;&#43;, C#, JS version sample</span></a><a name="_GoBack"></a></span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
