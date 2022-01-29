using UnityEngine;
using Runtime.Entities;
namespace Runtime.Cards.CardEffects {
    [CreateAssetMenu(fileName = "SummonEntity.asset", menuName = "Card Effects/Summon Entity")]
    public class SummonEntity : CardEffect {
        [SerializeField] EntityData entity;
        public override void OnPlay(CardEffectData data) {
            World.instance.InstantiateEntity(data.cell.gridPosition, entity);
        }
    }
}