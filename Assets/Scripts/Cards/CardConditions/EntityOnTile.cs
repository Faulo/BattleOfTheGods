using System;
using System.Linq;
using Runtime.Entities;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace Runtime.Cards.CardConditions {
    [CreateAssetMenu(fileName = "IsEntityOnTile.asset", menuName = "Card Conditions/Entity On Tile")]
    public class EntityOnTile : PlayCondition {
        [SerializeField, Expandable]
        EntityData[] requiredEntities = Array.Empty<EntityData>();
        public override bool Check(PlayConditionData data) {
            if (requiredEntities.Length <= 0)
                return false;

            return requiredEntities.Length <= 0 || requiredEntities.All(e => data.cell.entities.Any(entityOnCell => e == entityOnCell.type));
        }
    }
}