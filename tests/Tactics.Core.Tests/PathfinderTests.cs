using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tactics.Core.Movement;

namespace Tactics.Core.Tests
{
    public class PathfinderTests
    {
        [Fact]
        public void Open_ground_reaches_every_tile_within_range()
        {
            var state = new GameState(new BattleMap(8, 8));
            var knight = state.AddUnit("Knight", Team.Player, new GridPos(3, 3), maxHp: 3, moveRange: 2);

            var moves = Pathfinder.FindMoves(state, knight);

            // A diamond of radius 2, holds 13 tiles; the start tile isn't a destination.
            Assert.Equal(12, moves.Destinations.Count);
            Assert.True(moves.CanReach(new GridPos(5, 3)));
            Assert.False(moves.CanReach(new GridPos(5, 4)));
        }

        [Fact]
        public void Allies_can_be_passed_but_not_stopped_on()
        {
            var state = new GameState(new BattleMap(8, 8));
            var knight = state.AddUnit("Knight", Team.Player, new GridPos(1, 1), maxHp: 3, moveRange: 2);
            state.AddUnit("Archer", Team.Player, new GridPos(2, 1), maxHp: 2, moveRange: 2);

            var moves = Pathfinder.FindMoves(state, knight);

            Assert.True(moves.CanReach(new GridPos(3, 1)));  // only 2 steps by going through the archer.
            Assert.False(moves.CanReach(new GridPos(2, 1))); // but not onto it.
        }

        [Fact]
        public void Enemies_block_movement()
        {
            var state = new GameState(new BattleMap(8, 8));
            var knight = state.AddUnit("Knight", Team.Player, new GridPos(1, 1), maxHp: 3, moveRange: 2);
            state.AddUnit("Brute", Team.Enemy, new GridPos(2, 1), maxHp: 4, moveRange: 2);

            Assert.False(Pathfinder.FindMoves(state, knight).CanReach(new GridPos(3, 1)));
        }

        [Fact]
        public void PathTo_returns_adjacent_walkable_steps_ending_at_the_destinations()
        {
            var map = new BattleMap(8, 8);
            // The wall!
            for (var y = 0; y < 7; y++)
            {
                map[new GridPos(2, y)] = TileType.Blocked;
            }
            var state = new GameState(map);
            // Turbo knight!
            var knight = state.AddUnit("Knight", Team.Player, new GridPos(1, 1), maxHp: 3, moveRange: 14);

            var path = Pathfinder.FindMoves(state, knight).PathTo(new GridPos(3, 1));

            Assert.Equal(14, path.Count);
            Assert.Equal(new GridPos(3, 1), path[^1]);

            var previous = knight.Position;
            foreach (var step in path)
            {
                Assert.Equal(1, previous.DistanceTo(step));
                Assert.True(map.IsWalkable(step));
                previous = step;
            }
        }
    }
}
