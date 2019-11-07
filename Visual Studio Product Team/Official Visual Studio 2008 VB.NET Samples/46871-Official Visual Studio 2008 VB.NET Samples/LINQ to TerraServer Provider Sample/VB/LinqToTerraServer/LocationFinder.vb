Imports System.Linq.Expressions
Imports ETH = LinqToTerraServer.ExpressionTreeHelpers

Friend Class LocationFinder
    Inherits ExpressionVisitor

    Private _expression As Expression
    Private _locations As List(Of String)

    Public Sub New(ByVal exp As Expression)
        Me._expression = exp
    End Sub

    Public ReadOnly Property Locations() As List(Of String)
        Get
            If _locations Is Nothing Then
                _locations = New List(Of String)()
                Me.Visit(Me._expression)
            End If
            Return Me._locations
        End Get
    End Property

    Protected Overrides Function VisitBinary(ByVal be As BinaryExpression) As Expression
        ' Handles Visual Basic String semantics.
        be = ETH.ConvertVBStringCompare(be)

        If be.NodeType = ExpressionType.Equal Then
            If (ETH.IsMemberEqualsValueExpression(be, GetType(Place), "Name")) Then
                _locations.Add(ETH.GetValueFromEqualsExpression(be, GetType(Place), "Name"))
                Return be
            ElseIf (ETH.IsMemberEqualsValueExpression(be, GetType(Place), "State")) Then
                _locations.Add(ETH.GetValueFromEqualsExpression(be, GetType(Place), "State"))
                Return be
            Else
                Return MyBase.VisitBinary(be)
            End If
        Else
            Return MyBase.VisitBinary(be)
        End If
    End Function

    Protected Overrides Function VisitMethodCall(ByVal m As MethodCallExpression) As Expression
        If m.Method.DeclaringType Is GetType(String) And m.Method.Name = "StartsWith" Then
            If ETH.IsSpecificMemberExpression(m.Object, GetType(Place), "Name") Or _
               ETH.IsSpecificMemberExpression(m.Object, GetType(Place), "State") Then
                _locations.Add(ETH.GetValueFromExpression(m.Arguments(0)))
                Return m
            End If
        ElseIf m.Method.Name = "Contains" Then
            Dim valuesExpression As Expression = Nothing

            If m.Method.DeclaringType Is GetType(Enumerable) Then
                If ETH.IsSpecificMemberExpression(m.Arguments(1), GetType(Place), "Name") Or _
                   ETH.IsSpecificMemberExpression(m.Arguments(1), GetType(Place), "State") Then
                    valuesExpression = m.Arguments(0)
                End If

            ElseIf m.Method.DeclaringType Is GetType(List(Of String)) Then
                If ETH.IsSpecificMemberExpression(m.Arguments(0), GetType(Place), "Name") Or _
                   ETH.IsSpecificMemberExpression(m.Arguments(0), GetType(Place), "State") Then
                    valuesExpression = m.Object
                End If
            End If

            If valuesExpression Is Nothing OrElse valuesExpression.NodeType <> ExpressionType.Constant Then
                Throw New Exception("Could not find the location values.")
            End If

            Dim ce = CType(valuesExpression, ConstantExpression)

            Dim placeStrings = CType(ce.Value, IEnumerable(Of String))
            ' Add each string in the collection to the list of locations to obtain data about.
            For Each place In placeStrings
                _locations.Add(place)
            Next

            Return m
        End If

        Return MyBase.VisitMethodCall(m)
    End Function
End Class






