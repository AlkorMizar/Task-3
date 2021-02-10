using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Exc3
{
    class Game
    {
        private string[] moves;
        public int AmountOfMoves { get; }

        private string firstWon;
        public string FirstWon { get { return firstWon; } set { firstWon = value + " win!"; } }

        private string secondWon;
        public string SecondWon { get { return secondWon; } set { firstWon = value + " win!"; } }
        public string DeadHeat { get; } = "Dead heat";
        public string Error { get; } = "ERROR";


        public Game()
        {
            moves =new string[]{"sizor","rock","paper" };
            AmountOfMoves = moves.Length;
            secondWon = "First player win!";
            secondWon = "Second player win!";
        }

        public Game(string player1,string plaer2)
        {
            moves = new string[] { "sizor", "rock", "paper" };
            AmountOfMoves = moves.Length;
            firstWon = player1+ " win!";
            secondWon = plaer2+ " win!";
        }

        public override string ToString() 
        {
            StringBuilder result = new StringBuilder("Available moves:\n");
            for (int i =  0; i < moves.Length; i++)
            {
                result.Append( (i+1) + " - " + moves[i] + "\n");
            }
            return result.ToString();
        }

        public bool isSuitedMove(int move) {
            return move > 0 && move <= moves.Length;
        }

        public string getNameOfMove(int ind) {
            return moves[ind-1].Substring(0);
        }

        public bool trySetMoves(string[] moves) 
        {
            if (moves.Length % 2 != 0 && moves.Distinct().Count() == moves.Length) 
            {
                this.moves = (string[])moves.Clone();
                return true;
            }
            return false;
        }

public string play(int moveOfFirst, int moveOfSecond) 
        {
            moveOfFirst--;
            moveOfSecond--;

            if (moveOfFirst == moveOfSecond)
                return DeadHeat;
            
            int half = moves.Length / 2; //  half of the next/previous moves in a circle

            if (moveOfFirst >= half)
            {
                if (moveOfFirst > moveOfSecond && (moveOfFirst - half) <= moveOfSecond)
                    return FirstWon; //move of second player is in half of the previous moves in a circle
                else
                    return SecondWon;
            }
            else 
            {
                if (moveOfSecond > moveOfFirst && moveOfSecond <= (moveOfFirst + half))
                    return SecondWon; //move of second player is in half of the next moves in a circle
                else
                    return FirstWon;
            }
        }

    }

    class Program
    {
        static void Main(string[] args)
        {

            Game game = new Game("Computer","You");

            if (!game.trySetMoves(args)) 
            {
                Console.WriteLine("Incorrect arguments");
                return;
            }

            int movePlayer;
            do
            {
                RandomNumberGenerator randomGen = new RNGCryptoServiceProvider();
                byte[] key = new byte[16];
                randomGen.GetBytes(key);

                byte[] robotMove = new byte[1];
                randomGen.GetBytes(robotMove);
                robotMove[0] = (byte)(robotMove[0] % game.AmountOfMoves + 1);

                HMACSHA256 hmac = new HMACSHA256(key);
                byte[] hmacRes = hmac.ComputeHash(robotMove);
                Console.WriteLine("HMAC:");
                Console.WriteLine(BitConverter.ToString(hmacRes).Replace("-", "").ToLower());

                do
                {
                    Console.WriteLine(game);
                    Console.WriteLine("0 - exit\nEnter your move:");
                } while (!int.TryParse(Console.ReadLine(), out movePlayer) || movePlayer != 0 && !game.isSuitedMove(movePlayer));
                
                if (movePlayer != 0)
                {
                    Console.WriteLine("\nYour move :" + game.getNameOfMove(movePlayer));
                    Console.WriteLine("Computer move :" + game.getNameOfMove(robotMove[0]));
                    Console.WriteLine("\n" + game.play(robotMove[0], movePlayer));
                    Console.WriteLine("\nHMAC key :");
                    Console.WriteLine(BitConverter.ToString(key).Replace("-", "").ToLower()+"\n");
                }
            } while (movePlayer != 0);
        }
    }


    
}
