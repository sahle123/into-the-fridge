using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tactics.Core
{
    public enum Team { Player, Enemy }

    public sealed class Unit
    {
        public int Id { get; }
        public string Name { get; }
        public Team Team { get; }
        public int MaxHp { get; }
        public int MoveRange { get; }
        
        public GridPos Position { get; internal set; }
        public int Hp { get; internal set; }
        public bool HasMoved { get; internal set; }

        public bool IsAlive => Hp > 0;

        internal Unit(int id, string name, Team team, GridPos position, int maxHp, int moveRange)
        {
            Id = id; // DEV-NOTE: this seems like a bad idea. IDs shouldn't be set manually...
            Name = name;
            Team = team;
            Position = position;
            MaxHp = maxHp;
            Hp = maxHp;
            MoveRange = moveRange;
        }
    }
}
