using System.Windows;

namespace Cornball
{
    public class Deal
    {
        public Deal(Card card, Point source, Point target)
        {
            Card = card;
            Source = source;
            Target = target;
        }

        public Card Card { get; private set; }

        public Point Source { get; private set; }

        public Point Target { get; private set; }
    }
}