﻿using BattleShipLiteLibrary;
using BattleShipLiteLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipLite
{
    internal class Program
    {
        static void Main(string[] args)
        {
            WelcomeMessage();

            PlayerInfoModel activePlayer = CreatePlayer("Player 1");
            PlayerInfoModel nonActivePlayer = CreatePlayer("Player 2");
            PlayerInfoModel winner = null;

            do
            {
                DisplayShotGrid(activePlayer);

                RecordPlayerShot(activePlayer, nonActivePlayer);

                bool doesGameContinue = GameLogic.PlayerStillActive(nonActivePlayer);
                if (doesGameContinue)
                {
                    // Swap AP/NAP
                    (activePlayer, nonActivePlayer) = (nonActivePlayer, activePlayer);
                }
                else
                {
                    winner = activePlayer;
                }

                // TODO Display result

            } while (winner == null);

            IdentifyWinner(winner);

            Console.ReadLine();
        }

        private static void IdentifyWinner(PlayerInfoModel winner)
        {
            Console.WriteLine($"Congrats to {winner.UsersName} for winning!");
            Console.WriteLine($"{winner.UsersName} took {GameLogic.GetShotCount(winner)} shots.");
        }

        private static void RecordPlayerShot(PlayerInfoModel activePlayer, PlayerInfoModel nonActivePlayer)
        {
            bool isValidShot = false;
            string row = "z";
            int column = 0;

            do
            {
                string shot = AskForShot(activePlayer);
                (row, column) = GameLogic.SplitShotIntoRowAndColumn(shot);
                isValidShot = GameLogic.ValidateShot(row, column, activePlayer);

                if (!isValidShot)
                {
                    HighlightText(ConsoleColor.Red, "Invalid shot location. Please try again.");
                    Console.WriteLine();
                }

            } while (!isValidShot);

            bool isAHit = GameLogic.IdentifyShotResult(nonActivePlayer, row, column);

            GameLogic.MarkShotResult(activePlayer, row, column, isAHit);

            GameLogic.UpdateShipCount(nonActivePlayer, row, column);

            DisplayShotResult(row, column, isAHit);

            Console.WriteLine();
            Console.Write("Hit enter to continue...");
            Console.ReadLine();
            Console.Clear();
        }

        private static void DisplayShotResult(string row, int column, bool isAHit)
        {
            

            if (isAHit)
            {
                Console.Write($"{row}{column} is a direct ");
                HighlightText(ConsoleColor.Green, "HIT");
                Console.WriteLine("!");
            }
            else
            {
                Console.Write($"{row}{column} is a ");
                HighlightText(ConsoleColor.Red, "MISS");
                Console.WriteLine("!");
            }
        }

        private static void HighlightText(ConsoleColor temp, string result)
        {
            Console.ForegroundColor = temp;
            Console.Write(result);
            Console.ResetColor();
        }

        private static string AskForShot(PlayerInfoModel player)
        {
            Console.Write($"{player.UsersName}, Please enter your shot: ");
            string output = Console.ReadLine();
            Console.WriteLine();
            return output;
        }

        private static void DisplayShotGrid(PlayerInfoModel activePlayer)
        {
            string currentRow = activePlayer.ShotGrid[0].SpotLetter;

            foreach (var gridSpot in activePlayer.ShotGrid)
            {
                if (gridSpot.SpotLetter != currentRow)
                {
                    Console.WriteLine();
                    currentRow = gridSpot.SpotLetter;
                }

                if (gridSpot.Status == GridSpotStatus.Empty)
                {
                    Console.Write($" {gridSpot.SpotLetter}{gridSpot.SpotNumber}");
                }
                else if (gridSpot.Status == GridSpotStatus.Hit)
                { 
                    HighlightText(ConsoleColor.Green, " X ");
                }
                else if (gridSpot.Status == GridSpotStatus.Miss)
                {
                    HighlightText(ConsoleColor.Red, " O ");
                }
                else
                {
                    Console.Write(" ? ");
                }
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        private static void WelcomeMessage()
        {
            Console.WriteLine("Welcome to BattleShip Lite");
            Console.WriteLine("created by Corey Jordan");
            Console.WriteLine();
        }

        private static PlayerInfoModel CreatePlayer(string playerTitle)
        {
            PlayerInfoModel output = new PlayerInfoModel();

            Console.WriteLine($"Player information for {playerTitle}");

            // Ask the user for their name.
            output.UsersName = AskForUsersName();

            // Load up the shot grid.
            GameLogic.InitializeGrid(output);

            // Ask the user for their 5 ship placements
            PlaceShips(output);

            // Clear
            Console.Clear();

            return output;
        }

        private static string AskForUsersName()
        {
            string output;

            do
            {
                Console.Write("What is your name? ");
                output = Console.ReadLine();
            } while (output == string.Empty);

            return output;
        }

        private static void PlaceShips(PlayerInfoModel model)
        {
            do
            {
                Console.Write($"Where do you want to place ship number {model.ShipLocations.Count + 1}? ");
                string location = Console.ReadLine();

                bool isValidLocation = GameLogic.PlaceShip(model, location);

                if (!isValidLocation)
                {
                    HighlightText(ConsoleColor.Red, "That was not a valid location. ");
                    Console.WriteLine("Please try again.");
                }

            } while (model.ShipLocations.Count < 5);
        }
    }
}
