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
Imports Windows.UI.Notifications
Imports NotificationsExtensions.BadgeContent
Imports NotificationsExtensions

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class SendBadge
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        NumberOrGlyph.SelectedIndex = 0
        GlyphList.SelectedIndex = 0
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

#Region "Scenario-Template Code"
    Private Sub NumberOrGlyph_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If NumberOrGlyph.SelectedIndex = 0 Then
            NumberPanel.Visibility = Visibility.Visible
            GlyphPanel.Visibility = Visibility.Collapsed
        Else
            NumberPanel.Visibility = Visibility.Collapsed
            GlyphPanel.Visibility = Visibility.Visible
        End If
    End Sub

    Private Sub UpdateBadge_Click(sender As Object, e As RoutedEventArgs)
        If NumberOrGlyph.SelectedIndex = 0 Then
            Dim number As Integer
            If Int32.TryParse(NumberInput.Text, number) Then
                UpdateBadgeWithNumber(number)
            Else
                OutputTextBlock.Text = "Please enter a valid number!"
            End If
        Else
            UpdateBadgeWithGlyph(GlyphList.SelectedIndex)
        End If
    End Sub
#End Region

    Private Sub ClearBadge_Click(sender As Object, e As RoutedEventArgs)
        BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear()
        OutputTextBlock.Text = "Badge cleared"
    End Sub

    Private Sub UpdateBadgeWithNumber(number As Integer)
        Dim badgeContent As New BadgeNumericNotificationContent(CUInt(number))

        ' send the notification to the app's application tile
        BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeContent.CreateNotification())

        OutputTextBlock.Text = badgeContent.GetContent()
    End Sub

    Private Sub UpdateBadgeWithGlyph(index As Integer)
        Dim badgeContent As New BadgeGlyphNotificationContent(DirectCast(index, GlyphValue))

        ' send the notification to the app's application tile
        BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeContent.CreateNotification())

        OutputTextBlock.Text = badgeContent.GetContent()
    End Sub

End Class
