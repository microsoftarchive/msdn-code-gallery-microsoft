//
// SimplePage.xaml.h
// Declaration of the SimplePage class
//

#pragma once

#include "SimplePage.g.h"

namespace SDKSample
{
	static int _numberSimplePages;

	public ref class SimplePage sealed
	{
	public:
		SimplePage();
		
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
		  int _ID;
	};
}
