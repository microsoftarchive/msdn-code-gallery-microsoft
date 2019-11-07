'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************


Imports Microsoft.VisualBasic
	Imports System
	Imports System.Collections.Generic
	Imports System.Text
	Imports System.Runtime.InteropServices
	Imports System.Threading
	Imports Microsoft.VisualStudio.Shell.Interop
	Imports Microsoft.VisualStudio.Shell
Namespace Microsoft.Samples.VisualStudio.ComboBox.IntegrationTest

	''' <summary>
    ''' This class is responsible to close dialog boxes that pop up during different VS Calls.
	''' </summary>
	Friend Class DialogBoxPurger
		Implements IDisposable
		''' <summary>
		''' The default number of milliseconds to wait for the threads to signal to terminate.
		''' </summary>
		Private Const DefaultMillisecondsToWait As Integer = 3500

		''' <summary>
		''' Object used for synchronization between thread calls.
		''' </summary>
        Friend Shared Mutex As New Object()

		''' <summary>
		''' The IVsUIShell. This cannot be queried on the working thread from the service provider. Must be done in the main thread.!!
		''' </summary>
		Private uiShell As IVsUIShell

		''' <summary>
		''' The button to "press" on the dialog.
		''' </summary>
		Private buttonAction As Integer

		''' <summary>
		''' Thread signales to the calling thread that it is done.
		''' </summary>
		Private exitThread As Boolean = False

		''' <summary>
		''' Calling thread signales to this thread to die.
		''' </summary>
        Private threadDone As New AutoResetEvent(False)

		''' <summary>
		''' The queued thread started.
		''' </summary>
        Private threadStarted As New AutoResetEvent(False)

		''' <summary>
		''' The result of the dialogbox closing for all the dialog boxes. That is if there are two of them and one fails this will be false.
		''' </summary>
		Private dialogBoxCloseResult As Boolean = False

		''' <summary>
		''' The expected text to see on the dialog box. If set the thread will continue finding the dialog box with this text.
		''' </summary>
		Private expectedDialogBoxText As String = String.Empty

		''' <summary>
		''' The number of the same  dialog boxes to wait for.
		''' This is for scenarios when two dialog boxes with the same text are popping up.
		''' </summary>
		Private numberOfDialogsToWaitFor As Integer = 1

		''' <summary>
		''' Has the object been disposed.
		''' </summary>
		Private isDisposed As Boolean

		''' <summary>
		''' Overloaded ctor.
		''' </summary>
		''' <param name="buttonAction">The botton to "press" on the dialog box.</param>
		''' <param name="numberOfDialogsToWaitFor">The number of dialog boxes with the same message to wait for. This is the situation when the same action pops up two of the same dialog boxes</param>
		''' <param name="expectedDialogMesssage">The expected dialog box message to check for.</param>
		Friend Sub New(ByVal buttonAction As Integer, ByVal numberOfDialogsToWaitFor As Integer, ByVal expectedDialogMesssage As String)
			Me.buttonAction = buttonAction
			Me.numberOfDialogsToWaitFor = numberOfDialogsToWaitFor
			Me.expectedDialogBoxText = expectedDialogMesssage
		End Sub

		''' <summary>
		''' Overloaded ctor.
		''' </summary>
		''' <param name="buttonAction">The botton to "press" on the dialog box.</param>
		''' <param name="numberOfDialogsToWaitFor">The number of dialog boxes with the same message to wait for. This is the situation when the same action pops up two of the same dialog boxes</param>
		Friend Sub New(ByVal buttonAction As Integer, ByVal numberOfDialogsToWaitFor As Integer)
			Me.buttonAction = buttonAction
			Me.numberOfDialogsToWaitFor = numberOfDialogsToWaitFor
		End Sub

		''' <summary>
		''' Overloaded ctor.
		''' </summary>
		''' <param name="buttonAction">The botton to "press" on the dialog box.</param>
		''' <param name="expectedDialogMesssage">The expected dialog box message to check for.</param>
		Friend Sub New(ByVal buttonAction As Integer, ByVal expectedDialogMesssage As String)
			Me.buttonAction = buttonAction
			Me.expectedDialogBoxText = expectedDialogMesssage
		End Sub

		''' <summary>
		''' Overloaded ctor.
		''' </summary>
		''' <param name="buttonAction">The botton to "press" on the dialog box.</param>
		Friend Sub New(ByVal buttonAction As Integer)
			Me.buttonAction = buttonAction
		End Sub

		''' <summary>
		#Region "IDisposable Members"

		Private Sub Dispose() Implements IDisposable.Dispose
			If Me.isDisposed Then
				Return
			End If

			Me.WaitForDialogThreadToTerminate()

			Me.isDisposed = True
		End Sub

		''' <summary>
		''' Spawns a thread that will start listening to dialog boxes.
		''' </summary>
		Friend Sub Start()
			' We ask for the uishell here since we cannot do that on the therad that we will spawn.
			Dim uiShell As IVsUIShell = TryCast(Package.GetGlobalService(GetType(SVsUIShell)), IVsUIShell)

			If uiShell Is Nothing Then
				Throw New InvalidOperationException("Could not get the uiShell from the serviceProvider")
			End If

			Me.uiShell = uiShell

            Dim thread As New System.Threading.Thread(AddressOf Me.HandleDialogBoxes)
			thread.Start()

			' We should never deadlock here, hence do not use the lock. Wait to be sure that the thread started.
			Me.threadStarted.WaitOne(3500, False)
		End Sub

		''' <summary>
		''' Waits for the dialog box close thread to terminate. If the thread does not signal back within millisecondsToWait that it is shutting down,
		''' then it will tell to the thread to do it.
		''' </summary>
		Friend Function WaitForDialogThreadToTerminate() As Boolean
			Return Me.WaitForDialogThreadToTerminate(DefaultMillisecondsToWait)
		End Function

		''' <summary>
		''' Waits for the dialog box close thread to terminate. If the thread does not signal back within millisecondsToWait that it is shutting down,
		''' then it will tell to the thread to do it.
		''' </summary>
		''' <param name="millisecondsToWait">The number milliseconds to wait for until the dialog purger thread is signaled to terminate. This is just for safe precaution that we do not hang. </param>
		''' <returns>The result of the dialog boxes closing</returns>
		Friend Function WaitForDialogThreadToTerminate(ByVal numberOfMillisecondsToWait As Integer) As Boolean
			Dim signaled As Boolean = False

			' We give millisecondsToWait sec to bring up and close the dialog box.
			signaled = Me.threadDone.WaitOne(numberOfMillisecondsToWait, False)

			' Kill the thread since a timeout occured.
			If (Not signaled) Then
				SyncLock Mutex
                    ' Set the exit thread to true. Next time the thread will kill itselfes if it sees.
					Me.exitThread = True
				End SyncLock

				' Wait for the thread to finish. We should never deadlock here.
				Me.threadDone.WaitOne()
			End If

			Return Me.dialogBoxCloseResult
		End Function

		''' <summary>
		''' This is the thread method. 
		''' </summary>
		Private Sub HandleDialogBoxes()
            ' No synchronization numberOfDialogsToWaitFor since it is readonly.
			Dim hwnds As IntPtr() = New IntPtr(Me.numberOfDialogsToWaitFor - 1){}
			Dim dialogBoxCloseResults As Boolean() = New Boolean(Me.numberOfDialogsToWaitFor - 1){}

			Try
                ' Signal that we started.
				SyncLock Mutex
				   Me.threadStarted.Set()
				End SyncLock

				' The loop will be exited either if a message is send by the caller thread or if we found the dialog. If a message box text is specified the loop will not exit until the dialog is found.
				Dim stayInLoop As Boolean = True
				Dim dialogBoxesToWaitFor As Integer = 1

				Do While stayInLoop
					Dim hwndIndex As Integer = dialogBoxesToWaitFor - 1

					' We need to lock since the caller might set context to null.
					SyncLock Mutex
						If Me.exitThread Then
							Exit Do
						End If

						' We protect the shell too from reentrency.
						Me.uiShell.GetDialogOwnerHwnd(hwnds(hwndIndex))

					End SyncLock

					If hwnds(hwndIndex) <> IntPtr.Zero Then
                        Dim windowClassName As New StringBuilder(256)
						NativeMethods.GetClassName(hwnds(hwndIndex), windowClassName, windowClassName.Capacity)

						' The #32770 is the class name of a messagebox dialog.
						If windowClassName.ToString().Contains("#32770") Then
							Dim unmanagedMemoryLocation As IntPtr = IntPtr.Zero
							Dim dialogBoxText As String = String.Empty
							Try
								unmanagedMemoryLocation = Marshal.AllocHGlobal(10 * 1024)
								NativeMethods.EnumChildWindows(hwnds(hwndIndex), New NativeMethods.CallBack(AddressOf FindMessageBoxString), unmanagedMemoryLocation)
								dialogBoxText = Marshal.PtrToStringUni(unmanagedMemoryLocation)
							Finally
								If unmanagedMemoryLocation <> IntPtr.Zero Then
									Marshal.FreeHGlobal(unmanagedMemoryLocation)
								End If
							End Try

							SyncLock Mutex

								' Since this is running on the main thread be sure that we close the dialog.
								Dim dialogCloseResult As Boolean = False
								If Me.buttonAction <> 0 Then
									dialogCloseResult = NativeMethods.EndDialog(hwnds(hwndIndex), Me.buttonAction)
								End If

								' Check if we have found the right dialog box.
								If String.IsNullOrEmpty(Me.expectedDialogBoxText) OrElse ((Not String.IsNullOrEmpty(dialogBoxText)) AndAlso String.Compare(Me.expectedDialogBoxText, dialogBoxText.Trim(), StringComparison.OrdinalIgnoreCase) = 0) Then
									dialogBoxCloseResults(hwndIndex) = dialogCloseResult
                                    If (dialogBoxesToWaitFor >= Me.numberOfDialogsToWaitFor) Then
                                        dialogBoxesToWaitFor = dialogBoxesToWaitFor + 1
                                        stayInLoop = False

                                    End If
								End If
							End SyncLock
						End If
					End If
				Loop
			Finally
				'Let the main thread run a possible close command.
				System.Threading.Thread.Sleep(2000)

				For Each hwnd As IntPtr In hwnds
					' At this point the dialog should be closed, if not attempt to close it.
					If hwnd <> IntPtr.Zero Then
						NativeMethods.SendMessage(hwnd, NativeMethods.WM_CLOSE, 0, New IntPtr(0))
					End If
				Next hwnd

				SyncLock Mutex
					' Be optimistic.
					Me.dialogBoxCloseResult = True

					For i As Integer = 0 To dialogBoxCloseResults.Length - 1
						If (Not dialogBoxCloseResults(i)) Then
							Me.dialogBoxCloseResult = False
							Exit For
						End If
					Next i

					Me.threadDone.Set()
				End SyncLock
			End Try
		End Sub

		''' <summary>
		''' Finds a messagebox string on a messagebox.
		''' </summary>
		''' <param name="hwnd">The windows handle of the dialog</param>
		''' <param name="unmanagedMemoryLocation">A pointer to the memorylocation the string will be written to</param>
		''' <returns>True if found.</returns>
		Private Shared Function FindMessageBoxString(ByVal hwnd As IntPtr, ByVal unmanagedMemoryLocation As IntPtr) As Boolean
            Dim sb As New StringBuilder(512)
			NativeMethods.GetClassName(hwnd, sb, sb.Capacity)

			If sb.ToString().ToLower().Contains("static") Then
                Dim windowText As New StringBuilder(2048)
				NativeMethods.GetWindowText(hwnd, windowText, windowText.Capacity)

				If windowText.Length > 0 Then
					Dim stringAsPtr As IntPtr = IntPtr.Zero
					Try
						stringAsPtr = Marshal.StringToHGlobalAnsi(windowText.ToString())
						Dim stringAsArray As Char() = windowText.ToString().ToCharArray()

						' Since unicode characters are copied check if we are out of the allocated length.
						' If not add the end terminating zero.
						If (2 * stringAsArray.Length) + 1 < 2048 Then
							Marshal.Copy(stringAsArray, 0, unmanagedMemoryLocation, stringAsArray.Length)
							Marshal.WriteInt32(unmanagedMemoryLocation, 2 * stringAsArray.Length, 0)
						End If
					Finally
						If stringAsPtr <> IntPtr.Zero Then
							Marshal.FreeHGlobal(stringAsPtr)
						End If
					End Try
					Return False
				End If
			End If

			Return True
		End Function

		#End Region
	End Class
End Namespace
