//
// Globally disabled warnings
// These are superfluous errors at warning level 4.
//
#pragma warning(disable:4214)   // bit field types other than int
#pragma warning(disable:4200)   // non-standard extension used
#pragma warning(disable:4201)   // nameless struct/union
#pragma warning(disable:4115)   // named type definition in parentheses
#pragma warning(disable:4127)   // conditional expression is constant
#pragma warning(disable:4054)   // cast of function pointer to PVOID
#pragma warning(disable:4206)   // translation unit is empty


//
// For making calls into NDIS
//
#include <ndis.h>
#include "ntintsafe.h"

//
// Various functions and data structures provided by WiFi
//
#include <windot11.h>
#include "80211hdr.h"

//
// Global structures
//
#include "glb_defs.h"
#include "ath_glb_defs.h"
#include "data_glb_defs.h"
#include "mp_rwlock.h"
#include "scan_glb_defs.h"

// 
// Debugging/tracing functions
//
#include "mp_trace.h"
#include "mp_dbg.h"

//
// All the interface functions
//
#include "mp_intf.h"
#include "hvl_intf.h"
#include "port_intf.h"
#include "hw_intf.h"
#include "st_intf.h"
#include "ap_intf.h"

//
// Structures relevant for this layer
//
#include "mp_defs.h"

