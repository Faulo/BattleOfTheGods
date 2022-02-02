using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Runtime.Cards;
namespace Runtime.Entities {

    [RequireComponent(typeof(IEntity))]
    public class DestroyIfCondition : MonoBehaviour {
        [SerializeField] PlayCondition[] conditions;
        IEntity entity;
        protected void Start() {
            this.entity = World.instance.GetEntityByEntityObject(gameObject);
        }
        // Update is called once per frame
        void Update() {
            if (entity != default &&
                conditions.Any(c => c.Check(new PlayCondition.PlayConditionData(entity.ownerCell, default)))) 
                {
                Destroy(this.gameObject);
            }
        }
    }
}