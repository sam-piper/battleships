namespace Battleships.Model
{
    /// <summary>
    /// Defines a hit segment for a ship on a player's board.
    /// </summary>
    public class ShipSegment
    {
        #region Properties        
        /// <summary>
        /// Gets or sets the parent ship.
        /// </summary>
        public Ship Ship { get; set; }

        /// <summary>
        /// Gets or sets the index of the segment on the X-axis of the board.
        /// </summary>
        public int XIndex { get; set; }

        /// <summary>
        /// Gets or sets the index of the segment on the Y-axis of the board.
        /// </summary>
        public int YIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this segment has been hit or not.
        /// </summary>
        public bool IsHit { get; set; }
        #endregion
    }
}