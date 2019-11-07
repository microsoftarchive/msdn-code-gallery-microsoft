//
// SwapAPOGFX.cpp -- Copyright (c) Microsoft Corporation. All rights reserved.
//
// Description:
//
//  Implementation of CSwapAPOGFX
//

#include <atlbase.h>
#include <atlcom.h>
#include <atlcoll.h>
#include <atlsync.h>
#include <mmreg.h>

#include <audioenginebaseapo.h>
#include <baseaudioprocessingobject.h>
#include <resource.h>

#include <float.h>

#include "SwapAPO.h"
#include <initguid.h>
#include <CustomPropKeys.h>


// Static declaration of the APO_REG_PROPERTIES structure
// associated with this APO.  The number in <> brackets is the
// number of IIDs supported by this APO.  If more than one, then additional
// IIDs are added at the end
#pragma warning (disable : 4815)
const AVRT_DATA CRegAPOProperties<1> CSwapAPOGFX::sm_RegProperties(
    __uuidof(SwapAPOGFX),                           // clsid of this APO
    L"CSwapAPOGFX",                                 // friendly name of this APO
    L"Copyright (c) Microsoft Corporation",         // copyright info
    1,                                              // major version #
    0,                                              // minor version #
    __uuidof(ISwapAPOGFX)                           // iid of primary interface
//
// If you need to change any of these attributes, uncomment everything up to
// the point that you need to change something.  If you need to add IIDs, uncomment
// everything and add additional IIDs at the end.
//
//   Enable inplace processing for this APO.
    , (APO_FLAG)( APO_FLAG_SAMPLESPERFRAME_MUST_MATCH | APO_FLAG_FRAMESPERSECOND_MUST_MATCH |
                  APO_FLAG_BITSPERSAMPLE_MUST_MATCH | APO_FLAG_INPLACE)
//  , DEFAULT_APOREG_MININPUTCONNECTIONS
//  , DEFAULT_APOREG_MAXINPUTCONNECTIONS
//  , DEFAULT_APOREG_MINOUTPUTCONNECTIONS
//  , DEFAULT_APOREG_MAXOUTPUTCONNECTIONS
//  , DEFAULT_APOREG_MAXINSTANCES
//
    );

#pragma AVRT_CODE_BEGIN
//-------------------------------------------------------------------------
// Description:
//
//  Do the actual processing of data.
//
// Parameters:
//
//      u32NumInputConnections      - [in] number of input connections
//      ppInputConnections          - [in] pointer to list of input APO_CONNECTION_PROPERTY pointers
//      u32NumOutputConnections      - [in] number of output connections
//      ppOutputConnections         - [in] pointer to list of output APO_CONNECTION_PROPERTY pointers
//
// Return values:
//
//      void
//
// Remarks:
//
//  This function processes data in a manner dependent on the implementing
//  object.  This routine can not fail and can not block, or call any other
//  routine that blocks, or touch pagable memory.
//
STDMETHODIMP_(void) CSwapAPOGFX::APOProcess(
    UINT32 u32NumInputConnections,
    APO_CONNECTION_PROPERTY** ppInputConnections,
    UINT32 u32NumOutputConnections,
    APO_CONNECTION_PROPERTY** ppOutputConnections)
{
    UNREFERENCED_PARAMETER(u32NumInputConnections);
    UNREFERENCED_PARAMETER(u32NumOutputConnections);

    FLOAT32 *pf32InputFrames, *pf32OutputFrames;

    ATLASSERT(m_bIsLocked);

    // assert that the number of input and output connectins fits our registration properties
    ATLASSERT(m_pRegProperties->u32MinInputConnections <= u32NumInputConnections);
    ATLASSERT(m_pRegProperties->u32MaxInputConnections >= u32NumInputConnections);
    ATLASSERT(m_pRegProperties->u32MinOutputConnections <= u32NumOutputConnections);
    ATLASSERT(m_pRegProperties->u32MaxOutputConnections >= u32NumOutputConnections);

    // check APO_BUFFER_FLAGS.
    switch( ppInputConnections[0]->u32BufferFlags )
    {
        case BUFFER_INVALID:
        {
            ATLASSERT(false);  // invalid flag - should never occur.  don't do anything.
            break;
        }
        case BUFFER_VALID:
        {
            // get input pointer to connection buffer
            pf32InputFrames = reinterpret_cast<FLOAT32*>(ppInputConnections[0]->pBuffer);
            ATLASSERT( IS_VALID_TYPED_READ_POINTER(pf32InputFrames) );

            // get output pointer to connection buffer
            pf32OutputFrames = reinterpret_cast<FLOAT32*>(ppOutputConnections[0]->pBuffer);
            ATLASSERT( IS_VALID_TYPED_READ_POINTER(pf32OutputFrames) );

            // Swap only if we have more than one channel.
            if (m_fEnableSwapGFX && (1 < m_u32SamplesPerFrame))
            {
                ProcessSwapScale(pf32OutputFrames, pf32InputFrames,
                            ppInputConnections[0]->u32ValidFrameCount,
                            m_u32SamplesPerFrame, m_pf32Coefficients );
            }
            else
            {
                // copy the memory only if there is an output connection, and input/output pointers are unequal
                if ( (0 != u32NumOutputConnections) &&
                      (ppOutputConnections[0]->pBuffer != ppInputConnections[0]->pBuffer) )
                {
                    CopyMemory(pf32OutputFrames, pf32InputFrames,
                                ppInputConnections[0]->u32ValidFrameCount *
                                GetBytesPerSampleContainer() * GetSamplesPerFrame());
                }
            }

            // Set the valid frame count.
            ppOutputConnections[0]->u32ValidFrameCount = ppInputConnections[0]->u32ValidFrameCount;

            // pass along buffer flags
            ppOutputConnections[0]->u32BufferFlags = ppInputConnections[0]->u32BufferFlags;

            break;
        }
        case BUFFER_SILENT:
        {
            // Set valid frame count.
            ppOutputConnections[0]->u32ValidFrameCount = ppInputConnections[0]->u32ValidFrameCount;

            // pass along buffer flags
            ppOutputConnections[0]->u32BufferFlags = ppInputConnections[0]->u32BufferFlags;

            break;
        }
        default:
        {
            ATLASSERT(false);  // invalid flag - should never occur
            break;
        }
    } // switch

} // APOProcess
#pragma AVRT_CODE_END


// The method that this long comment refers to is "Initialize()"
//-------------------------------------------------------------------------
// Description:
//
//  Generic initialization routine for APOs.
//
// Parameters:
//
//     cbDataSize - [in] the size in bytes of the initialization data.
//     pbyData - [in] initialization data specific to this APO
//
// Return values:
//
//     S_OK                         Successful completion.
//     E_POINTER                    Invalid pointer passed to this function.
//     E_INVALIDARG                 Invalid argument
//     AEERR_ALREADY_INITIALIZED    APO is already initialized
//
// Remarks:
//
//  This method initializes the APO.  The data is variable length and
//  should have the form of:
//
//    struct MyAPOInitializationData
//    {
//        APOInitBaseStruct APOInit;
//        ... // add additional fields here
//    };
//
//  If the APO needs no initialization or needs no data to initialize
//  itself, it is valid to pass NULL as the pbyData parameter and 0 as
//  the cbDataSize parameter.
//
//  As part of designing an APO, decide which parameters should be
//  immutable (set once during initialization) and which mutable (changeable
//  during the lifetime of the APO instance).  Immutable parameters must
//  only be specifiable in the Initialize call; mutable parameters must be
//  settable via methods on whichever parameter control interface(s) your
//  APO provides. Mutable values should either be set in the initialize
//  method (if they are required for proper operation of the APO prior to
//  LockForProcess) or default to reasonable values upon initialize and not
//  be required to be set before LockForProcess.
//
//  Within the mutable parameters, you must also decide which can be changed
//  while the APO is locked for processing and which cannot.
//
//  All parameters should be considered immutable as a first choice, unless
//  there is a specific scenario which requires them to be mutable; similarly,
//  no mutable parameters should be changeable while the APO is locked, unless
//  a specific scenario requires them to be.  Following this guideline will
//  simplify the APO's state diagram and implementation and prevent certain
//  types of bug.
//
//  If a parameter changes the APOs latency or MaxXXXFrames values, it must be
//  immutable.
//
//  The default version of this function uses no initialization data, but does verify
//  the passed parameters and set the m_bIsInitialized member to true.
//
//  Note: This method may not be called from a real-time processing thread.
//

HRESULT CSwapAPOGFX::Initialize(UINT32 cbDataSize, BYTE* pbyData)
{
    HRESULT         hr = S_OK;
    PROPVARIANT     var;

    IF_TRUE_ACTION_JUMP( ((NULL == pbyData) && (0 != cbDataSize)), hr = E_INVALIDARG, Exit);
    IF_TRUE_ACTION_JUMP( ((NULL != pbyData) && (0 == cbDataSize)), hr = E_POINTER, Exit);
    IF_TRUE_ACTION_JUMP( (cbDataSize != sizeof(APOInitSystemEffects) ), hr = E_INVALIDARG, Exit);

    APOInitSystemEffects* papoSysFxInit = (APOInitSystemEffects*)pbyData;

    //
    //  Store locally for later reference
    //
    m_spAPOSystemEffectsProperties = papoSysFxInit->pAPOSystemEffectsProperties;

    //
    //  Get the current value
    //
    PropVariantInit(&var);

    if (m_spAPOSystemEffectsProperties != NULL)
    {
        // Get the state of whether channel swap GFX is enabled or not
        hr = m_spAPOSystemEffectsProperties->GetValue(PKEY_Endpoint_Enable_Channel_Swap_GFX, &var);

        if (SUCCEEDED(hr) && (var.vt == VT_UI4))
        {
            if (var.ulVal == 0L)
            {
                m_fEnableSwapGFX = FALSE;
            }
            else
            {
                m_fEnableSwapGFX = TRUE;
            }
        }
        else
        {
            PropVariantClear(&var);
        }
    }

    //
    //  Register for notification of registry updates
    //
    hr = m_spEnumerator.CoCreateInstance(__uuidof(MMDeviceEnumerator));
    IF_FAILED_JUMP(hr, Exit);

    hr = m_spEnumerator->RegisterEndpointNotificationCallback(this);
    IF_FAILED_JUMP(hr, Exit);

    m_bIsInitialized = true;
Exit:
    return hr;
}

//-------------------------------------------------------------------------
// Description:
//
//  Implementation of IMMNotificationClient::OnPropertyValueChanged
//
// Parameters:
//
//      pwstrDeviceId - [in] the id of the device whose property has changed
//      key - [in] the property that changed
//
// Return values:
//
//      Ignored by caller
//
// Remarks:
//
//      This method is called asynchronously.  No UI work should be done here.
//
HRESULT CSwapAPOGFX::OnPropertyValueChanged(LPCWSTR pwstrDeviceId, const PROPERTYKEY key)
{
    HRESULT     hr = S_OK;
    PROPVARIANT var;

    UNREFERENCED_PARAMETER(pwstrDeviceId);

    if (!m_spAPOSystemEffectsProperties)
    {
        return hr;
    }

    if (PK_EQUAL(key, PKEY_Endpoint_Enable_Channel_Swap_GFX))
    {
        PropVariantInit(&var);

        // Get the state of whether channel swap GFX is enabled or not
        hr = m_spAPOSystemEffectsProperties->GetValue(PKEY_Endpoint_Enable_Channel_Swap_GFX, &var);

        if (SUCCEEDED(hr) && (var.vt == VT_UI4))
        {
            InterlockedExchange(&m_fEnableSwapGFX, var.ulVal);
        }
        else
        {
            PropVariantClear(&var);
        }
    }

    return hr;
}

//-------------------------------------------------------------------------
// Description:
//
//  Destructor.
//
// Parameters:
//
//     void
//
// Return values:
//
//      void
//
// Remarks:
//
//      This method deletes whatever was allocated.
//
//      This method may not be called from a real-time processing thread.
//
CSwapAPOGFX::~CSwapAPOGFX(void)
{
    if (m_bIsInitialized)
    {
        //
        // unregister for callbacks
        //
        if (m_spEnumerator != NULL)
        {
            m_spEnumerator->UnregisterEndpointNotificationCallback(this);
        }
    }
    // Free locked memory allocations
    if (NULL != m_pf32Coefficients)
    {
        AERT_Free(m_pf32Coefficients);
        m_pf32Coefficients = NULL;
    }
} // ~CSwapAPOGFX


//-------------------------------------------------------------------------
// Description:
//
//  Validates input/output format pair during LockForProcess.
//
// Parameters:
//
//      u32NumInputConnections - [in] number of input connections attached to this APO
//      ppInputConnections - [in] format of each input connection attached to this APO
//      u32NumOutputConnections - [in] number of output connections attached to this APO
//      ppOutputConnections - [in] format of each output connection attached to this APO
//
// Return values:
//
//      S_OK                                Connections are valid.
//
// See Also:
//
//  CBaseAudioProcessingObject::LockForProcess
//
// Remarks:
//
//  This method is an internal call that is called by the default implementation of
//  CBaseAudioProcessingObject::LockForProcess().  This is called after the connections
//  are validated for simple conformance to the APO's registration properties.  It may be
//  used to verify that the APO is initialized properly and that the connections that are passed
//  agree with the data used for initialization.  Any failure code passed back from this
//  function will get returned by LockForProcess, and cause it to fail.
//
//  By default, this routine just ASSERTS and returns S_OK.
//
HRESULT CSwapAPOGFX::ValidateAndCacheConnectionInfo(UINT32 u32NumInputConnections,
                APO_CONNECTION_DESCRIPTOR** ppInputConnections,
                UINT32 u32NumOutputConnections,
                APO_CONNECTION_DESCRIPTOR** ppOutputConnections)
{
    ASSERT_NONREALTIME();
    HRESULT hResult;
    CComPtr<IAudioMediaType> pFormat;
    UNCOMPRESSEDAUDIOFORMAT UncompInputFormat, UncompOutputFormat;
    FLOAT32 f32InverseChannelCount;

    UNREFERENCED_PARAMETER(u32NumInputConnections);
    UNREFERENCED_PARAMETER(u32NumOutputConnections);

    _ASSERTE(!m_bIsLocked);
    _ASSERTE(((0 == u32NumInputConnections) || (NULL != ppInputConnections)) &&
              ((0 == u32NumOutputConnections) || (NULL != ppOutputConnections)));

    EnterCriticalSection(&m_CritSec);

    // get the uncompressed formats and channel masks
    hResult = ppInputConnections[0]->pFormat->GetUncompressedAudioFormat(&UncompInputFormat);
    IF_FAILED_JUMP(hResult, Exit);
    
    hResult = ppOutputConnections[0]->pFormat->GetUncompressedAudioFormat(&UncompOutputFormat);
    IF_FAILED_JUMP(hResult, Exit);

    // Since we haven't overridden the IsIn{Out}putFormatSupported APIs in this example, this APO should
    // always have input channel count == output channel count.  The sampling rates should also be eqaul,
    // and formats 32-bit float.
    _ASSERTE(UncompOutputFormat.fFramesPerSecond == UncompInputFormat.fFramesPerSecond);
    _ASSERTE(UncompOutputFormat. dwSamplesPerFrame == UncompInputFormat.dwSamplesPerFrame);

    // Allocate some locked memory.  We will use these as scaling coefficients during APOProcess->ProcessSwapScale
    hResult = AERT_Allocate(sizeof(FLOAT32)*m_u32SamplesPerFrame, (void**)&m_pf32Coefficients);
    IF_FAILED_JUMP(hResult, Exit);

    // Set scalars to decrease volume from 1.0 to 1.0/N where N is the number of channels
    // starting with the first channel.
    f32InverseChannelCount = 1.0f/m_u32SamplesPerFrame;
    for (UINT16 u16Index=0; u16Index<m_u32SamplesPerFrame; u16Index++)
    {
        m_pf32Coefficients[u16Index] = 1.0f - (FLOAT32)(f32InverseChannelCount)*u16Index;
    }

    
Exit:
    LeaveCriticalSection(&m_CritSec);
    return hResult;}

// ----------------------------------------------------------------------
// ----------------------------------------------------------------------
// IAudioSystemEffectsCustomFormats implementation

//
// For demonstration purposes we will add 44.1KHz, 16-bit stereo and 48KHz, 16-bit
// stereo formats.  These formats should already be available in mmsys.cpl.  We
// embellish the labels to make it obvious that these formats are coming from
// the APO.
//

struct CUSTOM_FORMAT_ITEM
{
    WAVEFORMATEXTENSIBLE wfxFmt;
    LPCWSTR              pwszRep;
};

#define STATIC_KSDATAFORMAT_SUBTYPE_AC3\
    DEFINE_WAVEFORMATEX_GUID(WAVE_FORMAT_DOLBY_AC3_SPDIF)
DEFINE_GUIDSTRUCT("00000092-0000-0010-8000-00aa00389b71", KSDATAFORMAT_SUBTYPE_AC3);
#define KSDATAFORMAT_SUBTYPE_AC3 DEFINE_GUIDNAMED(KSDATAFORMAT_SUBTYPE_AC3)
 
CUSTOM_FORMAT_ITEM _rgCustomFormats[] =
{
    {{ WAVE_FORMAT_EXTENSIBLE, 2, 44100, 176400, 4, 16, sizeof(WAVEFORMATEXTENSIBLE)-sizeof(WAVEFORMATEX), 16, KSAUDIO_SPEAKER_STEREO, KSDATAFORMAT_SUBTYPE_PCM},  L"Custom #1 (really 44.1 KHz, 16-bit, stereo)"},
    {{ WAVE_FORMAT_EXTENSIBLE, 2, 48000, 192000, 4, 16, sizeof(WAVEFORMATEXTENSIBLE)-sizeof(WAVEFORMATEX), 16, KSAUDIO_SPEAKER_STEREO, KSDATAFORMAT_SUBTYPE_PCM},  L"Custom #2 (really 48 KHz, 16-bit, stereo)"},
    {{ WAVE_FORMAT_EXTENSIBLE, 2, 48000, 192000, 4, 16, sizeof(WAVEFORMATEXTENSIBLE)-sizeof(WAVEFORMATEX), 16, KSAUDIO_SPEAKER_STEREO, KSDATAFORMAT_SUBTYPE_AC3},  L"Custom #3 (really 48 KHz AC-3)"}
};

#define _cCustomFormats (ARRAYSIZE(_rgCustomFormats))

//-------------------------------------------------------------------------
// Description:
//
//  Implementation of IAudioSystemEffectsCustomFormats::GetFormatCount
//
// Parameters:
//
//      pcFormats - [out] receives the number of formats to be added
//
// Return values:
//
//      S_OK        Success
//      E_POINTER   Null pointer passed
//
// Remarks:
//
STDMETHODIMP CSwapAPOGFX::GetFormatCount
(
    UINT* pcFormats
)
{
    if (pcFormats == NULL)
        return E_POINTER;

    *pcFormats = _cCustomFormats;
    return S_OK;
}

//-------------------------------------------------------------------------
// Description:
//
//  Implementation of IAudioSystemEffectsCustomFormats::GetFormat
//
// Parameters:
//
//      nFormat - [in] which format is being requested
//      IAudioMediaType - [in] address of a variable that will receive a ptr 
//                             to a new IAudioMediaType object
//
// Return values:
//
//      S_OK            Success
//      E_INVALIDARG    nFormat is out of range
//      E_POINTER       Null pointer passed
//
// Remarks:
//
STDMETHODIMP CSwapAPOGFX::GetFormat
(
    UINT              nFormat, 
    IAudioMediaType** ppFormat
)
{
    HRESULT hr;

    IF_TRUE_ACTION_JUMP((nFormat >= _cCustomFormats), hr = E_INVALIDARG, Exit);
    IF_TRUE_ACTION_JUMP((ppFormat == NULL), hr = E_POINTER, Exit);

    *ppFormat = NULL; 

    hr = CreateAudioMediaType(  (const WAVEFORMATEX*)&_rgCustomFormats[nFormat].wfxFmt, 
                                sizeof(_rgCustomFormats[nFormat].wfxFmt),
                                ppFormat);

Exit:
    return hr;
}

//-------------------------------------------------------------------------
// Description:
//
//  Implementation of IAudioSystemEffectsCustomFormats::GetFormatRepresentation
//
// Parameters:
//
//      nFormat - [in] which format is being requested
//      ppwstrFormatRep - [in] address of a variable that will receive a ptr 
//                             to a new string description of the requested format
//
// Return values:
//
//      S_OK            Success
//      E_INVALIDARG    nFormat is out of range
//      E_POINTER       Null pointer passed
//
// Remarks:
//
STDMETHODIMP CSwapAPOGFX::GetFormatRepresentation
(
    UINT                nFormat,
    _Outptr_ LPWSTR* ppwstrFormatRep
)
{
    HRESULT hr;
    size_t  cbRep;
    LPWSTR  pwstrLocal;

    IF_TRUE_ACTION_JUMP((nFormat >= _cCustomFormats), hr = E_INVALIDARG, Exit);
    IF_TRUE_ACTION_JUMP((ppwstrFormatRep == NULL), hr = E_POINTER, Exit);

    cbRep = (wcslen(_rgCustomFormats[nFormat].pwszRep) + 1) * sizeof(WCHAR);

    pwstrLocal = (LPWSTR)CoTaskMemAlloc(cbRep);
    IF_TRUE_ACTION_JUMP((pwstrLocal == NULL), hr = E_OUTOFMEMORY, Exit);

    hr = StringCbCopyW(pwstrLocal, cbRep, _rgCustomFormats[nFormat].pwszRep);
    if (FAILED(hr))
    {
        CoTaskMemFree(pwstrLocal);
    }
    else
    {
        *ppwstrFormatRep = pwstrLocal;
    }

Exit:
    return hr;
}
