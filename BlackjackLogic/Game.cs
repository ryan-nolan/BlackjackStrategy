using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackjackLogic.Strategies;

namespace BlackjackLogic
{
    public class Game
    {

        public int handsPlayed = 0;
        public int handsToBePlayed = 1000;
        public int cardCountWhenShuffle = 26;
        public bool humanPlayer = false;

        public int minBet = 20;
        public int maxBet = 500;

        public Deck deck;
        public List<Card> burntCards = new List<Card>();

        public Player player;
        public Dealer dealer;
        public Game()
        {
            //Game is initialised   //Parameters are set //Player strategy is chosen here
            //Deck is made
            //Cards are shuffled
            InitialiseGameAsPlayer();

            //A card is burnt
            burntCards.Add(deck.Cards.Pop());
            for (int i = 0; i < handsToBePlayed; i++)
            {
                //shuffle check     Shuffles first turn available when cards left are smaller than the number specified
                if (deck.Cards.Count < cardCountWhenShuffle)
                {
                    deck = new Deck();
                    deck.Shuffle();
                    burntCards.Clear();
                }

                //TODO Player places bet
                //Get Bet function
                player.AddBet(player.CalculateBet(minBet, maxBet));

                //Deal Cards
                DealHand();

                //Dealer reveals upcard //Reference to first card in hand
                dealer.upCard = dealer.hand.cards[0];
                Console.WriteLine($"Dealer's Up Card is: {dealer.upCard.ToString()}");
                Console.WriteLine($"Player's cards are: {player.hand.ToString()}");

                //Check naturals
                UpdateHandValues();
                if (player.hand.handValues.Contains(21))
                {
                    if (!dealer.hand.handValues.Contains(21))
                    {
                        //TODO PLAYER WINS
                        Console.WriteLine("Player Has a Natural");
                        Console.WriteLine("Game Result: Player Wins");
                        player.Chips += player.Stake * 2;
                        player.Stake = 0;
                    }
                    else
                    {
                        Console.WriteLine("Dealer Has a natural");
                        Console.WriteLine("Game Result: TIE");
                        player.Chips += player.Stake;
                        //TIE
                    }

                }
                else if (dealer.hand.handValues.Contains(21))
                {
                    if (!player.hand.handValues.Contains(21))
                    {
                        Console.WriteLine("Dealer Has a natural");
                        Console.WriteLine("Game Result: Dealer Wins");
                        player.Stake = 0;
                        //TODO DEALER WINS
                    }
                    else
                    {

                    
                        Console.WriteLine("Player Has a Natural");
                        Console.WriteLine("Game Result: TIE");
                        player.Chips += player.Stake;
                        //TIE
                    }
                }
                else
                {

                    //TODO Player reacts, can double down or split here
                    player.React(dealer.upCard);
                    if (player.CurrentState == PlayerState.DOUBLE_DOWN)
                    {
                        player.AddBet(player.Stake);
                        HitPlayer(player);
                        Console.WriteLine(player.hand.cards.Last());
                        if (player.hand.handValues.First() < 21)
                        {
                            player.CurrentState = PlayerState.STAND;
                        }
                        else
                        {
                            player.CurrentState = PlayerState.BUST;
                        }

                    }
                    while (player.CurrentState != PlayerState.BUST && player.CurrentState != PlayerState.STAND)
                    {
                        //dealer.React();
                        if (player.CurrentState == PlayerState.HIT)
                        {
                            player.WriteCurrentState();
                            HitPlayer(player);
                            Console.WriteLine(player.hand.cards.Last());
                            //dealer.hand.WriteHandAndHandValue();
                        }
                        player.React(dealer.upCard);
                    }
                    player.WriteCurrentState();
                    //If player is bust, player loses his bet and hand is over
                    if (player.CurrentState == PlayerState.BUST)
                    {
                        //PLAYER LOSES
                        Console.WriteLine("Game Result: Dealer Wins");
                        player.Stake = 0;
                    }
                    else
                    {
                        Console.Write("Dealer has: ");
                        dealer.hand.WriteHandAndHandValue();
                        //If not dealer reacts
                        dealer.React();
                        while (dealer.CurrentState != PlayerState.BUST && dealer.CurrentState != PlayerState.STAND)
                        {
                            //dealer.React();
                            if (dealer.CurrentState == PlayerState.HIT)
                            {
                                dealer.WriteCurrentState();
                                HitPlayer(dealer);
                                Console.WriteLine(dealer.hand.cards.Last());
                                //dealer.hand.WriteHandAndHandValue();
                            }
                            dealer.React();
                        }
                        dealer.WriteCurrentState();

                        //If dealer is bust and player is not, player wins
                        if (dealer.CurrentState == PlayerState.BUST)
                        {
                            Console.WriteLine("Game Result: Player Wins");
                            player.Chips += player.Stake * 2;
                            
                            //Player wins
                        }
                        else
                        {
                            player.hand.SetHandValues();
                            dealer.hand.SetHandValues();
                            if (player.hand.handValues.Last() > dealer.hand.handValues.Last())
                            {
                                Console.WriteLine("Game Result: Player Wins");
                                player.Chips += player.Stake * 2;
                                //Player Wins
                            }
                            else if (player.hand.handValues.Last() < dealer.hand.handValues.Last())
                            {
                                Console.WriteLine("Game Result: Dealer Wins");
                            }
                            else
                            {
                                Console.WriteLine("Game Result: TIE");
                                player.Chips += player.Stake;
                            }
                            
                        }
                    }
                }
                CleanupHand();
                if (humanPlayer)
                {
                    Console.ReadKey();
                    Console.Clear();
                }
                handsPlayed++;
                
            }




            //DealHand();


            ////DEALER LOGIC, DEALER REACTS AND SETS HIS STATE, SHOULD REACT BEFORE AND AFTER A DECISION IS MADE
            //dealer.React();
            //dealer.hand.WriteHandAndHandValue();
            //while (dealer.CurrentState != PlayerState.BUST && dealer.CurrentState != PlayerState.STAND)
            //{
            //    //dealer.React();
            //    if (dealer.React() == PlayerState.HIT)
            //    {
            //        dealer.WriteCurrentState();
            //        HitPlayer(dealer);
            //        dealer.hand.WriteHandAndHandValue();
            //    }
            //}
            ////If dealer is bust and player is not, player wins
            //dealer.WriteCurrentState();
            //CleanupHand();


            //while(handsPlayed >= handsToBePlayed)
            //{
            //    for (int j = burntCards.Count; j > cardCountWhenToShuffle; j = burntCards.Count)
            //    {


            //        handsPlayed++;
            //        //Perform hand cleanup
            //    }

            //    deck = new Deck();
            //    deck.Shuffle();
            //    burntCards.Clear();
            //    burntCards.Add(deck.Cards.Pop());
            //}

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

        public void UpdateHandValues()
        {
            player.hand.SetHandValues();
            dealer.hand.SetHandValues();
        }

        private void CleanupHand()
        {
            foreach(var c in player.hand.cards)
            {
                burntCards.Add(c);
            }
            player.hand.cards.Clear();
            foreach (var c in dealer.hand.cards)
            {
                burntCards.Add(c);
            }
            dealer.hand.cards.Clear();

            player.Stake = 0;
        }

        //Add player(and what strategy it plays) and dealer init
        private void InitialiseGame()
        {
            deck = new Deck();
            deck.Shuffle();

            player = new PlayerStrategy();
            dealer = new Dealer();
        }
        private void InitialiseGameAsPlayer()
        {
            deck = new Deck();
            deck.Shuffle();

            player = new HumanStrategy();
            dealer = new Dealer();
            humanPlayer = true;
        }
        private void InitialiseGame(string strategy)
        {
            deck = new Deck();
            deck.Shuffle();

            switch (strategy)
            {
                default:
                    player = new PlayerStrategy();
                    break;
            }



            dealer = new Dealer();
        }


        public void DealHand(/*Dealer dealer, Player player*/)
        {
            //TODO MAYBE BURN CARDS!!!!
            player.hand.cards.Add(deck.Cards.Pop());
            player.hand.cards.Add(deck.Cards.Pop());
            dealer.hand.cards.Add(deck.Cards.Pop());
            dealer.hand.cards.Add(deck.Cards.Pop());

            player.hand.SetHandValues();
            dealer.hand.SetHandValues();
        }

        public void HitPlayer(Actor actor)//, Deck deck)
        {
            actor.hand.cards.Add(deck.Cards.Pop());
            actor.hand.SetHandValues();
        }


        public void RepopulateDeck()
        {
            deck = new Deck();
            deck.Shuffle();
            burntCards.Clear();
            burntCards.Add(deck.Cards.Pop());
        }

        public void GameTest()
        {
            InitialiseGame();

            DealHand();

            player.hand.WriteHandAndHandValue();
            dealer.hand.WriteHandAndHandValue();

            Console.WriteLine();
            Console.WriteLine($"Cards Remaining:\t{deck.Cards.Count}");
            Console.WriteLine();

            HitPlayer(player);
            HitPlayer(dealer);

            player.hand.WriteHandAndHandValue();
            dealer.hand.WriteHandAndHandValue();

            Console.WriteLine();
            Console.WriteLine($"Cards Remaining:\t{deck.Cards.Count}");
        }


        public void DealerTest()
        {
            CleanupHand();
            for (int i = 0; i < 5; i++)
            {
                DealHand();
                Console.WriteLine($"Dealers cards are {dealer.hand.ToString()} and their value is {dealer.hand.handValues.First()}");
                Console.WriteLine($"Dealer reacts by: {dealer.React().ToString()}");
                dealer.React();
                while ((dealer.CurrentState != PlayerState.BUST && dealer.CurrentState != PlayerState.STAND))
                {
                    if (dealer.React() == PlayerState.HIT)
                    {
                        HitPlayer(dealer);
                        Console.WriteLine($"Dealers cards are {dealer.hand.ToString()} and their value is {dealer.hand.handValues.First()}");
                    }
                }
                Console.WriteLine($"Dealer reacts by: {dealer.React().ToString()}");
                CleanupHand();
                Console.WriteLine();
            }

            //TEST DEALER HAND CODE
            //dealer.hand.cards.Add(new Card(Suit.Club, Face.Ace));

            //DealHand();
            //Console.WriteLine($"Dealers cards are {dealer.hand.cards[0].ToString()} and {dealer.hand.cards[1].ToString()} and their value is {dealer.hand.handValues.First()}");
            //Console.WriteLine($"Dealer reacts by: {dealer.React().ToString()}");
            //CleanupHand();

            //DealHand();
            //Console.WriteLine($"Dealers cards are {dealer.hand.cards[0].ToString()} and {dealer.hand.cards[1].ToString()} and their value is {dealer.hand.handValues.First()}");
            //Console.WriteLine($"Dealer reacts by: {dealer.React().ToString()}");
            //CleanupHand();

            //DealHand();
            //Console.WriteLine($"Dealers cards are {dealer.hand.cards[0].ToString()} and {dealer.hand.cards[1].ToString()} and their value is {dealer.hand.handValues.First()}");
            //Console.WriteLine($"Dealer reacts by: {dealer.React().ToString()}");
            //CleanupHand();
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
