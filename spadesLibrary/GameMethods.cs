using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Text;

namespace spades
{
     public class GameMethods
    {
        char[] faceCards = new char[] { 'J', 'Q', 'K', 'A', 't', 'T', 'g', 'G' }; //t - 2spades, T - 2 diamonds, g - little joker, G - big Joker
        public string HexToCard(string hexString, bool AllowJailHousePlay)
        {
            char suit = hexString[hexString.Length - 1];
            StringBuilder sb = new StringBuilder();
            string denomString = hexString.Substring(0, hexString.Length - 1);
            int denomInt = Int32.Parse(denomString, System.Globalization.NumberStyles.HexNumber);
            denomInt += (AllowJailHousePlay)? 3 : 2;
            
            if (denomInt > 10)
                sb.Append(faceCards[denomInt % 10 - 1]);
            else
                sb.Append(denomInt);

            sb.Append(" " + suit);

            return sb.ToString();
        }
        
        public int MonteCarloTreeSearch(string[] currentTrick, List<string> validCards, Player trickLeader, Game theGame)
        {
            DecisionTree possibleTricks = new DecisionTree(currentTrick, validCards, trickLeader, theGame);
            

            int index = 0;
           // index = SelectNode(possibleTricks.Root.Children);

            //selection
            //expansion
            //rollout
            //backpropagate
            return 0;
        }

        private void Traverse(DecisionTree.Node node)
        {
            
        }
        


    }
}
