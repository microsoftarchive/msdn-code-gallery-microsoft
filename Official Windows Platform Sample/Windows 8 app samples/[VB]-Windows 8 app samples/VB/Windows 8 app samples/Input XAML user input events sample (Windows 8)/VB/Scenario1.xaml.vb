'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports System
Imports System.Collections.Generic
Imports Windows.Foundation
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Shapes
Imports Windows.UI.Xaml.Media
Imports Windows.UI

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    'Scenario specific constants
    Const SUPPORTEDCONTACTS As UInteger = 5
    Const STROKETHICKNESS As Double = 5
    Private numActiveContacts As UInteger
    Private contacts As Dictionary(Of UInteger, System.Nullable(Of Point))

    Public Sub New()
        Me.InitializeComponent()
        numActiveContacts = 0
        contacts = New Dictionary(Of UInteger, System.Nullable(Of Point))(CInt(SUPPORTEDCONTACTS))
        AddHandler Scenario1OutputRoot.PointerPressed, AddressOf Scenario1OutputRoot_PointerPressed
        AddHandler Scenario1OutputRoot.PointerMoved, AddressOf Scenario1OutputRoot_PointerMoved
        AddHandler Scenario1OutputRoot.PointerReleased, AddressOf Scenario1OutputRoot_PointerReleased
        AddHandler Scenario1OutputRoot.PointerExited, AddressOf Scenario1OutputRoot_PointerReleased
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    Private Sub Scenario1OutputRoot_PointerReleased(sender As Object, e As PointerRoutedEventArgs)
        Dim ptrId As UInteger = e.GetCurrentPoint(TryCast(sender, FrameworkElement)).PointerId
        If e.GetCurrentPoint(Scenario1OutputRoot).Properties.PointerUpdateKind <> Windows.UI.Input.PointerUpdateKind.Other Then
            Me.buttonPress.Text = e.GetCurrentPoint(Scenario1OutputRoot).Properties.PointerUpdateKind.ToString
        End If
        If contacts.ContainsKey(ptrId) Then
            contacts(ptrId) = Nothing
            contacts.Remove(ptrId)
            numActiveContacts -= 1
        End If
        e.Handled = True
    End Sub

    Private Sub Scenario1OutputRoot_PointerMoved(sender As Object, e As PointerRoutedEventArgs)
        Dim pt As Windows.UI.Input.PointerPoint = e.GetCurrentPoint(Scenario1OutputRoot)
        Dim ptrId As UInteger = pt.PointerId
        If pt.Properties.PointerUpdateKind <> Windows.UI.Input.PointerUpdateKind.Other Then
            Me.buttonPress.Text = pt.Properties.PointerUpdateKind.ToString
        End If
        If contacts.ContainsKey(ptrId) AndAlso contacts(ptrId).HasValue Then
            Dim currentContact As Point = pt.Position
            Dim previousContact As Point = contacts(ptrId).Value
            If Distance(currentContact, previousContact) > 4 Then
                Dim l As New Line With {.X1 = previousContact.X,
                                        .Y1 = previousContact.Y,
                                        .X2 = currentContact.X,
                                        .Y2 = currentContact.Y,
                                        .StrokeThickness = STROKETHICKNESS,
                                        .Stroke = New SolidColorBrush(Colors.Red),
                                        .StrokeEndLineCap = PenLineCap.Round
                                       }

                contacts(ptrId) = currentContact
                DirectCast(Scenario1OutputRoot.Children, System.Collections.Generic.IList(Of UIElement)).Add(l)
            End If
        End If

        e.Handled = True
    End Sub

    Private Function Distance(currentContact As Point, previousContact As Point) As Double
        Return Math.Sqrt(Math.Pow(currentContact.X - previousContact.X, 2) + Math.Pow(currentContact.Y - previousContact.Y, 2))
    End Function

    Private Sub Scenario1OutputRoot_PointerPressed(sender As Object, e As PointerRoutedEventArgs)
        If numActiveContacts >= SUPPORTEDCONTACTS Then
            ' cannot support more contacts
            Exit Sub
        End If
        Dim pt As Windows.UI.Input.PointerPoint = e.GetCurrentPoint(Scenario1OutputRoot)
        If pt.Properties.PointerUpdateKind <> Windows.UI.Input.PointerUpdateKind.Other Then
            Me.buttonPress.Text = pt.Properties.PointerUpdateKind.ToString
        End If
        contacts(pt.PointerId) = pt.Position
        e.Handled = True
        numActiveContacts += 1
    End Sub

    Private Sub Scenario1Reset(sender As Object, e As RoutedEventArgs)
        Scenario1ResetMethod()
    End Sub

    Public Sub Scenario1ResetMethod()
        DirectCast(Scenario1OutputRoot.Children, System.Collections.Generic.IList(Of UIElement)).Clear()
        numActiveContacts = 0
        If contacts IsNot Nothing Then
            contacts.Clear()
        End If
    End Sub
End Class
