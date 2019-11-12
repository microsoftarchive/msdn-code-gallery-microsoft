/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.IO;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using System.Runtime.InteropServices;

namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.UnitTests
{
    [System.Runtime.InteropServices.ComVisible(false)]
    internal class StreamConsts
    {
        public const int STREAM_SEEK_SET = 0x0;
        public const int STREAM_SEEK_CUR = 0x1;
        public const int STREAM_SEEK_END = 0x2;
    }
    
    /// <summary>
    ///     This class maps an Stream to an IStream. Instances of this class are
    ///     created by the <code>Stream.getComStream()</code> and
    /// <code>Stream.toComStream()</code> methods as needed. You would typically
    ///     never create instances of this class yourself.
    ///     The implementation of IStream provided by this class is not complete. In
    ///     particular, the <code>Clone</code>, <code>CopyTo</code>, and
    /// <code>Stat</code> methods return an E_NOTIMPL error code, and the
    /// <code>LockRegion</code>, <code>UnlockRegion</code>, and <code>Revert</code>
    ///     methods are simply empty implementations.
    /// </summary>

    [System.Runtime.InteropServices.ComVisible(false)]
    internal class ComStreamFromDataStream : IStream
    {
        protected Stream dataStream;

        // to support seeking ahead of the stream length...
        private long virtualPosition = -1;

        public ComStreamFromDataStream(Stream dataStream)
        {
            if (dataStream == null) throw new ArgumentNullException();
            this.dataStream = dataStream;
        }

        // Don't forget to set dataStream before using this object
        protected ComStreamFromDataStream()
        {
        }

        private void ActualizeVirtualPosition()
        {
            if (virtualPosition == -1) return;

            if (virtualPosition > dataStream.Length)
                dataStream.SetLength(virtualPosition);

            dataStream.Position = virtualPosition;

            virtualPosition = -1;
        }

        public virtual void Clone(out IStream ppstm)
        {
            NotImplemented();
            ppstm = null;
        }

        public virtual void Commit(uint grfCommitFlags)
        {
            dataStream.Flush();
            // Extend the length of the file if needed.
            ActualizeVirtualPosition();
        }

        public virtual void CopyTo(IStream pstm, ULARGE_INTEGER cb, ULARGE_INTEGER[] pcbRead, ULARGE_INTEGER[] pcbWritten)
        {
            long[] pcblRead = new long[1];
            pcbWritten[0].QuadPart = (ulong)CopyTo(pstm, (long)cb.QuadPart, pcblRead);
            pcbRead[0].QuadPart = (ulong)pcblRead[0];
        }

        public virtual long CopyTo(IStream pstm, long cb, long[] pcbRead)
        {
            int bufsize = 4096; // one page
            IntPtr buffer = Marshal.AllocHGlobal((IntPtr)bufsize);
            if (buffer == (IntPtr)0) throw new OutOfMemoryException();
            byte[] byteBuffer = new byte[bufsize];
            long written = 0;
            try
            {
                while (written < cb)
                {
                    int toRead = bufsize;
                    if (written + toRead > cb) toRead = (int)(cb - written);
                    int read = Read(buffer, toRead);
                    if (read == 0) break;
                    Marshal.PtrToStructure(buffer, byteBuffer);
                    uint pcbWritten = 0;
                    pstm.Write(byteBuffer, (uint)read, out pcbWritten);
                    if (pcbWritten != read)
                    {
                        throw EFail("Wrote an incorrect number of bytes");
                    }
                    written += read;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
            if (pcbRead != null && pcbRead.Length > 0)
            {
                pcbRead[0] = written;
            }

            return written;
        }

        public virtual Stream GetDataStream()
        {
            return dataStream;
        }

        public virtual void LockRegion(ULARGE_INTEGER libOffset, ULARGE_INTEGER cb, uint dwLockType)
        {
        }

        protected static ExternalException EFail(string msg)
        {
            ExternalException e = new ExternalException(msg, VSConstants.E_FAIL);
            throw e;
        }

        protected static void NotImplemented()
        {
            ExternalException e = new ExternalException("Not implemented.", VSConstants.E_NOTIMPL);
            throw e;
        }

        public virtual void Read(byte[] pv, uint cb, out uint pcbRead)
        {
            pcbRead = (uint)Read(pv, (int)cb);
        }

        public virtual int Read(IntPtr buf, /* cpr: int offset,*/  int length)
        {
            byte[] buffer = new byte[length];
            int count = Read(buffer, length);
            Marshal.Copy(buffer, 0, buf, length);
            return count;
        }

        public virtual int Read(byte[] buffer, /* cpr: int offset,*/  int length)
        {
            ActualizeVirtualPosition();
            return dataStream.Read(buffer, 0, length);
        }

        public virtual void Revert()
        {
            NotImplemented();
        }

        public virtual void Seek(LARGE_INTEGER offset, uint origin, ULARGE_INTEGER[] newPosition)
        {
            newPosition[0].QuadPart = (ulong)Seek((long)offset.QuadPart, (int)origin);
        }

        public virtual long Seek(long offset, int origin)
        {
            // Console.WriteLine("IStream::Seek("+ offset + ", " + origin + ")");
            long pos = virtualPosition;
            if (virtualPosition == -1)
            {
                pos = dataStream.Position;
            }
            long len = dataStream.Length;
            switch (origin)
            {
                case StreamConsts.STREAM_SEEK_SET:
                    if (offset <= len)
                    {
                        dataStream.Position = offset;
                        virtualPosition = -1;
                    }
                    else
                    {
                        virtualPosition = offset;
                    }
                    break;
                case StreamConsts.STREAM_SEEK_END:
                    if (offset <= 0)
                    {
                        dataStream.Position = len + offset;
                        virtualPosition = -1;
                    }
                    else
                    {
                        virtualPosition = len + offset;
                    }
                    break;
                case StreamConsts.STREAM_SEEK_CUR:
                    if (offset + pos <= len)
                    {
                        dataStream.Position = pos + offset;
                        virtualPosition = -1;
                    }
                    else
                    {
                        virtualPosition = offset + pos;
                    }
                    break;
            }
            if (virtualPosition != -1)
            {
                return virtualPosition;
            }
            else
            {
                return dataStream.Position;
            }
        }

        public virtual void SetSize(ULARGE_INTEGER value)
        {
            dataStream.SetLength((long)value.QuadPart);
        }

        public virtual void Stat(Microsoft.VisualStudio.OLE.Interop.STATSTG[] pstatstg, uint grfStatFlag)
        {
            NotImplemented();
        }

        public virtual void UnlockRegion(ULARGE_INTEGER libOffset, ULARGE_INTEGER cb, uint dwLockType)
        {
        }

        public virtual void Write(byte[] pv, uint cb, out uint pcbWritten)
        {
            pcbWritten = (uint)Write(pv, (int)cb);
        }

        public virtual int Write(IntPtr buf, /* cpr: int offset,*/ int length)
        {
            byte[] buffer = new byte[length];
            Marshal.Copy(buf, buffer, 0, length);
            return Write(buffer, length);
        }

        public virtual int Write(byte[] buffer, /* cpr: int offset,*/ int length)
        {
            ActualizeVirtualPosition();
            dataStream.Write(buffer, 0, length);
            return length;
        }
    }
}


