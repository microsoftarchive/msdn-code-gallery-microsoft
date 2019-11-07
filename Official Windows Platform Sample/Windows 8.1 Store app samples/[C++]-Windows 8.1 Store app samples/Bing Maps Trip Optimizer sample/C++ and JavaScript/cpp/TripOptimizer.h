//-----------------------------------------------------------------------
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
// 
// 
// THIS CODE AND INFORMATION ARE PROVIDED AS-IS WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//-----------------------------------------------------------------------
#pragma once

namespace TripOptimizerComponent 
{
    // Forward declarations.
    namespace Details
    {
        class TripOptimizerImpl;
    }
    
    // Defines the TripOptimizer class. This class interfaces between the app
    // and the implementation details.
    public ref class TripOptimizer sealed 
    {
    public:
        TripOptimizer();

        // Optimizes a trip as an asynchronous process.
        Windows::Foundation::IAsyncOperationWithProgress<
            Windows::Foundation::Collections::IMap<
                Platform::String^, 
                Windows::Foundation::Collections::IVector<Platform::String^>^>^, 
            Platform::String^>^ OptimizeTripAsync(
                Windows::Foundation::Collections::IVector<Platform::String^>^ waypoints, 
                Platform::String^ travelMode, 
                Platform::String^ optimize,
                Platform::String^ bingMapsKey, 
                double alpha, double beta, double rho,
                unsigned int iterations, bool parallel);

    private:
        ~TripOptimizer();
        // Defines implementation details of the optimization routine.
        std::unique_ptr<Details::TripOptimizerImpl> m_impl;
    };
}