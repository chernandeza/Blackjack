namespace BlackjackLibrary
{
    public class Dealer
    {
        private Deck _dealerDeck;

        public Deck DealerDeck
        {
            get { return _dealerDeck; }
            set { _dealerDeck = value; }
        }

        public Dealer()
        {
            this._dealerDeck = new Deck();
        }
        
        public Card Deal()
        {
            return DealerDeck.CardDeck.Pop();
        }
    }
}
