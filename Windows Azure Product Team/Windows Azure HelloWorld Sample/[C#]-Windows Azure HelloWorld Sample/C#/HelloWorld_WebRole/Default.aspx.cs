//
// <copyright file="Default.aspx.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
using System;
using System.Web.UI.WebControls;


namespace Microsoft.Samples.ServiceHosting.HelloWorld
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = "Hello World!";
            Label2.Text = "To get started creating applications for Windows Azure, see:";
            HyperLink1.Text = "Windows Azure Hands On Labs";
            HyperLink1.NavigateUrl = "http://msdn.microsoft.com/en-us/windowsazure/wazplatformtrainingcourse_windowsazure_unit";
            HyperLink2.Text = "Windows Azure Code Samples";
            HyperLink2.NavigateUrl = "http://msdn.microsoft.com/en-us/library/windows-azure-code-samples.aspx";
            HyperLink3.Text = "Windows Azure Code Quick Start";
            HyperLink3.NavigateUrl = "http://msdn.microsoft.com/en-us/library/gg663908.aspx";

        }
    }
}
