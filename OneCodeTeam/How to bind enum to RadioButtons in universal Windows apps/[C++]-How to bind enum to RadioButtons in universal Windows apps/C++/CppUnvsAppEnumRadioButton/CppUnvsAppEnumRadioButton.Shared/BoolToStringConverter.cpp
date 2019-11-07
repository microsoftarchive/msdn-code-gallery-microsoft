#include "pch.h"
#include "BoolToStringConverter.h"

using namespace CppUnvsAppEnumRadioButton::Common;

BoolToSexConverter::BoolToSexConverter()
{

}

BoolToSexConverter::~BoolToSexConverter()
{

}

Object^ BoolToSexConverter::Convert(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
	return (bool)value ? "Male" : "Female";
}

Object^ BoolToSexConverter::ConvertBack(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
	Platform::String^ sex = (Platform::String^)value;
	if (sex == "Female")
	{
		return false;
	}
	else
		return true;
}