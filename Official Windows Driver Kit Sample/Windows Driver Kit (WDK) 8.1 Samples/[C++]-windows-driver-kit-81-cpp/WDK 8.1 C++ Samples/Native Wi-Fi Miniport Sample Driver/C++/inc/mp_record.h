/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    mp_record.h

Abstract:
    Contains functions to do in memory recording of operations
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once

/**
 * \enum MP_DATA_RECORDER_DATA_TYPES
 * 
 * This enum contains the various type of data items that can be stored inside the 
 * data recorder object
 */
enum MP_DATA_RECORDER_DATA_TYPES {
   MP_NO_DATA = 0,     /**< Empty slot */
   MP_STRING           /**< Holds a (max MAX_RECORDED_STRING_LENGTH character) string*/
   };

/** The maximum lenght of a string stored inside the data recorder */
#define MAX_RECORDED_STRING_LENGTH  64
/** Default number of entries to save in the recoard */
#define DEFAULT_DATA_RECORDER_HISTORY_LENGTH    512

/**
 * \struct _MP_DATA_RECORD_ELEMENT
 * 
 * This is the actual element stored inside the data recorder
 */
typedef struct _MP_DATA_RECORD_ELEMENT
{
   /** The type of data present in the data item field */
   enum MP_DATA_RECORDER_DATA_TYPES    DataType;

   /** This union contains the actual data items */
   union _RecordedData
   {
      // Preferrably its always a string
      CHAR String[MAX_RECORDED_STRING_LENGTH];
   } Data;

} MP_DATA_RECORD_ELEMENT, PMP_DATA_RECORD_ELEMENT;

//
// The real data recorder structure
//
typedef struct _MP_DATA_RECORDER_OBJ
{
    MP_DATA_RECORD_ELEMENT*     RecordList;
    ULONG                       MaxLength;
    ULONG                       CurrentIndex;
}MP_DATA_RECORDER_OBJ, *PMP_DATA_RECORDER_OBJ;

NDIS_STATUS 
InitializeDataRecorder(
    NDIS_HANDLE             MiniportAdapterHandle,
    PMP_DATA_RECORDER_OBJ   Recorder, 
    ULONG                   MaxLength
    );

VOID 
TerminateDataRecorder (
    PMP_DATA_RECORDER_OBJ   Recorder
    );

NDIS_STATUS 
SaveToDataRecorder(
    PMP_DATA_RECORDER_OBJ       Recorder, 
    _Printf_format_string_ PSTR Format ,
    ...
    );


#ifdef MP_RECORD_ENABLED

//
// Use the real recorder object
//
typedef MP_DATA_RECORDER_OBJ   MP_DATA_RECORDER, *PMP_DATA_RECORDER;


// Handlers that outside callers should use
#define MP_OPEN_RECORDER(_Handle, _Recorder)         \
    InitializeDataRecorder(_Handle, &(_Recorder), DEFAULT_DATA_RECORDER_HISTORY_LENGTH)
    
#define MP_CLOSE_RECORDER(_Recorder)        \
    TerminateDataRecorder(&(_Recorder))

#define MP_RECORD_STRING_1(_Recorder, _Fmt, _Data1)   \
    SaveToDataRecorder(&(_Recorder), _Fmt, _Data1)

#define MP_RECORD_STRING_2(_Recorder, _Fmt, _Data1, _Data2)   \
    SaveToDataRecorder(&(_Recorder), _Fmt, _Data1, _Data2)

#define MP_RECORD_STRING_3(_Recorder, _Fmt, _Data1, _Data2, _Data3)   \
    SaveToDataRecorder(&(_Recorder), _Fmt, _Data1, _Data2, _Data3)


#else // MP_RECORD_ENABLED

//
// Dummy definitions
//
typedef PVOID   MP_DATA_RECORDER, *PMP_DATA_RECORDER;

#define MP_OPEN_RECORDER(_Handle, _Recorder)
#define MP_CLOSE_RECORDER(_Recorder)
#define MP_RECORD_STRING_1(_Recorder, _Fmt, _Data1)
#define MP_RECORD_STRING_2(_Recorder, _Fmt, _Data1, _Data2)
#define MP_RECORD_STRING_3(_Recorder, _Fmt, _Data1, _Data2, _Data3)

#endif
