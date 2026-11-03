using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tactics.Core
{
    public sealed class GameState
    {
        private readonly List<Unit> _units = new();

        // Valid battle phases
        private static readonly Dictionary<BattlePhase, BattlePhase[]> AllowedTransitions = new()
        {
            [BattlePhase.PlayerTurn] = new[] { BattlePhase.EnemyTurn, BattlePhase.Victory, BattlePhase.Defeat },
            [BattlePhase.EnemyTurn] = new[] { BattlePhase.PlayerTurn, BattlePhase.Victory, BattlePhase.Defeat },
            [BattlePhase.Victory] = Array.Empty<BattlePhase>(),
            [BattlePhase.Defeat] = Array.Empty<BattlePhase>(),
        };

        // Attempt to transition to another phase and throw an exception if it is not defined in `AllowedTransitions`.
        internal void TransitionTo(BattlePhase next)
        {
            if (!AllowedTransitions[Phase].Contains(next))
                throw new InvalidOperationException($"Can't go from {Phase} to {next}");
            Phase = next;
        }

        public BattleMap Map { get; }
        public BattlePhase Phase { get; private set; } = BattlePhase.PlayerTurn;
        public int TurnNumber { get; internal set; } = 1;
        public IReadOnlyList<Unit> Units => _units;

        public GameState(BattleMap map)
        {
            Map = map;
        }

        public Team? ActiveTeam => Phase switch
        {
            BattlePhase.PlayerTurn => Team.Player,
            BattlePhase.EnemyTurn => Team.Enemy,
            _ => null,
        };

        public Unit AddUnit(string name, Team team, GridPos position, int maxHp, int moveRange)
        {
            if (!Map.IsWalkable(position))
                throw new ArgumentException($"{position} is not walkable.", nameof(position));

            if (UnitAt(position) is not null)
                throw new ArgumentException($"{position} is already occupied.", nameof(position));

            var unit = new Unit(_units.Count + 1, name, team, position, maxHp, moveRange);

            _units.Add(unit);
            return unit;
        }

        public Unit? UnitAt(GridPos p) => _units.FirstOrDefault(u => u.IsAlive && u.Position == p);

        public Unit? FindUnit(int id) => _units.FirstOrDefault(u => u.Id == id);
    }
}
