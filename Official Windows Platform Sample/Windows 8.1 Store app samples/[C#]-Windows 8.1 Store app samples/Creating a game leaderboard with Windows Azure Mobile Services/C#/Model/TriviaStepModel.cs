// Copyright (c) Microsoft Corporation. All rights reserved 

namespace AzureMobileLeaderboard.Model
{
    using System.Collections.Generic;

    public class TriviaStepModel
    {
        public TriviaStepModel()
        {
            this.Answers = new List<AnswerModel>();
        }

        public string Question { get; set; }

        public IList<AnswerModel> Answers { get; set; }

        public int CorrectAnswer { get; set; }
    }
}
