using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tactics.Core.Movement
{
    public sealed class MovementMap
    {
        private readonly Dictionary<GridPos, GridPos> _cameFrom;
        private readonly HashSet<GridPos> _destinations;

        internal MovementMap(GridPos start, Dictionary<GridPos, GridPos> cameFrom, HashSet<GridPos> destinations)
        {
            Start = start;
            _cameFrom = cameFrom;
            _destinations = destinations;
        }

        public GridPos Start { get; }
        public IReadOnlyCollection<GridPos> Destinations => _destinations;
        public bool CanReach(GridPos tile) => _destinations.Contains(tile);

        // Using the stored _cameFrom BFS tree, we add the actual tiles the unit will take to reach their destination.
        // Or we throw an exception if it's not reachable. DEV-NOTE: Should we refactor this exception to better error handling???
        public IReadOnlyList<GridPos> PathTo(GridPos destination)
        {
            if (!CanReach(destination))
                throw new ArgumentException($"{destination} is not reachable.");

            var path = new List<GridPos>();
            for (var p = destination; p != Start; p = _cameFrom[p])
                path.Add(p);
            path.Reverse();
            return path;
        }
    }
}
