=============================================================================
          LIBRARY APPLICATION : CSClassLibrary Project Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

The code sample demonstrates a C# class library that we can use in other 
applications. The class library exposes a simple class named CSSimpleObject. 
The class contains:

Constructor:
    public CSSimpleObject();

Instance field and property:
    private float fField;
    public float FloatProperty

Instance method:
    public override string ToString();

Shared (static) method:
    public static int GetStringLength(string str);

Instance event:
    // The event is fired in the set accessor of FloatProperty
    public event PropertyChangingEventHandler FloatPropertyChanging;

The process of creating the class library is very straight-forward.


/////////////////////////////////////////////////////////////////////////////
Sample Relation:

CSReflection -> CSClassLibrary
CSReflection dynamically loads the assembly, CSClassLibrary.dll, and 
instantiate, examine and use its types.

CppHostCLR -> CSClassLibrary
CppHostCLR hosts CLR, instantiates a type exposed in CSClassLibrary.dll and 
calls its methods.

CSClassLibrary - VBClassLibrary
They are the same class library implemented in different languages.


/////////////////////////////////////////////////////////////////////////////
Implementation:

A. Creating the project

Step1. Create a Visual C# / Class Library project named CSClassLibrary in 
Visual Studio 2008.

B. Adding a class CSSimpleObject to the project and define its fields, 
properties, methods, and events.

Step1. In Solution Explorer, add a new Class item to the project and name it
as CSSimpleObject.

Step2. Edit the file CSSimpleObject.cs to add the fields, properties, methods,
and events.

C. Signing the assembly with a strong name (Optional)

Strong names are required to store shared assemblies in Global Assembly Cache
(GAC). This helps avoid DLL Hell. Strong names also protects the assembly 
from being hacked (replaced or injected). A strong name consists of the 
assembly's identity—its simple text name, version number, and culture info
(if provided)—plus a public key and a digital signature. It is generated 
from an assembly file using the corresponding private key. 

Step1. Right-click the project and open its Properties page.

Step2. Turn to the Signing tab, and check the Sign the assembly checkbox. 

Step3. In the Choose a strong name key file drop-down, select New. The 
"Create Strong Name Key" dialog appears. In the Key file name text box, type
the desired key name. If necessary, we can protect the strong name key file 
with a password. Click the OK button.


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: Creating Assemblies
http://msdn.microsoft.com/en-us/library/b0b8dk77.aspx

How to: Sign an Assembly with a Strong Name
http://msdn.microsoft.com/en-us/library/xc31ft41.aspx

How to: Create and Use C# DLLs (C# Programming Guide)
http://msdn.microsoft.com/en-us/library/3707x96z.aspx


/////////////////////////////////////////////////////////////////////////////
