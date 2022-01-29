using System;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Runtime {
    public class WorldInput : MonoBehaviour {
        public static Action<Vector3> onSelect;
        public static Action<Vector3> onClick;

        [SerializeField]
        LayerMask clickable;

        Camera cam;

        protected void OnEnable() {
            GameManager.input.GameplayActionMap.Move.performed += Move_performed;
            GameManager.input.GameplayActionMap.Click.performed += Click_performed;
            cam = FindObjectOfType<Camera>();
            if (cam == default) {
                Debug.LogError("Could not find camera, disabling world input.");
                gameObject.SetActive(false);
            }
        }

        protected void OnDisable() {
            GameManager.input.GameplayActionMap.Move.performed -= Move_performed;
            GameManager.input.GameplayActionMap.Click.performed -= Click_performed;
        }

        void Move_performed(InputAction.CallbackContext obj) {
            if (TryFindPoint(out var position)) {
                onSelect?.Invoke(position);
            }
        }

        void Click_performed(InputAction.CallbackContext obj) {
            if (TryFindPoint(out var position)) {
                onClick?.Invoke(position);
            }
        }

        bool TryFindPoint(out Vector3 position) {
            var clickPos = Mouse.current.position.ReadValue();
            var ray = cam.ScreenPointToRay(clickPos);
            if (Physics.Raycast(ray, out var info, Mathf.Infinity, clickable)) {
                position = info.point;
                return true;
            }
            position = default;
            return false;
        }
    }
}