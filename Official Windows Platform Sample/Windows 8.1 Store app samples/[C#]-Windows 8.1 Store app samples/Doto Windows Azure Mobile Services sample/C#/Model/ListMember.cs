// Copyright (c) Microsoft Corporation. All rights reserved

using Microsoft.WindowsAzure.MobileServices;
using System.Runtime.Serialization;

namespace Doto.Model
{
    /// <summary>
    /// A class that represents a user's membership of the list. Note that 
    /// there is no 'master' list class or table. The name of the list is stored
    /// redundantly across all listMember records.
    /// </summary>
    [DataContract(Name = "listMembers")]
    public class ListMember
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "userId")]
        public string UserId { get; set; }

        [DataMember(Name = "listId")]
        public string ListId { get; set; }
    }
}
