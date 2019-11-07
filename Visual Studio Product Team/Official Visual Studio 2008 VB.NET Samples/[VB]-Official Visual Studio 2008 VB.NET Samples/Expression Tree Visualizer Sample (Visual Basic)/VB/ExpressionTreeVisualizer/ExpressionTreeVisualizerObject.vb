Imports Microsoft.VisualStudio.DebuggerVisualizers
Imports System.IO
Imports System.Linq.Expressions
Imports System.Runtime.Serialization

Public Class ExpressionTreeVisualizerObjectSource
    Inherits VisualizerObjectSource

    Public Overrides Sub GetData(ByVal target As Object, ByVal outgoingData As Stream)
        Dim expr As Expression = target
        Dim browser = New ExpressionTreeNode(expr)
        Dim container = New ExprTreeContainer(browser, expr.ToString())

        VisualizerObjectSource.Serialize(outgoingData, container)
    End Sub

End Class

<Serializable()> _
Public Class ExprTreeContainer

    Public Sub New(ByVal tree As ExpressionTreeNode, ByVal expression As String)
        m_tree = Tree
        m_expression = Expression
    End Sub

    Private m_tree As ExpressionTreeNode
    Public Property Tree() As ExpressionTreeNode
        Get
            Return m_tree
        End Get
        Set(ByVal value As ExpressionTreeNode)
            m_tree = value
        End Set
    End Property

    Private m_expression As String
    Public Property Expression() As String
        Get
            Return m_expression
        End Get
        Set(ByVal value As String)
            m_expression = value
        End Set
    End Property

End Class
