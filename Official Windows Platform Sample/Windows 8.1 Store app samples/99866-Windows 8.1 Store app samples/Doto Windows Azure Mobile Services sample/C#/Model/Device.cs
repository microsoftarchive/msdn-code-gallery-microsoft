// Copyright (c) Microsoft Corporation. All rights reserved

using Microsoft.WindowsAzure.MobileServices;
using System.Runtime.Serialization;

namespace Doto.Model
{
    /// <summary>
    /// Used to store the channel URI for each user and installation in Windows Azure Mobile Service
    /// </summary>
    [DataContract(Name = "devices")]
    public class Device
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "userId")]
        public string UserId { get; set; }

        [DataMember(Name = "installationId")]
        public string InstallationId { get; set; }

        [DataMember(Name = "channelUri")]
        public string ChannelUri { get; set; }
    }
}
