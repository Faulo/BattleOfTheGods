using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime {
    [CreateAssetMenu(fileName = "Config.asset")]
    public class Config : ScriptableObject {

        public float energyIncreasePerTurn => _energyIncreasePerTurn;
        [SerializeField] float _energyIncreasePerTurn = 1;

        public float maxEnergy => _maxEnergy;
        [SerializeField] float _maxEnergy = 10;

        public int defaultEnergy => _defaultEnergy;
        [SerializeField] int _defaultEnergy = 3;

        public int openingHandSize => _openingHandSize;
        [SerializeField] int _openingHandSize = 4;

        public int maxAbsoluteInfluenceValue => _maxAbsoluteInfluenceValue;
        [SerializeField] int _maxAbsoluteInfluenceValue = 15;

        public List<Cards.CardData> defaultDeck;

        public static Config current;
    }
}