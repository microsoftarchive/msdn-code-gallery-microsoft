Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.PeerResolvers
Imports System.Configuration

    Public Class Form1

        Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim crs As New CustomPeerResolverService()
        crs.ControlShape = False

        ' Create a new service host
        Dim customResolver As New ServiceHost(crs)

        ' Open the custom resolver service and wait for user to press enter
        crs.Open()
        customResolver.Open()
        RichTextBox1.Text = "Custom resolver service is started"
        RichTextBox1.AppendText(" Press 'Stop Service' button to terminate service")

        End Sub

    Private Sub button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles button1.Click
        End
    End Sub
End Class

