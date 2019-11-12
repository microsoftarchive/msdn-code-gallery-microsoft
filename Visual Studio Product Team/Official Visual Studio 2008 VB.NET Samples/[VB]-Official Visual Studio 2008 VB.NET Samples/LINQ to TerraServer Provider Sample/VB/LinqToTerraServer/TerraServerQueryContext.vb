Imports System.Linq.Expressions

Public Class TerraServerQueryContext

    ' Executes the expression tree that is passed to it.
    Friend Shared Function Execute(ByVal expr As Expression, _
                                   ByVal IsEnumerable As Boolean) As Object

        ' The expression must represent a query over the data source.
        If Not IsQueryOverDataSource(expr) Then
            Throw New InvalidProgramException("No query over the data source was specified.")
        End If

        ' Find the call to Where() and get the lambda expression predicate.
        Dim whereFinder As New InnermostWhereFinder()
        Dim whereExpression As MethodCallExpression = _
            whereFinder.GetInnermostWhere(expr)
        Dim lambdaExpr As LambdaExpression
        lambdaExpr = CType(CType(whereExpression.Arguments(1), UnaryExpression).Operand, LambdaExpression)

        ' Send the lambda expression through the partial evaluator.
        lambdaExpr = CType(Evaluator.PartialEval(lambdaExpr), LambdaExpression)

        ' Get the place name(s) to query the Web service with.
        Dim lf As New LocationFinder(lambdaExpr.Body)
        Dim locations As List(Of String) = lf.Locations
        If locations.Count = 0 Then
            Dim s = "You must specify at least one place name in your query."
            Throw New InvalidQueryException(s)
        End If

        ' Call the Web service and get the results.
        Dim places() = WebServiceHelper.GetPlacesFromTerraServer(locations)

        ' Copy the IEnumerable places to an IQueryable.
        Dim queryablePlaces = places.AsQueryable()

        ' Copy the expression tree that was passed in, changing only the first
        ' argument of the innermost MethodCallExpression.
        Dim treeCopier As New ExpressionTreeModifier(queryablePlaces)
        Dim newExpressionTree = treeCopier.CopyAndModify(expr)

        ' This step creates an IQueryable that executes by replacing 
        ' Queryable methods with Enumerable methods.
        If (IsEnumerable) Then
            Return queryablePlaces.Provider.CreateQuery(newExpressionTree)
        Else
            Return queryablePlaces.Provider.Execute(newExpressionTree)
        End If
    End Function

    Private Shared Function IsQueryOverDataSource(ByVal expression As Expression) As Boolean
        ' If expression represents an unqueried IQueryable data source instance,
        ' expression is of type ConstantExpression, not MethodCallExpression.
        Return (TypeOf expression Is MethodCallExpression)
    End Function
End Class
