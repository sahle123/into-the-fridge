using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tactics.Core.Commands;

namespace Tactics.Core.Tests
{
    public class TurnTests
    {
        private static (GameState state, Unit Knight, Unit Brute) NewBattle()
        {
            var state = new GameState(new BattleMap(8, 8));
            var knight = state.AddUnit("Knight", Team.Player, new GridPos(1, 1), maxHp: 3, moveRange: 3);
            var brute = state.AddUnit("Brute", Team.Enemy, new GridPos(6, 6), maxHp: 4, moveRange: 2);

            return (state, knight, brute);
        }

        [Fact]
        public void A_battle_starts_on_the_player_first_turn()
        {
            var (state, _, _) = NewBattle();
            Assert.Equal(BattlePhase.PlayerTurn, state.Phase);
            Assert.Equal(1, state.TurnNumber);
        }

        [Fact]
        public void A_full_round_starts_turn_2_and_give_player_units_their_moves_back()
        {
            var (state, knight, _) = NewBattle();
            new MoveCommand(knight.Id, new GridPos(2, 1)).Execute(state);

            new EndTurnCommand().Execute(state);
            Assert.Equal(BattlePhase.EnemyTurn, state.Phase);

            new EndTurnCommand().Execute(state);

            Assert.Equal(BattlePhase.PlayerTurn, state.Phase);
            Assert.Equal(2, state.TurnNumber);
            Assert.False(knight.HasMoved);
        }

        [Fact]
        public void Units_can_only_move_on_their_own_teams_turn()
        {
            var (state, knight, brute) = NewBattle();
            Assert.False(new MoveCommand(brute.Id, new GridPos(6, 5)).Validate(state).Ok);

            new EndTurnCommand().Execute(state);

            Assert.False(new MoveCommand(knight.Id, new GridPos(2, 1)).Validate(state).Ok);
            Assert.True(new MoveCommand(brute.Id, new GridPos(6, 5)).Validate(state).Ok);
        }

        [Fact]
        public void A_finished_battle_cannot_continue()
        {
            var (state, _, _) = NewBattle();

            state.TransitionTo(BattlePhase.Victory);

            Assert.Throws<InvalidOperationException>(() => state.TransitionTo(BattlePhase.PlayerTurn));
            Assert.False(new EndTurnCommand().Validate(state).Ok);
        }
    }
}
