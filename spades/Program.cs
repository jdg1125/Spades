using NUnit.Framework;
using System.Collections.Generic;
using System;

namespace spades
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game1 = new Game(false);
            //foreach (var x in game1.Players)
            //      Console.WriteLine($"{x}");

            //  Console.WriteLine();

            game1.Play(13);
            //foreach(var card in game1.GenerateDeck(false))
              //  Console.Write("{0}  , ",card);

            //Console.WriteLine();

            //foreach (var x in game1.Players)
              //  Console.WriteLine($"{x}");
            //   Console.WriteLine($"Score: team1: {game1.Team1Score}, team2: {game1.Team2Score}");

    

            
            Console.ReadKey();
        }
    }
}
