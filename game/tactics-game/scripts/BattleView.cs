using System.Collections.Generic;
using System.Linq;
using Godot;
using Tactics.Core;
using Tactics.Core.Commands;

namespace TacticsGame.View
{
    public partial class BattleView : Node2D
    {
        private const int TileSize = 64;
        private static readonly Vector2 Origin = new(64, 64);
        private static readonly Vector2 TileVec = new(TileSize, TileSize);

        [Export] public Label StatusLabel { get; set; } = null!;

        private GameState _state = null!;
        private Unit? _selected;


        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _state = CreateTestBattle();
            StatusLabel.Text = "Click a blue unit.";
            QueueRedraw();
        }

        // Create a test board with a wall and 3 different units for the player.
        private static GameState CreateTestBattle()
        {
            var map = new BattleMap(8, 8);
            for (var x = 1; x <= 5; x++)
            {
                map[new GridPos(x, 4)] = TileType.Blocked; // A wall;
            }

            var state = new GameState(map);
            state.AddUnit("Knight", Team.Player, new GridPos(1, 6), maxHp: 3, moveRange: 3);
            state.AddUnit("Archer", Team.Player, new GridPos(3, 6), maxHp: 2, moveRange: 2);
            state.AddUnit("Brute", Team.Player, new GridPos(5, 1), maxHp: 4, moveRange: 2);
            return state;
        }

        // Coordinates: the only place that knows how big a tile is on screen.
        // DEV-NOTE: comment on this properly later with an example.
        private static Vector2 GridToLocal(GridPos gridPos) => Origin + new Vector2(gridPos.X, gridPos.Y) * TileSize;

        // DEV-NOTE: comment on this properly later with an example.
        private static GridPos LocalToGrid(Vector2 localPos)
        {
            var pos = (localPos - Origin) / TileSize;
            return new GridPos(Mathf.FloorToInt(pos.X), Mathf.FloorToInt(pos.Y));
        }

        // The view never re-implements movement rules; it asks MoveCommand.Validate
        private IEnumerable<GridPos> ReachableTiles(Unit unit)
        {
            for (var x = 0; x < _state.Map.Width; x++)
            {
                for (var y = 0; y < _state.Map.Height; y++)
                {
                    var pos = new GridPos(x, y);

                    if (new MoveCommand(unit.Id, pos).Validate(_state).Ok)
                        yield return pos;
                }
            }
        }

        // --- Drawing (Block 1 draws tiles + grid; Block 2 add units; Block 3 adds highlights) ---
        public override void _Draw()
        {
            var map = _state.Map;

            for (var x = 0; x < map.Width; x++)
            {
                for (var y = 0; y < map.Height; y++)
                {
                    var pos = new GridPos(x, y);
                    var rect = new Rect2(GridToLocal(pos), TileVec);
                    var colorFill = map[pos] == TileType.Blocked ? new Color(0.22f, 0.22f, 0.26f) : new Color(0.55f, 0.62f, 0.50f); // DEV-NOTE: I want these colors to be configurable in the class properties.
                    DrawRect(rect, colorFill);
                    DrawRect(rect, new Color(0, 0, 0, 0.35f), false, 1f); // Grid lines
                }
            }

            if (_selected is not null)
            {
                foreach (var gridPos in ReachableTiles(_selected))
                    DrawRect(new Rect2(GridToLocal(gridPos), TileVec), new Color(0.3f, 0.6f, 1f, 0.35f)); // DEV-NOTE: Change to configurable properties.
            }

            foreach (var unit in _state.Units.Where(u => u.IsAlive))
            {
                var center = GridToLocal(unit.Position) + TileVec / 2;
                var body = unit.Team == Team.Player
                    ? new Color(0.20f, 0.45f, 0.90f)
                    : new Color(0.85f, 0.25f, 0.20f); // DEV-NOTE: Change to configurable properties.
                DrawCircle(center, TileSize * 0.35f, body); // Friendlies are circles.

                if (unit == _selected)
                    DrawArc(center, TileSize * 0.42f, 0, Mathf.Tau, 32, Colors.White, 3f);
                DrawString(ThemeDB.FallbackFont, center + new Vector2(-6, 7), unit.Name[..1], HorizontalAlignment.Left, -1, 20, Colors.White);
            }
        }

        //
        // --- Input ---
        //
        public override void _UnhandledInput(InputEvent @event)
        {
            // Ignore any inputs that are not MouseButton.Left
            if (@event is not InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
                return;

            HandleClick(LocalToGrid(GetLocalMousePosition()));
            QueueRedraw();
        }

        private void HandleClick(GridPos clicked)
        {
            if (!_state.Map.InBounds(clicked))
            {
                _selected = null;
                StatusLabel.Text = "Click a blue units.";
                return;
            }

            var unitThere = _state.UnitAt(clicked);
            if (unitThere is { Team: Team.Player })
            {
                _selected = unitThere;
                StatusLabel.Text = $"{unitThere.Name} selected. Click a highlighted tile.";
                return;
            }

            if (_selected is null)
            {
                StatusLabel.Text = unitThere is null ? "Click a blue unit." : $"{unitThere.Name} is an enemy.";
                return;
            }

            var move = new MoveCommand(_selected.Id, clicked);
            var result = move.Validate(_state);
            if (!result.Ok)
            {
                StatusLabel.Text = result.Error ?? "Can't do that.";
                return;
            }
            move.Execute(_state);
            StatusLabel.Text = $"{_selected.Name} moved to {clicked}.";
            _selected = null;
        }


        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        { }
    }
}