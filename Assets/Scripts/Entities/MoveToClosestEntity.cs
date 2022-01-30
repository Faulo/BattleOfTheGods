using System.Collections.Generic;
using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace Runtime.Entities {
    public class MoveToClosestEntity : MonoBehaviour {
        [SerializeField, Expandable]
        EntityData targetType = default;
        [SerializeField]
        int minimumDistanceToTarget = 0;
        [SerializeField]
        bool rememberPreviousEntities = false;

        bool isInitialized = false;
        IEntity entity;
        readonly HashSet<IEntity> visitedEntities = new HashSet<IEntity>();

        protected void Start() {
            isInitialized = true;
            entity = World.instance.GetEntityByEntityObject(gameObject);
        }

        protected void OnSeasonChange() {
            if (!isInitialized) {
                return;
            }

            var candidates = World.instance
                .GetEntitiesByType(targetType)
                .ToList();

            if (rememberPreviousEntities) {
                if (candidates.All(visitedEntities.Contains)) {
                    visitedEntities.Clear();
                } else {
                    foreach (var visited in visitedEntities) {
                        candidates.Remove(visited);
                    }
                }
            }

            var targetEntity = candidates
                .OrderBy(targetEntity => World.Distance(entity.gridPosition, targetEntity.gridPosition))
                .FirstOrDefault();
            if (targetEntity != null) {
                if (World.instance.TryCalculatePath(entity.gridPosition, targetEntity.gridPosition, out var path) && path.Count > minimumDistanceToTarget) {
                    // Debug.Log($"{entity}: {string.Join(" > ", path)}");
                    World.instance.MoveEntity(entity, path[0]);
                } else {
                    if (rememberPreviousEntities) {
                        visitedEntities.Add(targetEntity);
                    }
                }
            }
        }
    }
}