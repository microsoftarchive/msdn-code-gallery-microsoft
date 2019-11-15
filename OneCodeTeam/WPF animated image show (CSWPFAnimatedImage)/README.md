# WPF animated image show (CSWPFAnimatedImage)
## Requires
- Visual Studio 2008
## License
- MS-LPL
## Technologies
- WPF
## Topics
- Animation
## Updated
- 03/01/2012
## Description

<h1><span style="">WPF animated image show (<span class="SpellE">CSWPFAnimatedImage</span>)
</span></h1>
<h2>Introduction</h2>
<p class="MsoNormal"><br>
The sample demonstrates how to display a series of photos just like a digital<span style="">
</span>picture frame with a &quot;Wipe&quot; effect.<span style=""> </span></p>
<h2>Running the Sample<span style=""> </span></h2>
<p class="MsoNormal"><span style="">Press F5 to run this application, you will see that the window displays
</span>a series of photos just like a digital<span style=""> </span>picture frame with a &quot;Wipe&quot; effect.<span style="">
</span></p>
<p class="MsoNormal"><span style=""><img src="53231-image.png" alt="" width="300" height="300" align="middle">
<span style="">&nbsp;</span> <img src="53232-image.png" alt="" width="300" height="300" align="middle">
</span><span style=""></span></p>
<h2>Using the Code<span style=""> </span></h2>
<p class="MsoListParagraphCxSpFirst" style=""><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Add two Image controls named myImage1 and myImage2 on a Window. The myImage1</span><span style="">
</span><span style="">lies on top of the myImage2. </span></p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Set the OpacityMask of the myImage1 to a LinearGradientBrush. Add two GradientStop in the LinearGradientBrush.
</span></p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Add two Storyboards in the resource dictionary of the Window. One storyboard is named VisibleToInvisible. It animates the two GradientStop above to hide themyImage1. The other storyboard is named InvisibleToVisible. It animates
 thetwo GradientStop to show the myImage1. </span></p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">4.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Create a collection of type List&lt;BitmapImage&gt; and add images to be shown in the collection when the Window is loaded.
</span></p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">5.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Subscribe the Completed event of the two storyboards.
</span></p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">6.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">In the Completed event handler of the VisibleToInvisible storyboard, changethe Source of the myImage1 to the next image to be shown. Get the InvisibleToVisible storyboard from the resource dictionary and start it.
</span></p>
<p class="MsoListParagraphCxSpLast" style=""><span style=""><span style="">7.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">In the Completed event handler of the InvisibleToVisible storyboard, changethe Source of the myImage2 to the next image to be shown. Get the</span><span style="">
</span><span style="">VisibleToInVisible storyboard from the resource dictionary and start it.</span><span style="">
</span></p>
<h2>More Information<span style=""> </span></h2>
<p class="MsoNormal"><span style=""><a href="http://msdn.microsoft.com/en-us/library/system.windows.media.animation.storyboard.aspx">Storyboard Class</a>
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
