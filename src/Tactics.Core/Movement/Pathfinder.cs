using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tactics.Core.Movement
{
    public static class Pathfinder
    {
        public static MovementMap FindMoves(GameState state, Unit unit)
        {
            var steps = new Dictionary<GridPos, int> { [unit.Position] = 0 };
            var cameFrom = new Dictionary<GridPos, GridPos>();
            var destinations = new HashSet<GridPos>();
            var frontier = new Queue<GridPos>();
            frontier.Enqueue(unit.Position);

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                if (steps[current] == unit.MoveRange) continue; // As far as the unit can go.

                // Search in all 4 directions
                foreach (var direction in GridPos.Directions)
                {
                    var next = current + direction;

                    // Check if we already walked or the path is not walkable.
                    if (steps.ContainsKey(next) || !state.Map.IsWalkable(next))
                        continue;

                    var occupant = state.UnitAt(next);

                    // Check if enemy block.
                    if (occupant is not null && occupant.Team != unit.Team)
                        continue;

                    steps[next] = steps[current] + 1; // increment step.
                    cameFrom[next] = current; // record previous tile.
                    frontier.Enqueue(next); // check the next tile.

                    if (occupant is null)
                        destinations.Add(next); // allies: pass through, can't stop.
                }
            }

            return new MovementMap(unit.Position, cameFrom, destinations);
        }
    }
}
