using System.Linq;
using UnityEngine;

namespace Runtime.Entities {
    public class CullOverpopulation : MonoBehaviour {
        [SerializeField, Range(0, 10)]
        int cullingThreshold = 2;

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
            if (count >= cullingThreshold) {
                World.instance.DestroyEntity(entity);
            }
        }
    }
}