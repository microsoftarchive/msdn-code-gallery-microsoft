/******************************** Module Header ********************************\
* Module Name:  Solution2.cs
* Project:      CSAutomatePowerPoint
* Copyright (c) Microsoft Corporation.
* 
* Solution2.AutomatePowerPoint demonstrates automating Microsoft PowerPoint 
* application by using Microsoft PowerPoint Primary Interop Assembly (PIA) and 
* forcing a garbage collection as soon as the automation function is off the 
* stack (at which point the Runtime Callable Wrapper (RCW) objects are no longer 
* rooted) to clean up RCWs and release COM objects.
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
using System.Text;
using System.IO;
using System.Reflection;

using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
#endregion


namespace CSAutomatePowerPoint
{
    static class Solution2
    {
        public static void AutomatePowerPoint()
        {
            AutomatePowerPointImpl();


            // Clean up the unmanaged PowerPoint COM resources by forcing a 
            // garbage collection as soon as the calling function is off the 
            // stack (at which point these objects are no longer rooted).

            GC.Collect();
            GC.WaitForPendingFinalizers();
            // GC needs to be called twice in order to get the Finalizers called 
            // - the first time in, it simply makes a list of what is to be 
            // finalized, the second time in, it actually is finalizing. Only 
            // then will the object do its automatic ReleaseComObject.
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private static void AutomatePowerPointImpl()
        {
            try
            {
                // Create an instance of Microsoft PowerPoint and make it 
                // invisible.

                PowerPoint.Application oPowerPoint = new PowerPoint.Application();


                 // By default PowerPoint is invisible, till you make it visible:
                // oPowerPoint.Visible = Office.MsoTriState.msoFalse;


                // Create a new Presentation.

                PowerPoint.Presentation oPre = oPowerPoint.Presentations.Add(
                    Microsoft.Office.Core.MsoTriState.msoTrue);
                Console.WriteLine("A new presentation is created");

                // Insert a new Slide and add some text to it.

                Console.WriteLine("Insert a slide");
                PowerPoint.Slide oSlide = oPre.Slides.Add(1,
                    PowerPoint.PpSlideLayout.ppLayoutText);

                Console.WriteLine("Add some texts");
                oSlide.Shapes[1].TextFrame.TextRange.Text =
                    "All-In-One Code Framework";

                // Save the presentation as a pptx file and close it.

                Console.WriteLine("Save and close the presentation");

                string fileName = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location) + "\\Sample2.pptx";
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
                Console.WriteLine("Solution2.AutomatePowerPoint throws the error: {0}",
                    ex.Message);
            }
        }
    }
}
