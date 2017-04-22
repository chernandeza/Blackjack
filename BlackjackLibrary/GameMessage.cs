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

        public GameMessage()
        {
            PlayerNumber = 0;
            PlayedCard = new Card();
            Message = Message.Ack;         
        }

        public GameMessage(Card c, Message m, int playerNum)
        {
            PlayedCard = c;
            Message = m;
            PlayerNumber = playerNum;
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
