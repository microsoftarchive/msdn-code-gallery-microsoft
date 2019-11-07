Namespace Common
    ''' <summary>
    '''  
    ''' </summary>
    ''' <remarks></remarks>
    Public Class NavigationHelper
        Inherits DependencyObject
        Private Property Page() As Page
        Private ReadOnly Property Frame() As Frame
            Get
                Return Me.Page.Frame
            End Get
        End Property

        Public Sub New(page As Page)
            Me.Page = page

            ' When this page is part of the visual tree make two changes:
            ' 1) Map application view state to visual state for the page
            ' 2) Handle keyboard and mouse navigation requests
            AddHandler Me.Page.Loaded,
                Sub(sender, e)
                    ' Keyboard and mouse navigation only apply when occupying the entire window
                    If Me.Page.ActualHeight = Window.Current.Bounds.Height AndAlso Me.Page.ActualWidth = Window.Current.Bounds.Width Then
                        ' Listen to the window directly so focus isn't required
                        AddHandler Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated, AddressOf CoreDispatcher_AcceleratorKeyActivated
                        AddHandler Window.Current.CoreWindow.PointerPressed, AddressOf Me.CoreWindow_PointerPressed
                    End If
                End Sub

            ' Undo the same changes when the page is no longer visible
            AddHandler Me.Page.Unloaded,
                Sub(sender, e)
                    RemoveHandler Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated, AddressOf CoreDispatcher_AcceleratorKeyActivated
                    RemoveHandler Window.Current.CoreWindow.PointerPressed, AddressOf Me.CoreWindow_PointerPressed
                End Sub
        End Sub

#Region "Navigation support"

        Private _goBackCommand As RelayCommand
        Public Property GoBackCommand() As RelayCommand
            Get
                If _goBackCommand Is Nothing Then
                    _goBackCommand = New RelayCommand(AddressOf Me.GoBack, AddressOf Me.CanGoBack)
                End If
                Return _goBackCommand
            End Get
            Set(value As RelayCommand)
                _goBackCommand = value
            End Set
        End Property

        Private _goForwardCommand As RelayCommand
        Public ReadOnly Property GoForwardCommand() As RelayCommand
            Get
                If _goForwardCommand Is Nothing Then
                    _goForwardCommand = New RelayCommand(AddressOf Me.GoForward, AddressOf Me.CanGoForward)
                End If
                Return _goForwardCommand
            End Get
        End Property

        Public Overridable Function CanGoBack() As Boolean
            Return Me.Frame IsNot Nothing AndAlso Me.Frame.CanGoBack
        End Function
        Public Overridable Function CanGoForward() As Boolean
            Return Me.Frame IsNot Nothing AndAlso Me.Frame.CanGoForward
        End Function

        Public Overridable Sub GoBack()
            If Me.Frame IsNot Nothing AndAlso Me.Frame.CanGoBack Then
                Me.Frame.GoBack()
            End If
        End Sub
        Public Overridable Sub GoForward()
            If Me.Frame IsNot Nothing AndAlso Me.Frame.CanGoForward Then
                Me.Frame.GoForward()
            End If
        End Sub

        ''' <summary>
        ''' Invoked on every keystroke, including system keys such as Alt key combinations, when
        ''' this page is active and occupies the entire window.  Used to detect keyboard navigation
        ''' between pages even when the page itself doesn't have focus.
        ''' </summary>
        ''' <param name="sender">Instance that triggered the event.</param>
        ''' <param name="e">Event data describing the conditions that led to the event.</param>
        Private Sub CoreDispatcher_AcceleratorKeyActivated(sender As Windows.UI.Core.CoreDispatcher,
                                                           e As Windows.UI.Core.AcceleratorKeyEventArgs)
            Dim virtualKey As Windows.System.VirtualKey = e.VirtualKey

            ' Only investigate further when Left, Right, or the dedicated Previous or Next keys
            ' are pressed
            If (e.EventType = Windows.UI.Core.CoreAcceleratorKeyEventType.SystemKeyDown OrElse
                e.EventType = Windows.UI.Core.CoreAcceleratorKeyEventType.KeyDown) AndAlso
                (virtualKey = Windows.System.VirtualKey.Left OrElse
                virtualKey = Windows.System.VirtualKey.Right OrElse
                virtualKey = 166 OrElse
                virtualKey = 167) Then

                ' Determine what modifier keys were held
                Dim coreWindow As Windows.UI.Core.CoreWindow = Window.Current.CoreWindow
                Dim downState As Windows.UI.Core.CoreVirtualKeyStates = Windows.UI.Core.CoreVirtualKeyStates.Down
                Dim menuKey As Boolean = (coreWindow.GetKeyState(Windows.System.VirtualKey.Menu) And downState) = downState
                Dim controlKey As Boolean = (coreWindow.GetKeyState(Windows.System.VirtualKey.Control) And downState) = downState
                Dim shiftKey As Boolean = (coreWindow.GetKeyState(Windows.System.VirtualKey.Shift) And downState) = downState
                Dim noModifiers As Boolean = Not menuKey AndAlso Not controlKey AndAlso Not shiftKey
                Dim onlyAlt As Boolean = menuKey AndAlso Not controlKey AndAlso Not shiftKey

                If (virtualKey = 166 AndAlso noModifiers) OrElse
                    (virtualKey = Windows.System.VirtualKey.Left AndAlso onlyAlt) Then

                    ' When the previous key or Alt+Left are pressed navigate back
                    e.Handled = True
                    Me.GoBackCommand.Execute(Nothing)
                ElseIf (virtualKey = 167 AndAlso noModifiers) OrElse
                    (virtualKey = Windows.System.VirtualKey.Right AndAlso onlyAlt) Then

                    ' When the next key or Alt+Right are pressed navigate forward
                    e.Handled = True
                    Me.GoBackCommand.Execute(Nothing)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Invoked on every mouse click, touch screen tap, or equivalent interaction when this
        ''' page is active and occupies the entire window.  Used to detect browser-style next and
        ''' previous mouse button clicks to navigate between pages.
        ''' </summary>
        ''' <param name="sender">Instance that triggered the event.</param>
        ''' <param name="e">Event data describing the conditions that led to the event.</param>
        Private Sub CoreWindow_PointerPressed(sender As Windows.UI.Core.CoreWindow,
                                              e As Windows.UI.Core.PointerEventArgs)
            Dim properties As Windows.UI.Input.PointerPointProperties = e.CurrentPoint.Properties

            ' Ignore button chords with the left, right, and middle buttons
            If properties.IsLeftButtonPressed OrElse properties.IsRightButtonPressed OrElse
                properties.IsMiddleButtonPressed Then Return

            ' If back or foward are pressed (but not both) navigate appropriately
            Dim backPressed As Boolean = properties.IsXButton1Pressed
            Dim forwardPressed As Boolean = properties.IsXButton2Pressed
            If backPressed Xor forwardPressed Then
                e.Handled = True
                If backPressed Then Me.GoBackCommand.Execute(Nothing)
                If forwardPressed Then Me.GoForwardCommand.Execute(Nothing)
            End If
        End Sub

#End Region

#Region "Process lifetime management"

        Private _pageKey As String

        ''' <summary>
        ''' Invoked when this page is about to be displayed in a Frame.
        ''' </summary>
        ''' <param name="e">Event data that describes how this page was reached.  The Parameter
        ''' property provides the group to be displayed.</param>
        Public Sub OnNavigatedTo(e As Navigation.NavigationEventArgs)
            Dim frameState As Dictionary(Of String, Object) = SuspensionManager.SessionStateForFrame(Me.Frame)
            Me._pageKey = "Page-" & Me.Frame.BackStackDepth

            If e.NavigationMode = Navigation.NavigationMode.New Then

                ' Clear existing state for forward navigation when adding a new page to the
                ' navigation stack
                Dim nextPageKey As String = Me._pageKey
                Dim nextPageIndex As Integer = Me.Frame.BackStackDepth
                While (frameState.Remove(nextPageKey))
                    nextPageIndex += 1
                    nextPageKey = "Page-" & nextPageIndex
                End While


                ' Pass the navigation parameter to the new page
                RaiseEvent LoadState(Me, New LoadStateEventArgs(e.Parameter, Nothing))
            Else

                ' Pass the navigation parameter and preserved page state to the page, using
                ' the same strategy for loading suspended state and recreating pages discarded
                ' from cache
                RaiseEvent LoadState(Me, New LoadStateEventArgs(e.Parameter, DirectCast(frameState(Me._pageKey), Dictionary(Of String, Object))))
            End If
        End Sub

        ''' <summary>
        ''' Invoked when this page will no longer be displayed in a Frame.
        ''' </summary>
        ''' <param name="e">Event data that describes how this page was reached.  The Parameter
        ''' property provides the group to be displayed.</param>
        Public Sub OnNavigatedFrom(e As Navigation.NavigationEventArgs)
            Dim frameState As Dictionary(Of String, Object) = SuspensionManager.SessionStateForFrame(Me.Frame)
            Dim pageState As New Dictionary(Of String, Object)()
            RaiseEvent SaveState(Me, New SaveStateEventArgs(pageState))
            frameState(_pageKey) = pageState
        End Sub

        Public Event LoadState As LoadStateEventHandler
        Public Event SaveState As SaveStateEventHandler
#End Region

    End Class

    Public Class LoadStateEventArgs
        Inherits EventArgs

        Public Property NavigationParameter() As Object
        Public Property PageState() As Dictionary(Of String, Object)

        Public Sub New(navigationParameter As Object, pageState As Dictionary(Of String, Object))
            MyBase.New()
            Me.NavigationParameter = navigationParameter
            Me.PageState = pageState
        End Sub
    End Class
    Public Delegate Sub LoadStateEventHandler(sender As Object, e As LoadStateEventArgs)

    Public Class SaveStateEventArgs
        Inherits EventArgs

        Public Property PageState() As Dictionary(Of String, Object)

        Public Sub New(pageState As Dictionary(Of String, Object))
            MyBase.New()
            Me.PageState = pageState
        End Sub

    End Class

    Public Delegate Sub SaveStateEventHandler(sender As Object, e As SaveStateEventArgs)
End Namespace