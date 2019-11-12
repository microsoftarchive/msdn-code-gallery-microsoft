#include "pch.h"
#include "helper.h"
#include <Strsafe.h>

Platform::String^ FormatStr(Platform::String^ format, Platform::String^ arg1)
{
	const int size = 1024;
	wchar_t formattedString[size];
	StringCchPrintf(formattedString, size, format->Data(), arg1->Data());

	Platform::String^ str1;
	str1 = ref new Platform::String(formattedString);

	return str1;
}

Platform::String^ FormatStr(Platform::String^ format, Platform::String^ arg1, Platform::String^ arg2)
{
	const int size = 2048;
	wchar_t formattedString[size];
	StringCchPrintf(formattedString, size, format->Data(), arg1->Data(), arg2->Data());

	Platform::String^ str1;
	str1 = ref new Platform::String(formattedString);

	return str1;
}

