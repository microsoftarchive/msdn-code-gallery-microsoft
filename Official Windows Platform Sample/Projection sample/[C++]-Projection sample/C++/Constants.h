//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once

namespace SDKSample
{
    public value struct Scenario
    {
        Platform::String^ Title;
        Platform::String^ ClassName;
    };

    partial ref class MainPage
    {
    internal:
        SecondaryViewsHelpers::ViewLifetimeControl^ ProjectionViewPageControl;

        static property Platform::String^ FEATURE_NAME 
        {
            Platform::String^ get() 
            {  
                return "Projection"; 
            }
        }

        static property Platform::Array<Scenario>^ scenarios 
        {
            Platform::Array<Scenario>^ get() 
            { 
                return scenariosInner; 
            }
        }
    private:
        static Platform::Array<Scenario>^ scenariosInner;
    };

    namespace Projection
    {
        // This is a simple container for a set of data used to initialize the projection view
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class ProjectionViewPageInitializationData sealed
        {
        internal:
            Windows::UI::Core::CoreDispatcher^ MainDispatcher;
            SecondaryViewsHelpers::ViewLifetimeControl^ ProjectionViewPageControl;
            int MainViewId;
        };
    }
}
