//
//  Windows Server (Printing) Driver Development Kit Samples.
//
//  Sample Print Provider template.
//
//  Copyright (c) 1990 - 2005 Microsoft Corporation.
//  All Rights Reserved.
//
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED AS IS WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
//

#ifndef __NOTIFYDATA_HPP__
#define __NOTIFYDATA_HPP__

//
// CPrintOEMAsyncNotifyDataObject implements data object that is sent
// with async notification.
//

class CPrintOEMAsyncNotifyDataObject : public IPrintAsyncNotifyDataObject
{
public:

    CPrintOEMAsyncNotifyDataObject(
        );

    CPrintOEMAsyncNotifyDataObject(
        _In_reads_bytes_opt_(Size)   BYTE*                           pData,
                                ULONG                           Size,
        _In_opt_                PrintAsyncNotificationType*     pType
        );

    HRESULT
    IsValid(
        VOID
        ) const;

    //
    // IUnknown methods
    //
    STDMETHODIMP
    QueryInterface(
        _In_ REFIID  riid,
        _Out_ VOID**  ppv
        );

    STDMETHODIMP_(ULONG)
    AddRef(
        VOID
        );

    STDMETHODIMP_(ULONG)
    Release(
        VOID
        );

    //
    // INotifyDataObject methods
    //

    //
    // Gets data associated with a notification.
    //

    STDMETHODIMP
    AcquireData(
        _Outptr_opt_result_buffer_(*pSize)  BYTE**                       ppbData,
        _Out_opt_                           ULONG*                       pSize,
        _Outptr_opt_                        PrintAsyncNotificationType** pType
        );

    //
    // Releases data associated with a notification.
    //

    STDMETHODIMP
    ReleaseData(
        VOID
        );

private:

    ~CPrintOEMAsyncNotifyDataObject(
        );

    //
    // Member variables
    //

    HRESULT                         m_hValid;
    BYTE*                           m_Data;
    ULONG                           m_Size;
    PrintAsyncNotificationType*     m_Type;
    LONG                            m_cRef;
};


#endif //__NOTIFYDATA_HPP__

