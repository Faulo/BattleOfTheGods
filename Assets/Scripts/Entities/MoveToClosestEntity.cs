using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace Runtime.Entities {
    public class MoveToClosestEntity : MonoBehaviour {
        [SerializeField, Expandable]
        EntityData targetType = default;

        IEntity entity;

        protected void Start() {
            entity = World.instance.GetEntityByEntityObject(gameObject);
        }

        protected void OnSeasonChange() {
            var targetEntity = World.instance
                .GetEntitiesByType(targetType)
                .OrderBy(targetEntity => World.Distance(entity.gridPosition, targetEntity.gridPosition))
                .FirstOrDefault();
            if (targetEntity != null) {
                foreach (var position in World.instance.CalculatePath(entity.gridPosition, targetEntity.gridPosition)) {
                    World.instance.MoveEntity(entity, position);
                    break;
                }
            }
        }
    }
}