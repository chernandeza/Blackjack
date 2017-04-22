using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackjackLibrary
{
    public class Deck
    {
        private List<Card> _cards;

        private Stack<Card> _deck;

        public Stack<Card> CardDeck
        {
            get { return _deck; }
            set { _deck = value; }
        }

        public Deck()
        {
            this._cards = new List<Card>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (CardValue value in Enum.GetValues(typeof(CardValue)))
                {
                    if (value != CardValue.One)
                    {
                        String id = value.ToString().ToLower() + "_of_" + suit.ToString().ToLower() + ".png";
                        this._cards.Add(new Card(suit, value, id));
                    }                    
                }
            }
            this.Shuffle();
            this._deck = new Stack<Card>(this._cards);
        }

        private void Shuffle()
        {
            Random r = new Random(Guid.NewGuid().GetHashCode());
            for (int i = this._cards.Count - 1; i > 0; i--)
            {
                Card temp = this._cards[i];
                var index = r.Next(0, i + 1);
                this._cards[i] = this._cards[index];
                this._cards[index] = temp;
            }
        }
    }
}
