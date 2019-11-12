/*
**  Microsoft Excel Developer's Toolkit
**  Version 12.0
**
**  File:           SRC\XLCALL.CPP
**  Description:    Code file for Excel 2007 callbacks
**  Platform:       Microsoft Windows
**
**  This file defines the entry points 
**  which are used in the Microsoft Excel 2007 C API.
**
*/

#ifndef _WINDOWS_
#include <windows.h>
#endif

#include "xlcall.h"

/*
** Excel 12 entry points backwards compatible with Excel 11
**
** Excel12 and Excel12v ensure backwards compatibility with Excel 11
** and earlier versions. These functions will return xlretFailed when
** used to callback into Excel 11 and earlier versions
*/

#define cxloper12Max 255
#define EXCEL12ENTRYPT "MdCallBack12"
#define XLCALL32DLL TEXT("XLCall32")
#define XLCALL32ENTRYPT "GetExcel12EntryPt"

typedef int (PASCAL *EXCEL12PROC) (int xlfn, int coper, LPXLOPER12 *rgpxloper12, LPXLOPER12 xloper12Res);
typedef EXCEL12PROC (PASCAL *PFNGETEXCEL12ENTRYPT) (void);

HMODULE hmodule;
EXCEL12PROC pexcel12;

__forceinline void FetchExcel12EntryPt(void)
{
	if (pexcel12 == NULL)
	{
		hmodule = GetModuleHandle(XLCALL32DLL);
		if (hmodule != NULL)
		{
			PFNGETEXCEL12ENTRYPT pfnGetExcel12EntryPt = (PFNGETEXCEL12ENTRYPT) GetProcAddress(hmodule, XLCALL32ENTRYPT);
			if (pfnGetExcel12EntryPt != NULL)
			{
				pexcel12 = (*pfnGetExcel12EntryPt)();
			}
		}
	}

	if (pexcel12 == NULL)
	{
		hmodule = GetModuleHandle(NULL);
		if (hmodule != NULL)
		{
			pexcel12 = (EXCEL12PROC) GetProcAddress(hmodule, EXCEL12ENTRYPT);
		}
	}
}

int _cdecl Excel12(int xlfn, LPXLOPER12 operRes, int count, ...)
{

	LPXLOPER12 rgxloper12[cxloper12Max];
	va_list ap;
	int ioper;
	int mdRet;

	FetchExcel12EntryPt();
	if (pexcel12 == NULL)
	{
		mdRet = xlretFailed;
	}
	else
	{
		mdRet = xlretInvCount;
		if ((count >= 0)  && (count <= cxloper12Max))
		{
			va_start(ap, count);
			for (ioper = 0; ioper < count ; ioper++)
			{
				rgxloper12[ioper] = va_arg(ap, LPXLOPER12);
			}
			va_end(ap);
			mdRet = (pexcel12)(xlfn, count, &rgxloper12[0], operRes);
		}
	}
	return(mdRet);
	
}

int pascal Excel12v(int xlfn, LPXLOPER12 operRes, int count, LPXLOPER12 opers[])
{

	int mdRet;

	FetchExcel12EntryPt();
	if (pexcel12 == NULL)
	{
		mdRet = xlretFailed;
	}
	else
	{
		mdRet = (pexcel12)(xlfn, count, &opers[0], operRes);
	}
	return(mdRet);

}
