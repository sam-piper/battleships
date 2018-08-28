using Battleships.Model;
using Battleships.Services;
using FluentAssertions;
using Xunit;

namespace Battleships.Tests.Services
{
    /// <summary>
    /// Provides unit tests for covering the functionality of the <see cref="GameManager" /> component.
    /// </summary>
    public class GameManagerTest
    {
        #region Test Methods        
        /// <summary>
        /// Verifies the creation of a new game.
        /// </summary>
        [Fact]
        public void CreateNewGame()
        {
            // arrange
            var manager = new GameManager();

            // act
            var game = manager.CreateNewGame("Player One", "Player Two", 40, 30, 2, 6);

            // assert
            game.PlayerOne.Should().NotBeNull();
            game.PlayerOne.Name.Should().Be("Player One");
            game.PlayerTwo.Should().NotBeNull();
            game.PlayerTwo.Name.Should().Be("Player Two");
            game.MinimumShipLength.Should().Be(2);
            game.MaximumShipLength.Should().Be(6);

            AssertBoard(game.PlayerOne.Board, game, 40, 30, 0);
            AssertBoard(game.PlayerTwo.Board, game, 40, 30, 0);
        }

        /// <summary>
        /// Verifies adding a ship to an existing game board.
        /// </summary>
        [Fact]
        public void AddShip()
        {
            // arrange
            var manager = new GameManager();
            var game = manager.CreateNewGame("Player One", "Player Two");

            // act / assert - vertical orientation
            var result = manager.AddShip(game.PlayerOne.Board, 4, 3, ShipOrientation.Vertical, 4);

            result.Should().BeTrue();
            game.PlayerOne.Board.Ships.Should().HaveCount(1);
            game.PlayerTwo.Board.Ships.Should().HaveCount(0);

            var ship = game.PlayerOne.Board.Ships[0];
            AssertShip(ship, false, ShipOrientation.Vertical, 4);
            AssertShipSegment(ship.Segments[0], ship, false, 4, 3);
            AssertShipSegment(ship.Segments[1], ship, false, 4, 4);
            AssertShipSegment(ship.Segments[2], ship, false, 4, 5);
            AssertShipSegment(ship.Segments[3], ship, false, 4, 6);

            // act / assert - horizontal orientation
            result = manager.AddShip(game.PlayerTwo.Board, 7, 4, ShipOrientation.Horizontal, 3);

            result.Should().BeTrue();
            game.PlayerOne.Board.Ships.Should().HaveCount(1);
            game.PlayerTwo.Board.Ships.Should().HaveCount(1);

            ship = game.PlayerTwo.Board.Ships[0];
            AssertShip(ship, false, ShipOrientation.Horizontal, 3);
            AssertShipSegment(ship.Segments[0], ship, false, 7, 4);
            AssertShipSegment(ship.Segments[1], ship, false, 8, 4);
            AssertShipSegment(ship.Segments[2], ship, false, 9, 4);
        }

        /// <summary>
        /// Verifies that adding a ship fails when the ship length is invalid for the game board.
        /// </summary>
        [Fact]
        public void AddShip_FailsWhenShipLengthInvalid()
        {
            // arrange
            var manager = new GameManager();
            var game = manager.CreateNewGame("Player One", "Player Two", minShipLength: 2, maxShipLength: 4);

            // act / assert - too small
            var result = manager.AddShip(game.PlayerOne.Board, 3, 2, ShipOrientation.Horizontal, 1);
            result.Should().BeFalse();
            game.PlayerOne.Board.Ships.Should().HaveCount(0);

            // act / assert - too large
            result = manager.AddShip(game.PlayerOne.Board, 3, 2, ShipOrientation.Horizontal, 5);
            result.Should().BeFalse();
            game.PlayerOne.Board.Ships.Should().HaveCount(0);
        }

        /// <summary>
        /// Verifies that adding a ship fails when the ship length falls outside the board.
        /// </summary>
        [Fact]
        public void AddShip_FailsWhenShipOutsideBoard()
        {
            // arrange
            var manager = new GameManager();
            var game = manager.CreateNewGame("Player One", "Player Two");

            // act / assert - exceeds X dimension
            var result = manager.AddShip(game.PlayerOne.Board, 9, 2, ShipOrientation.Horizontal, 2);
            result.Should().BeFalse();
            game.PlayerOne.Board.Ships.Should().HaveCount(0);

            // act / assert - exceeds Y dimension
            result = manager.AddShip(game.PlayerOne.Board, 3, 6, ShipOrientation.Vertical, 5);
            result.Should().BeFalse();
            game.PlayerOne.Board.Ships.Should().HaveCount(0);
        }

        /// <summary>
        /// Verifies that adding a ship fails when the ship length falls outside the board.
        /// </summary>
        [Fact]
        public void AddShip_FailsWhenShipSegmentAlreadyExists()
        {
            // arrange
            var manager = new GameManager();
            var game = manager.CreateNewGame("Player One", "Player Two");
            var result = manager.AddShip(game.PlayerOne.Board, 4, 2, ShipOrientation.Horizontal, 3);
            result.Should().BeTrue();
            game.PlayerOne.Board.Ships.Should().HaveCount(1);

            // act
            result = manager.AddShip(game.PlayerOne.Board, 6, 1, ShipOrientation.Vertical, 2);

            // assert
            result.Should().BeFalse();
            game.PlayerOne.Board.Ships.Should().HaveCount(1);
        }

        /// <summary>
        /// Verifies successful attack strategy on a known board, for hit / sunk / game over progression.
        /// </summary>
        [Fact]
        public void AttackSucceeds()
        {
            // arrange
            var manager = new GameManager();
            var game = manager.CreateNewGame("Player One", "Player Two");
            manager.AddShip(game.PlayerOne.Board, 3, 6, ShipOrientation.Horizontal, 3).Should().BeTrue();
            manager.AddShip(game.PlayerOne.Board, 6, 7, ShipOrientation.Vertical, 2).Should().BeTrue();

            // act / assert - single hit, not sunk, not game over
            var result = manager.Attack(game.PlayerOne.Board, 3, 6);
            AssertAttackResult(result, true, false, false);

            // act / assert - ship sunk, not game over
            result = manager.Attack(game.PlayerOne.Board, 4, 6);
            AssertAttackResult(result, true, false, false);

            result = manager.Attack(game.PlayerOne.Board, 5, 6);
            AssertAttackResult(result, true, true, false);

            // act / assert - all ships sunk, game over
            result = manager.Attack(game.PlayerOne.Board, 6, 7);
            AssertAttackResult(result, true, false, false);

            result = manager.Attack(game.PlayerOne.Board, 6, 8);
            AssertAttackResult(result, true, true, true);
        }

        /// <summary>
        /// Verifies that an attack result is correct when an attack should miss.
        /// </summary>
        [Fact]
        public void AttackMisses()
        {
            // arrange
            var manager = new GameManager();
            var game = manager.CreateNewGame("Player One", "Player Two");
            manager.AddShip(game.PlayerTwo.Board, 2, 5, ShipOrientation.Horizontal, 5).Should().BeTrue();
            game.PlayerTwo.Board.Ships.Should().HaveCount(1);

            // act
            var result = manager.Attack(game.PlayerTwo.Board, 1, 1);
            
            // assert
            AssertAttackResult(result, false, false, false);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Asserts state expectations against a board model.
        /// </summary>
        /// <param name="board">The actual board.</param>
        /// <param name="game">The expected game.</param>
        /// <param name="height">The expected height.</param>
        /// <param name="width">The expected width.</param>
        /// <param name="shipCount">The expected number of ships.</param>
        private static void AssertBoard(Board board, Game game, int height, int width, int shipCount)
        {
            board.Should().NotBeNull();
            board.Game.Should().Be(game);
            board.Height.Should().Be(height);
            board.Width.Should().Be(width);
            board.Ships.Count.Should().Be(shipCount);
        }

        /// <summary>
        /// Asserts state expectations against a ship model.
        /// </summary>
        /// <param name="ship">The actual ship.</param>
        /// <param name="isSunk">The expected sunk state flag.</param>
        /// <param name="orientation">The expected orientation.</param>
        /// <param name="segmentCount">The expected number of segments.</param>
        private static void AssertShip(Ship ship, bool isSunk, ShipOrientation orientation, int segmentCount)
        {
            ship.Should().NotBeNull();
            ship.IsSunk.Should().Be(isSunk);
            ship.Orientation.Should().Be(orientation);
            ship.Segments.Should().HaveCount(segmentCount);
        }

        /// <summary>
        /// Asserts state expectations against a ship segment model.
        /// </summary>
        /// <param name="segment">The actual segment.</param>
        /// <param name="ship">The expected ship.</param>
        /// <param name="isHit">The expected hit state flag.</param>
        /// <param name="xIndex">The expected X-index.</param>
        /// <param name="yIndex">The expected Y-index.</param>
        private static void AssertShipSegment(ShipSegment segment, Ship ship, bool isHit, int xIndex, int yIndex)
        {
            segment.Should().NotBeNull();
            segment.Ship.Should().Be(ship);
            segment.IsHit.Should().Be(isHit);
            segment.XIndex.Should().Be(xIndex);
            segment.YIndex.Should().Be(yIndex);
        }

        /// <summary>
        /// Asserts state expectations against an attack result model.
        /// </summary>
        /// <param name="result">The actual attack result.</param>
        /// <param name="isHit">The expected hit state flag.</param>
        /// <param name="isShipSunk">The expected ship sunk state flag.</param>
        /// <param name="isGameOver">The expected game over state flag.</param>
        private static void AssertAttackResult(AttackResult result, bool isHit, bool isShipSunk, bool isGameOver)
        {
            result.Should().NotBeNull();
            result.IsHit.Should().Be(isHit);
            result.IsShipSunk.Should().Be(isShipSunk);
            result.IsGameOver.Should().Be(isGameOver);
        }
        #endregion
    }
}