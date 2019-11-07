// Copyright (c) Microsoft Corporation. All rights reserved 

namespace AzureMobileLeaderboard.Entities
{
    using System.Runtime.Serialization;

    [DataContract(Name = "Leaderboard")]
    public class Leaderboard
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "position")]
        public int Position { get; set; }

        [DataMember(Name = "playerName")]
        public string PlayerName { get; set; }

        [DataMember(Name = "score")]
        public int Score { get; set; }
    }
}
