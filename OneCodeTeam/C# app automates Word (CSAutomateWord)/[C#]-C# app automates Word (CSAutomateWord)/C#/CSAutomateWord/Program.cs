/****************************** Module Header ******************************\
* Module Name:  Program.cs
* Project:      CSAutomateWord
* Copyright (c) Microsoft Corporation.
* 
* The CSAutomateWord example demonstrates the use of Visual C# code to create 
* a Microsoft Word instance, create a new document, insert a paragraph and a 
* table, save the document, close the Microsoft Word application and then 
* clean up unmanaged COM resources.
* 
* Office automation is based on Component Object Model (COM). When you call a 
* COM object of Office from managed code, a Runtime Callable Wrapper (RCW) is 
* automatically created. The RCW marshals calls between the .NET application 
* and the COM object. The RCW keeps a reference count on the COM object. If 
* all references have not been released on the RCW, the COM object of Office 
* does not quit and may cause the Office application not to quit after your 
* automation. In order to make sure that the Office application quits cleanly, 
* the sample demonstrates two solutions.
* 
* Solution1.AutomateWord demonstrates automating Microsoft Word application by 
* using Microsoft Word Primary Interop Assembly (PIA) and explicitly assigning 
* each COM accessor object to a new variable that you would explicitly call 
* Marshal.FinalReleaseComObject to release it at the end. 
* 
* Solution2.AutomateWord demonstrates automating Microsoft Word application by 
* using Microsoft Word PIA and forcing a garbage collection as soon as the 
* automation function is off the stack (at which point the RCW objects are no 
* longer rooted) to clean up RCWs and release COM objects.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using directives
using System;
#endregion


namespace CSAutomateWord
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Solution1.AutomateWord demonstrates automating Microsoft Word 
            // application by using Microsoft Word PIA and explicitly 
            // assigning each COM accessor object to a new variable that you 
            // would explicitly call Marshal.FinalReleaseComObject to release 
            // it at the end. 
            Solution1.AutomateWord();

            Console.WriteLine();

            // Solution2.AutomateWord demonstrates automating Microsoft Word 
            // application by using Microsoft Word PIA and forcing a garbage 
            // collection as soon as the automation function is off the stack 
            // (at which point the RCW objects are no longer rooted) to clean 
            // up RCWs and release COM objects.
            Solution2.AutomateWord();
        }
    }
}