using System;
using System.Linq;
using Runtime.Entities;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace Runtime.Cards.CardConditions {
    [CreateAssetMenu(fileName = "IsEntityOnTile.asset", menuName = "Card Conditions/Entity Type On Tile")]
    public class EntityTypeOnTile : PlayCondition {

        [SerializeField] EntityCategories[] denyingCategories;
        [SerializeField] EntityCategories[] requiredCategories;
        public override bool Check(PlayConditionData data) {
            if (data.cell.entities.Count() <= 0)
                return true;

            return  data.cell.entities.All(e => !denyingCategories.Contains(e.type.category)) &&
                    data.cell.entities.Any(e => requiredCategories.Contains(e.type.category));
        }
    }
}