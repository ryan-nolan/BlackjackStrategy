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
            InitialiseGame();

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
                    burntCards.Add(deck.Cards.Pop());
                }

                //TODO Player places bet
                //Get Bet function
                player.AddBet(player.CalculateBet(minBet, maxBet), ref player.Stake);

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

                    //Player reacts, can double down or split here
                    player.React(dealer.upCard, ref player.CurrentState, player.hand);
                    if (player.CurrentState == PlayerState.SPLIT)
                    {
                        //Splitting the hands
                        player.splitHand = new Hand();
                        player.splitHand.cards.Add(player.hand.cards.Last());
                        player.hand.cards.Remove(player.splitHand.cards.First());
                        HitPlayer(player);
                        HitPlayer(player.splitHand);
                        
                        Console.WriteLine(player.hand.ToString());
                        Console.WriteLine(player.splitHand.ToString());

                        player.AddBet(player.Stake, ref player.SplitHandStake);

                        //Playing the split hand
                        if (player.hand.cards.First().Face == Face.Ace && player.splitHand.cards.First().Face == Face.Ace)
                        {
                            player.CurrentState = PlayerState.STAND;
                            player.splitHandState = PlayerState.STAND;
                        }
                        else
                        { 
                            player.React(dealer.upCard, ref player.splitHandState, player.splitHand);
                        }
                        if (player.splitHandState == PlayerState.DOUBLE_DOWN)
                        {
                            player.AddBet(player.Stake, ref player.SplitHandStake);
                            HitPlayer(player.splitHand);
                            Console.WriteLine(player.splitHand.cards.Last());
                            if (player.hand.handValues.First() <= 21)
                            {
                                player.splitHandState = PlayerState.STAND;
                            }
                            else
                            {
                                player.splitHandState = PlayerState.BUST;
                            }
                        }
                        while (player.splitHandState != PlayerState.BUST && player.splitHandState != PlayerState.STAND)
                        {
                            //dealer.React();
                            if (player.splitHandState == PlayerState.HIT)
                            {
                                player.WriteCurrentState();
                                HitPlayer(player.splitHand);
                                Console.WriteLine(player.splitHand.ToString());
                                //dealer.hand.WriteHandAndHandValue();
                            }
                            player.React(dealer.upCard, ref player.splitHandState, player.splitHand);
                        }
                        if (player.splitHandState == PlayerState.BUST)
                        {
                            player.SplitHandStake = 0;
                        }

                    }
                    player.React(dealer.upCard, ref player.CurrentState, player.hand);
                    if (player.CurrentState == PlayerState.DOUBLE_DOWN)
                    {
                        player.AddBet(player.Stake, ref player.Stake);
                        HitPlayer(player);
                        Console.WriteLine(player.hand.cards.Last());
                        if (player.hand.handValues.First() <= 21)
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
                        player.React(dealer.upCard, ref player.CurrentState, player.hand);
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
                            if (player.splitHand != null)
                            {
                                Console.WriteLine("Game Result: Player Wins Split Hand");
                                player.Chips += player.SplitHandStake * 2;
                            }
                            
                            
                            //Player wins
                        }
                        //SETTLEMENT
                        else
                        {
                            player.hand.SetHandValues();
                            dealer.hand.SetHandValues();
                            if (player.splitHand != null)
                            {
                                if (player.splitHand.handValues.Last() > dealer.hand.handValues.Last())
                                {
                                    Console.WriteLine("Game Result: Player Wins Split Hand");
                                    player.Chips += player.SplitHandStake * 2;
                                    //Player Wins
                                }
                                else if (player.splitHand.handValues.Last() < dealer.hand.handValues.Last())
                                {
                                    Console.WriteLine("Game Result: Dealer Wins Split Hand");
                                }
                                else
                                {
                                    Console.WriteLine("Game Result: TIE Split Hand");
                                    player.Chips += player.SplitHandStake;
                                }
                            }
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
            Console.WriteLine(handsPlayed);
            Console.WriteLine(player.Chips);

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
                burntCards.Add(c.Clone());
            }
            player.hand.cards.Clear();
            player.splitHand = null;
            foreach (var c in dealer.hand.cards)
            {
                burntCards.Add(c);
            }
            dealer.hand.cards.Clear();

            player.Stake = 0;
            player.SplitHandStake = 0;
        }

        //Add player(and what strategy it plays) and dealer init
        private void InitialiseGame()
        {
            deck = new Deck();
            deck.Shuffle();

            player = new BasicStrategy();
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
        public void HitPlayer(Hand hand)//, Deck deck)
        {
            hand.cards.Add(deck.Cards.Pop());
            hand.SetHandValues();
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

        }
    }
}
