using System;
using System.Linq;
using Runtime.Entities;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace Runtime.Cards.CardConditions {
    [CreateAssetMenu(fileName = "IsEntityOnTile.asset", menuName = "Card Conditions/Entity On Tile")]
    public class EntityOnTile : PlayCondition {
        [SerializeField, Expandable]
        EntityData[] denyingEntites = Array.Empty<EntityData>();

        public override bool Check(PlayConditionData data) {
            //if any entity in denying entities is on target tile return false.
            foreach (var entity in World.instance.GetEntitiesByCell(data.cell)) {
                if (denyingEntites.Contains(entity.type)) {
                    return false;
                }
            }

            return true;
        }
    }
}