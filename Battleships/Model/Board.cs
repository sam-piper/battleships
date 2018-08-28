using System.Collections.Generic;

namespace Battleships.Model
{
    /// <summary>
    /// Defines a game board for a specific player.
    /// </summary>
    public class Board
    {
        #region Properties         
        /// <summary>
        /// Gets or sets the parent game associated with the board.
        /// </summary>
        public Game Game { get; set; }

        /// <summary>
        /// Gets or sets the height of the board, in cells.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the width of the board, in cells.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets a list of all the ships that have been placed on the board.
        /// </summary>
        public List<Ship> Ships { get; } = new List<Ship>();
        #endregion
    }
}