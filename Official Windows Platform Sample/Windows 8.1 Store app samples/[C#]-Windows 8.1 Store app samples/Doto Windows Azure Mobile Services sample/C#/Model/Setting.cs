// Copyright (c) Microsoft Corporation. All rights reserved

using Microsoft.WindowsAzure.MobileServices;
using System.Runtime.Serialization;

namespace Doto.Model
{
    /// <summary>
    /// A class used to store simple global app settings in Windows Azure Mobile Service
    /// </summary>
    [DataContract(Name = "settings")]
    public class Setting
    {
        public string Id { get; set; }

        [DataMember(Name = "key")]
        public string Key { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }
    }
}
