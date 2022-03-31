using System.Linq;
using Runtime.Entities;
using UnityEngine;

namespace Runtime.Cards.CardConditions {
    [CreateAssetMenu(fileName = "IsEntityTypeBlockingTile.asset", menuName = "Card Conditions/Entity Type Blocking Tile")]
    public class EntityTypeBlockingTile : PlayCondition {

        [SerializeField] EntityCategories[] blockingCategories;
        public override bool Check(PlayConditionData data) {
            if (data.cell.entities.Count() <= 0) {
                return true;
            }

            return !blockingCategories.Any(c => data.cell.entities.Any(e => e.type.category == c));
        }
    }
}