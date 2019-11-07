// Copyright (c) Microsoft Corporation. All rights reserved

using Microsoft.WindowsAzure.MobileServices;
using System.Runtime.Serialization;

namespace Doto.Model
{
    /// <summary>
    /// Represents an invite requesting another user to join a list
    /// which is stored in Windows Azure Mobile Service
    /// </summary>
    [DataContract(Name = "invites")]
    public class Invite
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "fromUserId")]
        public string FromUserId { get; set; }

        [DataMember(Name = "toUserId")]
        public string ToUserId { get; set; }

        [DataMember(Name = "fromUserName")]
        public string FromUserName { get; set; }

        [DataMember(Name = "fromImageUri")]
        public string FromImageUri { get; set; }

        [DataMember(Name = "toUserName")]
        public string ToUserName { get; set; }

        [DataMember(Name = "listName")]
        public string ListName { get; set; }

        [DataMember(Name = "listId")]
        public string ListId { get; set; }

        [DataMember(Name = "approved")]
        public bool IsApproved { get; set; }
    }
}
