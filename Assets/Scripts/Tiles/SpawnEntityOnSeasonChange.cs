using Runtime.Entities;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace Runtime.Tiles {
    public class SpawnEntityOnSeasonChange : MonoBehaviour {
        [SerializeField, Expandable]
        EntityData entityType = default;
        [SerializeField]
        Season season = Season.Spring;

        ITile tile;

        protected void OnEnable() {
            World.onStartSeasonChange += HandleSeasonChange;
        }
        protected void OnDisable() {
            World.onStartSeasonChange -= HandleSeasonChange;
        }
        protected void Start() {
            tile = World.instance.GetTileByTileObject(gameObject);
        }

        void HandleSeasonChange(Season season) {
            if (season != this.season) {
                return;
            }
            World.instance.InstantiateEntity(tile, entityType);
        }
        /*
        protected void OnDrawGizmos() {
            foreach (var position in World.instance.GetNeighboringPositions(World.instance.WorldToGrid(transform.position))) {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(World.instance.GridToWorld(position), 0.5f);
            }
        }
        //*/
    }
}