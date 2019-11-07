/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    Reg5933.h  - AMCC S5933 Register definitions

    Refer to the AMCC ref manual "PCI Products Data Book S5920/S5933" 
    for details on these register definitions.
    
Environment:

    Kernel mode

--*/

#ifndef __REG5933_H_
#define __REG5933_H_

//-----------------------------------------------------------------------------   
// Define the Interrupt Control/Status Register (INTCSR)
//-----------------------------------------------------------------------------   

typedef struct _INTCSR_REG {
    
    // Interrupt Selection
    unsigned int  OutMailBoxByteIntSel       : 2;  // bit 0-1
    unsigned int  OutMailBoxIntSel           : 2;  // bit 2-3
    unsigned int  EnableOutMailBoxInt        : 1;  // bit 4
    unsigned int  reserved1                  : 3;  // bit 5-7
    unsigned int  InMailBoxByteIntSel        : 2;  // bit 8-9
    unsigned int  InMailBoxIntSel            : 2;  // bit 10-11
    unsigned int  EnableInMailBoxInt         : 1;  // bit 12
    unsigned int  reserved2                  : 1;  // bit 13
    unsigned int  IntOnWriteTransferComplete : 1;  // bit 14
    unsigned int  IntOnReadTransferComplete  : 1;  // bit 15
                                             
    // Actual Interrupt                      
    unsigned int  OutMailBoxInt              : 1;  // bit 16
    unsigned int  InMailBoxInt               : 1;  // bit 17
    unsigned int  WriteTransferComplete      : 1;  // bit 18 
    unsigned int  ReadTransferComplete       : 1;  // bit 19     
    unsigned int  MasterAbort                : 1;  // bit 20
    unsigned int  TargetAbort                : 1;  // bit 21
    unsigned int  reserved3                  : 1;  // bit 22
    unsigned int  InterruptAsserted          : 1;  // bit 23
    unsigned int  FifoAndEndianControl       : 8;  // bit 24-31
                                             
} INTCSR_REG, * PINTCSR_REG;

//-----------------------------------------------------------------------------   
// Define the Master Control/Status Register (MCSR)
//-----------------------------------------------------------------------------   

typedef struct _MCSR_REG {
    
    // Status
    unsigned int  FifoStatus             : 6;  // bit 0-5
    unsigned int  AddOnToPciZeroCount    : 1;  // bit 6
    unsigned int  PciToAddOnZeroCount    : 1;  // bit 7

    // Control
    unsigned int  WriteVsReadPriority    : 1;  // bit 8
    unsigned int  WriteFifoMgmtScheme    : 1;  // bit 9
    unsigned int  WriteTransferEnable    : 1;  // bit 10
    unsigned int  reserved1              : 1;  // bit 11
    unsigned int  ReadVsWritePriority    : 1;  // bit 12
    unsigned int  ReadFifoMgmtScheme     : 1;  // bit 13
    unsigned int  ReadTransferEnable     : 1;  // bit 14
    unsigned int  EnableMemReadMultiple  : 1;  // bit 15
    unsigned int  NonVolMemAddr          : 8;  // bit 16-23
    unsigned int  AddOnPinReset          : 1;  // bit 24
    unsigned int  PciToAddOnFifoReset    : 1;  // bit 25
    unsigned int  AddOnToPciFifoReset    : 1;  // bit 26
    unsigned int  MailBoxReset           : 1;  // bit 27
    unsigned int  FifoLoopbackMode       : 1;  // bit 28
    unsigned int  NVRAMAccessControl     : 3;  // bit 29-31

} MCSR_REG, * PMCSR_REG;

//-----------------------------------------------------------------------------   
// REG5933 structure (abbreviations are AMCC's)
//-----------------------------------------------------------------------------   
typedef struct _REG5933 {

    unsigned int   OMB1   ;  // 0x00 
    unsigned int   OMB2   ;  // 0x04 
    unsigned int   OMB3   ;  // 0x08 
    unsigned int   OMB4   ;  // 0x0C 
    unsigned int   IMB1   ;  // 0x10 
    unsigned int   IMB2   ;  // 0x14 
    unsigned int   IMB3   ;  // 0x18 
    unsigned int   IMB4   ;  // 0x1C 
    unsigned int   FIFO   ;  // 0x20 
    unsigned int   MWAR   ;  // 0x24 
    unsigned int   MWTC   ;  // 0x28 
    unsigned int   MRAR   ;  // 0x2C 
    unsigned int   MRTC   ;  // 0x30 
    unsigned int   MBEF   ;  // 0x34 
    INTCSR_REG     INTCSR ;  // 0x38 
    MCSR_REG       MCSR   ;  // 0x3C 

} REG5933, * PREG5933; 
                                            
#endif  // __REG5933_H_
