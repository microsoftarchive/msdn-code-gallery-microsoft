// Copyright (c) Microsoft Corporation. All rights reserved

using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Runtime.Serialization;

namespace Doto.Model
{
    /// <summary>
    /// Represents a Todo (or Doto) item stored in Windows Azure Mobile Service
    /// </summary>
    [DataContract(Name = "items")]
    public class Item
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "listId")]
        public string ListId { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "created")]
        public DateTime Created { get; set; }

        [DataMember(Name = "createdBy")]
        public string CreatedBy { get; set; }
    }
}