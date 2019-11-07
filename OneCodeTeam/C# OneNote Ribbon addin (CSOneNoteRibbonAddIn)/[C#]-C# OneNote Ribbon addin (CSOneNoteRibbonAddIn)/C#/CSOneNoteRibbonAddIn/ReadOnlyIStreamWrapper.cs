/****************************** Module Header ******************************\
Module Name:  ReadOnlyIStreamWrapper.cs
Project:      CSOneNoteRibbonAddIn
Copyright (c) Microsoft Corporation.

IStream wrapper class

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Imports directives

using System;
using System.Runtime.InteropServices.ComTypes;
using System.IO; 
#endregion

namespace CSOneNoteRibbonAddIn
{
    /// <summary>
    /// IStream wrapper class
    /// </summary>
    internal class ReadOnlyIStreamWrapper : IStream
    {
        // _stream field
        private Stream _stream;

        /// <summary>
        /// CCOMStreamWrapper construct method
        /// </summary>
        /// <param name="streamWrap">stream</param>
        public ReadOnlyIStreamWrapper(Stream streamWrap)
        {
            this._stream = streamWrap;
        }

        // Summary:
        //     Creates a new stream object with its own seek pointer that references the
        //     same bytes as the original stream.
        //
        // Parameters:
        //   ppstm:
        //     When this method returns, contains the new stream object. This parameter
        //     is passed uninitialized.
        public void Clone(out IStream ppstm)
        {
            ppstm = new ReadOnlyIStreamWrapper(this._stream);
        }

        //
        // Summary:
        //     Ensures that any changes made to a stream object that is open in transacted
        //     mode are reflected in the parent storage.
        //
        // Parameters:
        //   grfCommitFlags:
        //     A value that controls how the changes for the stream object are committed.
        public void Commit(int grfCommitFlags)
        {
            this._stream.Flush();
        }

        //
        // Summary:
        //     Copies a specified number of bytes from the current seek pointer in the stream
        //     to the current seek pointer in another stream.
        //
        // Parameters:
        //   pstm:
        //     A reference to the destination stream.
        //
        //   cb:
        //     The number of bytes to copy from the source stream.
        //
        //   pcbRead:
        //     On successful return, contains the actual number of bytes read from the source.
        //
        //   pcbWritten:
        //     On successful return, contains the actual number of bytes written to the
        //     destination.
        public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
        {
            throw new NotSupportedException("ReadOnlyIStreamWrapper does not support CopyTo");
        }

        //
        // Summary:
        //     Restricts access to a specified range of bytes in the stream.
        //
        // Parameters:
        //   libOffset:
        //     The byte offset for the beginning of the range.
        //
        //   cb:
        //     The length of the range, in bytes, to restrict.
        //
        //   dwLockType:
        //     The requested restrictions on accessing the range.
        public void LockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new NotSupportedException("ReadOnlyIStreamWrapper does not support CopyTo");
        }

        //
        // Summary:
        //     Reads a specified number of bytes from the stream object into memory starting
        //     at the current seek pointer.
        //
        // Parameters:
        //   pv:
        //     When this method returns, contains the data read from the stream. This parameter
        //     is passed uninitialized.
        //
        //   cb:
        //     The number of bytes to read from the stream object.
        //
        //   pcbRead:
        //     A pointer to a ULONG variable that receives the actual number of bytes read
        //     from the stream object.
        public void Read(byte[] pv, int cb, IntPtr pcbRead)
        {
            System.Runtime.InteropServices.Marshal.WriteInt64(pcbRead, (long)this._stream.Read(pv, 0, cb));
        }

        //
        // Summary:
        //     Discards all changes that have been made to a transacted stream since the
        //     last System.Runtime.InteropServices.ComTypes.IStream.Commit(System.Int32)
        //     call.
        public void Revert()
        {
            throw new NotSupportedException("Stream does not support CopyTo");
        }

        //
        // Summary:
        //     Changes the seek pointer to a new location relative to the beginning of the
        //     stream, to the end of the stream, or to the current seek pointer.
        //
        // Parameters:
        //   dlibMove:
        //     The displacement to add to dwOrigin.
        //
        //   dwOrigin:
        //     The origin of the seek. The origin can be the beginning of the file, the
        //     current seek pointer, or the end of the file.
        //
        //   plibNewPosition:
        //     On successful return, contains the offset of the seek pointer from the beginning
        //     of the stream.
        public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
        {
            long num = 0L;
            System.Runtime.InteropServices.Marshal.WriteInt64(plibNewPosition, this._stream.Position);
            switch (dwOrigin)
            {
                case 0:
                    num = dlibMove;
                    break;

                case 1:
                    num = this._stream.Position + dlibMove;
                    break;

                case 2:
                    num = this._stream.Length + dlibMove;
                    break;

                default:
                    return;
            }
            if ((num >= 0L) && (num < this._stream.Length))
            {
                this._stream.Position = num;
                System.Runtime.InteropServices.Marshal.WriteInt64(plibNewPosition, this._stream.Position);
            }

        }

        //
        // Summary:
        //     Changes the size of the stream object.
        //
        // Parameters:
        //   libNewSize:
        //     The new size of the stream as a number of bytes.
        public void SetSize(long libNewSize)
        {
           this._stream.SetLength(libNewSize);
        }

        //
        // Summary:
        //     Retrieves the System.Runtime.InteropServices.STATSTG structure for this stream.
        //
        // Parameters:
        //   pstatstg:
        //     When this method returns, contains a STATSTG structure that describes this
        //     stream object. This parameter is passed uninitialized.
        //
        //   grfStatFlag:
        //     Members in the STATSTG structure that this method does not return, thus saving
        //     some memory allocation operations.
        public void Stat(out STATSTG pstatstg, int grfStatFlag)
        {
            pstatstg = new STATSTG();
            pstatstg.cbSize = this._stream.Length;
            if ((grfStatFlag & 1) == 0)
            {
                pstatstg.pwcsName = this._stream.ToString();
            }
        }

        //
        // Summary:
        //     Removes the access restriction on a range of bytes previously restricted
        //     with the System.Runtime.InteropServices.ComTypes.IStream.LockRegion(System.Int64,System.Int64,System.Int32)
        //     method.
        //
        // Parameters:
        //   libOffset:
        //     The byte offset for the beginning of the range.
        //
        //   cb:
        //     The length, in bytes, of the range to restrict.
        //
        //   dwLockType:
        //     The access restrictions previously placed on the range.
        public void UnlockRegion(long libOffset, long cb, int dwLockType)
        {
            throw new NotSupportedException("ReadOnlyIStreamWrapper does not support UnlockRegion");
        }

        //
        // Summary:
        //     Writes a specified number of bytes into the stream object starting at the
        //     current seek pointer.
        //
        // Parameters:
        //   pv:
        //     The buffer to write this stream to.
        //
        //   cb:
        //     The number of bytes to write to the stream.
        //
        //   pcbWritten:
        //     On successful return, contains the actual number of bytes written to the
        //     stream object. If the caller sets this pointer to System.IntPtr.Zero, this
        //     method does not provide the actual number of bytes written.
        public void Write(byte[] pv, int cb, IntPtr pcbWritten)
        {
            System.Runtime.InteropServices.Marshal.WriteInt64(pcbWritten, 0L);
            this._stream.Write(pv, 0, cb);
            System.Runtime.InteropServices.Marshal.WriteInt64(pcbWritten, (long)cb);
        }
    }
}
