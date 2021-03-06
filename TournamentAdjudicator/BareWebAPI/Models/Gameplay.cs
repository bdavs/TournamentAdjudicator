﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentAdjudicator.Controllers;
using TournamentAdjudicator.Models;

namespace TournamentAdjudicator.Models
{
    public static class Gameplay
    {
        // define the board dimensions
        static int Board_Height = 10;
        static int Board_Width = 10;
        public static int Player_Turn;
        public static bool Game_Started = false;
        public static bool firstTurn = true;
        public static bool validFirstMove = false;
        public static int Pass_Count = 0;

        static string[,,] board = new string[2, 10, 10]; // [letter assigned(1)/letter height(2) ,x,y]
        static string[,,] board_temp = new string[2, 10, 10];

        public static List<string> bag = new List<string>();

        //Instantiate the Validity class
        public static Validity moveChecker = new Validity();

        public static string[,,] Board
        {
            get
            {
                return board;
            }
            set
            {
                board = value;
            }
        }

        public static string[,,] Board_temp
        {
            get
            {
                return board_temp;
            }
            set
            {
                board_temp = value;
            }
        }

        public static bool turn(int id)
        {
            if (Player_Turn == id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //only increment playerturn. should we keep track of if players have passed?
        public static void pass()
        {
            Pass_Count++;
            Player_Turn = (Player_Turn % UserController.Players.Count) + 1;
        }


        public static bool accept_or_reject_move(Player p)
        {
            bool passStatus = true;
            // see where this should be done so that
            // a new instantiation of this class
            moveChecker.NewBoard = board_temp;
            moveChecker.OldBoard = board;


            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (board[0, i, j] != board_temp[0, i, j])
                        passStatus = false;
                }
            }
            if (passStatus)
            {
                pass();
                moveChecker.LogPassMove(p);
                return true;
            }

            else
            {

                // Proper way to copy contents of one list to another
                moveChecker.PlayerLetters.Clear();
                foreach (string s in p.Letters)
                    moveChecker.PlayerLetters.Add(s);

                if (p.ID != Player_Turn)
                        return false;

                if (moveChecker.CheckMoveValidity(firstTurn, p))
                {
                    firstTurn = false;
                    board = board_temp;
                    Player_Turn = (Player_Turn % UserController.Players.Count) + 1;

                    List<string> used_letters = moveChecker.UsedLetters;

                    foreach (string letter in used_letters)
                    {
                        p.Letters.Remove(letter);
                    }

                    give_letters(p, used_letters.Count);


                    //is a valid move
                    Pass_Count = 0;
                    return true;
                }
                else
                {
                    Player_Turn = (Player_Turn % UserController.Players.Count) + 1;

                    //not a valid move, they pass their turn
                    Pass_Count++;
                    return false;
                }
            }


        }

        //exchange a single letter instead of a move
        public static bool exchange_move(Player p)
        {
            if (p.Letters.Contains(p.ExchangeLetter))
            {
                p.Letters.Remove(p.ExchangeLetter);

                if (bag.Count > 0)
                {

                    Random rnd = new Random();
                    int start2;



                    start2 = rnd.Next(0, bag.Count);
                    p.addSingleLetter(bag[start2]);

                    bag.Remove(bag[start2]);

                    //moved the adding back in till after they remove a letter
                    bag.Add(p.ExchangeLetter);

                    Player_Turn = (Player_Turn % UserController.Players.Count) + 1;
                    Pass_Count = 0;

                    return true;
                }
                else
                {
                    Player_Turn = (Player_Turn % UserController.Players.Count) + 1;
                    Pass_Count++;
                    return false;
                }
            }

            else return false;
        }

        public static void example_board()
        {

            board[0, 5, 5] = "h";
            board[0, 6, 5] = "a";
            board[0, 7, 5] = "t";
            board[0, 5, 6] = "e";
            board[0, 5, 7] = "a";
            board[0, 5, 8] = "r";
            board[0, 5, 9] = "t";
            board[0, 7, 5] = "d";

            board[1, 5, 5] = "5";
            board[1, 6, 5] = "1";
            board[1, 7, 5] = "1";
            board[1, 5, 6] = "1";
            board[1, 5, 7] = "1";
            board[1, 5, 8] = "1";
            board[1, 5, 9] = "1";
            board[1, 7, 5] = "2";

        }

        // Initialize all game squares to "-"
        // Initialize heights to "0"
        public static void init_board()
        {
            for (int i = 0; i < Board_Height; i++)
            {
                for (int j = 0; j < Board_Width; j++)
                {
                    board[0, i, j] = "-";
                    board[1, i, j] = "0";
                }
            }

            Player_Turn = 1;
        }

        public static void Print_Board(string[,,] myBoard, int level)
        {
            Console.Write(" ");
            Console.Write(" ");
            Console.Write(" ");
            Console.Write(" ");
            Console.Write(" ");
            for (int i = 0; i < Board_Height + 1; i++)
            {
                for (int j = 0; j < Board_Width + 1; j++)
                {
                    if (i == 0)
                    {
                        if (!(j.Equals(10)))
                        {
                            Console.Write(" ");
                            Console.Write(" ");
                            Console.Write(j);
                            Console.Write(" ");
                            Console.Write(" ");
                        }
                    }
                    else if (j == 0)
                    {
                        Console.Write(" ");
                        Console.Write(" ");
                        Console.Write(i - 1);
                        Console.Write(" ");
                        Console.Write(" ");
                    }
                    else
                    {
                        Console.Write(" ");
                        Console.Write(" ");
                        Console.Write(myBoard[level, i - 1, j - 1]);
                        Console.Write(" ");
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        public static void initalize_bag()
        {

            for (int i = 0; i <= 9; i++)
            {
                for (int j = 0; j <= 9; j++)
                {
                    board[1, i, j] = "0";
                }
            }


            int temp = 0;
            bag.Add("V");
            bag.Add("Qu");
            bag.Add("J");
            bag.Add("X");
            bag.Add("Z");

            while (temp < 2)
            {
                bag.Add("K");
                bag.Add("W");
                bag.Add("Y");
                temp++;
            }
            temp = 0;
            while (temp < 3)
            {
                bag.Add("B");
                bag.Add("F");
                bag.Add("G");
                bag.Add("H");
                bag.Add("P");
                temp++;
            }
            temp = 0;
            while (temp < 4)
            {
                bag.Add("C");
                temp++;
            }
            temp = 0;
            while (temp < 5)
            {
                bag.Add("D");
                bag.Add("L");
                bag.Add("M");
                bag.Add("N");
                bag.Add("R");
                bag.Add("T");
                bag.Add("U");
                temp++;
            }
            temp = 0;
            while (temp < 6)
            {
                bag.Add("S");
                temp++;
            }
            temp = 0;
            while (temp < 7)
            {
                bag.Add("A");
                bag.Add("I");
                bag.Add("O");
                temp++;
            }
            temp = 0;
            while (temp < 8)
            {
                bag.Add("E");
                temp++;
            }
        }


        public static void initial_draw()
        {
            //give all players 7 tiles
            foreach (Player p in UserController.Players)
            {
                give_letters(p, 7);
            }

            //start game
            Player_Turn = 1;


        }


        public static void give_letters(Player p, int needed)
        {
            Random rnd = new Random();
            int start2;
            if (bag.Count > 0)
            {
                for (int i = 0; i < needed; i++)
                {
                    start2 = rnd.Next(0, bag.Count - 1);
                    p.addSingleLetter(bag[start2]);
                    //Console.WriteLine("p: " + p.Letters[i]);
                    bag.Remove(bag[start2]);
                }
            }
        }

        /*// for debug only
        static string[] desiredLetters = 
        {"Qu","R","U","S","H","I","O","I","T","P","R","A","O","A","S","H","D","S","F","E"};
        //{"P","E","E","R","L","E","B","B","A"};
        //1   2   3   4   5   6   7   1   2   3   4   5   6   7   1   2   3   4   5   6   7   1   2   3   4   5   6   7
        static int desiredMarker = 0;
        static int desiredLettersSize = desiredLetters.Length;
        public static void give_letters(Player p, int needed)
        {
            Random rnd = new Random();
            int start2;

            for (int i = 0; i < needed; i++)
            {
                if (desiredMarker < desiredLettersSize)
                {
                    if (bag.Contains(desiredLetters[desiredMarker]))
                    {
                        p.addSingleLetter(desiredLetters[desiredMarker]);
                        bag.Remove(desiredLetters[desiredMarker]);
                        desiredMarker++;
                    }
                }
                else
                {
                    start2 = rnd.Next(0, bag.Count - 1);
                    p.addSingleLetter(bag[start2]);
                    //Console.WriteLine("p: " + p.Letters[i]);
                    bag.Remove(bag[start2]);
                }
            }
        }*/
    }
}