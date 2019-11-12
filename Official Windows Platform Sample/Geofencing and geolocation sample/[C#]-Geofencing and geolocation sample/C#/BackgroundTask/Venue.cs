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
using System.Diagnostics;
using System.Runtime.Serialization;
using Windows.Data.Json;
using Windows.UI.Xaml;

namespace Geofencing4SqSampleTasks
{
    /// <summary>
    /// Encapsulates a Foursquare venue
    /// </summary>
    [DataContract]
    public sealed class Venue
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public float Latitude { get; set; }

        [DataMember]
        public float Longitude { get; set; }

        public string Address { get; set; }

        public string Uri { get; set; }

        public uint Likes { get; set; }

        public override string ToString() { return Name; }

        #region Databinding fields

        public string Title
        {
            get
            {
                return Name;
            }
        }

        public string Subtitle1
        {
            get
            {
                return Address;
            }
        }

        public string Subtitle2
        {
            get
            {
                return "Likes: " + Likes;
            }
        }

        public string LinkUrl
        {
            get
            {
                return Uri + "?ref=" + Constants.ClientId;
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

        public static Venue FromJson(JsonObject json)
        {
            Venue v = new Venue();
            v.Id = json["id"].GetString();
            v.Name = json["name"].GetString();

            // Note: the canonicalUrl is available from venue details
            // i.e. /venues/VenueId. Instead of making a separate request
            // per venue, we will use a shortcut here and compose
            // the canonicalUrl from the id. This is for convenience only.
            // If you are already planning to make a separate request for
            // complete venue information, use the canonicalUrl from there instead.
            v.Uri = "https://foursquare.com/v/" + v.Id;

            var locationAsJson = json["location"].GetObject();
            v.Address = locationAsJson["address"].GetString();
            v.Latitude = (float)locationAsJson["lat"].GetNumber();
            v.Longitude = (float)locationAsJson["lng"].GetNumber();

            var likesAsJson = json["likes"].GetObject();
            v.Likes = (uint)likesAsJson["count"].GetNumber();

            return v;
        }

        public static System.Collections.Generic.IList<Venue> ListFromJson(JsonObject json)
        {
            var items = json["groups"].GetArray()[0].GetObject()["items"].GetArray();
            List<Venue> result = new List<Venue>();
            foreach (var item in items)
            {
                try
                {
                    Venue v = Venue.FromJson(item.GetObject()["venue"].GetObject());
                    result.Add(v);
                }
                catch (KeyNotFoundException)
                {
                    // If key not found, skip the item
                    Debug.WriteLine("Skipping venue because of missing key");
                }
            }
            return result;
        }

        #endregion Parsers
    }
}
