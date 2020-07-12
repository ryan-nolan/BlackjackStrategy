using System;
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
        public int handsToBePlayed = 1000;
        public int cardCountWhenShuffle = 26;
        public bool humanPlayer = false;

        public int minBet = 10;
        public int maxBet = 500;

        public int startChips = 500;

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
            string filename = $"blackjackStrategy_hands({handsToBePlayed})shuffleFrequency({cardCountWhenShuffle}).csv";
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
                amountOfCardsInDeckBeforeTurn = deck.Cards.Count;
                playersDecisions = "";
                playersSplitHandDecisions = "";
                dealersDecisions = "";
                doesPlayerSplit = "N";
                //TODO Player places bet
                //Get Bet function
                playerStartingChips = player.Chips;
                player.AddBet(player.CalculateBet(minBet, maxBet), ref player.Stake);
                playerStakeForFile = player.Stake; //FILE

                //Deal Cards
                DealHand();
                playersStartingHandPreSplit = player.hand.ToString();
                //Set Starting Hand Values for file
                playersStartingHardHandValueForFile = player.hand.handValues.First();
                if (player.hand.handValues.Count > 1) { playersStartingSoftHandValueForFile = player.hand.handValues.Last(); }
                else { playersStartingSoftHandValueForFile = 0; }

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
                        playersDecisions = "PLAYER_NATURAL";
                        Console.WriteLine("Game Result: Player Wins");
                        player.Chips += player.Stake * 2;
                        player.Stake = 0;
                    }
                    else
                    {
                        Console.WriteLine("Dealer Has a natural");
                        playersDecisions = "PLAYER_NATURAL";
                        dealersDecisions = "DEALER_NATURAL";
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
                        dealersDecisions = "DEALER_NATURAL";
                        Console.WriteLine("Game Result: Dealer Wins");
                        player.Stake = 0;
                        //TODO DEALER WINS
                    }
                    else
                    {

                    
                        Console.WriteLine("Player Has a Natural");
                        Console.WriteLine("Game Result: TIE");
                        playersDecisions = "PLAYER_NATURAL";
                        dealersDecisions = "DEALER_NATURAL";
                        player.Chips += player.Stake;
                        //TIE
                    }
                }
                else
                {

                    //Player reacts, can double down or split here
                    player.React(dealer.upCard, ref player.CurrentState, player.hand);
                    //playersDecisions += player.CurrentState+"/";
                    if (player.CurrentState == PlayerState.SPLIT)
                    {
                        playersDecisions += player.CurrentState + "/";
                        doesPlayerSplit = "Y";
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
                            playersDecisions += player.CurrentState+ "/";
                            playersSplitHandDecisions += player.splitHandState + "/";
                        }
                        else
                        { 
                            player.React(dealer.upCard, ref player.splitHandState, player.splitHand);
                            playersSplitHandDecisions += player.splitHandState + "/";
                        }
                        if (player.splitHandState == PlayerState.DOUBLE_DOWN)
                        {
                            player.AddBet(player.Stake, ref player.SplitHandStake);
                            HitPlayer(player.splitHand);
                            Console.WriteLine(player.splitHand.cards.Last());
                            if (player.hand.handValues.First() <= 21)
                            {
                                player.splitHandState = PlayerState.STAND;
                                playersSplitHandDecisions += player.splitHandState + "/";
                            }
                            else
                            {
                                player.splitHandState = PlayerState.BUST;
                                playersSplitHandDecisions += player.splitHandState + "/";
                            }
                        }
                        while (player.splitHandState != PlayerState.BUST && player.splitHandState != PlayerState.STAND)
                        {
                            if (player.splitHandState == PlayerState.HIT)
                            {
                                player.WriteCurrentState();
                                HitPlayer(player.splitHand);
                                if (player.hand.handValues.First() > 21)
                                {
                                    player.splitHandState = PlayerState.BUST;
                                }
                                Console.WriteLine(player.splitHand.ToString());
                            }
                            player.React(dealer.upCard, ref player.splitHandState, player.splitHand);
                            playersSplitHandDecisions += player.splitHandState + "/";
                        }
                        if (player.splitHandState == PlayerState.BUST)
                        {
                            player.SplitHandStake = 0;
                        }

                    }
                    player.React(dealer.upCard, ref player.CurrentState, player.hand);
                    if (player.splitHand != null && player.splitHand.cards.First().Face == Face.Ace)
                    {
                        player.CurrentState = PlayerState.STAND;
                    }
                    playersDecisions += player.CurrentState+"/";
                    if (player.CurrentState == PlayerState.DOUBLE_DOWN)
                    {
                        player.AddBet(player.Stake, ref player.Stake);
                        HitPlayer(player);
                        Console.WriteLine($"Player DOUBLES DOWN:\t{player.hand.cards.Last()}");
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
                        if (player.CurrentState == PlayerState.HIT)
                        {
                            HitPlayer(player);
                            Console.WriteLine($"Player Hits:\t{player.hand.cards.Last()}\t{player.hand.handValues.Last()}");
                            if (player.hand.handValues.First() > 21)
                            {
                                player.CurrentState = PlayerState.BUST;
                            }
                            player.WriteCurrentState();
                        }
                        player.React(dealer.upCard, ref player.CurrentState, player.hand);
                        playersDecisions += player.CurrentState + "/";
                    }
                    Console.WriteLine($"PLAYER REACTS:\t{player.CurrentState}\t{player.hand.handValues.Last()}");
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
                        dealersDecisions += dealer.CurrentState + "/";
                        while (dealer.CurrentState != PlayerState.BUST && dealer.CurrentState != PlayerState.STAND)
                        {
                            if (dealer.CurrentState == PlayerState.HIT)
                            {
                                dealer.WriteCurrentState();
                                HitPlayer(dealer);
                                Console.WriteLine(dealer.hand.cards.Last());
                            }
                            dealer.React();
                            dealersDecisions += dealer.CurrentState + "/";
                        }
                        Console.WriteLine($"DEALER REACTS:\t{dealer.CurrentState}\t{dealer.hand.handValues.Last()}");
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
                            Console.WriteLine($"Player Chips:\t{player.Chips}");
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

                            //DO FILE STUFF
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
                }
                f.WriteLine($"{handsPlayed+1},{playerStartingChips},{player.Chips},{player.Chips - playerStartingChips},{amountOfCardsInDeckBeforeTurn},{deck.Cards.Count}," +
                    $"{playerStakeForFile},{player.hand.cards[0].ToString()} {player.hand.cards[1].ToString()}," +
                    $"{playersStartingHardHandValueForFile},{playersStartingSoftHandValueForFile},{player.hand},{player.hand.handValues.Last()},{playersDecisions}," +
                    $"{dealer.upCard},{dealer.hand.cards.First().Value},{dealer.hand.cards[0]} {dealer.hand.cards[1]},{dealer.hand},{dealer.hand.handValues.First()},{dealer.hand.handValues.Last()}," +
                    $"{dealer.hand.handValues.Last().ToString()},{dealersDecisions},{doesPlayerSplit},{playersStartingSplitHandForfile},{playersStartingHardHandValueForFile},{playersStartingSplitSoftHandValueForFile}," +
                    $"{playersEndSplitHand},{playersEndSplitHandValue},{playersSplitHandDecisions},{playersStartingHandPreSplit}"
                    );

                CleanupHand();
                if (humanPlayer)
                {
                    Console.ReadKey();
                    Console.Clear();
                }
                handsPlayed++;
                
            }
            f.Close();
            Console.WriteLine(handsPlayed);
            Console.WriteLine(player.Chips);

        }

        private StreamWriter InitialiseFile(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            StreamWriter file = new StreamWriter(filename, false);
            file.WriteLine("HandNumber,PlayersStartingChips,EndChips,ChipsWon,CardsInDeckBeforeTurn,CardsInDeckAfterTurn," +
                "PlayerStake,PlayersStartingHand,PlayerStartingHardHandValue,PlayerStartingSoftHandValue,PlayersEndHand,PlayersEndHandValue,PlayersDecisions," +
                "DealersUpCard,DealersUpCardValue,DealersStartHand,DealersEndHand,DealersHardEndHandValue,DealersSoftEndHandValue,DealersEndValue,DealersDecisions," +
                "DoesPlayerSplit,PlayersStartingSplitHand,PlayerStartingSplitHardHandValue,PlayerStartingSplitSoftHandValue,PlayersEndSplitHand,PlayersSplitEndHandValue,PlayersSplitHandDecisions" +
                ",PlayersHandPreSplit");
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

            player = new BasicStrategy();
            //player = new DealerStrategy();
            player.Chips = startChips;
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
