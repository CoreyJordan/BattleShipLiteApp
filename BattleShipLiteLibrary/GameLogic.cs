using BattleShipLiteLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipLiteLibrary
{
    public static class GameLogic
    {
        public static int GetShotCount(PlayerInfoModel winner)
        {
            int shotCount = 0;
            foreach (GridSpotModel spot in winner.ShotGrid)
            {
                if (spot.Status != GridSpotStatus.Empty)
                {
                    shotCount++;
                }
            }
            return shotCount;
        }

        public static bool IdentifyShotResult(PlayerInfoModel nonActivePlayer, string row, int column)
        {
            bool isHit = false;
            
            foreach (GridSpotModel spot in nonActivePlayer.ShipLocations)
            {
                if (spot.SpotLetter == row && spot.SpotNumber == column)
                {
                    isHit = true;
                }
            }
            return isHit;
        }

        public static void InitializeGrid(PlayerInfoModel model)
        {
            List<string> letters = new List<string>
            {
                "A", "B", "C", "D", "E"
            };

            List<int> numbers = new List<int>
            {
                1, 2, 3, 4, 5
            };

            foreach (var letter in letters)
            {
                foreach (var number in numbers)
                {
                    AddGridSpot(model, letter, number);
                }
            }
        }

        public static void MarkShotResult(PlayerInfoModel activePlayer, string row, int column, bool isAHit)
        {
            foreach (GridSpotModel spot in activePlayer.ShotGrid)
            {
                if (spot.SpotLetter == row && spot.SpotNumber == column && isAHit)
                {
                    spot.Status = GridSpotStatus.Hit;
                }
                else if (spot.SpotLetter == row && spot.SpotNumber == column && !isAHit)
                {
                    spot.Status = GridSpotStatus.Miss;
                }
            }
        }

        public static bool PlaceShip(PlayerInfoModel model, string location)
        {
            (string row, int column) = SplitShotIntoRowAndColumn(location);
            bool isValid = ValidateGridLocation(model, row, column);

            if (isValid)
            {
                foreach (GridSpotModel gridSpot in model.ShipLocations)
                {
                    if (gridSpot.SpotLetter == row && gridSpot.SpotNumber == column)
                    {
                        isValid = false;
                        break;
                    }
                }
            }

            if (isValid)
            {
                GridSpotModel shipSpot = new GridSpotModel() 
                {
                    SpotLetter = row, SpotNumber = column, Status = GridSpotStatus.Ship
                };
                model.ShipLocations.Add(shipSpot);
            }
            return isValid;
        }

        private static bool ValidateGridLocation(PlayerInfoModel model, string row, int column)
        {
            bool isValid = false;

            foreach (GridSpotModel spot in model.ShotGrid)
            {
                if (spot.SpotLetter == row && spot.SpotNumber == column)
                {
                    isValid = true;
                }
            }
            return isValid;
        }

        public static bool PlayerStillActive(PlayerInfoModel nonActivePlayer)
        {
            bool stillHasShips = false;
            foreach (GridSpotModel ship in nonActivePlayer.ShipLocations)
            {
                if (ship.Status != GridSpotStatus.Sunk)
                {
                    stillHasShips = true;
                    break;
                }
            }
            return stillHasShips;
        }

        public static (string row, int column) SplitShotIntoRowAndColumn(string shot)
        {
            string rowString = "z";
            int columnNumber = -1;

            if (shot.Length == 2 &&
                char.IsLetter(shot[0]) &&
                char.IsNumber(shot[1]))
            {
                rowString = shot.Substring(0, 1).ToUpper();
                columnNumber = int.Parse(shot.Substring(1));
            }

            return (rowString, columnNumber);
        }

        public static void UpdateShipCount(PlayerInfoModel nonActivePlayer, string row, int column)
        {
            foreach (GridSpotModel spot in nonActivePlayer.ShipLocations)
            {
                if (spot.SpotLetter == row && spot.SpotNumber == column)
                {
                    spot.Status = GridSpotStatus.Sunk;
                }
            }
        }

        public static bool ValidateShot(string row, int column, PlayerInfoModel activePlayer)
        {
            bool isValid = ValidateGridLocation(activePlayer, row, column);

            if (isValid)
            {
                foreach (GridSpotModel gridSpot in activePlayer.ShotGrid)
                {
                    if (gridSpot.SpotLetter == row &&
                        gridSpot.SpotNumber == column &&
                        (gridSpot.Status == GridSpotStatus.Miss ||
                        gridSpot.Status == GridSpotStatus.Hit))
                    {
                        isValid = false;
                        break;
                    }
                }
            }

            return isValid;
        }

        private static void AddGridSpot(PlayerInfoModel model, string letter, int number)
        {
            GridSpotModel spot = new GridSpotModel()
            {
                SpotLetter = letter,
                SpotNumber = number,
                Status = GridSpotStatus.Empty
            };

            model.ShotGrid.Add(spot);
        }
    }
}
