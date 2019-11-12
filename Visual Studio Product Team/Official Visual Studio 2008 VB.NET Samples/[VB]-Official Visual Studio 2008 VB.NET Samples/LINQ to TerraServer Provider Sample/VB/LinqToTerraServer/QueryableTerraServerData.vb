Imports System.Linq.Expressions

Public Class QueryableTerraServerData(Of TData)
    Implements IOrderedQueryable(Of TData)

#Region "Private members"

    Private _provider As TerraServerQueryProvider
    Private _expression As Expression

#End Region

#Region "Constructors"

    ''' <summary>
    ''' This constructor is called by the client to create the data source.
    ''' </summary>
    Public Sub New()
        Me._provider = New TerraServerQueryProvider()
        Me._expression = Expression.Constant(Me)
    End Sub

    ''' <summary>
    ''' This constructor is called by Provider.CreateQuery().
    ''' </summary>
    ''' <param name="_expression"></param>
    Public Sub New(ByVal _provider As TerraServerQueryProvider, ByVal _expression As Expression)

        If _provider Is Nothing Then
            Throw New ArgumentNullException("provider")
        End If

        If _expression Is Nothing Then
            Throw New ArgumentNullException("expression")
        End If

        If Not GetType(IQueryable(Of TData)).IsAssignableFrom(_expression.Type) Then
            Throw New ArgumentOutOfRangeException("expression")
        End If

        Me._provider = _provider
        Me._expression = _expression
    End Sub

#End Region

#Region "Properties"

    Public ReadOnly Property ElementType() As Type _
        Implements IQueryable(Of TData).ElementType
        Get
            Return GetType(TData)
        End Get
    End Property

    Public ReadOnly Property Expression() As Expression _
        Implements IQueryable(Of TData).Expression
        Get
            Return _expression
        End Get
    End Property

    Public ReadOnly Property Provider() As IQueryProvider _
        Implements IQueryable(Of TData).Provider
        Get
            Return _provider
        End Get
    End Property

#End Region

#Region "Enumerators"

    Public Function GetGenericEnumerator() As IEnumerator(Of TData) _
        Implements IEnumerable(Of TData).GetEnumerator

        Return (Me.Provider.Execute(Of IEnumerable(Of TData))(Me._expression)).GetEnumerator()
    End Function

    Public Function GetEnumerator() As IEnumerator _
        Implements IEnumerable.GetEnumerator

        Return (Me.Provider.Execute(Of IEnumerable)(Me._expression)).GetEnumerator()
    End Function

#End Region

End Class
