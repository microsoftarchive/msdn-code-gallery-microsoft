#pragma once
using namespace Platform;
using namespace Windows::UI::Xaml::Interop;
namespace CppUnvsAppEnumRadioButton
{
	namespace Common
	{
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class BoolToSexConverter sealed : Windows::UI::Xaml::Data::IValueConverter
		{
		public:
			BoolToSexConverter();
			virtual Object^ Convert(Object^ value, TypeName targetType, Object^ parameter, String^ language);
			virtual Object^ ConvertBack(Object^ value, TypeName targetType, Object^ parameter, String^ language);
		private:
			~BoolToSexConverter();
		};		
	}	
}