namespace Battleships.Model
{
    /// <summary>
    /// Provides the result of an attack operation on a game board.
    /// </summary>
    public class AttackResult
    {
        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this attack was successful or not.
        /// </summary>
        public bool IsHit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this attack has sunk the target ship as well.
        /// </summary>
        public bool IsShipSunk { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the attack has resulted in the game finishing, with the win to the attacking player.
        /// </summary>
        public bool IsGameOver { get; set; }
        #endregion
    }
}