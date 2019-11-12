//-----------------------------------------------------------------------
// <copyright file="DataManager.cpp" company="Microsoft">
//      Copyright (c) 2006 Microsoft Corporation. All rights reserved.
// </copyright>
//
// Module:
//      DataManager.cpp
//
// Description:
//
//-----------------------------------------------------------------------


#include "DataManager.h"
#include <intsafe.h>


const size_t NODES_TO_RESERVE = 256;
const size_t INVALID_APPLICATION_NODE_INDEX = (size_t)-1;


extern CDevice g_Device;


CDataManager::CDataManager(const CNodeDefaultBackground DefaultBackgroundNode) :
m_DefaultBackgroundNode(DefaultBackgroundNode)
{
    m_pNotificationNodes.reserve(NODES_TO_RESERVE);
    m_pApplicationNodes.reserve(NODES_TO_RESERVE);
    m_indexCurrentApplicationPage = 0;

    // Note, on target platforms earlier than Vista, InitializeCriticalSection can throw a
    // STATUS_NO_MEMORY exception and thus should be encapsulated in a try/catch. However, since
    // Windows SideShow drivers can only compile for Vista and later, this is not necessary.
    InitializeCriticalSection(&m_CriticalSection);

    // Send default background to device
    EnterCriticalSection(&m_CriticalSection);

    RenderAndSendDataToDevice();

    LeaveCriticalSection(&m_CriticalSection);

    return;
}


CDataManager::~CDataManager(void)
{
    DeleteAllApplications();
    DeleteCriticalSection(&m_CriticalSection);

    return;
}

//
// To avoid PREfast warning 28197: Possibly leaking memory
// STL vector does not have annotation to supress the warnning
//
HRESULT CDataManager::AddApplication(__drv_aliasesMem CNodeApplication *pApplication)
{
    HRESULT hr = S_OK;

    if (NULL != pApplication)
    {
        m_pApplicationNodes.push_back(pApplication);
    }
    else
    {
        hr = E_OUTOFMEMORY;
    }
    
    return hr;
}

//
// To avoid PREfast warning 28197: Possibly leaking memory
// STL vector does not have annotation to supress the warnning
//
HRESULT CDataManager::AddNotification(__drv_aliasesMem CNodeNotification *pNotfication)
{
    HRESULT hr = S_OK;

    if (NULL != pNotfication)
    {
        m_pNotificationNodes.push_back(pNotfication);
    }
    else
    {
        hr = E_OUTOFMEMORY;
    }

    return hr;
}


HRESULT CDataManager::AddApplication(GUID guidApplicationId,
                                     _In_reads_(SizeOfTitle) __drv_aliasesMem wchar_t* wszTitle,
                                     size_t SizeOfTitle,
                                     __drv_aliasesMem HICON* pIcon,
                                     size_t SizeOfIcon)
{
    EnterCriticalSection(&m_CriticalSection);

    HRESULT hr = S_OK;

    // 1. See if an existing node has this guidApplicationId.
    // 2. If yes, replace the data of that one with this one. If no, put this one at the end.

    const size_t QuantityOfApplicationNodes = m_pApplicationNodes.size();
    size_t index = 0;
    for ((index); index < QuantityOfApplicationNodes; index++)
    {
        if (TRUE == IsEqualGUID(guidApplicationId, m_pApplicationNodes.at(index)->m_guidApplicationId))
        {
            CNodeApplication* ExistingApplicationNode = m_pApplicationNodes[index];
            if (NULL != ExistingApplicationNode->m_wszTitle)
            {
                delete [] ExistingApplicationNode->m_wszTitle;
                // No need to set to NULL as next line sets it to a valid pointer
            }
            ExistingApplicationNode->m_wszTitle = wszTitle;
            ExistingApplicationNode->m_SizeOfTitle = SizeOfTitle;
            if (NULL != ExistingApplicationNode->m_pIcon)
            {
                delete ExistingApplicationNode->m_pIcon;
                // No need to set to NULL as next line sets it to a valid pointer
            }
            ExistingApplicationNode->m_pIcon = pIcon;
            ExistingApplicationNode->m_SizeOfIcon = SizeOfIcon;
            ExistingApplicationNode->m_fNeedToUpdateDevice = true;
            break;
        }
    }
    if (QuantityOfApplicationNodes == index)
    {
        CNodeApplication* pNewApplicationNode = new(std::nothrow) CNodeApplication(guidApplicationId,
                                                                                   wszTitle,
                                                                                   SizeOfTitle,
                                                                                   pIcon,
                                                                                   SizeOfIcon);
        hr = AddApplication(pNewApplicationNode);
    }

    RenderAndSendDataToDevice();

    LeaveCriticalSection(&m_CriticalSection);
    return hr;
}


// Update an application (node) from the ApplicationNode vector list. Does not create one if it doesn't exist
// Return: REGDB_E_CLASSNOTREG: The guid doesn't exist
// Return: S_OK: The application was updated
HRESULT CDataManager::UpdateApplicationNode(GUID guidApplicationId,
                                            _In_reads_(SizeOfBody) __drv_aliasesMem wchar_t* wszBody,
                                            size_t SizeOfBody)
{
    EnterCriticalSection(&m_CriticalSection);

    HRESULT hr = S_OK;

    const size_t QuantityOfApplicationNodes = m_pApplicationNodes.size();
    size_t index = 0;
    for ((index); index < QuantityOfApplicationNodes; index++)
    {
        if (TRUE == IsEqualGUID(guidApplicationId, m_pApplicationNodes.at(index)->m_guidApplicationId))
        {
            CNodeApplication* ExistingApplicationNode = m_pApplicationNodes[index];
            if (NULL != ExistingApplicationNode->m_wszBody)
            {
                delete [] ExistingApplicationNode->m_wszBody;
                // No need to set to NULL as next line sets it to a valid pointer
            }
            ExistingApplicationNode->m_wszBody = wszBody;
            ExistingApplicationNode->m_SizeOfBody = SizeOfBody;
            ExistingApplicationNode->m_fNeedToUpdateDevice = true;
            break;
        }
    }
    if (QuantityOfApplicationNodes == index)
    {
        hr = REGDB_E_CLASSNOTREG;
    }

    if (SUCCEEDED(hr))
    {
        RenderAndSendDataToDevice();
    }

    LeaveCriticalSection(&m_CriticalSection);
    return hr;
}


HRESULT CDataManager::DeleteApplicationContent(GUID guidApplicationId)
{
    EnterCriticalSection(&m_CriticalSection);

    HRESULT hr = S_OK;

    HRESULT hrNotifications = DeleteAllNotificationsForAnApplication_Internal(guidApplicationId);

    const size_t QuantityOfApplicationNodes = m_pApplicationNodes.size();
    size_t index = 0;
    for ((index); index < QuantityOfApplicationNodes; index++)
    {
        if (TRUE == IsEqualGUID(guidApplicationId, m_pApplicationNodes.at(index)->m_guidApplicationId))
        {
            CNodeApplication* ExistingApplicationNode = m_pApplicationNodes[index];
            if (NULL != ExistingApplicationNode->m_wszBody)
            {
                delete [] ExistingApplicationNode->m_wszBody;
                ExistingApplicationNode->m_wszBody = NULL;
            }
            ExistingApplicationNode->m_SizeOfBody = 0;
            ExistingApplicationNode->m_fNeedToUpdateDevice = true;
            break;
        }
    }
    if ((QuantityOfApplicationNodes == index) && (S_FALSE == hrNotifications))
    {
        hr = REGDB_E_CLASSNOTREG;
    }

    if (SUCCEEDED(hr))
    {
        RenderAndSendDataToDevice();
    }

    LeaveCriticalSection(&m_CriticalSection);
    return hr;
}


// Delete an application (node) from the ApplicationNode vector list
// Return: REGDB_E_CLASSNOTREG: The ApplicationID guid doesn't exist and there were no notifications using this ApplicationID guid
// Return: S_OK: The application was deleted
HRESULT CDataManager::DeleteApplication(GUID guidApplicationId)
{
    EnterCriticalSection(&m_CriticalSection);

    HRESULT hr = S_OK;

    HRESULT hrNotifications = DeleteAllNotificationsForAnApplication_Internal(guidApplicationId);

    const size_t QuantityOfApplicationNodes = m_pApplicationNodes.size();
    size_t index = 0;
    for ((index); index < QuantityOfApplicationNodes; index++)
    {
        if (TRUE == IsEqualGUID(guidApplicationId, m_pApplicationNodes.at(index)->m_guidApplicationId))
        {
            m_pApplicationNodes[index]->DeepDeleteNode();
            delete m_pApplicationNodes[index];
            m_pApplicationNodes.erase(m_pApplicationNodes.begin() + index);
            // Ensure the index to the current application page is still correct
            if (m_indexCurrentApplicationPage > index)
            {
                m_indexCurrentApplicationPage--;
            }
            else if (m_indexCurrentApplicationPage == index)
            {
                m_indexCurrentApplicationPage = ForwardFindValidApplicationNodeIndex();
            }
            // else if m_indexCurrentApplicationPage < index, nothing special is necesary
            break;
        }
    }
    if ((QuantityOfApplicationNodes == index) && (S_FALSE == hrNotifications))
    {
        hr = REGDB_E_CLASSNOTREG;
    }

    if (SUCCEEDED(hr))
    {
        RenderAndSendDataToDevice();
    }

    LeaveCriticalSection(&m_CriticalSection);
    return hr;
}


HRESULT CDataManager::DeleteAllApplications(void)
{
    EnterCriticalSection(&m_CriticalSection);

    DeleteAllNotifications();

    for (size_t index = 0; index < m_pApplicationNodes.size(); index++)
    {
        m_pApplicationNodes[index]->DeepDeleteNode();
        delete m_pApplicationNodes[index];
    }

    m_pApplicationNodes.clear();
    m_pApplicationNodes.reserve(NODES_TO_RESERVE);
    m_indexCurrentApplicationPage = 0;

    RenderAndSendDataToDevice();

    LeaveCriticalSection(&m_CriticalSection);
    return S_OK;
}


HRESULT CDataManager::GetApplications(
    _Outptr_result_maybenull_ APPLICATION_ID** ppAppIds,
    _Out_ DWORD* pcAppIds)
{
    if (NULL == ppAppIds || NULL == pcAppIds)
    {
        return E_INVALIDARG;
    }

    EnterCriticalSection(&m_CriticalSection);

    HRESULT hr = S_OK;
    const size_t  cApplications = m_pApplicationNodes.size();

    // Initialize out parameters
    *ppAppIds = NULL;
    *pcAppIds = 0;

    // Determine how much data we need to allocate
    if (DWORD_MAX > cApplications)
    {
        *pcAppIds = (DWORD)cApplications;
    }
    else
    {
        hr = E_UNEXPECTED;
    }     

    // Allocate memory to hold the APPLICATION_IDs
    if (SUCCEEDED(hr))
    {
        *ppAppIds = (APPLICATION_ID*)::CoTaskMemAlloc(*pcAppIds * sizeof(APPLICATION_ID));
        if (NULL == *ppAppIds)
        {
            hr = E_OUTOFMEMORY;
        }
        else
        {
            ::ZeroMemory(*ppAppIds, *pcAppIds * sizeof(APPLICATION_ID));
        }
    }

    for (size_t index = 0; SUCCEEDED(hr) && index < *pcAppIds; index++)
    {
        memcpy(&(*ppAppIds)[index], &m_pApplicationNodes[index]->m_guidApplicationId, sizeof(APPLICATION_ID));
    }

    LeaveCriticalSection(&m_CriticalSection);

    if (FAILED(hr))
    {
        ::CoTaskMemFree(*ppAppIds);
        *ppAppIds = NULL;
        *pcAppIds = 0;
    }

    return hr;
}

HRESULT CDataManager::ChangeSortOrder(_In_reads_(cApps) const APPLICATION_ID* pApps, const DWORD cApps)
{
    // If the count of apps is 0 or the pointer to the list of apps is NULL, then there is no work to do
    if ((NULL == pApps) ||
        (0 == cApps))
    {
        return S_FALSE;
    }

    HRESULT hr = S_OK;

    EnterCriticalSection(&m_CriticalSection);

    // 1. Store current app index
    size_t index = 0;
    CNodeApplication* pCurrentNode = NULL;

    if (0 != m_pApplicationNodes.size())
    {
        pCurrentNode = m_pApplicationNodes[m_indexCurrentApplicationPage];
    }

    // 2. Create a new app list, save the old one
    NODEAPPLICATIONVECTOR VectorOld;
    m_pApplicationNodes.swap(VectorOld);

    m_pApplicationNodes.clear();
    m_pApplicationNodes.reserve(NODES_TO_RESERVE);

    // 3. foreach app in pApps
    //      a. if exists in old list, copy into new list, and erase from old list
    //      b. else if not exists, create new empty node in new list
    for (DWORD nApp = 0; nApp < cApps; ++nApp)
    {
        CNodeApplication* pNewNode = NULL;

        const size_t QuantityOfApplicationNodes = VectorOld.size();
        for (index = 0; index < QuantityOfApplicationNodes; index++)
        {
            if (TRUE == IsEqualGUID(pApps[nApp], VectorOld.at(index)->m_guidApplicationId))
            {
                pNewNode = VectorOld[index];
                VectorOld.erase(VectorOld.begin() + index);
                break;
            }
        }
        if (QuantityOfApplicationNodes == index)
        {
            pNewNode = new(std::nothrow) CNodeApplication(pApps[nApp],
                                                          NULL,
                                                          0,
                                                          NULL,
                                                          0);
            if (NULL == pNewNode)
            {
                hr = E_OUTOFMEMORY;
            }
        }

        if (NULL != pNewNode)
        {
            AddApplication(pNewNode);
        }
    }

    // 4. copy any remaining nodes in old list to new list
    if (0 != VectorOld.size())
    {
        m_pApplicationNodes.insert(m_pApplicationNodes.end(), VectorOld.begin(), VectorOld.end());
    }

    // 5. free old list
    VectorOld.clear();

    // 6. reset current app index
    const size_t QuantityOfApplicationNodes = m_pApplicationNodes.size();
    for (index = 0; index < QuantityOfApplicationNodes; index++)
    {
        if (m_pApplicationNodes[index] == pCurrentNode)
        {
            break;
        }
    }

    if (index == QuantityOfApplicationNodes)
    {
        // We couldn't find the previously current node in the new list, so let's
        // find the first one that has valid data to display
        m_indexCurrentApplicationPage = 0;
        m_indexCurrentApplicationPage = ForwardFindValidApplicationNodeIndex();
    }
    else
    {
        // Reset the current node to what it was before we changed the order
        m_indexCurrentApplicationPage = index;
    }

    // 7. re-render data!
    RenderAndSendDataToDevice();

    LeaveCriticalSection(&m_CriticalSection);

    return hr;
}


HRESULT CDataManager::AddNotification(GUID guidApplicationId,
                                      unsigned long ulNotificationId,
                                      _In_reads_(SizeOfTitle) __drv_aliasesMem wchar_t* wszTitle,
                                      size_t SizeOfTitle,
                                      _In_reads_(SizeOfBody) __drv_aliasesMem wchar_t* wszBody,
                                      size_t SizeOfBody,
                                      __drv_aliasesMem HICON* pIcon,
                                      size_t SizeOfIcon,
                                      FILETIME notificationExpirationTime)
{
    EnterCriticalSection(&m_CriticalSection);

    CNodeNotification* pNewNotificationNode = new(std::nothrow) CNodeNotification(guidApplicationId,
                                                                                  ulNotificationId,
                                                                                  wszTitle,
                                                                                  SizeOfTitle,
                                                                                  wszBody,
                                                                                  SizeOfBody,
                                                                                  pIcon,
                                                                                  SizeOfIcon,
                                                                                  notificationExpirationTime);

    HRESULT hr = AddNotification(pNewNotificationNode);

    RenderAndSendDataToDevice();

    LeaveCriticalSection(&m_CriticalSection);
    return hr;
}


// This method deletes the first notification that matches the requirements. If an ISV
// sends multiple notifications with the same NotificationID, they will need to call this
// method multiple times.
HRESULT CDataManager::DeleteNotification(GUID guidApplicationId,
                                         unsigned long ulNotificationId)
{
    EnterCriticalSection(&m_CriticalSection);

    HRESULT hr = S_OK;

    const size_t QuantityOfNotificationNodes = m_pNotificationNodes.size();
    size_t index = 0;
    for ((index); index < QuantityOfNotificationNodes; index++)
    {
        // NotificationID and ApplicationID must match
        if ((m_pNotificationNodes.at(index)->m_ulNotificationId == ulNotificationId) &&
            (TRUE == IsEqualGUID(guidApplicationId, m_pNotificationNodes.at(index)->m_guidApplicationId)))
        {
            m_pNotificationNodes[index]->DeepDeleteNode();
            delete m_pNotificationNodes[index];
            m_pNotificationNodes.erase(m_pNotificationNodes.begin() + index);
            break;
        }
    }
    if (QuantityOfNotificationNodes == index)
    {
        hr = REGDB_E_CLASSNOTREG;
    }

    if (SUCCEEDED(hr))
    {
        RenderAndSendDataToDevice();
    }

    LeaveCriticalSection(&m_CriticalSection);
    return hr;
}


HRESULT CDataManager::DeleteAllNotificationsForAnApplication(const GUID guidApplicationId)
{
    EnterCriticalSection(&m_CriticalSection);

    HRESULT hr = DeleteAllNotificationsForAnApplication_Internal(guidApplicationId);

    LeaveCriticalSection(&m_CriticalSection);
    return hr;
}


// Return: S_OK: Deleted all notifications for the application
// Return: S_FALSE: There were no notifications to delete
HRESULT CDataManager::DeleteAllNotificationsForAnApplication_Internal(const GUID guidApplicationId)
{
    // Private Method. Does not need critical section

    const size_t QuantityOfNotificationNodes = m_pNotificationNodes.size();
    if (0 == QuantityOfNotificationNodes)
    {
        return S_FALSE;
    }

    // Enumerate through vector backwards so to delete multiple nodes in one pass
    for (size_t index = QuantityOfNotificationNodes - 1; index != (size_t)-1; index--) // Note: index is unsigned, which explains the termination check logic
    {
        if (TRUE == IsEqualGUID(guidApplicationId, m_pNotificationNodes.at(index)->m_guidApplicationId))
        {
            m_pNotificationNodes[index]->DeepDeleteNode();
            delete m_pNotificationNodes[index];
            m_pNotificationNodes.erase(m_pNotificationNodes.begin() + index);
        }
    }

    return S_OK;
}


HRESULT CDataManager::DeleteAllNotifications(void)
{
    // Private Method. Does not need critical section

    for (size_t index = 0; index < m_pNotificationNodes.size(); index++)
    {
        m_pNotificationNodes[index]->DeepDeleteNode();
        delete m_pNotificationNodes[index];
    }

    m_pNotificationNodes.clear();
    m_pNotificationNodes.reserve(NODES_TO_RESERVE);

    return S_OK;
}


HRESULT CDataManager::HandleDeviceEvent(CDevice::DeviceEvent DeviceEvent)
{
    EnterCriticalSection(&m_CriticalSection);

    const size_t QuantityOfNotificationNodes = m_pNotificationNodes.size();
    const size_t QuantityOfApplicationNodes = m_pApplicationNodes.size();
    bool fForceRender = false;

    // If the device triggered a RenderAgain event
    if (CDevice::RenderAgain == DeviceEvent)
    {
        fForceRender = true;
    }
    // If there is at least one notification, clear the last one (at end of vector) regardless of button press
    else if (0 != QuantityOfNotificationNodes)
    {
        m_pNotificationNodes[QuantityOfNotificationNodes - 1]->DeepDeleteNode();
        delete m_pNotificationNodes[QuantityOfNotificationNodes - 1];
        m_pNotificationNodes.erase(m_pNotificationNodes.end() - 1);
    }
    // If the next button, increment the current application page index (with wraparound)
    // If the previous button, decrement the current application page index (with wraparound)
    else if (0 != QuantityOfApplicationNodes)
    {
        switch (DeviceEvent)
        {
        case CDevice::ButtonNext:
            m_indexCurrentApplicationPage++;
            m_indexCurrentApplicationPage = ForwardFindValidApplicationNodeIndex();
            break;
        case CDevice::ButtonPrevious:
            if ((0 == m_indexCurrentApplicationPage) ||
                (QuantityOfApplicationNodes <= m_indexCurrentApplicationPage))
            {
                m_indexCurrentApplicationPage = QuantityOfApplicationNodes - 1;
            }
            else
            {
                m_indexCurrentApplicationPage--;
            }
            m_indexCurrentApplicationPage = ReverseFindValidApplicationNodeIndex();
            break;
        default:
            break;
        }
    }
    // else the current page is the default background. No work to do here.

    RenderAndSendDataToDevice(fForceRender);

    LeaveCriticalSection(&m_CriticalSection);
    return S_OK;
}


// Return: S_FALSE if display wasn't changed
// Return: S_OK if display is changed
HRESULT CDataManager::TimerMaintenance(void)
{
    EnterCriticalSection(&m_CriticalSection);

    HRESULT hr = S_OK;

    // Only change page if it's an application page
    if ((0 != m_pNotificationNodes.size()) ||
        (0 == m_pApplicationNodes.size()))
    {
        hr = S_FALSE;
    }

    if (S_OK == hr)
    {
        m_indexCurrentApplicationPage++;
        m_indexCurrentApplicationPage = ForwardFindValidApplicationNodeIndex();

        RenderAndSendDataToDevice();
    }

    LeaveCriticalSection(&m_CriticalSection);
    return hr;
}


// fForceRender: If true, this will re-render the current page regardless if the data has changed
// Return: S_OK: Render data is populated and needs to be sent to device
// Return: S_FALSE: Nothing needs to be rendered
HRESULT CDataManager::RenderProperPage(CRenderedData* pRenderedData,
                                       bool fForceRender) // Default fForceRender = false
{
    // Private Method. Does not need critical section

    HRESULT hr = S_FALSE;
    const size_t QuantityOfNotificationNodes = m_pNotificationNodes.size();
    m_indexCurrentApplicationPage = ForwardFindValidApplicationNodeIndex();
    //const size_t QuantityOfApplicationNodes = m_pApplicationNodes.size();

    // If Notification nodes vector isn't empty
    if (0 != QuantityOfNotificationNodes)
    {
        // Check if the last node in the vector needs to be rendered
        if ((true == m_pNotificationNodes[QuantityOfNotificationNodes - 1]->m_fNeedToUpdateDevice) ||
            (m_pLastRenderedNode != m_pNotificationNodes[QuantityOfNotificationNodes - 1]) ||
            (true == fForceRender))
        {
            // Render it
            m_pNotificationNodes[QuantityOfNotificationNodes - 1]->m_Renderer.RenderData(m_pNotificationNodes[QuantityOfNotificationNodes - 1],
                                                                                         pRenderedData);
            // Set ID of last rendered node
            m_pLastRenderedNode = m_pNotificationNodes[QuantityOfNotificationNodes - 1];

            hr = S_OK;
        }
    }
    // Else if Application nodes vector isn't empty
    else if (INVALID_APPLICATION_NODE_INDEX != m_indexCurrentApplicationPage)
    {
        // Check if the node needs to be rendered
        if ((true == m_pApplicationNodes[m_indexCurrentApplicationPage]->m_fNeedToUpdateDevice) ||
            (m_pLastRenderedNode != m_pApplicationNodes[m_indexCurrentApplicationPage]) ||
            (true == fForceRender))
        {
            // Render it
            m_pApplicationNodes[m_indexCurrentApplicationPage]->m_Renderer.RenderData(m_pApplicationNodes[m_indexCurrentApplicationPage],
                                                                                      pRenderedData);
            // Set ID of last rendered node
            m_pLastRenderedNode = m_pApplicationNodes[m_indexCurrentApplicationPage];

            hr = S_OK;
        }
    }
    else // Else if both the Notification vector and Application vector is empty
    {
        // Check if the node needs to be rendered
        if ((true == m_DefaultBackgroundNode.m_fNeedToUpdateDevice) ||
            (m_pLastRenderedNode != &m_DefaultBackgroundNode) ||
            (true == fForceRender))
        {
            // Render it
            m_DefaultBackgroundNode.m_Renderer.RenderData(&m_DefaultBackgroundNode,
                                                          pRenderedData);
            // Set ID of last rendered node
            m_pLastRenderedNode = &m_DefaultBackgroundNode;

            hr = S_OK;
        }
    }

    return hr;
}


HRESULT CDataManager::RenderAndSendDataToDevice(bool fForceRender) // Default fForceRender = false
{
    // Private Method. Does not need critical section

    CRenderedData RenderedData;
    if (S_OK == RenderProperPage(&RenderedData, fForceRender))
    {
        g_Device.SendRenderedDataToDevice(RenderedData);
    }
    RenderedData.DeleteData();

    return S_OK;
}


size_t CDataManager::ForwardFindValidApplicationNodeIndex(void)
{
    // Private Method. Does not need critical section

    const size_t QuantityOfApplicationNodes = m_pApplicationNodes.size();

    if (0 == QuantityOfApplicationNodes)
    {
        return INVALID_APPLICATION_NODE_INDEX;
    }

    // Ensure the index is in range
    if (QuantityOfApplicationNodes <= m_indexCurrentApplicationPage)
    {
        m_indexCurrentApplicationPage = 0;
    }

    size_t indexProposedApplicationPage = m_indexCurrentApplicationPage;
    // Loop until there is an application node that contains a non-NULL wszBody
    while (NULL == m_pApplicationNodes[indexProposedApplicationPage]->m_wszBody)
    {
        indexProposedApplicationPage++;
        // Wraparound if necessary
        if (QuantityOfApplicationNodes <= indexProposedApplicationPage)
        {
            indexProposedApplicationPage = 0;
        }

        // If a complete loop through the vector occurred, break
        if (m_indexCurrentApplicationPage == indexProposedApplicationPage)
        {
            indexProposedApplicationPage = INVALID_APPLICATION_NODE_INDEX;
            break;
        }
    }

    return indexProposedApplicationPage;
}


size_t CDataManager::ReverseFindValidApplicationNodeIndex(void)
{
    // Private Method. Does not need critical section

    const size_t QuantityOfApplicationNodes = m_pApplicationNodes.size();

    if (0 == QuantityOfApplicationNodes)
    {
        return INVALID_APPLICATION_NODE_INDEX;
    }

    // Ensure the index is in range
    if (QuantityOfApplicationNodes <= m_indexCurrentApplicationPage)
    {
        m_indexCurrentApplicationPage = 0;
    }

    size_t indexProposedApplicationPage = m_indexCurrentApplicationPage;
    // Loop until there is an application node that contains a non-NULL wszBody
    while (NULL == m_pApplicationNodes[indexProposedApplicationPage]->m_wszBody)
    {
        indexProposedApplicationPage--;
        // Wraparound if necessary
        if ((size_t)-1 == indexProposedApplicationPage)
        {
            indexProposedApplicationPage = QuantityOfApplicationNodes - 1;
        }

        // If a complete loop through the vector occurred, break
        if (m_indexCurrentApplicationPage == indexProposedApplicationPage)
        {
            indexProposedApplicationPage = INVALID_APPLICATION_NODE_INDEX;
            break;
        }
    }

    return indexProposedApplicationPage;
}
