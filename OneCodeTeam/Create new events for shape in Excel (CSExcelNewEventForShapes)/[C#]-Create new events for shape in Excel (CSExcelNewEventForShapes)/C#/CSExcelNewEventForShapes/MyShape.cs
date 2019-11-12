/****************************** Module Header ******************************\
* Module Name:  MyShape.cs
* Project:      CSExcelNewEventForShapes
* Copyright (c) Microsoft Corporation.
* 
* This Class represents Excel.Shape Object
* We can get and set the properties of Excel.Shape via this Class
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

namespace CSExcelNewEventForShapes
{
    public class MyShape
    {
        #region Private Fields

        private string name;
        private int id;
        private double top;
        private double left;
        private double height;
        private double width;

        #endregion 

        #region Properties

        // Name Property of Excel.Shape Object
        public string Name
        { get { return name; } }

        // ID Property of Excel.Shape Object
        // We can compare the ID of the Shape to detect selection change
        public int ID
        { get { return id; } }

        // We can compare the Top or Left Property of Shape to detect shape's position change
        // The distance from the top edge of Excel.Shape Object
        public double Top
        { get { return top; } }

        // The distance from the left edge of Excel.Shape Object 
        public double Left
        { get { return left; } }

        // We can compare the Height or Width Property of Shape to detect shape's Size change
        // Height of Excel.Shape Object
        public double Height
        { get { return height; } }

        // Width of Excel.Shape Object
        public double Width
        { get { return width; } }

        #endregion

        // Constructor Method
        public MyShape(Excel.Shape shape)
        {
            name = shape.Name;
            id = shape.ID;
            Update(shape);
        }

        // Update Shape Position and Size
        public void Update(Excel.Shape shape)
        {
            top = shape.Top;
            left = shape.Left;
            height = shape.Height;
            width = shape.Width;
            name = shape.Name;
        }
    }
}
