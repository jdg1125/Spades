using NUnit.Framework;
using System.Collections.Generic;

namespace spades
{
    [TestFixture]
    public class Tests
    {

        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void Test1()
        {
            //GameMethods methodsToTest = new GameMethods();

            Game game1 = new Game(true);
            foreach (var x in new string[] {"5C", "KD", "KS"})//"2C", "JC", "QC", "7D", "KD", "AD", "6H", "JH", "KH", "3S", "6S", "10S", "KS" })
                game1.Player0.Hand.Add(x);
            foreach (var x in new string[] { "4C", "8C", "9C", "10C", "KC", "AC", "7H", "QH", "AH", "2S", "8S", "JS", "AS" })
                game1.Player1.Hand.Add(x);

            //Assert.Contains("5C", methodsToTest.FindValidCards(player1, null, true));

            //Assert.Contains("KD", methodsToTest.FindValidCards(player1, null, true));
            Assert.Contains("KS", game1.FindValidCards(game1.Player1, null));

        }

        [Test]
        public void Test2()
        {

            Game game1 = new Game(true);
            //GameMethods play = new GameMethods();
            string[] trick = { "4S", "8S", "AH", "CH" };

            Assert.AreEqual(game1.WhoWonTrick(trick, game1.Player1).Number, 1);
        }

        [Test]

        public void Test3()
        {
            Game game1 = new Game(true);
            GameMethods play = new GameMethods();

            Assert.AreEqual(game1.FindNextPlayer(game1.Player3), game1.Player0);
        }

        [Test]
        
        // 0 1 2 3 4 5 6 7  8 9 A B C D E F
        // 3 4 5 6 7 8 9 10 J Q K A t T g G
        public void Test4()
        {
            GameMethods methods = new GameMethods();

            Assert.AreEqual(methods.HexToCard("FS", true), "G S");
        }

    }
}