/****************************** Module Header ******************************\
* Module Name:   ThisAddIn.cs
* Project:       CSExcelNewEventForShapes
* Copyright (c)  Microsoft Corporation.
* 
* This is a Add-in Class of Excel.
* The Excel object model doesn't have any events to control manipulations with shapes.
* The sample Add-in Project to handle some events of Excel.shape Object
* and log the events in custom task panel.
* This project creates four custom Events, they are ShapeSelectionChange, ShapeRemoved, 
* ShapeCreated and ShapeMoved.
* 
* 
* ShapeSelectionChange occurs when user changes selected shape.
* ShapeRemoved occurs when a shape is removed from the shapes collection of the current sheet.
* ShapeCreated occurs when a shape is added to the shapes collection of the current sheet.
* ShapeMoved occurs when a shape is moved.
*
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/



using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;

namespace CSExcelNewEventForShapes
{
    public partial class ThisAddIn
    {
        #region define Variables

        // Define name of CommandBarButton
        private const string StartJob = "Start checking shapes";
        private const string StopJob = "Stop checking shapes";

        // detect the CommandBarButton whether is working
        private bool isWorking = false;

        // CommandBars variable
        private Office.CommandBars commandBars;

        // CommandBar variable
        // Click commandBarbutton to start log shapes' events
        private Office.CommandBar commandBarStart;
        private Office.CommandBarButton commandBarButtonStart;

        // define a rectangle shape
        private MyShape rectangleShape;

        // define a circle shape
        private MyShape circleShape;

        // Task Panel object
        CustomTaskPanel customTaskPanel;

        // previous Selected shape and current selected shape
        // compare the ID in the two shape to detect Shapeselectionchange event       
        MyShape shapeSelectedNow = null;
        MyShape shapeSelectedLastTime = null;

        // previous existing shapes and current existing shapes
        // compare the Count property in the two shape collection to detect ShapeCreated and ShapeRemoved events
        MyShapes shapesExistingNow = null;
        MyShapes shapesExistingLastTime = null;

        // Previous selected Type Name and current selected Type Name
        string selectedTypeNameLastTime = null;
        string selecteTypeNameNow = null;

        #endregion

        // Initialize Control
        private void InitializeCustomComponents()
        {
            // Add a custom commandbar button and set the name is "Start checking shapes"
            commandBarStart = this.Application.CommandBars.ActiveMenuBar;

            // Add the commandbar button into the commandbar
            commandBarButtonStart = (Office.CommandBarButton)commandBarStart.Controls.Add(Office.MsoControlType.msoControlButton, Before: 1, Temporary: true);
            commandBarButtonStart.Style = Office.MsoButtonStyle.msoButtonCaption;
            commandBarButtonStart.Caption = StartJob;
            commandBarButtonStart.FaceId = 60;
            commandBarButtonStart.Visible = true;

            // Register click event of commandbarbutton
            commandBarButtonStart.Click += new Office._CommandBarButtonEvents_ClickEventHandler(commandBarButtonStart_Click);

            // Use CommandBars.OnUpdate event to detect shapes' event
            commandBars = this.Application.CommandBars;
            commandBars.OnUpdate += new Office._CommandBarsEvents_OnUpdateEventHandler(commandBars_OnUpdate);

            // Initialize Task Panel 
            // set the Position and width of task panel 
            customTaskPanel = new CustomTaskPanel();
            Microsoft.Office.Tools.CustomTaskPane mycustomerTaskPane = this.CustomTaskPanes.Add(customTaskPanel, "Custom Task Panel Tracking Event");
            mycustomerTaskPane.Visible = true;
            mycustomerTaskPane.Width = 430;
            mycustomerTaskPane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionRight;

            // Initialize Shapes in active worksheet
            Excel.Worksheet shapeworksheet = this.Application.ActiveSheet;
            Excel.Shape shapeRect;
            Excel.Shape shapeCircle;
            shapeRect = shapeworksheet.Shapes.AddShape(Office.MsoAutoShapeType.msoShapeRectangle, 60, 80, 80, 30);
            shapeCircle = shapeworksheet.Shapes.AddShape(Office.MsoAutoShapeType.msoShapeOval, 200, 30, 50, 50);

            rectangleShape = new MyShape(shapeRect);
            circleShape = new MyShape(shapeCircle);

            // Initialize Shapes and Selected shape before the shapes change
            shapeSelectedLastTime = GetShapeSelected();
            shapesExistingLastTime = new MyShapes(shapeworksheet);
        }

        /// <summary>
        /// CommandBarButton Click function
        /// </summary>
        /// <param name="cmdBarButton">CommandBarButton Object</param>
        /// <param name="cancel"></param>
        private void commandBarButtonStart_Click(Office.CommandBarButton cmdBarButton, ref bool cancel)
        {
            StartStop(!isWorking);
        }

        /// <summary>
        /// Start or Stop to log the events' message
        /// </summary>
        /// <param name="mode">true represents for logging events started,false is to stop log events</param>
        private void StartStop(bool mode)
        {
            isWorking = mode;
            if (isWorking)
            {
                customTaskPanel.Clear();
                commandBarButtonStart.Caption = StopJob;

                // Get previous Selected Object and Type Name
                selectedTypeNameLastTime = GetSelectedTypeName();

                // when User restarts to log events,
                // Must Initialize previous Selected shape and previous Existing shapes again.
                FillCollections();
                shapesExistingLastTime = shapesExistingNow;
                shapeSelectedLastTime = shapeSelectedNow;

                // Initialize previous Selected Object and Type Name again
                selectedTypeNameLastTime = selecteTypeNameNow;
            }
            else
            {
                commandBarButtonStart.Caption = StartJob;

                // Dispose object
                shapeSelectedLastTime = null;
                shapeSelectedNow = null;
                shapesExistingLastTime = null;
                shapesExistingNow = null;
                selectedTypeNameLastTime = null;
                selecteTypeNameNow = null;
            }
        }

        /// <summary>
        /// CommandBars Update function
        /// </summary>
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

        /// <summary>
        /// Get Selected shape and Existing shapes
        /// </summary>
        private void FillCollections()
        {
            if (selecteTypeNameNow == null || selectedTypeNameLastTime == null)
            {
                customTaskPanel.AddMessage("The type name of the selected object is " + selectedTypeNameLastTime);
            }
            else
            {
                if (selecteTypeNameNow != selectedTypeNameLastTime)
                {
                    customTaskPanel.AddMessage("The type name of the selected object is " + selecteTypeNameNow);
                }
            }

            // Get the current selected shape and current existing shapes
            shapeSelectedNow = GetShapeSelected();
            shapesExistingNow = new MyShapes(Application.ActiveSheet);

            // Get the current Selected Object and current Type Name
            selecteTypeNameNow = GetSelectedTypeName();
        }

        /// <summary>
        /// Get Selected Type Name
        /// </summary>
        /// <returns>return Selected TypeName</returns>
        private string GetSelectedTypeName()
        {
            string typename = null;
            object selection = this.Application.Selection;
            if (selection != null)
            {
                typename = Microsoft.VisualBasic.Information.TypeName(selection);
            }

            return typename;
        }

        /// <summary>
        /// Get Selected Shape
        /// </summary>
        /// <returns>Return Selected Shape</returns>
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
                    customTaskPanel.AddMessage("No shape selected");
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

        /// <summary>
        /// Analyze Existing Shapes
        /// </summary>
        private void ProcessExistingShapes()
        {
            MyShapes shapesCreated = shapesExistingNow.GetShapesMissingIn(shapesExistingLastTime);
            MyShapes shapesRemoved = shapesExistingLastTime.GetShapesMissingIn(shapesExistingNow);

            // ShapesRemoved Event Occurs If the count of the Removed Shapes is not zero.
            // Also, the number of Removed Shapes will be shown in Task Panel.
            if (shapesRemoved.Count != 0)
            {
                ShapesRemoved(shapesRemoved);
            }
            if (shapesCreated.Count != 0)
            {
                ShapesCreated(shapesCreated);
            }
        }

        /// <summary>
        ///  Analyze Selected Shape
        /// </summary>
        private void ProcessSelectedShape()
        {
            if (shapeSelectedLastTime == null || shapeSelectedNow == null)
            {
                return;
            }

            // Compare previous selected shape's ID with current selected shape's ID
            // to detect whether ShapeSelectionChange event occurs
            if (shapeSelectedNow.ID != shapeSelectedLastTime.ID)
            {
                this.ShapeSelctionChange(shapeSelectedNow, shapeSelectedLastTime);
            }
            else
            {
                // Compare previous selected shape's top or left with current selected shape's top or left
                // to detect whether ShapeMoved event occurs
                if (shapeSelectedNow.Top != shapeSelectedLastTime.Top || shapeSelectedNow.Left != shapeSelectedLastTime.Left)
                {
                    this.ShapeMoved(shapeSelectedNow);
                }
            }
        }


        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            // Initialize components in worksheet
            InitializeCustomComponents();
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion

        #region Custom Events

        /// <summary>
        /// Occurs when the user selected shape change
        /// </summary>
        /// <param name="newselectedshape">Selected shape Now</param>
        /// <param name="preselectedshape">Selected shape Before</param>
        private void ShapeSelctionChange(MyShape newselectedshape, MyShape preselectedshape)
        {
            customTaskPanel.AddMessage("ShapeSelctionChange, Selection from " + preselectedshape.Name + " to " + newselectedshape.Name);
        }


        /// <summary>
        /// Occurs when a shape(s) is removed from the Shapes collection of the current sheet
        /// </summary>
        /// <param name="shapesRemoved">The removed shapes</param>
        private void ShapesRemoved(MyShapes shapesRemoved)
        {
            customTaskPanel.AddMessage("ShapesRemoved Event Occurs." + shapesRemoved.Count.ToString() + " shape(s) are removed from Sheet.Shapes.");
        }
        /// <summary>
        /// Occurs when a shape is Created
        /// </summary>
        /// <param name="myshape">The Created shape</param>
        private void ShapesCreated(MyShapes shapesCreated)
        {
            customTaskPanel.AddMessage("ShapeCreated Event Occurs." + shapesCreated.Count.ToString() + " shape(s) are added to the Sheet.Shapes collection.");
        }

        /// <summary>
        /// Occurs when a shape is moved
        /// </summary>
        /// <param name="myshape">The moved shape</param>
        private void ShapeMoved(MyShape myshape)
        {
            customTaskPanel.AddMessage("ShapeMoved Event Occurs, Shape.Name='" + myshape.Name + "'");
        }
        #endregion
    }
}
