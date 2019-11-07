//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

namespace AzureMobileSendEmail
{
    using System;
    using System.Runtime.Serialization;

    [DataContract(Name = "Feedback")]
    public class Feedback
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "sentdate")]
        public DateTime SentDate { get; set; }
    }
}