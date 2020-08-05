using BlackjackLogic.Strategies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlackjackLogic
{
    public class Game
    {
        //Changable parameters by user
        public int HandsPlayed = 0;                      //The current hand played number
        public int HandsToBePlayed = 50000;             //The amount of hands wanted to be played
        public int CardCountWhenToShuffle = 13;         //Shuffle when val < this numner
        public bool humanPlayer = false;                //True if game is played by human
        public int MinBet = 2;                          //Min bet that can be placed
        public int MaxBet = 50;                         //Max bet that can be placed
        public string StrategyName = "simplepointcount";//The strategy name for documnetation
        public int StartChips = 500;                    //Chips player should start on
        public int DeckSize = 52;                       //Size of deck, must be multiple of 52
        public string Path = @".\Data\";
        //Deck and burnt card variables for game
        public Deck deck;
        public List<Card> burntCards = new List<Card>();
        //Player and dealer objects, to be assigned in InitialiseGame()
        public Player player;
        public Dealer dealer;

        /// <summary>
        /// Game constructor, takes in game parameters for main file and changes the game paramaters accordingly
        /// </summary>
        /// <param name="strategyName"></param>
        /// <param name="handsToBePlayed"></param>
        /// <param name="cardCountWhenToShuffle"></param>
        /// <param name="minBet"></param>
        /// <param name="maxBet"></param>
        /// <param name="startChips"></param>
        /// <param name="deckSize"></param>
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
        /// <summary>
        /// Launches simulation with chosen parameters
        /// </summary>
        public void RunGame()
        {
            //Game is initialised   //Parameters are set //Player strategy is chosen here
            //Deck is made
            //Cards are shuffled
            InitialiseGame(StrategyName);

            //Create file and write first line
            //File saved in data folder outside source
            //string path = @"C:\Users\Ryan\Desktop\Dissertation\Source\Data";
            string filename = $"{player.StrategyName}_hands({HandsToBePlayed})shuffleFrequency({CardCountWhenToShuffle})deckSize({DeckSize}).csv";
            StreamWriter f = InitialiseFile(filename, Path);

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

            //Play n hands, where n is the amount of hands to be played chosen by user
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
                        //Throws exception if deck cant be made
                        throw e;
                    }
                    //Shuffle deck and burn a card
                    deck.FisherYatesShuffle();
                    burntCards.Clear();
                    burntCards.Add(deck.Cards.Pop());
                    //Update count according to burnt card
                    player.UpdateCount(deck, burntCards, dealer.upCard);
                }
                //Write pre hand counts to console
                Console.Write($"Pre hand counts:\t");
                for (int j = 0; j < player.Count.Count; j++)
                {
                    Console.Write($"{player.Count[j]}\t");
                }
                Console.WriteLine();
                //Store counts and deck has for file
                firstCountBeforeHandForFile = player.Count[0];
                string currentTurnDeckHash = deck.GetDeckHash();
                amountOfCardsInDeckBeforeTurn = deck.Cards.Count;

                playersDecisions = "";//FOR FILE
                playersSplitHandDecisions = "";//FOR FILE
                dealersDecisions = "";//FOR FILE
                doesPlayerSplit = "N"; //FOR FILE

                playerStartingChips = player.Chips;//FOR FILE

                //Get Bet function
                player.AddBet(player.CalculateBet(MinBet, MaxBet), ref player.Stake);
                int CountZeroAtTimeOfBet = player.Count[0]; //FOR FILE
                int CountOneAtTimeOfBet = player.Count[1]; //FOR FILE
                //Write player stake to console
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
                    //Player has natural and dealer doesn't
                    if (!dealer.hand.handValues.Contains(21))
                    {
                        Console.WriteLine("Player Has a Natural");
                        playersDecisions = "PLAYER_NATURAL";
                        Console.WriteLine("Game Result: Player Wins");
                        player.Chips += (int)(player.Stake * 2.5);
                        player.Stake = 0;
                        //Player wins
                        gameResult = "W_N";
                    }
                    //Player and dealer have natural
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
                    //Dealer has natural and player doesn't
                    if (!player.hand.handValues.Contains(21))
                    {
                        Console.WriteLine("Dealer Has a natural");
                        dealersDecisions = "DEALER_NATURAL";
                        Console.WriteLine("Game Result: Dealer Wins");
                        player.Stake = 0;
                        //Player loses
                        gameResult = "L_N";
                    }
                    //Player and dealer have a natural
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
                //If no naturals continue playing
                else
                {
                    //Player reacts, can double down or split here
                    player.React(dealer.upCard, ref player.CurrentState, player.hand, player.Count);

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
                        //Write split hands to console
                        Console.WriteLine($"Player's Hand:\t{player.hand.ToString()}");
                        Console.WriteLine($"Player's Split Hand:\t{player.splitHand.ToString()}");

                        //Add splitBet stake so player is playing two hands for twice his stake
                        player.AddBet(player.Stake, ref player.SplitHandStake);
                        Console.WriteLine($"Player Split Hand Stake:\t{player.SplitHandStake}");

                        //Playing the split hand
                        //Split aces forces stand rule
                        if (player.hand.cards.First().Face == Face.Ace && player.splitHand.cards.First().Face == Face.Ace)
                        {
                            player.CurrentState = PlayerState.STAND;
                            player.splitHandState = PlayerState.STAND;
                            playersDecisions += player.CurrentState + "/";//FOR FILE
                            playersSplitHandDecisions += player.splitHandState + "/";//FOR FILE
                        }
                        else
                        {
                            player.React(dealer.upCard, ref player.splitHandState, player.splitHand, player.Count);
                            playersSplitHandDecisions += player.splitHandState + "/";//FOR FILE
                        }
                        //Player doubles down on split hand
                        if (player.splitHandState == PlayerState.DOUBLE_DOWN)
                        {
                            //Double his split hand stake
                            player.AddBet(player.SplitHandStake, ref player.SplitHandStake);
                            Console.WriteLine($"Player's split stake after double down:\t{player.SplitHandStake}");
                            //Hit player
                            HitPlayer(player.splitHand);
                            Console.WriteLine($"Player split double downs with:\t{player.splitHand.cards.Last()}");
                            //If player < 21 STAND
                            if (player.hand.handValues.First() <= 21)
                            {
                                player.splitHandState = PlayerState.STAND;
                                playersSplitHandDecisions += player.splitHandState + "/";//FOR FILE
                            }
                            //If player > 21 BUST
                            else
                            {
                                player.splitHandState = PlayerState.BUST;
                                playersSplitHandDecisions += player.splitHandState + "/";//FOR FILE
                            }
                        }
                        //While the player isn't standing or busting
                        while (player.splitHandState != PlayerState.BUST && player.splitHandState != PlayerState.STAND)
                        {
                            //Hit player is it reacts hit
                            if (player.splitHandState == PlayerState.HIT)
                            {
                                player.WriteCurrentState();
                                HitPlayer(player.splitHand);
                                //BUST CHECK
                                if (player.splitHand.handValues.First() > 21)
                                {
                                    player.splitHandState = PlayerState.BUST;
                                }
                                Console.WriteLine($"Player's split hand:\t{player.splitHand.ToString()}\t{player.splitHand.handValues.Last()}");
                            }
                            player.React(dealer.upCard, ref player.splitHandState, player.splitHand, player.Count);
                            playersSplitHandDecisions += player.splitHandState + "/";
                        }
                        //If player busts on split hand, lose split hand stake
                        if (player.splitHandState == PlayerState.BUST)
                        {
                            player.SplitHandStake = 0;
                            splitGameResult = "L";
                        }

                    }
                    //PLAY REGULAR HAND
                    player.React(dealer.upCard, ref player.CurrentState, player.hand, player.Count);
                    //IF PLAYER SPLIT ACES, STAND
                    if (player.splitHand != null && player.splitHand.cards.First().Face == Face.Ace)
                    {
                        player.CurrentState = PlayerState.STAND;
                    }
                    playersDecisions += player.CurrentState + "/";//FOR FILE

                    //IF PLAYER DOUBLE DOWNS, STAND OR BUST
                    if (player.CurrentState == PlayerState.DOUBLE_DOWN)
                    {
                        player.AddBet(player.Stake, ref player.Stake);
                        HitPlayer(player);
                        Console.WriteLine($"Player DOUBLES DOWN:\t{player.hand.cards.Last()}");
                        Console.WriteLine($"Player's stake after DD:\t{player.Stake}");
                        Console.WriteLine($"Player's Hand:\t{player.hand.ToString()}\t{player.hand.handValues.Max()}");
                        //STAND IF DOUBLE DOWN AND PLAYER < 21
                        if (player.hand.handValues.First() <= 21)
                        {
                            player.CurrentState = PlayerState.STAND;
                        }
                        //BUST IF DOUBLE DOWN AND PLAYER < 21
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
                        }
                        player.React(dealer.upCard, ref player.CurrentState, player.hand, player.Count);
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
                        //DEALER PLAYS HAND IF PLAYER DOESN'T LOSE
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
                            //If player hand value > dealers player wins
                            if (player.hand.handValues.Last() > dealer.hand.handValues.Last())
                            {
                                Console.WriteLine("Game Result: Player Wins");
                                player.Chips += player.Stake * 2;
                                //Player Wins
                                gameResult = "W";
                            }
                            //If player hand value < dealers player wins
                            else if (player.hand.handValues.Last() < dealer.hand.handValues.Last())
                            {
                                Console.WriteLine("Game Result: Dealer Wins");
                                gameResult = "L";
                            }
                            //If player hand value == dealers player ties
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
                //If there was a split hand this turn, document split hand values
                if (player.splitHand != null)
                {
                    playersStartingSplitHandForfile = player.splitHand.cards[0].ToString() + " " + player.splitHand.cards[1].ToString();
                    playersStartingSplitHardHandValueForFile = player.splitHand.handValues.First().ToString();
                    playersStartingSplitSoftHandValueForFile = player.splitHand.handValues.Last().ToString();
                    playersEndSplitHand = player.splitHand.ToString();
                    playersEndSplitHandValue = player.splitHand.handValues.Last().ToString();
                }
                //If no split hand change all split hand values to N/A
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
                //Write all relevant infromation to file after every hand
                f.WriteLine($"{HandsPlayed + 1},{playerStartingChips},{player.Chips},{player.Chips - playerStartingChips},{gameResult},{splitGameResult},{amountOfCardsInDeckBeforeTurn},{deck.Cards.Count}," +
                    $"{playerStakeForFile},{player.hand.cards[0].ToString()} {player.hand.cards[1].ToString()}," +
                    $"{playersStartingHardHandValueForFile},{playersStartingSoftHandValueForFile},{player.hand},{player.hand.handValues.Last()},{playersDecisions}," +
                    $"{dealer.upCard},{dealer.hand.cards.First().Value},{dealer.hand.cards[0]} {dealer.hand.cards[1]},{dealer.hand},{dealer.hand.handValues.First()},{dealer.hand.handValues.Last()}," +
                    $"{dealer.hand.handValues.Last().ToString()},{dealersDecisions},{doesPlayerSplit},{playersStartingSplitHandForfile},{playersStartingHardHandValueForFile},{playersStartingSplitSoftHandValueForFile}," +
                    $"{playersEndSplitHand},{playersEndSplitHandValue},{playersSplitHandDecisions},{playersStartingHandPreSplit}," +
                    $"{firstCountBeforeHandForFile},{firstCountAfterHandForFile},{CountZeroAtTimeOfBet},{CountOneAtTimeOfBet},{currentTurnDeckHash}"
                    );
                //Cleanup hand
                CleanupHand();
                //Update Count
                player.UpdateCount(deck, burntCards, dealer.upCard);
                //If human is playing await input
                if (humanPlayer)
                {
                    Console.ReadKey();
                    Console.Clear();
                }
                HandsPlayed++;

            }
            //Close file when all hands are played and write simulation summary to file
            f.Close();
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");//CONSOLE HAND SEPERATOR
            Console.WriteLine($"Strategy:\t{StrategyName}");
            Console.WriteLine($"Deck Size:\t{DeckSize}");
            Console.WriteLine($"Hands Played:\t{HandsPlayed}");
            Console.WriteLine($"Final Chip Count:\t{player.Chips}");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");//CONSOLE HAND SEPERATOR

        }
        /// <summary>
        /// Initialises file in data directory with name filename
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private StreamWriter InitialiseFile(string filename, string path)
        {
            if (File.Exists(path + "\\" + filename))
            {
                File.Delete(path + "\\" + filename);
            }
            StreamWriter file = new StreamWriter(path + "\\" + filename, false);
            file.WriteLine("HandNumber,PlayersStartingChips,EndChips,ChipsWon,GameResult,SplitGameResult,CardsInDeckBeforeTurn,CardsInDeckAfterTurn," +
                "PlayerStake,PlayersStartingHand,PlayerStartingHardHandValue,PlayerStartingSoftHandValue,PlayersEndHand,PlayersEndHandValue,PlayersDecisions," +
                "DealersUpCard,DealersUpCardValue,DealersStartHand,DealersEndHand,DealersHardEndHandValue,DealersSoftEndHandValue,DealersEndValue,DealersDecisions," +
                "DoesPlayerSplit,PlayersStartingSplitHand,PlayerStartingSplitHardHandValue,PlayerStartingSplitSoftHandValue,PlayersEndSplitHand,PlayersSplitEndHandValue,PlayersSplitHandDecisions" +
                ",PlayersHandPreSplit,CountBeforeHand,CountDuringHand,Count[0]AtTimeOfBet,Count[1]AtTimeOfBet,DeckHash");
            return file;
        }
        /// <summary>
        /// Updates player and dealer hand values
        /// </summary>
        public void UpdateHandValues()
        {
            player.hand.SetHandValues();
            dealer.hand.SetHandValues();
        }
        /// <summary>
        /// Cleans up hands, adds all played cards to burnt cards
        /// Sets split hand to null
        /// </summary>
        private void CleanupHand()
        {
            foreach (var c in player.hand.cards)
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
        /// <summary>
        /// Defunct method for manually playing blackjack
        /// Out of scope for software
        /// </summary>
        private void InitialiseGameAsPlayer()
        {
            deck = new Deck(DeckSize);
            deck.FisherYatesShuffle();

            player = new HumanStrategy();
            dealer = new Dealer();
            humanPlayer = true;
        }
        /// <summary>
        /// Parses a switch statement with a given strategy name
        /// Assigns strategy according to name
        /// </summary>
        /// <param name="strategy"></param>
        private void InitialiseGame(string strategy)
        {
            deck = new Deck(DeckSize);
            deck.FisherYatesShuffle();
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
                case "knockoutcount":
                    player = new KnockoutCountStrategy
                    {
                        Chips = StartChips
                    };
                    break;
                default:
                    Console.WriteLine("Strategy Not Found");
                    throw new Exception();
                    break;

            }
            dealer = new Dealer();
        }

        /// <summary>
        /// Deals hand to player and dealer, 2 cards each
        /// Then sets values
        /// </summary>
        public void DealHand()
        {
            try
            {
                player.hand.cards.Add(deck.Cards.Pop());
                player.hand.cards.Add(deck.Cards.Pop());
                dealer.hand.cards.Add(deck.Cards.Pop());
                dealer.hand.cards.Add(deck.Cards.Pop());

                player.hand.SetHandValues();
                dealer.hand.SetHandValues();
            }
            catch (InvalidOperationException)
            {
                try
                {
                    deck = new Deck(DeckSize);
                }
                catch (Exception e)
                {
                    //Throws exception if deck cant be made
                    throw e;
                }
                //Shuffle deck and burn a card
                deck.FisherYatesShuffle();
                burntCards.Clear();
                burntCards.Add(deck.Cards.Pop());

                player.hand.cards.Clear();
                dealer.hand.cards.Clear();

                player.hand.cards.Add(deck.Cards.Pop());
                player.hand.cards.Add(deck.Cards.Pop());
                dealer.hand.cards.Add(deck.Cards.Pop());
                dealer.hand.cards.Add(deck.Cards.Pop());

                player.hand.SetHandValues();
                dealer.hand.SetHandValues();

                player.UpdateCount(deck, burntCards, dealer.upCard);

            }
            
        }
        /// <summary>
        /// Hits an Actor with an extra card
        /// </summary>
        /// <param name="actor"></param>
        public void HitPlayer(Actor actor)
        {
            try
            {
                actor.hand.cards.Add(deck.Cards.Pop());
                actor.hand.SetHandValues();
            }
            catch (InvalidOperationException)
            {
                List<Card> playersCards = new List<Card>();
                List<Card> dealersCards = new List<Card>();

                foreach (var c in player.hand.cards)
                {
                    playersCards.Add(c.Clone());
                }
                foreach (var c in dealer.hand.cards)
                {
                    dealersCards.Add(c.Clone());
                }

                try
                {
                    deck = new Deck(DeckSize);
                }
                catch (Exception e)
                {
                    //Throws exception if deck cant be made
                    throw e;
                }
                //Shuffle deck and burn a card
                //deck.FisherYatesShuffle();
                burntCards.Clear();

                List<Card> shuffledDeck = deck.Cards.ToList();
                player.hand.cards.Clear();
                dealer.hand.cards.Clear();
                foreach (var c in playersCards)
                {
                    player.hand.cards.Add(c.Clone());
                    shuffledDeck.RemoveAll(x => x.Face == c.Face && x.Suit == c.Suit);
                }
                foreach (var c in dealersCards)
                {
                    dealer.hand.cards.Add(c.Clone());
                    shuffledDeck.RemoveAll(x => x.Face == c.Face && x.Suit == c.Suit);
                }
                deck.Cards.Clear();
                foreach (var c in shuffledDeck)
                {
                    deck.Cards.Push(c);
                }
                deck.FisherYatesShuffle();
                burntCards.Add(deck.Cards.Pop());

                actor.hand.cards.Add(deck.Cards.Pop());
                actor.hand.SetHandValues();

                player.UpdateCount(deck, burntCards, dealer.upCard);

            }

        }
        /// <summary>
        /// Hits a hand with an extra card
        /// </summary>
        /// <param name="hand"></param>
        public void HitPlayer(Hand hand)
        { 
            try
            {
                hand.cards.Add(deck.Cards.Pop());
                hand.SetHandValues();
                player.UpdateCount(deck, burntCards, dealer.upCard);
            }
            catch (InvalidOperationException)
            {
                List<Card> playersCards = new List<Card>();
                List<Card> dealersCards = new List<Card>();

                foreach (var c in player.hand.cards)
                {
                    playersCards.Add(c.Clone());
                }
                foreach (var c in dealer.hand.cards)
                {
                    dealersCards.Add(c.Clone());
                }

                try
                {
                    deck = new Deck(DeckSize);
                }
                catch (Exception e)
                {
                    //Throws exception if deck cant be made
                    throw e;
                }
                //Shuffle deck and burn a card
                //deck.FisherYatesShuffle();
                burntCards.Clear();

                List<Card> shuffledDeck = deck.Cards.ToList();

                foreach (var c in playersCards)
                {
                    player.hand.cards.Add(c.Clone());
                    shuffledDeck.RemoveAll(x => x.Face == c.Face && x.Suit == c.Suit);
                }
                foreach (var c in dealersCards)
                {
                    dealer.hand.cards.Add(c.Clone());
                    shuffledDeck.RemoveAll(x => x.Face == c.Face && x.Suit == c.Suit);
                }
                deck.Cards.Clear();
                foreach (var c in shuffledDeck)
                {
                    deck.Cards.Push(c);
                }
                deck.FisherYatesShuffle();
                burntCards.Add(deck.Cards.Pop());

                hand.cards.Add(deck.Cards.Pop());
                hand.SetHandValues();
                player.UpdateCount(deck, burntCards, dealer.upCard);

            }
        }

    }
}
