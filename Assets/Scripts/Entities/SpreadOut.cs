using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace Runtime.Entities {
    public class SpreadOut : MonoBehaviour {
        bool isInitialized = false;
        IEntity entity;

        protected void Start() {
            isInitialized = true;
            entity = World.instance.GetEntityByEntityObject(gameObject);
        }

        protected void OnSeasonChange() {
            if (!isInitialized) {
                return;
            }

            int count = entity.ownerCell.entities.Count();
            var cellCandidates = World.instance.GetNeighboringCells(entity.ownerCell)
                .Where(neighbor => neighbor.entities.Count() < count)
                .ToList();

            if (cellCandidates.Count > 0) {
                int minCount = cellCandidates.Min(cell => cell.entities.Count());
                var neighbor = cellCandidates.Where(cell => cell.entities.Count() == minCount).RandomElement();
                World.instance.MoveEntity(entity, neighbor.gridPosition);
            }
        }
    }
}