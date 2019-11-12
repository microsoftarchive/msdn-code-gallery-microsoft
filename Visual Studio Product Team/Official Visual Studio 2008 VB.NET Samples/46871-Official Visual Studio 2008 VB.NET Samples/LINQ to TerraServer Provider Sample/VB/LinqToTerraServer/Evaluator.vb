Imports System.Linq.Expressions

Public Module Evaluator
    ''' <summary>Performs evaluation and replacement of independent sub-trees</summary>
    ''' <param name="expr">The root of the expression tree.</param>
    ''' <param name="fnCanBeEvaluated">A function that decides whether a given expression node can be part of the local function.</param>
    ''' <returns>A new tree with sub-trees evaluated and replaced.</returns>
    Public Function PartialEval(ByVal expr As Expression, _
                                ByVal fnCanBeEvaluated As Func(Of Expression, Boolean)) _
                                      As Expression

        Return New SubtreeEvaluator(New Nominator(fnCanBeEvaluated).Nominate(expr)).Eval(expr)
    End Function

    ''' <summary>
    ''' Performs evaluation and replacement of independent sub-trees
    ''' </summary>
    ''' <param name="expression">The root of the expression tree.</param>
    ''' <returns>A new tree with sub-trees evaluated and replaced.</returns>
    Public Function PartialEval(ByVal expression As Expression) As Expression
        Return PartialEval(expression, AddressOf Evaluator.CanBeEvaluatedLocally)
    End Function

    Private Function CanBeEvaluatedLocally(ByVal expression As Expression) As Boolean
        Return expression.NodeType <> ExpressionType.Parameter
    End Function

    ''' <summary>
    ''' Evaluates and replaces sub-trees when first candidate is reached (top-down)
    ''' </summary>
    Class SubtreeEvaluator
        Inherits ExpressionVisitor

        Private candidates As HashSet(Of Expression)

        Friend Sub New(ByVal candidates As HashSet(Of Expression))
            Me.candidates = candidates
        End Sub

        Friend Function Eval(ByVal exp As Expression) As Expression
            Return Me.Visit(exp)
        End Function

        Protected Overrides Function Visit(ByVal exp As Expression) As Expression
            If exp Is Nothing Then
                Return Nothing
            ElseIf Me.candidates.Contains(exp) Then
                Return Me.Evaluate(exp)
            End If

            Return MyBase.Visit(exp)
        End Function

        Private Function Evaluate(ByVal e As Expression) As Expression
            If e.NodeType = ExpressionType.Constant Then
                Return e
            End If

            Dim lambda = Expression.Lambda(e)
            Dim fn As [Delegate] = lambda.Compile()

            Return Expression.Constant(fn.DynamicInvoke(Nothing), e.Type)
        End Function
    End Class


    ''' <summary>
    ''' Performs bottom-up analysis to determine which nodes can possibly
    ''' be part of an evaluated sub-tree.
    ''' </summary>
    Class Nominator
        Inherits ExpressionVisitor

        Private fnCanBeEvaluated As Func(Of Expression, Boolean)
        Private candidates As HashSet(Of Expression)
        Private cannotBeEvaluated As Boolean

        Friend Sub New(ByVal fnCanBeEvaluated As Func(Of Expression, Boolean))
            Me.fnCanBeEvaluated = fnCanBeEvaluated
        End Sub

        Friend Function Nominate(ByVal expr As Expression) As HashSet(Of Expression)
            Me.candidates = New HashSet(Of Expression)()
            Me.Visit(expr)

            Return Me.candidates
        End Function

        Protected Overrides Function Visit(ByVal expr As Expression) As Expression
            If expr IsNot Nothing Then

                Dim saveCannotBeEvaluated = Me.cannotBeEvaluated
                Me.cannotBeEvaluated = False

                MyBase.Visit(expr)

                If Not Me.cannotBeEvaluated Then
                    If Me.fnCanBeEvaluated(expr) Then
                        Me.candidates.Add(expr)
                    Else
                        Me.cannotBeEvaluated = True
                    End If
                End If

                Me.cannotBeEvaluated = Me.cannotBeEvaluated Or _
                                       saveCannotBeEvaluated
            End If

            Return expr
        End Function
    End Class
End Module
