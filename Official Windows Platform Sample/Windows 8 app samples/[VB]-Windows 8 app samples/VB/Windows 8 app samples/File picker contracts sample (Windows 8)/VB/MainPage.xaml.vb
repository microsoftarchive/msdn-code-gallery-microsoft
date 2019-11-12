'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System
Imports System.Collections.Generic
Imports Windows.UI.ViewManagement
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Markup
Imports Windows.UI.Xaml.Navigation

Namespace Global.SDKTemplate
    ''' <summary>
    ''' An empty page that can be used on its own or navigated to within a Frame.
    ''' </summary>
    Partial Public NotInheritable Class MainPage
        Inherits Global.SDKTemplate.Common.LayoutAwarePage
        Public Const FEATURE_NAME As String = "File picker contracts sample"

        Public Event ScenarioLoaded As System.EventHandler
        Public Event MainPageResized As EventHandler(Of MainPageSizeChangedEventArgs)
        Public AutoSizeInputSectionWhenSnapped As Boolean = True

        Public LaunchArgs As Windows.ApplicationModel.Activation.LaunchActivatedEventArgs

        Public Shared Current As Global.SDKTemplate.MainPage

        Private HiddenFrame As Frame = Nothing

        Private _scenariosList As New List(Of Scenario) From
        {
            New Scenario With {.Title = "File Open Picker contract",    .ClassType = GetType(Global.FilePickerContracts.MainPage_PickFile)},
            New Scenario With {.Title = "File Save Picker contract",    .ClassType = GetType(Global.FilePickerContracts.MainPage_SaveFile)},
            New Scenario With {.Title = "Cached File Updater contract", .ClassType = GetType(Global.FilePickerContracts.MainPage_CachedFile)}
        }

        Friend Function EnsureUnsnapped() As Boolean
            ' FilePicker APIs will not work if the application is in a snapped state.
            ' If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
            Dim unsnapped As Boolean = ((ApplicationView.Value <> ApplicationViewState.Snapped) OrElse ApplicationView.TryUnsnap())
            If Not unsnapped Then
                NotifyUser("Cannot unsnap the sample.", NotifyType.StatusMessage)
            End If

            Return unsnapped
        End Function

        Friend Sub ResetScenarioOutput(output As TextBlock)
            ' clear Error/Status
            NotifyUser("", NotifyType.ErrorMessage)
            NotifyUser("", NotifyType.StatusMessage)
            ' clear scenario output
            output.Text = ""
        End Sub

        Public Sub New()
            Me.InitializeComponent()

            ' This is a static public property that will allow downstream pages to get
            ' a handle to the MainPage instance in order to call methods that are in this class.
            Current = Me

            ' This frame is hidden, meaning it is never shown.  It is simply used to load
            ' each scenario page and then pluck out the input and output sections and
            ' place them into the UserControls on the main page.
            HiddenFrame = New Windows.UI.Xaml.Controls.Frame()
            HiddenFrame.Visibility = Windows.UI.Xaml.Visibility.Collapsed
            LayoutRoot.Children.Add(HiddenFrame)

            SetFeatureName(FEATURE_NAME)

            AddHandler ScenariosListbox.SelectionChanged, AddressOf Scenarios_SelectionChanged
            AddHandler SizeChanged, AddressOf MainPage_SizeChanged
        End Sub

        Private Sub MainPage_SizeChanged(sender As Object, e As SizeChangedEventArgs)
            InvalidateSize()

            Dim args As New MainPageSizeChangedEventArgs()
            args.ViewState = ApplicationView.Value
            RaiseEvent MainPageResized(Me, args)
        End Sub

        ''' <summary>
        ''' Invoked when this page is about to be displayed in a Frame.
        ''' </summary>
        ''' <param name="e">Event data that describes how this page was reached.  The Parameter
        ''' property is typically used to configure the page.</param>
        Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
            PopulateScenarios()
            InvalidateViewState()
        End Sub

        Private Sub InvalidateSize()
            ' Get the window width
            Dim windowWidth As Double = Me.ActualWidth

            If windowWidth <> 0.0 Then
                ' Get the width of the ListBox.
                Dim listBoxWidth As Double = ScenariosListbox.ActualWidth

                ' Is the ListBox using any margins that we need to consider?
                Dim listBoxMarginLeft As Double = ScenariosListbox.Margin.Left
                Dim listBoxMarginRight As Double = ScenariosListbox.Margin.Right

                ' Figure out how much room is left after considering the list box width
                Dim availableWidth As Double = windowWidth - listBoxWidth

                ' Is the top most child using margins?
                Dim layoutRootMarginLeft As Double = ContentRoot.Margin.Left
                Dim layoutRootMarginRight As Double = ContentRoot.Margin.Right

                ' We have different widths to use depending on the view state
                If ApplicationView.Value <> ApplicationViewState.Snapped Then
                    ' Make us as big as the the left over space, factoring in the ListBox width, the ListBox margins.
                    ' and the LayoutRoot's margins
                    InputSection.Width = ((availableWidth) - (layoutRootMarginLeft + layoutRootMarginRight + listBoxMarginLeft + listBoxMarginRight))
                Else
                    ' Make us as big as the left over space, factoring in just the LayoutRoot's margins.
                    If AutoSizeInputSectionWhenSnapped Then
                        InputSection.Width = (windowWidth - (layoutRootMarginLeft + layoutRootMarginRight))
                    End If
                End If
            End If
            InvalidateViewState()
        End Sub

        Private Sub InvalidateViewState()

            If ApplicationView.Value = ApplicationViewState.Snapped Then
                Grid.SetRow(DescriptionText, 3)
                Grid.SetColumn(DescriptionText, 0)

                Grid.SetRow(InputSection, 4)
                Grid.SetColumn(InputSection, 0)

                Grid.SetRow(FooterPanel, 2)
                Grid.SetColumn(FooterPanel, 0)
            Else
                Grid.SetRow(DescriptionText, 1)
                Grid.SetColumn(DescriptionText, 1)

                Grid.SetRow(InputSection, 2)
                Grid.SetColumn(InputSection, 1)

                Grid.SetRow(FooterPanel, 1)
                Grid.SetColumn(FooterPanel, 1)
            End If
        End Sub

        Private Sub PopulateScenarios()
            Dim LocalScenarioList As New System.Collections.ObjectModel.ObservableCollection(Of Object)()
            Dim i As Integer = 0

            For Each s As Scenario In _scenariosList
                Dim item As New ListBoxItem()
                s.Title = (System.Threading.Interlocked.Increment(i)).ToString & ") " & s.Title
                item.Content = s
                item.Name = s.ClassType.FullName
                LocalScenarioList.Add(item)
            Next

            ' Bind the ListBox to the scenario list.
            ScenariosListbox.ItemsSource = LocalScenarioList

            ' Starting scenario is the first or based upon a previous selection.
            Dim startingScenarioIndex As Integer = -1

            If SuspensionManager.SessionState.ContainsKey("SelectedScenarioIndex") Then
                Dim selectedScenarioIndex As Integer = Convert.ToInt32(SuspensionManager.SessionState("SelectedScenarioIndex"))
                startingScenarioIndex = selectedScenarioIndex
            End If

            ScenariosListbox.SelectedIndex = If(startingScenarioIndex <> -1, startingScenarioIndex, 0)
        End Sub

        ''' <summary>
        ''' This method is responsible for loading the individual input and output sections for each scenario.  This
        ''' is based on navigating a hidden Frame to the ScenarioX.xaml page and then extracting out the input
        ''' and output sections into the respective UserControl on the main page.
        ''' </summary>
        ''' <param name="scenarioName"></param>
        Public Sub LoadScenario(scenarioClass As Type)
            AutoSizeInputSectionWhenSnapped = True

            ' Load the ScenarioX.xaml file into the Frame.
            HiddenFrame.Navigate(scenarioClass, Me)

            If HiddenFrame.Content IsNot Nothing Then
                ' Get the top element, the Page, so we can look up the elements
                ' that represent the input and output sections of the ScenarioX file.
                Dim hiddenPage As Page = TryCast(HiddenFrame.Content, Page)

                ' Get each element.
                Dim input As UIElement = TryCast(hiddenPage.FindName("Input"), UIElement)
                Dim output As UIElement = TryCast(hiddenPage.FindName("Output"), UIElement)

                If input Is Nothing Then
                    ' Malformed input section.
                    NotifyUser(String.Format("Cannot load scenario input section for {0}.  Make sure root of input section markup has x:Name of 'Input'", scenarioClass.Name), NotifyType.ErrorMessage)
                    Return
                End If

                If output Is Nothing Then
                    ' Malformed output section.
                    NotifyUser(String.Format("Cannot load scenario output section for {0}.  Make sure root of output section markup has x:Name of 'Output'", scenarioClass.Name), NotifyType.ErrorMessage)
                    Return
                End If

                ' Find the LayoutRoot which parents the input and output sections in the main page.
                Dim panel As Panel = TryCast(hiddenPage.FindName("LayoutRoot"), Panel)

                If panel IsNot Nothing Then
                    ' Get rid of the content that is currently in the intput and output sections.
                    panel.Children.Remove(input)
                    panel.Children.Remove(output)

                    ' Populate the input and output sections with the newly loaded content.
                    InputSection.Content = input
                    OutputSection.Content = output

                    RaiseEvent ScenarioLoaded(Me, New EventArgs())
                Else
                    ' Malformed Scenario file.
                    NotifyUser(String.Format("Cannot load scenario: '{0}'.  Make sure root tag in the '{0}' file has an x:Name of 'LayoutRoot'", scenarioClass.Name), NotifyType.ErrorMessage)
                End If

            End If
        End Sub

        Private Sub Scenarios_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            If ScenariosListbox.SelectedItem IsNot Nothing Then
                NotifyUser("", NotifyType.StatusMessage)

                Dim selectedListBoxItem As ListBoxItem = TryCast(ScenariosListbox.SelectedItem, ListBoxItem)
                SuspensionManager.SessionState("SelectedScenarioIndex") = ScenariosListbox.SelectedIndex

                Dim scenario As Scenario = TryCast(selectedListBoxItem.Content, Scenario)
                LoadScenario(scenario.ClassType)
                InvalidateSize()
            End If
        End Sub

        Public Sub NotifyUser(strMessage As String, type As NotifyType)
            Select Case type
                ' Use the status message style.
                Case NotifyType.StatusMessage
                    StatusTextblock.Style = TryCast(Resources("StatusStyle"), Style)
                    Exit Select
                    ' Use the error message style.
                Case NotifyType.ErrorMessage
                    StatusTextblock.Style = TryCast(Resources("ErrorStyle"), Style)
                    Exit Select
            End Select
            StatusTextblock.Text = strMessage

            ' Collapse the StatusBlock if it has no text to conserve real estate.
            If StatusTextblock.Text <> String.Empty Then
                StatusTextblock.Visibility = Windows.UI.Xaml.Visibility.Visible
            Else
                StatusTextblock.Visibility = Windows.UI.Xaml.Visibility.Collapsed
            End If
        End Sub

        Private Async Sub Footer_Click(sender As Object, e As RoutedEventArgs)
            Await Windows.System.Launcher.LaunchUriAsync(New Uri(DirectCast(sender, HyperlinkButton).Tag.ToString))
        End Sub

        Private Sub SetFeatureName(str As String)
            FeatureNameTextblock.Text = str
        End Sub
    End Class

    Public Class MainPageSizeChangedEventArgs
        Inherits EventArgs
        Private m_viewState As ApplicationViewState

        Public Property ViewState() As ApplicationViewState
            Get
                Return m_viewState
            End Get
            Set(value As ApplicationViewState)
                m_viewState = value
            End Set
        End Property
    End Class
End Namespace
