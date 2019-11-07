// Class1.cpp
#include "pch.h"
#include "Class1.h"

using namespace AppResourceClassLibrary;
using namespace Platform;
using namespace Windows::ApplicationModel::Resources;

Class1::Class1()
{
	resourceLoader = ref new ResourceLoader("AppResourceClassLibrary/Resources");
}

 String^  Class1::GetString()
{
    return resourceLoader->GetString("string1");
}