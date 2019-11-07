#include "testingXlls.h"

void HelpRegister12(XCHAR* procedure, XCHAR* typeText, XCHAR* functionText, XCHAR* argumentText)
{
	//Create XLOPER12's
	XLOPER12 xResult;
	XLOPER12 xModuleText;
	Excel12(xlGetName, &xModuleText, 0);
	LPXLOPER12 pxProcedure = TempStr12(procedure);
	LPXLOPER12 pxTypeText = TempStr12(typeText);
	LPXLOPER12 pxFunctionText = TempStr12(functionText);
	LPXLOPER12 pxArgumentText = TempStr12(argumentText);

	//Register the function
	Excel12(xlfRegister, &xResult, 5, &xModuleText, pxProcedure, pxTypeText, pxFunctionText, pxArgumentText);

	//Clean up the XLOPER12's
	Excel12(xlFree, &xResult, 1, &xModuleText);

	xlAutoFree12(pxProcedure);
	xlAutoFree12(pxTypeText);
	xlAutoFree12(pxFunctionText);
	xlAutoFree12(pxArgumentText);
}

LPXLOPER TempErr(int i)
{
	LPXLOPER oper = new XLOPER;
	if (oper == NULL)
		return NULL;

	oper->xltype = xltypeErr | xlbitDLLFree;
	oper->val.err = i;
	return oper;	
}

LPXLOPER12 TempErr12(int i)
{
	LPXLOPER12 oper = new XLOPER12;
	if (oper == NULL)
		return NULL;

	oper->xltype = xltypeErr | xlbitDLLFree;
	oper->val.err = i;
	return oper;	
}

LPXLOPER12 TempInt12(int i)
{
	LPXLOPER12 oper = new XLOPER12;
	if (oper == NULL)
		return NULL;

	oper->xltype = xltypeInt | xlbitDLLFree;
	oper->val.w = i;
	return oper;
}

LPXLOPER12 TempMissing12()
{
	LPXLOPER12 oper = new XLOPER12;
	if (oper == NULL)
		return NULL;

	oper->xltype = xltypeMissing | xlbitDLLFree;
	return oper;
}

LPXLOPER12 TempNum12(double d)
{
	LPXLOPER12 oper = new XLOPER12;
	if (oper == NULL)
		return NULL;

	oper->xltype = xltypeNum | xlbitDLLFree;
	oper->val.num = d;
	return oper;
}
LPXLOPER12 TempOper12(LPXLOPER12 oper)
{
	LPXLOPER12 newOper = NULL;

	// String case - deep-copy the string.
	// Note: This function does not cover many other OPER types that require a deep copy,
	// e.g. xltypeMulti, or xltypeBigData with cbData > 0.
	if (oper->xltype & xltypeStr && oper->xltype != xltypeBigData)
	{
		newOper = TempStr12(L"");
		if (newOper == NULL)
			return NULL;

		wcscpy_s(newOper->val.str, MAX_STR_12 + 2, oper->val.str);
	}

	// Shallow copy.
	// Covers simple types, + xltypeBigData with handle (cbData == 0).
	else
	{
		newOper = new XLOPER12;
		if (newOper == NULL)
			return NULL;

		*newOper = *oper;
		newOper->xltype &= (~xlbitXLFree);
		newOper->xltype |= xlbitDLLFree;
	}
	return newOper;
}
LPXLOPER12 TempStr12(XCHAR* str)
{
	LPXLOPER12 oper = new XLOPER12;
	if (oper == NULL)
		return NULL;

	oper->xltype = xltypeStr | xlbitDLLFree;
	oper->val.str = new XCHAR[MAX_STR_12+2];
	if (oper->val.str == NULL)
	{
		delete oper;
		return NULL;
	}

	wcscpy_s(oper->val.str + 1, MAX_STR_12+1, str);
	oper->val.str[0] = (XCHAR) wcsnlen(oper->val.str + 1, MAX_STR_12);
	return oper;
}

LPXLOPER WINAPI xlAddInManagerInfo(LPXLOPER pxAction)
{
	return TempErr(xlerrValue);
}

LPXLOPER12 WINAPI xlAddInManagerInfo12(LPXLOPER12 pxAction)
{
	if (((pxAction->xltype & xltypeInt) && (pxAction->val.w == 1)) || 
		((pxAction->xltype & xltypeNum) && (pxAction->val.num == 1)))
	{
		return TempStr12(L"HPC Testing UDFs");
	}
	else
	{
		return TempErr12(xlerrValue);
	}
}

int WINAPI xlAutoAdd(void)
{
	return 1;
}

int WINAPI xlAutoClose(void)
{
	return 1;
}

void WINAPI xlAutoFree(LPXLOPER pxFree)
{
	//free the sub-parts properly
	switch (pxFree->xltype & (~xlbitDLLFree))
	{
		case xltypeStr:
			if (pxFree->val.str)
			{
				delete pxFree->val.str;
				pxFree->val.str = NULL;
			}
			break;
		case xltypeMulti:
			if (pxFree->val.array.lparray)
			{
				//delete the child elements
				int r;
				int c;
				for (r = 0; r < pxFree->val.array.rows; r++)
				{
					for (c = 0; c < pxFree->val.array.columns; c++)
					{
						xlAutoFree(&(pxFree->val.array.lparray[r * pxFree->val.array.columns + c]));
					}
				}
				delete pxFree->val.array.lparray;
				pxFree->val.array.lparray = NULL;
			}
			break;
	}

	//free the base oper if it's ours to free
	if (pxFree->xltype & xlbitDLLFree)
	{
		delete pxFree;
	}
}

void WINAPI xlAutoFree12(LPXLOPER12 pxFree)
{
	//free the sub-parts properly
	switch (pxFree->xltype & (~xlbitDLLFree))
	{
		case xltypeStr:
			if (pxFree->val.str)
			{
				delete pxFree->val.str;
				pxFree->val.str = NULL;
			}
			break;
		case xltypeMulti:
			if (pxFree->val.array.lparray)
			{
				//delete the child elements
				int r;
				int c;
				for (r = 0; r < pxFree->val.array.rows; r++)
				{
					for (c = 0; c < pxFree->val.array.columns; c++)
					{
						xlAutoFree12(&(pxFree->val.array.lparray[r * pxFree->val.array.columns + c]));
					}
				}
				delete pxFree->val.array.lparray;
				pxFree->val.array.lparray = NULL;
			}
			break;
	}

	//free the base oper if it's ours to free
	if (pxFree->xltype & xlbitDLLFree)
	{
		delete pxFree;
	}
}

int WINAPI xlAutoOpen(void)
{
	//Register XllEcho functions
	HelpRegister12(L"XllEcho", L"QQ", L"XllEcho", L"Anything");
	HelpRegister12(L"XllEchoA", L">QX", L"XllEchoA", L"Anything");

	return 1;
}

LPXLOPER WINAPI xlAutoRegister(LPXLOPER pxName)
{
	return NULL;
}

LPXLOPER12 WINAPI xlAutoRegister12(LPXLOPER12 pxName)
{
	return NULL;
}

int WINAPI xlAutoRemove(void)
{
	return 1;
}

void AsyncStubFailHelper(LPXLOPER12 asyncHandle)
{
	//an error handler with no heap/thread operations.  
	//used to send back #VALUE! when an async stub fails
	XLOPER12 operResult;

	XLOPER12 operErr;
	operErr.xltype = xltypeErr;
	operErr.val.err = xlerrValue;
	
	Excel12(xlAsyncReturn, &operResult, 2, asyncHandle, &operErr);
}

LPXLOPER12 WINAPI XllEcho(LPXLOPER12 oper)
{
	// Simulate a long operation
	Sleep(1000);

	if (oper->xltype & xltypeNum)
		oper->val.num *= 2;
	return oper;
}

DWORD WINAPI XllEchoSetReturn(LPVOID args)
{
	LPXLOPER12* opers = (LPXLOPER12*)args;
	XLOPER12 xlResult;

	// Simulate waiting for a long external operation.
	Sleep(1000);
	if (opers[0]->xltype & xltypeNum)
		opers[0]->val.num *= 2;

	int retval = Excel12(xlAsyncReturn, &xlResult, 2, opers[1], opers[0]);

	// Free the passed pointer array
	// (Excel itself calls xlAutoFree12 to free the XLOPERs, since they have xlbitDLLFree).  
	delete opers;

	ExitThread(0);
	return 0;
}

void WINAPI XllEchoA(LPXLOPER12 oper, LPXLOPER12 asyncHandle)
{
	// point to the arguments from a pointer array that will be freed by XllEchoSetReturn
	LPXLOPER12* argsArray = new LPXLOPER12[2];
	if (argsArray == NULL)
	{
		AsyncStubFailHelper(asyncHandle);
		return;
	}

	argsArray[0] = TempOper12(oper);
	if (argsArray[0] == NULL)
	{
		delete argsArray;
		AsyncStubFailHelper(asyncHandle);
	}

	argsArray[1] = TempOper12(asyncHandle);
	if (argsArray[1] == NULL)
	{
		xlAutoFree12(argsArray[0]);
		delete argsArray;
		AsyncStubFailHelper(asyncHandle);
	}
	
	// Simulate an external async operation - start a thread and return.
	if (CreateThread(NULL, 0, XllEchoSetReturn, argsArray, 0, NULL) == NULL)
	{
		xlAutoFree12(argsArray[1]);
		xlAutoFree12(argsArray[0]);
		delete argsArray;
		AsyncStubFailHelper(asyncHandle);
	}
}

