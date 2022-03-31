using TMPro;
using UnityEngine;
namespace Runtime.UI {



    public class EnergyTextUI : MonoBehaviour {
        TextMeshProUGUI text;

        void OnEnable() {
            text = GetComponent<TextMeshProUGUI>();
        }
        void Update() {
            string t = $"{GameManager.instance.player.energy}/{GameManager.instance.player.maxEnergy}";
            text.text = t;
        }
    }
}