using BlackjackLogic.Strategies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlackjackLogic.Game;

namespace BlackjackLogic
{
    public class Simulator
    {
        private int HandsPlayed { get; set; } //The current hand played number
        private SimulatorGameOptions _options;
        private List<Card> _burntCards = new List<Card>();

        //Player and dealer objects, to be assigned in InitialiseGame()
        public Player Player;
        public Dealer Dealer;
        //Deck and burnt card variables for game
        public Deck Deck;
        public BlackjackData blackjackData;

        /// <summary>
        /// Game constructor, takes in game parameters for main file and changes the game parameters accordingly
        /// </summary>
        /// <param name="strategyName"></param>
        /// <param name="handsToBePlayed"></param>
        /// <param name="cardCountWhenToShuffle"></param>
        /// <param name="minBet"></param>
        /// <param name="maxBet"></param>
        /// <param name="startChips"></param>
        /// <param name="deckSize"></param>
        public Simulator(string strategyName = null, int handsToBePlayed = 50000, int cardCountWhenToShuffle = 13, int minBet = 2, int maxBet = 50, int startChips = 500, int deckSize = 52)
        {
            _options = new SimulatorGameOptions(handsToBePlayed, cardCountWhenToShuffle, minBet, maxBet, strategyName, startChips, deckSize)
            {
                StrategyName = strategyName ?? "BasicStrategy"
            };
            blackjackData = new BlackjackData();
        }
        public Simulator(SimulatorGameOptions options)
        {
            this._options = options;
            blackjackData = new BlackjackData();
        }
        
        /// <summary>
        /// Launches simulation with chosen parameters
        /// </summary>
        public void RunGame()
        {
            //Game is initialised   
            InitialiseGame(_options.StrategyName);

            //Create file and write first line
            //File saved in data folder
            string filename = $"{Player.StrategyName}_hands({_options.HandsToBePlayed})shuffleFrequency({_options.CardCountWhenToShuffle})deckSize({_options.DeckSize}).csv";
            StreamWriter f = InitialiseStreamWriter(filename, _options.FilePath);
            //BlackjackHandData blackjackHandData = new BlackjackHandData();

            //A card is burnt
            _burntCards.Add(Deck.Cards.Pop());

            //Play n hands, where n is the amount of hands to be played chosen by user
            for (int i = 0; i < _options.HandsToBePlayed; i++)
            {
                BlackjackHandData blackjackHandData = new BlackjackHandData();
                Console.WriteLine($"-------------------------------------------------Hand Number:\t{HandsPlayed}---------------------------------------------------");//CONSOLE HAND SEPARATOR
                //shuffle check     Shuffles first turn available when cards left are smaller than the number specified
                if (Deck.Cards.Count < _options.CardCountWhenToShuffle)
                {
                    Deck = new Deck(_options.DeckSize);

                    //Shuffle deck and burn a card
                    Deck.FisherYatesShuffle();
                    _burntCards.Clear();
                    _burntCards.Add(Deck.Cards.Pop());

                    //Update count according to burnt card
                    Player.UpdateCount(Deck, _burntCards, Dealer.upCard);
                }

                //Write pre hand counts to console
                Console.Write("Pre hand counts:\t");
                foreach (var t in Player.Count)
                {
                    Console.Write($"{t}\t");
                }
                Console.WriteLine();

                //Store counts and deck has for file
                blackjackHandData.FirstCountBeforeHandForFile = Player.Count[0];
                blackjackHandData.CurrentTurnDeckHash = Deck.GetDeckHash();
                blackjackHandData.AmountOfCardsInDeckBeforeTurn = Deck.Cards.Count;
                blackjackHandData.PlayersDecisions = "";//FOR FILE
                blackjackHandData.PlayersSplitHandDecisions = "";//FOR FILE
                blackjackHandData.DealersDecisions = "";//FOR FILE
                blackjackHandData.DoesPlayerSplit = "N"; //FOR FILE
                blackjackHandData.PlayerStartingChips = Player.Chips;//FOR FILE

                //Get Bet function
                Player.AddBet(Player.CalculateBet(_options.MinBet, _options.MaxBet), ref Player.Stake);
                blackjackHandData.CountZeroAtTimeOfBet = Player.Count[0]; //FOR FILE
                blackjackHandData.CountOneAtTimeOfBet = Player.Count[1]; //FOR FILE
                //Write player stake to console
                Console.WriteLine($"Player's starting stake:\t{Player.Stake}");
                blackjackHandData.PlayerStakeForFile = Player.Stake; //FOR FILE

                //Deal Cards
                DealHand();
                blackjackHandData.PlayersStartingHandPreSplit = Player.hand.ToString();//FOR FILE

                //Set Starting Hand Values for file
                blackjackHandData.PlayersStartingHardHandValueForFile = Player.hand.handValues.First();//FOR FILE
                blackjackHandData.PlayersStartingSoftHandValueForFile = 
                    Player.hand.handValues.Count > 1 ? Player.hand.handValues.Last() : 0;

                //Dealer reveals upcard //Reference to first card in hand
                Dealer.upCard = Dealer.hand.cards[0];
                Console.WriteLine($"Dealer's Up Card is: {Dealer.upCard}");
                Console.WriteLine($"Player's cards are: {Player.hand}");

                //Check naturals
                UpdateHandValues();
                Player.UpdateCount(Deck, _burntCards, Dealer.upCard);
                if (CheckNaturals(ref blackjackHandData))
                {

                }
                else
                {
                    //Player reacts, can double down or split here
                    Player.React(Dealer.upCard, ref Player.CurrentState, Player.hand, Player.Count);

                    //PLAY SPLIT HANDS
                    if (Player.CurrentState == PlayerState.Split)
                    {
                        blackjackHandData.PlayersDecisions += Player.CurrentState + "/";//FOR FILE
                        blackjackHandData.DoesPlayerSplit = "Y";//FOR FILE

                        //Splitting the hands
                        Player.splitHand = new Hand();
                        Player.splitHand.cards.Add(Player.hand.cards.Last());
                        Player.hand.cards.Remove(Player.splitHand.cards.First());
                        HitPlayer(Player);
                        HitPlayer(Player.splitHand);
                        //Write split hands to console
                        Console.WriteLine($"Player's Hand:\t{Player.hand}");
                        Console.WriteLine($"Player's Split Hand:\t{Player.splitHand}");

                        //Add splitBet stake so player is playing two hands for twice his stake
                        Player.AddBet(Player.Stake, ref Player.SplitHandStake);
                        Console.WriteLine($"Player Split Hand Stake:\t{Player.SplitHandStake}");

                        //Playing the split hand
                        //Split aces forces stand rule
                        if (Player.hand.cards.First().Face == Face.Ace && Player.splitHand.cards.First().Face == Face.Ace)
                        {
                            Player.CurrentState = PlayerState.Stand;
                            Player.splitHandState = PlayerState.Stand;
                            blackjackHandData.PlayersDecisions += Player.CurrentState + "/";//FOR FILE
                            blackjackHandData.PlayersSplitHandDecisions += Player.splitHandState + "/";//FOR FILE
                        }
                        else
                        {
                            Player.React(Dealer.upCard, ref Player.splitHandState, Player.splitHand, Player.Count);
                            blackjackHandData.PlayersSplitHandDecisions += Player.splitHandState + "/";//FOR FILE
                        }
                        //Player doubles down on split hand
                        if (Player.splitHandState == PlayerState.DoubleDown)
                        {
                            //Double his split hand stake
                            Player.AddBet(Player.SplitHandStake, ref Player.SplitHandStake);
                            Console.WriteLine($"Player's split stake after double down:\t{Player.SplitHandStake}");
                            //Hit player
                            HitPlayer(Player.splitHand);
                            Console.WriteLine($"Player split double downs with:\t{Player.splitHand.cards.Last()}");
                            //If player < 21 STAND
                            if (Player.hand.handValues.First() <= 21)
                            {
                                Player.splitHandState = PlayerState.Stand;
                                blackjackHandData.PlayersSplitHandDecisions += Player.splitHandState + "/";//FOR FILE
                            }
                            //If player > 21 BUST
                            else
                            {
                                Player.splitHandState = PlayerState.Bust;
                                blackjackHandData.PlayersSplitHandDecisions += Player.splitHandState + "/";//FOR FILE
                            }
                        }
                        //While the player isn't standing or busting
                        while (Player.splitHandState != PlayerState.Bust && Player.splitHandState != PlayerState.Stand)
                        {
                            //Hit player is it reacts hit
                            if (Player.splitHandState == PlayerState.Hit)
                            {
                                Player.WriteCurrentState();
                                HitPlayer(Player.splitHand);
                                //BUST CHECK
                                if (Player.splitHand.handValues.First() > 21)
                                {
                                    Player.splitHandState = PlayerState.Bust;
                                }
                                Console.WriteLine($"Player's split hand:\t{Player.splitHand}\t{Player.splitHand.handValues.Last()}");
                            }
                            Player.React(Dealer.upCard, ref Player.splitHandState, Player.splitHand, Player.Count);
                            blackjackHandData.PlayersSplitHandDecisions += Player.splitHandState + "/";
                        }
                        //If player busts on split hand, lose split hand stake
                        if (Player.splitHandState == PlayerState.Bust)
                        {
                            Player.SplitHandStake = 0;
                            blackjackHandData.SplitGameResult = "L";
                        }

                    }
                    //PLAY REGULAR HAND
                    Player.React(Dealer.upCard, ref Player.CurrentState, Player.hand, Player.Count);
                    //IF PLAYER SPLIT ACES, STAND
                    if (Player.splitHand != null && Player.splitHand.cards.First().Face == Face.Ace)
                    {
                        Player.CurrentState = PlayerState.Stand;
                    }
                    blackjackHandData.PlayersDecisions += Player.CurrentState + "/";//FOR FILE

                    //IF PLAYER DOUBLE DOWNS, STAND OR BUST
                    if (Player.CurrentState == PlayerState.DoubleDown)
                    {
                        Player.AddBet(Player.Stake, ref Player.Stake);
                        HitPlayer(Player);
                        Console.WriteLine($"Player DOUBLES DOWN:\t{Player.hand.cards.Last()}");
                        Console.WriteLine($"Player's stake after DD:\t{Player.Stake}");
                        Console.WriteLine($"Player's Hand:\t{Player.hand}\t{Player.hand.handValues.Max()}");
                        //STAND IF DOUBLE DOWN AND PLAYER < 21
                        Player.CurrentState = Player.hand.handValues.First() <= 21 ? PlayerState.Stand : PlayerState.Bust;

                    }

                    //WHILE PLAYER ISN'T BUST OR STANDING
                    while (Player.CurrentState != PlayerState.Bust && Player.CurrentState != PlayerState.Stand)
                    {
                        if (Player.CurrentState == PlayerState.Hit)
                        {
                            HitPlayer(Player);
                            Console.WriteLine($"Player Hits:\t{Player.hand.cards.Last()}\t{Player.hand.handValues.Last()}");
                            if (Player.hand.handValues.First() > 21)
                            {
                                Player.CurrentState = PlayerState.Bust;
                            }
                        }
                        Player.React(Dealer.upCard, ref Player.CurrentState, Player.hand, Player.Count);
                        blackjackHandData.PlayersDecisions += Player.CurrentState + "/";//FOR FILE
                    }
                    Console.WriteLine($"PLAYER REACTS:\t{Player.CurrentState}\t{Player.hand.handValues.Last()}");
                    //If player is bust, player loses his bet and hand is over
                    if (Player.CurrentState == PlayerState.Bust)
                    {
                        //PLAYER LOSES
                        Console.WriteLine($"Dealer's Hand:\t{Dealer.hand}\t{Dealer.hand.handValues.Max()}");
                        Console.WriteLine("Game Result: Dealer Wins");
                        Player.Stake = 0;
                        blackjackHandData.GameResult = "L";
                        Console.WriteLine($"Player Chips:\t{Player.Chips}");
                    }
                    else
                    {
                        //DEALER PLAYS HAND IF PLAYER DOESN'T LOSE
                        Console.Write("Dealer has: ");
                        Dealer.hand.WriteHandAndHandValue();
                        //If not dealer reacts
                        Dealer.React();
                        blackjackHandData.DealersDecisions += Dealer.CurrentState + "/";//FOR FILE
                        while (Dealer.CurrentState != PlayerState.Bust && Dealer.CurrentState != PlayerState.Stand)
                        {
                            if (Dealer.CurrentState == PlayerState.Hit)
                            {
                                HitPlayer(Dealer);
                                Console.WriteLine($"Dealer draws: {Dealer.hand.cards.Last()}\t{Dealer.hand.handValues.Max()}");
                            }
                            Dealer.React();
                            blackjackHandData.DealersDecisions += Dealer.CurrentState + "/";//FOR FILE
                        }
                        Console.WriteLine($"DEALER REACTS:\t{Dealer.CurrentState}\t{Dealer.hand.handValues.Last()}");
                        //If dealer is bust and player is not, player wins
                        if (Dealer.CurrentState == PlayerState.Bust)
                        {
                            Console.WriteLine("Game Result: Player Wins");
                            Player.Chips += Player.Stake * 2;
                            blackjackHandData.GameResult = "W";
                            if (Player.splitHand != null && Player.splitHandState != PlayerState.Bust)
                            {
                                Console.WriteLine("Game Result: Player Wins Split Hand");
                                Player.Chips += Player.SplitHandStake * 2;
                                blackjackHandData.SplitGameResult = "W";
                            }
                            //Player wins
                            Console.WriteLine($"Player Chips:\t{Player.Chips}");

                        }
                        //SETTLEMENT
                        else
                        {
                            Player.hand.SetHandValues();
                            Dealer.hand.SetHandValues();
                            //SETTLE SPLIT HAND
                            if (Player.splitHand != null)
                            {
                                if (Player.splitHand.handValues.Last() > Dealer.hand.handValues.Last())
                                {
                                    Console.WriteLine("Game Result: Player Wins Split Hand");
                                    Player.Chips += Player.SplitHandStake * 2;
                                    //Player Wins Split Hand
                                    blackjackHandData.SplitGameResult = "W";
                                }
                                else if (Player.splitHand.handValues.Last() < Dealer.hand.handValues.Last())
                                {
                                    Console.WriteLine("Game Result: Dealer Wins Split Hand");
                                    blackjackHandData.SplitGameResult = "L";
                                }
                                else
                                {
                                    Console.WriteLine("Game Result: TIE Split Hand");
                                    Player.Chips += Player.SplitHandStake;
                                    blackjackHandData.SplitGameResult = "D";
                                }
                            }
                            //If player hand value > dealers player wins
                            if (Player.hand.handValues.Last() > Dealer.hand.handValues.Last())
                            {
                                Console.WriteLine("Game Result: Player Wins");
                                Player.Chips += Player.Stake * 2;
                                //Player Wins
                                blackjackHandData.GameResult = "W";
                            }
                            //If player hand value < dealers player wins
                            else if (Player.hand.handValues.Last() < Dealer.hand.handValues.Last())
                            {
                                Console.WriteLine("Game Result: Dealer Wins");
                                blackjackHandData.GameResult = "L";
                            }
                            //If player hand value == dealers player ties
                            else
                            {
                                Console.WriteLine("Game Result: TIE");
                                Player.Chips += Player.Stake;
                                blackjackHandData.GameResult = "D";
                            }
                            Console.WriteLine($"Player Chips:\t{Player.Chips}");
                        }
                    }
                }
                //If there was a split hand this turn, document split hand values
                if (Player.splitHand != null)
                {
                    blackjackHandData.PlayersStartingSplitHandForFile = Player.splitHand.cards[0] + " " + Player.splitHand.cards[1];
                    blackjackHandData.PlayersStartingSplitHardHandValueForFile = Player.splitHand.handValues.First().ToString();
                    blackjackHandData.PlayersStartingSplitSoftHandValueForFile = Player.splitHand.handValues.Last().ToString();
                    blackjackHandData.PlayersEndSplitHand = Player.splitHand.ToString();
                    blackjackHandData.PlayersEndSplitHandValue = Player.splitHand.handValues.Last().ToString();
                }
                //If no split hand change all split hand values to N/A
                else
                {
                    blackjackHandData.PlayersStartingSplitHandForFile = "N/A";
                    blackjackHandData.PlayersStartingSplitHardHandValueForFile = "N/A";
                    blackjackHandData.PlayersStartingSplitSoftHandValueForFile = "N/A";
                    blackjackHandData.PlayersEndSplitHand = "N/A";
                    blackjackHandData.PlayersEndSplitHandValue = "N/A";
                    blackjackHandData.PlayersSplitHandDecisions = "N/A";
                    blackjackHandData.PlayersStartingHandPreSplit = "N/A";
                    blackjackHandData.SplitGameResult = "N/A";
                }
                blackjackHandData.FirstCountAfterHandForFile = Player.UpdateCount(Deck, _burntCards, Dealer.upCard).First();
                blackjackData.blackjackHandDataCollection.Add(blackjackHandData);
                //Write all relevant information to file after every hand
                f.WriteLine(blackjackHandData.GetHandData(Player, HandsPlayed, Dealer, Deck));
                //blackjackFileData.WriteToFile(f, player, HandsPlayed, dealer, deck);

                //Cleanup hand and Update Counts
                CleanupHand();
                Player.UpdateCount(Deck, _burntCards, Dealer.upCard);

                //If human is playing await input
                if (_options.HumanPlayer)
                {
                    Console.ReadKey();
                    Console.Clear();
                }
                HandsPlayed++;

            }
            //Close file when all hands are played and write simulation summary to file
            f.Close();
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");//CONSOLE HAND SEPARATOR
            Console.WriteLine($"Strategy:\t{_options.StrategyName}");
            Console.WriteLine($"Deck Size:\t{_options.DeckSize}");
            Console.WriteLine($"Hands Played:\t{HandsPlayed}");
            Console.WriteLine($"Final Chip Count:\t{Player.Chips}");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");//CONSOLE HAND SEPARATOR

        }

        private bool CheckNaturals(ref BlackjackHandData blackjackHandData)
        {
            if (Player.hand.handValues.Contains(21))
            {
                //Player has natural and dealer doesn't
                if (!Dealer.hand.handValues.Contains(21))
                {
                    Console.WriteLine("Player Has a Natural");
                    if (blackjackHandData != null)
                        blackjackHandData.PlayersDecisions = "PLAYER_NATURAL";
                    Console.WriteLine("Game Result: Player Wins");
                    Player.Chips += (int)(Player.Stake * 2.5);
                    Player.Stake = 0;
                    //Player wins
                    if (blackjackHandData != null)
                        blackjackHandData.GameResult = "W_N";
                    return true;
                }
                //Player and dealer have natural
                else
                {
                    Console.WriteLine("Player Has a natural");
                    Console.WriteLine("Dealer Has a natural");
                    if (blackjackHandData != null)
                        blackjackHandData.PlayersDecisions = "PLAYER_NATURAL";
                    if (blackjackHandData != null)
                        blackjackHandData.DealersDecisions = "DEALER_NATURAL";
                    Console.WriteLine("Game Result: TIE");
                    Player.Chips += Player.Stake;
                    //TIE
                    if (blackjackHandData != null)
                        blackjackHandData.GameResult = "D_N";
                    return true;
                }

            }
            else if (Dealer.hand.handValues.Contains(21))
            {
                //Dealer has natural and player doesn't
                if (!Player.hand.handValues.Contains(21))
                {
                    Console.WriteLine("Dealer Has a natural");
                    if (blackjackHandData != null)
                        blackjackHandData.DealersDecisions = "DEALER_NATURAL";
                    Console.WriteLine("Game Result: Dealer Wins");
                    Player.Stake = 0;
                    //Player loses
                    if (blackjackHandData != null)
                        blackjackHandData.GameResult = "L_N";
                    return true;
                }
                //Player and dealer have a natural
                else
                {
                    Console.WriteLine("Player Has a Natural");
                    Console.WriteLine("Dealer Has a natural");
                    Console.WriteLine("Game Result: TIE");
                    if (blackjackHandData != null)
                        blackjackHandData.PlayersDecisions = "PLAYER_NATURAL";
                    if (blackjackHandData != null)
                        blackjackHandData.DealersDecisions = "DEALER_NATURAL";
                    Player.Chips += Player.Stake;
                    //TIE
                    if (blackjackHandData != null)
                        blackjackHandData.GameResult = "D_N";
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Initialises file in data directory with name filename
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private StreamWriter InitialiseStreamWriter(string filename, string path)
        {
            if (File.Exists(path + "\\" + filename))
            {
                File.Delete(path + "\\" + filename);
            }
            StreamWriter sw = new StreamWriter(path + "\\" + filename, false);
            sw.WriteLine("HandNumber,PlayersStartingChips,EndChips,ChipsWon,GameResult,SplitGameResult,CardsInDeckBeforeTurn,CardsInDeckAfterTurn," +
                "PlayerStake,PlayersStartingHand,PlayerStartingHardHandValue,PlayerStartingSoftHandValue,PlayersEndHand,PlayersEndHandValue,PlayersDecisions," +
                "DealersUpCard,DealersUpCardValue,DealersStartHand,DealersEndHand,DealersHardEndHandValue,DealersSoftEndHandValue,DealersEndValue,DealersDecisions," +
                "DoesPlayerSplit,PlayersStartingSplitHand,PlayerStartingSplitHardHandValue,PlayerStartingSplitSoftHandValue,PlayersEndSplitHand,PlayersSplitEndHandValue,PlayersSplitHandDecisions" +
                ",PlayersHandPreSplit,CountBeforeHand,CountDuringHand,Count[0]AtTimeOfBet,Count[1]AtTimeOfBet,DeckHash");
            return sw;
        }
        /// <summary>
        /// Updates player and dealer hand values
        /// </summary>
        public void UpdateHandValues()
        {
            Player.hand.SetHandValues();
            Dealer.hand.SetHandValues();
        }
        /// <summary>
        /// Cleans up hands, adds all played cards to burnt cards
        /// Sets split hand to null
        /// </summary>
        private void CleanupHand()
        {
            foreach (var c in Player.hand.cards)
            {
                _burntCards.Add(c.Clone());
            }
            Player.hand.cards.Clear();
            Player.splitHand = null;
            foreach (var c in Dealer.hand.cards)
            {
                _burntCards.Add(c);
            }
            Dealer.hand.cards.Clear();
            Dealer.upCard = null;
            Player.Stake = 0;
            Player.SplitHandStake = 0;
        }
        /// <summary>
        /// Defunct method for manually playing blackjack
        /// Out of scope for software
        /// </summary>
        private void InitialiseGameAsPlayer()
        {
            Deck = new Deck(_options.DeckSize);
            Deck.FisherYatesShuffle();

            Player = new HumanStrategy();
            Dealer = new Dealer();
            _options.HumanPlayer = true;
        }
        /// <summary>
        /// Parses a switch statement with a given strategy name
        /// Assigns strategy according to name
        /// Dealer, Deck, Player are assigned here
        /// </summary>
        /// <param name="strategy"></param>
        private void InitialiseGame(string strategy)
        {
            Deck = new Deck(_options.DeckSize);
            Deck.FisherYatesShuffle();
            Player = ParseStrategy(strategy);
            Dealer = new Dealer();
        }

        private Player ParseStrategy(string strategy)
        {
            switch (strategy?.ToLower())
            {
                case "basicstrategy":
                    return Player = new BasicStrategy
                    {
                        Chips = _options.StartChips
                    };
                case "acetofive":
                    return Player = new AceToFiveStrategy
                    {
                        Chips = _options.StartChips
                    };
                case "dealer":
                    return Player = new DealerStrategy
                    {
                        Chips = _options.StartChips
                    };

                case "fivecount":
                    return Player = new FiveCountStrategy
                    {
                        Chips = _options.StartChips
                    };

                case "simplepointcount":
                    return Player = new SimplePointCountStrategy
                    {
                        Chips = _options.StartChips
                    };

                case "tencount":
                    return Player = new TenCountStrategy
                    {
                        Chips = _options.StartChips
                    };

                case "completepointcount":
                    return Player = new CompletePointCountStrategy
                    {
                        Chips = _options.StartChips
                    };
 
                case "knockoutcount":
                    return Player = new KnockoutCountStrategy
                    {
                        Chips = _options.StartChips
                    };

                default:
                    Console.WriteLine("Strategy Not Found");
                    throw new Exception();
                    //break;

            }
        }

        /// <summary>
        /// Deals hand to player and dealer, 2 cards each
        /// Then sets values
        /// </summary>
        public void DealHand()
        {
            Player.hand.cards.Add(Deck.Cards.Pop());
            Player.hand.cards.Add(Deck.Cards.Pop());
            Dealer.hand.cards.Add(Deck.Cards.Pop());
            Dealer.hand.cards.Add(Deck.Cards.Pop());

            Player.hand.SetHandValues();
            Dealer.hand.SetHandValues();
        }
        /// <summary>
        /// Hits an Actor with an extra card
        /// </summary>
        /// <param name="actor"></param>
        public void HitPlayer(Actor actor)
        {
            actor.hand.cards.Add(Deck.Cards.Pop());
            actor.hand.SetHandValues();
        }
        /// <summary>
        /// Hits a hand with an extra card
        /// </summary>
        /// <param name="hand"></param>
        public void HitPlayer(Hand hand)
        {
            hand.cards.Add(Deck.Cards.Pop());
            hand.SetHandValues();
            Player.UpdateCount(Deck, _burntCards, Dealer.upCard);
        }

    }
}
