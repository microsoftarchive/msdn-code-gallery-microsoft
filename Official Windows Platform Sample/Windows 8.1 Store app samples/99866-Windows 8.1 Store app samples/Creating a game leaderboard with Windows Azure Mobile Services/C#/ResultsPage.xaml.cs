// Copyright (c) Microsoft Corporation. All rights reserved 

namespace AzureMobileLeaderboard
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using AzureMobileLeaderboard.Entities;
    using AzureMobileLeaderboard.Model;    
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;
    using System.Net.Http;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ResultsPage : Page
    {
        private MainPage rootPage;

        public ResultsPage()
        {
            this.InitializeComponent();
        }        

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.rootPage = e.Parameter as MainPage;

            var hits = App.Data.Steps.Count(item => item.Answers.ToList().FindIndex(a => a.IsSelected) == item.CorrectAnswer - 1);
            var misses = Constants.QuestionsCount - hits;
            var percentageAnswered = (hits * 100) / Constants.QuestionsCount;

            if (percentageAnswered == 0)
            {
                this.ResultsLegend.Text = Constants.LeaderboardMessage4;
            }
            else if (percentageAnswered == 100)
            {
                this.ResultsLegend.Text = Constants.LeaderboardMessage1;                    
            }
            else if (percentageAnswered <= 50)
            {
                this.ResultsLegend.Text = string.Format(Constants.LeaderboardMessage3, hits);
            }
            else
            {
                this.ResultsLegend.Text = string.Format(Constants.LeaderboardMessage2, hits);
            }

            this.SendResults(hits, misses);
        }

        private async void ShowError()
        {
            MessageDialog dialog = new MessageDialog("Correctly configure the Mobile Service url and key in the sample.", "Configuration error");
            await dialog.ShowAsync();
        }

        private async void SendResults(int hits, int misses)
        {
            var resultsEntity = new Result { PlayerName = App.Data.PlayerName, Hits = hits, Misses = misses };
             try
            {
                await App.MobileService.GetTable<Result>().InsertAsync(resultsEntity);
                this.RefreshLeaderboard(resultsEntity.Id);
            }
            catch (HttpRequestException)
            {
                ShowError();
            }
        }

        private async void RefreshLeaderboard(string resultsId)
        {
            var sw = new Stopwatch();
            sw.Start();

            var leaderboardUpdated = false;
            while (!leaderboardUpdated && sw.ElapsedMilliseconds < 5000)
            {
                var aux = await App.MobileService.GetTable<Result>().Where(r => r.Id == resultsId).ToEnumerableAsync();

                var resultsItem = aux.Single();
                leaderboardUpdated = resultsItem.LeaderboardUpdated;
            }

            sw.Stop();

            if (leaderboardUpdated)
            {
                var leaderboardItems = await App.MobileService.GetTable<Leaderboard>().ToEnumerableAsync();
                leaderboardItems = leaderboardItems.OrderBy(item => item.Position).Take(5);

                var model = new LeaderboardModel();
                foreach (var item in leaderboardItems)
                {
                    model.Items.Add(new LeaderboardItemModel
                    {
                        Player = item.PlayerName,
                        Position = item.Position,
                        Score = item.Score
                    });
                }

                this.DataContext = model;

                this.LoadingLeaderboardLegend.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.LoadingLeaderboardProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.LeaderboardGridView.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                this.LoadingLeaderboardLegend.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.LoadingLeaderboardProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                var msg = new MessageDialog("The leaderboard could not be retrieved, please check if the server-side script is properly configured on the mobile service.");
                await msg.ShowAsync();
            }
        }

        private void PlayAgainButton_Click(object sender, RoutedEventArgs e)
        {
            this.rootPage.GetFrameContent().Navigate(typeof(StartPage), this.rootPage);
        }
    }
}
