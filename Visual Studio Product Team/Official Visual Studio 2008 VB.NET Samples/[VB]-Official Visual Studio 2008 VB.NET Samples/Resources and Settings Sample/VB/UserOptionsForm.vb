''' <summary>
'''  Simple Tools/Options form that allows users to change the dont, foreground and background colors for the application.
''' </summary>
Public Class UserOptionsForm

    Private m_newFont As Font
    Private m_newBackColor As Color
    Private m_newForeColor As Color

    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        InitializeSettings()
        ResetControlValues()
    End Sub

    ' We should be able to use 2-way data binding for this, but it does not seem to work in this build...
    Private Sub ResetControlValues()

        Me.FontNameTextBox.Text = m_newFont.Name & ", " & m_newFont.SizeInPoints
        Me.ForeColorLabel.BackColor = m_newForeColor
        Me.BackColorLabel.BackColor = m_newBackColor

        ' Update the preview text box
        Me.OptionsPreviewTextBox.ForeColor = m_newForeColor
        Me.OptionsPreviewTextBox.BackColor = m_newBackColor
        Me.OptionsPreviewTextBox.Font = m_newFont
    End Sub


    ''' <summary>
    ''' If the selected color has changed, update the controls
    ''' </summary>
    Private Sub FontSelectButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FontSelectButton.Click
        Dim fontDialog As New FontDialog()
        fontDialog.Font = m_newFont
        If (fontDialog.ShowDialog() = Windows.Forms.DialogResult.OK) Then
            m_newFont = fontDialog.Font
            ResetControlValues()
        End If
    End Sub

    ''' <summary>
    ''' If the user selects okay, update My.Settings with the new settings values
    ''' </summary>
    Private Sub Ok_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Ok_Button.Click
        With My.Settings
            .BackColor = m_newBackColor
            .Font = m_newFont
            .ForeColor = m_newForeColor
        End With
        Me.Close()
    End Sub

    ''' <summary>
    ''' Do not save the settings changes.
    ''' </summary>
    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.Close()
    End Sub

    ''' <summary>
    ''' Update the background color
    ''' </summary>
    Private Sub SelectBackColorButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SelectBackColorButton.Click
        Dim colorPicker As New ColorDialog()
        colorPicker.Color = Me.m_newBackColor
        If (colorPicker.ShowDialog() = Windows.Forms.DialogResult.OK) Then
            m_newBackColor = colorPicker.Color
            ResetControlValues()
        End If
    End Sub

    ''' <summary>
    ''' Update the foreground color
    ''' </summary>
    Private Sub SelectForeGroundColorButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SelectForeGroundColorButton.Click
        Dim colorPicker As New ColorDialog()
        colorPicker.Color = Me.m_newForeColor
        If (colorPicker.ShowDialog() = Windows.Forms.DialogResult.OK) Then
            m_newForeColor = colorPicker.Color
            ResetControlValues()
        End If

    End Sub

    Private Sub InitializeSettings()
        m_newFont = My.Settings.Font
        m_newBackColor = My.Settings.BackColor
        m_newForeColor = My.Settings.ForeColor
    End Sub
End Class
