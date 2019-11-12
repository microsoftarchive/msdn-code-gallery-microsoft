/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    mp_record.c

Abstract:
    Implements helper routines useful in-memory recording of operations
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#include "precomp.h"
#include "mp_record.h"

#include <stdio.h>
#include <stdarg.h>
#include <ntstrsafe.h>

#if DOT11_TRACE_ENABLED
#include "mp_record.tmh"
#endif

/**
* Initializes the DataRecorder for storing data
* 
* \param MaxLength   The maximum number of elements to store at a time. Adding
*          more than this number is possible, it only causes wraparound, overwriting
*          earlier data
* \return NDIS_STATUS
* \sa Terminate(), Insert()
*/
NDIS_STATUS InitializeDataRecorder(
    NDIS_HANDLE             MiniportAdapterHandle,
    PMP_DATA_RECORDER_OBJ   Recorder, 
    ULONG                   MaxLength
    )
{
    ULONG                   count;
    
    Recorder->RecordList = NULL;
    Recorder->MaxLength = MaxLength;
    Recorder->CurrentIndex = 0;

    MP_ALLOCATE_MEMORY(MiniportAdapterHandle, 
        &Recorder->RecordList, 
        sizeof(MP_DATA_RECORD_ELEMENT) * MaxLength, 
        MP_MEMORY_TAG
        );
    if (Recorder->RecordList == NULL)
    {
        MpTrace (COMP_INIT_PNP, DBG_SERIOUS, ("Unable to allocate memory for data recorder\n"));
        return NDIS_STATUS_RESOURCES;
    }

    for (count = 0; count < MaxLength; count++)
    {
        // Initialize as empty
        Recorder->RecordList[count].DataType = MP_NO_DATA;
    }
    
    return NDIS_STATUS_SUCCESS;
}

/**
* Cleansup the DataRecorder
* 
* \sa Initialize()
*/
VOID TerminateDataRecorder (PMP_DATA_RECORDER_OBJ Recorder)
{
    Recorder->MaxLength = 0;
    Recorder->CurrentIndex = 0;
 
    if (Recorder->RecordList)
    {
        MP_FREE_MEMORY(Recorder->RecordList);
        Recorder->RecordList = NULL;
    }
}

/**
* Adds a string to the data recorder.
* 
* \return NDIS_STATUS
*/
NDIS_STATUS SaveToDataRecorder(PMP_DATA_RECORDER_OBJ Recorder, _Printf_format_string_ PSTR Format, ...)
{
    ULONG                       myIndex;
    va_list                     args;

    if (Recorder->RecordList)
    {
        myIndex = NdisInterlockedIncrement((PLONG)&(Recorder->CurrentIndex)) % Recorder->MaxLength;
        Recorder->RecordList[myIndex].DataType = MP_STRING;

        // Format the message        
        va_start(args, Format);
        if (NT_SUCCESS(RtlStringCchVPrintfA(
                            Recorder->RecordList[myIndex].Data.String, 
                            MAX_RECORDED_STRING_LENGTH, 
                            Format, 
                            args))
            )
        {
            Recorder->RecordList[myIndex].Data.String[MAX_RECORDED_STRING_LENGTH-1] = 0;
        }
        va_end (args);       
    }
    
    return NDIS_STATUS_SUCCESS;
}

