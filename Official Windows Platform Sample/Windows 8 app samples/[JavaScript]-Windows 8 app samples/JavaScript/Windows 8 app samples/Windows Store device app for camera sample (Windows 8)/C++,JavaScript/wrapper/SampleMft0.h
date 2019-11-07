

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.00.0591 */
/* @@MIDL_FILE_HEADING(  ) */

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 500
#endif

/* verify that the <rpcsal.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCSAL_H_VERSION__
#define __REQUIRED_RPCSAL_H_VERSION__ 100
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __samplemft0_h__
#define __samplemft0_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IMft0_FWD_DEFINED__
#define __IMft0_FWD_DEFINED__
typedef interface IMft0 IMft0;

#endif 	/* __IMft0_FWD_DEFINED__ */


#ifndef __Mft0_FWD_DEFINED__
#define __Mft0_FWD_DEFINED__

#ifdef __cplusplus
typedef class Mft0 Mft0;
#else
typedef struct Mft0 Mft0;
#endif /* __cplusplus */

#endif 	/* __Mft0_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "Inspectable.h"
#include "mftransform.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __IMft0_INTERFACE_DEFINED__
#define __IMft0_INTERFACE_DEFINED__

/* interface IMft0 */
/* [unique][nonextensible][oleautomation][uuid][object] */ 


EXTERN_C const IID IID_IMft0;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("F5208B72-A37A-457E-A309-AE3060780E21")
    IMft0 : public IUnknown
    {
    public:
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE UpdateDsp( 
            /* [in] */ UINT32 uiPercentOfScreen) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE Enable( void) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE Disable( void) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetDspSetting( 
            /* [out] */ __RPC__out UINT *puiPercentOfScreen,
            /* [out] */ __RPC__out BOOL *pIsEnabled) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IMft0Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            __RPC__in IMft0 * This,
            /* [in] */ __RPC__in REFIID riid,
            /* [annotation][iid_is][out] */ 
            __RPC__deref_out _Result_nullonfailure_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            __RPC__in IMft0 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            __RPC__in IMft0 * This);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *UpdateDsp )( 
            __RPC__in IMft0 * This,
            /* [in] */ UINT32 uiPercentOfScreen);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *Enable )( 
            __RPC__in IMft0 * This);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *Disable )( 
            __RPC__in IMft0 * This);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetDspSetting )( 
            __RPC__in IMft0 * This,
            /* [out] */ __RPC__out UINT *puiPercentOfScreen,
            /* [out] */ __RPC__out BOOL *pIsEnabled);
        
        END_INTERFACE
    } IMft0Vtbl;

    interface IMft0
    {
        CONST_VTBL struct IMft0Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IMft0_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IMft0_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IMft0_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IMft0_UpdateDsp(This,uiPercentOfScreen)	\
    ( (This)->lpVtbl -> UpdateDsp(This,uiPercentOfScreen) ) 

#define IMft0_Enable(This)	\
    ( (This)->lpVtbl -> Enable(This) ) 

#define IMft0_Disable(This)	\
    ( (This)->lpVtbl -> Disable(This) ) 

#define IMft0_GetDspSetting(This,puiPercentOfScreen,pIsEnabled)	\
    ( (This)->lpVtbl -> GetDspSetting(This,puiPercentOfScreen,pIsEnabled) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IMft0_INTERFACE_DEFINED__ */



#ifndef __SampleMft0Lib_LIBRARY_DEFINED__
#define __SampleMft0Lib_LIBRARY_DEFINED__

/* library SampleMft0Lib */
/* [version][uuid] */ 


EXTERN_C const IID LIBID_SampleMft0Lib;

EXTERN_C const CLSID CLSID_Mft0;

#ifdef __cplusplus

class DECLSPEC_UUID("7BB640D9-33A4-4759-B290-F41A31DCF848")
Mft0;
#endif
#endif /* __SampleMft0Lib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


