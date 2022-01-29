using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Entities;
using Runtime.Extensions;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

namespace Runtime {
    public class World : MonoBehaviour {
        readonly struct WorldCell : ICell {
            public WorldCell(Vector3Int gridPosition, Vector3 worldPosition) {
                this.gridPosition = gridPosition;
                this.worldPosition = worldPosition;
            }

            public Vector3Int gridPosition { get; }
            public Vector3 worldPosition { get; }

            public ITile tile => instance.GetTileByPosition(gridPosition);
            public IEnumerable<IEntity> entities => instance.GetEntitiesByPosition(gridPosition);
        }

        readonly struct WorldTile : ITile {
            public WorldTile(Vector3Int gridPosition, GameObject gameObject, TileBase type) {
                this.gameObject = gameObject;
                this.type = type;
                this.gridPosition = gridPosition;
            }
            public GameObject gameObject { get; }
            public TileBase type { get; }
            public Vector3Int gridPosition { get; }
            public Vector3 worldPosition => gameObject.transform.position;
            public ICell ownerCell => instance.GetCellByPosition(gridPosition);
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

        public delegate void OnSeasonChange();

        [Header("MonoBehaviour configuration")]
        [SerializeField]
        Grid grid = default;
        [SerializeField]
        Tilemap groundTilemap = default;
        [SerializeField]
        Transform entitiesContainer = default;

        [Header("Seasons")]
        [SerializeField]
        Season m_season = Season.Spring;
        public Season season => m_season;
        [SerializeField, Range(0, 60)]
        float minSeasonDuration = 1;
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
                var type = groundTilemap.GetTile(gridPosition);
                if (type) {
                    var worldPosition = groundTilemap.CellToWorld(gridPosition);

                    var obj = groundTilemap.GetInstantiatedObject(gridPosition);
                    Assert.IsTrue(obj, $"Missing object for tile {type} at position {worldPosition}");

                    tiles[gridPosition] = new WorldTile(gridPosition, obj, type);
                    cells[gridPosition] = new WorldCell(gridPosition, worldPosition);
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

        public bool TryGetCell(Vector3Int gridPosition, out ICell cell) {
            if (cells.TryGetValue(gridPosition, out var c)) {
                cell = c;
                return true;
            } else {
                cell = default;
                return false;
            }
        }

        public IEnumerator AdvanceSeasonRoutine() {
            ProcessQueues();

            m_season = (Season)(((int)m_season + 1) % 4);

            BroadcastMessage(nameof(OnSeasonChange));

            yield return Wait.forSeconds[minSeasonDuration];

            ProcessQueues();
        }
        void ProcessQueues() {
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

        #region Grid interactions
        public Vector3Int WorldToGrid(Vector3 position) => groundTilemap.WorldToCell(position);
        public Vector3 GridToWorld(Vector3Int position) => groundTilemap.CellToWorld(position);

        static readonly Vector3Int[] evenNeighbors = new[] {
            new Vector3Int(1, 0),
            new Vector3Int(0, -1),
            new Vector3Int(-1, -1),
            new Vector3Int(-1, 0),
            new Vector3Int(0, 1),
            new Vector3Int(-1, 1),
        };
        static readonly Vector3Int[] oddNeighbors = new[] {
            new Vector3Int(1, 1),
            new Vector3Int(1, 0),
            new Vector3Int(0, -1),
            new Vector3Int(-1, 0),
            new Vector3Int(1, -1),
            new Vector3Int(0, 1),
        };

        public static IEnumerable<Vector3Int> GetNeighboringPositions(Vector3Int position) {
            var neighbors = (position.y & 1) == 1
                ? oddNeighbors
                : evenNeighbors;
            return neighbors.Select(offset => offset + position);
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

        /// <summary>
        /// TODO: calculate actual hex distance
        /// </summary>
        /// <param name="positionA"></param>
        /// <param name="positionB"></param>
        /// <returns></returns>
        public static float Distance(Vector3Int positionA, Vector3Int positionB)
            => (positionA - positionB).magnitude;
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

            var instance = Instantiate(type.prefab, cell.worldPosition, Quaternion.identity, entitiesContainer);

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

        public ICell GetCellByPosition(Vector3Int gridPosition)
            => cells[gridPosition];

        public ITile GetTileByTileObject(GameObject tileObject)
            => tiles.Values.First(tile => tile.gameObject == tileObject);
        public ITile GetTileByPosition(Vector3Int position)
            => tiles[position];

        public IEnumerable<Vector3Int> CalculatePath(Vector3Int oldPosition, Vector3Int newPosition) {
            yield return newPosition;
        }

        #endregion
    }
}