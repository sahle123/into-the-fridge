using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tactics.Core
{
    public readonly record struct GridPos(int X, int Y)
    {
        public static readonly GridPos Up = new(0, -1);
        public static readonly GridPos Down = new(0, 1);
        public static readonly GridPos Left = new(-1, 0);
        public static readonly GridPos Right = new(1, 0);
        public static readonly IReadOnlyList<GridPos> Directions = new[] { Up, Down, Left, Right };

        public static GridPos operator +(GridPos a, GridPos b) => new(a.X + b.X, a.Y + b.Y);


        // Manhattan distance: steps needed with no diagonal moves and no obstacles.
        public int DistanceTo(GridPos other) => Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        public override string ToString() => $"({X}, {Y})";
    }
}
