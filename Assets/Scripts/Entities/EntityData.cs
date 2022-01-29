using UnityEngine;

namespace Runtime.Entities {
    [CreateAssetMenu(fileName = "EntityData.asset")]
    public class EntityData : ScriptableObject {
        [SerializeField]
        public GameObject prefab = default;
    }
}
