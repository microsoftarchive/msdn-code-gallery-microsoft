'******************************** Module Header **********************************'
' Module Name:  ExpressionEncoderWrapper.cs
' Project:      VBExtractAudioFile
' Copyright (c) Microsoft Corporation.
'
' The ExpressionEncoderWrapper class contains helper methods for converting and 
' extracting .MP3/.MP4/.WMA into .MP4/.WMA file format.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**********************************************************************************'

Imports Microsoft.Expression.Encoder
Imports Microsoft.Expression.Encoder.Profiles
Imports System.Collections.ObjectModel



Public Class ExpressionEncoderWrapper
    ''' <summary>
    ''' Extract audio file.
    ''' </summary>
    ''' <param name="sourceAudioFile">The full path of the source audio file</param>
    ''' <param name="outputDirectory">The output directory</param>
    ''' <param name="outputAudioType_Renamed">Output audio format (.mp4/.wma)</param>
    ''' <param name="startpoint">
    ''' The extracting startpoint in the source audio file in milliseconds
    ''' </param>
    ''' <param name="endpoint">
    ''' The extracting endpoint in the source audio file in milliseconds
    ''' </param>
    ''' <returns>The full path of the output file name</returns>
    Public Shared Function ExtractAudio(ByVal sourceAudioFile As String, ByVal outputDirectory As String, ByVal outputType As OutputAudioType, ByVal startpoint As Double, ByVal endpoint As Double) As String
        Using [job] As New Job()
            Dim src As New MediaItem(sourceAudioFile)
            Select Case outputType
                Case OutputAudioType.MP4
                    src.OutputFormat = New MP4OutputFormat()
                    src.OutputFormat.AudioProfile = New AacAudioProfile()
                    src.OutputFormat.AudioProfile.Codec = AudioCodec.AAC
                    src.OutputFormat.AudioProfile.BitsPerSample = 24
                Case OutputAudioType.WMA
                    src.OutputFormat = New WindowsMediaOutputFormat()
                    src.OutputFormat.AudioProfile = New WmaAudioProfile()
                    src.OutputFormat.AudioProfile.Bitrate = New VariableConstrainedBitrate(128, 192)
                    src.OutputFormat.AudioProfile.Codec = AudioCodec.WmaProfessional
                    src.OutputFormat.AudioProfile.BitsPerSample = 24
            End Select

            Dim spanStart As TimeSpan = TimeSpan.FromMilliseconds(startpoint)
            src.Sources(0).Clips(0).StartTime = spanStart
            Dim spanEnd As TimeSpan = TimeSpan.FromMilliseconds(endpoint)
            src.Sources(0).Clips(0).EndTime = spanEnd

            [job].MediaItems.Add(src)
            [job].OutputDirectory = outputDirectory
            [job].Encode()

            Return [job].MediaItems(0).ActualOutputFileFullPath
        End Using
    End Function
End Class

Public Enum OutputAudioType
    WMA
    MP4
End Enum

