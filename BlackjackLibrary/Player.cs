﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace BlackjackLibrary
{
    public class Player
    {
        private int _myTotal;

        public int Total { get { return _myTotal; } }

        public int CardCount { get { return Cards.Count; } }

        private List<Card> _myCards;

        public List<Card> Cards
        {
            get { return _myCards; }
            set { _myCards = value; }
        }

        private TcpClient _channel;

        public TcpClient Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }

        private PlayerStatus _status;

        public PlayerStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        private bool _isPlaying;

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set { _isPlaying = value; }
        }

        public Player()
        {
            this._myCards = new List<Card>();
            this._myTotal = 0;
            this.Channel = new TcpClient();
            this._status = PlayerStatus.Playing;
            this._isPlaying = true;
        }

        /// <summary>
        /// Takes the card from the dealer and determines the player status on the game. Player Logic.
        /// </summary>
        /// <param name="newCard">New card for the player's deck</param>
        /// <returns>0 if any of these conditions are met:
        /// - The sum of the cards is 21
        /// - The player gets a 2 card blackjack
        /// - The sum of the cards is greater than 21
        /// - The player has 5 cards and their sum is less than 21
        /// If none of the previous conditions are met, the function returns the sum of the cards in the player's deck.
        /// </returns>
        public void Play(Card newCard)
        {
            Cards.Add(newCard);
            switch (newCard.Value)
            {
                case CardValue.Ace:
                    if (this.CardCount == 2)
                    {
                        if (this.Cards[0].Value == CardValue.Jack || this.Cards[0].Value == CardValue.Queen || this.Cards[0].Value == CardValue.King)
                        {
                            this.Status = PlayerStatus.BlackJack;
                        }
                    }
                    int tempTotal = this._myTotal + 11;
                    if (tempTotal > 21)
                    {
                        this._myTotal += 1;
                        this.Cards[Cards.Count - 1].Value = CardValue.One; //Ace used as One
                    }
                    else
                    {
                        this._myTotal += 11;
                    }
                    break;
                case CardValue.Two:
                    this._myTotal += 2;
                    break;
                case CardValue.Three:
                    this._myTotal += 3;
                    break;
                case CardValue.Four:
                    this._myTotal += 4;
                    break;
                case CardValue.Five:
                    this._myTotal += 5;
                    break;
                case CardValue.Six:
                    this._myTotal += 6;
                    break;
                case CardValue.Seven:
                    this._myTotal += 7;
                    break;
                case CardValue.Eight:
                    this._myTotal += 8;
                    break;
                case CardValue.Nine:
                    this._myTotal += 9;
                    break;
                case CardValue.Ten:
                    this._myTotal += 10;
                    break;
                case CardValue.Jack:
                case CardValue.Queen:
                case CardValue.King:
                    if (this.CardCount == 2)
                    {
                        if (this.Cards[0].Value == CardValue.Ace)
                        {
                            this.Status = PlayerStatus.BlackJack;
                        }
                        else
                        {
                            this._myTotal += 10;
                        }
                    }
                    else
                    {
                        this._myTotal += 10;
                    }
                    break;
                default:
                    break;
            }
            if (this.CardCount == 5 && this.Total < 21)
            {
                this.Status = PlayerStatus.FiveCards;
            }
            if (this.Total > 21)
            {
                try
                {//Is there an Ace on my cards? Use it as a One instead of 11.
                    Card c = Cards.First(a => a.Value == CardValue.Ace);
                    int idx = Cards.FindIndex(u => u.Equals(c));
                    Cards[idx].Value = CardValue.One; //Avoids the card being used more than once as a One
                    this._myTotal -= 10; //Rest 10 to total, using Ace as One
                }
                catch (Exception)
                {
                    this.Status = PlayerStatus.Lost;
                }                
            }
            if (this.Total == 21)
            {
                this.Status = PlayerStatus.TwentyOne;
            }
        }        
    }

    public enum PlayerStatus
    {
        FiveCards,
        BlackJack,
        TwentyOne,
        Stay,
        Playing,           
        Lost
    }
}
