//-----------------------------------------------------------------------
// <copyright file="Node.h" company="Microsoft">
//      Copyright (c) 2006 Microsoft Corporation. All rights reserved.
// </copyright>
//
// Module:
//      Node.h
//
// Description:
//      Node Class
//
//-----------------------------------------------------------------------


#include <windows.h>
#include "Renderer.h"
#include <vector>


using namespace std;


class CNodeBase
{
public:
    enum NodeType
    {
        NodeTypeUndefined,
        NodeTypeApplication,
        NodeTypeNotification,
        NodeTypeDefaultBackground
    };

    CNodeBase(void){}

    ~CNodeBase(void){}

    HRESULT DeepDeleteNode(void); // Deep Deletes all the pointers for a node

    HRESULT ValidateNode(void); // Validates this node. Returns S_OK or E_INVALIDARG or E_POINTER (based on first found error).

    NodeType m_nodeType;
    GUID m_guidApplicationId;
    wchar_t* m_wszTitle; // Caller must new and delete it (DeepDeleteNode is a helper method)
    size_t m_SizeOfTitle;
    wchar_t* m_wszBody; // Caller must new and delete it (DeepDeleteNode is a helper method)
                        // If for application data, this can be NULL, meaning the AppId is valid, but there is
                        // no application data for it. If so, this page isn't considered for display.
    size_t m_SizeOfBody;
    HICON* m_pIcon; // Caller must new and delete it (DeepDeleteNode is a helper method)
                    // Can be NULL - show default icon
    size_t m_SizeOfIcon;

    bool m_fNeedToUpdateDevice; // Flag to determine whether the device needs to be updated
                                // This flag gets set to true whenever any item in the node
                                // is updated. When the renderer renders the page, this flag
                                // gets set to false.
};


class CNodeApplication : public CNodeBase
{
public:
    CNodeApplication(GUID guidApplicationId,
                     _In_opt_ __drv_aliasesMem wchar_t* wszTitle,
                     size_t SizeOfTitle,
                     __drv_aliasesMem HICON* pIcon,
                     size_t SizeOfIcon);
    ~CNodeApplication(void){}

    CRendererApplication m_Renderer;
};


class CNodeNotification : public CNodeBase
{
public:
    CNodeNotification(GUID guidApplicationId,
                      unsigned long ulNotificationId,
                      _In_reads_(SizeOfTitle) __drv_aliasesMem wchar_t* wszTitle,
                      size_t SizeOfTitle,
                      _In_reads_(SizeOfBody) __drv_aliasesMem wchar_t* wszBody,
                      size_t SizeOfBody,
                      HICON* pIcon,
                      size_t SizeOfIcon,
                      FILETIME notificationExpirationTime);
    ~CNodeNotification(void){}

    unsigned long m_ulNotificationId;
    FILETIME m_notificationExpirationTime;

    CRendererNotification m_Renderer;
};


class CNodeDefaultBackground : public CNodeBase
{
public:
    CNodeDefaultBackground(_In_reads_(SizeOfTitle) __drv_aliasesMem wchar_t* wszTitle,
                           size_t SizeOfTitle,
                           _In_reads_(SizeOfBody) __drv_aliasesMem wchar_t* wszBody,
                           size_t SizeOfBody,
                           __drv_aliasesMem HICON* pIcon,
                           size_t SizeOfIcon);
    ~CNodeDefaultBackground(void){}

    CRendererDefaultBackground m_Renderer;
};


typedef vector<CNodeApplication*> NODEAPPLICATIONVECTOR;
typedef vector<CNodeNotification*> NODENOTIFICATIONVECTOR;
