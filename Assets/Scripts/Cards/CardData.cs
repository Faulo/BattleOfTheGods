using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Cards {
    [CreateAssetMenu(fileName = "CardData.asset")]
    public class CardData : ScriptableObject
    {
        public List<PlayCondition> conditions;
        public List<CardEffect> effects;
        public int cost => _cost;
        [SerializeField] int _cost;

        public CardTypes type => _type;
        [SerializeField] CardTypes _type;

    }
}