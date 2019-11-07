// Machine generated IDispatch wrapper class(es) created with Add Class from Typelib Wizard

#import "ATLExeCOMServer.exe" no_namespace
// CATLExeSimpleObjectWrapper wrapper class

class CATLExeSimpleObjectWrapper : public COleDispatchDriver
{
public:
	CATLExeSimpleObjectWrapper(){} // Calls COleDispatchDriver default constructor
	CATLExeSimpleObjectWrapper(LPDISPATCH pDispatch) : COleDispatchDriver(pDispatch) {}
	CATLExeSimpleObjectWrapper(const CATLExeSimpleObjectWrapper& dispatchSrc) : COleDispatchDriver(dispatchSrc) {}

	// Attributes
public:

	// Operations
public:


	// ISimpleObject methods
public:
	float get_FloatProperty()
	{
		float result;
		InvokeHelper(0x1, DISPATCH_PROPERTYGET, VT_R4, (void*)&result, NULL);
		return result;
	}
	void put_FloatProperty(float newValue)
	{
		static BYTE parms[] = VTS_R4 ;
		InvokeHelper(0x1, DISPATCH_PROPERTYPUT, VT_EMPTY, NULL, parms, newValue);
	}
	CString HelloWorld()
	{
		CString result;
		InvokeHelper(0x2, DISPATCH_METHOD, VT_BSTR, (void*)&result, NULL);
		return result;
	}
	void GetProcessThreadID(long * pdwProcessId, long * pdwThreadId)
	{
		static BYTE parms[] = VTS_PI4 VTS_PI4 ;
		InvokeHelper(0x3, DISPATCH_METHOD, VT_EMPTY, NULL, parms, pdwProcessId, pdwThreadId);
	}

	// ISimpleObject properties
public:

};
