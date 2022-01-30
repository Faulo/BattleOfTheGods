using UnityEngine;

namespace Runtime.Entities {
    [CreateAssetMenu(fileName = "EntityData.asset")]
    public class EntityData : ScriptableObject {
        [SerializeField]
        public GameObject prefab = default;

        [SerializeField]
        EntityCategories _category;
        public EntityCategories category => _category;

        [Header("Combat")]
        [SerializeField]
        public Faction faction = Faction.Nobody;
        [SerializeField]
        public bool canParticipateInCombat = false;
        [SerializeField]
        public bool canStartCombat = false;
        [SerializeField, Range(0, 100)]
        public int attack = 0;
        [SerializeField, Range(0, 100)]
        public int health = 0;
    }
}
