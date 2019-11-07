//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

namespace StoreRoom.Entities
{
    using System;
    using System.Runtime.Serialization;

    [DataContract(Name = "Channel")]
    public class Channel
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }

        [DataMember(Name = "expirationTime")]
        public DateTimeOffset ExpirationTime { get; set; }
    }
}
