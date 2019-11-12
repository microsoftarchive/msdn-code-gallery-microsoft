//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DeviceResources.h"
#include "StepTimer.h"

namespace D2DGeometryRealizations
{
    // This sample renderer instantiates a basic rendering pipeline.
    class D2DGeometryRealizationsRenderer
    {
    public:
        D2DGeometryRealizationsRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources);
        void CreateDeviceDependentResources();
        void CreateWindowSizeDependentResources();
        void ReleaseDeviceDependentResources();
        void Update(DX::StepTimer const& timer);
        void Render();

        void SetWorldMatrix(D2D1::Matrix3x2F worldMatrix);

        void IncreasePrimitives();
        void DecreasePrimitives();

        bool GetRealizationsEnabled();
        void SetRealizationsEnabled(bool enabled);

        void RestoreDefaults();

        void SaveInternalState(Windows::Foundation::Collections::IPropertySet^ state);
        void LoadInternalState(Windows::Foundation::Collections::IPropertySet^ state);

    private:
        void CreateScaleDependentResources();

        void CheckScaleWindow();

        // Cached pointer to device resources.
        std::shared_ptr<DX::DeviceResources> m_deviceResources;

        // Sample-specific resources.

        // Gometry object that defines the shape to be rendered.
        Microsoft::WRL::ComPtr<ID2D1Geometry> m_geometry;

        // Objects that represent the realized version of the geometry.
        Microsoft::WRL::ComPtr<ID2D1GeometryRealization> m_filledGeometryRealization;
        Microsoft::WRL::ComPtr<ID2D1GeometryRealization> m_strokedGeometryRealization;

        // Brushes used to draw the geometry and its realizations.
        Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> m_fillBrush;
        Microsoft::WRL::ComPtr<ID2D1SolidColorBrush> m_strokeBrush;

        // Matrix to track the zoom and viewing position of the scene.
        D2D1_MATRIX_3X2_F m_worldMatrix;

        // Member variables that determine how the primitives are rendered.
        int m_numberOfRows;
        unsigned int m_angle;
        bool m_realizationsEnabled;

        // The scene scale at which the realizations were most recently created.
        float m_lastScale;
    };
}
