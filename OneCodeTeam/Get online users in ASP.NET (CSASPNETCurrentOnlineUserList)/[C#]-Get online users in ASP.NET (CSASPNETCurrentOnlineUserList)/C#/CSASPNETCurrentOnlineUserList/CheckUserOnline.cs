/**************************** Module Header ********************************\
* Module Name:    CheckUserOnline.cs
* Project:        CSASPNETCurrentOnlineUserList
* Copyright (c) Microsoft Corporation
*
* The Membership.GetNumberOfUsersOnline Method can get the number of online
* users,however many asp.net projects are not using membership.This project
* shows how to display a list of current online users' information without 
* using membership provider.

* This class used to add JavaScript code to the page.The JavaScript function
* can check the user's active time and post a request to the CheckUserOnlinePage.aspx 
* page.The project will auto delete the off line users'record from user's table by 
* checking the last active time.

* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\***************************************************************************/

using System;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.Configuration;

namespace CSASPNETCurrentOnlineUserList
{
    [Description("CheckUserOnline"), DefaultProperty(""),
    ToolboxData("<{0}:CheckUserOnline runat=server />")]
    public class CheckUserOnline : System.Web.UI.WebControls.PlaceHolder
    {
        /// <summary>
        /// Interval of refresh time，the default is 25.
        /// </summary>
        public virtual int RefreshTime
        {
            get
            {
                object _obj1 = this.ViewState["RefreshTime"];
                if (_obj1 != null) { return int.Parse(((string)_obj1).Trim()); }
                return 25;
            }
            set
            {
                this.ViewState["RefreshTime"] = value;
            }
        }
        protected override void Render(HtmlTextWriter writer)
        {
            // Get the visiting address for xmlhttp form web.config.
            string _refreshUrl = (string)ConfigurationManager.AppSettings["refreshUrl"];
            string _scriptString = @" <script language=""JavaScript"">";
            _scriptString += writer.NewLine;
            _scriptString += @"   window.attachEvent(""onload"", " + this.ClientID;
            _scriptString += @"_postRefresh);" + writer.NewLine;
            _scriptString += @"   var " + this.ClientID + @"_xmlhttp=null;";
            _scriptString += writer.NewLine;
            _scriptString += @"   function " + this.ClientID + @"_postRefresh(){";
            _scriptString += writer.NewLine;
            _scriptString += @"    var " + this.ClientID;
            _scriptString += @"_xmlhttp = new ActiveXObject(""Msxml2.XMLHTTP"");";
            _scriptString += writer.NewLine;
            _scriptString += @"    " + this.ClientID;
            _scriptString += @"_xmlhttp.Open(""POST"", """ + _refreshUrl + @""", false);";
            _scriptString += writer.NewLine;
            _scriptString += @"    " + this.ClientID + @"_xmlhttp.Send();";
            _scriptString += writer.NewLine;
            _scriptString += @"    var refreshStr= " + this.ClientID;
            _scriptString += @"_xmlhttp.responseText;";
            _scriptString += writer.NewLine;

            _scriptString += @"    try {";
            _scriptString += writer.NewLine;
            _scriptString += @"     var refreshStr2=refreshStr;";
            _scriptString += writer.NewLine;
            _scriptString += @"    }";
            _scriptString += writer.NewLine;
            _scriptString += @"    catch(e) {}";
            _scriptString += writer.NewLine;
            _scriptString += @"    setTimeout(""";
            _scriptString += this.ClientID + @"_postRefresh()"",";
            _scriptString += this.RefreshTime.ToString() + @"000);";
            _scriptString += writer.NewLine;
            _scriptString += @"   }" + writer.NewLine;
            _scriptString += @"<";
            _scriptString += @"/";
            _scriptString += @"script>" + writer.NewLine;
            writer.Write(writer.NewLine);
            writer.Write(_scriptString);
            writer.Write(writer.NewLine);
            base.Render(writer);
        }
    }
}