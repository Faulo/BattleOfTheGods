using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime {
    public class ActionPreviewer : MonoBehaviour {
        List<ActionPreviewController> activePreviews;
        [SerializeField] ActionPreviewController actionPreviewPrefab;

        private void OnEnable() {
            GameManager.playPhaseStarted += GameManager_playPhaseStarted;
            GameManager.playPhaseEnded += GameManager_playPhaseEnded;
        }
        private void OnDisable() {
            Cleanup();

            GameManager.playPhaseStarted -= GameManager_playPhaseStarted;
            GameManager.playPhaseEnded -= GameManager_playPhaseEnded;
        }

        private void Cleanup() {
            if (activePreviews != null) {
                foreach (var p in activePreviews) {
                    if (p != null)
                        Destroy(p.gameObject);
                }
                activePreviews.Clear();
            }
        }

        private void GameManager_playPhaseEnded() {
            Cleanup();

        }

        private void GameManager_playPhaseStarted() {
            if (activePreviews == null)
                activePreviews = new List<ActionPreviewController>();
            Cleanup();

            if (GameManager.instance.waveManager.currentIndex < 
                GameManager.instance.waveManager.scenario.waves.Length) {
               
                Wave wave = GameManager.instance.waveManager.scenario.waves[GameManager.instance.waveManager.currentIndex];

                foreach (var tp in wave.cardsWithTarget) {
                    ActionPreviewController inst = Instantiate(actionPreviewPrefab);
                    inst.Init(tp);
                    activePreviews.Add(inst);
                }
            }
        }
    }
}