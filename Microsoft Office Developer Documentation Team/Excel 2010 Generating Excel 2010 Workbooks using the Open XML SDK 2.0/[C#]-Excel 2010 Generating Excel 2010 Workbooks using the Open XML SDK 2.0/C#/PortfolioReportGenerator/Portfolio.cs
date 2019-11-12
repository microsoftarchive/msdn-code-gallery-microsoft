using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortfolioReportGenerator
{
    class Portfolio
    {

        public Portfolio(string name)
        {
            if (name == "Steve")
            {
                this.Name = name;
                this.AccountNumber = 443221;
                this.BeginningValueQTR = 10000;
                this.BeginningValueYTD = 5800;
                this.ContributionsQTR = 2000;
                this.ContributionsYTD = 6000;
                this.DistributionsQTR = -100;
                this.DistributionsYTD = -300;
                this.FeesQTR = -50;
                this.FeesYTD = -150;
                this.GainLossQTR = 700;
                this.GainLossYTD = 1200;
                this.Performance1Yr = .10;
                this.Performance2Yr = .06;
                this.Performance3Yr = .085;
                this.Performance5Yr = .09;
                this.Performance10Yr = .0825;
                this.WithdrawalsQTR = 500;
                this.WithdrawalsYTD = 500;
                this.Holdings = new PortfolioItem[2];

            }
            else if (name == "Kelly")
            {
                this.Name = name;
                this.AccountNumber = 443699;
                this.BeginningValueQTR = 11000;
                this.BeginningValueYTD = 5300;
                this.ContributionsQTR = 0;
                this.ContributionsYTD = 5000;
                this.DistributionsQTR = 0;
                this.DistributionsYTD = 0;
                this.FeesQTR = -75;
                this.FeesYTD = -150;
                this.GainLossQTR = 575;
                this.GainLossYTD = 1350;
                this.Performance1Yr = .12;
                this.Performance2Yr = .05;
                this.Performance3Yr = .09;
                this.Performance5Yr = .0927;
                this.Performance10Yr = .084;
                this.WithdrawalsQTR = 0;
                this.WithdrawalsYTD = 0;
                this.Holdings = new PortfolioItem[3];
            }
            InitializeHoldings();
        }

        public string Name { get; set; }
        public Int32 AccountNumber { get; set; }
        public double BeginningValueQTR { get; set; }
        public double BeginningValueYTD { get; set; }
        public double ContributionsQTR { get; set; }
        public double ContributionsYTD { get; set; }
        public double WithdrawalsQTR { get; set; }
        public double WithdrawalsYTD { get; set; }
        public double DistributionsQTR { get; set; }
        public double DistributionsYTD { get; set; }
        public double FeesQTR { get; set; }
        public double FeesYTD { get; set; }
        public double GainLossQTR { get; set; }
        public double GainLossYTD { get; set; }
        public double PerformanceQTR { get; set; }
        public double PerformanceYTD { get; set; }
        public double Performance1Yr { get; set; }
        public double Performance2Yr { get; set; }
        public double Performance3Yr { get; set; }
        public double Performance5Yr { get; set; }
        public double Performance10Yr { get; set; }
        public PortfolioItem[] Holdings { get; set; }

        private void InitializeHoldings()
        {
            if (this.Name == "Steve")
            {
                this.Holdings[0] = new PortfolioItem();
                this.Holdings[0].Cost = 5000;
                this.Holdings[0].CurrentPrice = 14.25;
                this.Holdings[0].Description = "Adventure Works";
                this.Holdings[0].High52Week = 17.50;
                this.Holdings[0].Low52Week = 10.25;
                this.Holdings[0].SharesHeld = 500;
                this.Holdings[0].MarketValue = this.Holdings[0].SharesHeld * this.Holdings[0].CurrentPrice;
                this.Holdings[0].Ticker = "AW";

                this.Holdings[1] = new PortfolioItem();
                this.Holdings[1].Cost = 7590;
                this.Holdings[1].CurrentPrice = 18.00;
                this.Holdings[1].Description = "Contoso";
                this.Holdings[1].High52Week = 19.50;
                this.Holdings[1].Low52Week = 12.80;
                this.Holdings[1].SharesHeld = 500;
                this.Holdings[1].MarketValue = this.Holdings[1].SharesHeld * this.Holdings[1].CurrentPrice;
                this.Holdings[1].Ticker = "CTSO";

            }
            else if (this.Name == "Kelly")
            {
                this.Holdings[0] = new PortfolioItem();
                this.Holdings[0].Cost = 4900;
                this.Holdings[0].CurrentPrice = 14.25;
                this.Holdings[0].Description = "Adventure Works";
                this.Holdings[0].High52Week = 17.50;
                this.Holdings[0].Low52Week = 10.25;
                this.Holdings[0].SharesHeld = 300;
                this.Holdings[0].MarketValue = this.Holdings[0].SharesHeld * this.Holdings[0].CurrentPrice;
                this.Holdings[0].Ticker = "AW";

                this.Holdings[1] = new PortfolioItem();
                this.Holdings[1].Cost = 7790;
                this.Holdings[1].CurrentPrice = 18.00;
                this.Holdings[1].Description = "Contoso";
                this.Holdings[1].High52Week = 19.50;
                this.Holdings[1].Low52Week = 12.80;
                this.Holdings[1].SharesHeld = 700;
                this.Holdings[1].MarketValue = this.Holdings[1].SharesHeld * this.Holdings[1].CurrentPrice;
                this.Holdings[1].Ticker = "CTSO";

                this.Holdings[2] = new PortfolioItem();
                this.Holdings[2].Cost = 10900;
                this.Holdings[2].CurrentPrice = 10.00;
                this.Holdings[2].Description = "Wingtip Bank";
                this.Holdings[2].High52Week = 11.50;
                this.Holdings[2].Low52Week = 8.10;
                this.Holdings[2].SharesHeld = 1000;
                this.Holdings[2].MarketValue = this.Holdings[2].SharesHeld * this.Holdings[2].CurrentPrice;
                this.Holdings[2].Ticker = "WTIP";
            }
        }

    }

    class PortfolioItem
    {
        public string Ticker { get; set; }
        public string Description { get; set; }
        public double CurrentPrice { get; set; }
        public double SharesHeld { get; set; }
        public double MarketValue { get; set; }
        public double Cost { get; set; }
        public double High52Week { get; set; }
        public double Low52Week { get; set; }
    }
}
