using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Runtime.Entities;
namespace Runtime.Cards.CardConditions {
    [CreateAssetMenu(fileName = "IsEntityOnTile.asset", menuName = "Card Conditions/Entity On Tile")]
    public class EntityOnTile : PlayCondition {

        [SerializeField] List<EntityData> denyingEntites;

        public override bool Check(PlayConditionData data) 
        {
            //if any entity in denying entities is on target tile return false.
            foreach(var entity in data.cell.entities) {
                if (entity.TryGetComponent<EntityController>(out EntityController ctrl) &&
                    denyingEntites.Contains(ctrl.data)) {
                    return false;
                }
            }

            return true;
        }
    }
}