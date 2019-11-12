Imports System
Imports System.Configuration
Imports System.ServiceModel
Imports System.ServiceModel.PeerResolvers

Namespace Microsoft.ServiceModel.Samples

    Public Class Form1
        Implements IChat

        Private member As String
        Private instanceContext As InstanceContext
        Private participant As IChatChannel
        Private ostat As IOnlineStatus
        Private factory As DuplexChannelFactory(Of IChatChannel)

        Public Sub New()
            ' This call is required by the Windows Form Designer.
            InitializeComponent()
            ' Add any initialization after the InitializeComponent() call.
        End Sub
        Public Sub New(ByVal member As String)
            Me.member = member
            InitializeComponent()
        End Sub

        Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
            If RichTextBox2.Text <> "" Then
                participant.Chat(member, RichTextBox2.Text)
                RichTextBox2.Text = ""
            End If
        End Sub

        Private Sub Connect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Connect.Click

            If TextBox1.Text <> "" Then
                member = TextBox1.Text
            Else
                member = "DefaultName"
            End If
            RichTextBox2.Text = "Message"
            RichTextBox1.Text = member + " is ready"
            RichTextBox1.AppendText(Chr(10) + "Type chat messages after going Online")
            RichTextBox1.Visible = True
            RichTextBox2.Visible = True
            Button1.Visible = True
            Connect.Visible = False
            TextBox1.Visible = False

            ' Construct InstanceContext to handle messages on callback interface. 
            ' An instance of ChatApp is created and passed to the InstanceContext.
            instanceContext = New InstanceContext(Me)

            ' Create the participant with the given endpoint configuration
            ' Each participant opens a duplex channel to the mesh
            ' participant is an instance of the chat application that has opened a channel to the mesh
            factory = New DuplexChannelFactory(Of IChatChannel)(instanceContext, "ChatEndpoint")
            participant = factory.CreateChannel()

            ' Retrieve the PeerNode associated with the participant and register for online/offline events
            ' PeerNode represents a node in the mesh. Mesh is the named collection of connected nodes.
            ostat = participant.GetProperty(Of IOnlineStatus)()
            AddHandler ostat.Online, AddressOf Me.OnOnline
            AddHandler ostat.Offline, AddressOf Me.OnOffline

            Try

                participant.Open()

            Catch generatedExceptionName As CommunicationException

                MsgBox("Could not find resolver.If you are using a custom resolver, please ensure that the service is running before executing this sample.  Refer to the readme for more details.")
                Return

            End Try


            ' Announce self to other participants
            participant.Join(member)

            ' loop until the user quits
            ' Leave the mesh
        End Sub

        Public Sub Chat(ByVal member As String, ByVal msg As String) Implements IChat.Chat

            RichTextBox1.AppendText(Chr(10) + "[" + member + "] " + msg)

        End Sub

        Public Sub Join(ByVal member As String) Implements IChat.Join
            RichTextBox1.AppendText(Chr(10) + "[" + member + " joined] ")

        End Sub

        Public Sub Leave1(ByVal member As String) Implements IChat.Leave
            RichTextBox1.AppendText(Chr(10) + "[" + member + " left] ")

        End Sub

        Public Sub OnOnline(ByVal sender As Object, ByVal e As EventArgs)
            RichTextBox1.AppendText(Chr(10) + "** Online")


        End Sub

        Public Sub OnOffline(ByVal sender As Object, ByVal e As EventArgs)
            RichTextBox1.AppendText(Chr(10) + "** Offline")


        End Sub

    End Class
End Namespace
