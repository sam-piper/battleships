namespace Battleships.Model
{
    /// <summary>
    /// Defines a model for a game player.
    /// </summary>
    public class Player
    {
        #region Properties        
        /// <summary>
        /// Gets or sets the name of the player.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the game board for the player.
        /// </summary>
        public Board Board { get; set; }
        #endregion
    }
}