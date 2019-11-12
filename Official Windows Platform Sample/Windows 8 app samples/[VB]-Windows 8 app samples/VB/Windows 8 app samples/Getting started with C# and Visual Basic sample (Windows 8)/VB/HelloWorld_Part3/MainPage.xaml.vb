'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

' The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

''' <summary>
''' A basic page that provides characteristics common to most applications.
''' </summary>
Public NotInheritable Class MainPage
    Inherits Common.LayoutAwarePage

    ''' <summary>
    ''' Populates the page with content passed during navigation.  Any saved state is also
    ''' provided when recreating a page from a prior session.
    ''' </summary>
    ''' <param name="navigationParameter">The parameter value passed to
    ''' <see cref="Frame.Navigate"/> when this page was initially requested.
    ''' </param>
    ''' <param name="pageState">A dictionary of state preserved by this page during an earlier
    ''' session.  This will be null the first time a page is visited.</param>
    Protected Overrides Sub LoadState(navigationParameter As Object, pageState As Dictionary(Of String, Object))

        ' Restore values stored in session state.
        If pageState IsNot Nothing AndAlso pageState.ContainsKey("greetingOutputText") Then
            greetingOutput.Text = pageState("greetingOutputText").ToString()
        End If

        ' Restore values stored in app data.
        Dim roamingSettings =
            Windows.Storage.ApplicationData.Current.RoamingSettings

        If roamingSettings.Values.ContainsKey("userName") Then
            nameInput.Text = roamingSettings.Values("userName").ToString()
        End If
    End Sub

    ''' <summary>
    ''' Preserves state associated with this page in case the application is suspended or the
    ''' page is discarded from the navigation cache.  Values must conform to the serialization
    ''' requirements of <see cref="Common.SuspensionManager.SessionState"/>.
    ''' </summary>
    ''' <param name="pageState">An empty dictionary to be populated with serializable state.</param>
    Protected Overrides Sub SaveState(pageState As Dictionary(Of String, Object))
        pageState("greetingOutputText") = greetingOutput.Text
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        greetingOutput.Text = "Hello, " & nameInput.Text & "!"
    End Sub

    Private Sub NameInput_TextChanged(sender As Object, e As TextChangedEventArgs) Handles nameInput.TextChanged
        Dim roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings
        roamingSettings.Values("userName") = nameInput.Text
    End Sub

    Private Sub PhotoPageButton_Click(sender As Object, e As RoutedEventArgs)
        ' Add this code.
        If Me.Frame IsNot Nothing Then
            Me.Frame.Navigate(GetType(PhotoPage))
        End If
    End Sub
End Class