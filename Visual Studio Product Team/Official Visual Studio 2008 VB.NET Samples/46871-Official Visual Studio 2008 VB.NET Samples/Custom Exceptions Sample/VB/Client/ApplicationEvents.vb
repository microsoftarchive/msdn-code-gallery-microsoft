Imports TailspinToysCRM

Namespace My

    ' The following events are availble for MyApplication:
    ' 
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication

        Private Sub MyApplication_UnhandledException(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs) Handles Me.UnhandledException
            If My.Forms.MainForm.ShouldTrapUnhandledException Then
                ' At this point you should be able to figure out what to do.
                ' You could log the error, show a messagebox, send an e-mail.
                ' The real key is to evaluate the exception you received
                ' using code like below.

                ' Note you should be careful. If you have an untrapped exception here,
                ' you will see the JIT debugging dialog, NOT the Windows Forms handler.
                ' Uncomment the line below to see.
                ' Throw New ArgumentException("Oopss! I did it again!")

                Dim exp As Exception = e.Exception

                If TypeOf exp Is CustomerException Then
                    ' Check for any CustomerExceptions
                ElseIf TypeOf exp Is ApplicationException Then
                    ' Check for any possible application exception
                ElseIf TypeOf exp Is ArithmeticException Then
                    ' You should see this dialog if you ran the
                    ' code in cmdUntrapped_Click.
                    ' Uncomment this line to verify.
                    ' MsgBox(exp.Message, MsgBoxStyle.OKOnly, exp.Source)
                ElseIf TypeOf exp Is SystemException Then
                    ' Check for any possible system exception
                Else
                    ' Finally just plain old System.Exception
                End If

                Dim message As String
                message = String.Format("An untrapped error occurred.{0}The error message was:{0}{1}", vbCrLf, exp.Message)

                MsgBox(message, MsgBoxStyle.OkOnly, "Global Exception Trap")
                ' Try to let the application resume execution.
                e.ExitApplication = False
            Else
                ' If global exception handling is turned off, re-throw the exception
                Throw e.Exception
            End If
            


        End Sub
    End Class

End Namespace

