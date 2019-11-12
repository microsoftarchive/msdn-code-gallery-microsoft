//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System.Collections.Generic;
using System;
using AzureMobileLeaderboard;

namespace AzureMobileLeaderboard
{
    public partial class MainPage
    {
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "Windows Azure Mobile Services - Leaderboard";
     }

    public class Scenario
    {
        public string Title { get; set; }

        public Type ClassType { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }

    public class Constants
    {
        public const int QuestionsCount = 3;

        public const string LeaderboardMessage1 = "Great Work!!! All the answers are correct!";

        public const string LeaderboardMessage2 = "Nice!! {0} correct answers!";

        public const string LeaderboardMessage3 = "Good. {0} correct answers. You can do it better next time!";

        public const string LeaderboardMessage4 = "None of the answers are correct. Please try again.";
    }
}
