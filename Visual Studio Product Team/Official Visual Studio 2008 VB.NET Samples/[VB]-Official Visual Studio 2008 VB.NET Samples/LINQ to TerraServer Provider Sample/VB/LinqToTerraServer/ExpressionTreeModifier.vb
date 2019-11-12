Imports System.Linq.Expressions

Friend Class ExpressionTreeModifier
    Inherits ExpressionVisitor

    Private queryablePlaces As IQueryable(Of Place)

    Friend Sub New(ByVal places As IQueryable(Of Place))
        Me.queryablePlaces = places
    End Sub

    Friend Function CopyAndModify(ByVal expression As Expression) As Expression
        Return Me.Visit(expression)
    End Function

    Protected Overrides Function VisitConstant(ByVal c As ConstantExpression) As Expression
        ' Replace the constant QueryableTerraServerData arg with the queryable Place collection.
        If c.Type Is GetType(QueryableTerraServerData(Of Place)) Then
            Return Expression.Constant(Me.queryablePlaces)
        Else
            Return c
        End If
    End Function
End Class
