Public Class frmPredictor

    Dim svc As New PredictorService.PredictorServiceClient

    Private Sub btnAsk_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAsk.Click
        lblAnswer.Text = svc.Ask(txtQuestion.Text)
    End Sub

End Class
