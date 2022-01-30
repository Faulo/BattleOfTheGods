using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime {
public class TargetingView : MonoBehaviour
{
        [SerializeField] Light mainLight;
        [SerializeField] GameObject highlightPrefab;
        List<GameObject> lights;
        float ogIntensity;
        private void OnEnable() {
            ogIntensity = mainLight.intensity;
            GameManager.targetingPhaseStarted += GameManager_targetingPhaseStarted;
            GameManager.targetingPhaseEnded += GameManager_targetingPhaseEnded;
        }
        private void OnDisable() {
            Cleanup();
            GameManager.targetingPhaseStarted -= GameManager_targetingPhaseStarted;
            GameManager.targetingPhaseEnded -= GameManager_targetingPhaseEnded;
        }
        private void GameManager_targetingPhaseEnded() {
            Cleanup();
        }

        

        private void GameManager_targetingPhaseStarted() {
            if (lights == null)
                lights = new List<GameObject>();
            mainLight.intensity = ogIntensity * .3f;
            foreach (var cell in GameManager.instance.currentLegalCells) {
                var inst = Instantiate(highlightPrefab, this.transform);
                inst.transform.position = cell.worldPosition;
                lights.Add(inst);
            }
        }

        private void Cleanup() {
            if (lights != null) {
                foreach (var i in lights)
                    Destroy(i.gameObject);
                lights.Clear();
            }
            mainLight.intensity = ogIntensity;
        }
    }
}