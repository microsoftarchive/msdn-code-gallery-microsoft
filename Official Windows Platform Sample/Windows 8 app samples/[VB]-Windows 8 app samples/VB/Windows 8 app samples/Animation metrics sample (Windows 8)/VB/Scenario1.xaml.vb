'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports Windows.UI.Core.AnimationMetrics

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

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    ''' <summary>
    ''' Retrieves the specified metrics and displays them in textual form.
    ''' </summary>
    ''' <param name="effect">The AnimationEffect whose metrics are to be displayed.</param>
    ''' <param name="target">The AnimationEffecTarget whose metrics are to be displayed.</param>
    Private Sub DisplayMetrics(effect As AnimationEffect, target As AnimationEffectTarget)
        Dim s = New System.Text.StringBuilder
        Dim animationDescription As New AnimationDescription(effect, target)
        s.AppendFormat("Stagger delay = {0}ms", animationDescription.StaggerDelay.Milliseconds)
        s.AppendLine()
        s.AppendFormat("Stagger delay factor = {0}", animationDescription.StaggerDelayFactor)
        s.AppendLine()
        s.AppendFormat("Delay limit = {0}ms", animationDescription.DelayLimit.Milliseconds)
        s.AppendLine()
        s.AppendFormat("ZOrder = {0}", animationDescription.ZOrder)
        s.AppendLine()
        s.AppendLine()

        Dim animationIndex As Integer = 0
        For Each animationVar In animationDescription.Animations
            s.AppendFormat("Animation #{0}:", System.Threading.Interlocked.Increment(animationIndex))
            s.AppendLine()

            Select Case animationVar.Type
                Case PropertyAnimationType.Scale
                    If True Then
                        Dim scale As ScaleAnimation = TryCast(animationVar, ScaleAnimation)
                        s.AppendLine("Type = Scale")
                        If scale.InitialScaleX.HasValue Then
                            s.AppendFormat("InitialScaleX = {0}", scale.InitialScaleX.Value)
                            s.AppendLine()
                        End If
                        If scale.InitialScaleY.HasValue Then
                            s.AppendFormat("InitialScaleY = {0}", scale.InitialScaleY.Value)
                            s.AppendLine()
                        End If
                        s.AppendFormat("FinalScaleX = {0}", scale.FinalScaleX)
                        s.AppendLine()
                        s.AppendFormat("FinalScaleY = {0}", scale.FinalScaleY)
                        s.AppendLine()
                        s.AppendFormat("Origin = {0}, {1}", scale.NormalizedOrigin.X, scale.NormalizedOrigin.Y)
                        s.AppendLine()
                    End If
                    Exit Select
                Case PropertyAnimationType.Translation
                    s.AppendLine("Type = Translation")
                    Exit Select
                Case PropertyAnimationType.Opacity
                    If True Then
                        Dim opacity As OpacityAnimation = TryCast(animationVar, OpacityAnimation)
                        s.AppendLine("Type = Opacity")
                        If opacity.InitialOpacity.HasValue Then
                            s.AppendFormat("InitialOpacity = {0}", opacity.InitialOpacity.Value)
                            s.AppendLine()
                        End If
                        s.AppendFormat("FinalOpacity = {0}", opacity.FinalOpacity)
                        s.AppendLine()
                    End If
                    Exit Select
            End Select

            s.AppendFormat("Delay = {0}ms", animationVar.Delay.Milliseconds)
            s.AppendLine()
            s.AppendFormat("Duration = {0}ms", animationVar.Duration.Milliseconds)
            s.AppendLine()
            s.AppendFormat("Cubic Bezier control points")
            s.AppendLine()
            s.AppendFormat("    X1 = {0}, Y1 = {1}", animationVar.Control1.X, animationVar.Control1.Y)
            s.AppendLine()
            s.AppendFormat("    X2 = {0}, Y2 = {1}", animationVar.Control2.X, animationVar.Control2.Y)
            s.AppendLine()
            s.AppendLine()
        Next

        Metrics.Text = s.ToString
    End Sub

    ''' <summary>
    ''' When the selection changes, update the output window.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Animations_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim lb As ListBox = TryCast(sender, ListBox)
        If lb IsNot Nothing Then
            Dim selectedListBoxItem As Object = lb.SelectedItem
            If selectedListBoxItem Is AddToListAdded Then
                DisplayMetrics(AnimationEffect.AddToList, AnimationEffectTarget.Added)
            ElseIf selectedListBoxItem Is AddToListAffected Then
                DisplayMetrics(AnimationEffect.AddToList, AnimationEffectTarget.Affected)
            ElseIf selectedListBoxItem Is EnterPage Then
                DisplayMetrics(AnimationEffect.EnterPage, AnimationEffectTarget.Primary)
            End If
        End If
    End Sub
End Class
