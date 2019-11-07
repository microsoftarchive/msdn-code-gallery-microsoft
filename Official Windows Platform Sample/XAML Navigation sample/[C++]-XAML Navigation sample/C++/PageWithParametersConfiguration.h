#pragma once

namespace SDKSample
{
	namespace Navigation
	{
		static int _numberPages;

		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class PageWithParametersConfiguration sealed
		{
		public:
			PageWithParametersConfiguration();

			property Platform::String^ Message
			{
				Platform::String^ get()
				{
					return _message;
				}
				void set(Platform::String^ value)
				{
					if (!_message->Equals(value))
					{
						_message = value;
					}
				}
			}
			property int ID
			{
				int get()
				{
					return _ID;
				}
				void set(int value)
				{
					_ID = value;
				}
			}
		private:
			Platform::String^ _message;
			int _ID;
		};
	}
}


