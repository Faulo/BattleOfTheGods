using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime {
    [CreateAssetMenu(fileName = "Scenario.asset", menuName = "Scenario")]
    public class Scenario : ScriptableObject {
        public Wave[] waves;


    }
}