//-----------------------------------------------------------------------
// <copyright file="Node.cpp" company="Microsoft">
//      Copyright (c) 2006 Microsoft Corporation. All rights reserved.
// </copyright>
//
// Module:
//      Node.cpp
//
// Description:
//
//-----------------------------------------------------------------------


#include "Node.h"


CNodeApplication::CNodeApplication(GUID guidApplicationId,
                                   _In_opt_ __drv_aliasesMem wchar_t* wszTitle,
                                   size_t SizeOfTitle,
                                   __drv_aliasesMem HICON* pIcon,
                                   size_t SizeOfIcon)
{
    m_nodeType = NodeTypeApplication;
    m_guidApplicationId = guidApplicationId;
    m_wszTitle = wszTitle;
    m_SizeOfTitle = SizeOfTitle;
    m_wszBody = NULL;
    m_SizeOfBody = 0;
    m_pIcon = pIcon;
    m_SizeOfIcon = SizeOfIcon;
    m_fNeedToUpdateDevice = true;

    return;
}


CNodeNotification::CNodeNotification(GUID guidApplicationId,
                                     unsigned long ulNotificationId,
                                     _In_reads_(SizeOfTitle) __drv_aliasesMem wchar_t* wszTitle,
                                     size_t SizeOfTitle,
                                     _In_reads_(SizeOfBody) __drv_aliasesMem wchar_t* wszBody,
                                     size_t SizeOfBody,
                                     HICON* pIcon,
                                     size_t SizeOfIcon,
                                     FILETIME notificationExpirationTime) :
m_ulNotificationId(ulNotificationId),
m_notificationExpirationTime(notificationExpirationTime)
{
    m_nodeType = NodeTypeNotification;
    m_guidApplicationId = guidApplicationId;
    m_wszTitle = wszTitle;
    m_SizeOfTitle = SizeOfTitle;
    m_wszBody = wszBody;
    m_SizeOfBody = SizeOfBody;
    m_pIcon = pIcon;
    m_SizeOfIcon = SizeOfIcon;
    m_fNeedToUpdateDevice = true;

    return;
}


CNodeDefaultBackground::CNodeDefaultBackground(_In_reads_(SizeOfTitle) __drv_aliasesMem wchar_t* wszTitle,
                                               size_t SizeOfTitle,
                                               _In_reads_(SizeOfBody) __drv_aliasesMem wchar_t* wszBody,
                                               size_t SizeOfBody,
                                               __drv_aliasesMem HICON* pIcon,
                                               size_t SizeOfIcon)
{
    m_nodeType = NodeTypeDefaultBackground;
    m_wszTitle = wszTitle;
    m_SizeOfTitle = SizeOfTitle;
    m_wszBody = wszBody;
    m_SizeOfBody = SizeOfBody;
    m_pIcon = pIcon;
    m_SizeOfIcon = SizeOfIcon;
    m_fNeedToUpdateDevice = true;

    return;
}


HRESULT CNodeBase::DeepDeleteNode(void)
{
    if (NULL != m_wszTitle)
    {
        delete [] m_wszTitle;
        m_wszTitle = NULL;
    }
    if (NULL != m_wszBody)
    {
        delete [] m_wszBody;
        m_wszBody = NULL;
    }
    if (NULL != m_pIcon)
    {
        delete m_pIcon;
        m_pIcon = NULL;
    }

    return S_OK;
}
