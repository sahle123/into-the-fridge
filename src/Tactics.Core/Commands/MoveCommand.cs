
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tactics.Core.Movement;

namespace Tactics.Core.Commands
{
    public sealed class MoveCommand : ICommand
    {
        private GridPos _from; // Remembered by Execute so Undo can restore it.
        private bool _executed;

        public int UnitId { get; }
        public GridPos Destination { get; }
        public bool IsUndoable => true;

        public MoveCommand(int unitId, GridPos destination)
        {

            UnitId = unitId;
            Destination = destination;
        }

        public CommandResult Validate(GameState state)
        {
            var unit = state.FindUnit(UnitId);
            if (unit is null || !unit.IsAlive)
            {
                return CommandResult.Fail("That unit doesn't exist.");
            }
            if (unit.Team != state.ActiveTeam)
            {
                return CommandResult.Fail($"It's not {unit.Name}'s turn.");
            }
            if (unit.HasMoved)
            {
                return CommandResult.Fail($"{unit.Name} has already moved this turn.");
            }
            if (!state.Map.IsWalkable(Destination))
            {
                return CommandResult.Fail("Can't move there.");
            }
            if (state.UnitAt(Destination) is not null)
            {
                return CommandResult.Fail("That tile is occupied."); // DEV-NOTE: This logic will be changed later. It does not consider walls.
            }
            //if (unit.Position.DistanceTo(Destination) > unit.MoveRange)
            //{
            //    return CommandResult.Fail($"Out of range ({unit.MoveRange}) tiles.");
            //}
            if (!Pathfinder.FindMoves(state, unit).CanReach(Destination))
            {
                return CommandResult.Fail("Can't reach that tile!");
            }

            return CommandResult.Success();
        }

        public void Execute(GameState state)
        {
            if (_executed) throw new InvalidOperationException("A command instance runs once.");

            var result = Validate(state);
            if (!result.Ok) throw new InvalidOperationException(result.Error);

            var unit = state.FindUnit(UnitId);
            _from = unit.Position;
            unit.Position = Destination;
            unit.HasMoved = true;
            _executed = true;
        }

        public void Undo(GameState state)
        {
            if (!_executed) throw new InvalidOperationException("Nothing to undo.");

            var unit = state.FindUnit(UnitId);
            unit.Position = _from;
            unit.HasMoved = false;
            _executed = false;
        }
    }
}
