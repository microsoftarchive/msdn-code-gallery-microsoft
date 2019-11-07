Imports System.Linq.Expressions

Class InnermostWhereFinder
    Inherits ExpressionVisitor

    Private innermostWhereExpression As MethodCallExpression

    Public Function GetInnermostWhere(ByVal expr As Expression) As MethodCallExpression
        Me.Visit(expr)
        Return innermostWhereExpression
    End Function

    Protected Overrides Function VisitMethodCall(ByVal expr As MethodCallExpression) As Expression
        If expr.Method.Name = "Where" Then
            innermostWhereExpression = expr
        End If

        Me.Visit(expr.Arguments(0))

        Return expr
    End Function
End Class
