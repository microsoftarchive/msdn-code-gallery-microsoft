

#pragma once

BEGIN_NAMESPACE

struct DiaFrameHolder
{
	DiaFrameHolder(IDiaStackFrame* pStackFrame)
	{
		assert(pStackFrame != NULL);

		pStackFrame->get_returnAddress(&ReturnAddress);
		
		// TODO: any other registers to grab?
		ULONGLONG val;
		pStackFrame->get_registerValue(CV_REG_EAX, &val);
		FrameContext.Eax = (DWORD)val;

		pStackFrame->get_registerValue(CV_REG_ECX, &val);
		FrameContext.Ecx = (DWORD)val;

		pStackFrame->get_registerValue(CV_REG_EDX, &val);
		FrameContext.Edx = (DWORD)val;

		pStackFrame->get_registerValue(CV_REG_EBX, &val);
		FrameContext.Ebx = (DWORD)val;

		pStackFrame->get_registerValue(CV_REG_ESP, &val);
		FrameContext.Esp = (DWORD)val;

		pStackFrame->get_registerValue(CV_REG_EBP, &val);
		FrameContext.Ebp = (DWORD)val;

		pStackFrame->get_registerValue(CV_REG_EIP, &val);
		FrameContext.Eip = (DWORD)val;

		pStackFrame->get_registerValue(CV_REG_EDX, &val);
		FrameContext.Edx = (DWORD)val;

		pStackFrame->get_registerValue(CV_REG_ESI, &val);
		FrameContext.Esi = (DWORD)val;

		pStackFrame->get_registerValue(CV_REG_EDI, &val);	
		FrameContext.Edi = (DWORD)val;

	}

	// TODO: What else to save from the frame?
	ULONGLONG ReturnAddress;
	CONTEXT FrameContext;
};

END_NAMESPACE