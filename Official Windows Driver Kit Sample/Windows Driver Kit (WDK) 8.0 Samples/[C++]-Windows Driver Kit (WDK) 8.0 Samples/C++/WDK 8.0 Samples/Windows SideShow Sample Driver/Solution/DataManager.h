//-----------------------------------------------------------------------
// <copyright file="DataManager.h" company="Microsoft">
//      Copyright (c) 2006 Microsoft Corporation. All rights reserved.
// </copyright>
//
// Module:
//      DataManager.h
//
// Description:
//      DataManager Class
//
//-----------------------------------------------------------------------


#include <windows.h>
#include <windowssideshowclassextension.h>
#include "Node.h"
#include "Device.h"
#include "RenderedData.h"


// Priority order of what page is displayed on the device
//      Most recent notification (if any)
//      Oldest notification (if any) (Notifications get stacked)
//      Application data for current page (if # applications >= 1) (Application data doesn't get stacked)
//      Default bitmap "Welcome to Windows SideShow"

// Assumptions:
// All Bitmap displays will have at least one button (such as a Next button)
// Notification expiration time is not supported in this driver.
//      Ramifications: An expired notification will have to be manually cleared on the device.
// The icon used for bitmap display will be the large icon.


// Singleton object
class CDataManager
{
public:
    CDataManager(const CNodeDefaultBackground DefaultBackgroundNode);
    ~CDataManager();

    // AddApplication adds (or updates) an application which creates a ApplicationNode (if one doesn't exist), but doesn't add application data to the node
    // This will also need to get the sort order to add the nodes in the proper order.
    // Caller must global new wszTitle, callee handles cleanup (delete [])
    HRESULT AddApplication(GUID guidApplicationId,
                           _In_reads_(SizeOfTitle) __drv_aliasesMem wchar_t* wszTitle,
                           size_t SizeOfTitle,
                           __drv_aliasesMem HICON* pIcon,
                           size_t SizeOfIcon);

    // Caller must global new wszBody, callee handles cleanup (delete [])
    HRESULT UpdateApplicationNode(GUID guidApplicationId,
                                  _In_reads_(SizeOfBody) __drv_aliasesMem wchar_t* wszBody,
                                  size_t SizeOfBody);
    HRESULT DeleteApplicationContent(GUID guidApplicationId); // This deletes all the content for one application (including notifications) but leaves the app
    HRESULT DeleteApplication(GUID guidApplicationId); // This deletes an application, its related ApplicationNode,
                                                       // and all associated notifications for the application
    HRESULT DeleteAllApplications(void); // This calls a loop of DeleteApplications.
                                         // It's also called upon SetCurrentUser user switch

    HRESULT GetApplications(_Outptr_result_maybenull_ APPLICATION_ID** ppAppIds, _Out_ DWORD* pcAppIds);

    // This changes the sort order for the applications. Note, this list can contain ApplicationId's which don't
    // correspond to existing ApplicationNodes. They will be stored here for later use.
    HRESULT ChangeSortOrder(_In_reads_(cApps) const APPLICATION_ID* pApps, const DWORD cApps);

    // Caller must global new wszTitle & wszBody, callee handles cleanup (delete [])
    HRESULT AddNotification(GUID guidApplicationId,
                            unsigned long ulNotificationId,
                            _In_reads_(SizeOfTitle) __drv_aliasesMem wchar_t* wszTitle,
                            size_t SizeOfTitle,
                            _In_reads_(SizeOfBody) __drv_aliasesMem wchar_t* wszBody,
                            size_t SizeOfBody,
                            __drv_aliasesMem HICON* pIcon,
                            size_t SizeOfIcon,
                            FILETIME notificationExpirationTime);
    HRESULT DeleteNotification(GUID guidApplicationId,
                               unsigned long ulNotificationId);

    HRESULT DeleteAllNotificationsForAnApplication(const GUID guidApplicationId);

    HRESULT HandleDeviceEvent(CDevice::DeviceEvent DeviceEvent); // Such as a Next or Previous button
                                                        // This determines what the event is, and performs the proper action which might include
                                                        // updating the display on the device

    HRESULT TimerMaintenance(void); // The timer thread calls this once every second.
                                    // This call will handle expired notifications, and auto-change the current page on the
                                    // device (defaults to every 5 seconds). This time will be set from the IHV property page in the CPL
                                    // Note, if the user presses Next, this resets the auto-change timer.

private:
    HRESULT DeleteAllNotificationsForAnApplication_Internal(const GUID guidApplicationId);
    HRESULT DeleteAllNotifications(void);
    HRESULT RenderProperPage(CRenderedData* pRenderedData,
                             bool fForceRender = false);
    HRESULT RenderAndSendDataToDevice(bool fForceRender = false);
    HRESULT AddApplication(__drv_aliasesMem CNodeApplication *pApplication);
    HRESULT AddNotification(__drv_aliasesMem CNodeNotification *pNotfication);

    // This method returns a valid application node index (one with wszBody != NULL),
    // starting with the current node and moving forward,
    // or it returns INVALID_APPLICATION_NODE_INDEX if there isn't one
    size_t ForwardFindValidApplicationNodeIndex(void);
    // This method returns a valid application node index (one with wszBody != NULL),
    // starting with the current node and moving in reverse,
    // or it returns INVALID_APPLICATION_NODE_INDEX if there isn't one
    size_t ReverseFindValidApplicationNodeIndex(void);

    NODENOTIFICATIONVECTOR m_pNotificationNodes;
    NODEAPPLICATIONVECTOR m_pApplicationNodes;
    CNodeDefaultBackground m_DefaultBackgroundNode;
    size_t m_indexCurrentApplicationPage; // Index of the current active application page on the device
                                          // Note this might be overridden by a notification page
                                          // If application vector is empty, then this value is irrelevant
    CNodeBase* m_pLastRenderedNode; // ID (pointer) of the last rendered node

    CRITICAL_SECTION m_CriticalSection;
};
