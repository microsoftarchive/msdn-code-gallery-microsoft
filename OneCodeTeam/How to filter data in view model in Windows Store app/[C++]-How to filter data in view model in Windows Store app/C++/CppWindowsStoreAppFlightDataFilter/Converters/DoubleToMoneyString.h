
using namespace Windows::UI::Xaml::Data;
using namespace Platform;

namespace CppWindowsStoreAppFlightDataFilter
{
	namespace Converters
	{
		/// <summary>
		/// Value converter that converts double to money string. 
		/// </summary>
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class DoubleToMoneyString sealed : Windows::UI::Xaml::Data::IValueConverter
		{
		public:
			virtual Platform::Object^ Convert(Platform::Object^ value, Windows::UI::Xaml::Interop::TypeName targetType, Platform::Object^ parameter, Platform::String^ language)
			{
				double price = static_cast<double>(value);
				return price;
			}
			virtual Platform::Object^ ConvertBack(Platform::Object^ value, Windows::UI::Xaml::Interop::TypeName targetType, Platform::Object^ parameter, Platform::String^ language)
			{
				throw ref new NotImplementedException();
			}
		};
	}
}