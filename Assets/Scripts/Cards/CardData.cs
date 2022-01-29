using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Runtime.Entities;

namespace Runtime.Cards {
    [CreateAssetMenu(fileName = "CardData.asset")]
    public class CardData : ScriptableObject
    {
        public List<PlayCondition> conditions;
        public List<CardEffect> effects;

        public string cardBody => _cardBody;
        [TextArea] [SerializeField] string _cardBody;

        public EntityData assignedEntity => _assignedEntity;
        [SerializeField] EntityData _assignedEntity;

        public Sprite sprite => _sprite;
        [SerializeField] Sprite _sprite;

        public int cost => _cost;
        [SerializeField] int _cost;

        public Faction type => _type;
        [SerializeField] Faction _type;

    }
}