using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Runtime.UI {


    
    public class EnergyTextUI : MonoBehaviour {
        TextMeshProUGUI text;

        private void OnEnable() {
            text = GetComponent<TextMeshProUGUI>();
        }
        void Update() {
            string t = $"{GameManager.instance.player.energy}/{GameManager.instance.player.maxEnergy}";
            text.text = t;
        }
    }
}