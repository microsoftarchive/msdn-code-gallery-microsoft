//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Collections.Generic;
using Windows.Data.Json;
using Windows.UI.Xaml;

using Geofencing4SqSampleTasks;

namespace Geofencing4SqSample
{
    /// <summary>
    /// Encapsulates a Foursquare checkin
    /// </summary>
    class Checkin
    {
        public string Id { get; set; }
        public DateTime UtcTimestamp { get; set; }
        public Venue Venue { get; set; }

        #region Databinding fields

        public string Title
        {
            get
            {
                return Venue.Name;
            }
        }

        public string Subtitle1
        {
            get
            {
                return Venue.Address;
            }
        }

        public string Subtitle2
        {
            get
            {
                return "Time: " + UtcTimestamp.ToLocalTime().ToString();
            }
        }

        public string LinkUrl
        {
            get
            {
                return null;
            }
        }

        public Visibility LinkVisibility
        {
            get
            {
                return String.IsNullOrEmpty(LinkUrl) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        #endregion Databinding fields

        #region Parsers

        public static Checkin FromJson(JsonObject json)
        {
            var c = new Checkin();
            c.UtcTimestamp = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds((long)json["createdAt"].GetNumber());
            c.Venue = Venue.FromJson(json["venue"].GetObject());

            return c;
        }

        public static List<Checkin> ListFromJson(JsonObject json)
        {
            var items = json["checkins"].GetObject()["items"].GetArray();
            var result = new List<Checkin>();
            foreach (var item in items)
            {
                try
                {
                    var v = Checkin.FromJson(item.GetObject());
                    result.Add(v);
                }
                catch (KeyNotFoundException)
                {
                    // If key not found, skip the item
                    Logger.Trace(TraceLevel.Warn, "Skipping checkin because of missing key");
                }
            }
            return result;
        }

        #endregion Parsers
    }
}
