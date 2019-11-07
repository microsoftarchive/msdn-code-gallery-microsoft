//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved
using Windows.UI.Xaml.Controls;

namespace InkingDemo
{
    public sealed partial class MainPage : Page
    {
        class Problem : Common.BindableBase
        {
            private System.Random randomGenerator;

            private int term1;
            public int Term1
            {
                get { return term1; }
                private set { SetProperty(ref term1, value); }
            }

            private int result;
            public int Result
            {
                get { return result; }
                private set { SetProperty(ref result, value); }
            }

            public Problem()
            {
                this.randomGenerator = new System.Random();
                this.GenerateNew();
            }

            public void GenerateNew()
            {
                this.Term1 = this.randomGenerator.Next(0, 10);
                this.Result = this.Term1 + this.randomGenerator.Next(0, 10);
            }
        }

        Problem problem;

        public MainPage()
        {
            this.InitializeComponent();

            this.problem = new Problem();
            this.DataContext = this.problem;

            this.EnableInput(true);
            this.inkingPanel.ProcessInputOnDelegateThread();
        }

        private void EnableInput(bool enable)
        {
            foreach (var child in this.stackPanelN.Children)
            {
                var button = child as Button;
                if (null != button)
                {
                    button.IsEnabled = enable;
                }
            }
            foreach (var child in this.stackPanelS.Children)
            {
                var button = child as Button;
                if (null != button)
                {
                    button.IsEnabled = enable;
                }
            }
            if (enable)
            {
                this.inkingPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                this.textBlockAnswer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                this.textBlockAnswer.Visibility = Windows.UI.Xaml.Visibility.Visible;
                this.inkingPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void OnTouchAnswer(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var btnSender = sender as Button;
            this.VerifyAnswer(btnSender.Content as string);
        }

        private void OnInkAnswer(object sender, string answer)
        {
            this.VerifyAnswer(answer);
        }

        private void VerifyAnswer(string answer)
        {
            bool generateNew = false;

            // Disable input methods for the time we are displaying the result
            this.EnableInput(false);
            try
            {
                var intAnswer = int.Parse(answer);
                if (intAnswer == this.problem.Result - this.problem.Term1)
                {
                    this.statusMessage.Foreground = this.textBlockAnswer.Foreground =
                        new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Green);
                    this.statusMessage.Text = "Good Job!";
                    generateNew = true;
                }
                else
                {
                    this.statusMessage.Foreground = this.textBlockAnswer.Foreground =
                        new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Red);
                    this.statusMessage.Text = "Try Again.";
                }
                this.textBlockAnswer.Text = answer;
            }
            catch (System.Exception e)
            {
                // Unexpected
                System.Diagnostics.Debug.Assert(true, e.Message);
            }

            // Wait 1s then clear result and generate new problem if necessary
            System.TimeSpan delay = System.TimeSpan.FromSeconds(1);
            Windows.System.Threading.ThreadPoolTimer.CreateTimer(
                (Windows.System.Threading.ThreadPoolTimer timer) =>
            {
                // Update the UI thread by using the UI core dispatcher
                var asyncAction = Dispatcher.RunAsync(
                    Windows.UI.Core.CoreDispatcherPriority.High,
                    () =>
                    {
                        if (generateNew)
                        {
                            this.problem.GenerateNew();
                        }
                        this.statusMessage.Text = "";
                        this.inkingPanel.Clear();
                        this.EnableInput(true);
                    });
            }, delay);
        }
    }
}
