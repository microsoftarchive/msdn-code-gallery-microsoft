/*++

Copyright (c) 1989-2001  Microsoft Corporation

Module Name:

    machash.c

Abstract:

    Implement MAC hash table

Author:

Revision History:

    09/10/2007      Created
--*/

#include "precomp.h"

PMAC_HASH_ENTRY
LookupMacHashTable(
    _In_ PCMAC_HASH_TABLE Table,
    _In_ const DOT11_MAC_ADDRESS * MacKey
    )
/*++

Routine Description:

    Look up hash table
    Note: spinlock should held

Arguments:
    Table: The MAC hash table, must not be NULL.
    MacKey: The MAC address
    
Return Value:
    The found entry
    NULL Not found

--*/
{
    const LIST_ENTRY * head;
    PLIST_ENTRY entry;
    ULONG hash = HASH_MAC(*MacKey);
    PMAC_HASH_ENTRY hashEntry = NULL;
    PMAC_HASH_ENTRY macEntry;

    head = &Table->Buckets[hash];
    entry = head->Flink;
    while(head != entry) {
        macEntry = CONTAINING_RECORD(entry, MAC_HASH_ENTRY, Linkage); 
        if (memcmp(macEntry->MacKey, *MacKey, sizeof(DOT11_MAC_ADDRESS)) == 0) {
            hashEntry = macEntry;
            break;
        }
        entry = entry->Flink;
    }

    return hashEntry;
}

VOID
EnumMacEntry(
    _In_ PMAC_HASH_TABLE Table,
    _In_ PENUM_MAC_ENTRY_CALLBACK CallbackFn,
    _In_ PVOID CallbackCtxt
    )
/*++

Routine Description:

    Enumerate hash table
    Note: spinlock should held

Arguments:
    Table: The MAC hash table, must not be NULL.
    CallbackFn: Pointer to the callback function, must not be NULL.
    CallbackCtxt: Pointer to the callback function context, will be passed to the callback function

Return Value:
    None
    
--*/
{
    int i;
    PLIST_ENTRY entry;
    PLIST_ENTRY head;
    PMAC_HASH_ENTRY macEntry;

    for(i = 0; i < MAC_HASH_BUCKET_NUMBER; i++) {
        head = Table->Buckets + i;
        entry = head->Flink;

        while(entry != head) {
            macEntry = CONTAINING_RECORD(entry, MAC_HASH_ENTRY, Linkage); 

            // Get the next entry before calling CallbackFn.
            //
            // CallbackFn may dereference macStateEntry and remove it from the
            // hash table!
            entry = entry->Flink;

            if (!CallbackFn(Table, macEntry, CallbackCtxt)) {
                return;
            }
        }
    }
}

