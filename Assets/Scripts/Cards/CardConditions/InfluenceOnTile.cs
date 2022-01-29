using System;
using System.Linq;
using Runtime.Entities;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace Runtime.Cards.CardConditions {
    [CreateAssetMenu(fileName = "IsInfluenceOnTile.asset", menuName = "Card Conditions/Influence On Tile")]
    public class InfluenceOnTile : PlayCondition {

        [SerializeField] int influenceAmount;
        public override bool Check(PlayConditionData data) {
            //todo: get influence & check´!
            return true;
        }
    }
}