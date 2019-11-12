/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    St_aplst.h

Abstract:
    STA layer BSS list functions
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    08-01-2005    Created
    01-15-2006    Rename sta_aplist.h to st_aplst.h

Notes:

--*/
#ifndef _STATION_AP_LIST_H_
#define _STATION_AP_LIST_H_

#define STA_IS_BASIC_RATE(rate)     ((rate) & 0x80)
#define STA_DATA_RATE(rate)         ((rate) & 0x7f)

/**
 * Determines the list of access points that can be used for association.
 * This uses the list of discovered access points along with parameters set
 * be the operating system to determine list of candidate access point. The
 * function populates the station's connection context structure. The
 * access points are sorted by priority with the first access point in 
 * the list as best association candidate
 * 
 * \param pStation  Station structure
 * \param bStrictFiltering     True if we want to be stricter filter in discarding APs. This
 *                  is FALSE when we are not connected and want to connect and is TRUE
 *                  if we are roaming when connected
 * \return NDIS_STATUS depending on whether or not it was able to obtain
 * list of candidate access points
 * \sa StaRankCandidateAPList
 */
NDIS_STATUS
StaGetCandidateAPList(
    _In_  PMP_EXTSTA_PORT                        pStation,
    _In_  BOOLEAN                         bStrictFiltering
    );


/**
 * Ranks the candidate access points list in the connection context.
 * It will rank and order the access points in the list such that the first
 * entry in the list is the most preferred access point for association. 
 *
 * \note APs which are not picked for use must have their refcounts decremented
 * 
 * \param pStation  Station structure contain the connection context
 * \return NDIS_STATUS
 * \sa StaGetCandidateAPLists
 */
NDIS_STATUS
StaRankCandidateAPList(
    _In_  PMP_EXTSTA_PORT                        pStation
    );

/**
 * Determines if the specified AP can be used for association based on
 * the current configuration set by the operating system.
 *
 * \note The BSSEntry must be locked
 * 
 * \param pBSSEntry  BSSEntry to be used for matching
 * \param pStation  Station structure
 * \return TRUE if the AP can be used for association. FALSE otherwise
 */
BOOLEAN
StaMatchBSSEntry(
    _In_  PMP_BSS_ENTRY                  pBSSEntry,
    _In_  PMP_EXTSTA_PORT                        pStation
    );

/**
 * Determines if the specified PhyId is in our desired PHY list.
 * 
 * \param pStation  Station structure
 * \param PhyId     the PhyId to check.
 * \return TRUE if the PhyId is in our desired PHY list. FALSE otherwise
 */
BOOLEAN
StaMatchPhyId(
    _In_  PMP_EXTSTA_PORT    pStation,
    _In_  ULONG       PhyId
    );


NDIS_STATUS
StaEnumerateAssociationInfoInfra(
    _In_  PMP_EXTSTA_PORT        pStation,
    _Out_ PDOT11_ASSOCIATION_INFO_LIST   pAssocInfoList,
    _In_  ULONG           TotalLength
    );

VOID
StaCheckForProtectionInERP(
    _In_  PMP_EXTSTA_PORT                        pStation,
    _In_reads_bytes_(infoBlobLength)  PUCHAR          pInfoBlob,
    _In_  ULONG           infoBlobLength
    );

NDIS_STATUS 
StaProcessBeaconForConfigInfo(
    _In_  PMP_EXTSTA_PORT                        pStation,
    _In_  PMP_RX_MPDU                    pFragment,
    _In_reads_bytes_(TotalLength)  PUCHAR                          pInfoBlob,
    _In_  ULONG                           TotalLength
    );

BOOLEAN
StaHasAPConfigurationChanged(
    _In_  PMP_EXTSTA_PORT                        pStation
    );

#endif
