using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Cornball.Common;
using Cornball.Resources;
using Cornball.Statistics;

namespace Cornball
{
    public partial class Main
    {
        private readonly DispatcherTimer _timer;
        private Queue<Deal> _deals;
        private Dialog _dialog;
        private Game _game;

        public Main()
        {
            InitializeComponent();
            BindSoundButton();

            Table.Width = (Game.CardWidth + Game.Margin)*Game.Columns + Game.Margin + Game.Padding*2;
            Table.Height = (Game.CardHeight + Game.Margin)*Game.Rows + Game.Margin + Game.Padding*2;

            _timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(20)};
            _timer.Tick += DealCard;

            HighScore.Text = String.Format(Strings.HighestScoreLoading, DateTime.Today.ToString("MMMM"));
            NewGame();
        }

        private void BindSoundButton()
        {
            SoundEnabledButton.Visibility = (Settings.SoundEnabled ? Visibility.Visible : Visibility.Collapsed);
            SoundDisabledButton.Visibility = (!Settings.SoundEnabled ? Visibility.Visible : Visibility.Collapsed);
        }

        #region Game methods

        private void NewGame()
        {
            GameStarted();

            _game = new Game();
            _game.Win += GameWin;
            _game.Lose += GameLose;
            _game.ReShuffle += GameReShuffle;

            UpdateHighscore();
            InitializeCards();
            RemoveCards();
            DrawCards();
        }

        private void NewRound(object sender, EventArgs e)
        {
            _game.Deck.ReShuffle();
            UpdateHighscore();
            DrawCards();
        }

        private void InitializeCards()
        {
            foreach (Card card in _game.Deck)
            {
                card.DoubleClick += CardDoubleClick;
                card.HintClick += CardHintClick;
                card.Dropped += CardDropped;
                card.Moved += CardMoved;
            }
        }

        private void DrawCards()
        {
            _deals = new Queue<Deal>();
            var start = new Point(0, 0);

            for (int row = 1; row <= Game.Rows; row++)
            {
                for (int column = 1; column <= Game.Columns; column++)
                {
                    Card card = _game.Deck[((row - 1)*Game.Columns) + column - 1];
                    if (!Table.Children.Contains(card))
                    {
                        var target =
                            new Point(Game.Padding + Game.Margin + ((column - 1)*(Game.CardWidth + Game.Margin)),
                                      Game.Padding + Game.Margin + ((row - 1)*(Game.CardHeight + Game.Margin)));
                        _deals.Enqueue(new Deal(card, start, target));
                    }
                }
            }

            _timer.Start();
            _game.LastMove = null;
            _game.CheckState();
            UpdateStatus();
            Sound.Play(Sounds.Shuffle);
        }

        private void DealCard(object sender, EventArgs e)
        {
            if (_deals.Count > 0)
            {
                Deal deal = _deals.Dequeue();
                Table.Children.Add(deal.Card);
                deal.Card.Animate(new Point(0, 0), deal.Target, false);
            }
            else
            {
                _timer.Stop();
            }
        }

        private void HideCards()
        {
            for (int i = Table.Children.Count - 1; i >= 0; i--)
            {
                if (((Card) Table.Children[i]).RoundPlaced == null)
                {
                    Table.Children.RemoveAt(i);
                }
            }
        }

        private void RemoveCards()
        {
            for (int i = Table.Children.Count - 1; i >= 0; i--)
            {
                if (Table.Children[i] is Card)
                {
                    Table.Children.RemoveAt(i);
                }
            }
        }

        private void UpdateHighscore()
        {
            var statistics = new StatisticsClient();
            statistics.GetHighscoresCompleted += GetHighscoreCompleted;
            statistics.GetHighscoresAsync(1, new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), null);
        }

        private void GetHighscoreCompleted(object sender, GetHighscoresCompletedEventArgs e)
        {
            if (e.Result != null && e.Result.Count > 0)
            {
                DataItem score = e.Result.First();
                HighScore.Text = String.Format(Strings.HighestScore, DateTime.Today.ToString("MMMM"), score.Value,
                                               score.Name);
            }
            else
            {
                HighScore.Text = String.Format(Strings.HighestScore, DateTime.Today.ToString("MMMM"), "-", "-");
            }
        }

        private void UpdateStatus()
        {
            Status.Text = String.Format("{0}: {1}/{2}     {3}: {4:N0}     {5}: {6}", Strings.Round, _game.Round,
                                        Game.Rounds,
                                        Strings.Score, _game.Score, Strings.Moves, _game.Moves);
        }

        #endregion

        #region Game events

        private void CardHintClick(object sender, EventArgs e)
        {
            List<Card> cards = _game.FindCardsForGap((Card) sender);
            foreach (Card card in cards)
            {
                card.FlashHint();
            }
            Sound.Play(cards.Count > 0 ? Sounds.Hint : Sounds.Error);
        }

        private void CardDoubleClick(object sender, EventArgs e)
        {
            if (!PerformMove((Card) sender, _game.FindGapForCard((Card) sender)))
            {
                ((Card) sender).FlashError();
                Sound.Play(Sounds.Error);
            }
            else
            {
                Sound.Play(Sounds.Moved);
            }
        }

        private void CardDropped(object sender, EventArgs e)
        {
            if (!PerformMove((Card) sender, _game.FindIntersectingGap((Card) sender)))
            {
                ((Card) sender).CancelMove();
                Sound.Play(Sounds.Error);
            }
            else
            {
                Sound.Play(Sounds.Moved);
            }
        }

        private void CardMoved(object sender, EventArgs e)
        {
            _game.CheckState();
            UpdateStatus();
        }

        private bool PerformMove(Card from, Card to)
        {
            if (from != null && to != null)
            {
                _game.LastMove = new Move(from, to);
                _game.ExecuteMove(from, to);
                return true;
            }
            return false;
        }

        private void GameReShuffle(object sender, EventArgs e)
        {
            HideCards();
            Sound.Play(Sounds.RoundEnd);
            int roundsLeft = (Game.Rounds - _game.Round) + 1;
            Dialog.Show(LayoutRoot, String.Format(Strings.RoundEndTitle, _game.Round - 1),
                        String.Format((roundsLeft == 1 ? Strings.LastRoundEndText : Strings.RoundEndText),
                                      _game.Score.ToString("N0"), _game.Moves.ToString("N0"), roundsLeft), NewRound);
        }

        private void GameLose(object sender, EventArgs e)
        {
            GameLost();
            _game.LastMove = null;
            HideCards();
            Sound.Play(Sounds.GameLost);
            Dialog.Show(LayoutRoot, Strings.GameLoseTitle,
                        String.Format(Strings.GameLoseText, _game.Score.ToString("N0"), _game.Moves.ToString("N0")),
                        null);
        }

        private void GameWin(object sender, EventArgs e)
        {
            GameWon(_game.Round);
            _game.LastMove = null;
            Sound.Play(Sounds.GameWon);

            var statistics = new StatisticsClient();
            statistics.IsHighscoreCompleted += IsHighscoreCompleted;
            statistics.IsHighscoreAsync(_game.Score, Game.HighScore, DateTime.Today.Day);

            var textBlock = new TextBlock
                                {
                                    Style = Styles.BodyText,
                                    Text = String.Format(Strings.GameWinText, _game.Round, _game.Score.ToString("N0"),
                                                         _game.Moves.ToString("N0")) + "\n\n" + Strings.GameWinChecking
                                };

            _dialog = new Dialog
                          {
                              DialogContents = {Content = new StackPanel()},
                              Header = {Text = Strings.GameWinTitle}
                          };
            _dialog.Closed += SubmitHighScore;
            ((StackPanel) _dialog.DialogContents.Content).Children.Add(textBlock);
            _dialog.Show(LayoutRoot);
        }

        private void IsHighscoreCompleted(object sender, IsHighscoreCompletedEventArgs e)
        {
            var textBlock = ((StackPanel) _dialog.DialogContents.Content).Children[0] as TextBlock;
            if (textBlock != null)
            {
                if (e.Result)
                {
                    textBlock.Text =
                        String.Format(Strings.GameWinText, _game.Round, _game.Score.ToString("N0"),
                                      _game.Moves.ToString("N0")) + "\n\n" +
                        Strings.GameWinHigh;
                    var textBox = new TextBox {Style = Styles.TextBox, MaxLength = 50};
                    ((StackPanel) _dialog.DialogContents.Content).Children.Add(textBox);
                }
                else
                {
                    textBlock.Text =
                        String.Format(Strings.GameWinText, _game.Round, _game.Score.ToString("N0"),
                                      _game.Moves.ToString("N0")) + "\n\n" +
                        Strings.GameWinNoHigh;
                }
            }
        }

        private void SubmitHighScore(object sender, EventArgs e)
        {
            if (((StackPanel) _dialog.DialogContents.Content).Children.Count > 1)
            {
                var player = ((StackPanel) _dialog.DialogContents.Content).Children[1] as TextBox;
                if (player != null)
                {
                    if (!String.IsNullOrEmpty(player.Text))
                    {
                        SaveScore(player.Text, _game.Score);
                    }
                }
            }
        }

        private void GridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double scaleY = e.NewSize.Height/Table.Height;
            double scaleX = e.NewSize.Width/Table.Width;
            scaleY = (scaleY < 0.5 ? 0.5 : (scaleY > 1 ? 1 : scaleY));
            scaleX = (scaleX < 0.5 ? 0.5 : (scaleX > 1 ? 1 : scaleX));
            TableScale.ScaleY = TableScale.ScaleX = Game.Scale = (scaleY > scaleX ? scaleX : scaleY);
        }

        #endregion

        #region Menu events

        private void MenuMouseEnter(object sender, MouseEventArgs e)
        {
            ((FrameworkElement) sender).BeginAnimation(OpacityProperty,
                                                       new DoubleAnimation
                                                           {
                                                               Duration = new TimeSpan(0, 0, 0, 0, 250),
                                                               To = 1.0
                                                           });
        }

        private void MenuMouseLeave(object sender, MouseEventArgs e)
        {
            ((FrameworkElement) sender).BeginAnimation(OpacityProperty,
                                                       new DoubleAnimation
                                                           {
                                                               Duration = new TimeSpan(0, 0, 0, 0, 250),
                                                               To = 0.5
                                                           });
        }

        private void NewClick(object sender, RoutedEventArgs e)
        {
            NewGame();
        }

        private void UndoClick(object sender, RoutedEventArgs e)
        {
            if (_game.LastMove != null)
            {
                Sound.Play(Sounds.Moved);
                PerformMove(_game.LastMove.From, _game.LastMove.To);
                _game.LastMove = null;
            }
            else
            {
                Sound.Play(Sounds.Error);
            }
        }

        private void SoundClick(object sender, RoutedEventArgs e)
        {
            Settings.SoundEnabled = !Settings.SoundEnabled;
            Sound.Play(Sounds.Click);
            BindSoundButton();
        }

        private void InfoClick(object sender, RoutedEventArgs e)
        {
            Sound.Play(Sounds.Click);
            var assemblyName = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
            var stackPanel = new StackPanel();
            var textBlock = new TextBlock();
            var image = new Image();

            textBlock.Style = Styles.BodyText;
            textBlock.Text = Strings.AboutText;
            stackPanel.Children.Add(textBlock);

            image.Source = new BitmapImage(new Uri("/Cornball;component/Resources/PoweredByAzure.png", UriKind.Relative));
            image.HorizontalAlignment = HorizontalAlignment.Center;
            image.Width = 300;
            image.Margin = new Thickness(10);
            stackPanel.Children.Add(image);

            Dialog.Show(LayoutRoot, String.Format("{0} ({1})", Strings.Cornball, assemblyName.Version), stackPanel, null);
        }

        #endregion

        #region High Score

        private void HighScoreClick(object sender, RoutedEventArgs e)
        {
            Sound.Play(Sounds.Click);
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(300, GridUnitType.Pixel)});
            grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(250, GridUnitType.Pixel)});

            _dialog = new Dialog {DialogContents = {Content = grid}, Header = {Text = Strings.HighScore}};
            _dialog.Loading(LayoutRoot);

            var statistics = new StatisticsClient();
            statistics.GetHighscoresCompleted += GetAllTimeHighCompleted;
            statistics.GetHighscoresAsync(Game.HighScore, null, null);
        }

        private void GetAllTimeHighCompleted(object sender, GetHighscoresCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                var grid = (Grid) _dialog.DialogContents.Content;
                var panel = new StackPanel();
                panel.SetValue(Grid.RowProperty, 0);
                panel.SetValue(Grid.ColumnProperty, 0);
                grid.Children.Add(panel);

                var header = new TextBlock
                                 {
                                     Style = Styles.StrongText,
                                     Margin = new Thickness(0, 0, 0, 5),
                                     Text = Strings.HighScoreAllTime
                                 };
                panel.Children.Add(header);

                int place = 1;
                foreach (DataItem score in e.Result)
                {
                    AddHighScore(panel, place, score.Name, score.Value);
                    place++;
                }
            }
            var statistics = new StatisticsClient();
            statistics.GetHighscoresCompleted += GetMonthHighCompleted;
            statistics.GetHighscoresAsync(Game.HighScore, new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                                          null);
        }

        private void GetMonthHighCompleted(object sender, GetHighscoresCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                var grid = (Grid) _dialog.DialogContents.Content;
                var panel = new StackPanel();
                panel.SetValue(Grid.RowProperty, 0);
                panel.SetValue(Grid.ColumnProperty, 1);
                grid.Children.Add(panel);

                var header = new TextBlock
                                 {
                                     Style = Styles.StrongText,
                                     Text = String.Format(Strings.HighScoreMonth, DateTime.Today.ToString("MMMM")),
                                     Margin = new Thickness(0, 0, 0, 5)
                                 };
                panel.Children.Add(header);

                int place = 1;
                foreach (DataItem score in e.Result)
                {
                    AddHighScore(panel, place, score.Name, score.Value);
                    place++;
                }
            }
            _dialog.LoadingCompleted();
        }

        private void AddHighScore(StackPanel parent, int place, string name, int score)
        {
            var panel = new StackPanel {Orientation = Orientation.Horizontal};
            parent.Children.Add(panel);

            var placeText = new TextBlock
                                {
                                    Style = Styles.BodyText,
                                    Width = 25,
                                    Text = place + "."
                                };
            panel.Children.Add(placeText);

            var headerText = new TextBlock
                                 {
                                     Style = Styles.BodyText,
                                     Width = 175,
                                     Text = name
                                 };
            panel.Children.Add(headerText);

            var scoreText = new TextBlock
                                {
                                    TextAlignment = TextAlignment.Right,
                                    Style = Styles.BodyText,
                                    Width = 50,
                                    Text = score.ToString("N0")
                                };
            panel.Children.Add(scoreText);
        }

        internal static void SaveScore(string name, int score)
        {
            var statistics = new StatisticsClient();
            statistics.SaveHighscoreAsync(name, score);
        }

        #endregion

        #region Statistics

        internal const string GamesWonRound = "GamesWonRound";
        internal const string GamesStarted = "GamesStarted";
        internal const string GamesLost = "GamesLost";

        private void StatsClick(object sender, RoutedEventArgs e)
        {
            Sound.Play(Sounds.Click);
            var statistics = new StatisticsClient();
            statistics.GetStatisticsCompleted += GetStatisticsCompleted;
            statistics.GetStatisticsAsync();

            _dialog = new Dialog {DialogContents = {Content = new StackPanel()}, Header = {Text = Strings.Statistics}};
            _dialog.Loading(LayoutRoot);
        }

        private void GetStatisticsCompleted(object sender, GetStatisticsCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                var panel = (StackPanel) _dialog.DialogContents.Content;
                panel.Children.Clear();

                Dictionary<string, int> values = e.Result.ToDictionary(value => value.Name, value => value.Value);
                int played = values[GamesLost];
                for (int i = 1; i <= Game.Rounds; i++)
                {
                    played += values[GamesWonRound + i];
                }
                AddStatistics(Strings.StatisticsPlayed, played, null);
                AddStatistics(Strings.StatisticsLost, values[GamesLost], values[GamesLost]/(decimal) played*100);
                for (int i = 1; i <= Game.Rounds; i++)
                {
                    AddStatistics(String.Format(Strings.StatisticsWonRound, i), values[GamesWonRound + i],
                                  values[GamesWonRound + i]/(decimal) played*100);
                }
            }
            _dialog.LoadingCompleted();
        }

        private void AddStatistics(string header, int value, decimal? percent)
        {
            var parent = (StackPanel) _dialog.DialogContents.Content;
            var panel = new StackPanel {Orientation = Orientation.Horizontal};
            parent.Children.Add(panel);

            var headerText = new TextBlock
                                 {
                                     Style = Styles.BodyText,
                                     Width = 150,
                                     Text = header
                                 };
            panel.Children.Add(headerText);

            var valueText = new TextBlock
                                {
                                    TextAlignment = TextAlignment.Right,
                                    Style = Styles.BodyText,
                                    Width = 150,
                                    Text = value.ToString("N0")
                                };
            panel.Children.Add(valueText);

            if (percent.HasValue)
            {
                var percentText = new TextBlock
                                      {
                                          TextAlignment = TextAlignment.Right,
                                          Style = Styles.BodyText,
                                          Width = 150,
                                          Text = percent.Value.ToString("F2") + " %"
                                      };
                panel.Children.Add(percentText);
            }
        }

        internal static void GameStarted()
        {
            IncreaseValue(GamesStarted, 1);
        }

        internal static void GameLost()
        {
            IncreaseValue(GamesLost, 1);
        }

        internal static void GameWon(int round)
        {
            IncreaseValue(GamesWonRound + round, 1);
        }

        private static void IncreaseValue(string name, int value)
        {
            var statistics = new StatisticsClient();
            statistics.IncreaseValueAsync(name, value);
        }

        #endregion
    }
}