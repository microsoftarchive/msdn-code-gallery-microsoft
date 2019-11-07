/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    AbstractDeviceContent.h

Abstract:


--*/

#pragma once

class AbstractDeviceContent
{
public:
    AbstractDeviceContent() :
        CanDelete(false), 
        RequiredScope(FULL_DEVICE_ACCESS),
        MarkedForDeletion(false),
        m_ClientCount(0)        
    {
        Format = WPD_OBJECT_FORMAT_UNSPECIFIED;
        ContentType = WPD_CONTENT_TYPE_UNSPECIFIED;
    }

    AbstractDeviceContent(const AbstractDeviceContent& src) :
        CanDelete(false),
        m_ClientCount(0)
    {
        *this = src;
    }

    virtual ~AbstractDeviceContent()
    {
        for(size_t index = 0; index < m_Children.GetCount(); index++)
        {
            if (m_Children[index])
            {
                delete(m_Children[index]);
                m_Children[index] = NULL;
            }
        }
        m_Children.RemoveAll();
    }

    virtual AbstractDeviceContent& operator= (const AbstractDeviceContent& src)
    {
        ObjectID                    = src.ObjectID;
        PersistentUniqueID          = src.PersistentUniqueID;
        ParentID                    = src.ParentID;
        Name                        = src.Name;
        ContentType                 = src.ContentType;
        Format                      = src.Format;
        CanDelete                   = src.CanDelete;
        RequiredScope               = src.RequiredScope;
        ParentPersistentUniqueID    = src.ParentPersistentUniqueID;
        ContainerFunctionalObjectID = src.ContainerFunctionalObjectID;
        m_ClientCount               = src.m_ClientCount;

        return *this;
    }

    virtual HRESULT InitializeContent(
        _Inout_ DWORD *pdwLastObjectID);
   
    virtual HRESULT InitializeEnumerationContext(
                ACCESS_SCOPE                Scope,
        _Out_   WpdObjectEnumeratorContext* pEnumeratorContext);

    virtual HRESULT GetSupportedProperties(
        _Out_   IPortableDeviceKeyCollection *pKeys);

    virtual HRESULT GetPropertyAttributes(
                REFPROPERTYKEY         Key,
        _Out_   IPortableDeviceValues* pAttributes);

    virtual HRESULT GetValue(
                REFPROPERTYKEY         Key, 
        _Out_   IPortableDeviceValues* pStore);
        
    virtual HRESULT WriteValue(
                REFPROPERTYKEY         Key, 
                const PROPVARIANT&     Value);

    virtual HRESULT CreatePropertiesOnlyObject(
        _In_    IPortableDeviceValues* pObjectProperties, 
        _Inout_ DWORD*                 pdwLastObjectID,
        _Inout_ AbstractDeviceContent**          ppNewObject);

    virtual HRESULT WriteValues(
        _In_    IPortableDeviceValues* pValues,
        _Inout_ IPortableDeviceValues* pResults,
        _Out_   bool*                  pbObjectChanged); 

    virtual HRESULT DispatchClientArrival();

    virtual HRESULT DispatchClientDeparture();

    virtual HRESULT DispatchClientApplicationActivated();

    virtual HRESULT DispatchClientApplicationSuspended();  

    virtual HRESULT DispatchDeviceConnected();

    virtual HRESULT DispatchDeviceDisconnected();    

public:
    bool CanAccess(
                ACCESS_SCOPE Scope);

    HRESULT GetAllValues(
        _Inout_   IPortableDeviceValues* pStore);

    _Success_(return == true)
    bool FindNext(
                            ACCESS_SCOPE  Scope,
                            const DWORD   dwIndex,
        _Outptr_            AbstractDeviceContent** ppChild);

    HRESULT GetContent(
                ACCESS_SCOPE   Scope,
        _In_    LPCWSTR        wszObjectID,
        _Out_   AbstractDeviceContent**  ppContent);

    HRESULT GetObjectIDsByFormat(
                ACCESS_SCOPE                          Scope,
                REFGUID                               Format,
                const DWORD                           dwDepth,
        _Inout_ IPortableDevicePropVariantCollection* pObjectIDs);

    HRESULT GetObjectIDByPersistentID(
                ACCESS_SCOPE                          Scope,
        _In_    LPCWSTR                               wszPersistentID,
        _Out_   IPortableDevicePropVariantCollection* pObjectIDs);

    HRESULT MarkForDelete(
                const DWORD dwOptions);

    HRESULT RemoveObjectsMarkedForDeletion(
                ACCESS_SCOPE Scope);

public:
    CAtlStringW     ObjectID;
    CAtlStringW     PersistentUniqueID;
    CAtlStringW     ParentID;
    CAtlStringW     Name;
    CAtlStringW     ParentPersistentUniqueID;
    CAtlStringW     ContainerFunctionalObjectID;
    GUID            ContentType;
    GUID            Format;
    bool            CanDelete;
    bool            MarkedForDeletion;
    
    // A bitmask of all the required scopes in order to access this object
    ACCESS_SCOPE    RequiredScope;

    CComAutoCriticalSection           m_ChildrenCS;
    CAtlArray<AbstractDeviceContent*> m_Children;

    CComAutoCriticalSection           m_ClientCS;
    ULONG                             m_ClientCount;
};

