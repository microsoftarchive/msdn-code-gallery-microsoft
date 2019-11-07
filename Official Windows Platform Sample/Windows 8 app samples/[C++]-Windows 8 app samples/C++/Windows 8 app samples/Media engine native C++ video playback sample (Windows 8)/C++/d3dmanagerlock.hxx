//+-------------------------------------------------------------------------
//
//  Member:     d3dManagerLock.hxx
//
//  Synopsis:   Contains a class to aid aquiring a D3D9 or DXGI device from a 
//              device manager
//
//--------------------------------------------------------------------------

//+-----------------------------------------------------------------------------
//
//  Class:      CAutoDXDevLock
//
//  Synopsis:   Class to handle extracting a locked DX device from a 
//              a D3D9 or DXGI Device manager
//
//------------------------------------------------------------------------------
template<class DevManager, class DevInterface>
class CAutoDXDevLockBase
{
protected:
    Microsoft::WRL::ComPtr<DevManager> m_spDevManager;
    HANDLE m_hDev;
    bool m_locked;

public:           
    //+-------------------------------------------------------------------------
    //
    //  Member:     CAutoDXDevLockBase
    //
    //  Synopsis:   Constructor
    //
    //--------------------------------------------------------------------------
    CAutoDXDevLockBase(
        _In_ Microsoft::WRL::ComPtr<DevManager> spDevManager
        ) :
        m_locked(false),
        m_hDev(NULL),
        m_spDevManager(spDevManager)
    {
        m_spDevManager->OpenDeviceHandle(&m_hDev);
    }

    //+-------------------------------------------------------------------------
    //
    //  Member:     ~CAutoDXDevLockBase
    //
    //  Synopsis:   destructor
    //
    //--------------------------------------------------------------------------
    ~CAutoDXDevLockBase()
    {
        if (m_hDev != NULL)
        {
            if (m_locked)
            {
                m_spDevManager->UnlockDevice(m_hDev, FALSE);
            }
            m_spDevManager->CloseDeviceHandle(m_hDev);
        }
    }

    //+-------------------------------------------------------------------------
    //
    //  Member:     LockDevice
    //
    //  Synopsis:   
    //
    //--------------------------------------------------------------------------
    virtual HRESULT LockDevice(
        _Out_ Microsoft::WRL::ComPtr<DevInterface>& spDev
        ) = 0;
};

class CAutoDXGILock : public CAutoDXDevLockBase<IMFDXGIDeviceManager, ID3D11Device>
{

public:
    CAutoDXGILock(_In_ Microsoft::WRL::ComPtr<IMFDXGIDeviceManager> spDevManager) :
        CAutoDXDevLockBase(spDevManager)
    {}

    HRESULT LockDevice(
        _Out_ Microsoft::WRL::ComPtr<ID3D11Device>& spDev
        )
    {
		HRESULT hr = S_OK;

        if (m_locked)
        {            
            hr = E_FAIL;
        }

        MEDIA::ThrowIfFailed(
			m_spDevManager->LockDevice(
			m_hDev,
			__uuidof(ID3D11Device),
			&spDev,
			TRUE)
			);

        m_locked = true;

        return hr;
    }
};                                       
