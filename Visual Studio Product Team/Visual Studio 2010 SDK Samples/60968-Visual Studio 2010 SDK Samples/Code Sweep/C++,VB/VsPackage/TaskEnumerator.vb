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
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    Friend Class TaskEnumerator
        Implements IVsEnumTaskItems
        ''' <summary>
        ''' Creates a new task enumerator.
        ''' </summary>
        ''' <param name="items">The items this enumerator will enumerate.</param>
        ''' <param name="showIgnored">Determines whether tasks for which Ignored==true will be skipped.</param>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>items</c> is null.</exception>
        ''' <remarks>
        ''' This enumerator operates on a copy of the contents of <c>items</c>, so if <c>items</c>
        ''' is changed after this call, it will not affect the results.
        ''' </remarks>
        Public Sub New(ByVal items As IEnumerable(Of Task), ByVal showIgnored As Boolean)
            _showIgnored = showIgnored
            _items = New List(Of Task)(items)
        End Sub

#Region "IVsEnumTaskItems Members"

        ''' <summary>
        ''' Creates a new enumerator with identical content to this one.
        ''' </summary>
        ''' <param name="ppenum">The newly created enumerator.</param>
        ''' <returns>An HRESULT indicating the success of the operation.</returns>
        Public Function Clone(<System.Runtime.InteropServices.Out()> ByRef ppenum As IVsEnumTaskItems) As Integer Implements IVsEnumTaskItems.Clone
            ppenum = New TaskEnumerator(_items, _showIgnored)
            Return VSConstants.S_OK
        End Function

        ''' <summary>
        ''' Enumerates a requested number of items.
        ''' </summary>
        ''' <param name="celt">The number of items to return.</param>
        ''' <param name="rgelt">The array of items that will be returned.</param>
        ''' <param name="pceltFetched">The array whose first element will be set to the actual number of items returned.</param>
        ''' <returns>S_OK if all requested items were returned; S_FALSE if fewer were available; E_INVALIDARG if <c>celt</c> is less than zero, <c>rgelt</c> is null, or <c>pceltFetched</c> is null.</returns>
        ''' <remarks>
        ''' This method returns failure codes instead of throwing exceptions because it is intended to be called from native code.
        ''' If the task provider's IsShowingIgnoredInstances property is false, ignored instances will be skipped.
        ''' </remarks>
        Public Function [Next](ByVal celt As UInteger, ByVal rgelt() As Microsoft.VisualStudio.Shell.Interop.IVsTaskItem, Optional ByVal pceltFetched() As UInteger = Nothing) As Integer Implements IVsEnumTaskItems.Next
            pceltFetched(0) = 0

            Do While pceltFetched(0) < celt AndAlso _next < _items.Count
                If _showIgnored OrElse (Not _items(_next).Ignored) Then
                    rgelt(CInt(pceltFetched(0))) = _items(_next)
                    pceltFetched(0) = CUInt(pceltFetched(0) + 1)
                End If
                _next += 1
            Loop

            If pceltFetched(0) = celt Then
                Return VSConstants.S_OK
            Else
                Return VSConstants.S_FALSE
            End If
        End Function

        ''' <summary>
        ''' Resets the enumerator so that the next call to <c>Next</c> will begin at the first element.
        ''' </summary>
        ''' <returns>S_OK</returns>
        Public Function Reset() As Integer Implements IVsEnumTaskItems.Reset
            _next = 0
            Return VSConstants.S_OK
        End Function

        ''' <summary>
        ''' Skips a specified number of items.
        ''' </summary>
        ''' <param name="celt">The number of items to skip.</param>
        ''' <returns>S_OK if the requested number of items was skipped; S_FALSE if <c>celt</c> is larger than the number of available items; E_INVALIDARG if <c>celt</c> is less than zero.</returns>
        ''' <remarks>
        ''' This method returns failure codes instead of throwing exceptions because it is intended to be called from native code.
        ''' If the task provider's IsShowingIgnoredInstances property is false, ignored instances will not be counted.
        ''' </remarks>
        Public Function Skip(ByVal celt As UInteger) As Integer Implements IVsEnumTaskItems.Skip
            Dim items As IVsTaskItem() = New IVsTaskItem(CInt(celt - 1)) {}
            Dim fetched() As UInteger = {0}

            Return [Next](celt, items, fetched)
        End Function

#End Region

#Region "Private Members"

        Private ReadOnly _items As List(Of Task)
        Private _next As Integer = 0
        Private ReadOnly _showIgnored As Boolean

#End Region
    End Class
End Namespace
