using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime {
    public class WaveManager : MonoBehaviour {
        public Scenario scenario;
        public int currentIndex;
        public Transform cards => CardManager.waveParent;
    }
}