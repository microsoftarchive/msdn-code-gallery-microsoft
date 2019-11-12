=============================================================================
      DYNAMIC LINK LIBRARY : CppCLINETAssemblyWrapper Project Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

The code in this file declares the C++ wrapper class CSSimpleObjectWrapper for 
the .NET class CSSimpleObject defined in the .NET class library 
CSClassLibrary. Your native C++ application can include this wrapper class 
and link to the DLL to indirectly call the .NET class.

  CppCallNETAssemblyWrapper (a native C++ application)
          -->
      CppCLINETAssemblyWrapper (this C++/CLI wrapper)
              -->
          CSClassLibrary (a .NET assembly)


/////////////////////////////////////////////////////////////////////////////
Sample Relation:
(The relationship between the current sample and the rest samples in 
Microsoft All-In-One Code Framework http://1code.codeplex.com)

CppCLINETAssemblyWrapper -> CSClassLibrary
The C++/CLI sample module CppCLINETAssemblyWrapper wraps the .NET class 
defined in the C# class library CSClassLibrary. The wrapper class can be 
called by any native C++ applications to indirectly interoperate with the 
.NET class.


/////////////////////////////////////////////////////////////////////////////
Implementation:

Step1. Create a Visual C++ / CLR / Class Library project named 
CppCLINETAssemblyWrapper in Visual Studio 2008. The project wizard generates 
a default empty C++/CLI class:

    namespace CppCLINativeDllWrapper {

	    public ref class Class1
	    {
		    // TODO: Add your methods for this class here.
	    };
    }

Step2. Reference the C# class library CSClassLibrary in the C++/CLI class 
library project.

Step3. Configure the C++/CLI class library to export symbols. The symbols 
can be imported and called by native C++ applications.

Add CPPCLINETASSEMBLYWRAPPER_EXPORTS to the preprocessor definitions of the 
project (see the C/C++ / Preprocessor page in the project Properties dialog). 
All files within this DLL are compiled with the symbol 
CPPCLINETASSEMBLYWRAPPER_EXPORTS. This symbol should not be defined on any 
project that uses this DLL. 

In the header file CppCLINETAssemblyWrapper.h, add the following definitions:

    #ifdef CPPCLINETASSEMBLYWRAPPER_EXPORTS
    #define SYMBOL_DECLSPEC __declspec(dllexport)
    #else
    #define SYMBOL_DECLSPEC	__declspec(dllimport)
    #endif

Any other project whose source files include this header file see 
SYMBOL_DECLSPEC classes and functions as being imported from a DLL, whereas 
this DLL sees symbols defined with this macro as being exported.

Because the header file may be included by any other native C++ project, the 
file should only contain native C++ types, includes and keywords.

Step4. Design the C++ wrapper class CSSimpleObjectWrapper for the .NET class 
CSimpleObject defined in the C# class library CSClassLibrary.

In the header file CppCLINETAssemblyWrapper.h, declare the class.

    class SYMBOL_DECLSPEC CSSimpleObjectWrapper
    {
    public:
        CSSimpleObjectWrapper(void);
        virtual ~CSSimpleObjectWrapper(void);

        // Property
        float get_FloatProperty(void);
        void set_FloatProperty(float fVal);

        // Method
        HRESULT ToString(PWSTR pszBuffer, DWORD dwSize);

        // Static method
        static int GetStringLength(PCWSTR pszString);

    private:
        void *m_impl;
    };

The class contains a native C++ generic pointer (void *m_impl;) to the 
wrapped .NET object. It is initialized in the constructor 
CSSimpleObjectWrapper(void);, and the wrapped object is freed in the 
desctructor (virtual ~CSSimpleObjectWrapper(void);).

    CSSimpleObjectWrapper::CSSimpleObjectWrapper(void)
    {
        // Instantiate the C# class CSSimpleObject.
        CSSimpleObject ^ obj = gcnew CSSimpleObject();

        // Pin the CSSimpleObject .NET object, and record the address of the 
        // pinned object in m_impl. 
        m_impl = GCHandle::ToIntPtr(GCHandle::Alloc(obj)).ToPointer(); 
    }

    CSSimpleObjectWrapper::~CSSimpleObjectWrapper(void)
    {
        // Get the GCHandle associated with the pinned object based on its 
        // address, and free the GCHandle. At this point, the CSSimpleObject 
        // object is eligible for GC.
        GCHandle h = GCHandle::FromIntPtr(IntPtr(m_impl));
        h.Free();
    }

The public member methods of CSSimpleObjectWrapper wraps those of the C# class 
CSSimpleObject. They redirects the calls to CSSimpleObject through the 
CSSimpleObject object pointed by m_impl. Type marshaling takes place between 
the managed and native code.

    float CSSimpleObjectWrapper::get_FloatProperty(void)
    {
        // Get the pinned CSSimpleObject object from its memory address.
        GCHandle h = GCHandle::FromIntPtr(IntPtr(m_impl));
        CSSimpleObject ^ obj = safe_cast<CSSimpleObject^>(h.Target);

        // Redirect the call to the corresponding property of the wrapped 
        // CSSimpleObject object.
        return obj->FloatProperty;
    }

    void CSSimpleObjectWrapper::set_FloatProperty(float fVal)
    {
        // Get the pinned CSSimpleObject object from its memory address.
        GCHandle h = GCHandle::FromIntPtr(IntPtr(m_impl));
        CSSimpleObject ^ obj = safe_cast<CSSimpleObject^>(h.Target);

        // Redirect the call to the corresponding property of the wrapped 
        // CSSimpleObject object.
        obj->FloatProperty = fVal;
    }

    HRESULT CSSimpleObjectWrapper::ToString(PWSTR pszBuffer, DWORD dwSize)
    {
        // Get the pinned CSSimpleObject object from its memory address.
        GCHandle h = GCHandle::FromIntPtr(IntPtr(m_impl));
        CSSimpleObject ^ obj = safe_cast<CSSimpleObject^>(h.Target);

        String ^ str;
        HRESULT hr;
        try
        {
            // Redirect the call to the corresponding method of the wrapped 
            // CSSimpleObject object.
            str = obj->ToString();
        }
        catch (Exception ^ e)
        {
            hr = Marshal::GetHRForException(e);
        }

        if (SUCCEEDED(hr))
        {
            // Convert System::String to PCWSTR.
            marshal_context ^ context = gcnew marshal_context();
            PCWSTR pszStr = context->marshal_as<const wchar_t*>(str);
            hr = StringCchCopy(pszBuffer, dwSize, pszStr == NULL ? L"" : pszStr);
            delete context; // This will also free the memory pointed by pszStr
        }

        return hr;
    }

    int CSSimpleObjectWrapper::GetStringLength(PCWSTR pszString)
    {
        return CSSimpleObject::GetStringLength(gcnew String(pszString));
    }


/////////////////////////////////////////////////////////////////////////////
References:

Using C++ Interop (Implicit PInvoke)
http://msdn.microsoft.com/en-us/library/2x8kf7zx.aspx


/////////////////////////////////////////////////////////////////////////////