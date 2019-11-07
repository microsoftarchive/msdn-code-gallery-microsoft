'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System.Collections.Generic
Imports System
Imports SimpleImaging
Imports Windows.Graphics.Imaging
Imports Windows.Storage
Imports Windows.Storage.FileProperties
Imports Windows.Storage.Pickers
Imports Windows.UI.ViewManagement
Imports System.Threading.Tasks

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        Public Const FEATURE_NAME As String = "Simple Imaging Sample"

        ' This list defines the scenarios covered in this sample and their titles.
        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "Image Editor (Windows.Storage.FileProperties)", .ClassType = GetType(ImagingProperties)},
                                                                   New Scenario With {.Title = "Image Editor (Windows.Graphics.Imaging)", .ClassType = GetType(ImagingTransforms)}
                                                                   }

        Public LaunchArgs As Windows.ApplicationModel.Activation.LaunchActivatedEventArgs

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

    ''' <summary>
    ''' Contains helper functionality, including handling/converting EXIF orientation values.
    ''' </summary>
    Public Module Helpers

        ''' <summary>
        ''' Retrieves all of the file extensions supported by the bitmap codecs on the system,
        ''' and inserts them into the provided fileTypeFilter parameter.
        ''' </summary>
        ''' <param name="fileTypeFilter">FileOpenPicker.FileTypeFilter member</param>
        Public Sub FillDecoderExtensions(fileTypeFilter As IList(Of String))
            Dim codecInfoList As IReadOnlyList(Of BitmapCodecInformation) = BitmapDecoder.GetDecoderInformationEnumerator()

            For Each decoderInfo As BitmapCodecInformation In codecInfoList
                ' Each bitmap codec contains a list of file extensions it supports; add each list item
                ' to fileTypeFilter.
                For Each extension As String In decoderInfo.FileExtensions
                    fileTypeFilter.Add(extension)
                Next
            Next
        End Sub

        ''' <summary>
        ''' Returns a StorageFile containing an image in a supported format.
        ''' </summary>
        Public Async Function GetFileFromOpenPickerAsync() As Task(Of StorageFile)
            ''' Attempt to ensure that the view is not snapped, otherwise the picker will not display.
            If ((ApplicationView.Value = ApplicationViewState.Snapped) AndAlso
                (ApplicationView.TryUnsnap() = False)) Then
                Throw New Exception("File picker cannot display in snapped view.")
            End If

            Dim picker As New FileOpenPicker()
            FillDecoderExtensions(picker.FileTypeFilter)
            Dim file As StorageFile = Await picker.PickSingleFileAsync()

            If file Is Nothing Then
                Throw New Exception("User did not select a file.")
            End If

            Return file
        End Function

        ''' <summary>
        ''' Returns a StorageFile that the user has selected as the encode destination.
        ''' Selects a few common encoding formats.
        ''' </summary>
        Public Async Function GetFileFromSavePickerAsync() As Task(Of StorageFile)
            ''' Attempt to ensure that the view is not snapped, otherwise the picker will not display.
            If ((ApplicationView.Value = ApplicationViewState.Snapped) AndAlso
                (ApplicationView.TryUnsnap() = False)) Then
                Throw New Exception("File picker cannot display in snapped view.")
            End If

            Dim picker As New FileSavePicker()
            picker.FileTypeChoices.Add("JPEG image", New String() {".jpg"})
            picker.FileTypeChoices.Add("PNG image", New String() {".png"})
            picker.FileTypeChoices.Add("BMP image", New String() {".bmp"})
            picker.DefaultFileExtension = ".png"
            picker.SuggestedFileName = "Output file"
            Dim file As StorageFile = Await picker.PickSaveFileAsync()

            If file Is Nothing Then
                Throw New Exception("User did not select a file.")
            End If

            Return file
        End Function

        ''' <summary>
        ''' Converts a PhotoOrientation value into a human readable string.
        ''' The text is adapted from the EXIF specification.
        ''' Note that PhotoOrientation uses a counterclockwise convention,
        ''' while the EXIF spec uses a clockwise convention.
        ''' </summary>
        Public Function GetOrientationString(input As PhotoOrientation) As String
            Select Case input
                Case PhotoOrientation.Normal
                    Return "No rotation"
                Case PhotoOrientation.FlipHorizontal
                    Return "Flip horizontally"
                Case PhotoOrientation.Rotate180
                    Return "Rotate 180° clockwise"
                Case PhotoOrientation.FlipVertical
                    Return "Flip vertically"
                Case PhotoOrientation.Transpose
                    Return "Rotate 270° clockwise, then flip horizontally"
                Case PhotoOrientation.Rotate270
                    Return "Rotate 90° clockwise"
                Case PhotoOrientation.Transverse
                    Return "Rotate 90° clockwise, then flip horizontally"
                Case PhotoOrientation.Rotate90
                    Return "Rotate 270° clockwise"
                Case PhotoOrientation.Unspecified
                    Return "Unspecified"
                Case Else
                    Return "Unspecified"
            End Select

        End Function

        ''' <summary>
        ''' Converts a Windows.Storage.FileProperties.PhotoOrientation value into a
        ''' Windows.Graphics.Imaging.BitmapRotation value.
        ''' For PhotoOrientation values reflecting a flip/mirroring operation, returns "None";
        ''' therefore this is a potentially lossy transformation.
        ''' Note that PhotoOrientation uses a counterclockwise convention,
        ''' while BitmapRotation uses a clockwise convention.
        ''' </summary>
        Public Function ConvertToBitmapRotation(input As PhotoOrientation) As BitmapRotation
            Select Case input
                Case PhotoOrientation.Normal
                    Return BitmapRotation.None
                Case PhotoOrientation.Rotate270
                    Return BitmapRotation.Clockwise90Degrees
                Case PhotoOrientation.Rotate180
                    Return BitmapRotation.Clockwise180Degrees
                Case PhotoOrientation.Rotate90
                    Return BitmapRotation.Clockwise270Degrees                    
                Case Else
                    ' Ignore any flip/mirrored values.
                    Return BitmapRotation.None
            End Select
        End Function

        ''' <summary>
        ''' Converts a Windows.Graphics.Imaging.BitmapRotation value into a
        ''' Windows.Storage.FileProperties.PhotoOrientation value.
        ''' Note that PhotoOrientation uses a counterclockwise convention,
        ''' while BitmapRotation uses a clockwise convention.
        ''' </summary>
        Public Function ConvertToPhotoOrientation(input As BitmapRotation) As PhotoOrientation
            Select Case input
                Case BitmapRotation.None
                    Return PhotoOrientation.Normal
                Case BitmapRotation.Clockwise90Degrees
                    Return PhotoOrientation.Rotate270
                Case BitmapRotation.Clockwise180Degrees
                    Return PhotoOrientation.Rotate180
                Case BitmapRotation.Clockwise270Degrees
                    Return PhotoOrientation.Rotate90
                Case Else
                    Return PhotoOrientation.Normal
            End Select
        End Function

        ''' <summary>
        ''' Converts an unsigned integer, corresponding to the EXIF orientation flag, to a
        ''' Windows.Storage.FileProperties.PhotoOrientation value. Note that the actual PhotoOrientation
        ''' enumeration values directly map to the EXIF orientation flag; this method simply provides
        ''' a typesafe means of converting between the two in C#, in addition to handling the Unspecified case.
        ''' </summary>
        Public Function ConvertToPhotoOrientation(input As UShort) As PhotoOrientation
            Select Case input
                Case 1
                    Return PhotoOrientation.Normal
                Case 2
                    Return PhotoOrientation.FlipHorizontal
                Case 3
                    Return PhotoOrientation.Rotate180
                Case 4
                    Return PhotoOrientation.FlipVertical
                Case 5
                    Return PhotoOrientation.Transpose
                Case 6
                    Return PhotoOrientation.Rotate270
                Case 7
                    Return PhotoOrientation.Transverse
                Case 8
                    Return PhotoOrientation.Rotate90
                Case Else
                    Return PhotoOrientation.Unspecified
            End Select
        End Function

        ''' <summary>
        ''' Counterpart to ConvertToPhotoOrientation(ushort input), maps PhotoOrientation enumeration
        ''' values to an unsigned 16-bit integer representing the EXIF orientation flag.
        ''' </summary>
        Public Function ConvertToExifOrientationFlag(input As PhotoOrientation) As UShort
            Select Case input
                Case PhotoOrientation.Normal
                    Return 1
                Case PhotoOrientation.FlipHorizontal
                    Return 2
                Case PhotoOrientation.Rotate180
                    Return 3
                Case PhotoOrientation.FlipVertical
                    Return 4
                Case PhotoOrientation.Transpose
                    Return 5
                Case PhotoOrientation.Rotate270
                    Return 6
                Case PhotoOrientation.Transverse
                    Return 7
                Case PhotoOrientation.Rotate90
                    Return 8
                Case Else
                    Return 1
            End Select
        End Function

        ''' <summary>
        ''' "Adds" two PhotoOrientation values. For simplicity, does not handle any values with
        ''' flip/mirroring; therefore this is a potentially lossy transformation.
        ''' Note that PhotoOrientation uses a counterclockwise convention.
        ''' </summary>
        Public Function AddPhotoOrientation(value1 As PhotoOrientation, value2 As PhotoOrientation) As PhotoOrientation
            Select Case value2
                Case PhotoOrientation.Rotate90
                    Return Add90DegreesCCW(value1)
                Case PhotoOrientation.Rotate180
                    Return Add90DegreesCCW(Add90DegreesCCW(value1))
                Case PhotoOrientation.Rotate270
                    Return Add90DegreesCW(value1)
                Case PhotoOrientation.Normal
                    ' Ignore any values with flip/mirroring.
                    Return value1
                Case Else
                    ' Ignore any values with flip/mirroring.
                    Return value1
            End Select

        End Function

        ''' <summary>
        ''' "Add" 90 degrees clockwise rotation to a PhotoOrientation value.
        ''' For simplicity, does not handle any values with flip/mirroring; therefore this is a potentially
        ''' lossy transformation.
        ''' Note that PhotoOrientation uses a counterclockwise convention.
        ''' </summary>
        Public Function Add90DegreesCW(input As PhotoOrientation) As PhotoOrientation

            Select Case input
                Case PhotoOrientation.Normal
                    Return PhotoOrientation.Rotate270
                Case PhotoOrientation.Rotate90
                    Return PhotoOrientation.Normal
                Case PhotoOrientation.Rotate180
                    Return PhotoOrientation.Rotate90
                Case PhotoOrientation.Rotate270
                    Return PhotoOrientation.Rotate180
                Case Else
                    ' Ignore any values with flip/mirroring.
                    Return PhotoOrientation.Unspecified
            End Select
        End Function

        ''' <summary>
        ''' "Add" 90 degrees counter-clockwise rotation to a PhotoOrientation value.
        ''' For simplicity, does not handle any values with flip/mirroring; therefore this is a potentially
        ''' lossy transformation.
        ''' Note that PhotoOrientation uses a counterclockwise convention.
        ''' </summary>
        Public Function Add90DegreesCCW(input As PhotoOrientation) As PhotoOrientation
            Select Case input
                Case PhotoOrientation.Normal
                    Return PhotoOrientation.Rotate90
                Case PhotoOrientation.Rotate90
                    Return PhotoOrientation.Rotate180
                Case PhotoOrientation.Rotate180
                    Return PhotoOrientation.Rotate270
                Case PhotoOrientation.Rotate270
                    Return PhotoOrientation.Normal
                Case Else
                    ' Ignore any values with flip/mirroring.
                    Return PhotoOrientation.Unspecified
            End Select
        End Function
    End Module
End Namespace
