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
Imports System.IO
Imports Microsoft.VisualStudio.OLE.Interop

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider
	''' <summary>
	''' Implements a managed Stream object on top of a COM IStream.
	''' </summary>
	Public NotInheritable Class DataStreamFromComStream
		Inherits Stream
		Implements IDisposable

		Private comStream As IStream

		''' <summary>
        ''' Build the managed Stream object on top of the IStream COM object.
		''' </summary>
		''' <param name="comStream">The COM IStream object.</param>
		Public Sub New(ByVal comStream As IStream)
			MyBase.New()
			Me.comStream = comStream
		End Sub

		''' <summary>
		''' Gets or sets the position (relative to the stream's begin) inside the stream.
		''' </summary>
		Public Overrides Property Position() As Long
			Get
				Return Seek(0, SeekOrigin.Current)
			End Get

			Set(ByVal value As Long)
				Seek(value, SeekOrigin.Begin)
			End Set
		End Property

		''' <summary>
		''' True if it is possible to write on the stream.
		''' </summary>
		Public Overrides ReadOnly Property CanWrite() As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' True if it is possible to change the current position inside the stream.
		''' </summary>
		Public Overrides ReadOnly Property CanSeek() As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' True if it is possible to read from the stream.
		''' </summary>
		Public Overrides ReadOnly Property CanRead() As Boolean
			Get
				Return True
			End Get
		End Property

		''' <summary>
		''' Gets the length of the stream.
		''' </summary>
		Public Overrides ReadOnly Property Length() As Long
			Get
				Dim curPos As Long = Me.Position
				Dim endPos As Long = Seek(0, SeekOrigin.End)
				Me.Position = curPos
				Return endPos - curPos
			End Get
		End Property

		Private Sub _NotImpl(ByVal message As String)
            Dim ex As New NotSupportedException()
			Throw ex
		End Sub

		''' <summary>
		''' Dispose this object and release the COM stream.
		''' </summary>
		Public Shadows Sub Dispose() Implements IDisposable.Dispose
			Try
                If comStream IsNot Nothing Then
                    Flush()
                    comStream = Nothing
                End If
			Finally
				MyBase.Dispose()
			End Try

		End Sub

		''' <summary>
		''' Flush the pending data to the stream.
		''' </summary>
		Public Overrides Sub Flush()
            If comStream IsNot Nothing Then
                Try
                    comStream.Commit(0)
                Catch e1 As Exception
                End Try
            End If
		End Sub

		''' <summary>
		''' Reads a buffer of data from the stream.
		''' </summary>
		''' <param name="buffer">The buffer to read into.</param>
		''' <param name="index">Starting position inside the buffer.</param>
		''' <param name="count">Number of bytes to read.</param>
		''' <returns>The number of bytes read.</returns>
		Public Overrides Function Read(ByVal buffer As Byte(), ByVal index As Integer, ByVal count As Integer) As Integer
			Dim bytesRead As UInteger
			Dim b As Byte() = buffer

			If index <> 0 Then
				b = New Byte(buffer.Length - index - 1){}
				buffer.CopyTo(b, 0)
			End If

			comStream.Read(b, CUInt(count), bytesRead)

			If index <> 0 Then
				b.CopyTo(buffer, index)
			End If

			Return CInt(Fix(bytesRead))
		End Function

		''' <summary>
		''' Sets the length of the stream.
		''' </summary>
		''' <param name="value">The new lenght.</param>
		Public Overrides Sub SetLength(ByVal value As Long)
            Dim ul As New ULARGE_INTEGER()
			ul.QuadPart = CULng(value)
			comStream.SetSize(ul)
		End Sub

		''' <summary>
		''' Changes the seek pointer to a new location relative to the current seek pointer
		''' or the beginning or end of the stream.
		''' </summary>
		''' <param name="offset">Displacement to be added to the location indicated by origin.</param>
		''' <param name="origin">Specifies the origin for the displacement.</param>
		''' <returns>Pointer to the location where this method writes the value of the new seek pointer from the beginning of the stream.</returns>
		Public Overrides Function Seek(ByVal offset As Long, ByVal origin As SeekOrigin) As Long
            Dim l As New LARGE_INTEGER()
			Dim ul As ULARGE_INTEGER() = New ULARGE_INTEGER(0){}
			ul(0) = New ULARGE_INTEGER()
			l.QuadPart = offset
			comStream.Seek(l, CUInt(origin), ul)
			Return CLng(Fix(ul(0).QuadPart))
		End Function

		''' <summary>
		''' Writes a specified number of bytes into the stream object starting at the current seek pointer.
		''' </summary>
		''' <param name="buffer">The buffer to write into the stream.</param>
		''' <param name="index">Index inside the buffer of the first byte to write.</param>
		''' <param name="count">Number of bytes to write.</param>
		Public Overrides Sub Write(ByVal buffer As Byte(), ByVal index As Integer, ByVal count As Integer)
			Dim bytesWritten As UInteger

			If count > 0 Then

				Dim b As Byte() = buffer

				If index <> 0 Then
					b = New Byte(buffer.Length - index - 1){}
					buffer.CopyTo(b, 0)
				End If

				comStream.Write(b, CUInt(count), bytesWritten)
                If bytesWritten <> count Then
                    ' @TODO: Localize this.
                    Throw New IOException("Didn't write enough bytes to IStream!")
                End If

				If index <> 0 Then
					b.CopyTo(buffer, index)
				End If
			End If
		End Sub

		''' <summary>
		''' Close the stream.
		''' </summary>
		Public Overrides Sub Close()
            If comStream IsNot Nothing Then
                Flush()
                comStream = Nothing
                GC.SuppressFinalize(Me)
            End If
		End Sub

		''' <summary></summary>
		Protected Overrides Sub Finalize()
            ' CANNOT CLOSE NATIVE STREAMS IN FINALIZER THREAD.
			' Close();
		End Sub
	End Class
End Namespace
