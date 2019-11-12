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
Imports Windows.ApplicationModel
Imports Windows.ApplicationModel.Activation
Imports Windows.Storage
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage
        Public Const FEATURE_NAME As String = "Removable Storage Sample"

        Public Shared _ScenarioList As New List(Of Scenario)() From {New Scenario With {.Title = "List removable storage devices", .ClassType = GetType(Microsoft.Samples.Devices.Portable.RemovableStorageSample.S1_ListStorages)},
                                                          New Scenario With {.Title = "Send file to storage device", .ClassType = GetType(Microsoft.Samples.Devices.Portable.RemovableStorageSample.S2_SendToStorage)},
                                                          New Scenario With {.Title = "Get image file from storage device", .ClassType = GetType(Microsoft.Samples.Devices.Portable.RemovableStorageSample.S3_GetFromStorage)},
                                                          New Scenario With {.Title = "Get image file from camera or camera memory (Autoplay)", .ClassType = GetType(Microsoft.Samples.Devices.Portable.RemovableStorageSample.S4_Autoplay)}
                                                         }

        Public Const autoplayScenarioIndex As Integer = 3

        ' Contains the list of Windows.Storage.StorageItem's provided when this application is activated to handle
        ' the supported file types specified in the manifest (here, these will be image files).
        Public Property FileActivationFiles() As IReadOnlyList(Of IStorageItem)
            Get
                Return m_FileActivationFiles
            End Get
            Set(value As IReadOnlyList(Of IStorageItem))
                m_FileActivationFiles = value
            End Set
        End Property
        Private m_FileActivationFiles As IReadOnlyList(Of IStorageItem)

        ' Contains the storage folder (representing a file-system removable storage) when this application is activated by Content Autoplay
        Public Property AutoplayFileSystemDeviceFolder() As StorageFolder
            Get
                Return m_AutoplayFileSystemDeviceFolder
            End Get
            Set(value As StorageFolder)
                m_AutoplayFileSystemDeviceFolder = value
            End Set
        End Property
        Private m_AutoplayFileSystemDeviceFolder As StorageFolder

        ' Contains the device identifier (representing a non-file system removable storage) provided when this application
        ' is activated by Device Autoplay
        Public Property AutoplayNonFileSystemDeviceId() As String
            Get
                Return m_AutoplayNonFileSystemDeviceId
            End Get
            Set(value As String)
                m_AutoplayNonFileSystemDeviceId = value
            End Set
        End Property
        Private m_AutoplayNonFileSystemDeviceId As String

        ' Selects and loads the Autoplay scenario
        Public Sub LoadAutoplayScenario()
            LoadScenario(_scenarioList(MainPage.autoplayScenarioIndex).ClassType)
            scenarios.SelectedIndex = MainPage.autoplayScenarioIndex
        End Sub
    End Class

    Public Class Scenario
        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(value As String)
                m_Title = value
            End Set
        End Property
        Private m_Title As String

        Public Property ClassType() As Type
            Get
                Return m_ClassType
            End Get
            Set(value As Type)
                m_ClassType = value
            End Set
        End Property
        Private m_ClassType As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class

    Partial Public Class App
        Inherits Application
        ''' <summary>
        ''' Invoked when the application is launched to open a specific file or to access
        ''' specific content. This is the entry point for Content Autoplay when camera
        ''' memory is attached to the PC.
        ''' </summary>
        ''' <param name="args">Details about the file activation request.</param>
        Protected Overrides Sub OnFileActivated(args As FileActivatedEventArgs)
            Dim rootFrame As Frame = Nothing
            If Window.Current.Content Is Nothing Then
                rootFrame = New Frame()
                rootFrame.Navigate(GetType(MainPage))
                Window.Current.Content = rootFrame
            Else
                rootFrame = TryCast(Window.Current.Content, Frame)
            End If
            Window.Current.Activate()
            Dim mainPage As MainPage = TryCast(DirectCast(rootFrame.Content, MainPage), MainPage)

            ' Clear any device id so we always use the latest connected device
            mainPage.AutoplayNonFileSystemDeviceId = Nothing

            If args.Verb = "storageDevice" Then
                ' Launched from Autoplay for content. This will return a single storage folder
                ' representing that file system device.
                mainPage.AutoplayFileSystemDeviceFolder = TryCast(args.Files(0), StorageFolder)
                mainPage.FileActivationFiles = Nothing
            Else
                ' Launched to handle a file type.  This will return a list of image files that the user
                ' requests for this application to handle.
                mainPage.FileActivationFiles = args.Files
                mainPage.AutoplayFileSystemDeviceFolder = Nothing
            End If

            ' Select the Autoplay scenario
            mainPage.LoadAutoplayScenario()
        End Sub

        ''' <summary>
        ''' Invoked when the application is activated.
        ''' This is the entry point for Device Autoplay when a device is attached to the PC.
        ''' Other activation kinds (such as search and protocol activation) may also be handled here.
        ''' </summary>
        ''' <param name="args">Details about the activation request.</param>
        Protected Overrides Sub OnActivated(args As IActivatedEventArgs)
            If args.Kind = ActivationKind.Device Then
                Dim rootFrame As Frame = Nothing
                If Window.Current.Content Is Nothing Then
                    rootFrame = New Frame()
                    rootFrame.Navigate(GetType(MainPage))
                    Window.Current.Content = rootFrame
                Else
                    rootFrame = TryCast(Window.Current.Content, Frame)
                End If
                Window.Current.Activate()
                Dim mainPage As MainPage = TryCast(DirectCast(rootFrame.Content, MainPage), MainPage)

                ' Launched from Autoplay for device, receiving the device information identifier.
                Dim deviceArgs As DeviceActivatedEventArgs = TryCast(args, DeviceActivatedEventArgs)
                mainPage.AutoplayNonFileSystemDeviceId = deviceArgs.DeviceInformationId

                ' Clear any saved drive or file so we always use the latest connected device
                mainPage.AutoplayFileSystemDeviceFolder = Nothing
                mainPage.FileActivationFiles = Nothing

                ' Select the Autoplay scenario
                mainPage.LoadAutoplayScenario()
            End If
        End Sub
    End Class
End Namespace
