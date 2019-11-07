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
Imports System.Diagnostics
Imports Microsoft.VisualStudio.Shell.Interop

Namespace Microsoft.Samples.VisualStudio.Services
	''' <summary>
	''' This class is used to expose some utility functions used in this project.
	''' </summary>
	Friend Class HelperFunctions
		''' <summary>
		''' This function is used to write a string on the Output window of Visual Studio.
		''' </summary>
		''' <param name="provider">The service provider to query for SVsOutputWindow</param>
		''' <param name="text">The text to write</param>
		Private Sub New()
		End Sub
		Friend Shared Sub WriteOnOutputWindow(ByVal provider As IServiceProvider, ByVal text As String)
			' At first write the text on the debug output.
            Debug.WriteLine(text)

            ' Check if we have a provider.
            If provider Is Nothing Then
                ' If there is no provider we can not do anything; exit now.
                Debug.WriteLine("No service provider passed to WriteOnOutputWindow.")
                Return
            End If

            ' Now get the SVsOutputWindow service from the service provider.
            Dim outputWindow As IVsOutputWindow = TryCast(provider.GetService(GetType(SVsOutputWindow)), IVsOutputWindow)
            If outputWindow Is Nothing Then
                ' If the provider doesn't expose the service there is nothing we can do.
                ' Write a message on the debug output and exit.
                Debug.WriteLine("Can not get the SVsOutputWindow service.")
                Return
            End If

            ' We can not write on the Output window itself, but only on one of its panes.
            ' Here we try to use the "General" pane.
            Dim guidGeneral As Guid = Microsoft.VisualStudio.VSConstants.GUID_OutWindowGeneralPane
            Dim windowPane As IVsOutputWindowPane = Nothing
            If Microsoft.VisualStudio.ErrorHandler.Failed(outputWindow.GetPane(guidGeneral, windowPane)) OrElse (Nothing Is windowPane) Then
                Microsoft.VisualStudio.ErrorHandler.Failed(outputWindow.CreatePane(guidGeneral, "General", 1, 0))
                If Microsoft.VisualStudio.ErrorHandler.Failed(outputWindow.GetPane(guidGeneral, windowPane)) OrElse (Nothing Is windowPane) Then
                    ' Again, there is nothing we can do to recover from this error, so write on the
                    ' debug output and exit.
                    Debug.WriteLine("Failed to get the Output window pane.")
                    Return
                End If
                Microsoft.VisualStudio.ErrorHandler.Failed(windowPane.Activate())
            End If

            ' Finally we can write on the window pane.
            If Microsoft.VisualStudio.ErrorHandler.Failed(windowPane.OutputString(text)) Then
                Debug.WriteLine("Failed to write on the Output window pane.")
            End If
		End Sub
	End Class
End Namespace
