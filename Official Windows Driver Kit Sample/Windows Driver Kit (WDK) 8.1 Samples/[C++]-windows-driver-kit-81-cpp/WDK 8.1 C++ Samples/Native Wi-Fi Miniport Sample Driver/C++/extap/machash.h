/*++

Copyright (c) 1989-2001  Microsoft Corporation

Module Name:

    machash.h

Abstract:

    HASH Table for 802 MAC address.

Note:
    1. Caller must handle synchronization
    2. If duplicate is undesirable, caller must deal
       with it (e.g. look up the entry before inserting
       it)

Revision History:

    09/10/2007      Created
--*/

#pragma once
#ifndef _MACHASH_H
#define _MACHASH_H

#define MAC_HASH_BUCKET_NUMBER      256

/** MAC hash function */
#define HASH_MAC(Mac)               (((Mac)[5] ^ (Mac)[4]) % MAC_HASH_BUCKET_NUMBER)

/**
 * A hash table based on the MAC address of a station.
 * Stations with the same hash value are organized as a linked list.
 */
typedef struct _MAC_HASH_TABLE {
    /**
     * the table
     */
    LIST_ENTRY Buckets[MAC_HASH_BUCKET_NUMBER];

    /**
     * number of entries
     */
    ULONG EntryCount;
} MAC_HASH_TABLE, *PMAC_HASH_TABLE;

typedef const MAC_HASH_TABLE* PCMAC_HASH_TABLE;

/**
 * A hash table entry.
 */
typedef struct _MAC_HASH_ENTRY {
    LIST_ENTRY Linkage;

    /**
     * MAC address as the key of an entry
     */
    DOT11_MAC_ADDRESS MacKey;
} MAC_HASH_ENTRY, *PMAC_HASH_ENTRY;

typedef const MAC_HASH_ENTRY* PCMAC_HASH_ENTRY;

/**
 * Initialize a MAC hash table.
 */
VOID
FORCEINLINE
InitMacHashTable(
    _In_ PMAC_HASH_TABLE Table
    )
{
    int i;

    for(i = 0; i < MAC_HASH_BUCKET_NUMBER; i++) {
        InitializeListHead(&Table->Buckets[i]);
    }

    Table->EntryCount = 0;
}

/**
 * Initialize an MAC hash entry.
 */
VOID
FORCEINLINE
InitalizeMacHashEntry(
    _In_ PMAC_HASH_ENTRY Entry,
    _In_ const DOT11_MAC_ADDRESS * MacKey
    )
{
    RtlCopyMemory(
            Entry->MacKey,
            *MacKey,
            sizeof(DOT11_MAC_ADDRESS)
            );
    InitializeListHead(&Entry->Linkage);
}

/**
 * De-initialize a MAC hash table.
 */
VOID
FORCEINLINE
DeinitializeMacHashTable(
    _In_ PMAC_HASH_TABLE Table
    )
{
    int i;
    UNREFERENCED_PARAMETER(Table);

    for(i = 0; i < MAC_HASH_BUCKET_NUMBER; i++) {
        MPASSERT(IsListEmpty(&Table->Buckets[i]));
    }

    MPASSERT(0 == Table->EntryCount);
}

/**
 * Return the number of entries in the hash table
 */
ULONG
FORCEINLINE
GetMacHashTableEntryCount(
    _In_ PMAC_HASH_TABLE Table
    )
{
    return Table->EntryCount;
}

/**
 * Look up an entry in a MAC hash table using the MAC address as the key.
 */
PMAC_HASH_ENTRY
LookupMacHashTable(
    _In_ PCMAC_HASH_TABLE Table,
    _In_ const DOT11_MAC_ADDRESS * MacKey
    );

/**
 * Insert an entry into a MAC hash table.
 */
VOID
FORCEINLINE
InsertMacHashTable(
    _In_ PMAC_HASH_TABLE Table,
    _In_ PMAC_HASH_ENTRY Entry
    )
{
    ULONG hash = HASH_MAC(Entry->MacKey);

    InsertTailList(&Table->Buckets[hash], &Entry->Linkage);

    Table->EntryCount++;
}

/**
 * Remove an entry from a MAC hash table.
 */
VOID
FORCEINLINE
RemoveMacHashEntry(
    _In_ PMAC_HASH_TABLE Table,
    _In_ PMAC_HASH_ENTRY Entry
    )
{
    RemoveEntryList(&Entry->Linkage);

    Table->EntryCount--;
}

/**
 * Remove an entry with the matching MAC address from a MAC hash table.
 */
PMAC_HASH_ENTRY
FORCEINLINE
RemoveFromMacHashTable(
    _In_ PMAC_HASH_TABLE Table,
    _In_ const DOT11_MAC_ADDRESS * MacKey
    )
{
    PMAC_HASH_ENTRY entry;
    
    entry = LookupMacHashTable(Table, MacKey);
    if (entry) {
        RemoveMacHashEntry(Table, entry);
    }

    return entry;
}


/**
 * Callback function for APEnumMacEntry.
 *
 * The callback can return FALSE to terminate the
 * enumeration prematurely.
 */
typedef BOOLEAN
(*PENUM_MAC_ENTRY_CALLBACK)(
    _In_ PMAC_HASH_TABLE Table,
    _In_ PMAC_HASH_ENTRY MacEntry,
    _In_ PVOID CallbackCtxt
    );

/**
 * Enumerate a MAC hash table.
 */
VOID
EnumMacEntry(
    _In_ PMAC_HASH_TABLE Table,
    _In_ PENUM_MAC_ENTRY_CALLBACK CallbackFn,
    _In_ PVOID CallbackCtxt
    );

#endif  // _MACHASH_H
