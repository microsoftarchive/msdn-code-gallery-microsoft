/****************************** Module Header ******************************\
Module Name:  MessageBoxCore.cs
Project:      CSASPNETIntelligentErrorPage
Copyright (c) Microsoft Corporation.
 
The sample code demonstrates how to create a MessageBox in asp.net, usually we
often use JavaScript functions "alert()" or "confirm()" to show simple messages
and make a simple choice with customers, but these dialog boxes is very simple,
we can not add any different and complicate controls, images or styles. As we know,
good web sites always have their own web styles, such as typeface and colors, 
and in this situation, JavaScript dialog boxes looks not very well. So this sample
shows how to make an Asp.net MessageBox.

The MessageBoxCore class stores the MessageBox's core html code, we will replace 
the important parts of MessageBox, such as text, title, icons, script and so on.
 
This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL
All other rights reserved.
 
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSASPNETMessageBox
{
    public class MessageBoxCore
    {
        public static string MessageBoxButtonHtml = @"<input type='button' value='{0}' class='{1}' onclick='{2}' />";

        public static string MessageBoxHTML = @"<div id='wholePage' class=page>&nbsp;</div>
                                              <div id='messageBox' class='content'>
	                                          <div style='margin:4px;'>
		                                      <table  style='background-color:#FFFFFF; border:1px solid #999999; width: 100%;height:192px '>
			                                  <tr>
				                              <td colspan='2' style='border-bottom:1px solid #CCCCCC;font-family:tahoma; font-size:11px; font-weight:bold; padding-left:5px; color:Black; text-align:center; height:30px;'>MessageBox-{0}</td>
			                                  </tr><tr><tr><td></td><td></td></tr><tr><td></td><td></td></tr>
				                              <td style='width:150px;text-align:center;'>{1}</td>
				                              <td style='font-family:tahoma; font-size:14px;padding-left:5px;'>
                                              <textarea style='overflow:hidden; width: 99%;height:62px; border:none; font-size:14px'>{2}</textarea>
                                              </td></tr>
			                                  <tr>
				                              <td colspan='2' style='margin-top:5px;text-align:center;'>{3}</td>
			                                  </tr>
		                                      </table>
	                                          </div>
                                              </div>";

        public static string MessageBoxScript = @"var xmlHttpRequest;
                                               function Yes() {{
                                               var info = document.getElementById('result');
                                               info.innerHTML = """";
                                               var back = document.getElementById('wholePage');
                                               back.parentNode.removeChild(back);
                                               var message = document.getElementById('messageBox');
                                               message.parentNode.removeChild(message);{0}}}

                                               function Success(result) {{
                                                   var info = document.getElementById('result');
                                                   info.innerHTML = info.innerHTML + ""<br />"" + result;
                                               }}

                                               function Failed(error) {{
                                                   var info = document.getElementById('result');
                                                   info.innerHTML = info.innerHTML + ""<br />"" + error;
                                               }}

                                               function No() {{
                                                   var info = document.getElementById('result');
                                                   info.innerHTML = """";
                                                   var back = document.getElementById('wholePage');
                                                   back.parentNode.removeChild(back);
                                                   var message = document.getElementById('messageBox');
                                                   message.parentNode.removeChild(message);{1}}}";
    }
}