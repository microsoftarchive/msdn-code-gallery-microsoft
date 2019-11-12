#define WIN32_LEAN_AND_MEAN
#include <malloc.h>
#include <math.h>
#include <wchar.h>
#include <windows.h>
#include "xlcall.h"

#define MAX_STR_12 32767

void HelpRegister12(XCHAR* procedure, XCHAR* typeText, XCHAR* functionText, XCHAR* argumentText);

LPXLOPER TempErr(int i);
LPXLOPER12 TempErr12(int i);
LPXLOPER12 TempInt12(int i);
LPXLOPER12 TempMissing12();
LPXLOPER12 TempNum12(double d);
LPXLOPER12 TempOper12(LPXLOPER12 oper);
LPXLOPER12 TempStr12(XCHAR* str);

LPXLOPER WINAPI xlAddInManagerInfo(LPXLOPER pxAction);
LPXLOPER12 WINAPI xlAddInManagerInfo12(LPXLOPER12 pxAction);
int WINAPI xlAutoAdd(void);
int WINAPI xlAutoClose(void);
void WINAPI xlAutoFree(LPXLOPER pxFree);
void WINAPI xlAutoFree12(LPXLOPER12 pxFree);
LPXLOPER WINAPI xlAutoRegister(LPXLOPER pxName);
LPXLOPER12 WINAPI xlAutoRegister12(LPXLOPER12 pxName);
int WINAPI xlAutoOpen(void);
int WINAPI xlAutoRemove(void);
