using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tactics.Core.Commands
{
    public sealed class EndTurnCommand : ICommand
    {
        public bool IsUndoable => false;

        public CommandResult Validate(GameState state) =>
            state.ActiveTeam is null
                ? CommandResult.Fail("The battle is over.")
                : CommandResult.Success();
        public void Execute(GameState state)
        {
            var result = Validate(state);

            if (!result.Ok)
                throw new InvalidOperationException(result.Error);

            if (state.Phase == BattlePhase.PlayerTurn)
            {
                state.TransitionTo(BattlePhase.EnemyTurn);
            }
            else
            {
                state.TransitionTo(BattlePhase.PlayerTurn);
                state.TurnNumber++;
            }

            // The team whose turn starts get its moves back.
            foreach (var unit in state.Units.Where(u => u.Team == state.ActiveTeam))
            {
                unit.HasMoved = false;
            }
        }

        public void Undo(GameState state) =>
            throw new NotSupportedException("Ending a turn can't be undone.");
    }
}
