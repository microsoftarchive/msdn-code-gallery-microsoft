/*++

Copyright (c)  Microsoft Corporation

Module Name:

    kmdf_vdev.h

Abstract:

  Base header file for WDF hybrid 1394 virtual device driver.


--*/
#ifndef _KMDF_VDEV_H_
#define _KMDF_VDEV_H_

#include <wdm.h>
#include <wdf.h>

#pragma warning(disable:4214)  // bit field types other than int warning

#include <1394.h>

#pragma warning(default:4214)

#include "wdf_common.h"
#include "wdf_vdev_api.h"
#include "kmdf_vdev_debug.h"
#include "kmdf_vdev_sample.h"

#endif // #ifndef _KMDF_VDEV_H_
