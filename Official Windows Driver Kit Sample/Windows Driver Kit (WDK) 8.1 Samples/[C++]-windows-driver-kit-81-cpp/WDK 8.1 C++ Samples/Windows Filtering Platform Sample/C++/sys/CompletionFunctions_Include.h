///////////////////////////////////////////////////////////////////////////////
//
//   Copyright (c) 2012 Microsoft Corporation.  All Rights Reserved.
//
//   Module Name:
//      CompletionFunctions_Include.h
//
//   Abstract:
//      This module contains a central repository of headers which contain prototypes
//      for all of the completion functions.
//
//   Author:
//      Dusty Harper      (DHarper)
//
//   Revision History:
//
//      [ Month ][Day] [Year] - [Revision]-[ Comments ]
//      May       01,   2010  -     1.0   -  Creation
//
///////////////////////////////////////////////////////////////////////////////

#ifndef COMPLETION_INCLUDE_H
#define COMPLETION_INCLUDE_H

#include "CompletionFunctions_BasicPacketInjectionCallouts.h"    /// .
#include "CompletionFunctions_BasicPacketModificationCallouts.h" /// .
#include "CompletionFunctions_BasicStreamInjectionCallouts.h"    /// .
#include "CompletionFunctions_FastPacketInjectionCallouts.h"     /// .
#include "CompletionFunctions_FastStreamInjectionCallouts.h"     /// .
#include "CompletionFunctions_PendAuthorizationCallouts.h"       /// .
#include "CompletionFunctions_ProxyCallouts.h"                   /// .
#include "CompletionFunctions_TransparentProxyCallouts.h"        /// .

#endif /// COMPLETION_INCLUDE_H