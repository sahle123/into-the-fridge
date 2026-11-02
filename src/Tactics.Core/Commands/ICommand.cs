using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tactics.Core.Commands
{
    public readonly record struct CommandResult(bool Ok, string? Error = null)
    {
        public static CommandResult Success() => new(true);
        public static CommandResult Fail(string error) => new(false, error);
    }

    public interface ICommand
    {
        CommandResult Validate(GameState state);
        void Execute(GameState state);
        void Undo(GameState state);
    }
}
