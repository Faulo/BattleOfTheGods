using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Runtime.Cards;
using System;
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