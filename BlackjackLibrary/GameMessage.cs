using System;

namespace BlackjackLibrary
{
    [Serializable]
    public class GameMessage
    {
        private int _playerNum;

        public int PlayerNumber
        {
            get { return _playerNum; }
            set { _playerNum = value; }
        }

        private Card _card;

        public Card PlayedCard
        {
            get { return _card; }
            set { _card = value; }
        }

        private Message _msg;

        public Message Message
        {
            get { return _msg; }
            set { _msg = value; }
        }

        private int _deckValue;
            
        public int DeckValue
        {
            get { return _deckValue; }
            set { _deckValue = value; }
        }

        public GameMessage()
        {
            DeckValue = 0;
            PlayerNumber = 0;
            PlayedCard = new Card();
            Message = Message.Ack;         
        }

        public GameMessage(Card c, Message m, int playerNum, int value)
        {
            PlayedCard = c;
            Message = m;
            PlayerNumber = playerNum;
            DeckValue = value;
        }
    }

    [Serializable]
    public enum Message : byte
    {
        Error,
        Ack,
        Hello,
        Ready,
        Deal,
        Stay,
        Tie,
        FiveCards,
        TwentyOne,
        BlackJack,
        PlayerWins,
        PlayerLooses
    }
}
