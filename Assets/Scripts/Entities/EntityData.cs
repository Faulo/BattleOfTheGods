using UnityEngine;

namespace Runtime.Entities {
    [CreateAssetMenu(fileName = "EntityData.asset")]
    public class EntityData : ScriptableObject {
        [SerializeField]
        public GameObject prefab = default;

        public EntityCategories category => _category;
        [SerializeField] EntityCategories _category;
    }
}
