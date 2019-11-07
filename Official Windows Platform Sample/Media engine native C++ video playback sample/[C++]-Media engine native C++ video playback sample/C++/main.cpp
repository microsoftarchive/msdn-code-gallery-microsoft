#include "pch.h"
#include "FrameworkView.h"

[Platform::MTAThread]
int __cdecl main(Platform::Array<Platform::String^>^)
{
    auto frameworkView = ref new FrameworkViewSource();
    Windows::ApplicationModel::Core::CoreApplication::Run(frameworkView);
}
