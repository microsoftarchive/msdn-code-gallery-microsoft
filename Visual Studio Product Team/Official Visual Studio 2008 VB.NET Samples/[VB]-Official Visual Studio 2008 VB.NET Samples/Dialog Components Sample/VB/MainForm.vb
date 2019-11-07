' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class MainForm

    Private filename As String

    Private Sub openTextFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles openTextFile.Click
        Try
            With odlgTextFile
                ' See btnRetriveFileNames_Click for explanations of default values 
                ' for the properties.

                ' Check to ensure that the selected file exists.  Dialog box displays 
                ' a warning otherwise.
                .CheckFileExists = True

                ' Check to ensure that the selected path exists.  Dialog box displays 
                ' a warning otherwise.
                .CheckPathExists = True

                ' Get or set default extension. Doesn't include the leading ".".
                .DefaultExt = "txt"

                ' Return the file referenced by a link? If False, simply returns the selected link
                ' file. If True, returns the file linked to the LNK file.
                .DereferenceLinks = True

                ' Just as in VB6, use a set of pairs of filters, separated with "|". Each 
                ' pair consists of a description|file spec. Use a "|" between pairs. No need to put a
                ' trailing "|". You can set the FilterIndex property as well, to select the default
                ' filter. The first filter is numbered 1 (not 0). The default is 1. 
                .Filter = _
                "Text files (*.txt)|*.txt|All files|*.*"

                .Multiselect = False

                ' Restore the original directory when done selecting
                ' a file? If False, the current directory changes
                ' to the directory in which you selected the file.
                ' Set this to True to put the current folder back
                ' where it was when you started.
                ' The default is False.
                .RestoreDirectory = True

                ' Show the Help button and Read-Only checkbox?
                .ShowHelp = True
                .ShowReadOnly = False

                ' Start out with the read-only check box checked?
                ' This only make sense if ShowReadOnly is True.
                .ReadOnlyChecked = False

                .Title = "Select a file to open"

                ' Only accept valid Win32 file names?
                .ValidateNames = True

                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    Try
                        txtFileContents.Text = My.Computer.FileSystem.ReadAllText(.FileName)
                    Catch fileException As Exception
                        Throw fileException
                    End Try
                End If

            End With
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
    End Sub

    Private Sub btnSaveTextFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveTextFile.Click
        Try
            With sdlgTextFile
                ' See the code demonstrating the OpenFileDialog control
                ' for examples using most properties, which are the same
                ' for both controls, for the most part.

                ' Add the default extension, if the user neglects to add an extension.
                ' The default is True.
                .AddExtension = True

                ' Check to verify that the output path actually exists. Prompt before
                ' creating a new file? Prompt before overwriting? 
                ' The default is True.
                .CheckPathExists = True
                ' The default is False.
                .CreatePrompt = False
                ' The default is True.
                .OverwritePrompt = True
                ' The default is True.
                .ValidateNames = True
                ' The default is False.
                .ShowHelp = True

                ' If the user doesn't supply an extension, and if the AddExtension property is
                ' True, use this extension. The default is "".
                .DefaultExt = "txt"

                ' Prompt with the current file name if you've specified it.
                ' The default is "".
                .FileName = filename

                ' The default is "".
                .Filter = _
                "Text files (*.txt)|*.txt|" & _
                "All files|*.*"
                .FilterIndex = 1

                If .ShowDialog() = Windows.Forms.DialogResult.OK Then
                    My.Computer.FileSystem.WriteAllText(.FileName, txtFileContents.Text, False)
                End If

            End With
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
    End Sub

    Private Sub btnSelectColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectColor.Click
        ' Initialize CustomColors with an array of integers.
        ' Note that these colors aren't in the same format 
        ' as .NET colors -- these are the old VB6-type
        ' color values. For example, 0 is Black, 255 is Red, 
        ' and so on. You can use the VB6 RGB function to create 
        ' these values.

        ' For this example, put Red, Green, and Blue in the custom 
        ' colors section.
        Static CustomColors() As Integer = _
         {RGB(255, 0, 0), RGB(0, 255, 0), RGB(0, 0, 255)}

        Try
            With cdlgText
                ' Initialize the selected color to match the color currently used
                ' by the TextBox's foreground color.
                ' The default is Black.
                .Color = txtFileContents.ForeColor

                ' Fill the custom colors on the dialog box with the array you've supplied. 
                .CustomColors = CustomColors

                ' Allow the user to create custom colors?
                ' The default is True.
                .AllowFullOpen = True

                ' Display all of the basic colors?
                ' The default is False.
                .AnyColor = True

                ' If True, dialog box displays with the custom 
                ' color settings side open, as well.
                ' The default is False.
                .FullOpen = False

                ' Restrict users to solid colors only.
                ' The default is False.
                .SolidColorOnly = True

                ' If ShowHelp is true, the control will display its Help
                ' button, and you can react to the control's HelpRequest event. 
                ' The default is False.
                .ShowHelp = True

                If .ShowDialog() = Windows.Forms.DialogResult.OK Then
                    txtFileContents.ForeColor = .Color

                    ' Store the custom colors for future use. 
                    CustomColors = .CustomColors
                End If

                ' Reset all the colors in the dialog box.
                ' This isn't necessary, but it does make sure the dialog box is in a 
                ' known state for its next use.
                cdlgText.Reset()

            End With

            ' You can get away with much less code. 
            ' Minimally, this will set the color
            ' for you -- just won't give you any options:
            'With cdlgText
            '  .Color = txtFileContents.ForeColor
            '  If .ShowDialog() = DialogResult.OK Then
            '    txtFileContents.ForeColor = .Color
            '  End If
            'End With

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
    End Sub

    Private Sub btnSelectFont_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectFont.Click
        Try
            With fdlgText
                ' Initialize the dialog box to match
                ' the font used in the text box.

                .Font = txtFileContents.Font
                ' The default is Black.
                .Color = txtFileContents.ForeColor

                ' Allow the users to select colors.
                ' The default is False.
                .ShowColor = True

                ' Show the Apply button on the dialog box.
                ' The default is False.
                .ShowApply = True

                ' Allow users to select effects.
                ' The default is True.
                .ShowEffects = True

                ' Show the Help button.
                ' The default is False.
                .ShowHelp = True

                ' Let the user change the character set, 
                ' but don't allow simulations, vector fonts, 
                ' or vertical fonts.
                ' The default is True.
                .AllowScriptChange = True
                ' The default is True.
                .AllowSimulations = False
                ' The default is True.
                .AllowVectorFonts = False
                ' The default is True.
                .AllowVerticalFonts = False

                ' Allow fixed and proportional fonts, 
                ' and only allow fonts that exist.
                ' Set the minimum and maximum selectable
                ' font sizes, as well. These are arbitrary
                ' values, in this example.

                ' The default is False.
                .FixedPitchOnly = False
                ' The default is False.
                .FontMustExist = True

                ' The default for both these is 0.
                .MaxSize = 48
                .MinSize = 8

                ' Display the dialog box, and then
                ' fix up the font as requested.
                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    txtFileContents.Font = .Font
                End If

            End With
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
    End Sub

    Private Sub btnBrowseFolders_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowseFolders.Click
        Try
            With fldlgList
                'Typically, after creating a new FolderBrowserDialog, 
                ' you set the RootFolder to the location from which 
                ' to start browsing. 
                ' You can choose from the list of system special folders, 
                ' which are folders such as Program Files, Programs, 
                ' System, or Startup, which contain common information.  
                .RootFolder = Environment.SpecialFolder.Personal

                ' You can also optionally set the Description property to 
                ' provide additional instructions to the user.
                .Description = "Select the directory you want to use as the default."

                ' You can use the ShowNewFolderButton property to control 
                ' if the user is able to create new folders via the New Folder button.
                .ShowNewFolderButton = True

                If .ShowDialog = Windows.Forms.DialogResult.OK Then
                    txtDirectory.Text = .SelectedPath
                End If

            End With
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
    End Sub

    Private Sub btnRetriveFileNames_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRetriveFileNames.Click
        Try
            With odlgFileNames
                ' You may not always want to do this, but you can set the initial directory.
                ' In this example, the code only sets this property if you've 
                ' set the DefaultFolder value since the last time opening 
                ' the dialog box. If you don't call the Reset method, your
                ' initial directory won't affect the dialog box, if you've already
                ' selected a file using it.
                .Reset()
                .InitialDirectory = My.Computer.FileSystem.SpecialDirectories.Temp

                ' Add default extension to file name if the user doesn't type it.
                ' The default is True.
                .AddExtension = True

                ' Check to ensure that the selected file exists.  Dialog box displays 
                ' a warning otherwise.
                ' The default is True.
                .CheckFileExists = True

                ' Check to ensure that the selected path exists.  Dialog box displays 
                ' a warning otherwise.
                ' The default is True.
                .CheckPathExists = True

                ' Get or set default extension. Doesn't include the leading ".".
                ' The default is "".
                .DefaultExt = "txt"

                ' Return the file referenced by a link? If False, simply returns the selected link
                ' file. If True, returns the file linked to the LNK file.
                ' The default is True.
                .DereferenceLinks = True

                ' Just as in VB6, use a set of pairs of filters, separated with "|". Each 
                ' pair consists of a description|file spec. Use a "|" between pairs. No need to put a
                ' trailing "|". You can set the FilterIndex property as well, to select the default
                ' filter. The first filter is  numbered 1 (not 0). The default is 1. 
                ' The default is "".
                .Filter = _
                "Text files (*.txt)|*.txt|" & _
                "All files|*.*"

                ' If you want to allow users to select more than one file, set this to True. 
                ' If you set this to True, retrieve the selected files using the FileNames
                ' property of the dialog box.
                ' The default is False.
                .Multiselect = True

                ' Restore the original directory when done selecting
                ' a file? If False, the current directory changes
                ' to the directory in which you selected the file.
                ' Set this to True to put the current folder back
                ' where it was when you started.
                ' The default is False.
                .RestoreDirectory = True

                ' Show the Help button and Read-Only checkbox?
                ' The Default for each is False.
                .ShowHelp = True
                .ShowReadOnly = False

                ' Start out with the read-only check box checked?
                ' This only make sense if ShowReadOnly is True.
                ' The default is False.
                ' .ReadOnlyChecked = False

                ' The default is "".
                .Title = "Select a file"

                ' Only accept valid Win32 file names?
                ' The default is True.
                .ValidateNames = True

                If .ShowDialog() = Windows.Forms.DialogResult.OK Then
                    ' You have a choice here. You can either
                    ' use the FileName or FileNames properties to get the name 
                    ' you selected, or you can use the OpenFile
                    ' method to open the file as a read-only Stream.
                    lstFiles.DataSource = .FileNames

                    ' You could also write code like this, 
                    ' to loop through the selected file names:
                    'Dim strName As String
                    'For Each strName In .FileNames
                    '    lstFiles.Items.Add(strName)
                    'Next
                End If

            End With
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, Me.Text)
        End Try
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        End
    End Sub
End Class
