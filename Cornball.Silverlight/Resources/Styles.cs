using System.Windows;

namespace Cornball.Resources
{
    public static class Styles
    {
        internal static Style BodyText
        {
            get { return (Style) Application.Current.Resources["BodyText"]; }
        }

        internal static Style StrongText
        {
            get { return (Style) Application.Current.Resources["StrongText"]; }
        }

        internal static Style TextBox
        {
            get { return (Style) Application.Current.Resources["TextBox"]; }
        }
    }
}