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
Imports Windows.ApplicationModel

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Private Function VersionString(version As PackageVersion) As String
        VersionString = String.Format("{0}.{1}.{2}.{3}", _
                                      version.Major, version.Minor, version.Build, version.Revision)
    End Function

    Private Function ArchitectureString(architecture As Windows.System.ProcessorArchitecture) As String
        Select Case architecture
            Case Windows.System.ProcessorArchitecture.X86
                ArchitectureString = "x86"
            Case Windows.System.ProcessorArchitecture.Arm
                ArchitectureString = "arm"
            Case Windows.System.ProcessorArchitecture.X64
                ArchitectureString = "x64"
            Case Windows.System.ProcessorArchitecture.Neutral
                ArchitectureString = "neutral"
            Case Windows.System.ProcessorArchitecture.Unknown
                ArchitectureString = "unknown"
            Case Else
                ArchitectureString = "???"
        End Select
    End Function

    Private Sub GetPackage_Click(sender As Object, e As RoutedEventArgs)
        Dim package As Package = Package.Current
        Dim packageId As PackageId = package.Id

        OutputTextBlock.Text = String.Format("Name: ""{0}""" & vbLf & _
                                             "Version: {1}" & vbLf & _
                                             "Architecture: {2}" & vbLf & _
                                             "ResourceId: ""{3}""" & vbLf & _
                                             "Publisher: ""{4}""" & vbLf & _
                                             "PublisherId: ""{5}""" & vbLf & _
                                             "FullName: ""{6}""" & vbLf & _
                                             "FamilyName: ""{7}""" & vbLf & _
                                             "IsFramework: {8}", _
                                             packageId.Name, _
                                             VersionString(packageId.Version), _
                                             ArchitectureString(packageId.Architecture), _
                                             packageId.ResourceId, _
                                             packageId.Publisher, _
                                             packageId.PublisherId, _
                                             packageId.FullName, _
                                             packageId.FamilyName, _
                                             package.IsFramework)
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub
End Class
