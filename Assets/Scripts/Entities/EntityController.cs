using Slothsoft.UnityExtensions;
using UnityEngine;

namespace Runtime.Entities {
    public class EntityController : MonoBehaviour {
        [SerializeField, Expandable]
        public EntityData type = default;
    }
}