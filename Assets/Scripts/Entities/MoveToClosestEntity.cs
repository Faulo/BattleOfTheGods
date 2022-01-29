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
                if (World.instance.TryCalculatePath(entity.gridPosition, targetEntity.gridPosition, out var path)) {
                    // Debug.Log($"{entity}: {string.Join(" > ", path)}");
                    World.instance.MoveEntity(entity, path[0]);
                }
            }
        }
    }
}