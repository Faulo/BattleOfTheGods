using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Runtime.UI {
    [RequireComponent(typeof(Button))]
    public class EndTurnButton : MonoBehaviour {
        Button btn;
        private void OnEnable() {
            btn = GetComponent<Button>();
            btn.onClick.AddListener(EndTurnClicked);
        }

        private void EndTurnClicked() => GameManager.instance.SetTurnEvaluate();

        private void OnDisable() {
            btn.onClick.RemoveAllListeners();
        }
        void Update() {
            btn.interactable = GameManager.instance.state == GameManager.States.PlayingCardsIdle;
        }
    }
}