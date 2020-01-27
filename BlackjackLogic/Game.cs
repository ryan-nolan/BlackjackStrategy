﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackLogic
{
    public class Game
    {

        public int handsPlayed = 0;
        public int turnsPlayed = 0;

        public int handsToBePlayed = 1000;
        public int cardCountWhenToShuffle = 26;

        public Deck deck;
        public List<Card> burntCards = new List<Card>();

        public Player player;
        public Dealer dealer;
        public Game()
        {
            //Game is initialised   //Parameters are set //Player strategy is chosen here
            //Deck is made
            //Cards are shuffled
            InitialiseGame();

            //A card is burnt
            burntCards.Add(deck.Cards.Pop());

            while(handsPlayed >= handsToBePlayed)
            {
                for (int j = burntCards.Count; j > cardCountWhenToShuffle; j = burntCards.Count)
                {







                    handsPlayed++;
                }

                deck = new Deck();
                deck.Shuffle();
                burntCards.Clear();
                burntCards.Add(deck.Cards.Pop());
            }

            //foreach hand
                //for half the deck is used
                    //hand loop
                    //Player places bet
                    //deal 2 cards to player
                    //deal 2 cards to dealer
                    //Reveal first dealer card
                    //Check naturals
                    //while(player is not bust or stands) Player makes decisions //send player game state and wait for a state return //doubles down or splits here
                    //If player busts, turn ends
                    //if not player busts, dealer plays
                    //Settlement
                //shuffle


        }

        //Add player(and what strategy it plays) and dealer init
        public void InitialiseGame()
        {
            deck = new Deck();
            deck.Shuffle();

            player = new Player();
            dealer = new Dealer();
        }


        public void DealHand(Dealer dealer, Player player)
        {
            //TODO MAYBE BURN CARDS!!!!
            player.hand.cards.Add(deck.Cards.Pop());
            dealer.hand.cards.Add(deck.Cards.Pop());
            player.hand.cards.Add(deck.Cards.Pop());
            dealer.hand.cards.Add(deck.Cards.Pop());
        }

        public void HitPlayer(Actor actor)//, Deck deck)
        {
            actor.hand.cards.Add(deck.Cards.Pop());
        }

        public void WriteHandAndHandValue(Actor actor)
        {
            foreach (var c in actor.hand.cards)
            {
                Console.WriteLine(c);
            }

            foreach (var hv in actor.hand.GetHandValues())
            {
                Console.WriteLine($"Player hand value:\t{hv}");
            }
            Console.WriteLine();
        }

        public void GameTest()
        {
            InitialiseGame();

            DealHand(dealer, player);

            WriteHandAndHandValue(player);
            WriteHandAndHandValue(dealer);

            Console.WriteLine();
            Console.WriteLine($"Cards Remaining:\t{deck.Cards.Count}");
            Console.WriteLine();

            HitPlayer(player);
            HitPlayer(dealer);

            WriteHandAndHandValue(player);
            WriteHandAndHandValue(dealer);

            Console.WriteLine();
            Console.WriteLine($"Cards Remaining:\t{deck.Cards.Count}");
        }









        //public Game()
        //{
        //    Deck deck = new Deck();
        //    deck.Shuffle();

        //    Hand testHand = new Hand();
        //    testHand.hand.Add(deck.Cards.Pop());
        //    Card ace = new Card(Suit.Club, Face.Ace);
        //    Card ten = new Card(Suit.Diamond, Face.Ten);
        //    testHand.hand.Add(ace);


        //    foreach (var c in testHand.hand)
        //    {
        //        Console.WriteLine(c);
        //    }
        //    testHand.SetHandValue();
        //    foreach (var v in testHand.handValues)
        //    {
        //        Console.WriteLine(v);
        //    }
        //}

    }
}
