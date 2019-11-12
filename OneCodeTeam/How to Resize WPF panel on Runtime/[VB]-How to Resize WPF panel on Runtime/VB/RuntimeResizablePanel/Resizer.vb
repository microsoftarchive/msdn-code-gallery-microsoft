'****************************** Module Header ******************************
'Module Name:  Resizer.vb
'Project:      RuntimeResizablePanel
'Copyright (c) Microsoft Corporation.

'Resizer is a class to represent the Thumb used for resizing.
'This helps us to object for each Thumb direction and resize only sides where the change is made.

'This source is subject to the Microsoft Public License.
'See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'All other rights reserved.

'THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************

Imports System.Windows
Imports System.Windows.Controls
Imports System.Collections.Generic
Imports System.Windows.Controls.Primitives



''' <summary>
''' Class to represent the Thumb used for resizing the panel.
''' </summary>
Public Class Resizer
    Inherits Thumb
    ''' <summary>
    ''' Direction to resize.
    ''' </summary>
    Public Shared ThumbDirectionProperty As DependencyProperty = DependencyProperty.Register("ThumbDirection", GetType(ResizeDirections), GetType(Resizer))

    Public Property ThumbDirection() As ResizeDirections
        Get
            Return CType(GetValue(ThumbDirectionProperty), ResizeDirections)
        End Get
        Set(value As ResizeDirections)
            SetValue(Resizer.ThumbDirectionProperty, value)
        End Set
    End Property

    Shared Sub New()
        ' This will allow us to create a Style with target type Resizer.
        DefaultStyleKeyProperty.OverrideMetadata(GetType(Resizer), New FrameworkPropertyMetadata(GetType(Resizer)))
    End Sub

    Public Sub New()
        AddHandler DragDelta, AddressOf Resizer_DragDelta
    End Sub

    Private Sub Resizer_DragDelta(sender As Object, e As DragDeltaEventArgs)
        Dim designerItem As Control = TryCast(Me.DataContext, Control)

        If designerItem IsNot Nothing Then
            Dim deltaVertical As Double, deltaHorizontal As Double

            Select Case ThumbDirection

                Case ResizeDirections.TopLeft
                    deltaVertical = ResizeTop(e, designerItem)
                    deltaHorizontal = ResizeLeft(e, designerItem)
                    Exit Select
                Case ResizeDirections.Left
                    deltaHorizontal = ResizeLeft(e, designerItem)
                    Exit Select
                Case ResizeDirections.BottomLeft
                    deltaVertical = ResizeBottom(e, designerItem)
                    deltaHorizontal = ResizeLeft(e, designerItem)
                    Exit Select
                Case ResizeDirections.Bottom
                    deltaVertical = ResizeBottom(e, designerItem)
                    Exit Select
                Case ResizeDirections.BottomRight
                    deltaVertical = ResizeBottom(e, designerItem)
                    deltaHorizontal = ResizeRight(e, designerItem)
                    Exit Select
                Case ResizeDirections.Right
                    deltaHorizontal = ResizeRight(e, designerItem)
                    Exit Select
                Case ResizeDirections.TopRight
                    deltaVertical = ResizeTop(e, designerItem)
                    deltaHorizontal = ResizeRight(e, designerItem)
                    Exit Select
                Case ResizeDirections.Top
                    deltaVertical = ResizeTop(e, designerItem)
                    Exit Select
                Case Else
                    Exit Select


            End Select
        End If

        e.Handled = True
    End Sub

    Private Shared Function ResizeRight(e As DragDeltaEventArgs, designerItem As Control) As Double
        Dim deltaHorizontal As Double
        deltaHorizontal = Math.Min(-e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth)
        designerItem.Width -= deltaHorizontal
        Return deltaHorizontal
    End Function

    Private Shared Function ResizeTop(e As DragDeltaEventArgs, designerItem As Control) As Double
        Dim deltaVertical As Double
        deltaVertical = Math.Min(e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight)
        Canvas.SetTop(designerItem, Canvas.GetTop(designerItem) + deltaVertical)
        designerItem.Height -= deltaVertical
        Return deltaVertical
    End Function

    Private Shared Function ResizeLeft(e As DragDeltaEventArgs, designerItem As Control) As Double
        Dim deltaHorizontal As Double
        deltaHorizontal = Math.Min(e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth)
        Canvas.SetLeft(designerItem, Canvas.GetLeft(designerItem) + deltaHorizontal)
        designerItem.Width -= deltaHorizontal
        Return deltaHorizontal
    End Function

    Private Shared Function ResizeBottom(e As DragDeltaEventArgs, designerItem As Control) As Double
        Dim deltaVertical As Double
        deltaVertical = Math.Min(-e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight)
        designerItem.Height -= deltaVertical
        Return deltaVertical
    End Function

End Class

''' <summary>
''' Enum to maintain the direction user can resize.
''' </summary>
Public Enum ResizeDirections
    TopLeft = 0
    Left
    BottomLeft
    Bottom
    BottomRight
    Right
    TopRight
    Top
End Enum
