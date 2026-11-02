using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tactics.Core
{
    public enum TileType { Floor, Blocked }

    public sealed class BattleMap
    {
        private readonly TileType[,] _tiles;
        
        public int Width { get; }
        public int Height { get; }

        public BattleMap(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Map must be at least 1x1.");
            Width = width;
            Height = height;
            _tiles = new TileType[width, height]; // every tile starts as Floor (enum value 0).
        }

        public bool InBounds(GridPos p) => p.X >= 0 && p.Y >= 0 && p.X < Width && p.Y < Height;
        public TileType this[GridPos p]
        {
            get => _tiles[p.X, p.Y];
            set => _tiles[p.X, p.Y] = value;
        }
        public bool IsWalkable(GridPos p) => InBounds(p) && this[p] == TileType.Floor;
    }
}
