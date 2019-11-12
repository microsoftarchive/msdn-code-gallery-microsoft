#include <windows.h>
#include <Winreg.h>
#include <dbt.h>

// ATL includes
#include <ATLBase.h>
#include <ATLCom.h>
#include <ATLColl.h>
#include <ATLStr.h>

// Sensor includes
#include <SensorsApi.h>
#include <sensors.h>
#include <Initguid.h>
#include <propkeydef.h>

// Device includes
#include <setupapi.h>
#include <Strsafe.h>
#include <devioctl.h>

// TODO: Set these to the unique Sensor ID for device
DEFINE_GUID(SENSOR_GUID, 0xa3921b4f, 0xd0d8, 0x4686, 0x94, 0xdb, 0x47, 0xf5, 0x9c, 0xbd, 0xe2, 0xc9);
#define SENSOR_GUID_STRING L"{A3921B4F-D0D8-4686-94DB-47F59CBDE2C9}"

// radio mamanager includes
#include "RadioMgr.h"
#include "SampleRM.h"
#include "internalInterfaces.h"
#include "SensorCommunication.h"
#include "SampleRadioInstance.h"
#include "SampleInstanceCollection.h"
#include "SensorManagerEvents.h"
#include "SampleRadioManager.h"

extern HINSTANCE g_hInstance;

// Device Interface GUID used for Radio Management communication
// TODO: Match this GUID to value used in driver
// {3BA2B796-B3F0-4225-9F8E-62A38E444208}
DEFINE_GUID(GUID_DEVINTERFACE_GPS_RADIO_MANAGEMENT,
    0x3ba2b796, 0xb3f0, 0x4225, 0x9f, 0x8e, 0x62, 0xa3, 0x8e, 0x44, 0x42, 0x8);

#define IOCTL_INDEX                         0x800
#define FILE_DEVICE_GPS_RADIO_MANAGEMENT    0x64900 // TODO: Match this value to one used in driver

#define IOCTL_GPS_RADIO_MANAGEMENT_GET_RADIO_STATE CTL_CODE(    \
    FILE_DEVICE_GPS_RADIO_MANAGEMENT,                           \
    IOCTL_INDEX,                                                \
    METHOD_BUFFERED,                                            \
    FILE_READ_ACCESS)

#define IOCTL_GPS_RADIO_MANAGEMENT_GET_PREVIOUS_RADIO_STATE CTL_CODE(    \
    FILE_DEVICE_GPS_RADIO_MANAGEMENT,                           \
    IOCTL_INDEX + 1,                                            \
    METHOD_BUFFERED,                                            \
    FILE_READ_ACCESS)

#define IOCTL_GPS_RADIO_MANAGEMENT_SET_RADIO_STATE CTL_CODE(    \
    FILE_DEVICE_GPS_RADIO_MANAGEMENT,                           \
    IOCTL_INDEX + 2,                                            \
    METHOD_BUFFERED,                                            \
    FILE_WRITE_ACCESS)

#define IOCTL_GPS_RADIO_MANAGEMENT_SET_PREVIOUS_RADIO_STATE CTL_CODE(    \
    FILE_DEVICE_GPS_RADIO_MANAGEMENT,                           \
    IOCTL_INDEX + 3,                                            \
    METHOD_BUFFERED,                                            \
    FILE_WRITE_ACCESS)
