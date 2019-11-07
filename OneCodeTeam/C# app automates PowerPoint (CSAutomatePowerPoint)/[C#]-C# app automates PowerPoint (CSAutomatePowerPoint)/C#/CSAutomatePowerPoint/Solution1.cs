/******************************** Module Header ********************************\
* Module Name:  Solution1.cs
* Project:      CSAutomatePowerPoint
* Copyright (c) Microsoft Corporation.
* 
* Solution1.AutomatePowerPoint demonstrates automating Microsoft PowerPoint 
* application by using Microsoft PowerPoint Primary Interop Assembly (PIA) and 
* explicitly assigning each COM accessor object to a new variable that you would 
* explicitly call Marshal.FinalReleaseComObject to release it at the end. When 
* you use this solution, it is important to avoid calls that tunnel into the 
* object model because they will orphan Runtime Callable Wrapper (RCW) on the 
* heap that you will not be able to access in order to call 
* Marshal.ReleaseComObject. You need to be very careful. For example, 
* 
*   PowerPoint.Presentation oPre = oPowerPoint.Presentations.Add(
*     Office.MsoTriState.msoTrue);
*  
* Calling oPowerPoint.Presentations.Add creates an RCW for the Presentations 
* object. If you invoke these accessors via tunneling as this code does, the RCW 
* for Presentations is created on the GC heap, but the reference is created under 
* the hood on the stack and are then discarded. As such, there is no way to call 
* MarshalFinalReleaseComObject on this RCW. To get such kind of RCWs released, 
* you would either need to force a garbage collection as soon as the calling 
* function is off the stack (see Solution2.AutomatePowerPoint), or you would need 
* to explicitly assign each accessor object to a variable and free it.
* 
*   PowerPoint.Presentations oPres = oPowerPoint.Presentations;
*   PowerPoint.Presentation oPre = oPres.Add(Office.MsoTriState.msoTrue);
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*******************************************************************************/

#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using System.Runtime.InteropServices;
#endregion


namespace CSAutomatePowerPoint
{
    static class Solution1
    {
        public static void AutomatePowerPoint()
        {
            PowerPoint.Application oPowerPoint = null;
            PowerPoint.Presentations oPres = null;
            PowerPoint.Presentation oPre = null;
            PowerPoint.Slides oSlides = null;
            PowerPoint.Slide oSlide = null;
            PowerPoint.Shapes oShapes = null;
            PowerPoint.Shape oShape = null;
            PowerPoint.TextFrame oTxtFrame = null;
            PowerPoint.TextRange oTxtRange = null;

            try
            {
                // Create an instance of Microsoft PowerPoint and make it 
                // invisible.
                oPowerPoint = new PowerPoint.Application();

                // By default PowerPoint is invisible, till you make it visible.
                // oPowerPoint.Visible = Office.MsoTriState.msoFalse;



                // Create a new Presentation.

                oPres = oPowerPoint.Presentations;
                oPre = oPres.Add(Office.MsoTriState.msoTrue);
                Console.WriteLine("A new presentation is created");

                // Insert a new Slide and add some text to it.

                Console.WriteLine("Insert a slide");
                oSlides = oPre.Slides;
                oSlide = oSlides.Add(1, PowerPoint.PpSlideLayout.ppLayoutText);

                Console.WriteLine("Add some texts");
                oShapes = oSlide.Shapes;
                oShape = oShapes[1];
                oTxtFrame = oShape.TextFrame;
                oTxtRange = oTxtFrame.TextRange;
                oTxtRange.Text = "All-In-One Code Framework";

                // Save the presentation as a pptx file and close it.

                Console.WriteLine("Save and close the presentation");

                string fileName = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location) + "\\Sample1.pptx";
                oPre.SaveAs(fileName,
                    PowerPoint.PpSaveAsFileType.ppSaveAsOpenXMLPresentation,
                    Office.MsoTriState.msoTriStateMixed);
                oPre.Close();

                // Quit the PowerPoint application.

                Console.WriteLine("Quit the PowerPoint application");
                oPowerPoint.Quit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Solution1.AutomatePowerPoint throws the error: {0}",
                    ex.Message);
            }
            finally
            {
                // Clean up the unmanaged PowerPoint COM resources by explicitly 
                // calling Marshal.FinalReleaseComObject on all accessor objects. 
                // See http://support.microsoft.com/kb/317109.

                if (oTxtRange != null)
                {
                    Marshal.FinalReleaseComObject(oTxtRange);
                    oTxtRange = null;
                }
                if (oTxtFrame != null)
                {
                    Marshal.FinalReleaseComObject(oTxtFrame);
                    oTxtFrame = null;
                }
                if (oShape != null)
                {
                    Marshal.FinalReleaseComObject(oShape);
                    oShape = null;
                }
                if (oShapes != null)
                {
                    Marshal.FinalReleaseComObject(oShapes);
                    oShapes = null;
                }
                if (oSlide != null)
                {
                    Marshal.FinalReleaseComObject(oSlide);
                    oSlide = null;
                }
                if (oSlides != null)
                {
                    Marshal.FinalReleaseComObject(oSlides);
                    oSlides = null;
                }
                if (oPre != null)
                {
                    Marshal.FinalReleaseComObject(oPre);
                    oPre = null;
                }
                if (oPres != null)
                {
                    Marshal.FinalReleaseComObject(oPres);
                    oPres = null;
                }
                if (oPowerPoint != null)
                {
                    Marshal.FinalReleaseComObject(oPowerPoint);
                    oPowerPoint = null;
                }
            }
        }
    }
}
