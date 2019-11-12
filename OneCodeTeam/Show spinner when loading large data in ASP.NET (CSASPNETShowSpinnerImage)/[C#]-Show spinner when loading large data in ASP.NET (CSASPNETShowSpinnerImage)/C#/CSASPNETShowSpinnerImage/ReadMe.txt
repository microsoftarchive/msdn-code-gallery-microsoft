========================================================================
                  CSASPNETShowSpinnerImage Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

This project illustrates how to show spinner image while retrieving huge 
amount of data. As we know, handle a time-consuming operate always requiring 
a long time, we need to show a spinner image for better user experience.

/////////////////////////////////////////////////////////////////////////////
Demo:

Please follow these demonstration steps below.

Step 1: Open the CSASPNETShowSpinnerImage.sln.

Step 2: Expand the CSASPNETShowSpinnerImage web application and press 
        Ctrl + F5 to show the Default.aspx.

Step 3: You will see the date time and a button on Default.aspx page, please
        click the button to retrieve data from XML file.

Step 4: The application will show a popup for displaying spinner image, after 
        several seconds, you can find the data is been shown in GridView 
		control.

Step 5: Validation finished.


/////////////////////////////////////////////////////////////////////////////
Implementation:

Step 1. Create a C# "ASP.NET Empty Web Application" in Visual Studio 2010 or
        Visual Web Developer 2010. Name it as "CSASPNETShowSpinnerImage". 

Step 2. Add a web form in the root directory of application, name it as 
        "Default.aspx".

Step 3. Add three folders, "Image", "UserControl", "XMLFile". The "Image" folder
        includes image files that you want to show. The "UserControl" folder 
		includes User Controls. The XML file includes XML files as the data
		source of GridView control of Default page.

Step 4. The Default web form page includes an UpdatePanel control and an 
        UpdateProgress control. UpdatePanel Control includes retrieve data button
		and GridView, the UpdateProgeress control includes PopupProgress user
		control. The HTML code of Default page will be like this:
		[code]
		<head id="Head1" runat="server">
        <title></title>
           <style type="text/css"> 
           .modalBackground 
           { 
               background-color: Gray; 
               opacity: 0.7; 
           } 
        </style> 
        </head>
        <body id="body1" runat="server" >
           <form id="form1" runat="server">
           <div>           
              <asp:ToolkitScriptManager ID="ToolkitScriptManagerPopup" runat="server" />      
              <asp:UpdatePanel ID="updatePanel" UpdateMode="Conditional" runat="server"> 
                  <ContentTemplate>  
                      <%=DateTime.Now.ToString() %> 
                      <br /> 
                      <asp:Button ID="btnRefresh" runat="server" Text="Refresh Panel"
					   OnClick="btnRefresh_Click"
					   OnClientClick="document.getElementById('PopupProgressUserControl_btnLink').click();" /> 
                      <br /> 
                      <asp:GridView ID="gvwXMLData" runat="server">
                      </asp:GridView>
                  </ContentTemplate>           
              </asp:UpdatePanel> 
       
        
        
              <asp:UpdateProgress ID="updateProgress" runat="server" AssociatedUpdatePanelID="updatePanel"> 
                  <ProgressTemplate> 
                      <uc1:PopupProgress ID="PopupProgressUserControl" runat="server" />
                  </ProgressTemplate>
              </asp:UpdateProgress>           
           </div>   
           </form>
        </body>
		[/code]
		
Step 5  The btnRefresh Click event code in Default.aspx.cs file.
		[code]
         protected void btnRefresh_Click(object sender, EventArgs e)
         {
            // Here we use Thread.Sleep() to suspends the thread for 10 seconds for imitating
            // an expensive, time-consuming operate of retrieve data. (Such as connect network
            // database to retrieve mass data.)
            // So in practice application, you can remove this line. 
            System.Threading.Thread.Sleep(10000);

            // Retrieve data from XML file as sample data.
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "XMLFile/XMLData.xml");
            DataTable tabXML = new DataTable();
            DataColumn columnName = new DataColumn("Name", Type.GetType("System.String"));
            DataColumn columnAge = new DataColumn("Age", Type.GetType("System.Int32"));
            DataColumn columnCountry = new DataColumn("Country", Type.GetType("System.String"));
            DataColumn columnComment = new DataColumn("Comment", Type.GetType("System.String"));
            tabXML.Columns.Add(columnName);
            tabXML.Columns.Add(columnAge);
            tabXML.Columns.Add(columnCountry);
            tabXML.Columns.Add(columnComment);
            XmlNodeList nodeList = xmlDocument.SelectNodes("Root/Person");
            foreach (XmlNode node in nodeList)
            {
                DataRow row = tabXML.NewRow();
                row["Name"] = node.SelectSingleNode("Name").InnerText;
                row["Age"] = node.SelectSingleNode("Age").InnerText;
                row["Country"] = node.SelectSingleNode("Country").InnerText;
                row["Comment"] = node.SelectSingleNode("Comment").InnerText;
                tabXML.Rows.Add(row);
            }
            gvwXMLData.DataSource = tabXML;
            gvwXMLData.DataBind();
        }
	    [/code]

Step 6. The PopupPregress user control is used to show a popup by ASP.NET Ajax control
        ModalPopupExtender. The ModalPopupExtender can show a Panel when target button
		has been clicked. The HTML code of PopupProgress control as shown below:
		[code]
		<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %> 
        <script language="javascript" type="text/javascript">
        <% =LoadImage() %>
    
        // The JavaScript function can shows loaded image in Image control.
        var imgStep = 0;
        function slide()
        {
            var img = document.getElementById("PopupProgressUserControl_imgProgress");      
            if (document.all)
            {
                img.filters.blendTrans.apply();
            } 
           
            img.title=imgMessage[imgStep];  
            img.src=imgUrl[imgStep]; 
                      
            if (document.all)
            {
                img.filters.blendTrans.play();
            }
        
            imgStep = (imgStep < (imgUrl.length-1)) ? (imgStep + 1) : 0;
            (new Image()).src = imgUrl[imgStep];
        }
        setInterval("slide()",1000);

        </script>
        <asp:Panel ID="pnlProgress" runat="server" CssClass="modalpopup"> 
            <div class="popupcontainerLoading"> 
                <div class="popupbody"> 
                    <table width="100%"> 
                    <tr> 
                        <td align="center"> 
                        <asp:Image ID="imgProgress" runat="server" style="filter: blendTrans(duration=0.618)"  ImageUrl="~/Image/0.jpg"/>                       
                        </td> 
                    </tr> 
                    <tr> 
                        <td>
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" /></td>
                    </tr>
                    </table> 
               </div> 
           </div> 
        </asp:Panel> 
        <asp:LinkButton ID="btnLink" runat="server" Text=""></asp:LinkButton> 
        <asp:ModalPopupExtender ID="mpeProgress" runat="server" TargetControlID="btnLink"
            X="500" Y="0" PopupControlID="pnlProgress" BackgroundCssClass="modalBackground" DropShadow="true"  CancelControlID="btnCancel" > 
        </asp:ModalPopupExtender>
		[/code]

		The PopupProgress.ascx.cs file:

		[code]
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
		[/code]

Step 7. Build the application and you can debug it.


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: ModalPopupExtender experiences
http://weblogs.asp.net/jgonzalez/archive/2007/03/02/modalpopupextender-experiences.aspx

MSDN: ASP.NET User Controls
http://msdn.microsoft.com/en-us/library/y6wb1a0e.aspx

MSDN: XmlDocument Class
http://msdn.microsoft.com/en-us/library/system.xml.xmldocument.aspx


/////////////////////////////////////////////////////////////////////////////