Imports Windows.UI.ViewManagement
Imports Windows.UI.Xaml
Imports Windows.Foundation
Imports Windows.UI.Xaml.Media.Animation
Imports System.Collections.Generic

Public Delegate Sub InputPaneShowingHandler(sender As Object, e As InputPaneVisibilityEventArgs)
Public Delegate Sub InputPaneHidingHandler(input As InputPane, e As InputPaneVisibilityEventArgs)

Public Class InputPaneHelper
    Private handlerMap As Dictionary(Of UIElement, InputPaneShowingHandler)
    Private lastFocusedElement As UIElement = Nothing
    Private hidingHandlerDelegate As InputPaneHidingHandler = Nothing

    Public Sub New()
        handlerMap = New Dictionary(Of UIElement, InputPaneShowingHandler)
    End Sub

    Public Sub SubscribeToKeyboard(subscribe As Boolean)
        Dim input As InputPane = InputPane.GetForCurrentView

        If subscribe Then
            AddHandler input.Showing, AddressOf ShowingHandler
            AddHandler input.Hiding, AddressOf HidingHandler
        Else
            RemoveHandler input.Showing, AddressOf ShowingHandler
            RemoveHandler input.Hiding, AddressOf HidingHandler
        End If
    End Sub

    Public Sub AddShowingHandler(element As UIElement, handler As InputPaneShowingHandler)
        If handlerMap.ContainsKey(element) Then
            Throw New System.Exception("A handler is already registered!")
        Else
            handlerMap.Add(element, handler)
            AddHandler element.GotFocus, AddressOf GotFocusHandler
            AddHandler element.LostFocus, AddressOf LostFocusHandler
        End If
    End Sub

    ' It's important to tell which element got focus in order to decide if
    ' the keyboard handler should be called
    Private Sub GotFocusHandler(sender As Object, e As RoutedEventArgs)
        lastFocusedElement = DirectCast(sender, UIElement)
    End Sub

    Private Sub LostFocusHandler(sender As Object, e As RoutedEventArgs)
        If lastFocusedElement Is DirectCast(sender, UIElement) Then
            lastFocusedElement = Nothing
        End If
    End Sub

    Private Sub ShowingHandler(sender As InputPane, e As InputPaneVisibilityEventArgs)
        If lastFocusedElement IsNot Nothing AndAlso handlerMap.Count > 0 Then
            handlerMap(lastFocusedElement)(lastFocusedElement, e)
        End If
        lastFocusedElement = Nothing
    End Sub

    Private Sub HidingHandler(sender As InputPane, e As InputPaneVisibilityEventArgs)
        hidingHandlerDelegate(sender, e)
        lastFocusedElement = Nothing
    End Sub

    Public Sub SetHidingHandler(handler As InputPaneHidingHandler)
        Me.hidingHandlerDelegate = handler
    End Sub

    Public Sub RemoveShowingHandler(element As UIElement)
        handlerMap.Remove(element)
        RemoveHandler element.GotFocus, AddressOf GotFocusHandler
        RemoveHandler element.LostFocus, AddressOf LostFocusHandler
    End Sub
End Class
