using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Cornball
{
    public class Game
    {
        internal const int HighScore = 10;
        internal const int Padding = 30;
        internal const int Margin = 10;
        internal const int CardWidth = 72;
        internal const int CardHeight = 97;
        internal const int Rounds = 5;
        internal const int Columns = 13;
        internal const int Rows = 4;
        internal static double Scale = 1;
        public Deck Deck;
        public bool Finished;
        public Move LastMove;
        public int Moves;
        public int Round;

        public Game()
        {
            Deck = new Deck();
            Deck.Shuffle();
            Finished = false;
            Moves = 0;
            Round = 1;
        }

        public int Score
        {
            get
            {
                int score = 0;
                foreach (Card card in Deck)
                {
                    if (card.RoundPlaced != null)
                    {
                        if (card.Value == Value.King)
                        {
                            score += (Rounds - card.RoundPlaced.Value + 1)*60;
                        }
                        else if (card.Value >= Value.Ten)
                        {
                            score += (Rounds - card.RoundPlaced.Value + 1)*40;
                        }
                        else
                        {
                            score += (Rounds - card.RoundPlaced.Value + 1)*20;
                        }
                    }
                }
                score -= (Round - 1)*100;
                score -= Moves*5;
                return score;
            }
        }

        public event EventHandler ReShuffle;
        public event EventHandler Lose;
        public event EventHandler Win;

        public bool MoveAllowed(Card from, Card to)
        {
            if (from.Value != Value.Ace && to.Value == Value.Ace)
            {
                int index = Deck.IndexOf(to);
                if (index%13 == 0 && from.Value == Value.Two)
                {
                    return true;
                }
                if (from.Value != Value.Two && Deck[index - 1].Suit == from.Suit &&
                    Deck[index - 1].Value == from.Value - 1)
                {
                    return true;
                }
            }
            return false;
        }

        public void ExecuteMove(Card from, Card to)
        {
            Deck.Swap(Deck.IndexOf(from), Deck.IndexOf(to));
            from.ExecuteMove(to);
            Moves++;
        }

        public void CheckState()
        {
            if (Finished)
            {
                return;
            }
            CheckPlacedCards();
            int locked =
                Deck.Where(
                    (t, i) =>
                    t.Value == Value.Ace &&
                    (i%13 != 0 && (Deck[i - 1].Value == Value.King || Deck[i - 1].Value == Value.Ace))).Count();
            if (locked == 4)
            {
                if (Deck.Count(c => c.RoundPlaced == null) == 4)
                {
                    Finished = true;
                    if (Win != null)
                    {
                        Win(null, EventArgs.Empty);
                    }
                }
                else
                {
                    if (Round == Rounds)
                    {
                        Finished = true;
                        if (Lose != null)
                        {
                            Lose(null, EventArgs.Empty);
                        }
                    }
                    else
                    {
                        Round++;
                        if (ReShuffle != null)
                        {
                            ReShuffle(null, EventArgs.Empty);
                        }
                    }
                }
            }
        }

        private void CheckPlacedCards()
        {
            Suit? suit = null;
            for (int i = 0; i < Deck.Count; i++)
            {
                if (i%13 == 0)
                {
                    if (Deck[i].Value == Value.Two)
                    {
                        if (Deck[i].RoundPlaced == null)
                        {
                            Deck[i].RoundPlaced = Round;
                        }
                        suit = Deck[i].Suit;
                    }
                }
                else if (Deck[i].Suit == suit && Deck[i].Value == (Value) ((i%13) + 2))
                {
                    if (Deck[i].RoundPlaced == null)
                    {
                        Deck[i].RoundPlaced = Round;
                    }
                }
                else
                {
                    Deck[i].RoundPlaced = null;
                    suit = null;
                }
            }
        }

        public Card FindGapForCard(Card card)
        {
            if (card.Value == Value.Two)
            {
                for (int i = 0; i < Deck.Count; i += 13)
                {
                    if (Deck[i].Value == Value.Ace)
                    {
                        return Deck[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < Deck.Count; i++)
                {
                    if (Deck[i].Suit == card.Suit && Deck[i].Value == card.Value - 1)
                    {
                        if (i < Deck.Count - 1 && Deck[i + 1].Value == Value.Ace && (i + 1)%13 != 0)
                        {
                            return Deck[i + 1];
                        }
                    }
                }
            }
            return null;
        }

        public List<Card> FindCardsForGap(Card gap)
        {
            int index = Deck.IndexOf(gap);
            var cards = new List<Card>();
            if (index%13 == 0)
            {
                cards.AddRange(Deck.Where(c => c.Value == Value.Two));
            }
            else if (Deck[index - 1].Value != Value.King && Deck[index - 1].Value != Value.Ace)
            {
                cards.Add(Deck.First(c => c.Suit == Deck[index - 1].Suit && c.Value == Deck[index - 1].Value + 1));
            }
            return cards;
        }

        public Card FindIntersectingGap(Card movedCard)
        {
            foreach (Card targetCard in Deck.Where(c => c.Value == Value.Ace))
            {
                Rect targetRect = targetCard.Bounds;
                targetRect.Intersect(movedCard.Bounds);
                if (!targetRect.IsEmpty && MoveAllowed(movedCard, targetCard))
                {
                    return targetCard;
                }
            }
            return null;
        }
    }
}