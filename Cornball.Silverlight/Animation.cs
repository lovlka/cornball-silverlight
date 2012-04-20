using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Cornball
{
    public static class Animation
    {
        public static void BeginAnimation(this DependencyObject obj, DependencyProperty property,
                                          DoubleAnimation animation)
        {
            BeginAnimation(obj, property, animation, null);
        }

        public static void BeginAnimation(this DependencyObject obj, DependencyProperty property,
                                          DoubleAnimation animation, EventHandler completed)
        {
            var storyboard = new Storyboard();
            Storyboard.SetTargetProperty(storyboard, new PropertyPath(property));
            Storyboard.SetTarget(storyboard, obj);
            storyboard.Children.Add(animation);
            storyboard.Completed += completed;
            storyboard.Begin();
        }
    }
}