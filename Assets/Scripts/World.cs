using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Extensions;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

namespace Runtime {
    public class World : MonoBehaviour {
        static World m_instance;
        public static World instance {
            get {
                if (!m_instance) {
                    m_instance = FindObjectOfType<World>();
                }
                return m_instance;
            }
        }

        public event Action<Season> onSeasonChange;

        [Header("MonoBehaviour configuration")]
        [SerializeField]
        Grid grid = default;
        [SerializeField]
        Tilemap groundTilemap = default;
        [SerializeField]
        Transform entitiesContainer = default;
        [Header("Debug settings")]
        [SerializeField]
        public Season currentSeason = Season.Spring;
        [SerializeField]
        public bool autoAdvanceSeasons = false;
        [SerializeField, Range(0, 60)]
        public float autoAdvanceSeasonDuration = 1;

        public IEnumerable<WorldCell> cellValues => cells.Values;
        readonly Dictionary<Vector3Int, WorldCell> cells = new Dictionary<Vector3Int, WorldCell>();

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
                var tile = groundTilemap.GetTile(gridPosition);
                if (tile) {
                    var worldPosition = groundTilemap.CellToWorld(gridPosition);
                    cells[gridPosition] = new WorldCell(gridPosition, worldPosition, tile);
                }
            }
            foreach (Transform entity in entitiesContainer) {
                var gridPosition = groundTilemap.WorldToCell(entity.position);
                Assert.IsTrue(cells.ContainsKey(gridPosition), $"Entity {entity} is out of bounds");
                cells[gridPosition].entities.Add(entity.gameObject);
            }
        }

        void Awake() {
            SetUpCells();
        }

        IEnumerator Start() {
            while (true) {
                if (autoAdvanceSeasons) {
                    AdvanceSeason();
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

        public void AdvanceSeason() {
            currentSeason = (Season)(((int)currentSeason + 1) % 4);
            onSeasonChange?.Invoke(currentSeason);
        }

        public Vector3Int WorldToGrid(Vector3 position) => groundTilemap.WorldToCell(position);
        public Vector3 GridToWorld(Vector3Int position) => groundTilemap.CellToWorld(position);

        public void InstantiateEntity(Vector3Int position, GameObject prefab) {
            if (cells.TryGetValue(position, out var cell)) {
                var instance = Instantiate(prefab, cell.worldPosition, Quaternion.identity, entitiesContainer);
                cell.entities.Add(instance);
            }
        }

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

        public IEnumerable<Vector3Int> GetNeighboringPositions(Vector3Int position) {
            var neighbors = (position.y & 1) == 1
                ? oddNeighbors
                : evenNeighbors;
            return neighbors.Select(offset => offset + position);
        }

        public IEnumerable<ICell> GetNeighboringCells(Vector3Int position) {
            foreach (var neighbor in GetNeighboringPositions(position)) {
                if (TryGetCell(neighbor, out var cell)) {
                    yield return cell;
                }
            }
        }

        public void MoveEntity(Vector3Int oldPosition, Vector3Int newPosition, GameObject entity) {
            if (!cells.TryGetValue(oldPosition, out var oldCell)) {
                Debug.LogWarning($"Position {oldPosition} is out of bounds");
                return;
            }
            if (!cells.TryGetValue(newPosition, out var newCell)) {
                Debug.LogWarning($"Position {oldPosition} is out of bounds");
                return;
            }
            oldCell.entities.Remove(entity);
            newCell.entities.Add(entity);
            entity.transform.position = newCell.worldPosition;
        }
    }
}