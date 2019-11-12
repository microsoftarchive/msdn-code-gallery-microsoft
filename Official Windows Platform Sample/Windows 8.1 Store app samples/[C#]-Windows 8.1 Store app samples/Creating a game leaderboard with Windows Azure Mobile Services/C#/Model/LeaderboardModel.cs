// Copyright (c) Microsoft Corporation. All rights reserved 

namespace AzureMobileLeaderboard.Model
{
    using System.Collections.Generic;

    public class LeaderboardModel
    {
        public LeaderboardModel()
        {
            this.Items = new List<LeaderboardItemModel>();
        }

        public IList<LeaderboardItemModel> Items { get; set; }
    }
}
