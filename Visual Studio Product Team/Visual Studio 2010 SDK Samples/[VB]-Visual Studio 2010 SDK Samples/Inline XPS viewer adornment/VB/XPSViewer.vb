'***************************************************************************
'
'    Copyright (c) Microsoft Corporation. All rights reserved.
'    This code is licensed under the Visual Studio SDK license terms.
'    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'***************************************************************************

Imports System.IO
Imports System.IO.Packaging
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Windows.Threading
Imports System.Windows.Xps.Packaging
Imports Microsoft.VisualStudio.Text
Imports Microsoft.VisualStudio.Text.Editor
Imports Microsoft.VisualStudio.Text.Formatting
Imports System.Collections.ObjectModel

Namespace InlineXPSViewer

    '''<summary>
    '''InlineXPSViewer places an xps viewer under line containing "ShowXPS[filename]"
    '''</summary>
    Public Class lineXPSViewer
        Implements ILineTransformSource

        Private adormentLayer As IAdornmentLayer
        Private wpfTextView As IWpfTextView

        Private Shared mapping As New Dictionary(Of String, XpsDocument)
        Private linesWithAdornments As New HashSet(Of Object)
        Private Const ViewerHeight As Integer = 350
        Private Const ViewerWidth As Integer = 600

        Public Sub New(ByVal view As IWpfTextView)
            wpfTextView = view
            adormentLayer = view.GetAdornmentLayer("InlineXPSViewer")
            'Listen to event that changes the layout
            AddHandler wpfTextView.LayoutChanged, AddressOf OnLayoutChanged
            AddHandler wpfTextView.Closed, AddressOf OnViewClosed
        End Sub


        Public Function GetLineTransform(ByVal line As ITextViewLine, ByVal yPosition As Double, ByVal placement As ViewRelativePosition) As LineTransform Implements ILineTransformSource.GetLineTransform
            Dim document As XpsDocument = Me.DocumentForLine(line)
            If document IsNot Nothing Then
                Return New LineTransform(0, ViewerHeight, line.LineTransform.VerticalScale)
            Else
                Return New LineTransform(0, 0, line.LineTransform.VerticalScale)
            End If
        End Function

        ''' <summary>
        ''' Get the XpsDocument in the cache.
        ''' </summary>
        ''' <param name="line"></param>
        ''' <returns></returns>
        Private Function DocumentForLine(ByVal line As ITextViewLine) As XpsDocument
            Dim document As XpsDocument = Nothing
            Dim file As String = MatchFileNamePattern(line.Extent.GetText())

            If Not String.IsNullOrEmpty(file) Then
                SyncLock mapping
                    mapping.TryGetValue(file, document)
                End SyncLock
            End If

            Return document
        End Function

        ''' <summary>
        ''' Scan the line and return the xps filename if the line contains 
        ''' the pattern "ShowXPS[path to filename]"
        ''' </summary>
        ''' <param name="line"></param>
        ''' <returns></returns>
        Private Function MatchFileNamePattern(ByVal line As String) As String
            Return (New Regex("ShowXPS\[(?<fileName>.*?)\]")).Match(line).Groups("fileName").Value
        End Function

        ''' <summary>
        ''' Check file existence and build XPS document in the background thread.
        ''' Once finished, we can call back to the UI thread to refresh UI.
        ''' </summary>
        ''' <param name="fileNames"></param>
        ''' <param name="textViewLines"></param>
        ''' <param name="dispatcher"></param>
        Private Sub PrepareXpsDocuments(ByVal fileNames As List(Of String), ByVal dispatcher As Dispatcher)
            Dim foundNewXps As Boolean = False

            For Each fileName As String In fileNames
                Dim xps As XpsDocument
                ' create XpsDocument object from the file given 
                If File.Exists(fileName) Then
                    Try
                        xps = New XpsDocument(fileName, FileAccess.Read)
                        SyncLock mapping
                            If wpfTextView.IsClosed Then
                                xps.Close()
                                Return
                            End If
                            mapping(fileName) = xps
                        End SyncLock
                        foundNewXps = True
                    Catch
                    End Try
                End If
            Next fileName

            If foundNewXps Then
                ' Now call back to UI thread
                dispatcher.BeginInvoke(New Action(Sub()
                                                      If Not wpfTextView.IsClosed Then
                                                          wpfTextView.DisplayTextLineContainingBufferPosition(wpfTextView.TextViewLines(0).Start, wpfTextView.TextViewLines(0).Top - wpfTextView.ViewportTop, ViewRelativePosition.Top)
                                                      End If
                                                  End Sub))
            End If

        End Sub

        ''' <summary>
        ''' Release all XpsDocument once the view is closed.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub OnViewClosed(ByVal sender As Object, ByVal e As EventArgs)
            SyncLock mapping
                For Each doc In mapping.Values
                    If doc IsNot Nothing Then
                        doc.Close()
                    End If
                Next doc
                mapping.Clear()
            End SyncLock
        End Sub

        ''' <summary>
        ''' On layout change add the adornment to any reformatted lines
        ''' </summary>
        Private Sub OnLayoutChanged(ByVal sender As Object, ByVal e As TextViewLayoutChangedEventArgs)
            Me.UpdateVisuals(e.NewOrReformattedLines)
        End Sub

        ''' <summary>
        ''' Updates Visuals
        ''' </summary>
        ''' <param name="textViewLines"></param>
        Private Sub UpdateVisuals(ByVal newOrReformattedLines As ICollection(Of ITextViewLine))
            Dim fileNames As New List(Of String)
            'Check new or reformatted lines where we might have new documents to load
            For Each line As ITextViewLine In newOrReformattedLines
                Dim content As String = line.Extent.GetText()
                Dim fileName As String = MatchFileNamePattern(content)
                If Not String.IsNullOrEmpty(fileName) Then
                    SyncLock mapping
                        If Not mapping.ContainsKey(fileName) Then
                            mapping(fileName) = Nothing
                            fileNames.Add(fileName)
                        End If
                    End SyncLock
                End If
            Next line

            'Check file existence and build XPS document in the background thread
            If fileNames.Count <> 0 Then
                ThreadPool.QueueUserWorkItem(Sub() PrepareXpsDocuments(fileNames, Dispatcher.CurrentDispatcher))
            End If

            'Make sure that each line that should have an adornment actually does.
            'We need to check all lines since we are loading the visuals asynchronously
            'on the background thread.
            For Each line As IWpfTextViewLine In wpfTextView.TextViewLines
                'Don’t do anything if we already have an adornment for the line.
                If Not linesWithAdornments.Contains(line.IdentityTag) Then
                    Dim document As XpsDocument = Me.DocumentForLine(line)
                    If document IsNot Nothing Then
                        Dim viewer As New DocumentViewer
                        viewer.Document = document.GetFixedDocumentSequence()
                        viewer.Width = ViewerWidth
                        viewer.Height = ViewerHeight

                        Canvas.SetLeft(viewer, line.Left)
                        Canvas.SetTop(viewer, line.TextBottom)

                        Dim transform As New MatrixTransform(1.0, 0.0, 0.0, line.LineTransform.VerticalScale, 0.0, 0.0)
                        viewer.RenderTransform = transform
                        adormentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, line.Extent, line.IdentityTag, viewer, AddressOf OnAdornmentRemoved)
                        linesWithAdornments.Add(line.IdentityTag)
                    End If
                End If
            Next line
        End Sub

        ''' <summary>
        ''' Remove adornment callback.
        ''' </summary>
        ''' <param name="tag"></param>
        ''' <param name="element"></param>
        Private Sub OnAdornmentRemoved(ByVal tag As Object, ByVal element As UIElement)
            linesWithAdornments.Remove(tag)
        End Sub

    End Class
End Namespace
