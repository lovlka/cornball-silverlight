using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Cornball
{
    public partial class Dialog
    {
        private bool _wait;

        public Dialog()
        {
            InitializeComponent();
        }

        public event EventHandler Closed;

        public void Show(Panel parent)
        {
            ContentBorder.Visibility = Visibility.Collapsed;
            parent.Children.Add(this);
            FadeInBackground.Begin();
        }

        public void Loading(Panel parent)
        {
            ContentBorder.Visibility = Visibility.Collapsed;
            parent.Children.Add(this);
            FadeInBackground.Begin();
            _wait = true;
        }

        public void LoadingCompleted()
        {
            ContentBorder.Visibility = Visibility.Visible;
            ScaleUpContent.Begin();
        }

        private void FadeInBackgroundCompleted(object sender, EventArgs e)
        {
            if (!_wait)
            {
                LoadingCompleted();
            }
        }

        private void ScaleDownContentCompleted(object sender, EventArgs e)
        {
            ContentBorder.Visibility = Visibility.Collapsed;
            FadeOutBackground.Begin();
        }

        private void FadeOutBackgroundCompleted(object sender, EventArgs e)
        {
            if (Closed != null)
            {
                Closed(this, EventArgs.Empty);
            }
            if (Parent != null && Parent is Panel)
            {
                ((Panel) Parent).Children.Remove(this);
            }
        }

        private void OkMouseEnter(object sender, MouseEventArgs e)
        {
            ((FrameworkElement) sender).BeginAnimation(OpacityProperty,
                                                       new DoubleAnimation
                                                           {
                                                               Duration = new TimeSpan(0, 0, 0, 0, 250),
                                                               To = 1.0
                                                           });
        }

        private void OkMouseLeave(object sender, MouseEventArgs e)
        {
            ((FrameworkElement) sender).BeginAnimation(OpacityProperty,
                                                       new DoubleAnimation
                                                           {
                                                               Duration = new TimeSpan(0, 0, 0, 0, 250),
                                                               To = 0.5
                                                           });
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            Sound.Play(Sounds.Click);
            ScaleDownContent.Begin();
        }

        public static void Show(Panel parent, string title, string text, EventHandler closed)
        {
            var textBlock = new TextBlock
                                {
                                    Style = Styles.BodyText,
                                    Text = text
                                };
            Show(parent, title, textBlock, closed);
        }

        public static void Show(Panel parent, string title, UIElement content, EventHandler closed)
        {
            var dialog = new Dialog();
            if (closed != null)
            {
                dialog.Closed += closed;
            }
            dialog.Header.Text = title;
            dialog.DialogContents.Content = content;
            dialog.Show(parent);
        }
    }
}