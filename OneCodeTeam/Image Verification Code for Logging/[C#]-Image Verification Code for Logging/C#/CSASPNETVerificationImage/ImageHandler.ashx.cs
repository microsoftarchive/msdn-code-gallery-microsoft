/****************************** Module Header ******************************\
* Module Name:    ImageHandler.ashx.cs
* Project:        CSASPNETVerificationImage
* Copyright (c) Microsoft Corporation
*
* Use Graphics to generate an image with random numbers or letters. Then send
* it back to the client and save the random content into Session.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Web;
using System.Web.Services;
using System.Web.SessionState;
using System.Drawing;
using System.Text;
using System.Drawing.Imaging;

namespace CSASPNETVerificationImage
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class ImageHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            // Whether to generate verification code or not.
            bool isCreate = true;

            // Session["CreateTime"]: The createTime of verification code
            if (context.Session["CreateTime"] == null)
            {
                context.Session["CreateTime"] = DateTime.Now;
            }
            else
            {
                DateTime startTime = Convert.ToDateTime(context.Session["CreateTime"]);
                DateTime endTime = Convert.ToDateTime(DateTime.Now);
                TimeSpan ts = endTime - startTime;

                // The time interval to generate a verification code.
                if (ts.Minutes > 15)
                {
                    isCreate = true;
                    context.Session["CreateTime"] = DateTime.Now;
                }
                else
                {
                    isCreate = false;
                }
            }


            context.Response.ContentType = "image/gif";

            // Create Bitmap object and to draw
            Bitmap basemap = new Bitmap(200, 60);
            Graphics graph = Graphics.FromImage(basemap);
            graph.FillRectangle(new SolidBrush(Color.White), 0, 0, 200, 60);
            Font font = new Font(FontFamily.GenericSerif, 48, FontStyle.Bold, GraphicsUnit.Pixel);
            Random r = new Random();
            string letters = "ABCDEFGHIJKLMNPQRSTUVWXYZabcdefghijklmnpqrstuvwxyz0123456789";
            string letter;
            StringBuilder s = new StringBuilder();

            if (isCreate)
            {
                // Add a random string
                for (int x = 0; x < 5; x++)
                {
                    letter = letters.Substring(r.Next(0, letters.Length - 1), 1);
                    s.Append(letter);

                    // Draw the String
                    graph.DrawString(letter, font, new SolidBrush(Color.Black), x * 38, r.Next(0, 10));
                }
            }
            else
            {
                // Using the previously generated verification code.
                string currentCode = context.Session["ValidateCode"].ToString();
                s.Append(currentCode);

                foreach (char item in currentCode)
                {
                    letter = item.ToString();

                    // Draw the String
                    graph.DrawString(letter, font, new SolidBrush(Color.Black), currentCode.IndexOf(item) * 38, r.Next(0, 10));
                }
            }

            // Confuse background
            Pen linePen = new Pen(new SolidBrush(Color.Black), 2);
            for (int x = 0; x < 6; x++)
            {
                graph.DrawLine(linePen, new Point(r.Next(0, 199), r.Next(0, 59)), new Point(r.Next(0, 199), r.Next(0, 59)));
            }

            // Save the picture to the output stream     
            basemap.Save(context.Response.OutputStream, ImageFormat.Gif);

            // If you do not implement the IRequiresSessionState,it will be wrong here,and it can not generate a picture also.
            context.Session["ValidateCode"] = s.ToString();
            context.Response.End();

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
