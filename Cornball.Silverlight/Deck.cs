using System;
using System.Collections.Generic;

namespace Cornball
{
    public class Deck : List<Card>
    {
        public Deck() : base(Game.Rows*Game.Columns)
        {
            for (int suit = 1; suit <= Game.Rows; suit++)
            {
                for (int value = 1; value <= Game.Columns; value++)
                {
                    Add(new Card((Suit) suit, (Value) value));
                }
            }
        }

        public void Shuffle()
        {
            var random = new Random();
            for (int i = Count - 1; i >= 0; i--)
            {
                Swap(random.Next(i), i);
            }
        }

        public void ReShuffle()
        {
            var shuffle = new List<int>();
            for (int i = 0; i < Count; i++)
            {
                if (this[i].RoundPlaced == null)
                {
                    shuffle.Add(i);
                }
            }
            var random = new Random();
            for (int i = shuffle.Count - 1; i >= 0; i--)
            {
                Swap(shuffle[random.Next(i)], shuffle[i]);
            }
        }

        internal void Swap(int fromIndex, int toIndex)
        {
            Card temp = this[fromIndex];
            this[fromIndex] = this[toIndex];
            this[toIndex] = temp;
        }
    }
}