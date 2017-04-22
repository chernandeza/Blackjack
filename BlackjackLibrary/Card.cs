using System;

namespace BlackjackLibrary
{
    public class Card
    {
        private Suit _suit;

        public Suit Suit
        {
            get { return _suit; }
            set { _suit = value; }
        }

        private CardValue _value;

        public CardValue Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private String _fileID;

        public String FileID
        {
            get { return _fileID; }
            set { _fileID = value; }
        }

        public Card(Suit suit, CardValue value, String img)
        {
            this._suit = suit;
            this._value = value;
            this._fileID = img;            
        }

        public Card()
        {
        }
    }

    public enum Suit
    {
        Clubs,
        Diamonds,
        Hearts,        
        Spades
    }

    //Si da tiempo, cambiar value para que sea de tipo CardValue.
    public enum CardValue 
    {
        One, //This value is used to control the Ace as 11 or Ace as 1 condition.
        Ace,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }
}
