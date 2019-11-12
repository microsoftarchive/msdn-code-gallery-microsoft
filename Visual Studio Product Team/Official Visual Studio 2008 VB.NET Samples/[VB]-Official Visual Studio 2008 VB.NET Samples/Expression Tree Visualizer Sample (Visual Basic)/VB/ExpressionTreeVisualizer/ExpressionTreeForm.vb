Imports System.Windows.Forms

Public Class ExpressionTreeForm

    Sub New(ByVal container As ExprTreeContainer, ByVal expr As String)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.ErrorMessageBox.Text = expr
        Me.TreeBrowser1.Add(container.Tree)
        Me.TreeBrowser1.ExpandAll()
    End Sub

End Class

Public Class TreeBrowser
    Inherits TreeView

    Public Sub Add(ByVal root As TreeNode)
        Nodes.Add(root)
    End Sub

End Class