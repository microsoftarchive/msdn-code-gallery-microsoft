#include "precomp.h"

#if DOT11_TRACE_ENABLED
#include "Hvl_context.tmh"
#endif

VOID
HvlReturnContext(
    _In_  PHVL            pHvl,
    _In_  PHVL_CONTEXT    pCtx
    );

VOID
HvlinitContext(
    _In_ PHVL_CONTEXT pCtx,
    _In_ BOOLEAN fInUse
    )
{
    pCtx->fCtxInUse = fInUse;
    InitializeListHead (&pCtx->Link);
    pCtx->ulNumVNics = 0;
    InitializeListHead (&pCtx->VNicList);
    NdisZeroMemory(&pCtx->CtxSig, sizeof(VNIC_SIGNATURE));
}

VOID
HvlCtxSetSignature(
    PHVL_CONTEXT pCtx,
    VNIC_SIGNATURE Sig
    )
{
    MpTrace(COMP_HVL, DBG_NORMAL, ("Set signature %d for context (%p)", Sig.ulPhyId, pCtx));

    pCtx->CtxSig = Sig;
}

VOID
HvlCtxUpdateSignature(
    PHVL_CONTEXT pCtx
    )
{
    LIST_ENTRY *pEntryVNic = NULL;
    PVNIC pVNic = NULL;
    VNIC_SIGNATURE mergedSig = {0}, vnicSig = {0};
    BOOLEAN fFirst = TRUE;

    ASSERT(!IsListEmpty(&pCtx->VNicList));

    // BUGBUG: Can this be optimized so that we do not always query the VNIC
    /*
        Go through the list of VNICs and merge their signatures
        */
    pEntryVNic = pCtx->VNicList.Flink;
    while (pEntryVNic != &pCtx->VNicList)
    {
        pVNic = CONTAINING_RECORD(pEntryVNic, VNIC, CtxLink);

        vnicSig = VNic11GetSignature(pVNic);

        MpTrace(COMP_HVL, DBG_NORMAL, ("Got signature %d for VNIC (%d) in context (%p)", vnicSig.ulPhyId, VNIC_PORT_NO, pCtx));

        if (fFirst)
        {
            mergedSig = vnicSig;
            fFirst = FALSE;
        }
        else
        {
            mergedSig = VNic11MergeSignatures(&vnicSig, &mergedSig);
        }
        
        pEntryVNic = pEntryVNic->Flink;
    }

    HvlCtxSetSignature(pCtx, mergedSig);
}

/*
    This function assumes that all the VNICs in the same context have compatible signatures that
    can be merged. This is guaranteed because
    a.  whenever the signature of a VNIC becomes incompatible, it will split itself into a separate 
       context
    b.  two contexts are merged only if their signatures are compatible
    */
VNIC_SIGNATURE
HvlCtxGetSignature(
    PHVL_CONTEXT pCtx
    )
{
    HvlCtxUpdateSignature(pCtx);
    return pCtx->CtxSig;
}

VOID
HvlAddVNicToCtx(
    PVNIC pVNic,
    PHVL_CONTEXT pCtx
    )
{
    ASSERT(pCtx->fCtxInUse);
    
    // move this VNIC to the destination context
    InsertTailList(&pCtx->VNicList, &pVNic->CtxLink);
    
    // increment the VNIC count in the destination context
    pCtx->ulNumVNics++;

    // also add the link to the context in the VNIC's structure
    pVNic->pHvlCtx = pCtx;
}

/*
    Removes a VNIC from the HVL context. if this is the last VNIC in the context, it returns the 
    context as well
    */
VOID
HvlRemoveVNicFromCtx(
    _In_  PHVL            pHvl,
    _In_  PVNIC           pVNicToRemove
    )
{
    PHVL_CONTEXT pCtx = NULL;
    LIST_ENTRY *pEntryVNic = NULL;
    PVNIC pVNic = NULL;
    BOOLEAN fFound = FALSE;

    ASSERT(pVNicToRemove);

    pCtx = pVNicToRemove->pHvlCtx;

    // go through the VNIC list in the context and try to find this VNIC
    pEntryVNic = pCtx->VNicList.Flink;
    while (!fFound && (pEntryVNic != &pCtx->VNicList))
    {
        pVNic = CONTAINING_RECORD(pEntryVNic, VNIC, CtxLink);
        if (pVNicToRemove == pVNic)
        {
            fFound = TRUE;
        }
        else
        {
            pEntryVNic = pEntryVNic->Flink;
        }
    }

    ASSERT(fFound);
    
    ASSERT(pVNic == pVNicToRemove);

    if (NULL == pVNic)
    {
        ASSERT(pVNic != NULL);
        return;
    }

    // Remove the VNIC from the context
    RemoveEntryList (&pVNic->CtxLink);
    InitializeListHead(&pVNic->CtxLink);
    pCtx->ulNumVNics--;
    
    // Reset the HVL context reference in the VNIC
    pVNic->pHvlCtx = NULL;

    // if this is the last VNIC in the context, return the context as well
    if (IsListEmpty(&pCtx->VNicList))
    {
        HvlReturnContext(pHvl, pCtx);
    }
    
    return;
}


/*
    This function assigns the VNIC to an unused HVL context
    */
VOID
HvlAssignVNicToContext(
    _In_  PHVL            pHvl,
    _In_  PVNIC           pVNic,
    _Out_ PHVL_CONTEXT   *ppCtx
    )
{
    PHVL_CONTEXT pCtx = NULL;
    BOOLEAN fFoundCtx = FALSE;
    ULONG ulCtxIndex = 0;

    ASSERT(ppCtx);
    ASSERT(HvlIsLocked(pHvl));
    
    do
    {
        *ppCtx = NULL;

        for (ulCtxIndex = 0; ulCtxIndex < HVL_NUM_CONTEXTS; ulCtxIndex++)
        {
            pCtx = &(pHvl->HvlContexts[ulCtxIndex]);
            if (pCtx->fCtxInUse == FALSE)
            {
                fFoundCtx = TRUE;
                break;
            }
        }

        ASSERT(fFoundCtx && ulCtxIndex < HVL_NUM_CONTEXTS);

        // setup this context
        HvlinitContext(pCtx, TRUE);

        // Add the VNIC to the VNIC list in the context
        HvlAddVNicToCtx(pVNic, pCtx);
        
        // set the context signature from the VNIC
        HvlCtxUpdateSignature(pCtx);

        pHvl->ulNumPortCtxs++;
        
        MpTrace(COMP_HVL, DBG_NORMAL, ("Associated VNIC (%d) to context (%p)", VNIC_PORT_NO, pCtx));
        
        *ppCtx = pCtx;
    } while (FALSE);
    
    return;
}

/*
    This function is called with the Hvl locked

    This function removes the passed in context from the HVL and removes any references to it
    e.g. as the active context or as the helper port context
    */
VOID
HvlRemoveCtxReferences(
    PHVL            pHvl,
    PHVL_CONTEXT    pCtxToRemove
    )
{
    LIST_ENTRY *pEntryCtx = NULL;
    PHVL_CONTEXT pCtx = NULL;
    BOOLEAN fFound = FALSE;

    ASSERT(HvlIsLocked(pHvl));
    
    ASSERT(pCtxToRemove);

    do
    {
        /*
            Is it the currently active context
            */
        if (pCtxToRemove == pHvl->pActiveContext)
        {
            pHvl->pActiveContext = NULL;
            break;
        }

        /*
            Is it the helper port context
            */
        if (pCtxToRemove == pHvl->pHelperPortCtx)
        {
            pHvl->pHelperPortCtx = NULL;
            break;
        }
        
        /*
            Traverse the inactive context list to check if pCtxToRemove is present there
            */

        pEntryCtx = pHvl->InactiveContextList.Flink;
        while (!fFound && (pEntryCtx != &pHvl->InactiveContextList))
        {
            pCtx = CONTAINING_RECORD(pEntryCtx, HVL_CONTEXT, Link);
            if (pCtxToRemove == pCtx)
            {
                fFound = TRUE;
                RemoveEntryList (&pCtx->Link);
                InitializeListHead(&pCtx->Link);
            }
            else
            {
                pEntryCtx = pEntryCtx->Flink;
            }
        }
    } while (FALSE);
    
    return;
}

/*
    Removes all the references to this context
    */
VOID
HvlReturnContext(
    _In_  PHVL            pHvl,
    _In_  PHVL_CONTEXT    pCtx
    )
{
    ASSERT(HvlIsLocked(pHvl));
    ASSERT(pCtx->fCtxInUse);
    ASSERT(IsListEmpty(&pCtx->VNicList) && pCtx->ulNumVNics == 0);

    // remove all the references to this context
    HvlRemoveCtxReferences(pHvl, pCtx);

    // set the context as not being used
    HvlinitContext(pCtx, FALSE);

    pHvl->ulNumPortCtxs--;
}

/*
    Merges pCtxToMerge with pDstCtx. pDstCtx has the merged context
    */
VOID
HvlMergeCtxs(
    PHVL pHvl,
    PHVL_CONTEXT pCtxToMerge,
    PHVL_CONTEXT pDstCtx
    )
{
    LIST_ENTRY *pEntryVNic = NULL;
    PVNIC pVNic = NULL;
    VNIC_SIGNATURE Ctx1Sig = HvlCtxGetSignature(pCtxToMerge);
    VNIC_SIGNATURE Ctx2Sig = HvlCtxGetSignature(pDstCtx);

    // this function should only be called if the signatures are compatible
    ASSERT(VNic11AreCompatibleSignatures(&Ctx1Sig, &Ctx2Sig));

    MpTrace(COMP_HVL, DBG_NORMAL, ("Merging context %p into context %p. ", pCtxToMerge, pDstCtx));

    /*
        Go through the list of VNICs and add them to the destination context
        */
    pEntryVNic = pCtxToMerge->VNicList.Flink;
    while (pEntryVNic != &pCtxToMerge->VNicList)
    {
        pVNic = CONTAINING_RECORD(pEntryVNic, VNIC, CtxLink);

        // remove the VNIC from this context
        HvlRemoveVNicFromCtx(pHvl, pVNic);

        // move this VNIC to the destination context
        HvlAddVNicToCtx(pVNic, pDstCtx);

        /*
            Make the entry point to the first VNIC in the list
            */
        pEntryVNic = pCtxToMerge->VNicList.Flink;
    }

    ASSERT(IsListEmpty(&pCtxToMerge->VNicList));

    // set the merged signature on the destination context
    HvlCtxSetSignature(pDstCtx, VNic11MergeSignatures(&Ctx1Sig, &Ctx2Sig));        
}

/*
    Our merge algorithm is pretty straightforward. 
    1. If the active context is not null and not the helper context, init the list of known contexts with
      the active context
    2. For all contexts in the inactive context list
        a. get the signature of the context
        b. For each context constructed so far
            if the signature of the two context are compatible
                merge the two the context
            else
                create a new context and add it to the list of constructed contexts        
    */
VOID
HvlPerformCtxMerge(
    PHVL pHvl,
    BOOLEAN *pfMerged
    )
{
    LIST_ENTRY *pEntryCtx = NULL;
    PHVL_CONTEXT pCtx = NULL;
    VNIC_SIGNATURE Ctx1Sig = {0}, Ctx2Sig = {0};
    BOOLEAN fMerge = FALSE;
    PHVL_CONTEXT HvlCtxArray[HVL_NUM_CONTEXTS] = {0};
    ULONG ulNumCtxs = 0, ulCtxIndex = 0, ulNumCtxsBeforeMerge = 0;

    ASSERT(HvlIsLocked(pHvl));

    *pfMerged = FALSE;
    
    if (pHvl->ulNumPortCtxs <= 2)
    {
        return;
    }

    ulNumCtxsBeforeMerge = pHvl->ulNumPortCtxs;
    
    /*
        1. If the active context is not null and not the helper context, init the list of known contexts with
          the active context
        */
    if (pHvl->pActiveContext && pHvl->pHelperPortCtx != pHvl->pActiveContext)
    {
        HvlCtxArray[ulNumCtxs++] = pHvl->pActiveContext;
    }

    /*
        2. For all contexts in the inactive context list
        */
    pEntryCtx = pHvl->InactiveContextList.Flink;
    while (pEntryCtx != &pHvl->InactiveContextList)
    {
        pCtx = CONTAINING_RECORD(pEntryCtx, HVL_CONTEXT, Link);

        // remove this context from the linked list
        RemoveEntryList (&pCtx->Link);
        InitializeListHead(&pCtx->Link);

        /*
            a. get the signature of the context
            */
        Ctx1Sig = HvlCtxGetSignature(pCtx);

        /*
            b. For each context constructed so far
            */
        fMerge = FALSE;
        for (ulCtxIndex = 0; ulCtxIndex < ulNumCtxs; ulCtxIndex++)
        {
            Ctx2Sig = HvlCtxGetSignature(HvlCtxArray[ulCtxIndex]);
            if (VNic11AreCompatibleSignatures(&Ctx1Sig, &Ctx2Sig))
            {
                fMerge = TRUE;
                break;
            }
        }

        /*
            if the signature of the two context are compatible
                merge the two the context
            else
                create a new context and add it to the list of constructed contexts        
            */
        if (fMerge)
        {
            HvlMergeCtxs(pHvl, pCtx, HvlCtxArray[ulCtxIndex]);
        }
        else
        {
            HvlCtxArray[ulNumCtxs++] = pCtx;
        }

        // move to the next context in the inactive context list
        pEntryCtx = pHvl->InactiveContextList.Flink;
    }

    /*
        We have walked the complete linked list of contexts
        */
    ASSERT(IsListEmpty(&pHvl->InactiveContextList));

    /*
        Now build up the Inactive context list again from the context array we have constructed
        */
    ulCtxIndex = 0;
    if (pHvl->pActiveContext == HvlCtxArray[0])
    {
        /*
            The first context is already referred by the active context. Skip it
            */
        ulCtxIndex++;
    }

    for ( ; ulCtxIndex < ulNumCtxs; ulCtxIndex++)
    {
        InsertTailList(&pHvl->InactiveContextList, &HvlCtxArray[ulCtxIndex]->Link);
    }

    ulNumCtxs = ulNumCtxs + 1; // +1 for the helper port context
    if (ulNumCtxsBeforeMerge != ulNumCtxs)
    {
        ASSERT(ulNumCtxs < ulNumCtxsBeforeMerge);
        ASSERT(pHvl->ulNumPortCtxs == ulNumCtxs); 
        *pfMerged = TRUE;
        MpTrace(COMP_HVL, DBG_NORMAL, ("Merged contexts. Previous # contexts = %d, new # contexts = %d", ulNumCtxsBeforeMerge, ulNumCtxs));
    }
    else
    {
        ASSERT(!fMerge);
    }
    
}

/*
    Split the VNIC into a separate context
    */
VOID
HvlPerformCtxSplit(
    PHVL pHvl,
    PVNIC pVNic
    )
{
    PHVL_CONTEXT pCtx = pVNic->pHvlCtx, pNewCtx = NULL;
    
    if (pCtx->ulNumVNics > 1)
    {
        HvlRemoveVNicFromCtx(pHvl, pVNic);

        HvlAssignVNicToContext(pHvl, pVNic, &pNewCtx);

        /*
            Make the new context part of the inactive context list. It will be picked up whenever 
            we next context switch to it
            */
        InsertTailList(&pHvl->InactiveContextList, &pNewCtx->Link);

        MpTrace(COMP_HVL, DBG_NORMAL, ("Splitted VNIC (%d, context = %p) into a separate context %p", VNIC_PORT_NO, pCtx, pNewCtx));
    }
    else
    {
        // there is nothing to be done
    }        
}

_IRQL_requires_(DISPATCH_LEVEL)
VOID
HvlNotifyAllVNicsInContext(
    PHVL            pHvl,
    PHVL_CONTEXT    pCtx,
    PVNIC_FUNCTION  pVnicFn,
    ULONG           ulFlags
    )
{
    LIST_ENTRY *pEntryVNic = NULL;
    PVNIC pVNic = NULL;

    _Analysis_assume_lock_held_(pHvl->Lock.SpinLock);

    HvlUnlock(pHvl);
    
    pEntryVNic = pCtx->VNicList.Flink;
    while (pEntryVNic != &pCtx->VNicList)
    {
        pVNic = CONTAINING_RECORD(pEntryVNic, VNIC, CtxLink);
        pVnicFn(pVNic, ulFlags);
        pEntryVNic = pEntryVNic->Flink;
    }
    HvlLock(pHvl);
}

// This function is called with the HVL locked
PHVL_CONTEXT
HvlFindNextCtx(PHVL pHvl)
{
    PHVL_CONTEXT pNextCtx = NULL;
    LIST_ENTRY *pEntryCtx = NULL, *pEntryVNic = NULL;
    PVNIC pNextVNic = NULL;
    
    ASSERT(HvlIsLocked(pHvl));

    if (IsListEmpty(&pHvl->InactiveContextList))
    {
        // This is the only context we have
        return pHvl->pActiveContext;
    }
    else
    {
        pEntryCtx = pHvl->InactiveContextList.Flink;
        while (pEntryCtx != &pHvl->InactiveContextList)
        {
            pNextCtx = CONTAINING_RECORD(pEntryCtx, HVL_CONTEXT, Link);

            pEntryVNic = pNextCtx->VNicList.Flink;
            while (pEntryVNic != &pNextCtx->VNicList)
            {
                pNextVNic = CONTAINING_RECORD(pEntryVNic, VNIC, CtxLink);
            
                if (VNic11IsOkToCtxS(pNextVNic))
                {
                    return pNextCtx;
                }

                pEntryVNic = pEntryVNic->Flink;

            }
            
            pEntryCtx = pEntryCtx->Flink;
        }
    }    
        
    return NULL;
}

// This function is called with the HVL locked
VOID
HvlUpdateActiveCtx(PHVL pHvl, PHVL_CONTEXT pCurrCtx, PHVL_CONTEXT pNextCtx)
{
    ASSERT(HvlIsLocked(pHvl));

    // remove the next context from the inactive context list
    RemoveEntryList (&pNextCtx->Link);
    InitializeListHead(&pNextCtx->Link);
    
    // change the active context
    pHvl->pActiveContext = pNextCtx;
    
    if (pCurrCtx && pCurrCtx != pHvl->pHelperPortCtx)
    {
        // add the currently active context to the inactive context list
        InsertTailList (&pHvl->InactiveContextList, &pCurrCtx->Link);
    }
}


