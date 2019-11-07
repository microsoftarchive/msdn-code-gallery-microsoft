Imports Microsoft.VisualStudio.DebuggerVisualizers
Imports System.Linq.Expressions

Partial Public Class Form1
    Inherits Form

    Private Sub ShowVisualizerButton_Click() Handles ShowVisualizerButton.Click
        Dim expr As Expression(Of Func(Of Integer, Boolean)) = Function(x) x = 10
        Dim host = New VisualizerDevelopmentHost(expr, _
                    GetType(ExpressionTreeVisualizer), _
                    GetType(ExpressionTreeVisualizerObjectSource))
        host.ShowVisualizer(Me)
    End Sub

End Class
