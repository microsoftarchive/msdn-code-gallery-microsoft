// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Windows7.Sensors.Internal
{
    /// <summary>
    /// A structure containing the actual data type ID and a union of supported property value data types.
    /// Contains conversions to .NET objects.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct PROPVARIANT
    {
        [FieldOffset(0)]
        private VarEnum vt;

        [FieldOffset(8)]
        private SByte cVal;
        [FieldOffset(8)]
        private Byte bVal;
        [FieldOffset(8)]
        private UInt16 iVal;
        [FieldOffset(8)]
        private UInt16 uiVal;
        [FieldOffset(8)]
        private Int32 lVal;
        [FieldOffset(8)]
        private UInt32 ulVal;
        [FieldOffset(8)]
        private Int32 intVal;
        [FieldOffset(8)]
        private UInt32 uintVal;
        [FieldOffset(8)]
        private Int64 hVal;
        [FieldOffset(8)]
        private UInt64 uhVal;
        [FieldOffset(8)]
        private Single fltVal;
        [FieldOffset(8)]
        private Double dblVal;
        [FieldOffset(8), MarshalAs(UnmanagedType.VariantBool)]
        private Boolean boolVal;
        [FieldOffset(8), MarshalAs(UnmanagedType.Error)]
        private Int32 scode;
        [FieldOffset(8)]
        private System.Runtime.InteropServices.ComTypes.FILETIME filetime;
        [FieldOffset(8)]
        private IntPtr ptr;
        [FieldOffset(8)]
        private CArray cArray;

        public VarEnum VarType
        {
            get { return vt; }
        }

        public object Value
        {
            get { return GetValue(VarType); }
        }

        public void SetValue(bool value)
        {
            vt = VarEnum.VT_BOOL;
            boolVal = value;
        }

        public void SetValue(float value)
        {
            vt = VarEnum.VT_R4;
            fltVal = value;
        }

        public void SetValue(double value)
        {
            vt = VarEnum.VT_R8;
            dblVal = value;
        }

        public void SetFileTime(DateTime value)
        {
            vt = VarEnum.VT_FILETIME;
            hVal = value.ToFileTimeUtc();
        }

        public void SetValue(string value)
        {
            vt = VarEnum.VT_LPWSTR;
            ptr = Marshal.StringToCoTaskMemUni(value);
        }

        private unsafe object GetValue(VarEnum vt)
        {
            object value = null;

            switch ((VarEnum)vt)
            {
                case VarEnum.VT_EMPTY:
                case VarEnum.VT_NULL:
                    value = null;
                    break;

                case VarEnum.VT_I1:
                    value = cVal;
                    break;

                case VarEnum.VT_UI1:
                    value = bVal;
                    break;

                case VarEnum.VT_I2:
                    value = iVal;
                    break;

                case VarEnum.VT_UI2:
                    value = uiVal;
                    break;

                case VarEnum.VT_I4:
                    value = lVal;
                    break;

                case VarEnum.VT_UI4:
                    value = ulVal;
                    break;

                case VarEnum.VT_I8:
                    value = hVal;
                    break;

                case VarEnum.VT_UI8:
                    value = uhVal;
                    break;

                case VarEnum.VT_R4:
                    value = fltVal;
                    break;

                case VarEnum.VT_R8:
                    value = dblVal;
                    break;

                case VarEnum.VT_INT:
                    value = intVal;
                    break;

                case VarEnum.VT_UINT:
                    value = uintVal;
                    break;

                case VarEnum.VT_ERROR:
                    value = scode;
                    break;

                case VarEnum.VT_BOOL:
                    value = boolVal;
                    break;

                case VarEnum.VT_CY:
                    value = decimal.FromOACurrency(hVal);
                    break;

                case VarEnum.VT_DATE:
                    value = DateTime.FromOADate(dblVal);
                    break;

                case VarEnum.VT_FILETIME:
                    value = DateTime.FromFileTime(hVal);
                    break;

                case VarEnum.VT_BSTR:
                    value = Marshal.PtrToStringBSTR(ptr);
                    break;

                case VarEnum.VT_LPSTR:
                    value = Marshal.PtrToStringAnsi(ptr);
                    break;

                case VarEnum.VT_LPWSTR:
                    value = Marshal.PtrToStringUni(ptr);
                    break;

                case VarEnum.VT_UNKNOWN:
                    value = Marshal.GetObjectForIUnknown(ptr);
                    break;

                case VarEnum.VT_DISPATCH:
                    value = Marshal.GetObjectForIUnknown(ptr);
                    break;
                case VarEnum.VT_VECTOR | VarEnum.VT_I1:
                    value = GetVectorData<sbyte>();
                    break;
                case VarEnum.VT_VECTOR | VarEnum.VT_UI1:
                    value = GetVectorData<byte>();
                    break;
                case VarEnum.VT_VECTOR | VarEnum.VT_I2:
                    value = GetVectorData<short>();
                    break;
                case VarEnum.VT_VECTOR | VarEnum.VT_UI2:
                    value = GetVectorData<ushort>();
                    break;
                case VarEnum.VT_VECTOR | VarEnum.VT_I4:
                    value = GetVectorData<int>();
                    break;
                case VarEnum.VT_VECTOR | VarEnum.VT_UI4:
                    value = GetVectorData<uint>();
                    break;
                case VarEnum.VT_VECTOR | VarEnum.VT_I8:
                    value = GetVectorData<long>();
                    break;
                case VarEnum.VT_VECTOR | VarEnum.VT_UI8:
                    value = GetVectorData<ulong>();
                    break;
                case VarEnum.VT_VECTOR | VarEnum.VT_R4:
                    value = GetVectorData<float>();
                    break;
                case VarEnum.VT_VECTOR | VarEnum.VT_R8:
                    value = GetVectorData<double>();
                    break;
                case VarEnum.VT_VECTOR | VarEnum.VT_INT:
                    value = GetVectorData<int>();
                    break;
                case VarEnum.VT_VECTOR | VarEnum.VT_UINT:
                    value = GetVectorData<uint>();
                    break;
                case VarEnum.VT_VECTOR | VarEnum.VT_BOOL:
                    value = GetVectorData<bool>();
                    break;
                case VarEnum.VT_CLSID:
                    value = Marshal.PtrToStructure(ptr, typeof(Guid));
                    break;
                default:
                    throw new NotSupportedException("The type of this variable is not supported ('" + vt.ToString() + "')");
                    
            }

            return value;
        }

        [DllImport("ole32.dll")]
        private static extern Int32 PropVariantClear(ref PROPVARIANT pvar);

        [DllImport("kernel32.dll")]
        private static extern void RtlMoveMemory(IntPtr dst, IntPtr src, uint size);

        private unsafe T[] GetVectorData<T>() where T : struct
        {
            if (uiVal == 0 || cArray.pElems == IntPtr.Zero)
                return new T[0];

            T[] data = new T[uiVal];
            uint tSize = (uint) Marshal.SizeOf(typeof(T));
            
            GCHandle pinningGCH = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                IntPtr dst = pinningGCH.AddrOfPinnedObject();
                RtlMoveMemory(dst, cArray.pElems, uiVal * tSize);
            }
            finally
            {
                pinningGCH.Free();
            }
            
            return data;
        }

        public void Clear()
        {
            // Can't pass "this" by ref, so make a copy to call PropVariantClear with
            PROPVARIANT var = this;
            PropVariantClear(ref var);

            // Since we couldn't pass "this" by ref, we need to clear the member fields manually
            // NOTE: PropVariantClear already freed heap data for us, so we are just setting
            //       our references to null.
            vt = VarEnum.VT_EMPTY;
            ptr = IntPtr.Zero;
        }
    }

    public struct CArray
    {
        public uint cElems;
        public IntPtr pElems;
    }
}