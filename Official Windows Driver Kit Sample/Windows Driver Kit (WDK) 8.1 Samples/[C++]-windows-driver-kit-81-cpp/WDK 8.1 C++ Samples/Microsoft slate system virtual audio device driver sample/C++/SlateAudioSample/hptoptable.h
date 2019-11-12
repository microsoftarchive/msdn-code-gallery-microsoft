/*++

Copyright (c) Microsoft Corporation All Rights Reserved

Module Name:

    hptopotable.h

Abstract:

    Common declaration of topology table for the headphone.

--*/

#ifndef _SYSVAD_HPTOPTABLE_H_
#define _SYSVAD_HPTOPTABLE_H_


//=============================================================================
static
KSJACK_DESCRIPTION HpJackDesc =
{
    KSAUDIO_SPEAKER_MONO,
    JACKDESC_RGB(179, 201, 140),
    eConnTypeCombination,
    eGeoLocRear,
    eGenLocPrimaryBox,
    ePortConnJack,
    TRUE
};

#endif // _SYSVAD_HPTOPTABLE_H_
