using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tactics.Core.Commands
{
    public sealed class CommandHistory
    {
        private readonly Stack<ICommand> _undoable = new();
        private readonly List<ICommand> _log = new();

        public IReadOnlyList<ICommand> Log => _log;
        public bool CanUndo => _undoable.Count > 0;

        // The one way commands should be run from now on: validate, execute, record.
        public CommandResult Execute(ICommand command, GameState state)
        {
            var result = command.Validate(state);

            if (!result.Ok)
                return result;

            command.Execute(state);
            _log.Add(command);

            if (command.IsUndoable)
                _undoable.Push(command);
            else
                _undoable.Clear(); // Nothing before a non-undoable command can be undone.

            return result;
        }

        public bool Undo(GameState state)
        {
            if (!CanUndo)
                return false;

            var command = _undoable.Pop();
            command.Undo(state);

            _log.RemoveAt(_log.Count - 1);
            return true;
        }
    }
}
