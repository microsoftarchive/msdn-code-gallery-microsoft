# Create new events for shape in Excel (CSExcelNewEventForShapes)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Office
## Topics
- Excel
- Shape
## Updated
- 12/20/2012
## Description

<h1>How to create new events for shape in Excel (CSExcelNewEventForShapes)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">This sample demonstrates how to create new events&#8203;<span style="">&nbsp;
</span>for shape in Excel by VSTO. The excel object model doesn't have any events to control manipulations with shapes.<span style="">&nbsp;
</span>In this project I demonstrate an approach to solve this problem.</p>
<p class="MsoNormal">The project creates four custom events to demonstrate, you also can add others events according to the codes of this project.<span style="">&nbsp;
</span>The four events are ShapeSelectionChange, ShapeRemoved, ShapeCreated and ShapeMoved.</p>
<p class="MsoNormal">ShapeSelectionChange occurs when user changes the selected shape; ShapeRemoved occurs when a shape is removed from the shapes collection of the current sheet; ShapeCreated occurs when a shape is added to the shapes collection of the current
 sheet; ShapeMoved occurs when a shape is moved.<span style=""> The custom task panel is used to track the event of shapes and show the event's message.
</span></p>
<h2>Building the Sample</h2>
<p class="MsoNormal" style="margin-bottom:7.5pt; line-height:normal">Before you build the sample, you must install Microsoft office2010 on your Operation System<span style="font-size:12.0pt; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;; color:black">.</span><span style="font-size:12.0pt; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;; color:black">
</span></p>
<p class="MsoNormal" style="margin-bottom:7.5pt; line-height:normal">This project references the
<span style="">P</span>rimary <span style="">I</span>nterop <span style="">A</span>ssembly
<span style="">(PIA )</span>for Microsoft Office <span style="">Excel </span>20<span style="">10</span><span style="font-size:12.0pt; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;; color:black">
</span></p>
<p class="MsoNormal" style="margin-bottom:7.5pt; line-height:normal">Be sure that your Excel2010 is not running when building sample.</p>
<p class="MsoNormal" style="margin-bottom:7.5pt; line-height:normal"><span style="font-size:12.0pt; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;; color:black"></span></p>
<h2>Running the Sample</h2>
<p class="MsoNormal">The following steps walk through a demonstration of creating new events for shapes.</p>
<p class="MsoNormal">Step1. Open CS<span style="">Excel</span>NewEventForShapes.sln to open the excel Add-in project<span style=""> and then click F5 to run this project.
</span></p>
<p class="MsoNormal"><span style="">You will see the following form: </span></p>
<p class="MsoNormal"><span style=""><img src="73216-image.png" alt="" width="774" height="620" align="middle">
</span><span style=""></span></p>
<p class="MsoNormal">Step2. <span style="">Select Add-Ins ribbon and click &quot;start checking shapes&quot; button to start tracking events of shapes.
</span></p>
<p class="MsoNormal"><span style="">You will see the below form: </span></p>
<p class="MsoNormal"><span style=""><img src="73217-image.png" alt="" width="778" height="627" align="middle">
</span><span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">Step3. Select<span style="">&nbsp; </span>a shape(for example a rectangle), the task panel will show &quot;The type name of the selected object is</span><span style="font-size:9.5pt; font-family:新宋体; color:#A31515">
</span><span style="">Rectangle&quot; message. </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">Step4. When you change selected shape (for example, you can now select a circle shape now),
</span><span style="">&nbsp;</span><span style="">the </span>ShapeSelectionChange<span style=""> event<span style="">&nbsp;
</span>occurs at this time, you will see the message &quot;ShapeSelctionChange, Selection form Rectangle 1 to Oval 2&quot; in custom task panel.<span style="">&nbsp;
</span></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">Step5. When you move a shape, the ShapeMoved event will occur and there are corresponding message to show in custom task panel, you also can delete or add a shape or shapes, the ShapeRemove or
</span>ShapeCreated<span style=""> event will occur. </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">Step6. The name of &quot;start checking shapes&quot; button changes to &quot;stop checking shapes&quot; after you click &quot;start checking shapes&quot; button, you can click &quot;stop checking shapes&quot; to stop track events of shapes and
 you also can restart to track events by clicking &quot;start checking shapes&quot; button.
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">Step7. Close Excel 2010 and In the Solution Explorer, right click </span>
CSExcelNewEventForShapes<span style=""> and click Clean. </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""></span></p>
<h2>Using the Code<span style=""> </span></h2>
<p class="MsoNormal"><span style="">Step1. Create an Excel Add-in project. On the File Menu, choose New, Project, In the templates pane, under the node for language you want to use, expand office and choose 2010 node, select Excel 2010 Add-in and the enter
 the name of the project. </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">Step2. Declare two classes,</span><span style="font-size:9.5pt; font-family:新宋体; color:#2B91AF">
</span><span style="">MyShape.cs class represents Excel.Shape Object and MyShapes.cs class represents the collection of Excel.Shape object.
</span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:新宋体; color:#2B91AF"></span></p>
<p class="MsoNormal"><span style="">Step3. Create a Windows Forms User Control named CustomTaskPanel as a task panel. Add a ListBox Control to show the events' message in User Control.
</span></p>
<p class="MsoNormal"><span style="">Step4. Declare instance of CustomTaskPanel and add it to CustomTaskPanes collection.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
// Task Panel object
CustomTaskPanel customTaskPanel;
 
    customTaskPanel = new CustomTaskPanel();
    <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/Microsoft.Office.Tools.CustomTaskPane.aspx" target="_blank" title="Auto generated link to Microsoft.Office.Tools.CustomTaskPane">Microsoft.Office.Tools.CustomTaskPane</a> mycustomerTaskPane = this.CustomTaskPanes.Add(customTaskPanel,&quot;Custom Task Panel Tracking Event&quot;);
    mycustomerTaskPane.Visible = true;
    mycustomerTaskPane.Width = 450;
    mycustomerTaskPane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionRight;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style=""></span></p>
<p class="MsoNormal"><span style="">Step5. Create custom<span style="">&nbsp; </span>
command bar button, click the button to start to track events of shapes. </span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
// CommandBar variable
// Click commandBarbutton to start log shapes' events
private Office.CommandBar commandBarStart;
private Office.CommandBarButton commandBarButtonStart;
// Add a custom commandbar button and set the name is &quot;Start checking shapes&quot;
commandBarStart = this.Application.CommandBars.ActiveMenuBar;


// Add the commandbar button into the commandbar
commandBarButtonStart = (Office.CommandBarButton)commandBarStart.Controls.Add(Office.MsoControlType.msoControlButton, Before: 1,Temporary: true);
commandBarButtonStart.Style = Office.MsoButtonStyle.msoButtonCaption;
commandBarButtonStart.Caption = StartJob;
commandBarButtonStart.FaceId = 60;
commandBarButtonStart.Visible = true;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style=""></span></p>
<p class="MsoNormal"><span style="">Step6. Create Custom events for shapes. </span>
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
        /// &lt;summary&gt;
        /// Occurs when the user selected shape change
        /// &lt;/summary&gt;
        /// &lt;param name=&quot;newselectedshape&quot;&gt;Selected shape Now&lt;/param&gt;
        /// &lt;param name=&quot;preselectedshape&quot;&gt;Selected shape Before&lt;/param&gt;
        private void ShapeSelctionChange(MyShape newselectedshape, MyShape preselectedshape)
        {
            customTaskPanel.AddMessage(&quot;ShapeSelctionChange, Selction from &quot; &#43; preselectedshape.Name &#43; &quot; to &quot; &#43; newselectedshape.Name);
        }




        /// &lt;summary&gt;
        /// Occurs when a shape(s) is removed from the Shapes collection of the current sheet
        /// &lt;/summary&gt;
        /// &lt;param name=&quot;shapesRemoved&quot;&gt;The removed shapes&lt;/param&gt;
        private void ShapesRemoved(MyShapes shapesRemoved)
        {
            customTaskPanel.AddMessage(&quot;ShapesRemoved Event Occures.&quot; &#43; shapesRemoved.Count.ToString() &#43; &quot; shape(s) are removed from Sheet.Shapes.&quot;);
        }
        /// &lt;summary&gt;
        /// Occurs when a shape is Created
        /// &lt;/summary&gt;
        /// &lt;param name=&quot;myshape&quot;&gt;The Created shape&lt;/param&gt;
        private void ShapesCreated(MyShapes shapesCreated)
        {
            customTaskPanel.AddMessage(&quot;ShapeCreated Event Occures.&quot;&#43;shapesCreated.Count.ToString() &#43; &quot; shape(s) are added to the Sheet.Shapes collection.&quot;);
        }


        /// &lt;summary&gt;
        /// Occurs when a shape is moved
        /// &lt;/summary&gt;
        /// &lt;param name=&quot;myshape&quot;&gt;The moved shape&lt;/param&gt;
        private void ShapeMoved(MyShape myshape)
        {
            customTaskPanel.AddMessage(&quot;ShapeMoved Event Occurs, Shape.Name='&quot; &#43; myshape.Name &#43; &quot;'&quot;);
        }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style=""><span style="">&nbsp;</span> </span></p>
<p class="MsoNormal"><span style="">Step7. Implement CommandBars.OnUpdate event to detect event occurring.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
      /// &lt;summary&gt;
      /// CommandBars Update function
      /// &lt;/summary&gt;
      private void commandBars_OnUpdate()
      {
          if (!isWorking)
          {
              return;
          }


          selecteTypeNameNow = GetSelectedTypeName();


          // Initialize current Selected shape and  current Existing shapes
          FillCollections();


          // detect whether the custom events occur
          ProcessExistingShapes();
          ProcessSelectedShape();


          // Initialize previous Selected shape and previous Existing shapes again
          shapesExistingLastTime = shapesExistingNow;
          shapeSelectedLastTime = shapeSelectedNow;


          // Initialize previous Selected Object and Type Name again
          selectedTypeNameLastTime = selecteTypeNameNow;
      }


      /// &lt;summary&gt;
      /// Get Selected shape and Existing shapes
      /// &lt;/summary&gt;
      private void FillCollections()
      {
          if (selecteTypeNameNow == null || selectedTypeNameLastTime == null)
          {
              customTaskPanel.AddMessage(&quot;The type name of the selected object is &quot; &#43; selectedTypeNameLastTime);
          }
          else
          {
              if (selecteTypeNameNow != selectedTypeNameLastTime)
              {
                  customTaskPanel.AddMessage(&quot;The type name of the selected object is &quot; &#43; selecteTypeNameNow);
              }
          }


          // Get the current selected shape and current existing shapes
          shapeSelectedNow = GetShapeSelected();
          shapesExistingNow = new MyShapes(Application.ActiveSheet);


          // Get the current Selected Object and current Type Name
          selecteTypeNameNow = GetSelectedTypeName();
      }


      /// &lt;summary&gt;
      /// Get Selected Type Name
      /// &lt;/summary&gt;
      /// &lt;returns&gt;return Selected TypeName&lt;/returns&gt;
      private string GetSelectedTypeName()
      {
          string typename = null;
          object selection = this.Application.Selection;
          if (selection != null)
          {
              typename = <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/Microsoft.VisualBasic.Information.TypeName.aspx" target="_blank" title="Auto generated link to Microsoft.VisualBasic.Information.TypeName">Microsoft.VisualBasic.Information.TypeName</a>(selection);
          }


          return typename;
      }


      /// &lt;summary&gt;
      /// Get Selected Shape
      /// &lt;/summary&gt;
      /// &lt;returns&gt;Return Selected Shape&lt;/returns&gt;
      private MyShape GetShapeSelected()
      {
          Excel.Shape selectedShape = null;
          MyShape myselectedShape = null;
          Excel.ShapeRange selectedShapeRange = null;
          try
          {
              selectedShapeRange = this.Application.Selection.ShapeRange;
          }
          catch
          {
              if (selecteTypeNameNow != selectedTypeNameLastTime)
              {
                  customTaskPanel.AddMessage(&quot;No shape selected&quot;);
              }
          }
          if (selectedShapeRange != null)
          {
              selectedShape = selectedShapeRange.Item(1) as Excel.Shape;
              if (selectedShape != null)
              {
                  myselectedShape = new MyShape(selectedShape);
                  return myselectedShape;
              }
          }


          return myselectedShape;
      }


      /// &lt;summary&gt;
      /// Analyze Existing Shapes
      /// &lt;/summary&gt;
      private void ProcessExistingShapes()
      {
          MyShapes shapesCreated = shapesExistingNow.GetShapesMissingIn(shapesExistingLastTime);
          MyShapes shapesRemoved = shapesExistingLastTime.GetShapesMissingIn(shapesExistingNow);


          // If Removed Shapes is not zero
          // Then ShapesRemoved Event Occurs
          // Show the number of Removed Shapes in Task Panel
          if (shapesRemoved.Count != 0)
          {
              ShapesRemoved(shapesRemoved);
          }
          if (shapesCreated.Count != 0)
          {
              ShapesCreated(shapesCreated);
          }
      }


      /// &lt;summary&gt;
      ///  Analyze Selected Shape
      /// &lt;/summary&gt;
      private void ProcessSelectedShape()
      {
          if (shapeSelectedLastTime == null || shapeSelectedNow == null)
          {
              return;
          }
          // compare previous selected shape's ID with current sekected shape's ID
          // to detect whether ShapeSelectionChange event occurs
          if (shapeSelectedNow.ID != shapeSelectedLastTime.ID)
          {
              this.ShapeSelctionChange(shapeSelectedNow, shapeSelectedLastTime);
          }
          else
          {
              // compare previous selected shape's top or left with current slected shape's top or left
              // to detect whether ShapeMoved event occurs
              if (shapeSelectedNow.Top != shapeSelectedLastTime.Top || shapeSelectedNow.Left != shapeSelectedLastTime.Left)
              {
                  this.ShapeMoved(shapeSelectedNow);
              }
          }
      }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style=""></span></p>
<h2>More Information</h2>
<p class="MsoNormal"><span style=""><a href="http://msdn.microsoft.com/en-us/library/aa942846.aspx">How to Add a custom task panel</a>
</span></p>
<p class="MsoNormal"><span style=""><a href="http://msdn.microsoft.com/en-US/library/0batekf4(v=vs.80).aspx">How to create office Menu</a>
</span></p>
<p class="MsoNormal"><span style=""><a href="http://msdn.microsoft.com/en-us/library/microsoft.office.core._commandbarsevents_event.onupdate.aspx">CommandBars.OnUpdate Event</a>
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
