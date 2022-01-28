using System;
using System.Collections;
using System.Collections.Generic;
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

        readonly Dictionary<Vector3Int, ICell> cells = new Dictionary<Vector3Int, ICell>();

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

        public bool TryGetCell(Vector3 worldPosition, out ICell cell)
            => cells.TryGetValue(groundTilemap.WorldToCell(worldPosition), out cell);
        public bool TryGetCell(Vector3Int gridPosition, out ICell cell)
            => cells.TryGetValue(gridPosition, out cell);

        public void AdvanceSeason() {
            currentSeason = (Season)(((int)currentSeason + 1) % 4);
            onSeasonChange?.Invoke(currentSeason);
        }

        public Vector3Int WorldToGrid(Vector3 position) => groundTilemap.WorldToCell(position);
        public Vector3 GridToWorld(Vector3Int position) => groundTilemap.CellToWorld(position);

        public void InstantiateEntity(Vector3Int position, GameObject prefab) {
            if (TryGetCell(position, out var cell)) {
                var instance = Instantiate(prefab, cell.worldPosition, Quaternion.identity, entitiesContainer);
                cell.entities.Add(instance);
            }
        }
        public IEnumerable<ICell> GetNeighboringCells(Vector3Int position) {
            foreach (var cell in cells.Values) {
                if ((cell.gridPosition - position).sqrMagnitude == 1) {
                    yield return cell;
                }
            }
        }
        public void MoveEntity(Vector3Int oldPosition, Vector3Int newPosition, GameObject entity) {
            if (!TryGetCell(oldPosition, out var oldCell)) {
                Debug.LogWarning($"Position {oldPosition} is out of bounds");
                return;
            }
            if (!TryGetCell(newPosition, out var newCell)) {
                Debug.LogWarning($"Position {oldPosition} is out of bounds");
                return;
            }
            oldCell.entities.Remove(entity);
            newCell.entities.Add(entity);
            entity.transform.position = newCell.worldPosition;
        }
    }
}