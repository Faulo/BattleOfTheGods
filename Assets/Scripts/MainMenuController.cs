using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Runtime {
    public class MainMenuController : MonoBehaviour {
        public void StartNature() {
            SceneManager.LoadScene("Nature");
        }

        public void StartHuman() {
            SceneManager.LoadScene("Human");
        }

        public void Quit() {
            Application.Quit();
        }
    }
}