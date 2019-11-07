# How to crop image using mouse selection in PictureBox in Windows form
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- .NET
- Windows Forms
- Windows
- Windows Desktop App Development
## Topics
- Image
## Updated
- 04/06/2016
## Description

<p>&nbsp;</p>
<h1>Crop the image from Windows Forms PictureBox control (<span class="SpellE">CSWinformCropImage</span>) by Visual Studio 2010</h1>
<h2>Requirement</h2>
<p>Crop an image using Windows Forms PictureBox control</p>
<h2>Technology</h2>
<p>Windows Forms Application, VB.NET, Visual Studio 2010.<br>
The sample demonstrates how to crop the image from a specific Picturebox control into another Picturebox control using mouse selection or specified coordinates.This code shows how to crop the image from Windown Forms PictureBox control (CSWinformCropImage)
 by Visual Studio 2010</p>
<h2>To Run the sample</h2>
<div class="endscriptcode"></div>
<p>Open the project - CropPicture.sln in Visual Studio 2010Run the application (CTRL&#43;F5).Select the area to crop using mouse. View the coordinates selected.Click on &quot;Crop&quot; button.<br>
Alternately, you can select the check box - &quot;Use Coordinates&quot;Enter the coordinates for X1,Y1,X2 and Y2Click on &quot;Crop&quot; buttonThe area will be highlighted using dashes in the original picture and that area will be cropped as well.</p>
<h2>Code Used</h2>
<p>PictureBox1 is the source image box.</p>
<p>PictureBox2 is the destination image box.</p>
<p>CheckBox1 is the &quot;Use Coordinates checkbox&quot;</p>
<h2>For Mouse Up event</h2>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>
<pre class="hidden">    Private Sub PicBox_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseUp
        mouseClicked = False
        If (endPoint.X &lt;&gt; -1) Then
            Dim currentPoint As New Point(e.X, e.Y)
            Y1.Text = e.X.ToString()
            Y2.Text = e.Y.ToString()
        End If

        endPoint.X = -1
        endPoint.Y = -1
        startPoint.X = -1
        startPoint.Y = -1
    End Sub</pre>
<div class="preview">
<pre class="vbs">&nbsp;&nbsp;&nbsp;&nbsp;<span class="vbScript__keyword">Private</span>&nbsp;<span class="vbScript__keyword">Sub</span>&nbsp;PicBox_MouseUp(<span class="vbScript__keyword">ByVal</span>&nbsp;sender&nbsp;<span class="vbScript__keyword">As</span>&nbsp;System.<span class="vbScript__keyword">Object</span>,&nbsp;<span class="vbScript__keyword">ByVal</span>&nbsp;e&nbsp;<span class="vbScript__keyword">As</span>&nbsp;System.Windows.Forms.MouseEventArgs)&nbsp;<span class="vbScript__keyword">Handles</span>&nbsp;PictureBox1.MouseUp&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;mouseClicked&nbsp;=&nbsp;<span class="vbScript__keyword">False</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="vbScript__keyword">If</span>&nbsp;(endPoint.X&nbsp;&lt;&gt;&nbsp;-<span class="vbScript__number">1</span>)&nbsp;<span class="vbScript__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="vbScript__keyword">Dim</span>&nbsp;currentPoint&nbsp;<span class="vbScript__keyword">As</span>&nbsp;<span class="vbScript__keyword">New</span>&nbsp;Point(e.X,&nbsp;e.Y)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Y1.Text&nbsp;=&nbsp;e.X.ToString()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Y2.Text&nbsp;=&nbsp;e.Y.ToString()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="vbScript__keyword">End</span>&nbsp;<span class="vbScript__keyword">If</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;endPoint.X&nbsp;=&nbsp;-<span class="vbScript__number">1</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;endPoint.Y&nbsp;=&nbsp;-<span class="vbScript__number">1</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;startPoint.X&nbsp;=&nbsp;-<span class="vbScript__number">1</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;startPoint.Y&nbsp;=&nbsp;-<span class="vbScript__number">1</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="vbScript__keyword">End</span>&nbsp;<span class="vbScript__keyword">Sub</span></pre>
</div>
</div>
</div>
<h2>For mouse down event</h2>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>
<pre class="hidden">    Private Sub PicBox_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseDown
        mouseClicked = True
        startPoint.X = e.X
        startPoint.Y = e.Y
        'Display coordinates
        X1.Text = startPoint.X.ToString()
        Y1.Text = startPoint.Y.ToString()

        endPoint.X = -1
        endPoint.Y = -1

        rectCropArea = New Rectangle(New Point(e.X, e.Y), New Size())
    End Sub</pre>
<div class="preview">
<pre class="vb">&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Private</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;PicBox_MouseDown(<span class="visualBasic__keyword">ByVal</span>&nbsp;sender&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;System.<span class="visualBasic__keyword">Object</span>,&nbsp;<span class="visualBasic__keyword">ByVal</span>&nbsp;e&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;System.Windows.Forms.MouseEventArgs)&nbsp;<span class="visualBasic__keyword">Handles</span>&nbsp;PictureBox1.MouseDown&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;mouseClicked&nbsp;=&nbsp;<span class="visualBasic__keyword">True</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;startPoint.X&nbsp;=&nbsp;e.X&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;startPoint.Y&nbsp;=&nbsp;e.Y&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'Display&nbsp;coordinates</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;X1.Text&nbsp;=&nbsp;startPoint.X.ToString()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Y1.Text&nbsp;=&nbsp;startPoint.Y.ToString()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;endPoint.X&nbsp;=&nbsp;-<span class="visualBasic__number">1</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;endPoint.Y&nbsp;=&nbsp;-<span class="visualBasic__number">1</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea&nbsp;=&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;Rectangle(<span class="visualBasic__keyword">New</span>&nbsp;Point(e.X,&nbsp;e.Y),&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;Size())&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span></pre>
</div>
</div>
</div>
<h2>For Mouse move event</h2>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>
<pre class="hidden">Private Sub PicBox_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseMove
        Dim ptCurrent As New Point(e.X, e.Y)
        If (mouseClicked) Then
            If (endPoint.X &lt;&gt; -1) Then
                'Display Coordinates
                X1.Text = startPoint.X.ToString()
                Y1.Text = startPoint.Y.ToString()
                X2.Text = e.X.ToString()
                Y2.Text = e.Y.ToString()
            End If
            endPoint = ptCurrent
            If (e.X &gt; startPoint.X And e.Y &gt; startPoint.Y) Then
                rectCropArea.Width = e.X - startPoint.X
                rectCropArea.Height = e.Y - startPoint.Y


            ElseIf (e.X &lt; startPoint.X And e.Y &gt; startPoint.Y) Then
                rectCropArea.Width = startPoint.X - e.X
                rectCropArea.Height = e.Y - startPoint.Y
                rectCropArea.X = e.X
                rectCropArea.Y = startPoint.Y

            ElseIf (e.X &gt; startPoint.X And e.Y &lt; startPoint.Y) Then
                rectCropArea.Width = e.X - startPoint.X
                rectCropArea.Height = startPoint.Y - e.Y
                rectCropArea.X = startPoint.X
                rectCropArea.Y = e.Y

            Else
                rectCropArea.Width = startPoint.X - e.X
                rectCropArea.Height = startPoint.Y - e.Y
                rectCropArea.X = e.X
                rectCropArea.Y = e.Y
            End If
            PictureBox1.Refresh()
        End If
    End Sub</pre>
<div class="preview">
<pre class="vb"><span class="visualBasic__keyword">Private</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;PicBox_MouseMove(<span class="visualBasic__keyword">ByVal</span>&nbsp;sender&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;System.<span class="visualBasic__keyword">Object</span>,&nbsp;<span class="visualBasic__keyword">ByVal</span>&nbsp;e&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;System.Windows.Forms.MouseEventArgs)&nbsp;<span class="visualBasic__keyword">Handles</span>&nbsp;PictureBox1.MouseMove&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;ptCurrent&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;Point(e.X,&nbsp;e.Y)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;(mouseClicked)&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;(endPoint.X&nbsp;&lt;&gt;&nbsp;-<span class="visualBasic__number">1</span>)&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'Display&nbsp;Coordinates</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;X1.Text&nbsp;=&nbsp;startPoint.X.ToString()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Y1.Text&nbsp;=&nbsp;startPoint.Y.ToString()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;X2.Text&nbsp;=&nbsp;e.X.ToString()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Y2.Text&nbsp;=&nbsp;e.Y.ToString()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;endPoint&nbsp;=&nbsp;ptCurrent&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;(e.X&nbsp;&gt;&nbsp;startPoint.X&nbsp;<span class="visualBasic__keyword">And</span>&nbsp;e.Y&nbsp;&gt;&nbsp;startPoint.Y)&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Width&nbsp;=&nbsp;e.X&nbsp;-&nbsp;startPoint.X&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Height&nbsp;=&nbsp;e.Y&nbsp;-&nbsp;startPoint.Y&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">ElseIf</span>&nbsp;(e.X&nbsp;&lt;&nbsp;startPoint.X&nbsp;<span class="visualBasic__keyword">And</span>&nbsp;e.Y&nbsp;&gt;&nbsp;startPoint.Y)&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Width&nbsp;=&nbsp;startPoint.X&nbsp;-&nbsp;e.X&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Height&nbsp;=&nbsp;e.Y&nbsp;-&nbsp;startPoint.Y&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.X&nbsp;=&nbsp;e.X&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Y&nbsp;=&nbsp;startPoint.Y&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">ElseIf</span>&nbsp;(e.X&nbsp;&gt;&nbsp;startPoint.X&nbsp;<span class="visualBasic__keyword">And</span>&nbsp;e.Y&nbsp;&lt;&nbsp;startPoint.Y)&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Width&nbsp;=&nbsp;e.X&nbsp;-&nbsp;startPoint.X&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Height&nbsp;=&nbsp;startPoint.Y&nbsp;-&nbsp;e.Y&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.X&nbsp;=&nbsp;startPoint.X&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Y&nbsp;=&nbsp;e.Y&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Width&nbsp;=&nbsp;startPoint.X&nbsp;-&nbsp;e.X&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Height&nbsp;=&nbsp;startPoint.Y&nbsp;-&nbsp;e.Y&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.X&nbsp;=&nbsp;e.X&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea.Y&nbsp;=&nbsp;e.Y&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;PictureBox1.Refresh()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span></pre>
</div>
</div>
</div>
<h2>To display the dashes</h2>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>
<pre class="hidden">    Private Sub PicBox_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles PictureBox1.Paint
        Dim drawLine As New Pen(Color.Red)
        drawLine.DashStyle = DashStyle.Dash
        e.Graphics.DrawRectangle(drawLine, rectCropArea)
    End Sub</pre>
<div class="preview">
<pre class="vb">&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Private</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;PicBox_Paint(<span class="visualBasic__keyword">ByVal</span>&nbsp;sender&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;System.<span class="visualBasic__keyword">Object</span>,&nbsp;<span class="visualBasic__keyword">ByVal</span>&nbsp;e&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;System.Windows.Forms.PaintEventArgs)&nbsp;<span class="visualBasic__keyword">Handles</span>&nbsp;PictureBox1.Paint&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;drawLine&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;Pen(Color.Red)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;drawLine.DashStyle&nbsp;=&nbsp;DashStyle.Dash&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;e.Graphics.DrawRectangle(drawLine,&nbsp;rectCropArea)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span></pre>
</div>
</div>
</div>
<h2>For &quot;CROP&quot; button click event</h2>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>
<pre class="hidden">    Private Sub btnCrop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        PictureBox2.Refresh()

        Dim sourceBitmap As New Bitmap(PictureBox1.Image, PictureBox1.Width, PictureBox1.Height)
        Dim g As Graphics = PictureBox2.CreateGraphics()

        If Not (CheckBox1.Checked) Then
            g.DrawImage(sourceBitmap, New Rectangle(0, 0, PictureBox2.Width, PictureBox2.Height), rectCropArea, GraphicsUnit.Pixel)
            sourceBitmap.Dispose()

        Else
            Dim x1, x2, y1, y2 As Integer
            Try
                x1 = Convert.ToInt32(CX1.Text)
                x2 = Convert.ToInt32(CX2.Text)
                y1 = Convert.ToInt32(CY1.Text)
                y2 = Convert.ToInt32(CY2.Text)
            Catch ex As Exception
                MessageBox.Show(&quot;Enter valid Coordinates (only Integer values)&quot;)
            End Try

            If ((x1 &lt; x2 And y1 &lt; y2)) Then
                rectCropArea = New Rectangle(x1, y1, x2 - x1, y2 - y1)
            ElseIf (x2 &lt; x1 And y2 &gt; y1) Then
                rectCropArea = New Rectangle(x2, y1, x1 - x2, y2 - y1)
            ElseIf (x2 &gt; x1 And y2 &lt; y1) Then
                rectCropArea = New Rectangle(x1, y2, x2 - x1, y1 - y2)
            Else
                rectCropArea = New Rectangle(x2, y2, x1 - x2, y1 - y2)
            End If

            PictureBox1.Refresh() 'This repositions the dashed box to new location as per coordinates entered.

            g.DrawImage(sourceBitmap, New Rectangle(0, 0, PictureBox2.Width, PictureBox2.Height), rectCropArea, GraphicsUnit.Pixel)
            sourceBitmap.Dispose()
        End If
    End Sub	</pre>
<div class="preview">
<pre class="vb">&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Private</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;btnCrop_Click(<span class="visualBasic__keyword">ByVal</span>&nbsp;sender&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;System.<span class="visualBasic__keyword">Object</span>,&nbsp;<span class="visualBasic__keyword">ByVal</span>&nbsp;e&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;System.EventArgs)&nbsp;<span class="visualBasic__keyword">Handles</span>&nbsp;Button1.Click&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;PictureBox2.Refresh()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;sourceBitmap&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;Bitmap(PictureBox1.Image,&nbsp;PictureBox1.Width,&nbsp;PictureBox1.Height)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;g&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Graphics&nbsp;=&nbsp;PictureBox2.CreateGraphics()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;<span class="visualBasic__keyword">Not</span>&nbsp;(CheckBox1.Checked)&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;g.DrawImage(sourceBitmap,&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;Rectangle(<span class="visualBasic__number">0</span>,&nbsp;<span class="visualBasic__number">0</span>,&nbsp;PictureBox2.Width,&nbsp;PictureBox2.Height),&nbsp;rectCropArea,&nbsp;GraphicsUnit.Pixel)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;sourceBitmap.Dispose()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;x1,&nbsp;x2,&nbsp;y1,&nbsp;y2&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Integer</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;x1&nbsp;=&nbsp;Convert.ToInt32(CX1.Text)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;x2&nbsp;=&nbsp;Convert.ToInt32(CX2.Text)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;y1&nbsp;=&nbsp;Convert.ToInt32(CY1.Text)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;y2&nbsp;=&nbsp;Convert.ToInt32(CY2.Text)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Catch</span>&nbsp;ex&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Exception&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;MessageBox.Show(<span class="visualBasic__string">&quot;Enter&nbsp;valid&nbsp;Coordinates&nbsp;(only&nbsp;Integer&nbsp;values)&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Try</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;((x1&nbsp;&lt;&nbsp;x2&nbsp;<span class="visualBasic__keyword">And</span>&nbsp;y1&nbsp;&lt;&nbsp;y2))&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea&nbsp;=&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;Rectangle(x1,&nbsp;y1,&nbsp;x2&nbsp;-&nbsp;x1,&nbsp;y2&nbsp;-&nbsp;y1)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">ElseIf</span>&nbsp;(x2&nbsp;&lt;&nbsp;x1&nbsp;<span class="visualBasic__keyword">And</span>&nbsp;y2&nbsp;&gt;&nbsp;y1)&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea&nbsp;=&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;Rectangle(x2,&nbsp;y1,&nbsp;x1&nbsp;-&nbsp;x2,&nbsp;y2&nbsp;-&nbsp;y1)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">ElseIf</span>&nbsp;(x2&nbsp;&gt;&nbsp;x1&nbsp;<span class="visualBasic__keyword">And</span>&nbsp;y2&nbsp;&lt;&nbsp;y1)&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea&nbsp;=&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;Rectangle(x1,&nbsp;y2,&nbsp;x2&nbsp;-&nbsp;x1,&nbsp;y1&nbsp;-&nbsp;y2)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;rectCropArea&nbsp;=&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;Rectangle(x2,&nbsp;y2,&nbsp;x1&nbsp;-&nbsp;x2,&nbsp;y1&nbsp;-&nbsp;y2)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;PictureBox1.Refresh()&nbsp;<span class="visualBasic__com">'This&nbsp;repositions&nbsp;the&nbsp;dashed&nbsp;box&nbsp;to&nbsp;new&nbsp;location&nbsp;as&nbsp;per&nbsp;coordinates&nbsp;entered.</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;g.DrawImage(sourceBitmap,&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;Rectangle(<span class="visualBasic__number">0</span>,&nbsp;<span class="visualBasic__number">0</span>,&nbsp;PictureBox2.Width,&nbsp;PictureBox2.Height),&nbsp;rectCropArea,&nbsp;GraphicsUnit.Pixel)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;sourceBitmap.Dispose()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Sub</span>&nbsp;&nbsp;&nbsp;&nbsp;</pre>
</div>
</div>
</div>
<div class="endscriptcode">The sample demonstrates how to crop the image from a specific Picturebox control into another Picturebox control using mouse selection or specified coordinates.This code shows how to crop the image from Windown Forms PictureBox
 control (CSWinformCropImage) by Visual Studio 2010</div>
<div class="endscriptcode">
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt=""></a></div>
<div></div>
</div>
