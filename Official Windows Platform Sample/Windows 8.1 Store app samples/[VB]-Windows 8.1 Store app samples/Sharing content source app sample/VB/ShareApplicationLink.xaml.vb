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
Imports System.Collections.ObjectModel
Imports SDKTemplate
Imports Windows.ApplicationModel.DataTransfer
Imports Windows.UI.Xaml.Controls
Imports Windows.Storage.Streams
Imports Windows.UI
Imports Windows.UI.Xaml.Data

Partial Public NotInheritable Class ShareApplicationLink
    Inherits SDKTemplate.Common.SharePage

    Public Sub New()
        Me.InitializeComponent()

        ' Populate the ApplicationLinkComboBox with the deep links for all of the scenarios
        Dim scenarioList As New ObservableCollection(Of Object)()
        For Each scenario As Scenario In MainPage.scenariosList
            scenarioList.Add(scenario.ApplicationLink)
        Next scenario
        ApplicationLinkComboBox.ItemsSource = scenarioList
        ApplicationLinkComboBox.SelectedItem = ApplicationLink ' Default selection to the deep link for this scenario
    End Sub

    Protected Overrides Function GetShareContent(ByVal request As DataRequest) As Boolean
        Dim succeeded As Boolean = False

        Dim selectedApplicationLink As Uri = TryCast(ApplicationLinkComboBox.SelectedItem, Uri)
        If selectedApplicationLink IsNot Nothing Then
            Dim requestData As DataPackage = request.Data
            requestData.Properties.Title = TitleInputBox.Text
            requestData.Properties.Description = DescriptionInputBox.Text ' The description is optional.
            requestData.Properties.ContentSourceApplicationLink = ApplicationLink
            requestData.SetApplicationLink(selectedApplicationLink)

            ' Place the selected logo and the background color in the data package properties
            If MicrosoftLogo.IsChecked.Value Then
                requestData.Properties.Square30x30Logo = RandomAccessStreamReference.CreateFromUri(New Uri("ms-appx:///assets/microsoftLogo.png"))
                requestData.Properties.LogoBackgroundColor = GetLogoBackgroundColor()
            ElseIf VisualStudioLogo.IsChecked.Value Then
                requestData.Properties.Square30x30Logo = RandomAccessStreamReference.CreateFromUri(New Uri("ms-appx:///assets/visualStudioLogo.png"))
                requestData.Properties.LogoBackgroundColor = GetLogoBackgroundColor()
            End If

            succeeded = True
        Else
            request.FailWithDisplayText("Select the application link you would like to share and try again.")
        End If

        Return succeeded
    End Function

    ''' <summary>
    ''' Reads out the values of the colors and constructs a color from their values.
    ''' </summary>
    ''' <returns></returns>
    Private Function GetLogoBackgroundColor() As Color
        ' The values are guaranteed to be within the appropriate byte range due to setting a minimum of 0
        ' and a maximum of 255 on the sliders. However, it is necessary to convert them to bytes because
        ' the slider exposes its value as a double.
        Dim backgroundColor As New Color()
        backgroundColor.R = Convert.ToByte(RedSlider.Value)
        backgroundColor.G = Convert.ToByte(GreenSlider.Value)
        backgroundColor.B = Convert.ToByte(BlueSlider.Value)
        backgroundColor.A = Convert.ToByte(AlphaSlider.Value)

        Return backgroundColor
    End Function
End Class

''' <summary>
''' A value converter which negates a bool. This converter is used for the color sliders to bind their enabled
''' state to the negation of the selection state of the default logo radio button.
''' </summary>
Public NotInheritable Class NegatingBoolConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal language As String) As Object Implements IValueConverter.Convert
        Return Not CBool(value)
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal language As String) As Object Implements IValueConverter.ConvertBack
        Return Not CBool(value)
    End Function
End Class
