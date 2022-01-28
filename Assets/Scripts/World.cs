using System.Collections.Generic;
using Runtime.Extensions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

namespace Runtime {
    public class World : MonoBehaviour {
        [SerializeField]
        Grid grid = default;
        [SerializeField]
        Tilemap groundTilemap = default;
        [SerializeField]
        Transform entitiesContainer = default;

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

        void Start() {
            SetUpCells();
        }
    }
}