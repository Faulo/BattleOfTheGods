using UnityEngine;

namespace Runtime.Entities {
    public class ConstantInfluenceOnEntity : MonoBehaviour {
        [SerializeField]
        int influencePerTurn = 1;
        [SerializeField]
        int range = 1;
        [SerializeField]
        int[] influenceByRange;
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
            if (range > 0) {
                foreach (var pos in World.GetInDistance(entity.gridPosition, range, true)) {
                    World.instance.AddInfluence(pos, influencePerTurn);
                }
            }

            for (int i = 0; i < influenceByRange.Length; i++) {
                foreach(var pos in World.GetRing(entity.gridPosition, i+1)) {
                    World.instance.AddInfluence(pos, influenceByRange[i]);
                }
            }
        }
    }
}