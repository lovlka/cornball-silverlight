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
	public partial class Stats : UserControl {
		internal const string GamesWonRound = "games_won_round";
		internal const string GamesStarted = "games_started";
		internal const string GamesLost = "games_lost";

		public Stats() {
			InitializeComponent();
		}

		public void Show() {
			Header.Text = String.Format("{0} ({1})", Strings.Statistics, Strings.Loading);

			Statistics.StatisticsClient statistics = new Statistics.StatisticsClient();
			statistics.GetValuesCompleted += new EventHandler<Statistics.GetValuesCompletedEventArgs>(GetValuesCompleted);
			statistics.GetValuesAsync();

			Values.Visibility = Visibility.Collapsed;
			Visibility = Visibility.Visible;
		}

		private void Ok_Click(object sender, RoutedEventArgs e) {
			Visibility = Visibility.Collapsed;
		}

		private void GetValuesCompleted(object sender, Statistics.GetValuesCompletedEventArgs e) {
			if(e.Result != null) {
				Values.Visibility = Visibility.Visible;
				Values.Children.Clear();

				Dictionary<string, int> values = new Dictionary<string, int>();
				foreach(Statistics.DataItem value in e.Result) {
					values.Add(value.Name, value.Value);
				}
				Header.Text = Strings.Statistics;
				int played = values[GamesLost];
				for(int i = 1; i <= Game.Rounds; i++) {
					played += values[GamesWonRound + i];
				}
				AddStatistics(Strings.StatisticsPlayed, played, null);
				AddStatistics(Strings.StatisticsLost, values[GamesLost], (decimal)values[GamesLost] / (decimal)played * 100);
				for(int i = 1; i <= Game.Rounds; i++) {
					AddStatistics(String.Format(Strings.StatisticsWonRound, i), values[GamesWonRound + i], (decimal)values[GamesWonRound + i] / (decimal)played * 100);
				}
			}
		}

		private void AddStatistics(string header, int value, decimal? percent) {
			StackPanel panel = new StackPanel();
			panel.Orientation = Orientation.Horizontal;
			Values.Children.Add(panel);

			TextBlock headerText = new TextBlock();
			headerText.Style = (Style)App.Current.Resources["BodyText"];
			headerText.Width = 150;
			headerText.Text = header;
			panel.Children.Add(headerText);

			TextBlock valueText = new TextBlock();
			valueText.TextAlignment = TextAlignment.Right;
			valueText.Style = (Style)App.Current.Resources["BodyText"];
			valueText.Width = 150;
			valueText.Text = value.ToString("N0");
			panel.Children.Add(valueText);

			if(percent.HasValue) {
				TextBlock percentText = new TextBlock();
				percentText.TextAlignment = TextAlignment.Right;
				percentText.Style = (Style)App.Current.Resources["BodyText"];
				percentText.Width = 150;
				percentText.Text = percent.Value.ToString("F2") + " %";
				panel.Children.Add(percentText);
			}
		}

		internal static void GameStarted() {
			IncreaseValue(GamesStarted, 1);
		}

		internal static void GameLost() {
			IncreaseValue(GamesLost, 1);
		}

		internal static void GameWon(int round) {
			IncreaseValue(GamesWonRound + round, 1);
		}

		private static void IncreaseValue(string name, int value) {
			Statistics.StatisticsClient statistics = new Statistics.StatisticsClient();
			statistics.IncreaseValueAsync(name, value);
		}
	}
}