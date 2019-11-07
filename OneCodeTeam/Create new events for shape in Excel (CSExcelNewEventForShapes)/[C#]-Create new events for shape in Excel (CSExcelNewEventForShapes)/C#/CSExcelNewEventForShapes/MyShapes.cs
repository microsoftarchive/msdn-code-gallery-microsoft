/****************************** Module Header ******************************\
* Module Name:   MyShapes.cs
* Project:       CSExcelNewEventForShapes
* Copyright (c)  Microsoft Corporation.
* 
* This Class represents the collection of Excel.Shape Object
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



using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;

namespace CSExcelNewEventForShapes
{
    public class MyShapes
    {
        // Collection of Excel.Shape Object
        List<MyShape> myShapes = null;
        
        /// <summary>
        /// Constructor function without params
        /// </summary>
        public MyShapes()
        {
            myShapes = new List<MyShape>();
        }

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="shapes">Collection of Shape</param>
        public MyShapes(List<MyShape> shapes)
        {
            myShapes = shapes;
        }

        /// <summary>
        /// Constructor function
        /// </summary>
        /// <param name="shapeSource">Worksheet Object</param>
        public MyShapes(Excel._Worksheet shapeSource)
        {
            InitializeListOfShapes(shapeSource.Shapes);
        }

        // Count Property of Shapes
        // Use this property to detect creating or deleting shape event
        public int Count
        {
            get
            {
                return myShapes.Count;
            }
        }

        // Initialize Shapes List
        private void InitializeListOfShapes(Excel.Shapes shapes)
        {
            myShapes = new List<MyShape>();
            for (int i = 1; i <= shapes.Count; i++)
            {
                Excel.Shape shape = shapes.Item(i);
                MyShape myShape = new MyShape(shape);
                myShapes.Add(myShape);
            }
        }

        /// <summary>
        /// Verify Shapes Collection whether contain a shape
        /// </summary>
        /// <param name="id">Shape Id</param>
        /// <returns>If Contain an same shape return true,else return false</returns>
        public bool Contains(int id)
        {
            foreach (MyShape aShape in myShapes)
            {
                if (aShape.ID == id)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get Created Shape Collection or Removed Shape Collection
        /// </summary>
        /// <param name="shapesToCheck">Shapes Collections to be Checked</param>
        /// <returns>Return ShapesCreated Collection or ShapesRemoved</returns>
        public MyShapes GetShapesMissingIn(MyShapes shapesToCheck)
        {
            MyShapes newShapes = new MyShapes();
            foreach (MyShape shape in myShapes)
            {
                if (shapesToCheck == null || !shapesToCheck.Contains(shape.ID))
                {
                    newShapes.Add(shape);
                }
            }
            return newShapes;
        }

        /// <summary>
        /// Add Shape Method
        /// </summary>
        /// <param name="shapeToAdd">The Added Shape</param>
        public void Add(MyShape shapeToAdd)
        {
            myShapes.Add(shapeToAdd);
        }

        /// <summary>
        /// Remove Shape Method
        /// </summary>
        /// <param name="shapeToRemove">The Removed Shape</param>
        public void Remove(MyShape shapeToRemove)
        {
            myShapes.Remove(shapeToRemove);
        }
    }
}
