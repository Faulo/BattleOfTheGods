using Slothsoft.UnityExtensions;
using UnityEngine;

namespace Runtime.Entities {
    public class SpawnEntityOnEntity : MonoBehaviour {
        [SerializeField, Expandable]
        EntityData spawnType = default;
        [SerializeField, Range(0, 10)]
        int maxSpawnDistance = 0;

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

            var targetCell = World.instance
                .GetCircularCells(entity.gridPosition, maxSpawnDistance)
                .RandomElement();

            World.instance.InstantiateEntity(targetCell, spawnType);
        }
    }
}