# How to crop bitmap in a Windows Store app (C#)
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Windows 8
## Topics
- Crop Bitmap
## Updated
- 04/16/2013
## Description

<h1><span><a href="http://www.microsoft.com/click/services/Redirect2.ashx?CR_CC=200144425" target="_blank"><img id="79968" src="http://i1.code.msdn.s-msft.com/cswindowsstoreappadditem-a5d7fbcc/image/file/79968/1/dpe_w8_728x90_1022_v2.jpg" alt="" width="728" height="90" style="width:100%"></a></span></h1>
<h1><span>Crop bitmap in </span><span>a Windows Store</span><span> app (CSWindowsStoreAppCropBitmap)
</span></h1>
<h2><span>Introduction </span></h2>
<p class="MsoNormal">This sample demonstrates how to crop bitmap in <span>Windows Store</span><span>
</span>App.</p>
<p class="MsoNormal">To crop a bitmap in a <span>Windows Store</span><span> </span>
app, we can use a <a href="http://msdn.microsoft.com/en-us/library/windows/apps/windows.graphics.imaging.bitmaptransform">
<span class="SpellE">BitmapTransform</span></a> class when reading in the bitmap in a
<a href="http://msdn.microsoft.com/en-us/library/windows/apps/windows.graphics.imaging.bitmapdecoder">
BitmapDecoder</a>.</p>
<h2>Demo</h2>
<p><object width="540" height="330" data="data:application/x-silverlight-2," type="application/x-silverlight-2"> <param name="source" value="/scriptcenter/Content/Common/videoplayer.xap" /> <param name="initParams" value="deferredLoad=false,duration=0,m=http://media.ch9.ms/ch9/836e/5a36836c-f87f-41a0-b53e-ad242f2b836e/CropImageWinStoreApp_Source.wmv,autostart=true,autohide=true,showembed=true"
 /> <param name="background" value="#00FFFFFF" /> <param name="minRuntimeVersion" value="3.0.40624.0" /> <param name="enableHtmlAccess" value="true" /> <param name="src" value="http://media.ch9.ms/ch9/836e/5a36836c-f87f-41a0-b53e-ad242f2b836e/CropImageWinStoreApp_Source.wmv"
 /> <param name="id" value="68000" /> <param name="name" value="CropImageWinStoreApp_Source.wmv" /><span><a href="http://go.microsoft.com/fwlink/?LinkID=149156" style="text-decoration:none"><img src="http://go.microsoft.com/fwlink/?LinkId=108181" alt="Get Microsoft Silverlight" style="border-style:none"></a></span>
 </object> </p>
<h2><span>Running the Sample </span></h2>
<p class="MsoListParagraph">1.<span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Open this sample in VS2012 on Win8 machine, and press F5 to run it.</p>
<p class="MsoListParagraph">2.<span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>After the app is launched, you will see following screen.</p>
<p class="MsoListParagraph"><img src="73763-image.png" alt="" width="595" height="342" align="middle">&nbsp;</p>
<p class="MsoListParagraph">3.<span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Press the &quot;Open an Image&quot; button, and then choose an image file (*.jpg, *.png or *.bmp).&nbsp;</p>
<p class="MsoListParagraph"><img id="75615" src="http://i1.code.msdn.s-msft.com/cswin8appcropbitmap-52fa1ad7/image/file/75615/1/temppic.png" alt="" width="605" height="348"></p>
<p class="MsoListParagraph">4.<span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Select the region to crop.</p>
<p class="MsoListParagraph">5.<span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Click the &quot;Save Cropped Image&quot; button, and select the destination path, you will find the cropped image file in local disk.</p>
<p class="MsoListParagraph"><span>&nbsp;</span></p>
<h2><span>Using the Code </span></h2>
<p class="MsoNormal">1.<span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Crop bitmap by using a <a href="http://msdn.microsoft.com/en-us/library/windows/apps/windows.graphics.imaging.bitmaptransform">
BitmapTransform</a> class when reading in the bitmap in a <a href="http://msdn.microsoft.com/en-us/library/windows/apps/windows.graphics.imaging.bitmapdecoder">
BitmapDecoder</a> .</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">/// &lt;summary&gt;
/// Get a cropped bitmap from a image file.
/// &lt;/summary&gt;
/// &lt;param name=&quot;originalImgFile&quot;&gt;
/// The original image file.
/// &lt;/param&gt;
/// &lt;param name=&quot;startPoint&quot;&gt;
/// The start point of the region to be cropped.
/// &lt;/param&gt;
/// &lt;param name=&quot;corpSize&quot;&gt;
/// The size of the region to be cropped.
/// &lt;/param&gt;
/// &lt;returns&gt;
/// The cropped image.
/// &lt;/returns&gt;
async public static Task&lt;ImageSource&gt; GetCroppedBitmapAsync(StorageFile originalImgFile,
&nbsp;&nbsp;&nbsp; Point startPoint, Size corpSize, double scale)
{
&nbsp;&nbsp;&nbsp; if (double.IsNaN(scale) || double.IsInfinity(scale))
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; scale = 1;
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; // Convert start point and size to integer.
&nbsp;&nbsp;&nbsp; uint startPointX = (uint)Math.Floor(startPoint.X * scale);
&nbsp;&nbsp;&nbsp; uint startPointY = (uint)Math.Floor(startPoint.Y * scale);
&nbsp;&nbsp;&nbsp; uint height = (uint)Math.Floor(corpSize.Height * scale);
&nbsp;&nbsp;&nbsp; uint width = (uint)Math.Floor(corpSize.Width * scale);


&nbsp;&nbsp;&nbsp; using (IRandomAccessStream stream = await originalImgFile.OpenReadAsync())
&nbsp;&nbsp;&nbsp; {


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Create a decoder from the stream. With the decoder, we can get 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// the properties of the image.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
&nbsp; 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// The scaledSize of original image.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; uint scaledWidth = (uint)Math.Floor(decoder.PixelWidth * scale);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; uint scaledHeight = (uint)Math.Floor(decoder.PixelHeight * scale);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Refine the start point and the size. 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;if (startPointX &#43; width &gt; scaledWidth)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; startPointX = scaledWidth - width;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (startPointY &#43; height &gt; scaledHeight)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; startPointY = scaledHeight - height;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Create cropping BitmapTransform and define the bounds.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; BitmapTransform transform = new BitmapTransform();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; BitmapBounds bounds = new BitmapBounds();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; bounds.X = startPointX;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; bounds.Y = startPointY;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; bounds.Height = height;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; bounds.Width = width;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; transform.Bounds = bounds;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; transform.ScaledWidth = scaledWidth;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; transform.ScaledHeight = scaledHeight;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// Get the cropped pixels within the bounds of transform.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; PixelDataProvider pix = await decoder.GetPixelDataAsync(
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; BitmapPixelFormat.Bgra8,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; BitmapAlphaMode.Straight,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; transform,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ExifOrientationMode.IgnoreExifOrientation,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ColorManagementMode.ColorManageToSRgb);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; byte[] pixels = pix.DetachPixelData();


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Stream the bytes into a WriteableBitmap
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; WriteableBitmap cropBmp = new WriteableBitmap((int)width, (int)height);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Stream pixStream = cropBmp.PixelBuffer.AsStream();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; pixStream.Write(pixels, 0, (int)(width * height * 4));


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; return cropBmp;
&nbsp;&nbsp;&nbsp; }


}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;&nbsp;</div>
<p class="MsoNormal">2.<span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Save bitmap by using a <strong><a href="http://msdn.microsoft.com/en-us/library/windows/apps/windows.graphics.imaging.bitmapencoder">BitmapEncoder</a></strong>.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">/// &lt;summary&gt;
/// Save the cropped bitmap to a image file.
/// &lt;/summary&gt;
/// &lt;param name=&quot;originalImgFile&quot;&gt;
/// The original image file.
/// &lt;/param&gt;
/// &lt;param name=&quot;newImgFile&quot;&gt;
/// The target file.
/// &lt;/param&gt;
/// &lt;param name=&quot;startPoint&quot;&gt;
/// The start point of the region to be cropped.
/// &lt;/param&gt;
/// &lt;param name=&quot;size&quot;&gt;
/// The size of the region to be cropped.
/// &lt;/param&gt;
/// &lt;returns&gt;
/// Whether the operation is successful.
/// &lt;/returns&gt;
async public static Task SaveCroppedBitmapAsync(StorageFile originalImgFile, StorageFile newImgFile,
&nbsp;&nbsp;&nbsp; Point startPoint, Size size)
{


&nbsp;&nbsp;&nbsp; // Convert start point and size to integer.
&nbsp;&nbsp;&nbsp; uint startPointX = (uint)Math.Floor(startPoint.X);
&nbsp;&nbsp;&nbsp; uint startPointY = (uint)Math.Floor(startPoint.Y);
&nbsp;&nbsp;&nbsp; uint height = (uint)Math.Floor(size.Height);
&nbsp;&nbsp;&nbsp; uint width = (uint)Math.Floor(size.Width);


&nbsp;&nbsp;&nbsp; using (IRandomAccessStream originalImgFileStream = await originalImgFile.OpenReadAsync())
&nbsp;&nbsp;&nbsp; {




&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Create a decoder from the stream. With the decoder, we can get 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// the properties of the image.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; BitmapDecoder decoder = await BitmapDecoder.CreateAsync(originalImgFileStream);


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Refine the start point and the size. 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;if (startPointX &#43; width &gt; decoder.PixelWidth)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; startPointX = decoder.PixelWidth - width;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (startPointY &#43; height &gt; decoder.PixelHeight)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;startPointY = decoder.PixelHeight - height;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Create cropping BitmapTransform to define the bounds.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; BitmapTransform transform = new BitmapTransform();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; BitmapBounds bounds = new BitmapBounds();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; bounds.X = startPointX;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; bounds.Y = startPointY;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; bounds.Height = height;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; bounds.Width = width;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; transform.Bounds = bounds;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Get the cropped pixels within the the bounds of transform.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; PixelDataProvider pix = await decoder.GetPixelDataAsync(
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; BitmapPixelFormat.Bgra8,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; BitmapAlphaMode.Straight,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; transform,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ExifOrientationMode.IgnoreExifOrientation,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ColorManagementMode.ColorManageToSRgb);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; byte[] pixels = pix.DetachPixelData();


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; using (IRandomAccessStream newImgFileStream = await newImgFile.OpenAsync(FileAccessMode.ReadWrite))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Guid encoderID = Guid.Empty;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; switch (newImgFile.FileType.ToLower())
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;case &quot;.png&quot;:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; encoderID = BitmapEncoder.PngEncoderId;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; break;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; case &quot;.bmp&quot;:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; encoderID = BitmapEncoder.BmpEncoderId;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; break;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; default:
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;encoderID = BitmapEncoder.JpegEncoderId;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; break;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Create a bitmap encoder


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; BitmapEncoder bmpEncoder = await BitmapEncoder.CreateAsync(
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; encoderID,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; newImgFileStream);


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Set the pixel data to the cropped image.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; bmpEncoder.SetPixelData(
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; BitmapPixelFormat.Bgra8,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; BitmapAlphaMode.Straight,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; width,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; height,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; decoder.DpiX,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; decoder.DpiY,
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; pixels);


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Flush the data to file.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; await bmpEncoder.FlushAsync();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; }


}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">&nbsp;</p>
<h2><span>More Information </span></h2>
<p class="MsoListParagraph"><span style="font-family:Symbol">&bull;</span><span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Windows 8 app samples<br>
<a href="http://code.msdn.microsoft.com/Windows-8-Modern-Style-App-Samples">http://code.msdn.microsoft.com/Windows-8-Modern-Style-App-Samples</a></p>
<p class="MsoListParagraph"><span style="font-family:Symbol">&bull;</span><span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>BitmapTransform <br>
<a href="http://msdn.microsoft.com/en-us/library/windows/apps/windows.graphics.imaging.bitmaptransform">http://msdn.microsoft.com/en-us/library/windows/apps/windows.graphics.imaging.bitmaptransform</a></p>
<p class="MsoListParagraph"><span style="font-family:Symbol">&bull;</span><span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>BitmapDecoder<br>
<a href="http://msdn.microsoft.com/en-us/library/windows/apps/windows.graphics.imaging.bitmapdecoder">http://msdn.microsoft.com/en-us/library/windows/apps/windows.graphics.imaging.bitmapdecoder</a></p>
<p class="MsoListParagraph"><span style="font-family:Symbol">&bull;</span><span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>BitmapEncoder<br>
<a href="http://msdn.microsoft.com/en-us/library/windows/apps/windows.graphics.imaging.bitmapencoder">http://msdn.microsoft.com/en-us/library/windows/apps/windows.graphics.imaging.bitmapencoder</a></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
