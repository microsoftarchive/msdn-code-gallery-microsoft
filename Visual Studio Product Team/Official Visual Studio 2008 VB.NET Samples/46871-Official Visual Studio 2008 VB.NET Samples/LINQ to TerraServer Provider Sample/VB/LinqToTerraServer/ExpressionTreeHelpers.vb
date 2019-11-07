Imports System.Linq.Expressions

Friend Class ExpressionTreeHelpers
    ' Visual Basic encodes string comparisons as a method call to
    ' Microsoft.VisualBasic.CompilerServices.Operators.CompareString.
    ' This method will convert the method call into a binary operation instead.
    ' Note that this makes the string comparison case sensitive.
    Friend Shared Function ConvertVBStringCompare(ByVal exp As BinaryExpression) As BinaryExpression

        If exp.Left.NodeType = ExpressionType.Call Then
            Dim compareStringCall = CType(exp.Left, MethodCallExpression)

            If compareStringCall.Method.DeclaringType.FullName = _
                "Microsoft.VisualBasic.CompilerServices.Operators" AndAlso _
                compareStringCall.Method.Name = "CompareString" Then

                Dim arg1 = compareStringCall.Arguments(0)
                Dim arg2 = compareStringCall.Arguments(1)

                Select Case exp.NodeType
                    Case ExpressionType.LessThan
                        Return Expression.LessThan(arg1, arg2)
                    Case ExpressionType.LessThanOrEqual
                        Return Expression.GreaterThan(arg1, arg2)
                    Case ExpressionType.GreaterThan
                        Return Expression.GreaterThan(arg1, arg2)
                    Case ExpressionType.GreaterThanOrEqual
                        Return Expression.GreaterThanOrEqual(arg1, arg2)
                    Case Else
                        Return Expression.Equal(arg1, arg2)
                End Select
            End If
        End If
        Return exp
    End Function

    Friend Shared Function IsMemberEqualsValueExpression(ByVal exp As Expression, _
                                                         ByVal declaringType As Type, _
                                                         ByVal memberName As String) As Boolean

        If exp.NodeType <> ExpressionType.Equal Then
            Return False
        End If

        Dim be = CType(exp, BinaryExpression)

        ' Assert.
        If IsSpecificMemberExpression(be.Left, declaringType, memberName) AndAlso _
           IsSpecificMemberExpression(be.Right, declaringType, memberName) Then

            Throw New Exception("Cannot have 'member' = 'member' in an expression!")
        End If

        Return IsSpecificMemberExpression(be.Left, declaringType, memberName) OrElse _
               IsSpecificMemberExpression(be.Right, declaringType, memberName)
    End Function


    Friend Shared Function IsSpecificMemberExpression(ByVal exp As Expression, _
                                                      ByVal declaringType As Type, _
                                                      ByVal memberName As String) As Boolean

        Return (TypeOf exp Is MemberExpression) AndAlso _
               (CType(exp, MemberExpression).Member.DeclaringType Is declaringType) AndAlso _
               (CType(exp, MemberExpression).Member.Name = memberName)
    End Function


    Friend Shared Function GetValueFromEqualsExpression(ByVal be As BinaryExpression, _
                                                        ByVal memberDeclaringType As Type, _
                                                        ByVal memberName As String) As String

        If be.NodeType <> ExpressionType.Equal Then
            Throw New Exception("There is a bug in this program.")
        End If

        If be.Left.NodeType = ExpressionType.MemberAccess Then
            Dim mEx = CType(be.Left, MemberExpression)

            If mEx.Member.DeclaringType Is memberDeclaringType AndAlso _
               mEx.Member.Name = memberName Then
                Return GetValueFromExpression(be.Right)
            End If
        ElseIf be.Right.NodeType = ExpressionType.MemberAccess Then
            Dim mEx = CType(be.Right, MemberExpression)

            If mEx.Member.DeclaringType Is memberDeclaringType AndAlso _
               mEx.Member.Name = memberName Then
                Return GetValueFromExpression(be.Left)
            End If
        End If

        ' Should have returned by now.
        Throw New Exception("There is a bug in this program.")
    End Function

    Friend Shared Function GetValueFromExpression(ByVal expr As expression) As String
        If expr.NodeType = ExpressionType.Constant Then
            Return CStr(CType(expr, ConstantExpression).Value)
        Else
            Dim s = "The expression type {0} is not supported to obtain a value."
            Throw New InvalidQueryException(String.Format(s, expr.NodeType))
        End If
    End Function
End Class
