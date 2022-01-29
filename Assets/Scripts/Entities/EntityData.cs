using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Entities {
    [CreateAssetMenu(fileName = "EntityData.asset")]
    public class EntityData : ScriptableObject
    {
        public EntityController prefab => _prefab;
        [SerializeField] EntityController _prefab;
    }
}
