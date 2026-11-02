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

        public BattleMap Map { get; }
        public IReadOnlyList<Unit> Units => _units;

        public GameState(BattleMap map)
        {
            Map = map;
        }

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
