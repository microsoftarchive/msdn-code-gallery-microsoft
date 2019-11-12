'Add this namespace because it includes the CredentialCache class
Imports System.Net
'Add this namespace because we will get the chart as a stream
Imports System.IO

Public Class Form1

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'In this example, we'll use the ExcelRest service 
        'to generate a PNG file of an Excel Chart and display
        'it in an PictreBox control.

        'Start by formulating the URL to the ExcelRest service.
        Dim Url As String = "http://intranet.contoso.com/_vti_bin/ExcelRest.aspx"
        'Add the path, relative to the site, where the spreadsheet is stored
        Url += "/shared%20documents/cyclepartsales.xlsx"
        'Add the path to the chart and specify that we want an image
        Url += "/model/Charts('Chart%201')?$format=image"
        'Let's show the user the full URL
        Label1.Text = Url
        'We cannot simply set the pictureBox's ImageLocation property to this URL
        'This is because this method does not authenticate with SharePoint 
        'Instead, create a WebRequest object with the URL we just formulated
        Dim chartRequest As WebRequest = WebRequest.Create(Url)
        'Now we can specify credentials, in this case those of the current user
        chartRequest.Credentials = CredentialCache.DefaultCredentials
        'Execute the request and store the response
        Dim response As WebResponse = chartRequest.GetResponse()
        'Save the response as a stream
        Dim chartStream As Stream = response.GetResponseStream()
        'Create an Image object from the stream
        Dim chartImage As Image = Image.FromStream(chartStream)
        'We can load this image into the pictureBox
        PictureBox1.Image = chartImage
    End Sub

End Class
