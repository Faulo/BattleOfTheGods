using UnityEngine;

namespace Runtime {
    [CreateAssetMenu(fileName = "Scenario.asset", menuName = "Scenario")]
    public class Scenario : ScriptableObject {
        public Wave[] waves;


    }
}