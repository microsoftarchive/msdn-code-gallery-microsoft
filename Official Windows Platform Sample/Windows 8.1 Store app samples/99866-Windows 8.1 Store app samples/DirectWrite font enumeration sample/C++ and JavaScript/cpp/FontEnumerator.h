#pragma once

namespace FontEnumeration
{
    public ref class FontEnumerator sealed
    {
    public:
        Platform::Array<Platform::String^>^ ListSystemFonts();
    };
}