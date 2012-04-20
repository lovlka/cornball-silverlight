namespace Cornball
{
    public class Move
    {
        public Move(Card from, Card to)
        {
            From = from;
            To = to;
        }

        public Card From { get; private set; }

        public Card To { get; private set; }
    }
}