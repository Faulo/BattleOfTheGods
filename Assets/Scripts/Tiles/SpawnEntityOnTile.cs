using Runtime.Entities;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace Runtime.Tiles {
    public class SpawnEntityOnTile : MonoBehaviour {
        [SerializeField, Expandable]
        EntityData entityType = default;
        [SerializeField]
        Season season = Season.Spring;

        bool isInitialized = false;
        ITile tile;

        protected void Start() {
            isInitialized = true;
            tile = World.instance.GetTileByTileObject(gameObject);
        }

        protected void OnSeasonChange() {
            if (!isInitialized) {
                return;
            }

            if (World.instance.season != season) {
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