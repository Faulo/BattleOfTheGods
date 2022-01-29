using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace Runtime.Entities {
    public class SpreadOut : MonoBehaviour {
        protected void OnEnable() {
            World.instance.onStartSeasonChange += HandleSeasonChange;
        }
        protected void OnDisable() {
            World.instance.onStartSeasonChange -= HandleSeasonChange;
        }

        void HandleSeasonChange(Season season) {
            var position = World.instance.WorldToGrid(transform.position);
            if (World.instance.TryGetCell(position, out var cell)) {
                var cellCandidates = World.instance.GetNeighboringCells(position)
                    .Where(neighbor => neighbor.entities.Count < cell.entities.Count)
                    .ToList();

                if (cellCandidates.Count > 0) {
                    int minCount = cellCandidates.Min(cell => cell.entities.Count);
                    var neighbor = cellCandidates.Where(cell => cell.entities.Count == minCount).RandomElement();
                    World.instance.MoveEntity(position, neighbor.gridPosition, gameObject);
                }
            }
        }
    }
}