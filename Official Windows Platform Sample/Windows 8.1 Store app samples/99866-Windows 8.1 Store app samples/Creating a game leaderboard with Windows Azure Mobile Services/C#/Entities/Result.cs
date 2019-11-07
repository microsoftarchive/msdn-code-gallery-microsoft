// Copyright (c) Microsoft Corporation. All rights reserved 

namespace AzureMobileLeaderboard.Entities
{
    using System.Runtime.Serialization;

    [DataContract(Name = "Result")]
    public class Result
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "playerName")]
        public string PlayerName { get; set; }

        [DataMember(Name = "hits")]
        public int Hits { get; set; }

        [DataMember(Name = "misses")]
        public int Misses { get; set; }

        [DataMember(Name = "leaderboardUpdated")]
        public bool LeaderboardUpdated { get; set; }
    }
}
