Namespace Common

    ''' <summary>
    ''' Typical implementation of Page that provides several important conveniences:
    ''' <list type="bullet">
    ''' <item>
    ''' <description>Application view state to visual state mapping</description>
    ''' </item>
    ''' <item>
    ''' <description>GoBack, GoForward, and GoHome event handlers</description>
    ''' </item>
    ''' <item>
    ''' <description>Mouse and keyboard shortcuts for navigation</description>
    ''' </item>
    ''' <item>
    ''' <description>State management for navigation and process lifetime management</description>
    ''' </item>
    ''' <item>
    ''' <description>A default view model</description>
    ''' </item>
    ''' </list>
    ''' </summary>
    <Windows.Foundation.Metadata.WebHostHidden>
    Public Class LayoutAwarePage
        Inherits Page

        ''' <summary>
        ''' Identifies the <see cref="DefaultViewModel"/> dependency property.
        ''' </summary>
        Public Shared ReadOnly DefaultViewModelProperty As DependencyProperty =
            DependencyProperty.Register("DefaultViewModel", GetType(IObservableMap(Of String, Object)),
            GetType(LayoutAwarePage), Nothing)

        Private _layoutAwareControls As List(Of Control)

        ''' <summary>
        ''' When this page is part of the visual tree make two changes:
        ''' 1) Map application view state to visual state for the page
        ''' 2) Handle keyboard and mouse navigation requests
        ''' </summary>
        ''' <param name="sender">The object that initiated the request.</param>
        ''' <param name="e">Event data that describes how the request was made.</param>
        Private Sub OnLoaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.StartLayoutUpdates(sender, e)

            ' Keyboard and mouse navigation only apply when occupying the entire window
            If Me.ActualHeight = Window.Current.Bounds.Height AndAlso
                    Me.ActualWidth = Window.Current.Bounds.Width Then

                ' Listen to the window directly so focus isn't required
                AddHandler Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated,
                    AddressOf Me.CoreDispatcher_AcceleratorKeyActivated
                AddHandler Window.Current.CoreWindow.PointerPressed,
                    AddressOf Me.CoreWindow_PointerPressed
            End If
        End Sub

        ''' <summary>
        ''' Undo the changes made in <see cref="OnLoaded"/> when the page is no longer visible.
        ''' </summary>
        ''' <param name="sender">The object that initiated the request.</param>
        ''' <param name="e">Event data that describes how the request was made.</param>
        Private Sub OnUnloaded(sender As Object, e As RoutedEventArgs) Handles Me.Unloaded
            Me.StopLayoutUpdates(sender, e)
            RemoveHandler Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated,
                AddressOf Me.CoreDispatcher_AcceleratorKeyActivated
            RemoveHandler Window.Current.CoreWindow.PointerPressed,
                AddressOf Me.CoreWindow_PointerPressed
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the <see cref="LayoutAwarePage"/> class.
        ''' </summary>
        Public Sub New()
            If Windows.ApplicationModel.DesignMode.DesignModeEnabled Then Return

            ' Create an empty default view model
            Me.DefaultViewModel = New ObservableDictionary(Of String, Object)
        End Sub

        ''' <summary>
        ''' An implementation of <see cref="IObservableMap(Of String, Object)"/> designed to be
        ''' used as a trivial view model.
        ''' </summary>
        Protected Property DefaultViewModel As IObservableMap(Of String, Object)
            Get
                Return DirectCast(Me.GetValue(DefaultViewModelProperty), IObservableMap(Of String, Object))
            End Get
            Set(value As IObservableMap(Of String, Object))
                Me.SetValue(DefaultViewModelProperty, value)
            End Set
        End Property

#Region "Navigation support"

        ''' <summary>
        ''' Invoked as an event handler to navigate backward in the page's associated
        ''' <see cref="Frame"/> until it reaches the top of the navigation stack.
        ''' </summary>
        ''' <param name="sender">Instance that triggered the event.</param>
        ''' <param name="e">Event data describing the conditions that led to the event.</param>
        Protected Overridable Sub GoHome(sender As Object, e As RoutedEventArgs)

            ' Use the navigation frame to return to the topmost page
            If Me.Frame IsNot Nothing Then
                While Me.Frame.CanGoBack
                    Me.Frame.GoBack()
                End While
            End If
        End Sub

        ''' <summary>
        ''' Invoked as an event handler to navigate backward in the navigation stack
        ''' associated with this page's <see cref="Frame"/>.
        ''' </summary>
        ''' <param name="sender">Instance that triggered the event.</param>
        ''' <param name="e">Event data describing the conditions that led to the event.</param>
        Protected Overridable Sub GoBack(sender As Object, e As RoutedEventArgs)

            ' Use the navigation frame to return to the previous page
            If Me.Frame IsNot Nothing AndAlso Me.Frame.CanGoBack Then
                Me.Frame.GoBack()
            End If
        End Sub

        ''' <summary>
        ''' Invoked as an event handler to navigate forward in the navigation stack
        ''' associated with this page's <see cref="Frame"/>.
        ''' </summary>
        ''' <param name="sender">Instance that triggered the event.</param>
        ''' <param name="e">Event data describing the conditions that led to the event.</param>
        Protected Overridable Sub GoForward(sender As Object, e As RoutedEventArgs)

            ' Use the navigation frame to advance to the next page
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
        ''' <param name="args">Event data describing the conditions that led to the event.</param>
        Private Sub CoreDispatcher_AcceleratorKeyActivated(sender As Windows.UI.Core.CoreDispatcher,
                                                           args As Windows.UI.Core.AcceleratorKeyEventArgs)
            Dim virtualKey As Windows.System.VirtualKey = args.VirtualKey

            ' Only investigate further when Left, Right, or the dedicated Previous or Next keys
            ' are pressed
            If (args.EventType = Windows.UI.Core.CoreAcceleratorKeyEventType.SystemKeyDown OrElse
                args.EventType = Windows.UI.Core.CoreAcceleratorKeyEventType.KeyDown) AndAlso
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
                    args.Handled = True
                    Me.GoBack(Me, New RoutedEventArgs())
                ElseIf (virtualKey = 167 AndAlso noModifiers) OrElse
                    (virtualKey = Windows.System.VirtualKey.Right AndAlso onlyAlt) Then

                    ' When the next key or Alt+Right are pressed navigate forward
                    args.Handled = True
                    Me.GoForward(Me, New RoutedEventArgs())
                End If
            End If
        End Sub

        ''' <summary>
        ''' Invoked on every mouse click, touch screen tap, or equivalent interaction when this
        ''' page is active and occupies the entire window.  Used to detect browser-style next and
        ''' previous mouse button clicks to navigate between pages.
        ''' </summary>
        ''' <param name="sender">Instance that triggered the event.</param>
        ''' <param name="args">Event data describing the conditions that led to the event.</param>
        Private Sub CoreWindow_PointerPressed(sender As Windows.UI.Core.CoreWindow,
                                              args As Windows.UI.Core.PointerEventArgs)
            Dim properties As Windows.UI.Input.PointerPointProperties = args.CurrentPoint.Properties

            ' Ignore button chords with the left, right, and middle buttons
            If properties.IsLeftButtonPressed OrElse properties.IsRightButtonPressed OrElse
                properties.IsMiddleButtonPressed Then Return

            ' If back or foward are pressed (but not both) navigate appropriately
            Dim backPressed As Boolean = properties.IsXButton1Pressed
            Dim forwardPressed As Boolean = properties.IsXButton2Pressed
            If backPressed Xor forwardPressed Then
                args.Handled = True
                If backPressed Then Me.GoBack(Me, New RoutedEventArgs())
                If forwardPressed Then Me.GoForward(Me, New RoutedEventArgs())
            End If
        End Sub

#End Region

#Region "Visual state switching"

        ''' <summary>
        ''' Invoked as an event handler, typically on the <see cref="Loaded"/> event of a
        ''' <see cref="Control"/> within the page, to indicate that the sender should start receiving
        ''' visual state management changes that correspond to application view state changes.
        ''' </summary>
        ''' <param name="sender">Instance of <see cref="Control"/> that supports visual state
        ''' management corresponding to view states.</param>
        ''' <param name="e">Event data that describes how the request was made.</param>
        ''' <remarks>The current view state will immediately be used to set the corresponding
        ''' visual state when layout updates are requested.  A corresponding
        ''' <see cref="Unloaded"/> event handler connected to <see cref="StopLayoutUpdates"/> is
        ''' strongly encouraged.  Instances of <see cref="LayoutAwarePage"/> automatically invoke
        ''' these handlers in their Loaded and Unloaded events.</remarks>
        ''' <seealso cref="DetermineVisualState"/>
        ''' <seealso cref="InvalidateVisualState"/>
        Public Sub StartLayoutUpdates(sender As Object, e As RoutedEventArgs)
            If Windows.ApplicationModel.DesignMode.DesignModeEnabled Then Return

            Dim control As Control = TryCast(sender, Control)
            If control Is Nothing Then Return

            If Me._layoutAwareControls Is Nothing Then

                ' Start listening to view state changes when there are controls interested in
                ' updates
                AddHandler Window.Current.SizeChanged, AddressOf Me.WindowSizeChanged
                Me._layoutAwareControls = New List(Of Control)
            End If
            Me._layoutAwareControls.Add(control)

            ' Set the initial visual state of the control
            VisualStateManager.GoToState(control, DetermineVisualState(ApplicationView.Value), False)
        End Sub

        Private Sub WindowSizeChanged(sender As Object, e As Windows.UI.Core.WindowSizeChangedEventArgs)
            Me.InvalidateVisualState()
        End Sub

        ''' <summary>
        ''' Invoked as an event handler, typically on the <see cref="Unloaded"/> event of a
        ''' <see cref="Control"/>, to indicate that the sender should start receiving visual state
        ''' management changes that correspond to application view state changes.
        ''' </summary>
        ''' <param name="sender">Instance of <see cref="Control"/> that supports visual state
        ''' management corresponding to view states.</param>
        ''' <param name="e">Event data that describes how the request was made.</param>
        ''' <remarks>The current view state will immediately be used to set the corresponding
        ''' visual state when layout updates are requested.</remarks>
        ''' <seealso cref="StartLayoutUpdates"/>
        Public Sub StopLayoutUpdates(sender As Object, e As RoutedEventArgs)
            If Windows.ApplicationModel.DesignMode.DesignModeEnabled Then Return

            Dim control As Control = TryCast(sender, Control)
            If control Is Nothing OrElse Me._layoutAwareControls Is Nothing Then Return

            Me._layoutAwareControls.Remove(control)
            If Me._layoutAwareControls.Count = 0 Then

                ' Stop listening to view state changes when no controls are interested in updates
                Me._layoutAwareControls = Nothing
                RemoveHandler Window.Current.SizeChanged, AddressOf Me.WindowSizeChanged
            End If
        End Sub

        ''' <summary>
        ''' Translates <see cref="ApplicationViewState"/> values into strings for visual state
        ''' management within the page.  The default implementation uses the names of enum values.
        ''' Subclasses may override this method to control the mapping scheme used.
        ''' </summary>
        ''' <param name="viewState">View state for which a visual state is desired.</param>
        ''' <returns>Visual state name used to drive the <see cref="VisualStateManager"/></returns>
        ''' <seealso cref="InvalidateVisualState"/>
        Protected Overridable Function DetermineVisualState(viewState As ApplicationViewState) As String
            Return viewState.ToString()
        End Function

        ''' <summary>
        ''' Updates all controls that are listening for visual state changes with the correct
        ''' visual state.
        ''' </summary>
        ''' <remarks>
        ''' Typically used in conjunction with overriding <see cref="DetermineVisualState"/> to
        ''' signal that a different value may be returned even though the view state has not
        ''' changed.
        ''' </remarks>
        Public Sub InvalidateVisualState()
            If Me._layoutAwareControls IsNot Nothing Then
                Dim visualState As String = DetermineVisualState(ApplicationView.Value)
                For Each layoutAwareControl As Control In Me._layoutAwareControls
                    VisualStateManager.GoToState(layoutAwareControl, visualState, False)
                Next
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
        Protected Overrides Sub OnNavigatedTo(e As Navigation.NavigationEventArgs)

            ' Returning to a cached page through navigation shouldn't trigger state loading
            If Me._pageKey IsNot Nothing Then Return

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
                Me.LoadState(e.Parameter, Nothing)
            Else

                ' Pass the navigation parameter and preserved page state to the page, using
                ' the same strategy for loading suspended state and recreating pages discarded
                ' from cache
                Me.LoadState(e.Parameter, DirectCast(frameState(Me._pageKey), Dictionary(Of String, Object)))
            End If
        End Sub

        ''' <summary>
        ''' Invoked when this page will no longer be displayed in a Frame.
        ''' </summary>
        ''' <param name="e">Event data that describes how this page was reached.  The Parameter
        ''' property provides the group to be displayed.</param>
        Protected Overrides Sub OnNavigatedFrom(e As Navigation.NavigationEventArgs)
            Dim frameState As Dictionary(Of String, Object) = SuspensionManager.SessionStateForFrame(Me.Frame)
            Dim pageState As New Dictionary(Of String, Object)()
            Me.SaveState(pageState)
            frameState(_pageKey) = pageState
        End Sub

        ''' <summary>
        ''' Populates the page with content passed during navigation.  Any saved state is also
        ''' provided when recreating a page from a prior session.
        ''' </summary>
        ''' <param name="navigationParameter">The parameter value passed to
        ''' <see cref="Frame.Navigate"/> when this page was initially requested.
        ''' </param>
        ''' <param name="pageState">A dictionary of state preserved by this page during an earlier
        ''' session.  This will be null the first time a page is visited.</param>
        Protected Overridable Sub LoadState(navigationParameter As Object, pageState As Dictionary(Of String, Object))

        End Sub

        ''' <summary>
        ''' Preserves state associated with this page in case the application is suspended or the
        ''' page is discarded from the navigation cache.  Values must conform to the serialization
        ''' requirements of <see cref="SuspensionManager.SessionState"/>.
        ''' </summary>
        ''' <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        Protected Overridable Sub SaveState(pageState As Dictionary(Of String, Object))

        End Sub

#End Region

        ''' <summary>
        ''' Implementation of IObservableMap that supports reentrancy for use as a default
        ''' view model.
        ''' </summary>
        Private Class ObservableDictionary(Of K, V)
            Implements IObservableMap(Of K, V)

            Private Class ObservableDictionaryChangedEventArgs
                Implements IMapChangedEventArgs(Of K)

                Private _change As CollectionChange
                Private _key As K

                Public Sub New(change As CollectionChange, key As K)
                    Me._change = change
                    Me._key = key
                End Sub

                ReadOnly Property CollectionChange As CollectionChange Implements IMapChangedEventArgs(Of K).CollectionChange
                    Get
                        Return _change
                    End Get
                End Property

                ReadOnly Property Key As K Implements IMapChangedEventArgs(Of K).Key
                    Get
                        Return _key
                    End Get
                End Property

            End Class

            Public Event MapChanged(sender As IObservableMap(Of K, V), [event] As IMapChangedEventArgs(Of K)) Implements IObservableMap(Of K, V).MapChanged
            Private _dictionary As New Dictionary(Of K, V)()

            Private Sub InvokeMapChanged(change As CollectionChange, key As K)
                RaiseEvent MapChanged(Me, New ObservableDictionaryChangedEventArgs(change, key))
            End Sub

            Public Sub Add(key As K, value As V) Implements IDictionary(Of K, V).Add
                Me._dictionary.Add(key, value)
                Me.InvokeMapChanged(CollectionChange.ItemInserted, key)
            End Sub

            Public Sub AddKeyValuePair(item As KeyValuePair(Of K, V)) Implements ICollection(Of KeyValuePair(Of K, V)).Add
                Me.Add(item.Key, item.Value)
            End Sub

            Public Function Remove(key As K) As Boolean Implements IDictionary(Of K, V).Remove
                If Me._dictionary.Remove(key) Then
                    Me.InvokeMapChanged(CollectionChange.ItemRemoved, key)
                    Return True
                End If
                Return False
            End Function

            Public Function RemoveKeyValuePair(item As KeyValuePair(Of K, V)) As Boolean Implements ICollection(Of KeyValuePair(Of K, V)).Remove
                Dim currentValue As V
                If Me._dictionary.TryGetValue(item.Key, currentValue) AndAlso
                    Object.Equals(item.Value, currentValue) AndAlso Me._dictionary.Remove(item.Key) Then

                    Me.InvokeMapChanged(CollectionChange.ItemRemoved, item.Key)
                    Return True
                End If
                Return False
            End Function

            Default Public Property Item(key As K) As V Implements IDictionary(Of K, V).Item
                Get
                    Return Me._dictionary(key)
                End Get
                Set(value As V)
                    Me._dictionary(key) = value
                    Me.InvokeMapChanged(CollectionChange.ItemChanged, key)
                End Set
            End Property

            Public Sub Clear() Implements ICollection(Of KeyValuePair(Of K, V)).Clear
                Dim priorKeys As K() = Me._dictionary.Keys.ToArray()
                Me._dictionary.Clear()
                For Each key As K In priorKeys
                    Me.InvokeMapChanged(CollectionChange.ItemRemoved, key)
                Next
            End Sub

            Public Function Contains(item As KeyValuePair(Of K, V)) As Boolean Implements ICollection(Of KeyValuePair(Of K, V)).Contains
                Return Me._dictionary.Contains(item)
            End Function

            Public ReadOnly Property Count As Integer Implements ICollection(Of KeyValuePair(Of K, V)).Count
                Get
                    Return Me._dictionary.Count
                End Get
            End Property

            Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of KeyValuePair(Of K, V)).IsReadOnly
                Get
                    Return False
                End Get
            End Property

            Public Function ContainsKey(key As K) As Boolean Implements IDictionary(Of K, V).ContainsKey
                Return Me._dictionary.ContainsKey(key)
            End Function

            Public ReadOnly Property Keys As ICollection(Of K) Implements IDictionary(Of K, V).Keys
                Get
                    Return Me._dictionary.Keys
                End Get
            End Property

            Public Function TryGetValue(key As K, ByRef value As V) As Boolean Implements IDictionary(Of K, V).TryGetValue
                Return Me._dictionary.TryGetValue(key, value)
            End Function

            Public ReadOnly Property Values As ICollection(Of V) Implements IDictionary(Of K, V).Values
                Get
                    Return Me._dictionary.Values
                End Get
            End Property

            Public Function GetGenericEnumerator() As IEnumerator(Of KeyValuePair(Of K, V)) Implements IEnumerable(Of KeyValuePair(Of K, V)).GetEnumerator
                Return Me._dictionary.GetEnumerator()
            End Function

            Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
                Return Me._dictionary.GetEnumerator()
            End Function

            Public Sub CopyTo(array() As KeyValuePair(Of K, V), arrayIndex As Integer) Implements ICollection(Of KeyValuePair(Of K, V)).CopyTo
                Dim arraySize As Integer = array.Length
                For Each pair As KeyValuePair(Of K, V) In Me._dictionary
                    If arrayIndex >= arraySize Then Exit For
                    array(arrayIndex) = pair
                    arrayIndex += 1
                Next
            End Sub

        End Class

    End Class

End Namespace
