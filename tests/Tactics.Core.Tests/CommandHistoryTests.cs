using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tactics.Core.Commands;

namespace Tactics.Core.Tests
{
    public class CommandHistoryTests
    {
        private static (GameState State, CommandHistory History, Unit Knight, Unit Archer) NewBattle()
        {
            var state = new GameState(new BattleMap(8, 8));
            var knight = state.AddUnit("Knight", Team.Player, new GridPos(1, 1), maxHp: 3, moveRange: 3);
            var archer = state.AddUnit("Archer", Team.Player, new GridPos(4, 4), maxHp: 2, moveRange: 2);
            return (state, new CommandHistory(), knight, archer);
        }

        [Fact]
        public void Undo_reverses_moves_in_reverse_order()
        {
            var (state, history, knight, archer) = NewBattle();

            history.Execute(new MoveCommand(knight.Id, new GridPos(2, 1)), state);
            history.Execute(new MoveCommand(archer.Id, new GridPos(4, 5)), state);

            Assert.True(history.Undo(state));
            Assert.Equal(new GridPos(4, 4), archer.Position);
            Assert.Equal(new GridPos(2, 1), knight.Position);

            Assert.True(history.Undo(state));
            Assert.Equal(new GridPos(1, 1), knight.Position);
            Assert.False(history.CanUndo);
        }

        [Fact]
        public void Invalid_commands_are_not_recorded()
        {
            var (state, history, knight, _) = NewBattle();

            var result = history.Execute(new MoveCommand(knight.Id, new GridPos(7, 7)), state);

            Assert.False(result.Ok);
            Assert.False(history.CanUndo);
            Assert.Empty(history.Log);
        }

        [Fact]
        public void Ending_the_turn_seals_everything_before_it()
        {
            var (state, history, knight, _) = NewBattle();
            history.Execute(new MoveCommand(knight.Id, new GridPos(2, 1)), state);
            history.Execute(new EndTurnCommand(), state);

            Assert.False(history.CanUndo);
            Assert.False(history.Undo(state));
            Assert.Equal(2, history.Log.Count);
        }
    }
}
