using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Runtime {
    public class WorldInput : MonoBehaviour {

        private void OnEnable() {
            GameManager.input.GameplayActionMap.Click.performed += Click_performed;
        }

        private void OnDisable() {
            GameManager.input.GameplayActionMap.Click.performed -= Click_performed;
        }

        private void Click_performed(InputAction.CallbackContext obj) {
        }

    }
}