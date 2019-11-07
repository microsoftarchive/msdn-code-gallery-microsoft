Namespace Common
    ''' <summary>
    '''  
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RelayCommand
        Implements ICommand
        Private ReadOnly _execute As Action
        Private ReadOnly _canExecute As Func(Of Boolean)

        Public Sub New(execute As Action)
            Me.New(execute, Nothing)
        End Sub

        Public Sub New(execute As Action, canExecute As Func(Of Boolean))
            If execute Is Nothing Then
                Throw New ArgumentNullException("execute")
            End If
            _execute = execute
            _canExecute = canExecute
        End Sub
        Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged
        Public Sub RaiseCanExecuteChanged()
            RaiseEvent CanExecuteChanged(Me, EventArgs.Empty)
        End Sub
        Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
            Return If(_canExecute Is Nothing, True, _canExecute())
        End Function
        Public Sub Execute(parameter As Object) Implements ICommand.Execute
            _execute()
        End Sub
    End Class

End Namespace