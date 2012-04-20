using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Cornball {
	public partial class HighScore : UserControl {
		public HighScore() {
			InitializeComponent();
		}

		public void Show() {
			Header.Text = String.Format("{0} ({1})", Strings.HighScore, Strings.Loading);

			Statistics.StatisticsClient statistics = new Statistics.StatisticsClient();
			statistics.GetScoresCompleted += new EventHandler<Statistics.GetScoresCompletedEventArgs>(GetScoresCompleted);
			statistics.GetScoresAsync(Game.HighScore);

			Values.Visibility = Visibility.Collapsed;
			Visibility = Visibility.Visible;
		}

		private void Ok_Click(object sender, RoutedEventArgs e) {
			Visibility = Visibility.Collapsed;
		}

		private void GetScoresCompleted(object sender, Statistics.GetScoresCompletedEventArgs e) {
			if(e.Result != null) {
				Values.Visibility = Visibility.Visible;
				Values.Children.Clear();

				int place = 1;
				Header.Text = Strings.HighScore;
				foreach(Statistics.DataItem score in e.Result) {
					AddStatistics(place, score.Name, score.Value);
					place++;
				}
			}
		}

		private void AddStatistics(int place, string name, int score) {
			StackPanel panel = new StackPanel();
			panel.Orientation = Orientation.Horizontal;
			Values.Children.Add(panel);

			TextBlock placeText = new TextBlock();
			placeText.Style = (Style)App.Current.Resources["BodyText"];
			placeText.Width = 30;
			placeText.Text = place + ".";
			panel.Children.Add(placeText);

			TextBlock headerText = new TextBlock();
			headerText.Style = (Style)App.Current.Resources["BodyText"];
			headerText.Width = 150;
			headerText.Text = name;
			panel.Children.Add(headerText);

			TextBlock scoreText = new TextBlock();
			scoreText.TextAlignment = TextAlignment.Right;
			scoreText.Style = (Style)App.Current.Resources["BodyText"];
			scoreText.Width = 150;
			scoreText.Text = score.ToString("N0");
			panel.Children.Add(scoreText);
		}

		internal static void SaveScore(string name, int score) {
			Statistics.StatisticsClient statistics = new Statistics.StatisticsClient();
			statistics.SaveScoreAsync(name, score);
		}
	}
}