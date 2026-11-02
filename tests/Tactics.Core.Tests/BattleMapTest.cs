using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tactics.Core.Tests
{
    public class BattleMapTest
    {
        [Fact]
        public void New_map_is_all_floor()
        {
            var map = new BattleMap(8, 8);
            Assert.True(map.IsWalkable(new GridPos(0, 0)));
            Assert.True(map.IsWalkable(new GridPos(7, 7)));
        }

        [Theory]
        [InlineData(-1, 0)]
        [InlineData(0, -1)]
        [InlineData(8, 0)]
        [InlineData(0, 8)]
        public void Positions_outside_the_map_are_not_walkable(int x, int y)
        {
            var map = new BattleMap(8, 8);
            Assert.False(map.IsWalkable(new GridPos(x, y)));
        }

        [Fact]
        public void Blocked_tiles_are_not_walkable()
        {
            var map = new BattleMap(8, 8);
            map[new GridPos(3, 3)] = TileType.Blocked;
            Assert.False(map.IsWalkable(new GridPos(3, 3)));
        }
    }
}
