Imports System
Imports Microsoft.Samples.VisualStudio.IDE.ToolWindow
Imports System.Globalization
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports System.Collections
Imports Microsoft.VisualStudio.Shell.Interop
Imports MsVsShell = Microsoft.VisualStudio.Shell
Imports VsConstants = Microsoft.VisualStudio.VSConstants
Imports ErrorHandler = Microsoft.VisualStudio.ErrorHandler



Public Class PersistedWindowWPFControl
    ' List of tool windows.
    Private toolWindowsList As WindowList = Nothing
    ' Cached Selection Tracking service used to expose properties.
    Private trackSelection_Renamed As ITrackSelection = Nothing
    ' Object holding the current selection properties.
    Private selectionContainer As New MsVsShell.SelectionContainer()
    ' This allows us to prevent infinite recursion when we are changing the selection items ourselves.
    Private ignoreSelectedObjectsChanges As Boolean = False

    ''' <summary>
    ''' This constructor is the default for a user control.
    ''' </summary>
    Public Sub New()
        ' Normal control initialization.
        InitializeComponent()

        ' Create an instance of our window list object.
        toolWindowsList = New WindowList()
    End Sub

    ''' <summary>
    ''' Track selection service for the tool window.
    ''' This should be set by the tool window pane as soon as the tool
    ''' window is created.
    ''' </summary>
    Friend Property TrackSelection() As ITrackSelection
        Get
            Return CType(trackSelection_Renamed, ITrackSelection)
        End Get
        Set(ByVal value As ITrackSelection)
            If value Is Nothing Then
                Throw New ArgumentNullException("TrackSelection")
            End If
            trackSelection_Renamed = value
            ' Inititalize with an empty selection
            ' Failure to do this would result in our later calls to 
            ' OnSelectChange to be ignored (unless focus is lost
            ' and regained).
            selectionContainer.SelectableObjects = Nothing
            selectionContainer.SelectedObjects = Nothing
            trackSelection_Renamed.OnSelectChange(selectionContainer)
            AddHandler selectionContainer.SelectedObjectsChanged, AddressOf selectionContainer_SelectedObjectsChanged
        End Set
    End Property

    ''' <summary>
    ''' Repopulate the listview with the latest data.
    ''' </summary>
    Friend Sub RefreshData()
        ' Update the list
        toolWindowsList.RefreshList()
        ' Update the listview
        PopulateListView()
    End Sub

    ''' <summary>
    ''' Repopulate the listview with the data provided.
    ''' </summary>
    Private Sub PopulateListView()
        ' Empty the list.
        listView1.Items.Clear()
        ' Fill in the data.
        For Each windowName As String In toolWindowsList.WindowNames
            listView1.Items.Add(windowName)
        Next windowName

        ' Unselect every thing.
        listView1.SelectedItems.Clear()
        ' Keep the property grid in sync.
        listView1_SelectionChanged(Me, Nothing)


    End Sub

    ''' <summary>
    ''' Handle change to the current selection is done throught the properties window
    ''' drop down list.
    ''' </summary>
    ''' <param name="sender">Sender</param>
    ''' <param name="e">Arguments</param>
    Private Sub selectionContainer_SelectedObjectsChanged(ByVal sender As Object, ByVal e As EventArgs)
        ' Set the flag letting us know we are changing the selection ourself.
        ignoreSelectedObjectsChanges = True
        Try
            ' First clear the current selection.
            Me.listView1.SelectedItems.Clear()
            ' See if we have something selected.
            If selectionContainer.SelectedObjects.Count > 0 Then
                ' We only support single selection, so pick the first one.
                Dim enumerator As IEnumerator = selectionContainer.SelectedObjects.GetEnumerator()
                If enumerator.MoveNext() Then
                    Dim newSelection As SelectionProperties = CType(enumerator.Current, SelectionProperties)
                    Dim index As Integer = newSelection.Index
                    ' Select the corresponding item.
                    Me.listView1.SelectedIndex = index
                End If
            End If
        Finally
            ' Make sure we react to future events.
            ignoreSelectedObjectsChanges = False
        End Try
    End Sub

    ''' <summary>
    ''' Push properties for the selected item to the properties window.
    ''' Note that throwing from a Windows Forms event handler would cause
    ''' Visual Studio to crash. So if you expect your code to throw
    ''' you should make sure to catch the exceptions you expect.
    ''' </summary>
    ''' <param name="sender">Event sender</param>
    ''' <param name="e">Arguments</param>
    Private Sub listView1_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        ' If the change originates from us setting the selection, ignore the event.
        If ignoreSelectedObjectsChanges Then
            Return
        End If
        ' Create the array that will hold the properties.(one set of properties per item selected).
        Dim selectedObjects As New ArrayList()

        If listView1.SelectedItems.Count > 0 Then
            ' Get the index of the selected item.
            Dim index As Integer = listView1.Items.IndexOf(listView1.SelectedItems(0))
            ' Get the IVsWindowFrame for that item.
            Dim frame As IVsWindowFrame = toolWindowsList(index)
            ' Add the properties for the selected item.
            Dim properties As SelectionProperties = toolWindowsList.GetFrameProperties(frame)
            ' Keeping track of the index helps us know which tool window was selected
            ' when the change is done through the property window drop-down.
            properties.Index = index
            ' This sample only supports single selection, but if multiple
            ' selection is supported, multiple items could be added. The
            ' properties that they had in common would then be shown.
            selectedObjects.Add(properties)
        End If

        ' Update our selection container.
        selectionContainer.SelectedObjects = selectedObjects
        ' In order to enable the drop-down of the properties window to display
        ' all our possible items, we need to provide the list.
        selectionContainer.SelectableObjects = toolWindowsList.WindowsProperties
        ' Inform Visual Studio that we changed the selection and push the new list of properties.
        TrackSelection.OnSelectChange(selectionContainer)
    End Sub
End Class
