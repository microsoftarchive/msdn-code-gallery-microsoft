using System;

namespace CritterStomp
{
    public static class Constants
    {
        public const string OpCodeParams = "1";
        public const string OpCodeCritterBorn = "2";
        public const string OpCodeCritterStomp = "3";
        public const string OpCodeUpdateScore = "4";
        public const string OpCodeSendScore = "5";
        public const string OpCodeUpdateClientTime = "6";
        public const string OpCodeSendDisplayName = "7";
        public const string OpCodeNumPlayers = "8";

        public const int SinglePlayer = 1;
        public const int MultiPlayer = 2;

        public const int FontSizeNormal = 12;
        public const int FontSizeSmall = 6;

        public const int ScoreIncrement = 5;

        public const int TimeUnit = 200;            // Basic time unit, in milliseconds.
        public const int TimeUnitsPerSecond = 5;
        public const int SecondsPerMinute = 60;

        public const int TimeToLiveMin = 3;         // Minimum living time, in TimeUnit.
        public const int TimeToLiveMax = 8;         // Maximum living time, in TimeUnit.
        public const int TimeToSpawnMin = 4;        // Minimum spawning time, in TimeUnit.
        public const int TimeToSpawnMax = 8;        // Maximum spawning time, in TimeUnit.
        public const int TimeToShowAsStomped = 5;   // Time to show after a critter is stomped, in TimeUnit.

        public const int PlayTime = 30;             // In seconds.

        public const int CritterCount = 3;

        public const int GridRows = 5;
        public const int GridColumns = 6;
        public const int CellHeight = 120;
        public const int CellWidth = 220;

        public const int MinimumScreenWidth = 1366;
    }
}
