using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace spades
{
    public class Game
    {
        public Player[] Players { get; set; }
        public InfoSet GameData { get; set; }
        public Game()
        {

        }
        public Game(bool AllowJailhousePlay)
        {
            Players = new Player[] { new Player(0), new Player(1), new Player(2), new Player(3) };
            GameData = new InfoSet(0, Players);

            List<string> deck = GenerateDeck(AllowJailhousePlay);
            Shuffle(deck);
            Deal(deck);
            // for (int i = 0; i < 4; i++)
            //  GameData.remainingCards.Add(Players[i].Hand);

        }
        public List<string> GenerateDeck(bool AllowJailHousePlay)
        {
            List<string> deck = new List<string>(52);
            StringBuilder sb = new StringBuilder();

            foreach (var x in new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B' })
                foreach (var y in new char[] { 'H', 'C', 'D', 'S' })
                {
                    sb.Append(x.ToString() + y.ToString());
                    deck.Add(sb.ToString());
                    sb.Clear();
                }

            if (AllowJailHousePlay)
                foreach (var x in new char[] { 'C', 'D', 'E', 'F' })
                {
                    sb.Append(x.ToString() + 'S');
                    deck.Add(sb.ToString());
                    sb.Clear();
                }
            else
                foreach (var x in new char[] { 'H', 'C', 'D', 'S' })
                {
                    sb.Append('C' + x.ToString());
                    deck.Add(sb.ToString());
                    sb.Clear();
                }

            return deck;
        }
        protected void Shuffle(List<string> deck)
        {
            Random rnd = new Random();
            int i = 0;
            int j = 0;
            string tmp;

            for (i = 0; i < deck.Count; i++)
            {
                j = rnd.Next(0, deck.Count);
                if (i != j)
                {
                    tmp = deck[j];
                    deck[j] = deck[i];
                    deck[i] = tmp;
                }
            }

        }
        protected virtual void Deal(List<string> deck)
        {
            for (int i = 0; i < deck.Count; i++)
            {
                int next = FindNextPlayer(GameData.leaderNumber);
                Players[next].Hand.Add(deck[i]);
                GameData.leaderNumber = next;
            }
        }
        public int FindNextPlayer(int leaderNumber)
        {
            if (leaderNumber == 3)
                return 0;
            else
                return leaderNumber + 1;
        }
        public List<string> FindValidCards(Player paramPlayer, string leadCard)
        {
            List<string> validCards = new List<string>();

            if (leadCard == null)
            {
                if (GameData.areSpadesBroken)
                    validCards = paramPlayer.Hand;
                else
                {
                    foreach (var x in paramPlayer.Hand)
                        if (x[1] != 'S')
                            validCards.Add(x);

                    if (validCards.Count == 0)
                        validCards = paramPlayer.Hand;
                }
            }
            else
            {
                foreach (var x in paramPlayer.Hand)
                    if (x[1] == leadCard[1])
                        validCards.Add(x);

                if (validCards.Count == 0)
                    validCards = paramPlayer.Hand;
            }
            return validCards;
        }
        public Player WhoWonTrick(Player leader, string[] trick)
        {
            char leadSuit = trick[leader.Number][1];
            char leadDenomination = trick[leader.Number][0];
            char holder = '0';
            int winnerIndex = leader.Number;
            char tmpSuit = '0';
            char tmpDenomination = '0';

            for (int i = 0; i < trick.Length; i++)
            {
                if (i == leader.Number)
                    continue;

                tmpSuit = trick[i][1];

                if (leadSuit == 'S' && tmpSuit == leadSuit) //if tmpSuit!=leadSuit, leadSuit (S) trumps tmpSuit 
                {
                    tmpDenomination = trick[i][0];

                    if (tmpDenomination > trick[winnerIndex][0])
                        winnerIndex = i;
                }
                else
                {
                    if (tmpSuit == leadSuit) //both are not spade cards
                    {
                        if (trick[winnerIndex][1] == 'S')
                            continue;
                        tmpDenomination = trick[i][0];

                        //Console.WriteLine($"\n in WhoWonTrick, i is {i}, tmpdenomination is {tmpDenomination} and leaddenomination is {leadDenomination}");
                        if (tmpDenomination > trick[winnerIndex][0])
                            winnerIndex = i;
                    }
                    else
                    {
                        if (tmpSuit == 'S')
                        {
                            if (trick[winnerIndex][1] == 'S') //current card and previously winning card are both spades
                            {
                                holder = tmpDenomination = trick[i][0];
                                tmpDenomination = trick[winnerIndex][0];

                                if (holder > tmpDenomination)
                                    winnerIndex = i;
                            }
                            else
                                winnerIndex = i;
                        }
                    }
                }
            }

            return Players[winnerIndex];

        }
        public void Play(int numberRounds)
        {
            for (int i = 0; i < numberRounds; i++)
                GameDriver();

            Console.WriteLine();
            //Console.WriteLine(GameData);
            // MakeTrick1();
            int winIndex = (GameData.scoreByPlayerNumber[0] + GameData.scoreByPlayerNumber[2] > GameData.scoreByPlayerNumber[1] + GameData.scoreByPlayerNumber[3]) ? 0 : 1;
            Console.WriteLine("The winner is team " + winIndex);
            /* Console.WriteLine('\n' + "in game, players are: ");

             foreach (var player in Players)
                 Console.WriteLine(player);*/

        }
        public void MakeTrick()
        {
            int current = GameData.leaderNumber;
            string leadCard = null;
            GameData.trick = new string[4];

            for (int i = 0; i < 4; i++)
            {
                /*if (current == 0)
                {
                    Console.WriteLine($"Please make a decision. Spades are broken: {GameData.areSpadesBroken}. {leadCard} led the trick.  Your cards are:");
                    Players[current].ToString();
                    //read a card
                    //vet the card
                    //update leadcard, spadesbroken, suitsplayerslack if necessary
                    //remove card from hand
                }
                else*/
                {
                    List<string> validCards = FindValidCards(Players[current], leadCard);
                    Random r = new Random();
                    int j = r.Next(0, validCards.Count); //MakeDecision(Players[current], validCards);
                    GameData.trick[current] = validCards[j];

                    if (leadCard == null)
                        leadCard = validCards[j];

                    if (leadCard != null && validCards[j][1] != leadCard[1])   //update record that current player "threw off"
                        GameData.suitsPlayersLack[leadCard[1]].Add(current);

                    if (!GameData.areSpadesBroken && validCards[j][1] == 'S')
                        GameData.areSpadesBroken = true;

                    Players[current].Hand.Remove(validCards[j]);
                }
                do
                {
                    current = FindNextPlayer(current);
                } while (current == GameData.leaderNumber);
            }

            for (int i = 0; i < 4; i++)
                Console.Write(GameData.trick[i] + ", ");
            Console.WriteLine();

            GameData.leaderNumber = WhoWonTrick(Players[GameData.leaderNumber], GameData.trick).Number;
            GameData.scoreByPlayerNumber[GameData.leaderNumber]++;

        }
        public void MakeTrick1()
        {
            int current = GameData.leaderNumber;
            string leadCard = null;
            GameData.trick = new string[4];

            for (int i = 0; i < 1; i++)
            {
                List<string> validCards = FindValidCards(Players[current], leadCard);
                if (i == 0)
                {
                    Console.WriteLine();
                    int count = 0;
                    foreach (var s in validCards)
                        Console.WriteLine("{0} at {1}, ", s, count++);
                    //Console.WriteLine("MakeDecision: \n");
                    int res = MakeDecision(Players[current], validCards);
                    Console.WriteLine("\nCard chosen: {0} at {1}", validCards[res], res);
                    GameData.trick[current] = validCards[res];
                    for (int j = 0; j < 4; j++)
                        Console.Write(GameData.trick[j] + ", ");
                    Console.WriteLine();
                    //GameData.trick[current] = validCards[0]; //remove this
                    /*GameData.trick[current] = validCards[0];
                    Players[current].Hand.Remove(validCards[0]);
                    SimGame sim1 = new SimGame(GameData, Players[current]);
                    sim1.Simulate();*/
                    Console.WriteLine();
                }
                else
                {
                    GameData.trick[current] = validCards[0];

                    if (leadCard == null)
                        leadCard = validCards[0];

                    if (leadCard != null && validCards[0][1] != leadCard[1])   //update record that current player "threw off"
                        GameData.suitsPlayersLack[leadCard[1]].Add(current);

                    if (!GameData.areSpadesBroken && validCards[0][1] == 'S')
                        GameData.areSpadesBroken = true;

                    Players[current].Hand.Remove(validCards[0]);

                    do
                    {
                        current = FindNextPlayer(current);
                    } while (current == GameData.leaderNumber);
                    for (int j = 0; j < 4; j++)
                        Console.Write(GameData.trick[j] + ", ");
                    Console.WriteLine();
                }
            }



            //SimGame sim1 = new SimGame(GameData, Players[current]);
            //Console.WriteLine(sim1);



            //GameData.leaderNumber = WhoWonTrick(Players[GameData.leaderNumber]).Number;
            // GameData.scoreByPlayerNumber[GameData.leaderNumber]++;

        }

        private int MakeDecision(Player current, List<string> validCards)
        {
            if (validCards.Count == 1)
                return 0;

            List<TreeNode> decisionTree = new List<TreeNode>(validCards.Count);
            foreach (var card in validCards)
                decisionTree.Add(new TreeNode(GameData, current, card));

            int timeOut = 1;
            while (timeOut++ < validCards.Count * 4)
            {
                double maxNodeValue = double.MinValue, currentValue = 0;
                int maxNodeIndex = 0;
                for (int i = 0; i < decisionTree.Count; i++)
                {
                    if ((currentValue = UCB1(decisionTree[i], timeOut, validCards.Count)) > maxNodeValue)
                    {
                        maxNodeValue = currentValue;
                        maxNodeIndex = i;
                    }
                    //  Console.WriteLine(currentValue + "at" + i);
                }

                decisionTree[maxNodeIndex].UpdateProfit(decisionTree[maxNodeIndex].Simulation.Simulate());
                //Console.WriteLine();
            }
            int maxIndex = 0;
            int maxProfit = 0;
            bool isEvenPlayer = current.Number % 2 == 0;
            for (int i = 0; i < decisionTree.Count; i++)
            {
               // Console.WriteLine(decisionTree[i]);
                if (isEvenPlayer && (decisionTree[i].Profit[0] + decisionTree[i].Profit[2]) > maxProfit)
                {
                    maxProfit = decisionTree[i].Profit[0] + decisionTree[i].Profit[2];
                    maxIndex = i;
                }
                else if (!isEvenPlayer && (decisionTree[i].Profit[1] + decisionTree[i].Profit[3]) > maxProfit)
                {
                    maxProfit = decisionTree[i].Profit[1] + decisionTree[i].Profit[3];
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        private double UCB1(TreeNode node, int parentSampleCount, int weight)
        {
            double explorationFactor = (node.TimesVisited == 0) ? double.Epsilon : node.TimesVisited;
            explorationFactor = Math.Sqrt(Math.Log(parentSampleCount) / explorationFactor);
            double profit = (node.Simulation.pointOfView.Number % 2 == 0) ? (node.Profit[0] + node.Profit[2]) : (node.Profit[1] + node.Profit[3]);
            profit /= node.TimesVisited == 0 ? 1 : node.TimesVisited;

            return profit + weight * explorationFactor;
        }

        private void GameDriver()
        {
            int current = GameData.leaderNumber;
            string leadCard = null;
            GameData.trick = new string[4];

            for (int i = 0; i < 4; i++)
            {
                List<string> validCards = FindValidCards(Players[current], leadCard);
                string chosen = " ";
                if (current == 0)
                {
                    Console.Write("Trick contains: ");
                    foreach (var card in GameData.trick)
                        Console.Write(card + ", ");
                    
                    Console.WriteLine($"Please make a decision. Spades are broken: {GameData.areSpadesBroken}. {leadCard} led the trick.  " + Players[current]);
                    
                    bool wasValid = false;
                    do
                    { 
                        chosen = Console.ReadLine();
                        if ((wasValid = validCards.Contains(chosen)) == false)
                            Console.WriteLine("Invalid card");
                    } while (!wasValid);

                    GameData.trick[current]=chosen;
                }
                else
                {
                    int j = MakeDecision(Players[current], validCards);
                    chosen = GameData.trick[current] = validCards[j];
                }
                if (leadCard == null)
                    leadCard = chosen;

                if (leadCard != null && chosen[1] != leadCard[1])   //update record that current player "threw off"
                    GameData.suitsPlayersLack[leadCard[1]].Add(current);

                if (!GameData.areSpadesBroken && chosen[1] == 'S')
                    GameData.areSpadesBroken = true;

                Players[current].Hand.Remove(chosen);

                do
                {
                    current = FindNextPlayer(current);
                } while (current == GameData.leaderNumber);   //unreachable??
            }

            GameData.leaderNumber = WhoWonTrick(Players[GameData.leaderNumber], GameData.trick).Number;
            GameData.scoreByPlayerNumber[GameData.leaderNumber]++;

            for (int i = 0; i < 4; i++)            
                Console.Write(GameData.trick[i] + ", ");
               
            Console.Write("\t");
            for (int i = 0; i < 4; i++)
                Console.Write(GameData.scoreByPlayerNumber[i] + ", ");
            Console.WriteLine();
            
        }

    }
    public class Player
    {
        public int Number { get; private set; } // players 0 and 2 together; players 1 and 3 together
        public List<string> Hand { get; set; }
        public Player(int paramNumber)
        {
            Number = paramNumber;
            Hand = new List<string>();
        }

        public override string ToString()
        {
            StringBuilder build = new StringBuilder();
            build.Append("Player " + Number + " has cards: ");
            foreach (var x in Hand)
                build.Append(x + ",  ");
            return build.ToString();
        }

    }
    public class InfoSet
    {
        public Player[] Players { get; set; }
        public Dictionary<char, HashSet<int>> suitsPlayersLack;
        public bool areSpadesBroken;
        public string[] trick;
        public int leaderNumber;
        public int[] scoreByPlayerNumber;
        public InfoSet(int trickLeaderNumber, Player[] players)
        {
            suitsPlayersLack = new Dictionary<char, HashSet<int>>(4);
            for (int i = 0; i < 4; i++)
                suitsPlayersLack.Add("CHDS"[i], new HashSet<int>());
            areSpadesBroken = false;
            //trick = new string[4];
            leaderNumber = trickLeaderNumber;
            scoreByPlayerNumber = new int[4];
            Players = players;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var key in suitsPlayersLack.Keys)
            {
                sb.Append($"Suit {key} is not present for these players: ");
                foreach (var item in suitsPlayersLack[key])
                    sb.Append(item);
            }
            sb.Append('\n' + "SpadesBroken? " + areSpadesBroken + "\nleader of trick is " + leaderNumber + '\n');

            for (int i = 0; i < 4; i++)
                sb.Append($"Trick[{i}] contains {trick[i]},   ");
            sb.Append('\n');
            for (int i = 0; i < 4; i++)
                sb.Append($"Player {i} has score: {scoreByPlayerNumber[i]},   ");
         
            sb.Append('\n');
            return sb.ToString();
        }

    }

    public class SimGame : Game
    {
        public string[] trick;
        public Player pointOfView;
        List<string> cardsOutThere;
        public SimGame(InfoSet gameState, Player current) : base()
        {
            Players = new Player[] { new Player(0), new Player(1), new Player(2), new Player(3) };
            pointOfView = Players[current.Number];

            GameData = gameState;

            trick = new string[4];
            for (int i = 0; i < 4; i++)
                trick[i] = GameData.trick[i];  //We take snapshot of the current game without changing the game. pointOfView player has played the card we're testing


        }

        private List<string> GatherCards()
        {
            List<string> deck = new List<string>();
            for (int i = 0; i < 4; i++)
                foreach (var card in GameData.Players[i].Hand)
                    if (i != pointOfView.Number)
                        deck.Add(card);

            return deck;
        }
        protected override void Deal(List<string> deck)
        {
            int[] numCardsNeeded = new int[4];
            int count = deck.Count;

            for (int i = 0; i < 4; i++)
                if (trick[i] != null && i != pointOfView.Number)
                {
                    numCardsNeeded[i] = -1;
                    count++;
                }

            int n = pointOfView.Number;
            while (count-- > 0)
            {
                do
                {
                    n = FindNextPlayer(n);
                } while (n == pointOfView.Number);

                numCardsNeeded[n]++;
            }

            /* for (int i = 0; i < 4; i++)
                 Console.Write("Deck count is {0}.  numCardsNeeded for player {1} is {2},   ", deck.Count, i, numCardsNeeded[i]);

             Console.WriteLine();*/
            List<string> buffer = new List<string>();
            Random r = new Random();
            int next = r.Next(0, 4);

            for (int i = 0; i < deck.Count; i++)
            {
                do
                {
                    next = FindNextPlayer(next);
                } while (next == pointOfView.Number);

                if (GameData.suitsPlayersLack[deck[i][1]].Contains(next) == false && Players[next].Hand.Count < numCardsNeeded[next])
                    Players[next].Hand.Add(deck[i]);
                else
                    buffer.Add(deck[i]);
            }

            EmptyBuffer(buffer, numCardsNeeded);
        }

        private void EmptyBuffer(List<string> buffer, int[] numCardsNeeded)
        {

            Random r = new Random();
            int next = r.Next(0, 4);
            int timeOut = 20;

            while (!AreHandsFull(numCardsNeeded) && timeOut-- > 0)
            {
                do
                {
                    next = FindNextPlayer(next);
                } while (next == pointOfView.Number);

                int count = buffer.Count * 4;
                int j = 0;

                if ((count = Players[next].Hand.Count) == numCardsNeeded[next])
                {
                    j = r.Next(0, count);
                    buffer.Add(Players[next].Hand[j]);
                    Players[next].Hand.RemoveAt(j);
                }

                else if (count < numCardsNeeded[next])
                {
                    bool notFound = false;
                    do
                    {
                        j = r.Next(0, buffer.Count);
                    }
                    while (buffer.Count != 0 && (notFound = GameData.suitsPlayersLack[buffer[j][1]].Contains(next) == true) && count-- >= 0);

                    if (!notFound && buffer.Count != 0)
                    {
                        Players[next].Hand.Add(buffer[j]);
                        buffer.RemoveAt(j);
                    }
                }

            }

            for (int i = 0; i < 40; i++)
            {
                int j = r.Next(0, buffer.Count);
                if (buffer.Count > 0 && Players[i % 4].Hand.Count < numCardsNeeded[i % 4] && GameData.suitsPlayersLack[buffer[j][1]].Contains(i % 4) == false) //try to add to a player's hand 
                {
                    Players[i % 4].Hand.Add(buffer[j]);
                    buffer.RemoveAt(j);
                }
            }

            if (buffer.Count != 0)
            {
                //Console.WriteLine("BUFFER NOT EMPTY");
                PrepareDeck();
            }

        }

        private void PrepareDeck()
        {
            for (int i = 0; i < 4; i++)
                if (i != pointOfView.Number)
                    Players[i] = new Player(i);
                else if (Players[i].Hand.Count == 0)
                    foreach (var card in GameData.Players[i].Hand)
                        if (card != trick[i])
                            Players[i].Hand.Add(card);


            if (cardsOutThere == null)
            {
                cardsOutThere = GatherCards();

            }

            Shuffle(cardsOutThere);
            Deal(cardsOutThere);

        }

        private bool AreHandsFull(int[] numCardsNeeded)
        {
            bool result = numCardsNeeded[0] == Players[0].Hand.Count;
            result = result && numCardsNeeded[1] == Players[1].Hand.Count;
            result = result && numCardsNeeded[2] == Players[2].Hand.Count;
            return result && numCardsNeeded[3] == Players[3].Hand.Count;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();


            sb.Append($"Player POV is {pointOfView.Number}.\n");
            for (int i = 0; i < Players.Length; i++)
                sb.Append(Players[i] + "\n");

            return sb.ToString();
        }

        public int[] Simulate()
        {

            bool areSpadesBroken = GameData.areSpadesBroken;
            int nextUp = FindNextPlayer(pointOfView.Number);
            int leader = GameData.leaderNumber;
            string[] currentTrick = new string[4];
            int[] score = new int[4];
            List<string> validCards;
            Random r = new Random();

            for (int i = 0; i < 4; i++)
                currentTrick[i] = trick[i];

            PrepareDeck();

            while (nextUp != leader)//finish currentTrick
            {
                validCards = FindValidCards(Players[nextUp], currentTrick[leader]);
                int j = r.Next(0, validCards.Count);
                currentTrick[nextUp] = validCards[j];
                if (validCards[j][1] == 'S' && !areSpadesBroken)
                    areSpadesBroken = true;
                Players[nextUp].Hand.Remove(validCards[j]);

                nextUp = FindNextPlayer(nextUp);
            }

            /*for (int i = 0; i < 4; i++)
                Console.Write(currentTrick[i] + ", ");
            Console.WriteLine();
            // */
            nextUp = leader = WhoWonTrick(Players[leader], currentTrick).Number;
            score[leader]++;


            while (Players[leader].Hand.Count > 0)//play the rest of the game, making random choices
            {
                currentTrick = new string[4];

                do
                {
                    validCards = FindValidCards(Players[nextUp], currentTrick[leader]);
                    if (validCards.Count == 0)
                        return new int[] { 0, 0, 0, 0 }; //what's happening here?
                    int j = r.Next(0, validCards.Count);
                    currentTrick[nextUp] = validCards[j];
                    if (validCards[j][1] == 'S' && !areSpadesBroken)
                        areSpadesBroken = true;
                    Players[nextUp].Hand.Remove(validCards[j]);
                    nextUp = FindNextPlayer(nextUp);


                } while (nextUp != leader);
               /* for (int i = 0; i < 4; i++)
                    Console.Write(currentTrick[i] + ", ");
                Console.WriteLine();//*/
                nextUp = leader = WhoWonTrick(Players[leader], currentTrick).Number;
                score[leader]++;
            }

           /* for (int i = 0; i < 4; i++)
                Console.Write(score[i] + ", ");
            Console.WriteLine();//*/
            ClearPlayerHands();
            return score;
        }

        private void ClearPlayerHands()
        {
            foreach (var p in Players)
                p.Hand.Clear();
        }
    }

    public class TreeNode
    {
        public SimGame Simulation { get; set; }
        public int[] Profit { get; set; }
        public int TimesVisited { get; set; }

        public string[] CurrentTrick { get; set; }
        public string CardChosen { get; set; }

        public TreeNode(InfoSet gameState, Player current, string card)
        {
            Simulation = new SimGame(gameState, current);
            Profit = new int[4];
            TimesVisited = 0;
            CardChosen = card;
            Simulation.trick[current.Number] = CardChosen;
            CurrentTrick = new string[4];
            for (int i = 0; i < 4; i++)
                CurrentTrick[i] = Simulation.trick[i];
        }

        public void UpdateProfit(int[] scores)
        {
            TimesVisited++;

            for (int i = 0; i < 4; i++)
                Profit[i] += scores[i];
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Simulation + "\n");
            sb.Append("Node players saw profit: \n");
            for (int i = 0; i < 4; i++)
                sb.Append($"{i} : {Profit[i]},   ");
            sb.Append($"\n and the node was visited {TimesVisited} times.   The card chosen by player at this node is {CardChosen}.\n");
            return sb.ToString();
        }

    }
}
