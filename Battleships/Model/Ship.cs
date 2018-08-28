using System.Collections.Generic;

namespace Battleships.Model
{
    /// <summary>
    /// Defines the model for a ship on a player's board.
    /// </summary>
    public class Ship
    {
        #region Properties        
        /// <summary>
        /// Gets or sets the orientation of the ship on the parent board.
        /// </summary>
        public ShipOrientation Orientation { get; set; } = ShipOrientation.Horizontal;

        /// <summary>
        /// Gets a list of ship segments that track the position and hit status of the ship.
        /// </summary>
        public List<ShipSegment> Segments { get; } = new List<ShipSegment>();

        /// <summary>
        /// Gets or sets a value indicating whether this ship is already sunk or not.
        /// </summary>
        public bool IsSunk { get; set; }
        #endregion
    }
}