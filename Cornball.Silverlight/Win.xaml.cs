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
	public partial class Win : UserControl {
		private int m_Score = 0;

		public Win() {
			InitializeComponent();
		}

		public void Show(int score) {
			m_Score = score;
			Header.Text = Strings.GameWinTitle;
			Message.Text = Strings.GameWinText + "\n\n" + Strings.GameWinChecking;

			Statistics.StatisticsClient statistics = new Statistics.StatisticsClient();
			statistics.IsHighScoreCompleted += new EventHandler<Statistics.IsHighScoreCompletedEventArgs>(IsHighScoreCompleted);
			statistics.IsHighScoreAsync(m_Score, Game.HighScore);

			Player.Visibility = Visibility.Collapsed;
			Visibility = Visibility.Visible;
		}

		private void IsHighScoreCompleted(object sender, Statistics.IsHighScoreCompletedEventArgs e) {
			if(e.Result) {
				Message.Text = Strings.GameWinText + "\n\n" + Strings.GameWinHigh;
				Player.Visibility = Visibility.Visible;
			}
			else {
				Message.Text = Strings.GameWinText + "\n\n" + Strings.GameWinNoHigh;
				Player.Visibility = Visibility.Collapsed;
			}
		}

		private void Ok_Click(object sender, RoutedEventArgs e) {
			if(Player.Visibility == Visibility.Visible && !String.IsNullOrEmpty(Player.Text)) {
				SaveScore(Player.Text, m_Score);
			}
			Visibility = Visibility.Collapsed;
		}
	}
}