namespace Battleships.Model
{
    /// <summary>
    /// Defines the state model for a game of Battleships.
    /// </summary>
    public class Game
    {
        #region Properties        
        /// <summary>
        /// Gets or sets the model for player one.
        /// </summary>
        public Player PlayerOne { get; set; }

        /// <summary>
        /// Gets or sets the model for player two.
        /// </summary>
        public Player PlayerTwo { get; set; }

        /// <summary>
        /// Gets or sets the minimum length of each ship added to a game board.
        /// </summary>
        public int MinimumShipLength { get; set; }

        /// <summary>
        /// Gets or sets the maximum length of each ship added to a game board.
        /// </summary>
        public int MaximumShipLength { get; set; }
        #endregion
    }
}