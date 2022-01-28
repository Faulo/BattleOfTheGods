using UnityEngine;

namespace Runtime.Entities {
    public class SpreadOut : MonoBehaviour {
        protected void OnEnable() {
            World.instance.onSeasonChange += HandleSeasonChange;
        }
        protected void OnDisable() {
            World.instance.onSeasonChange -= HandleSeasonChange;
        }

        void HandleSeasonChange(Season season) {
            var position = World.instance.WorldToGrid(transform.position);
            if (World.instance.TryGetCell(position, out var cell)) {
                foreach (var neighbor in World.instance.GetNeighboringCells(position)) {
                    if (neighbor.entities.Count < cell.entities.Count) {
                        World.instance.MoveEntity(position, neighbor.gridPosition, gameObject);
                        break;
                    }
                }
            }
        }
    }
}