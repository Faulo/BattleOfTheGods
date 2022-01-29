using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
namespace Runtime {
    public class WorldInput : MonoBehaviour {

        public Action<Vector3> clicked;
        [SerializeField] LayerMask clickable;
        Camera cam;

        private void OnEnable() {
            GameManager.input.GameplayActionMap.Click.performed += Click_performed;
            cam = FindObjectOfType<Camera>();
            if (cam == default) {
                Debug.LogError("Could not find camera, disabling world input.");
                this.gameObject.SetActive(false);
            }
        }

        private void OnDisable() {
            GameManager.input.GameplayActionMap.Click.performed -= Click_performed;
        }

        private void Click_performed(InputAction.CallbackContext obj) 
        {
            Vector2 clickPos = Mouse.current.position.ReadValue();
            Ray r = cam.ScreenPointToRay(clickPos);

            if (Physics.Raycast(r, out RaycastHit hitInfo, Mathf.Infinity, clickable)) {
                clicked?.Invoke(hitInfo.point);
                Debug.Log($"clicked {hitInfo.point}");
            }
        }

    }
}