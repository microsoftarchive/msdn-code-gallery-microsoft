Public Class frmPredictor

    'For ASMX Services, a SOAP or SOAP1.2 endpoint must be selected 
    Dim svc As New PredictorService.PredictorServiceSoapClient()

    Private Sub btnAsk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAsk.Click
        lblAnswer.Text = svc.Ask(txtQuestion.Text)
    End Sub

End Class
