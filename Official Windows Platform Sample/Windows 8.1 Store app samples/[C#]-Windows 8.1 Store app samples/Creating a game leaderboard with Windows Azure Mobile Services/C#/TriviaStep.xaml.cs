// Copyright (c) Microsoft Corporation. All rights reserved 

namespace AzureMobileLeaderboard
{
    using System.Linq;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TriviaStep : Page
    {
        private int stepNumber = -1;
        private MainPage rootPage;
        private Model.TriviaStepModel currentStepModel;
        private bool disableButtons;

        public TriviaStep()
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

            this.stepNumber = 1;
            this.RefreshData();
        }

        private void RefreshData()
        {
            this.ResetButtonsBackground();
            this.currentStepModel = App.Data.Steps.ElementAt(this.stepNumber - 1);
            this.DataContext = this.currentStepModel;
            this.AnswerA.Content = this.currentStepModel.Answers[0].Answer;
            this.AnswerB.Content = this.currentStepModel.Answers[1].Answer;
            this.AnswerC.Content = this.currentStepModel.Answers[2].Answer;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            this.disableButtons = false;

            if (this.stepNumber == Constants.QuestionsCount)
            {
                this.rootPage.GetFrameContent().Navigate(typeof(ResultsPage), this.rootPage);
                return;
            }
            
            if (this.stepNumber == Constants.QuestionsCount - 1)
            {
                this.NextButton.Content = "Finish";
            }

            this.NextButton.IsEnabled = false;
            this.stepNumber++;
            this.RefreshData();
        }

        private void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!this.disableButtons)
            {
                this.disableButtons = true;
                var selectedButton = sender as Button;
                var correctAnswer = this.currentStepModel.Answers[this.currentStepModel.CorrectAnswer - 1].Answer;

                if (selectedButton.Content as string != correctAnswer)
                {
                    selectedButton.Background = new SolidColorBrush(Colors.Red);
                    switch (this.currentStepModel.CorrectAnswer)
                    {
                        case 1:
                            this.AnswerA.Background = new SolidColorBrush(Colors.Green);
                            break;
                        case 2:
                            this.AnswerB.Background = new SolidColorBrush(Colors.Green);
                            break;
                        case 3:
                            this.AnswerC.Background = new SolidColorBrush(Colors.Green);
                            break;
                    }
                }
                else
                {
                    selectedButton.Background = new SolidColorBrush(Colors.Green);
                }

                var selectedButtonText = (e.OriginalSource as Button).Content;
                var answer = this.currentStepModel.Answers.Single(a => a.Answer.Equals(selectedButtonText));

                if (answer != null)
                {
                    this.NextButton.IsEnabled = true;
                    answer.IsSelected = true;
                }
            }
        }

        private void ResetButtonsBackground()
        {
            this.AnswerA.Background = null;
            this.AnswerB.Background = null;
            this.AnswerC.Background = null;
        }
    }
}
