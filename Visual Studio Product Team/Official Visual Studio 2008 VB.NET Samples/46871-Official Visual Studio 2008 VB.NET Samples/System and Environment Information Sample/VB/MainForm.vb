' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Environment

Public Class MainForm

#Region " Enum Handling "
    Private Sub LoadList(ByVal lst As ListBox, ByVal typ As Type)
        lst.DataSource = System.Enum.GetNames(typ)
    End Sub

    Private Function GetSpecialFolderFromList() As Environment.SpecialFolder
        ' lstFolders.SelectedItem returns the name of the 
        ' special folder. System.Enum.Parse will turn that
        ' into an object corresponding to the enumerated value
        ' matching the specific text. CType then converts the
        ' object into an Environment.SpecialFolder object.
        ' This is all required because Option Strict is on.
        Return CType( _
        System.Enum.Parse(GetType(Environment.SpecialFolder), _
        lstFolders.SelectedItem.ToString), _
        Environment.SpecialFolder)
    End Function

    Private Sub LoadList(ByVal lst As ListBox, ByVal ic As ICollection)
        ' This procedure sets the data source 
        ' for a list box to be the contents
        ' of an object that implements
        ' the ICollection interface. You must
        ' first copy the contents to something
        ' that implements IList (such as an array)
        ' and then bind to that.
        Dim astrItems(ic.Count - 1) As String
        ic.CopyTo(astrItems, 0)
        lst.DataSource = astrItems
    End Sub
#End Region

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Environment.Exit(CInt(nudExitCode.Value))
    End Sub

    Private Sub btnExpand_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExpand.Click
        lblExpandResults.Text = Environment.ExpandEnvironmentVariables(txtExpand.Text)
    End Sub

    Private Sub btnGetEnvironmentVariable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetEnvironmentVariable.Click
        lblTEMP.Text = Environment.GetEnvironmentVariable("TEMP")
    End Sub

    Private Sub btnGetSystemFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetSystemFolder.Click
        ' If you just want to retrieve a single 
        ' special folder path, pass in one of the
        ' SpecialFolder enumeration values.
        ' GetFolderPath is actually System.Environment.GetFolderPath.
        ' See the Imports statement at the top of this file.
        lblSystemFolder.Text = GetFolderPath(SpecialFolder.System)
    End Sub

    Private Sub btnRefreshTickCount_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRefreshTickCount.Click
        lblTickCount.Text = Environment.TickCount.ToString
    End Sub

    Private Sub btnStackTrace_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStackTrace.Click
        Try
            MsgBox(Environment.StackTrace, MsgBoxStyle.OKOnly, Me.Text)
        Catch exp As System.Security.SecurityException
            MsgBox("A security exception occurred." & vbCrLf & exp.Message, MsgBoxStyle.Critical, exp.Source)
        Catch exp As System.Exception
            MsgBox("An unexpected error occurred accessing the StackTrace." & vbCrLf & exp.Message, MsgBoxStyle.Critical, exp.Source)
        End Try
    End Sub

    Private Sub frmEnvironment_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' The LoadList procedure is in 
        ' the "Enum Handling" region above.
        ' It loads a list box with all the 
        ' elements of an enumeration. If you're
        ' interested, give it a look. You could load
        ' the list with items one at a time, but this
        ' technique is far simpler.
        LoadList(lstFolders, GetType(Environment.SpecialFolder))

        ' LoadList (in the "Enum Handling" 
        ' region) also loads a list box given
        ' an ICollection object. The alternative would be 
        ' to loop through all the elements of the collection.
        LoadList(lstEnvironmentVariables, GetEnvironmentVariables.Keys)

        ' Display properties on the Properties tab.
        LoadProperties()

        ' Set up simple methods.
        RunMethods()

        ' Load values from SystemInformation class
        LoadSystemInformation()
    End Sub

    Private Sub lstEnvironmentVariables_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstEnvironmentVariables.SelectedIndexChanged
        ' GetEnvironmentVariable is actually 
        ' System.Environment.GetEnvironmentVariable.
        ' lstEnvironmentVariables contains a list
        ' of all the current environment variables.
        ' GetEnvironmentVariable returns the value
        ' associated with an environment variable.
        lblEnvironmentVariable.Text = _
        GetEnvironmentVariable(lstEnvironmentVariables.SelectedItem.ToString)
    End Sub

    Private Sub lstFolders_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lstFolders.SelectedIndexChanged
        Dim sf As Environment.SpecialFolder

        ' GetSpecialFolderFromList is a method
        ' in the hidden "Enum Handling" region 
        ' above. It returns a member of the
        ' Environment.SpecialFolder enumeration, 
        ' and is specific to this demonstration.
        sf = GetSpecialFolderFromList()

        ' GetFolderPath is a method provided by 
        ' the System.Environment namespace.

        ' Specifically, you could call the GetFolderPath
        ' method like this:
        ' YourPath = GetFolderPath(SpecialFolder.Favorites)

        ' GetFolderPath is actually System.Environment.GetFolderPath.
        ' See the Imports statement at the top of this file.
        lblSpecialFolder.Text = GetFolderPath(sf)
    End Sub

    Private Sub lvwSystemInformation_Resize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lvwSystemInformation.Resize
        lvwSystemInformation.Columns(1).Width = _
        lvwSystemInformation.ClientRectangle.Width - lvwSystemInformation.Columns(0).Width
    End Sub

    Private Sub LoadProperties()
        ' This procedure fills in items
        ' on the Properties tab. These are
        ' most of the properties provided by the 
        ' Environment class.
        lblWorkingSet.Text = Environment.WorkingSet.ToString
        lblVersion.Text = Environment.Version.ToString
        lblUserName.Text = Environment.UserName
        lblUserDomainName.Text = Environment.UserDomainName
        lblTickCount.Text = Environment.TickCount.ToString
        lblSystemDirectory.Text = Environment.SystemDirectory
        lblOSVersion.Text = Environment.OSVersion.ToString
        lblMachineName.Text = Environment.MachineName
        lblCurrentDirectory.Text = Environment.CurrentDirectory
        lblCommandLine.Text = Environment.CommandLine
    End Sub

    Private Sub LoadSystemInformation()
        ' Add information about the Shared properties
        ' of the SystemInformation class to a ListView control.
        ' The text for the item is the name 
        ' of the property provided by the 
        ' SystemInformation class. The first (and only)
        ' subitem is the value of the property.
        ' See help on the SystemInformation class
        ' for more details about each individual 
        ' property.
        With lvwSystemInformation.Items.Add("ArrangeDirection")
            .SubItems.Add(SystemInformation.ArrangeDirection.ToString())
        End With
        With lvwSystemInformation.Items.Add("ArrangeStartingPosition")
            .SubItems.Add(SystemInformation.ArrangeStartingPosition.ToString())
        End With
        With lvwSystemInformation.Items.Add("BootMode")
            .SubItems.Add(SystemInformation.BootMode.ToString())
        End With
        With lvwSystemInformation.Items.Add("Border3DSize")
            .SubItems.Add(SystemInformation.Border3DSize.ToString())
        End With
        With lvwSystemInformation.Items.Add("BorderSize")
            .SubItems.Add(SystemInformation.BorderSize.ToString())
        End With
        With lvwSystemInformation.Items.Add("CaptionButtonSize")
            .SubItems.Add(SystemInformation.CaptionButtonSize.ToString())
        End With
        With lvwSystemInformation.Items.Add("CaptionHeight")
            .SubItems.Add(SystemInformation.CaptionHeight.ToString())
        End With
        With lvwSystemInformation.Items.Add("ComputerName")
            .SubItems.Add(SystemInformation.ComputerName.ToString())
        End With
        With lvwSystemInformation.Items.Add("CursorSize")
            .SubItems.Add(SystemInformation.CursorSize.ToString())
        End With
        With lvwSystemInformation.Items.Add("DbcsEnabled")
            .SubItems.Add(SystemInformation.DbcsEnabled.ToString())
        End With
        With lvwSystemInformation.Items.Add("DebugOS")
            .SubItems.Add(SystemInformation.DebugOS.ToString())
        End With
        With lvwSystemInformation.Items.Add("DoubleClickSize")
            .SubItems.Add(SystemInformation.DoubleClickSize.ToString())
        End With
        With lvwSystemInformation.Items.Add("DoubleClickTime")
            .SubItems.Add(SystemInformation.DoubleClickTime.ToString())
        End With
        With lvwSystemInformation.Items.Add("DragFullWindows")
            .SubItems.Add(SystemInformation.DragFullWindows.ToString())
        End With
        With lvwSystemInformation.Items.Add("DragSize")
            .SubItems.Add(SystemInformation.DragSize.ToString())
        End With
        With lvwSystemInformation.Items.Add("FixedFrameBorderSize")
            .SubItems.Add(SystemInformation.FixedFrameBorderSize.ToString())
        End With
        With lvwSystemInformation.Items.Add("FrameBorderSize")
            .SubItems.Add(SystemInformation.FrameBorderSize.ToString())
        End With
        With lvwSystemInformation.Items.Add("HighContrast")
            .SubItems.Add(SystemInformation.HighContrast.ToString())
        End With
        With lvwSystemInformation.Items.Add("HorizontalScrollBarArrowWidth")
            .SubItems.Add(SystemInformation.HorizontalScrollBarArrowWidth.ToString())
        End With
        With lvwSystemInformation.Items.Add("HorizontalScrollBarHeight")
            .SubItems.Add(SystemInformation.HorizontalScrollBarHeight.ToString())
        End With
        With lvwSystemInformation.Items.Add("HorizontalScrollBarThumbWidth")
            .SubItems.Add(SystemInformation.HorizontalScrollBarThumbWidth.ToString())
        End With
        With lvwSystemInformation.Items.Add("IconSize")
            .SubItems.Add(SystemInformation.IconSize.ToString())
        End With
        With lvwSystemInformation.Items.Add("IconSpacingSize")
            .SubItems.Add(SystemInformation.IconSpacingSize.ToString())
        End With
        With lvwSystemInformation.Items.Add("KanjiWindowHeight")
            .SubItems.Add(SystemInformation.KanjiWindowHeight.ToString())
        End With
        With lvwSystemInformation.Items.Add("MaxWindowTrackSize")
            .SubItems.Add(SystemInformation.MaxWindowTrackSize.ToString())
        End With
        With lvwSystemInformation.Items.Add("MenuButtonSize")
            .SubItems.Add(SystemInformation.MenuButtonSize.ToString())
        End With
        With lvwSystemInformation.Items.Add("MenuCheckSize")
            .SubItems.Add(SystemInformation.MenuCheckSize.ToString())
        End With
        With lvwSystemInformation.Items.Add("MenuFont")
            .SubItems.Add(SystemInformation.MenuFont.ToString())
        End With
        With lvwSystemInformation.Items.Add("MenuHeight")
            .SubItems.Add(SystemInformation.MenuHeight.ToString())
        End With
        With lvwSystemInformation.Items.Add("MidEastEnabled")
            .SubItems.Add(SystemInformation.MidEastEnabled.ToString())
        End With
        With lvwSystemInformation.Items.Add("MinimizedWindowSize")
            .SubItems.Add(SystemInformation.MinimizedWindowSize.ToString())
        End With
        With lvwSystemInformation.Items.Add("MinimizedWindowSpacingSize")
            .SubItems.Add(SystemInformation.MinimizedWindowSpacingSize.ToString())
        End With
        With lvwSystemInformation.Items.Add("MinimumWindowSize")
            .SubItems.Add(SystemInformation.MinimumWindowSize.ToString())
        End With
        With lvwSystemInformation.Items.Add("MinWindowTrackSize")
            .SubItems.Add(SystemInformation.MinWindowTrackSize.ToString())
        End With
        With lvwSystemInformation.Items.Add("MonitorCount")
            .SubItems.Add(SystemInformation.MonitorCount.ToString())
        End With
        With lvwSystemInformation.Items.Add("MonitorsSameDisplayFormat")
            .SubItems.Add(SystemInformation.MonitorsSameDisplayFormat.ToString())
        End With
        With lvwSystemInformation.Items.Add("MouseButtons")
            .SubItems.Add(SystemInformation.MouseButtons.ToString())
        End With
        With lvwSystemInformation.Items.Add("MouseButtonsSwapped")
            .SubItems.Add(SystemInformation.MouseButtonsSwapped.ToString())
        End With
        With lvwSystemInformation.Items.Add("MousePresent")
            .SubItems.Add(SystemInformation.MousePresent.ToString())
        End With
        With lvwSystemInformation.Items.Add("MouseWheelPresent")
            .SubItems.Add(SystemInformation.MouseWheelPresent.ToString())
        End With
        With lvwSystemInformation.Items.Add("MouseWheelScrollLines")
            .SubItems.Add(SystemInformation.MouseWheelScrollLines.ToString())
        End With
        With lvwSystemInformation.Items.Add("NativeMouseWheelSupport")
            .SubItems.Add(SystemInformation.NativeMouseWheelSupport.ToString())
        End With
        With lvwSystemInformation.Items.Add("Network")
            .SubItems.Add(SystemInformation.Network.ToString())
        End With
        With lvwSystemInformation.Items.Add("PenWindows")
            .SubItems.Add(SystemInformation.PenWindows.ToString())
        End With
        With lvwSystemInformation.Items.Add("PrimaryMonitorMaximizedWindowSize")
            .SubItems.Add(SystemInformation.PrimaryMonitorMaximizedWindowSize.ToString())
        End With
        With lvwSystemInformation.Items.Add("PrimaryMonitorSize")
            .SubItems.Add(SystemInformation.PrimaryMonitorSize.ToString())
        End With
        With lvwSystemInformation.Items.Add("RightAlignedMenus")
            .SubItems.Add(SystemInformation.RightAlignedMenus.ToString())
        End With
        With lvwSystemInformation.Items.Add("Secure")
            .SubItems.Add(SystemInformation.Secure.ToString())
        End With
        With lvwSystemInformation.Items.Add("ShowSounds")
            .SubItems.Add(SystemInformation.ShowSounds.ToString())
        End With
        With lvwSystemInformation.Items.Add("SmallIconSize")
            .SubItems.Add(SystemInformation.SmallIconSize.ToString())
        End With
        With lvwSystemInformation.Items.Add("ToolWindowCaptionButtonSize")
            .SubItems.Add(SystemInformation.ToolWindowCaptionButtonSize.ToString())
        End With
        With lvwSystemInformation.Items.Add("ToolWindowCaptionHeight")
            .SubItems.Add(SystemInformation.ToolWindowCaptionHeight.ToString())
        End With
        With lvwSystemInformation.Items.Add("UserDomainName")
            .SubItems.Add(SystemInformation.UserDomainName.ToString())
        End With
        With lvwSystemInformation.Items.Add("UserInteractive")
            .SubItems.Add(SystemInformation.UserInteractive.ToString())
        End With
        With lvwSystemInformation.Items.Add("UserName")
            .SubItems.Add(SystemInformation.UserName.ToString())
        End With
        With lvwSystemInformation.Items.Add("VerticalScrollBarArrowHeight")
            .SubItems.Add(SystemInformation.VerticalScrollBarArrowHeight.ToString())
        End With
        With lvwSystemInformation.Items.Add("VerticalScrollBarThumbHeight")
            .SubItems.Add(SystemInformation.VerticalScrollBarThumbHeight.ToString())
        End With
        With lvwSystemInformation.Items.Add("VerticalScrollBarWidth")
            .SubItems.Add(SystemInformation.VerticalScrollBarWidth.ToString())
        End With
        With lvwSystemInformation.Items.Add("VirtualScreen")
            .SubItems.Add(SystemInformation.VirtualScreen.ToString())
        End With
        With lvwSystemInformation.Items.Add("WorkingArea")
            .SubItems.Add(SystemInformation.WorkingArea.ToString())
        End With
    End Sub

    Private Sub RunMethods()
        ' Environment.GetLogicalDrives returns an 
        ' array of drive names. You can iterate
        ' through the array just like you would with 
        ' any other array, or you can use the array
        ' as the data source for a list, as seen here.

        ' Once you have a list of drives, you might
        ' want to retrieve information about the files
        ' on those drives. See the File and Directory
        ' classes for more infomation on working 
        ' with those logical entities.
        lstLogicalDrives.DataSource = _
        Environment.GetLogicalDrives()

        lstCommandLineArgs.DataSource = _
        Environment.GetCommandLineArgs()
    End Sub

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
