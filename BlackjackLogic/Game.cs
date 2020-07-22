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

        public int HandsPlayed = 0;
        public int HandsToBePlayed = 50000;
        public int CardCountWhenToShuffle = 13;
        public bool humanPlayer = false;
        public int MinBet = 2;
        public int MaxBet = 50;
        public string StrategyName = "simplepointcount";
        public int StartChips = 500;
        public int DeckSize = 52;

        //Counts
        List<int> count = new List<int>() { 0, 0 };
        //string countType = "five";
        

        public Deck deck;
        public List<Card> burntCards = new List<Card>();

        public Player player;
        public Dealer dealer;

        public Game(string strategyName = null, int handsToBePlayed = 50000, int cardCountWhenToShuffle = 13, int minBet = 2, int maxBet = 50, int startChips = 500, int deckSize = 52)
        {
            HandsToBePlayed = handsToBePlayed;
            CardCountWhenToShuffle = cardCountWhenToShuffle;
            MinBet = minBet;
            MaxBet = maxBet;
            if (strategyName != null)
            {
                StrategyName = strategyName;
            }
            StartChips = startChips;
            DeckSize = deckSize;
        }
        public void RunGame()
        {
            //Game is initialised   //Parameters are set //Player strategy is chosen here
            //Deck is made
            //Cards are shuffled
            InitialiseGame(StrategyName);

            //Create file and write first line
            string path = @"C:\Users\Ryan\Desktop\Dissertation\Data";
            string filename = $"{player.StrategyName}_hands({HandsToBePlayed})shuffleFrequency({CardCountWhenToShuffle})deckSize({DeckSize}).csv";
            StreamWriter f = InitialiseFile(filename, path);


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
            int firstCountBeforeHandForFile = 0;
            int firstCountAfterHandForFile = 0;
            //A card is burnt
            burntCards.Add(deck.Cards.Pop());

            for (int i = 0; i < HandsToBePlayed; i++)
            {
                Console.WriteLine($"-------------------------------------------------Hand Number:\t{HandsPlayed}---------------------------------------------------");//CONSOLE HAND SEPERATOR
                //shuffle check     Shuffles first turn available when cards left are smaller than the number specified
                if (deck.Cards.Count < CardCountWhenToShuffle)
                {
                    try
                    {
                        deck = new Deck(DeckSize);
                    }
                    catch (Exception e)
                    {

                        throw e;
                    }
                    deck.Shuffle();
                    burntCards.Clear();
                    burntCards.Add(deck.Cards.Pop());
                    count = player.UpdateCount(deck, burntCards, dealer.upCard);
                }
                Console.Write($"Pre hand counts:\t");
                for(int j = 0; j < count.Count; j++)
                {
                    Console.Write($"{count[j]}\t");
                }
                Console.WriteLine();
                firstCountBeforeHandForFile = count[0];
                string currentTurnDeckHash = deck.GetDeckHash();
                amountOfCardsInDeckBeforeTurn = deck.Cards.Count;

                playersDecisions = "";
                playersSplitHandDecisions = "";
                dealersDecisions = "";
                doesPlayerSplit = "N";

                playerStartingChips = player.Chips;//FOR FILE
                //Get Bet function
                player.AddBet(player.CalculateBet(MinBet, MaxBet, count), ref player.Stake);
                Console.WriteLine($"Player's starting stake:\t{player.Stake}");
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
                player.UpdateCount(deck, burntCards, dealer.upCard);
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
                        Console.WriteLine($"Player Split Hand Stake:\t{player.SplitHandStake}");
                        
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
                            Console.WriteLine($"Player's split stake after double down:\t{player.SplitHandStake}");
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
                        Console.WriteLine($"Player's stake after DD:\t{player.Stake}");
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
                            if (player.splitHand != null && player.splitHandState != PlayerState.BUST)
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
                firstCountAfterHandForFile = player.UpdateCount(deck, burntCards, dealer.upCard).First();
                f.WriteLine($"{HandsPlayed+1},{playerStartingChips},{player.Chips},{player.Chips - playerStartingChips},{gameResult},{splitGameResult},{amountOfCardsInDeckBeforeTurn},{deck.Cards.Count}," +
                    $"{playerStakeForFile},{player.hand.cards[0].ToString()} {player.hand.cards[1].ToString()}," +
                    $"{playersStartingHardHandValueForFile},{playersStartingSoftHandValueForFile},{player.hand},{player.hand.handValues.Last()},{playersDecisions}," +
                    $"{dealer.upCard},{dealer.hand.cards.First().Value},{dealer.hand.cards[0]} {dealer.hand.cards[1]},{dealer.hand},{dealer.hand.handValues.First()},{dealer.hand.handValues.Last()}," +
                    $"{dealer.hand.handValues.Last().ToString()},{dealersDecisions},{doesPlayerSplit},{playersStartingSplitHandForfile},{playersStartingHardHandValueForFile},{playersStartingSplitSoftHandValueForFile}," +
                    $"{playersEndSplitHand},{playersEndSplitHandValue},{playersSplitHandDecisions},{playersStartingHandPreSplit}," +
                    $"{firstCountBeforeHandForFile},{firstCountAfterHandForFile},{count[1]},{currentTurnDeckHash}"
                    );

                CleanupHand();
                count = player.UpdateCount(deck, burntCards, dealer.upCard);

                if (humanPlayer)
                {
                    Console.ReadKey();
                    Console.Clear();
                }
                HandsPlayed++;
                
            }
            f.Close();
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");//CONSOLE HAND SEPERATOR
            Console.WriteLine($"Strategy:\t{StrategyName}");
            Console.WriteLine($"Deck Size:\t{DeckSize}");
            Console.WriteLine($"Hands Played:\t{HandsPlayed}");
            Console.WriteLine($"Final Chip Count:\t{player.Chips}");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");//CONSOLE HAND SEPERATOR

        }

        private StreamWriter InitialiseFile(string filename, string path)
        {
            if (File.Exists(path+ "\\" + filename))
            {
                File.Delete(path + "\\" + filename);
            }
            StreamWriter file = new StreamWriter(path+"\\"+filename, false);
            file.WriteLine("HandNumber,PlayersStartingChips,EndChips,ChipsWon,GameResult,SplitGameResult,CardsInDeckBeforeTurn,CardsInDeckAfterTurn," +
                "PlayerStake,PlayersStartingHand,PlayerStartingHardHandValue,PlayerStartingSoftHandValue,PlayersEndHand,PlayersEndHandValue,PlayersDecisions," +
                "DealersUpCard,DealersUpCardValue,DealersStartHand,DealersEndHand,DealersHardEndHandValue,DealersSoftEndHandValue,DealersEndValue,DealersDecisions," +
                "DoesPlayerSplit,PlayersStartingSplitHand,PlayerStartingSplitHardHandValue,PlayerStartingSplitSoftHandValue,PlayersEndSplitHand,PlayersSplitEndHandValue,PlayersSplitHandDecisions" +
                ",PlayersHandPreSplit,CountBeforeHand,CountDuringHand,Count[1],DeckHash");
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
            dealer.upCard = null;
            player.Stake = 0;
            player.SplitHandStake = 0;
        }

        private void InitialiseGameAsPlayer()
        {
            deck = new Deck(DeckSize);
            deck.Shuffle();

            player = new HumanStrategy();
            dealer = new Dealer();
            humanPlayer = true;
        }
        private void InitialiseGame(string strategy)
        {
            deck = new Deck(DeckSize);
            deck.Shuffle();
            switch (strategy.ToLower())
            {
                case "basicstrategy":
                    player = new BasicStrategy
                    {
                        Chips = StartChips
                    };
                    break;
                case "acetofive":
                    player = new AceToFiveStrategy
                    {
                        Chips = StartChips
                    };
                    break;
                case "dealer":
                    player = new DealerStrategy
                    {
                        Chips = StartChips
                    };
                    break;
                case "fivecount":
                    player = new FiveCountStrategy
                    {
                        Chips = StartChips
                    };
                    break;
                case "simplepointcount":
                    player = new SimplePointCountStrategy
                    {
                        Chips = StartChips
                    };
                    break;
                case "tencount":
                    player = new TenCountStrategy
                    {
                        Chips = StartChips
                    };
                    break;
                case "completepointcount":
                    player = new CompletePointCountStrategy
                    {
                        Chips = StartChips
                    };
                    break;
                default:
                    player = new SimplePointCountStrategy
                    {
                        Chips = StartChips
                    };
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
            count = player.UpdateCount(deck, burntCards, dealer.upCard);
        }


        public void GameTest()
        {
            InitialiseGame(strategy: StrategyName);

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
