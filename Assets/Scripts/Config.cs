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

        public static Config current;
    }
}