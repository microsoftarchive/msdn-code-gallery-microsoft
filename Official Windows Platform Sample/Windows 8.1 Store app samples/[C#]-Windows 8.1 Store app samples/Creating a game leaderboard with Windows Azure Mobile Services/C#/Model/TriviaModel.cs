// Copyright (c) Microsoft Corporation. All rights reserved 

namespace AzureMobileLeaderboard.Model
{
    using System.Collections.Generic;

    public class TriviaModel
    {
        public TriviaModel()
        {
            this.Steps = new List<TriviaStepModel>();
        }

        public string PlayerName { get; set; }

        public IList<TriviaStepModel> Steps { get; set; }
    }
}
