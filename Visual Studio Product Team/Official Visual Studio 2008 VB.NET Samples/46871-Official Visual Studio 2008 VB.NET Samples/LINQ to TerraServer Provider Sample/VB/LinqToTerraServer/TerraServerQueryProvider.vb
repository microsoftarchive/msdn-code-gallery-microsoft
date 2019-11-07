Imports System.Linq.Expressions
Imports System.Reflection

Public Class TerraServerQueryProvider
    Implements IQueryProvider

    Public Function CreateQuery(ByVal expression As Expression) As IQueryable _
        Implements IQueryProvider.CreateQuery

        Dim elementType As Type = TypeSystem.GetElementType(expression.Type)

        Try
            Dim qType = GetType(QueryableTerraServerData(Of )).MakeGenericType(elementType)
            Dim args = New Object() {Me, expression}
            Dim instance = Activator.CreateInstance(qType, args)

            Return CType(instance, IQueryable)
        Catch tie As TargetInvocationException
            Throw tie.InnerException
        End Try
    End Function

    ' Queryable's collection-returning standard query operators call this method.
    Public Function CreateQuery(Of TResult)(ByVal expression As Expression) As IQueryable(Of TResult) _
        Implements IQueryProvider.CreateQuery

        Return New QueryableTerraServerData(Of TResult)(Me, expression)
    End Function

    Public Function Execute(ByVal expression As Expression) As Object _
        Implements IQueryProvider.Execute

        Return TerraServerQueryContext.Execute(expression, False)
    End Function

    ' Queryable's "single value" standard query operators call this method.
    ' It is also called from QueryableTerraServerData.GetEnumerator().
    Public Function Execute(Of TResult)(ByVal expression As Expression) As TResult _
        Implements IQueryProvider.Execute

        Dim IsEnumerable As Boolean = (GetType(TResult).Name = "IEnumerable`1")

        Dim result = TerraServerQueryContext.Execute(expression, IsEnumerable)
        Return CType(result, TResult)
    End Function
End Class
