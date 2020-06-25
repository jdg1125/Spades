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
            foreach (var x in new Player[] { game1.Player0, game1.Player1, game1.Player2, game1.Player3 })
                    Console.WriteLine($"{x}");

            Console.WriteLine();

            game1.Play(1);

            Console.WriteLine();


            Console.WriteLine($"Score: team1: {game1.Team1Score}, team2: {game1.Team2Score}");
            

            Console.ReadKey();
        }
    }
}
