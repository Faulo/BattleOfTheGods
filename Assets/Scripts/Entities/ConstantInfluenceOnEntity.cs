using UnityEngine;

namespace Runtime.Entities {
    public class ConstantInfluenceOnEntity : MonoBehaviour {
        [SerializeField]
        int influencePerTurn = 1;

        bool isInitialized = false;
        IEntity entity;

        protected void Start() {
            isInitialized = true;
            entity = World.instance.GetEntityByEntityObject(gameObject);
        }

        protected void OnAwardInfluence() {
            if (!isInitialized) {
                return;
            }

            World.instance.AddInfluence(entity.gridPosition, influencePerTurn);
        }
    }
}