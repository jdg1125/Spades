using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;


namespace spades
{
    public class Game
    {
        public int Team1Score { get; set; }
        public int Team2Score { get; set; }
        public Player Player0 { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Player Player3 { get; set; }
        public bool AreSpadesBroken { get; set; }
        public Player Leader { get; set; }
        public Game()
        {

        }
        public Game(bool AllowJailhousePlay)
        {
            Team1Score = Team2Score = 0;
            Player0 = new Player(0);
            Player1 = new Player(1);
            Player2 = new Player(2);
            Player3 = new Player(3);
            AreSpadesBroken = false;
            Leader = Player0;
            List<string> deck = GenerateDeck(AllowJailhousePlay);
            Shuffle(deck);
            Deal(deck);
        }
        private List<string> GenerateDeck(bool AllowJailHousePlay)
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
                tmp = deck[j];
                deck[j] = deck[i];
                deck[i] = tmp;
            }

        }
        protected virtual void Deal(List<string> deck)
        {
            while (deck.Count != 0)
            {
                Player next = FindNextPlayer(Leader);
                next.Hand.Add(deck[0]);
                deck.RemoveAt(0);
                Leader = next;
            }
        }
        public Player FindNextPlayer(Player current)
        {
            if (current.Number == 3)
                return Player0;
            else if (current.Number == 2)
                return Player3;
            else if (current.Number == 1)
                return Player2;
            else
                return Player1;
        }
        public List<string> FindValidCards(Player paramPlayer, string leadCard)
        {
            List<string> validCards = new List<string>();

            if (leadCard == null)
            {
                if (AreSpadesBroken)
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
        public Player WhoWonTrick(string[] trick, Player leader)
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

            if (Player0.Number == winnerIndex)
                return Player0;
            else if (Player1.Number == winnerIndex)
                return Player1;
            else if (Player2.Number == winnerIndex)
                return Player2;
            else
                return Player3;

        }
        public void Play(int numberRounds)
        {

            for (int i = 0; i < numberRounds; i++)
                MakeTrick();
        }
        public void MakeTrick()
        {
            string[] trick = new string[4];
            List<string> validCards;
            Player trickLeader = Leader;
            string leadCard = null;
            Random rnd = new Random();
            int i = 0;

            do
            {
                if ((validCards = FindValidCards(Leader, leadCard)).Count != 0)
                {
                    if (Leader.Number==0)
                    {
                        string input;
                        Console.Write("Choose a card to play: ");
                        foreach (var x in validCards)
                            Console.Write(x + ", ");
                        Console.WriteLine();
                        while (validCards.Contains(input = Console.ReadLine().Trim()) == false)
                            Console.WriteLine("Invalid choice. Choose a valid card.");
                        trick[Leader.Number] = validCards[i=validCards.FindIndex(x => x.CompareTo(input)==0)];
                    }
                    else
                    {
                        //i = rnd.Next(0, validCards.Count);

                        DecisionTree decision = new DecisionTree(trick, validCards, trickLeader, this);
                        //choose a card and add to trick i=decision.ChooseCard();
                        //trick[Leader.Number] = validCards[i];
                        //Console.WriteLine(decision.Root.ToString());
                        //Console.WriteLine();
                        Console.WriteLine(decision.Root.SimGame.ToString());
                        Console.WriteLine();
                        foreach (var x in decision.Root.Children)
                        {
                            Console.Write($"{x.ToString()},   ");
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine(x.SimGame);
                        }
                        Console.WriteLine();
                        Console.WriteLine("function selected: {0}", decision.SelectNode(decision.Root.Children));
                        Console.WriteLine();
                        decision.TreeSearch();
                        Console.WriteLine();
                        Console.WriteLine(decision.Root.Children[0].SimGame);
                        //foreach(var x in validCards)
                          //  Console.Write($"{x}, ");


                        return;
                    }
                    if (leadCard == null)
                        leadCard = validCards[i];

                    if (validCards[i][1] == 'S' && AreSpadesBroken == false)
                        AreSpadesBroken = true;

                    Leader.Hand.Remove(validCards[i]);
                }

                Leader = FindNextPlayer(Leader);

            } while (Leader.Number != trickLeader.Number);

            Leader = trickLeader = WhoWonTrick(trick, trickLeader);

            foreach (var x in trick)
                Console.Write(x + ", ");
            Console.WriteLine();

            if (trickLeader.Number == 0 || trickLeader.Number == 2)
                Team1Score++;
            else
                Team2Score++;

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
    public class DecisionTree
    {
        public string[] CurrentTrick { get; set; }
        public List<string> ValidCards { get; set; }
        public int POV { get; set; }
        public Player TrickLeader { get; set; }
        public Player SimulationTrickLeader { get; set; }
        public Player WhoseTurnItIs { get; set; }
        public SimulationGame TheGame { get; private set; }
        public Node Root { get; set; }
        public DecisionTree(string[] currentTrick, List<string> validCards, Player trickLeader, Game theGame)
        {
            CurrentTrick = new string[4];
            for (int i = 0; i < 4; i++)
                CurrentTrick[i] = currentTrick[i]; 
            ValidCards = new List<string>();
            foreach (var x in validCards)
                ValidCards.Add(x);

            WhoseTurnItIs = theGame.Leader;
            TrickLeader = trickLeader;
            SimulationTrickLeader = trickLeader;

            POV = WhoseTurnItIs.Number;
            TheGame = new SimulationGame(theGame);

            Root = new Node(CurrentTrick, null, TheGame );
            MakeChildrenOfRoot(Root);
        }
        public class Node
        {
            public Node Parent { get; set; }
            public List<Node> Children { get; set; }
            public string[] TrickRepresentedByNode { get; set; }
            public int Profit { get; set; }
            public double TimesSampled { get; set; }
            public SimulationGame SimGame { get; set; }
            public Node(string[] trick, Node parent, SimulationGame prevGame)
            {
                int playersLeftToPlay = 0;
                TrickRepresentedByNode = new string[4];
                for (int i = 0; i < 4; i++)
                {
                    TrickRepresentedByNode[i] = trick[i];
                    if (trick[i] == null)
                        playersLeftToPlay++;
                }
                Parent = parent;
                Profit = 0;
                TimesSampled = 0.01;
                SimGame = new SimulationGame(prevGame); 
                Children = new List<Node>();  //what are we going do do here?
            }
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Profit: " + Profit.ToString() + ". Times Sampled: " + TimesSampled.ToString() + ". And the trick contains:  ");
                foreach (var x in TrickRepresentedByNode)
                    if (x != null)
                        sb.Append(x + ", ");
                return sb.ToString();
            }

        }
        public void MakeChildrenOfRoot(Node node)
        {
            Node newNode; 
            for (int i = 0; i < ValidCards.Count; i++) 
            {
                newNode = MakeNewNode(node.TrickRepresentedByNode, ValidCards[i], TheGame.Leader.Number, node);
                newNode.SimGame.UpdateHands(newNode.TrickRepresentedByNode);
                node.Children.Add(newNode);
            }
        }
        public Node MakeNewNode(string[] trick, string cardToAdd, int index, Node parent)
        {
            string[] newTrick=new string[4];
            for (int i = 0; i < 4; i++)
                newTrick[i] = trick[i];
            newTrick[index] = cardToAdd;
            return new Node(newTrick, parent, new SimulationGame(parent.SimGame));
        }
        public void TreeSearch()
        {
            int index = 0;
            index = SelectNode(Root.Children);
            Simulate(Root.Children[index].TrickRepresentedByNode, Root.Children[index].SimGame);

        }
        public int SelectNode(List<Node> children)
        {
            int index = 0;
            double functionValue = 0;
            double tmp = 0;
            for (int i = 0; i < children.Count; i++)
            {
                tmp = UCB1(children[i]);
                if (tmp > functionValue)
                {
                    functionValue = tmp;
                    index = i;
                }
            }
            return index;
        }
        private double UCB1(Node node)
        {
            double NumberTimesParentSampled = (node.Parent == null) ? 0 : node.Parent.TimesSampled;

            return node.Profit + 2 * Math.Sqrt(NumberTimesParentSampled / node.TimesSampled);
        }

        public void Simulate(string[] trick, SimulationGame nodesGame)
        {
            SimulationGame oneToChange = new SimulationGame(nodesGame); //We don't want to alter the SimulationGame passed in to us.
            oneToChange.Leader = oneToChange.FindNextPlayer(WhoseTurnItIs);
            string leadCard=null;
            List<string> validCards;
            Random rnd = new Random();
            int i = 0;

            while (oneToChange.Leader.Number != SimulationTrickLeader.Number)
            {
                if (trick[SimulationTrickLeader.Number] != null)
                    leadCard = trick[SimulationTrickLeader.Number];
             
                validCards = oneToChange.FindValidCards(oneToChange.Leader, leadCard);
                i = rnd.Next(0, validCards.Count);
                if ((trick[oneToChange.Leader.Number] = validCards[i])[1] == 'S')
                    oneToChange.AreSpadesBroken = true;
                oneToChange.Leader.Hand.Remove(validCards[i]);
                oneToChange.RemainingCards.Remove(validCards[i]);

                oneToChange.Leader = oneToChange.FindNextPlayer(oneToChange.Leader);
            }

            oneToChange.Leader = oneToChange.WhoWonTrick(trick, SimulationTrickLeader);
            if (oneToChange.Leader.Number == 0 || oneToChange.Leader.Number == 2)
                oneToChange.Team1Score++;
            else
                oneToChange.Team1Score++;

            i = oneToChange.Leader.Hand.Count;
            for (int j = 0; j < i; j++)
                SimTrick(oneToChange);
            Console.WriteLine($"team1: {oneToChange.Team1Score}, team2: {oneToChange.Team2Score}");
        }

        private void SimTrick(SimulationGame simGame)
        {
            string[] trick = new string[4];
            List<string> validCards;
            Player trickLeader = simGame.Leader;
            string leadCard = null;
            Random rnd = new Random();
            int i = 0;

            do
            {
                if ((validCards = simGame.FindValidCards(simGame.Leader, leadCard)).Count != 0)
                {
                    i = rnd.Next(0, validCards.Count);

                    if (leadCard == null)
                        leadCard = validCards[i];

                    trick[simGame.Leader.Number] = validCards[i];
                    
                    if (validCards[i][1] == 'S' && simGame.AreSpadesBroken == false)
                        simGame.AreSpadesBroken = true;

                    simGame.Leader.Hand.Remove(validCards[i]);
                }

                simGame.Leader = simGame.FindNextPlayer(simGame.Leader);

            } while (simGame.Leader.Number != trickLeader.Number);

            simGame.Leader = trickLeader = simGame.WhoWonTrick(trick, trickLeader);

            foreach (var x in trick)
                Console.Write(x + ", ");
            Console.WriteLine();

            if (trickLeader.Number == 0 || trickLeader.Number == 2)
                simGame.Team1Score++;
            else
                simGame.Team2Score++;
        }

    }
    public class SimulationGame : Game
    {
        public List<string> RemainingCards { get; set; }
        public SimulationGame(SimulationGame parentGame) : base() //for cloning
        {
            RemainingCards = new List<string>(parentGame.RemainingCards.Count);
            foreach (var x in parentGame.RemainingCards)
                RemainingCards.Add(x);

            Player0 = new Player(0);            
            CloneHand(parentGame.Player0, Player0);
            Player1 = new Player(1);
            CloneHand(parentGame.Player1, Player1);
            Player2 = new Player(2); 
            CloneHand(parentGame.Player2, Player2);
            Player3 = new Player(3);
            CloneHand(parentGame.Player3, Player3);

            Team1Score = parentGame.Team1Score;
            Team2Score = parentGame.Team2Score;

            AreSpadesBroken = parentGame.AreSpadesBroken;
            switch (parentGame.Leader.Number)
            {
                case 0:
                    Leader = Player0;
                    break;
                case 1:
                    Leader = Player1;
                    break;
                case 2:
                    Leader = Player2;
                    break;
                case 3:
                    Leader = Player3;
                    break;
            }

        }
        public SimulationGame(Game theGame) : base()
        {
            Player0 = new Player(0);  
            Player1 = new Player(1);
            Player2 = new Player(2);
            Player3 = new Player(3);
            Team1Score = theGame.Team1Score;
            Team2Score = theGame.Team2Score;

            AreSpadesBroken = theGame.AreSpadesBroken;
            
            RemainingCards = GatherCards(theGame);

            foreach (var x in new Player[] { Player0, Player1, Player2, Player3 })
                if (x.Number == theGame.Leader.Number)
                {
                    foreach(var y in theGame.Leader.Hand)
                        x.Hand.Add(y);
                    Leader = x;
                }
            
            Shuffle(RemainingCards);
            Deal(RemainingCards);
        }
        public List<string> GatherCards(Game theGame)
        {
            List<string> deck = new List<string>();

            foreach (var x in new Player[] { theGame.Player0, theGame.Player1, theGame.Player2, theGame.Player3 })
                if (x.Number != theGame.Leader.Number)
                    foreach (var y in x.Hand)
                        deck.Add(y);

            return deck;
        }
        protected override void Deal(List<string> deck)
        {
            Player current = this.Leader;
            Player next;

            for(int i=0; i<deck.Count; i++)
            {
                do
                {
                    next = FindNextPlayer(current);
                    current = next;
                } while (next.Number == this.Leader.Number);

                next.Hand.Add(deck[i]);
            }
        }
        private void CloneHand(Player parentGamePlayer, Player thisGamePlayer)
        {
            foreach (var x in parentGamePlayer.Hand)
                thisGamePlayer.Hand.Add(x);
        }
        public void UpdateHands(string[] trick)
        {
            Player[] players = new Player[] { Player0, Player1, Player2, Player3 };
            int index = 0;
            for (int i = 0; i < 4; i++)
                if (trick[i] != null) 
                {
                    if ((index = players[i].Hand.FindIndex(item => item.CompareTo(trick[i]) == 0)) > 0)
                    {
                        players[i].Hand.RemoveAt(index);
                        Console.WriteLine("HERE HERE HERE");
                    }
                    if ((index = RemainingCards.FindIndex(item => item.CompareTo(trick[i]) == 0)) > 0)
                        RemainingCards.RemoveAt(index);
                }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var x in new Player[] { Player0, Player1, Player2, Player3 })
                sb.Append(x.ToString() + "\n");
            sb.Append("\n Remaining Cards: \n");
            foreach (var x in RemainingCards)
                sb.Append(x + ", ");
            sb.Append("\n and the leader of the trick is : " + Leader.Number.ToString());

            return sb.ToString();
        }

    }

  
}
