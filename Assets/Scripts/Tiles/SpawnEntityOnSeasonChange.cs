using UnityEngine;

namespace Runtime.Tiles {
    public class SpawnEntityOnSeasonChange : MonoBehaviour {
        [SerializeField]
        GameObject entityPrefab = default;
        [SerializeField]
        Season season = Season.Spring;

        protected void OnEnable() {
            World.instance.onSeasonChange += HandleSeasonChange;
        }
        protected void OnDisable() {
            World.instance.onSeasonChange -= HandleSeasonChange;
        }
        void HandleSeasonChange(Season season) {
            if (season != this.season) {
                return;
            }
            var position = World.instance.WorldToGrid(transform.position);
            World.instance.InstantiateEntity(position, entityPrefab);
        }
    }
}