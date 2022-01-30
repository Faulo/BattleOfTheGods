using System;
using UnityEngine;

namespace Runtime.Entities {
    public class ConstantInfluenceOnEntity : MonoBehaviour {
        [SerializeField]
        int[] influenceOverRange = Array.Empty<int>();

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

            for (int i = 0; i < influenceOverRange.Length; i++) {
                foreach (var pos in World.GetRing(entity.gridPosition, i)) {
                    World.instance.AddInfluence(pos, influenceOverRange[i]);
                }
            }
        }
    }
}