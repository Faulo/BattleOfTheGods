using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Entities;
using Runtime.Extensions;
using Runtime.Tiles;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

namespace Runtime {
    public class World : MonoBehaviour {
        class WorldCell : ICell {
            public WorldCell(Vector3Int gridPosition, Vector3 worldPosition) {
                this.gridPosition = gridPosition;
                this.worldPosition = worldPosition;
            }

            public Vector3Int gridPosition { get; }
            public Vector3 worldPosition { get; }

            public ITile tile => instance.GetTileByPosition(gridPosition);
            public IEnumerable<IEntity> entities => instance.GetEntitiesByPosition(gridPosition);

            public int influence = 0;
            public Faction owningFaction => Math.Sign(influence) switch {
                1 => Faction.Civilization,
                0 => Faction.Nobody,
                -1 => Faction.Nature,
                _ => throw new NotImplementedException(),
            };

            public override string ToString() => $"{tile} ({string.Join(", ", entities.Select(e => e.type.name))})";
        }

        class WorldTile : ITile {
            public WorldTile(Vector3Int gridPosition, GameObject gameObject, ScriptableTile type) {
                this.gameObject = gameObject;
                this.type = type;
                this.gridPosition = gridPosition;
            }
            public GameObject gameObject { get; }
            public ScriptableTile type { get; }
            public Vector3Int gridPosition { get; }
            public Vector3 worldPosition => gameObject.transform.position;
            public ICell ownerCell => instance.GetCellByPosition(gridPosition);

            int m_borderId;
            public int border {
                get => m_borderId;
                set {
                    if (m_borderId != value) {
                        m_borderId = value;
                        UpdateColor();
                    }
                }
            }
            int m_emissionId;
            public int emission {
                get => m_emissionId;
                set {
                    if (m_emissionId != value) {
                        m_emissionId = value;
                        UpdateColor();
                    }
                }
            }

            void UpdateColor() {
                var color = new Color(type.tileId / 256f, m_borderId / 2f, m_emissionId / 256f, 0);
                instance.groundTilemap.SetColor(gridPosition, color);
            }

            public override string ToString() => $"{type.name} {gridPosition}";
        }
        class WorldEntity : IEntity {
            public WorldEntity(Vector3Int gridPosition, GameObject gameObject, EntityData type) {
                this.gameObject = gameObject;
                this.type = type;
                this.gridPosition = gridPosition;
            }
            public GameObject gameObject { get; }
            public EntityData type { get; }
            public Vector3Int gridPosition { get; set; }
            public Vector3 worldPosition => gameObject.transform.position;
            public ICell ownerCell => instance.GetCellByPosition(gridPosition);
            public override string ToString() => $"{type.name} {gridPosition}";
        }

        static World m_instance;
        public static World instance {
            get {
                if (!m_instance) {
                    m_instance = FindObjectOfType<World>();
                }
                return m_instance;
            }
        }

        public static event Action<IEntity> onSpawnEntity;
        public static event Action<IEntity> onMoveEntity;
        public static event Action<IEntity> onDestroyEntity;
        public static event Action<ICell> onChangeFaction;
        public static event Action<ICell, Faction, int> onAttackByFaction;

        public delegate void OnSeasonChange();
        public delegate void OnAwardInfluence();

        [Header("MonoBehaviour configuration")]
        [SerializeField]
        Grid grid = default;
        [SerializeField]
        Tilemap groundTilemap = default;
        [SerializeField]
        Transform entitiesContainer = default;

        [Header("Cell Settings")]
        [SerializeField, Range(0, 1)]
        float maxDistanceToCenter = 0.25f;
        public Vector3 randomDistanceToCenter {
            get {
                var offset = UnityEngine.Random.insideUnitCircle * maxDistanceToCenter;
                return new Vector3(offset.x, 0, offset.y);
            }
        }

        [Header("Seasons")]
        [SerializeField]
        Season m_season = Season.Spring;
        public Season season => m_season;
        [SerializeField, Range(0, 60)]
        float seasonDuration = 0.25f;
        [SerializeField, Range(0, 60)]
        float combatDuration = 0.25f;
        [SerializeField, Range(0, 60)]
        float influenceDuration = 0.25f;

        [Space]
        [SerializeField]
        bool autoAdvanceSeasons = false;
        [SerializeField, Range(0, 60)]
        float autoAdvanceSeasonDuration = 1;


        public IEnumerable<ICell> cellValues => cells.Values.Cast<ICell>();
        readonly Dictionary<Vector3Int, WorldCell> cells = new Dictionary<Vector3Int, WorldCell>();
        readonly Dictionary<Vector3Int, WorldTile> tiles = new Dictionary<Vector3Int, WorldTile>();
        readonly HashSet<WorldEntity> entities = new HashSet<WorldEntity>();
        readonly HashSet<WorldEntity> instantiationQueue = new HashSet<WorldEntity>();
        readonly Dictionary<WorldEntity, Vector3Int> moveQueue = new Dictionary<WorldEntity, Vector3Int>();
        readonly HashSet<WorldEntity> destructionQueue = new HashSet<WorldEntity>();

        protected void OnValidate() {
            if (!grid) {
                transform.TryGetComponentInChildren(out grid);
            }
            if (!groundTilemap) {
                transform.TryGetComponentInChildren(out groundTilemap);
            }
        }

        void SetUpCells() {
            foreach (var gridPosition in groundTilemap.cellBounds.allPositionsWithin) {
                var type = groundTilemap.GetTile<ScriptableTile>(gridPosition);
                if (type) {
                    var worldPosition = groundTilemap.CellToWorld(gridPosition);

                    var obj = groundTilemap.GetInstantiatedObject(gridPosition);
                    Assert.IsTrue(obj, $"Missing object for tile {type} at position {worldPosition}");

                    cells[gridPosition] = new WorldCell(gridPosition, worldPosition);
                    tiles[gridPosition] = new WorldTile(gridPosition, obj, type);
                }
            }
            foreach (var entityController in entitiesContainer.GetComponentsInChildren<EntityController>()) {
                var gridPosition = groundTilemap.WorldToCell(entityController.transform.position);
                Assert.IsTrue(cells.ContainsKey(gridPosition), $"Entity {entityController} is out of bounds");

                var entity = new WorldEntity(gridPosition, entityController.gameObject, entityController.type);

                entities.Add(entity);
            }
        }

        void Awake() {
            SetUpCells();
        }

        IEnumerator Start() {
            while (true) {
                if (autoAdvanceSeasons) {
                    yield return AdvanceSeasonRoutine();
                }
                yield return Wait.forSeconds[autoAdvanceSeasonDuration];
            }
        }

        public IEnumerator AdvanceSeasonRoutine() {
            ProcessEntities();

            m_season = (Season)(((int)m_season + 1) % 4);

            BroadcastMessage(nameof(OnSeasonChange), SendMessageOptions.DontRequireReceiver);
            yield return Wait.forSeconds[seasonDuration];
            ProcessEntities();

            ProcessCombat();
            yield return Wait.forSeconds[combatDuration];
            ProcessEntities();

            BroadcastMessage(nameof(OnAwardInfluence), SendMessageOptions.DontRequireReceiver);
            yield return Wait.forSeconds[influenceDuration];
            ProcessEntities();
        }
        void ProcessEntities() {
            foreach (var entity in instantiationQueue) {
                entities.Add(entity);
            }
            instantiationQueue.Clear();

            foreach (var (entity, position) in moveQueue) {
                entity.gridPosition = position;
            }
            moveQueue.Clear();

            foreach (var entity in destructionQueue) {
                entities.Remove(entity);
                Destroy(entity.gameObject);
            }
            destructionQueue.Clear();
        }
        void ProcessCombat() {
            foreach (var cell in cells.Values) {
                var factions = new Dictionary<Faction, List<IEntity>>() {
                    [Faction.Civilization] = new List<IEntity>(),
                    [Faction.Nature] = new List<IEntity>(),
                    [Faction.Nobody] = new List<IEntity>(),
                };
                foreach (var entity in cell.entities) {
                    if (entity.type.canParticipateInCombat) {
                        factions[entity.type.faction].Add(entity);
                    }
                }
                foreach (var (attacker, defender) in new[] { (Faction.Civilization, Faction.Nature), (Faction.Nature, Faction.Civilization) }) {
                    int attack = factions[attacker].Sum(entity => entity.type.attack);
                    if (factions[attacker].Any(entity => entity.type.canStartCombat) && factions[defender].Count > 0) {
                        onAttackByFaction?.Invoke(cell, attacker, attack);
                        foreach (var entity in factions[defender].OrderBy(entity => entity.type.health)) {
                            if (attack >= entity.type.health) {
                                attack -= entity.type.health;
                                DestroyEntity(entity);
                            } else {
                                break;
                            }
                        }
                    }
                }
            }
        }

        #region Grid interactions
        public Vector3Int WorldToGrid(Vector3 position) => groundTilemap.WorldToCell(position);
        public Vector3 GridToWorld(Vector3Int position) => groundTilemap.CellToWorld(position);

        #region axial coordinates
        public static Vector3Int AxialToGrid(Vector2Int position) {
            int col = position.x + ((position.y - (position.y & 1)) / 2);
            int row = position.y;
            return new Vector3Int(col, row);
        }
        public static Vector2Int GridToAxial(Vector3Int position) {
            int q = position.x - ((position.y - (position.y & 1)) / 2);
            int r = position.y;
            return new Vector2Int(q, r);
        }
        static readonly Vector2Int[] axialNeighbors = new[] {
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1),
            new Vector2Int(0, 1),
        };
        public static IEnumerable<Vector3Int> GetNeighboringPositions(Vector3Int position) {
            var axial = GridToAxial(position);
            return axialNeighbors
                .Select(offset => offset + axial)
                .Select(AxialToGrid);
        }
        public static IEnumerable<Vector3Int> GetCircularPositions(Vector3Int position, int radius) {
            yield return position;

            var positions = new HashSet<Vector3Int> { position };
            for (int i = 0; i < radius; i++) {
                foreach (var newPosition in positions.ToList()) {
                    foreach (var setPosition in GetNeighboringPositions(newPosition)) {
                        if (positions.Add(setPosition)) {
                            yield return setPosition;
                        }
                    }
                }
            }
        }
        public static int Distance(Vector3Int positionA, Vector3Int positionB)
            => AxialMagnitude(GridToAxial(positionA) - GridToAxial(positionB));
        public static int AxialMagnitude(Vector2Int position)
            => (Math.Abs(position.x) + Math.Abs(position.x + position.y) + Math.Abs(position.y)) / 2;
        #endregion

        #region cube coordinates

        static Vector3Int CubeScale(Vector3Int pos, int factor) {
            return new Vector3Int(pos.x * factor, pos.y * factor, pos.z * factor);
        }
        //From: https://github.com/Unity-Technologies/2d-extras/issues/69
        public static Vector3Int GridToCube(Vector3Int cell) {
            int yCell = cell.x;
            int xCell = cell.y;
            int x = yCell - ((xCell - (xCell & 1)) / 2);
            int z = xCell;
            int y = -x - z;
            return new Vector3Int(x, y, z);
        }
        public static Vector3Int CubeToGrid(Vector3Int cube) {
            int x = cube.x;
            int z = cube.z;
            int col = x + ((z - (z & 1)) / 2);
            int row = z;

            return new Vector3Int(col, row, 0);
        }
        static readonly Vector3Int[] cubeNeighbours = new[] {
            new Vector3Int(1, 0, -1),
            new Vector3Int(1, -1, 0),
            new Vector3Int(0, -1, 1),
            new Vector3Int(-1, 0, 1),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(0, 1, -1)
        };
        static Vector3Int CubeNeighbour(Vector3Int pos, int neighbour) {
            Assert.IsTrue(neighbour >= 0 && neighbour < 6, "Invalid neighbour index in cube coordinates");
            return CubeAdd(pos, cubeNeighbours[neighbour]);
        }
        static Vector3Int CubeAdd(Vector3Int pos, Vector3Int dir) {
            return new Vector3Int(pos.x + dir.x, pos.y + dir.x, pos.z + dir.y);
        }
        public static IEnumerable<Vector3Int> GetRing(Vector3Int center, int distance) {
            if (distance == 0) {
                return new HashSet<Vector3Int> { center };
            }

            var output = new HashSet<Vector3Int>();
            center = GridToCube(center);
            var curr = CubeAdd(center, CubeScale(cubeNeighbours[4], distance));

            for (int i = 0; i < 6; i++) {
                for (int j = 0; j < distance; j++) {
                    output.Add(CubeToGrid(curr));
                    curr = CubeNeighbour(curr, i);
                }
            }

            return output;
        }
        public static IEnumerable<Vector3Int> GetInDistance(Vector3Int center, int distance, bool excludeSelf = false) {
            var output = new HashSet<Vector3Int>();
            center = GridToCube(center);
            for (int x = -distance; x <= distance; x++) {
                for (int z = Mathf.Max(-distance, -x - distance); z <= Mathf.Min(distance, -x + distance); z++) {
                    if (excludeSelf && x == 0 && z == 0) {
                        continue;
                    }

                    int y = -x - z;
                    var toAdd = new Vector3Int(center.x + x, center.y + y, center.z + z);

                    output.Add(CubeToGrid(toAdd));
                }
            }

            return output;
        }
        #endregion

        #endregion

        #region Entity interactions
        public void InstantiateEntity(ICell cell, EntityData type)
            => InstantiateEntity(cell.gridPosition, type);
        public void InstantiateEntity(ITile tile, EntityData type)
            => InstantiateEntity(tile.gridPosition, type);
        public void InstantiateEntity(Vector3Int position, EntityData type) {
            if (!cells.TryGetValue(position, out var cell)) {
                Debug.LogWarning($"Position {position} is out of bounds");
                return;
            }

            var instance = Instantiate(type.prefab, cell.worldPosition + randomDistanceToCenter, Quaternion.identity, entitiesContainer);

            var entity = new WorldEntity(position, instance, type);

            instantiationQueue.Add(entity);

            onSpawnEntity?.Invoke(entity);
        }
        public void MoveEntity(IEntity entity, Vector3Int newPosition) {
            if (!cells.TryGetValue(newPosition, out var newCell)) {
                Debug.LogWarning($"Position {newPosition} is out of bounds");
                return;
            }

            moveQueue[entity as WorldEntity] = newPosition;

            onMoveEntity?.Invoke(entity);
        }
        public void DestroyEntity(IEntity entity) {
            destructionQueue.Add(entity as WorldEntity);

            onDestroyEntity?.Invoke(entity);
        }
        #endregion


        #region data retrieval
        public IEnumerable<ICell> GetNeighboringCells(ICell cell)
            => GetNeighboringCells(cell.gridPosition);
        public IEnumerable<ICell> GetNeighboringCells(Vector3Int position) {
            foreach (var neighbor in GetNeighboringPositions(position)) {
                if (TryGetCell(neighbor, out var cell)) {
                    yield return cell;
                }
            }
        }
        public IEnumerable<ICell> GetCircularCells(Vector3Int position, int radius) {
            foreach (var neighbor in GetCircularPositions(position, radius)) {
                if (TryGetCell(neighbor, out var cell)) {
                    yield return cell;
                }
            }
        }

        public IEntity GetEntityByEntityObject(GameObject entityObject) => entities
            .Union(instantiationQueue)
            .First(entity => entity.gameObject == entityObject);

        public IEnumerable<IEntity> GetEntitiesByCell(ICell cell)
            => GetEntitiesByPosition(cell.gridPosition);
        public IEnumerable<IEntity> GetEntitiesByPosition(Vector3Int gridPosition)
            => entities.Where(entity => entity.gridPosition == gridPosition);
        public IEnumerable<IEntity> GetEntitiesByType(EntityData type)
            => entities.Where(entity => entity.type == type);

        public bool TryGetCell(Vector3Int gridPosition, out ICell cell) {
            if (cells.TryGetValue(gridPosition, out var c)) {
                cell = c;
                return true;
            } else {
                cell = default;
                return false;
            }
        }
        public ICell GetCellByPosition(Vector3Int gridPosition)
            => cells[gridPosition];

        public bool TryGetTile(Vector3Int gridPosition, out ITile tile) {
            if (tiles.TryGetValue(gridPosition, out var t)) {
                tile = t;
                return true;
            } else {
                tile = default;
                return false;
            }
        }
        public ITile GetTileByTileObject(GameObject tileObject)
            => tiles.Values.First(tile => tile.gameObject == tileObject);
        public ITile GetTileByPosition(Vector3Int position)
            => tiles[position];

        public bool TryCalculatePath(Vector3Int oldPosition, Vector3Int newPosition, out List<Vector3Int> path) {
            if (oldPosition == newPosition) {
                path = default;
                return false;
            }

            var openSet = new HashSet<Vector3Int> { oldPosition };

            var cameFrom = new Dictionary<Vector3Int, Vector3Int>();

            var gScore = new Dictionary<Vector3Int, float> {
                [oldPosition] = 0
            };

            var fScore = new Dictionary<Vector3Int, float> {
                [oldPosition] = 1
            };

            while (openSet.Count > 0) {
                var current = openSet.OrderBy(position => fScore[position]).First();
                if (current == newPosition) {
                    path = new List<Vector3Int> { };
                    while (current != oldPosition) {
                        path.Add(current);
                        current = cameFrom[current];
                    };
                    path.Reverse();
                    return true;
                }
                openSet.Remove(current);
                foreach (var neighbor in GetNeighboringPositions(current)) {
                    if (TryGetTile(neighbor, out var tile)) {
                        float tentative_gScore = gScore[current] + Distance(current, neighbor);
                        if (!gScore.ContainsKey(neighbor) || tentative_gScore < gScore[neighbor]) {
                            cameFrom[neighbor] = current;
                            gScore[neighbor] = tentative_gScore;
                            fScore[neighbor] = tentative_gScore + tile.type.movementCost;
                            openSet.Add(neighbor);
                        }
                    }
                }
            }
            path = default;
            return false;
        }

        #endregion

        #region influence
        public void AddInfluence(Vector3Int position, int influence) {
            if (!cells.TryGetValue(position, out var cell)) {
                Debug.LogWarning($"Position {position} is out of bounds");
                return;
            }
            var currentFaction = cell.owningFaction;
            cell.influence += influence;
            if (cell.owningFaction != currentFaction) {
                onChangeFaction?.Invoke(cell);
            }
        }
        #endregion
    }
}