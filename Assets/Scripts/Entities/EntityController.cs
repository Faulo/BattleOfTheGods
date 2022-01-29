using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Entities {
    public class EntityController : MonoBehaviour {

        public EntityData data { get; private set; }

        public void Init(EntityData data) {
            this.data = data;
        }
    }
}