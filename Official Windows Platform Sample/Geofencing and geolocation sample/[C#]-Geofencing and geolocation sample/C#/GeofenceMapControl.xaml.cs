//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using Bing.Maps;
using System;
using Windows.UI.Xaml.Controls;

namespace Geofencing4SqSample
{
    public sealed partial class GeofenceMapControl : UserControl
    {
        private Map _map;
        private double _radiusInMeters;

        public GeofenceMapControl(Map map)
        {
            this.InitializeComponent();
            _map = map;
            _map.ViewChanged += (s, e) =>
            {
                UpdateAccuracyCircle();
            };
        }

        public double RadiusInMeters
        {
            get
            {
                return _radiusInMeters;
            }

            set
            {
                _radiusInMeters = value;
                UpdateAccuracyCircle();
            }
        }

        public int ListViewIndex { get; set; }

        private void UpdateAccuracyCircle()
        {
            if (_radiusInMeters >= 0)
            {
                const double EarthRadiusMeters = 6378137.0;

                // Calculate the ground resolution in meters/pixel
                // Math based on http://msdn.microsoft.com/en-us/library/bb259689.aspx
                double groundResolution = Math.Cos(_map.Center.Latitude * Math.PI / 180) * Math.PI * EarthRadiusMeters / (256 * Math.Pow(2, _map.ZoomLevel));

                // Calculate the radius of the accuracy circle in pixels
                double pixelRadius = _radiusInMeters / groundResolution;

                // Update the accuracy circle dimensions
                AccuracyCircle.Width = pixelRadius;
                AccuracyCircle.Height = pixelRadius;

                // Use the margin property to center the accuracy circle
                AccuracyCircle.Margin = new Windows.UI.Xaml.Thickness(-pixelRadius / 2, -pixelRadius / 2, 0, 0);
            }
        }
    }
}
