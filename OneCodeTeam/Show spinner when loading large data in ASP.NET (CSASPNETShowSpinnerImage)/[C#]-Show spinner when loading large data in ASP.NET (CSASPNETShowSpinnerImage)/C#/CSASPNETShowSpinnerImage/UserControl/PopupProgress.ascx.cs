/****************************** Module Header ******************************\
* Module Name: PopupProgress.ascx.cs
* Project:     CSASPNETShowSpinnerImage
* Copyright (c) Microsoft Corporation
*
* This project illustrates how to show spinner image while retrieving huge of 
* data. As we know, handle a time-consuming operate always requiring a long 
* time, we need to show a spinner image for better user experience.
* 
* This user control will show a popup when user click the button on 
* Default.aspx page. 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace CSASPNETShowSpinnerImage
{
    public partial class PopupProgress : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        /// <summary>
        /// This method is used to load images of customize files and 
        /// register JavaScript code on User Control page.
        /// </summary>
        /// <returns></returns>
        public string LoadImage()
        {
            StringBuilder strbScript = new StringBuilder();
            string imageUrl = "";

            strbScript.Append("var imgMessage = new Array();");
            strbScript.Append("var imgUrl = new Array();");
            string[] strs = new string[7];
            strs[0] = "Image/0.jpg";
            strs[1] = "Image/1.jpg";
            strs[2] = "Image/2.jpg";
            strs[3] = "Image/3.jpg";
            strs[4] = "Image/4.jpg";
            strs[5] = "Image/5.jpg";
            strs[6] = "Image/6.jpg";
            for (int i = 0; i < strs.Length; i++)
            {
                imageUrl = strs[i];

                strbScript.Append(String.Format("imgUrl[{0}] = '{1}';", i, imageUrl));
                strbScript.Append(String.Format("imgMessage[{0}] = '{1}';", i, imageUrl.Substring(imageUrl.LastIndexOf('.') - 1)));
            }
            strbScript.Append("for (var i=0; i<imgUrl.length; i++)");
            strbScript.Append("{ (new Image()).src = imgUrl[i]; }");
            return strbScript.ToString();
        }

    }
}