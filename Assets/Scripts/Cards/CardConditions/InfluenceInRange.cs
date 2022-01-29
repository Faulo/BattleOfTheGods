using System;
using System.Linq;
using Runtime.Entities;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace Runtime.Cards.CardConditions {
    [CreateAssetMenu(fileName = "IsInfluenceInRange.asset", menuName = "Card Conditions/Influence In Range")]
    public class InfluenceInRange : PlayCondition {

        [SerializeField] int influenceAmount;
        public override bool Check(PlayConditionData data) {
            //Todo: Get all tiles in range
            return true;
        }
    }
}