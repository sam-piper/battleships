using System.Linq;
using Battleships.Model;

namespace Battleships.Services
{
    /// <summary>
    /// Defines a management service for creating and tracking game state.
    /// </summary>
    public class GameManager
    {
        #region Public Methods        
        /// <summary>
        /// Creates a new game of Battleships.
        /// </summary>
        /// <param name="playerOneName">The name of player one.</param>
        /// <param name="playerTwoName">The name of player two.</param>
        /// <param name="boardHeight">The height of the board. Defaults to 10.</param>
        /// <param name="boardWidth">The width of the board. Defaults to 10.</param>
        /// <param name="minShipLength">The minimum length of any ship. Defaults to 1.</param>
        /// <param name="maxShipLength">The maximum length of any ship. Defaults to 5.</param>
        /// <returns>A new <see cref="Game" /> instance.</returns>
        public Game CreateNewGame(string playerOneName, string playerTwoName, int boardHeight = 10, int boardWidth = 10,
                                  int minShipLength = 1, int maxShipLength = 5)
        {
            var game = new Game
            {
                PlayerOne = new Player
                {
                    Name = playerOneName,
                    Board = new Board
                    {
                        Height = boardHeight,
                        Width = boardWidth
                    }
                },
                PlayerTwo = new Player
                {
                    Name = playerTwoName,
                    Board = new Board
                    {
                        Height = boardHeight,
                        Width = boardWidth
                    }
                },
                MinimumShipLength = minShipLength,
                MaximumShipLength = maxShipLength
            };

            game.PlayerOne.Board.Game = game;
            game.PlayerTwo.Board.Game = game;

            return game;
        }

        /// <summary>
        /// Adds a new ship to the board at the specified position.
        /// </summary>
        /// <param name="board">The board to update.</param>
        /// <param name="xIndex">The index on the X-axis.</param>
        /// <param name="yIndex">The index on the Y-axis.</param>
        /// <param name="orientation">The axis orientation of the ship on the board.</param>
        /// <param name="length">The number of segments to create.</param>
        /// <returns>True if the ship was added successfully, False if the ship could not be added.</returns>
        public bool AddShip(Board board, int xIndex, int yIndex, ShipOrientation orientation, int length)
        {
            if (length > board.Game.MaximumShipLength || length < board.Game.MinimumShipLength)
            {
                return false;
            }

            var ship = new Ship { Orientation = orientation };
            for (var i = 0; i < length; i++)
            {
                // check if position is currently outside board
                if (xIndex < 0 || xIndex > board.Width - 1 || yIndex < 0 || yIndex > board.Height - 1)
                {
                    return false;
                }

                // check if there is already a segment at this position
                var segment = FindSegmentAtPosition(board, xIndex, yIndex);
                if (segment != null)
                {
                    return false;
                }
                
                // add the segment at current position
                ship.Segments.Add(new ShipSegment
                {
                    Ship = ship,
                    XIndex = xIndex,
                    YIndex = yIndex
                });

                // increment position to next segment if needed
                if (i < length - 1)
                {
                    switch (orientation)
                    {
                        case ShipOrientation.Horizontal:
                            xIndex++;
                            break;

                        case ShipOrientation.Vertical:
                            yIndex++;
                            break;
                    }
                }
            }

            // all segments created successfully so add ship to board
            board.Ships.Add(ship);
            return true;
        }

        /// <summary>
        /// Attacks the specified board at the specified position, returning a result that indicates if the hit was a success, if the target ship is now sunk, and if the game is over.
        /// </summary>
        /// <param name="board">The board to attack.</param>
        /// <param name="xIndex">The index on the X-axis.</param>
        /// <param name="yIndex">The index on the Y-axis.</param>
        /// <returns>An <see cref="AttackResult" /> instance.</returns>
        public AttackResult Attack(Board board, int xIndex, int yIndex)
        {
            var result = new AttackResult();
            var segment = FindSegmentAtPosition(board, xIndex, yIndex);

            if (segment != null)
            { 
                segment.IsHit = true;
                segment.Ship.IsSunk = segment.Ship.Segments.All(s => s.IsHit);

                result.IsHit = true;
                result.IsShipSunk = segment.Ship.IsSunk;
                result.IsGameOver = board.Ships.All(s => s.IsSunk);
            }

            return result;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Attempts to find a ship segment at the specified position on a board.
        /// </summary>
        /// <param name="board">The board.</param>
        /// <param name="xIndex">The index on the X-axis.</param>
        /// <param name="yIndex">The index on the Y-axis.</param>
        /// <returns>A <see cref="ShipSegment" /> at the specified position, or null if no segment was found.</returns>
        private static ShipSegment FindSegmentAtPosition(Board board, int xIndex, int yIndex)
        {
            foreach (var ship in board.Ships)
            {
                foreach (var segment in ship.Segments)
                {
                    if (segment.XIndex == xIndex && segment.YIndex == yIndex)
                    {
                        return segment;
                    }
                }
            }

            return null;
        }
        #endregion
    }
}