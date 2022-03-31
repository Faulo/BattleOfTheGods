using System.Collections.Generic;
using UnityEngine;

namespace Runtime {
    public class ActionPreviewer : MonoBehaviour {
        List<ActionPreviewController> activePreviews;
        [SerializeField] ActionPreviewController actionPreviewPrefab;

        void OnEnable() {
            GameManager.playPhaseStarted += GameManager_playPhaseStarted;
            GameManager.playPhaseEnded += GameManager_playPhaseEnded;
        }
        void OnDisable() {
            Cleanup();

            GameManager.playPhaseStarted -= GameManager_playPhaseStarted;
            GameManager.playPhaseEnded -= GameManager_playPhaseEnded;
        }

        void Cleanup() {
            if (activePreviews != null) {
                foreach (var p in activePreviews) {
                    if (p != null) {
                        Destroy(p.gameObject);
                    }
                }
                activePreviews.Clear();
            }
        }

        void GameManager_playPhaseEnded() {
            Cleanup();

        }

        void GameManager_playPhaseStarted() {
            if (activePreviews == null) {
                activePreviews = new List<ActionPreviewController>();
            }

            Cleanup();

            if (GameManager.instance.waveManager.currentIndex <
                GameManager.instance.waveManager.scenario.waves.Length) {

                var wave = GameManager.instance.waveManager.scenario.waves[GameManager.instance.waveManager.currentIndex];

                foreach (var tp in wave.cardsWithTarget) {
                    var inst = Instantiate(actionPreviewPrefab);
                    inst.Init(tp);
                    activePreviews.Add(inst);
                }
            }
        }
    }
}