using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tactics.Core.Tests
{
    public class GameStateTests
    {
        [Fact]
        public void Added_units_get_unit_ids_and_can_be_found_by_position()
        {
            var state = new GameState(new BattleMap(8, 8));
            var a = state.AddUnit("A", Team.Player, new GridPos(0, 0), maxHp: 3, moveRange: 3);
            var b = state.AddUnit("B", Team.Enemy, new GridPos(1, 0), maxHp: 3, moveRange: 3);

            Assert.NotEqual(a.Id, b.Id);
            Assert.Same(b, state.UnitAt(new GridPos(1, 0)));
        }

        [Fact]
        public void Cannot_place_a_unit_on_a_blocked_tile()
        {
            var map = new BattleMap(8, 8);
            map[new GridPos(2, 2)] = TileType.Blocked;
            var state = new GameState(map);

            Assert.Throws<ArgumentException>(() =>
            {
                state.AddUnit("A", Team.Player, new GridPos(2, 2), maxHp: 3, moveRange: 3);
            });
        }

        [Fact]
        public void Cannot_place_two_units_on_one_tile()
        {
            var state = new GameState(new BattleMap(8, 8));
            state.AddUnit("A", Team.Player, new GridPos(0, 0), maxHp: 3, moveRange: 3);

            Assert.Throws<ArgumentException>(() =>
            {
                state.AddUnit("B", Team.Enemy, new GridPos(0, 0), maxHp: 3, moveRange: 3);
            });
        }
    }
}
