using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//Add this namespace because it includes the CredentialCache class
using System.Net;
//Add this namespace because we will get the chart as a stream
using System.IO;

namespace REST_ViewingAChart
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //In this example, we'll use the ExcelRest service 
            //to generate a PNG file of an Excel Chart and display
            //it in an PictreBox control.

            //Start by formulating the URL to the ExcelRest service.
            string Url = "http://intranet.contoso.com/_vti_bin/ExcelRest.aspx";
            //Add the path, relative to the site, where the spreadsheet is stored
            Url += "/shared%20documents/cyclepartsales.xlsx";
            //Add the path to the chart and specify that we want an image
            Url += "/model/Charts('Chart%201')?$format=image";
            //Let's show the user the full URL
            label1.Text = Url;
            //We cannot simply set the pictureBox's ImageLocation property to this URL
            //This is because this method does not authenticate with SharePoint 
            //Instead, create a WebRequest object with the URL we just formulated
            WebRequest chartRequest = WebRequest.Create(Url);
            //Now we can specify credentials, in this case those of the current user
            chartRequest.Credentials = CredentialCache.DefaultCredentials;
            //Execute the request and store the response
            WebResponse response = chartRequest.GetResponse();
            //Save the response as a stream
            Stream chartStream = response.GetResponseStream();
            //Create an Image object from the stream
            Image chartImage = Image.FromStream(chartStream);
            //We can load this image into the pictureBox
            pictureBox1.Image = chartImage;
        }
    }
}
