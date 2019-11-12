Imports System.Linq.Expressions
imports Microsoft.VisualStudio.DebuggerVisualizers

<Assembly: DebuggerVisualizer(GetType(ExpressionTreeVisualizer), GetType(ExpressionTreeVisualizerObjectSource), Target:=GetType(Expression), Description:="Expression Tree Visualizer")> 

Public Class ExpressionTreeVisualizer
    Inherits DialogDebuggerVisualizer

    Private m_modalService As IDialogVisualizerService

    Protected Overrides Sub Show(ByVal windowService As IDialogVisualizerService, ByVal objectProvider As IVisualizerObjectProvider)
        m_modalService = windowService
        If m_modalService Is Nothing Then
            Throw New ApplicationException("This debugger does not support modal visualizers")
        End If

        Dim container As ExprTreeContainer = objectProvider.GetObject()
        Dim treeForm = New ExpressionTreeForm(container, container.Expression)
        m_modalService.ShowDialog(treeForm)
    End Sub

End Class
