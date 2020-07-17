﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackjackLogic.Strategies;

namespace BlackjackLogic
{
    public class Game
    {

        public int handsPlayed = 0;
        public int handsToBePlayed = 10000;
        public int cardCountWhenShuffle = 13;
        public bool humanPlayer = false;
        public int minBet = 10;
        public int maxBet = 50;

        public int startChips = 500;

        //Counts
        int fiveCount = 0;
        int count = 0;
        string countType = "five";

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

            //Create file and write first line
            string filename = $"{player.StrategyName}_hands({handsToBePlayed})shuffleFrequency({cardCountWhenShuffle}).csv";
            StreamWriter f = InitialiseFile(filename);


            //Console.WriteLine($"Player's starting chips {player.Chips}");
            //Initialise File Variables
            int amountOfCardsInDeckBeforeTurn = 0;
            int playerStakeForFile = 0;
            int playerStartingChips = 0;
            int playersStartingHardHandValueForFile = 0;
            int playersStartingSoftHandValueForFile = 0;
            string playersDecisions = "";
            string playersSplitHandDecisions = "";
            string dealersDecisions = "";
            string doesPlayerSplit = "N";
            string playersStartingSplitHandForfile = "";
            string playersStartingSplitHardHandValueForFile = "";
            string playersStartingSplitSoftHandValueForFile = "";
            string playersEndSplitHand = "";
            string playersEndSplitHandValue = "";
            string playersStartingHandPreSplit = "";
            string gameResult = "";
            string splitGameResult = "";
            //A card is burnt
            burntCards.Add(deck.Cards.Pop());

            for (int i = 0; i < handsToBePlayed; i++)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");//CONSOLE HAND SEPERATOR
                //shuffle check     Shuffles first turn available when cards left are smaller than the number specified
                if (deck.Cards.Count < cardCountWhenShuffle)
                {
                    deck = new Deck();
                    deck.Shuffle();
                    burntCards.Clear();
                    burntCards.Add(deck.Cards.Pop());
                    UpdateCounts();
                }
                Console.WriteLine($"Pre hand count:\t{count}");

                amountOfCardsInDeckBeforeTurn = deck.Cards.Count;

                playersDecisions = "";
                playersSplitHandDecisions = "";
                dealersDecisions = "";
                doesPlayerSplit = "N";

                playerStartingChips = player.Chips;//FOR FILE
                //Get Bet function
                player.AddBet(player.CalculateBet(minBet, maxBet, count), ref player.Stake);
                playerStakeForFile = player.Stake; //FOR FILE

                //Deal Cards
                DealHand();
                playersStartingHandPreSplit = player.hand.ToString();//FOR FILE

                //Set Starting Hand Values for file
                playersStartingHardHandValueForFile = player.hand.handValues.First();//FOR FILE
                if (player.hand.handValues.Count > 1) { playersStartingSoftHandValueForFile = player.hand.handValues.Last(); }//FOR FILE
                else { playersStartingSoftHandValueForFile = 0; }//FOR FILE

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
                        Console.WriteLine("Player Has a Natural");
                        playersDecisions = "PLAYER_NATURAL";
                        Console.WriteLine("Game Result: Player Wins");
                        player.Chips += (int)(player.Stake * 2.5);
                        player.Stake = 0;
                        gameResult = "W_N";
                    }
                    else
                    {
                        Console.WriteLine("Player Has a natural");
                        Console.WriteLine("Dealer Has a natural");
                        playersDecisions = "PLAYER_NATURAL";
                        dealersDecisions = "DEALER_NATURAL";
                        Console.WriteLine("Game Result: TIE");
                        player.Chips += player.Stake;
                        //TIE
                        gameResult = "D_N";
                    }

                }
                else if (dealer.hand.handValues.Contains(21))
                {
                    if (!player.hand.handValues.Contains(21))
                    {
                        Console.WriteLine("Dealer Has a natural");
                        dealersDecisions = "DEALER_NATURAL";
                        Console.WriteLine("Game Result: Dealer Wins");
                        player.Stake = 0;
                        gameResult = "L_N";
                    }
                    else
                    {

                        Console.WriteLine("Player Has a Natural");
                        Console.WriteLine("Dealer Has a natural");
                        Console.WriteLine("Game Result: TIE");
                        playersDecisions = "PLAYER_NATURAL";
                        dealersDecisions = "DEALER_NATURAL";
                        player.Chips += player.Stake;
                        //TIE
                        gameResult = "D_N";
                    }
                }
                else
                {

                    //Player reacts, can double down or split here
                    player.React(dealer.upCard, ref player.CurrentState, player.hand, count);

                    //PLAY SPLIT HANDS
                    if (player.CurrentState == PlayerState.SPLIT)
                    {
                        playersDecisions += player.CurrentState + "/";//FOR FILE
                        doesPlayerSplit = "Y";//FOR FILE

                        //Splitting the hands
                        player.splitHand = new Hand();
                        player.splitHand.cards.Add(player.hand.cards.Last());
                        player.hand.cards.Remove(player.splitHand.cards.First());
                        HitPlayer(player);
                        HitPlayer(player.splitHand);
                        
                        Console.WriteLine($"Player's Hand:\t{player.hand.ToString()}");
                        Console.WriteLine($"Player's Split Hand:\t{player.splitHand.ToString()}");

                        //TODO Maybe document the different stakes, e.g. splitHandStake
                        player.AddBet(player.Stake, ref player.SplitHandStake);
                        
                        //Playing the split hand
                        if (player.hand.cards.First().Face == Face.Ace && player.splitHand.cards.First().Face == Face.Ace)
                        {
                            player.CurrentState = PlayerState.STAND;
                            player.splitHandState = PlayerState.STAND;
                            playersDecisions += player.CurrentState+ "/";//FOR FILE
                            playersSplitHandDecisions += player.splitHandState + "/";//FOR FILE
                        }
                        else
                        { 
                            player.React(dealer.upCard, ref player.splitHandState, player.splitHand, count);
                            playersSplitHandDecisions += player.splitHandState + "/";//FOR FILE
                        }
                        if (player.splitHandState == PlayerState.DOUBLE_DOWN)
                        {
                            player.AddBet(player.SplitHandStake, ref player.SplitHandStake);
                            HitPlayer(player.splitHand);
                            Console.WriteLine($"Player split double downs with:\t{player.splitHand.cards.Last()}");
                            if (player.hand.handValues.First() <= 21)
                            {
                                player.splitHandState = PlayerState.STAND;
                                playersSplitHandDecisions += player.splitHandState + "/";//FOR FILE
                            }
                            else
                            {
                                player.splitHandState = PlayerState.BUST;
                                playersSplitHandDecisions += player.splitHandState + "/";//FOR FILE
                            }
                        }
                        while (player.splitHandState != PlayerState.BUST && player.splitHandState != PlayerState.STAND)
                        {
                            if (player.splitHandState == PlayerState.HIT)
                            {
                                player.WriteCurrentState();
                                HitPlayer(player.splitHand);
                                if (player.splitHand.handValues.First() > 21)
                                {
                                    player.splitHandState = PlayerState.BUST;
                                }
                                Console.WriteLine($"Player's split hand:\t{player.splitHand.ToString()}\t{player.splitHand.handValues.Last()}");
                            }
                            player.React(dealer.upCard, ref player.splitHandState, player.splitHand, count);
                            playersSplitHandDecisions += player.splitHandState + "/";
                        }
                        if (player.splitHandState == PlayerState.BUST)
                        {
                            player.SplitHandStake = 0;
                            splitGameResult = "L";
                        }

                    }
                    //PLAY HAND
                    player.React(dealer.upCard, ref player.CurrentState, player.hand, count);
                    //IF PLAYER SPLIT ACES, STAND
                    if (player.splitHand != null && player.splitHand.cards.First().Face == Face.Ace)
                    {
                        player.CurrentState = PlayerState.STAND;
                    }
                    playersDecisions += player.CurrentState+"/";//FOR FILE

                    //IF PLAYER DOUBLE DOWNS, STAND OR BUST
                    if (player.CurrentState == PlayerState.DOUBLE_DOWN)
                    {
                        player.AddBet(player.Stake, ref player.Stake);
                        HitPlayer(player);
                        Console.WriteLine($"Player DOUBLES DOWN:\t{player.hand.cards.Last()}");
                        Console.WriteLine($"Player's Hand:\t{player.hand.ToString()}\t{player.hand.handValues.Max()}");
                        if (player.hand.handValues.First() <= 21)
                        {
                            player.CurrentState = PlayerState.STAND;
                        }
                        else
                        {
                            player.CurrentState = PlayerState.BUST;
                        }

                    }

                    //WHILE PLAYER ISN'T BUST OR STANDING
                    while (player.CurrentState != PlayerState.BUST && player.CurrentState != PlayerState.STAND)
                    {
                        if (player.CurrentState == PlayerState.HIT)
                        {
                            HitPlayer(player);
                            Console.WriteLine($"Player Hits:\t{player.hand.cards.Last()}\t{player.hand.handValues.Last()}");
                            if (player.hand.handValues.First() > 21)
                            {
                                player.CurrentState = PlayerState.BUST;
                            }
                            //player.WriteCurrentState();
                        }
                        player.React(dealer.upCard, ref player.CurrentState, player.hand, count);
                        playersDecisions += player.CurrentState + "/";//FOR FILE
                    }
                    Console.WriteLine($"PLAYER REACTS:\t{player.CurrentState}\t{player.hand.handValues.Last()}");
                    //If player is bust, player loses his bet and hand is over
                    if (player.CurrentState == PlayerState.BUST)
                    {
                        //PLAYER LOSES
                        Console.WriteLine($"Dealer's Hand:\t{dealer.hand}\t{dealer.hand.handValues.Max()}");
                        Console.WriteLine("Game Result: Dealer Wins");
                        player.Stake = 0;
                        gameResult = "L";
                        Console.WriteLine($"Player Chips:\t{player.Chips}");
                    }
                    else
                    {
                        Console.Write("Dealer has: ");
                        dealer.hand.WriteHandAndHandValue();
                        //If not dealer reacts
                        dealer.React();
                        dealersDecisions += dealer.CurrentState + "/";//FOR FILE
                        while (dealer.CurrentState != PlayerState.BUST && dealer.CurrentState != PlayerState.STAND)
                        {
                            if (dealer.CurrentState == PlayerState.HIT)
                            {
                                HitPlayer(dealer);
                                Console.WriteLine($"Dealer draws: {dealer.hand.cards.Last()}\t{dealer.hand.handValues.Max()}");
                            }
                            dealer.React();
                            dealersDecisions += dealer.CurrentState + "/";//FOR FILE
                        }
                        Console.WriteLine($"DEALER REACTS:\t{dealer.CurrentState}\t{dealer.hand.handValues.Last()}");
                        //If dealer is bust and player is not, player wins
                        if (dealer.CurrentState == PlayerState.BUST)
                        {
                            Console.WriteLine("Game Result: Player Wins");
                            player.Chips += player.Stake * 2;
                            gameResult = "W";
                            if (player.splitHand != null)
                            {
                                Console.WriteLine("Game Result: Player Wins Split Hand");
                                player.Chips += player.SplitHandStake * 2;
                                splitGameResult = "W";
                            }
                            //Player wins
                            Console.WriteLine($"Player Chips:\t{player.Chips}");

                        }
                        //SETTLEMENT
                        else
                        {
                            player.hand.SetHandValues();
                            dealer.hand.SetHandValues();
                            //SETTLE SPLIT HAND
                            if (player.splitHand != null)
                            {
                                if (player.splitHand.handValues.Last() > dealer.hand.handValues.Last())
                                {
                                    Console.WriteLine("Game Result: Player Wins Split Hand");
                                    player.Chips += player.SplitHandStake * 2;
                                    //Player Wins Split Hand
                                    splitGameResult = "W";
                                }
                                else if (player.splitHand.handValues.Last() < dealer.hand.handValues.Last())
                                {
                                    Console.WriteLine("Game Result: Dealer Wins Split Hand");
                                    splitGameResult = "L";
                                }
                                else
                                {
                                    Console.WriteLine("Game Result: TIE Split Hand");
                                    player.Chips += player.SplitHandStake;
                                    splitGameResult = "D";
                                }
                            }
                            if (player.hand.handValues.Last() > dealer.hand.handValues.Last())
                            {
                                Console.WriteLine("Game Result: Player Wins");
                                player.Chips += player.Stake * 2;
                                //Player Wins
                                gameResult = "W";
                            }
                            else if (player.hand.handValues.Last() < dealer.hand.handValues.Last())
                            {
                                Console.WriteLine("Game Result: Dealer Wins");
                                gameResult = "L";
                            }
                            else
                            {
                                Console.WriteLine("Game Result: TIE");
                                player.Chips += player.Stake;
                                gameResult = "D";
                            }
                            Console.WriteLine($"Player Chips:\t{player.Chips}");
                        }
                    }
                }
                //TODO Specific ToString For files
                //file.WriteLine("PlayersStartingChips,EndChips,ChipsWon,CardsInDeckBeforeTurn,CardsInDeckAfterTurn" +
                //"PlayerStake,PlayersStartingHand,PlayerStartingHardHandValue,PlayerStartingHardHandValue,PlayersEndHand,PlayersEndHandValue,PlayersDecisions," +
                //"DealersUpCard,DealersUpCardValue,DealersStartHand,DealersEndHand,DealersHardHandValue,DealersSoftValue,DealersDecisions," +
                //"DoesPlayerSplit,PlayersStartingSplitHand,PlayerStartingSplitHandValue,PlayersEndSplitHand,PlayersSplitEndHandValue,PlayersSplitHandDecisions");
                if (player.splitHand != null)
                {
                    playersStartingSplitHandForfile = player.splitHand.cards[0].ToString() +" "+ player.splitHand.cards[1].ToString();
                    playersStartingSplitHardHandValueForFile = player.splitHand.handValues.First().ToString();
                    playersStartingSplitSoftHandValueForFile = player.splitHand.handValues.Last().ToString();
                    playersEndSplitHand = player.splitHand.ToString();
                    playersEndSplitHandValue = player.splitHand.handValues.Last().ToString();
                    //playersSplitHandDecisions
                }
                else
                {
                    playersStartingSplitHandForfile = "N/A";
                    playersStartingSplitHardHandValueForFile = "N/A";
                    playersStartingSplitSoftHandValueForFile = "N/A";
                    playersEndSplitHand = "N/A";
                    playersEndSplitHandValue = "N/A";
                    playersSplitHandDecisions = "N/A";
                    playersStartingHandPreSplit = "N/A";
                    splitGameResult = "N/A";
                }
                f.WriteLine($"{handsPlayed+1},{playerStartingChips},{gameResult},{splitGameResult},{player.Chips},{player.Chips - playerStartingChips},{amountOfCardsInDeckBeforeTurn},{deck.Cards.Count}," +
                    $"{playerStakeForFile},{player.hand.cards[0].ToString()} {player.hand.cards[1].ToString()}," +
                    $"{playersStartingHardHandValueForFile},{playersStartingSoftHandValueForFile},{player.hand},{player.hand.handValues.Last()},{playersDecisions}," +
                    $"{dealer.upCard},{dealer.hand.cards.First().Value},{dealer.hand.cards[0]} {dealer.hand.cards[1]},{dealer.hand},{dealer.hand.handValues.First()},{dealer.hand.handValues.Last()}," +
                    $"{dealer.hand.handValues.Last().ToString()},{dealersDecisions},{doesPlayerSplit},{playersStartingSplitHandForfile},{playersStartingHardHandValueForFile},{playersStartingSplitSoftHandValueForFile}," +
                    $"{playersEndSplitHand},{playersEndSplitHandValue},{playersSplitHandDecisions},{playersStartingHandPreSplit},{fiveCount}"
                    );

                CleanupHand();
                //UpdateCount(ref int count, Face face);
                //UpdateCount(ref int count, int value);
                UpdateCounts();

                if (humanPlayer)
                {
                    Console.ReadKey();
                    Console.Clear();
                }
                handsPlayed++;
                
            }
            f.Close();
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");//CONSOLE HAND SEPERATOR
            Console.WriteLine($"Hands Played:\t{handsPlayed}");
            Console.WriteLine($"Final Chip Count:\t{player.Chips}");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");//CONSOLE HAND SEPERATOR

        }

        private void UpdateCounts()
        {
            count = 0;
            fiveCount = 0;
            if (countType == "five")
            {
                foreach (var c in burntCards)
                {
                    if (c.Face == Face.Five)
                    {
                        count++;
                        fiveCount++;
                    }
                }
            }
            
        }

        private StreamWriter InitialiseFile(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            StreamWriter file = new StreamWriter(filename, false);
            file.WriteLine("HandNumber,GameResult,SplitGameResult,PlayersStartingChips,EndChips,ChipsWon,CardsInDeckBeforeTurn,CardsInDeckAfterTurn," +
                "PlayerStake,PlayersStartingHand,PlayerStartingHardHandValue,PlayerStartingSoftHandValue,PlayersEndHand,PlayersEndHandValue,PlayersDecisions," +
                "DealersUpCard,DealersUpCardValue,DealersStartHand,DealersEndHand,DealersHardEndHandValue,DealersSoftEndHandValue,DealersEndValue,DealersDecisions," +
                "DoesPlayerSplit,PlayersStartingSplitHand,PlayerStartingSplitHardHandValue,PlayerStartingSplitSoftHandValue,PlayersEndSplitHand,PlayersSplitEndHandValue,PlayersSplitHandDecisions" +
                ",PlayersHandPreSplit,FiveCount");
            return file;
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

            //player = new BasicStrategy();
            //player = new DealerStrategy();
            player = new FiveCountStrategy
            {
                Chips = startChips
            };
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
