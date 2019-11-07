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
Imports System.IO
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.OLE.Interop
Imports System.Runtime.InteropServices

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
	<System.Runtime.InteropServices.ComVisible(False)> _
	Friend Class StreamConsts
		Public Const STREAM_SEEK_SET As Integer = &H0
		Public Const STREAM_SEEK_CUR As Integer = &H1
		Public Const STREAM_SEEK_END As Integer = &H2
	End Class

	''' <summary>
	'''     This class maps an Stream to an IStream. Instances of this class are
	'''     created by the <code>Stream.getComStream()</code> and
	''' <code>Stream.toComStream()</code> methods as needed. You would typically
	'''     never create instances of this class yourself.
	'''     The implementation of IStream provided by this class is not complete. In
	'''     particular, the <code>Clone</code>, <code>CopyTo</code>, and
	''' <code>Stat</code> methods return an E_NOTIMPL error code, and the
	''' <code>LockRegion</code>, <code>UnlockRegion</code>, and <code>Revert</code>
	'''     methods are simply empty implementations.
	''' </summary>

	<System.Runtime.InteropServices.ComVisible(False)> _
	Friend Class ComStreamFromDataStream
		Implements IStream
		Protected dataStream As Stream

        ' To support seeking ahead of the stream length...
		Private virtualPosition As Long = -1

		Public Sub New(ByVal dataStream As Stream)
			If dataStream Is Nothing Then
			Throw New ArgumentNullException()
			End If
			Me.dataStream = dataStream
		End Sub

        ' Don't forget to set dataStream before using this object.
		Protected Sub New()
		End Sub

		Private Sub ActualizeVirtualPosition()
			If virtualPosition = -1 Then
			Return
			End If

			If virtualPosition > dataStream.Length Then
				dataStream.SetLength(virtualPosition)
			End If

			dataStream.Position = virtualPosition

			virtualPosition = -1
		End Sub

		Public Overridable Sub Clone(<System.Runtime.InteropServices.Out()> ByRef ppstm As IStream) Implements IStream.Clone
			NotImplemented()
			ppstm = Nothing
		End Sub

		Public Overridable Sub Commit(ByVal grfCommitFlags As UInteger) Implements IStream.Commit
			dataStream.Flush()
			' Extend the length of the file if needed.
			ActualizeVirtualPosition()
		End Sub

		Public Overridable Sub CopyTo(ByVal pstm As IStream, ByVal cb As ULARGE_INTEGER, ByVal pcbRead As ULARGE_INTEGER(), ByVal pcbWritten As ULARGE_INTEGER()) Implements IStream.CopyTo
            Dim pcblRead As Long() = New Long(0) {}
			pcbWritten(0).QuadPart = CULng(CopyTo(pstm, CLng(Fix(cb.QuadPart)), pcblRead))
			pcbRead(0).QuadPart = CULng(pcblRead(0))
		End Sub

        Public Overridable Function CopyTo(ByVal pstm As IStream, ByVal cb As Long, ByVal pcbRead As Long()) As Long
            ' One page
            Dim bufsize As Integer = 4096
            Dim buffer As IntPtr = Marshal.AllocHGlobal(New IntPtr(bufsize))
            If buffer = CType(0, IntPtr) Then
                Throw New OutOfMemoryException()
            End If
            Dim byteBuffer As Byte() = New Byte(bufsize - 1) {}
            Dim written As Long = 0
            Try
                Do While written < cb
                    Dim toRead As Integer = bufsize
                    If written + toRead > cb Then
                        toRead = CInt(Fix(cb - written))
                    End If
                    Dim read As Integer = Me.Read(buffer, toRead)
                    If read = 0 Then
                        Exit Do
                    End If
                    Marshal.PtrToStructure(buffer, byteBuffer)
                    Dim pcbWritten As UInteger = 0
                    pstm.Write(byteBuffer, CUInt(read), pcbWritten)
                    If pcbWritten <> read Then
                        Throw EFail("Wrote an incorrect number of bytes")
                    End If
                    written += read
                Loop
            Finally
                Marshal.FreeHGlobal(buffer)
            End Try
            If pcbRead IsNot Nothing AndAlso pcbRead.Length > 0 Then
                pcbRead(0) = written
            End If

            Return written
        End Function

		Public Overridable Function GetDataStream() As Stream
			Return dataStream
		End Function

		Public Overridable Sub LockRegion(ByVal libOffset As ULARGE_INTEGER, ByVal cb As ULARGE_INTEGER, ByVal dwLockType As UInteger) Implements IStream.LockRegion
		End Sub

		Protected Shared Function EFail(ByVal msg As String) As ExternalException
            Dim e As New ExternalException(msg, VSConstants.E_FAIL)
			Throw e
		End Function

		Protected Shared Sub NotImplemented()
            Dim e As New ExternalException("Not implemented.", VSConstants.E_NOTIMPL)
			Throw e
		End Sub

        Public Overridable Sub Read(ByVal pv As Byte(), ByVal cb As UInteger, ByRef pcbRead As UInteger) Implements IStream.Read, ISequentialStream.Read
            pcbRead = CUInt(Read(pv, CInt(Fix(cb))))
        End Sub

        Public Overridable Function Read(ByVal buf As IntPtr, ByVal length As Integer) As Integer
            Dim buffer As Byte() = New Byte(length - 1) {}
            Dim count As Integer = Read(buffer, length)
            Marshal.Copy(buffer, 0, buf, length)
            Return count
        End Function
        Public Overridable Function Read(ByVal buffer As Byte(), ByVal length As Integer) As Integer
            ActualizeVirtualPosition()
            Return dataStream.Read(buffer, 0, length)
        End Function

		Public Overridable Sub Revert() Implements IStream.Revert
			NotImplemented()
		End Sub

		Public Overridable Sub Seek(ByVal offset As LARGE_INTEGER, ByVal origin As UInteger, ByVal newPosition As ULARGE_INTEGER()) Implements IStream.Seek
			newPosition(0).QuadPart = CULng(Seek(CLng(Fix(offset.QuadPart)), CInt(Fix(origin))))
		End Sub

		Public Overridable Function Seek(ByVal offset As Long, ByVal origin As Integer) As Long
            ' MsgBox("IStream::Seek(" & offset & ", " & origin & ")").
			Dim pos As Long = virtualPosition
			If virtualPosition = -1 Then
				pos = dataStream.Position
			End If
			Dim len As Long = dataStream.Length
			Select Case origin
				Case StreamConsts.STREAM_SEEK_SET
					If offset <= len Then
						dataStream.Position = offset
						virtualPosition = -1
					Else
						virtualPosition = offset
					End If
				Case StreamConsts.STREAM_SEEK_END
					If offset <= 0 Then
						dataStream.Position = len + offset
						virtualPosition = -1
					Else
						virtualPosition = len + offset
					End If
				Case StreamConsts.STREAM_SEEK_CUR
					If offset + pos <= len Then
						dataStream.Position = pos + offset
						virtualPosition = -1
					Else
						virtualPosition = offset + pos
					End If
			End Select
			If virtualPosition <> -1 Then
				Return virtualPosition
			Else
				Return dataStream.Position
			End If
		End Function

		Public Overridable Sub SetSize(ByVal value As ULARGE_INTEGER) Implements IStream.SetSize
			dataStream.SetLength(CLng(Fix(value.QuadPart)))
		End Sub

		Public Overridable Sub Stat(ByVal pstatstg As Microsoft.VisualStudio.OLE.Interop.STATSTG(), ByVal grfStatFlag As UInteger) Implements IStream.Stat
			NotImplemented()
		End Sub

		Public Overridable Sub UnlockRegion(ByVal libOffset As ULARGE_INTEGER, ByVal cb As ULARGE_INTEGER, ByVal dwLockType As UInteger) Implements IStream.UnlockRegion
		End Sub

        Public Overridable Sub Write(ByVal pv As Byte(), ByVal cb As UInteger, ByRef pcbWritten As UInteger) Implements IStream.Write, ISequentialStream.Write
            pcbWritten = CUInt(Write(pv, CInt(Fix(cb))))
        End Sub

        Public Overridable Function Write(ByVal buf As IntPtr, ByVal length As Integer) As Integer
            Dim buffer As Byte() = New Byte(length - 1) {}
            Marshal.Copy(buf, buffer, 0, length)
            Return Write(buffer, length)
        End Function
        Public Overridable Function Write(ByVal buffer As Byte(), ByVal length As Integer) As Integer
            ActualizeVirtualPosition()
            dataStream.Write(buffer, 0, length)
            Return length
        End Function
	End Class
End Namespace


