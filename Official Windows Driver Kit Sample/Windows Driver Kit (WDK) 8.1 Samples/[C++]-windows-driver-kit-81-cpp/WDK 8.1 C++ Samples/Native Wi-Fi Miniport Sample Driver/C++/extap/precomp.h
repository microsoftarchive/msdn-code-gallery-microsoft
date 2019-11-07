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

//
// For safe integer arithmetic 
//
#include <ntintsafe.h>

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

// 
// Debugging/tracing functions
//
#include "mp_trace.h"
#include "mp_dbg.h"
#include "glb_utils.h"

//
// All the interface functions
//
#include "mp_intf.h"
#include "port_intf.h"
#include "vnic_intf.h"

//
// Structures relevant for this layer
//
#include "machash.h"
#include "port_defs.h"
#include "base_port_intf.h"
#include "ap_config.h"
#include "ap_assocmgr.h"
#include "ap_defs.h"
#include "ap_intf.h"
#include "ap_action.h"
#include "ap_recv.h"
#include "ap_send.h"
#include "ap_misc.h"
