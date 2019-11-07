'****************************** Module Header ******************************\
' Module Name:  ViewModelLocator.vb
' Project:      VBWindowsStoreAppFlightDataFilter
' Copyright (c) Microsoft Corporation.
'
' ViewModelLocator. 
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Namespace ViewModel
    Public Class ViewModelLocator
        ''' <summary>
        ''' Initializes a new instance of the ViewModelLocator class.
        ''' </summary>
        Shared Sub New()
            _main = New MainViewModel()
        End Sub

        Public Shared _main As MainViewModel
        Public ReadOnly Property Main() As MainViewModel
            Get
                Return _main
            End Get
        End Property

        Public Shared Sub Cleanup()
            ' TODO Clear the ViewModels
        End Sub
    End Class

End Namespace