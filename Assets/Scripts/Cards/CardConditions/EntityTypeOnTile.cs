using System;
using System.Linq;
using Runtime.Entities;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace Runtime.Cards.CardConditions {
    [CreateAssetMenu(fileName = "IsEntityOnTile.asset", menuName = "Card Conditions/Entity On Tile")]
    public class EntityTypeOnTile : PlayCondition {

        [SerializeField] EntityCategories[] denyingCategories;

        public override bool Check(PlayConditionData data) {
            return !data.cell.entities.Any(e => denyingCategories.Contains(e.type.category));
        }
    }
}