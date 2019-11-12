#pragma once

namespace AppResourceClassLibrary
{
    public ref class Class1 sealed
    {
    public:
        Class1();
		String^ GetString();
	private:
		Windows::ApplicationModel::Resources::ResourceLoader ^resourceLoader;
    };
}