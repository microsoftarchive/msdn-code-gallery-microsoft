// Copyright (c) Microsoft Corporation. All rights reserved 

namespace AzureMobileLeaderboard
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AzureMobileLeaderboard.Model;    
    using Windows.Data.Json;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartPage : Page
    {
        private MainPage rootPage;

        public StartPage()
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
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            App.Data = new TriviaModel();

            var triviaSteps = await this.GetTriviaData();
            foreach (var step in triviaSteps)
            {
                App.Data.Steps.Add(step);
            }

            App.Data.PlayerName = PlayerName.Text;

            this.rootPage.GetFrameContent().Navigate(typeof(TriviaStep), this.rootPage);
        }

        private async Task<IEnumerable<TriviaStepModel>> GetTriviaData()
        {
            var triviaData = await this.GetTriviaJsonData();
            
            var questions = triviaData.GetNamedArray("questions");

            var selectedQuestions = new List<int>();
            this.GetRandomQuestions(selectedQuestions, questions);

            var result = new List<TriviaStepModel>();
            foreach (var selectedQuestion in selectedQuestions)
            {
                var triviaStepModel = new TriviaStepModel 
                {
                    Question = questions.GetObjectAt((uint)selectedQuestion)["title"].GetString(),
                    CorrectAnswer = (int)questions.GetObjectAt((uint)selectedQuestion)["correctAnswer"].GetNumber()
                };

                triviaStepModel.Answers.Add(new AnswerModel 
                { 
                    Answer = questions.GetObjectAt((uint)selectedQuestion)["answer1"].GetString() 
                });

                triviaStepModel.Answers.Add(new AnswerModel
                {
                    Answer = questions.GetObjectAt((uint)selectedQuestion)["answer2"].GetString()
                }); 

                triviaStepModel.Answers.Add(new AnswerModel
                {
                    Answer = questions.GetObjectAt((uint)selectedQuestion)["answer3"].GetString()
                });

                result.Add(triviaStepModel);
            }

            return result;
        }

        private void GetRandomQuestions(List<int> selectedQuestions, JsonArray questions)
        {
            var generator = new Random();
            for (int i = 0; i < Constants.QuestionsCount; i++)
            {
                var questionIndex = generator.Next(0, questions.Count);
                while (selectedQuestions.Contains(questionIndex))
                {
                    questionIndex = generator.Next(0, questions.Count);
                }

                selectedQuestions.Add(questionIndex);
            }
        }

        private async Task<JsonObject> GetTriviaJsonData()
        {
            var path = @"Data\Data.json";
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var file = await folder.GetFileAsync(path);
            var jsonStr = await Windows.Storage.FileIO.ReadTextAsync(file);

            return JsonObject.Parse(jsonStr);
        }

        private void PlayerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            StartButton.IsEnabled = !string.IsNullOrEmpty(PlayerName.Text);
        }
    }
}
