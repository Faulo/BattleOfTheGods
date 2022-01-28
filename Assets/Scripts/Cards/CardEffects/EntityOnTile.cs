using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Runtime.Entities;
namespace Runtime.Cards.CardEffects {
    public class EntityOnTile : PlayCondition {

        [SerializeField] List<EntityData> denyingEntites;

        public override bool Check(PlayConditionData data) 
        {
            //if any entity in denying entities is on target tile return false.

            return true;
        }
    }
}