' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Net
Imports System.Data


Public Class MainForm

    Const DataRetrievalError As String = _
        "An error occurred trying to retrieve the data from the Web Service." & _
        "The Web service may currently be down. You might attempt to access " & _
        "it directly: "


    ' Calls the EuroConvert Web service to get a DataSet of all the possible
    ' currencies that can be converted from the Euro dollar. The DataSet is then
    ' bound to a ComboBox for use on the Currency tab.
    Private Sub LoadCurrencyConverterComboBox()
        ' Create and fill a DataSet from an XML document.
        Dim dsCountries As New DataSet()

        Try
            dsCountries.ReadXml(New System.IO.StringReader(My.Resources.ecc_countries))
        Catch exp As Exception
            MsgBox("Error loading DataSet from XML: ", MsgBoxStyle.OKOnly, Me.Text)
            Exit Sub
        End Try

        ' Bind the DataSet to the ComboBox for display.
        With cboConvertTo
            .DataSource = dsCountries.Tables(0)
            .DisplayMember = "Description"
            .ValueMember = "Currency"
        End With
    End Sub


    ' Turns off the status indicators activated by ShowStatusIndicators().
    Private Sub ResetStatusIndicators()
        ' Reset the status indicators, no matter what happens.
        Me.Cursor = Cursors.Default
        'frmStatus.Hide()
    End Sub

    ' Displays various Web service connection and data status indicators for UI 
    ' feedback.
    Private Sub ShowStatusIndicators()
        ' Display appropriate cursor. Make sure you call DoEvents()
        ' or this code will not run until the entire Click event is processed, at 
        ' which point it will not matter (nor will it even be visible).
        Me.Cursor = Cursors.WaitCursor
        Application.DoEvents()
    End Sub

    ' Handles the "Convert" button click event for the Currency tab. This handler 
    ' connects to a Web service, passes in the country that the currency is being
    ' converted from and the country the currency is being converted to, and 
    ' returns the exchange rate. This is then used to calculate the final value.
    Private Sub btnConvert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConvert.Click

        ' Create an instance of the Web service proxy class.
        Dim wsEuroConverter As New WebServiceClient.ConvertEuros.IEuroservice

        ShowStatusIndicators()

        ' Retrieve the data from the Web service.
        Try
            lblConvertedAmount.Text = wsEuroConverter.FromEuro(cboConvertTo.SelectedValue.ToString, CLng(txtAmount.Text)).ToString
        Catch exp As Exception
            MessageBox.Show(DataRetrievalError & _
                wsEuroConverter.Url, _
                "Web Service Demo Error", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        Finally
            ResetStatusIndicators()
        End Try
    End Sub

    ' Handles the "Get Cartoon!" button click event for the Diblert tab. 
    Private Sub btnCartoon_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCartoon.Click

        ' Create an instance of the Web service proxy class.
        Dim wsDailyDiblert As New WebServiceClient.DailyDiblert.DailyDilbert

        ' Display a status form so that the delay caused by accessing the Web 
        ' service is not mistaken as the Form not loading properly or some other
        ' problem.
        ShowStatusIndicators()

        ' Retrieve the data from the Web service. The Web service also exposes a 
        ' method that returns a Url to the image, but using that method here would 
        ' not have been as instructive.
        Try
            ' Call the Web method asynchronously.
            wsDailyDiblert.DailyDilbertImageAsync()
            AddHandler wsDailyDiblert.DailyDilbertImageCompleted, AddressOf DiblertCompleted
        Catch exp As Exception
            MessageBox.Show(DataRetrievalError & _
                wsDailyDiblert.Url, _
                "Web Service Demo Error", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        Finally
            ' Hide the status form, whether the cartoon is retrieved or an error
            ' occurred.
            ResetStatusIndicators()
        End Try
    End Sub

    Sub DiblertCompleted(ByVal sender As Object, ByVal e As WebServiceClient.DailyDiblert.DailyDilbertImageCompletedEventArgs)
        Try
            Dim ms As New System.IO.MemoryStream(e.Result)

            ' Display in the PictureBox control with a simple call to the 
            ' FromStream method.
            With picDilbert
                .Image = Image.FromStream(ms)
                .SizeMode = PictureBoxSizeMode.CenterImage
                .BorderStyle = BorderStyle.Fixed3D
            End With
        Catch ex As Exception
            MessageBox.Show("The cartoon was not retrieved in the " & _
                "time you specified.", "Web Service Demo Information", _
                MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try
    End Sub

    ' Handles "Get Data" button click event for the Books tab. This handler connects
    ' to the SalesRankNPrice Web service and downloads Amazon and Barnes & Noble 
    ' sales ranking and price for a given book by ISBN number.
    Private Sub btnGetBookData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetBookData.Click

        ' Create an instance of the Web service proxy class and the All class, also
        ' provided by the Web service as a convenient type for holding the data
        ' returned by the GetAll method.
        Dim salesRank As New WebServiceClient.BandN.BNPrice
        Dim price As String

        ShowStatusIndicators()

        ' Retrieve the data from the Web service.
        Try
            price = salesRank.GetBNQuote(txtISBN.Text)
        Catch exp As Exception
            MsgBox(DataRetrievalError & salesRank.Url, MsgBoxStyle.OKOnly, Me.Text)
            Exit Sub
        Finally
            ' Reset the status indicators, no matter what happens.
            ResetStatusIndicators()
        End Try

        ' Create a ListViewItem object and set the first column's text.
        Dim lvItem As New ListViewItem()
        lvItem.Text = txtISBN.Text

        ' Set the text in the remaining columns and add the ListViewItem object
        ' to the ListView.
        With lvItem.SubItems
            .Add(price)
        End With
        lvwBooks.Items.Add(lvItem)
    End Sub

    ' Handles "Get Time" button click event for the Local Time tab. This handler 
    ' connects to the Local Time Web service and downloads the date and time for a 
    ' given zip code.
    Private Sub btnGetTime_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetTime.Click

        ' Create an instance of the Web service proxy class.
        Dim wsLocalTime As New WebServiceClient.LocalTime.LocalTime

        ShowStatusIndicators()

        Try
            ' Retrieve the time from the Web service.
            lblTime.Text = wsLocalTime.LocalTimeByZipCode(txtZipCodeForTime.Text)
        Catch exp As Exception
            MsgBox(DataRetrievalError & wsLocalTime.Url, MsgBoxStyle.OkOnly, Me.Text)
            Exit Sub
        Finally
            ' Reset the status indicators, no matter what happens.
            ResetStatusIndicators()
        End Try

    End Sub


    ' Handles the Form's load event which binds a ComboBox used on the Currency
    ' tab.
    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LoadCurrencyConverterComboBox()
    End Sub


    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
