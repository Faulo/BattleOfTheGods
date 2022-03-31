using UnityEngine;
using UnityEngine.UI;
namespace Runtime.UI {
    [RequireComponent(typeof(Button))]
    public class EndTurnButton : MonoBehaviour {
        Button btn;
        void OnEnable() {
            btn = GetComponent<Button>();
            btn.onClick.AddListener(EndTurnClicked);
        }

        void EndTurnClicked() => GameManager.instance.SetTurnEvaluate();

        void OnDisable() {
            btn.onClick.RemoveAllListeners();
        }
        void Update() {
            btn.interactable = GameManager.instance.state == GameManager.States.PlayingCardsIdle;
        }
    }
}