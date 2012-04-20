using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Cornball
{
    public partial class Card
    {
        private bool _isMouseCaptured;
        private int? _roundPlaced;

        public Card(Suit suit, Value value)
        {
            InitializeComponent();
            Height = Game.CardHeight;
            Width = Game.CardWidth;
            RoundPlaced = null;
            Value = value;
            Suit = suit;

            if (Value != Value.Ace)
            {
                var uri = new Uri("/Cornball;component/Resources/Deck/" + Suit + (int) Value + ".png", UriKind.Relative);
                CardImage.Source = new BitmapImage(uri);

                MouseMove += CardMouseMove;
                MouseLeftButtonUp += CardMouseLeftButtonUp;
                MouseRightButtonDown += CardMouseRightButtonDown;
            }
            MouseLeftButtonDown += CardMouseLeftButtonDown;
            Cursor = Cursors.Hand;
        }

        internal Point Original { get; private set; }
        internal Point Start { get; private set; }
        public Suit Suit { get; private set; }
        public Value Value { get; private set; }

        private bool IsMouseCaptured
        {
            get { return _isMouseCaptured; }
            set
            {
                if (value)
                {
                    if (!CaptureMouse())
                    {
                        IsMouseCaptured = false;
                    }
                    Canvas.SetZIndex(this, 2);
                }
                else
                {
                    ReleaseMouseCapture();
                    Canvas.SetZIndex(this, 1);
                }
                _isMouseCaptured = value;
            }
        }

        internal int? RoundPlaced
        {
            get { return _roundPlaced; }
            set
            {
                InOrderRect.Visibility = (value != null ? Visibility.Visible : Visibility.Collapsed);
                _roundPlaced = value;
                if (value > 0)
                {
                    Sound.Play(Sounds.Correct);
                }
            }
        }

        internal Point Location
        {
            get { return new Point((double) GetValue(Canvas.LeftProperty), (double) GetValue(Canvas.TopProperty)); }
            private set
            {
                SetValue(Canvas.TopProperty, value.Y);
                SetValue(Canvas.LeftProperty, value.X);
            }
        }

        internal Rect Bounds
        {
            get { return new Rect(Location, new Size(Game.CardWidth, Game.CardHeight)); }
        }

        public event EventHandler DoubleClick;
        public event EventHandler HintClick;
        public event EventHandler Dropped;
        public event EventHandler Moved;

        private void CardMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Value != Value.Ace)
            {
                switch (e.ClickCount)
                {
                    case 1:
                        Original = new Point((double) GetValue(Canvas.LeftProperty),
                                             (double) GetValue(Canvas.TopProperty));
                        Start = new Point(e.GetPosition(null).X/Game.Scale, e.GetPosition(null).Y/Game.Scale);
                        IsMouseCaptured = true;
                        break;
                    case 2:
                        IsMouseCaptured = false;
                        if (Original == Location)
                        {
                            if (DoubleClick != null)
                            {
                                DoubleClick(this, EventArgs.Empty);
                            }
                        }
                        break;
                }
            }
            else
            {
                if (HintClick != null)
                {
                    HintClick(this, EventArgs.Empty);
                }
            }
        }

        private void CardMouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                double mouseX = e.GetPosition(null).X/Game.Scale;
                double mouseY = e.GetPosition(null).Y/Game.Scale;
                Location = new Point(mouseX - Start.X + Location.X, mouseY - Start.Y + Location.Y);
                Start = new Point(mouseX, mouseY);
            }
        }

        private void CardMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                if (Original != Location && Dropped != null)
                {
                    Dropped(this, EventArgs.Empty);
                }
                IsMouseCaptured = false;
            }
        }

        private void CardMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        public void ExecuteMove(Card target)
        {
            Point temp = (IsMouseCaptured ? Original : Location);
            Animate(Location, target.Location, true);
            target.Location = temp;
        }

        public void CancelMove()
        {
            Animate(Location, Original, false);
        }

        internal void Animate(Point from, Point to, bool movedEvent)
        {
            Canvas.SetZIndex(this, 1);
            TimeSpan duration = TimeSpan.FromMilliseconds(250);
            var backEase = new BackEase {Amplitude = 0.2, EasingMode = EasingMode.EaseOut};

            var x = new DoubleAnimation
                        {
                            Duration = duration,
                            From = from.X,
                            To = to.X,
                            EasingFunction = backEase
                        };
            Storyboard.SetTargetProperty(x, new PropertyPath(Canvas.LeftProperty));
            Storyboard.SetTarget(x, this);

            var y = new DoubleAnimation
                        {
                            Duration = duration,
                            From = from.Y,
                            To = to.Y,
                            EasingFunction = backEase
                        };
            Storyboard.SetTargetProperty(y, new PropertyPath(Canvas.TopProperty));
            Storyboard.SetTarget(y, this);

            var storyboard = new Storyboard();
            storyboard.Children.Add(y);
            storyboard.Children.Add(x);

            if (movedEvent)
            {
                storyboard.Completed += MoveCompleted;
            }
            else
            {
                storyboard.Completed += MoveCompletedSilent;
            }
            storyboard.Begin();
        }

        private void MoveCompletedSilent(object sender, EventArgs e)
        {
            Canvas.SetZIndex(this, 0);
        }

        private void MoveCompleted(object sender, EventArgs e)
        {
            if (Moved != null)
            {
                Moved(this, EventArgs.Empty);
            }
            Canvas.SetZIndex(this, 0);
        }

        public void FlashHint()
        {
            Hint.Begin();
        }

        public void FlashError()
        {
            Error.Begin();
        }
    }

    public enum Suit
    {
        Clubs = 1,
        Diamonds = 2,
        Hearts = 3,
        Spades = 4
    }

    public enum Value
    {
        Ace = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13
    }
}