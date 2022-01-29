using System.Linq;
using UnityEngine;

namespace Runtime.Entities {
    public class CullOverpopulation : MonoBehaviour {
        [SerializeField, Range(0, 10)]
        int cullingThreshold = 2;

        IEntity entity;

        protected void OnEnable() {
            World.onStartSeasonChange += HandleSeasonChange;
        }
        protected void OnDisable() {
            World.onStartSeasonChange -= HandleSeasonChange;
        }
        protected void Start() {
            entity = World.instance.GetEntityByEntityObject(gameObject);
        }

        void HandleSeasonChange(Season season) {
            int count = entity.ownerCell.entities.Count();
            if (count >= cullingThreshold) {
                World.instance.DestroyEntity(entity);
            }
        }
    }
}