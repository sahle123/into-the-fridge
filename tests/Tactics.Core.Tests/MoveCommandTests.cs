using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tactics.Core.Commands;

namespace Tactics.Core.Tests
{
    public class MoveCommandTests
    {
        private static (GameState, Unit Knight) NewBattle()
        {
            var state = new GameState(new BattleMap(8, 8));
            var knight = state.AddUnit("Knight", Team.Player, new GridPos(1, 1), maxHp: 3, moveRange: 3);
            return (state, knight);
        }

        [Fact]
        public void Moving_within_range_changes_position()
        {
            var (state, knight) = NewBattle();
            var move = new MoveCommand(knight.Id, new GridPos(3, 2));

            Assert.True(move.Validate(state).Ok);
            move.Execute(state);

            Assert.Equal(new GridPos(3, 2), knight.Position);
            Assert.True(knight.HasMoved);
        }

        [Fact]
        public void Moving_out_of_range_is_rejected()
        {
            var (state, knight) = NewBattle();
            var result = new MoveCommand(knight.Id, new GridPos(5, 5)).Validate(state);

            Assert.False(result.Ok);
            Assert.Equal(new GridPos(1, 1), knight.Position);
        }

        [Fact]
        public void Moving_onto_a_blocked_tile_is_rejected()
        {
            var (state, knight) = NewBattle();
            state.Map[new GridPos(2, 1)] = TileType.Blocked;

            Assert.False(new MoveCommand(knight.Id, new GridPos(2, 1)).Validate(state).Ok);
        }

        [Fact]
        public void Moving_onto_another_unit_is_rejected()
        {
            var (state, knight) = NewBattle();
            state.AddUnit("Brute", Team.Enemy, new GridPos(2, 1), maxHp: 4, moveRange: 2);

            Assert.False(new MoveCommand(knight.Id, new GridPos(2, 1)).Validate(state).Ok);
        }

        [Fact]
        public void A_unit_cannot_move_twice_in_one_turn()
        {
            var (state, knight) = NewBattle();
            new MoveCommand(knight.Id, new GridPos(2, 1)).Execute(state);

            Assert.False(new MoveCommand(knight.Id, new GridPos(3, 1)).Validate(state).Ok);
        }

        [Fact]
        public void Undo_restores_position_and_lets_the_unit_move_again()
        {
            var (state, knight) = NewBattle();
            var move = new MoveCommand(knight.Id, new GridPos(3, 2));
            move.Execute(state);
            move.Undo(state);

            Assert.Equal(new GridPos(1, 1), knight.Position);
            Assert.False(knight.HasMoved);
        }

        [Fact(Skip = "Week 2: movement must follow paths around walls")]
        public void Range_counts_the_path_around_walls_not_the_straight_distance()
        {
            var (state, knight) = NewBattle();
            // A wall down column 2 with a single gap at the bottom (y = 7).
            for (var y = 0; y < 7; y++) state.Map[new GridPos(2, y)] = TileType.Blocked;

            // (3, 1) is 2 tiles away in a straight line but 14 steps around the wall.

            Assert.False(new MoveCommand(knight.Id, new GridPos(3, 1)).Validate(state).Ok);
        }
    }
}
