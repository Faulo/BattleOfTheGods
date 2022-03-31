using System;
using System.Collections.Generic;
using Runtime.Cards;
using UnityEngine;
namespace Runtime {
    [CreateAssetMenu(fileName = "Wave.asset", menuName = "Wave")]
    public class Wave : ScriptableObject {

        public List<CardTargetTuple> cardsWithTarget;

    }

    [Serializable]
    public class CardTargetTuple {
        public Vector3Int target;
        public CardData card;
    }
}