Imports Windows.UI
Imports Windows.UI.Core.AnimationMetrics
Imports Windows.UI.ViewManagement
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Media.Animation
Imports Windows.UI.Xaml.Navigation

Enum ResizeType
    NoResize = 0
    ResizeFromShow = 1
    ResizeFromHide = 2
End Enum

Partial Public NotInheritable Class KeyboardPage
    Inherits Page

    Private displacement As Double = 0
    Private viewSize As Double = 0
    Private bottomOfList As Double = 0
    Private resized As Boolean = False
    Private shouldResize As ResizeType = ResizeType.NoResize
    Private inputPaneHelper As InputPaneHelper

    Public Sub New()
        InitializeComponent()

        ' Each scrollable area should be large enough to demonstrate scrolling
        Dim listHeight As Double = Window.Current.Bounds.Height * 2
        LeftList.Height = listHeight
        MiddleList.Height = listHeight

        ' InputPaneHelper is a custom class that allows keyboard event listeners to
        ' be attached to individual elements
        inputPaneHelper = New InputPaneHelper
        inputPaneHelper.SubscribeToKeyboard(True)
        inputPaneHelper.AddShowingHandler(CustomHandlingBox, AddressOf CustomKeyboardHandler)
        inputPaneHelper.SetHidingHandler(AddressOf InputPaneHiding)
    End Sub

    Private Sub CloseView_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.GoBack()
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        inputPaneHelper.SubscribeToKeyboard(False)
        inputPaneHelper.RemoveShowingHandler(CustomHandlingBox)
        inputPaneHelper.SetHidingHandler(Nothing)
    End Sub

    Private Sub CustomKeyboardHandler(sender As Object, e As InputPaneVisibilityEventArgs)
        ' This function animates the middle scroll area up, then resizes the rest of
        ' the viewport. The order of operations is important to ensure that the user
        ' doesn't see a blank spot appear behind the keyboard
        viewSize = e.OccludedRect.Y

        ' Keep in mind that other elements could be shifting out of your control. The sticky app bar, for example
        ' will move on its own. You should make sure the input element doesn't get occluded by the bar
        displacement = -e.OccludedRect.Height
        bottomOfList = MiddleScroller.VerticalOffset + MiddleScroller.ActualHeight

        e.EnsuredFocusedElementInView = True

        ShowingMoveSpline.Value = displacement
        MoveMiddleOnShowing.Begin()
    End Sub

    Private Sub ShowAnimationComplete(sender As Object, e As Object)
        ' Once the animation completes, the app is resized
        shouldResize = ResizeType.ResizeFromShow
        Container.SetValue(Grid.HeightProperty, viewSize)
        MiddleTranslate.Y = 0
    End Sub

    Private Sub InputPaneHiding(sender As InputPane, e As InputPaneVisibilityEventArgs)
        If displacement <> 0.0 Then
            MoveMiddleOnShowing.Stop()
            ' Keep in mind that other elements could be shifting out of your control. The sticky app bar, for example
            ' will move on its own. You should make sure the input element doesn't get occluded by the bar
            bottomOfList = MiddleScroller.VerticalOffset + MiddleScroller.ActualHeight

            ' If the middle area has actually completed resize, then we want to ignore
            ' the default system behavior
            If (resized) Then
                e.EnsuredFocusedElementInView = True
            End If

            ' If the container has already been resized, it should be sized back to the right size
            ' Otherwise, there's no need to change the height
            If Double.IsNaN(Container.GetValue(Grid.HeightProperty)) Then
                MoveMiddleOnHiding.Begin()
            Else
                shouldResize = ResizeType.ResizeFromHide
                Container.ClearValue(Grid.HeightProperty)
            End If
        End If
    End Sub

    Private Sub MiddleScroller_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        ' Scrolling should occur after the scrollable element has been resized to ensure
        ' that the items the user was looking at remain in viewZ
        If shouldResize = ResizeType.ResizeFromShow Then
            resized = True
            shouldResize = ResizeType.NoResize
            MiddleScroller.ScrollToVerticalOffset(bottomOfList - MiddleScroller.ActualHeight)
        ElseIf shouldResize = ResizeType.ResizeFromHide Then
            shouldResize = ResizeType.NoResize
            MiddleTranslate.Y = displacement
            MiddleScroller.ScrollToVerticalOffset(bottomOfList - MiddleScroller.ActualHeight)

            displacement = 0
            resized = False

            MoveMiddleOnHiding.Begin()
        End If
    End Sub
End Class
