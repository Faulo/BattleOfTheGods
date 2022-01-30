using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
namespace Runtime {
public class UIController : MonoBehaviour
{
        public static UIController instance;


        public static GameObject gameOverPanel => instance._gameOverPanel;
        [SerializeField] GameObject _gameOverPanel;

        public static TextMeshProUGUI gameOverMessage => instance._gameOverMessage;
        [SerializeField] TextMeshProUGUI _gameOverMessage;

        public static TextMeshProUGUI debugText => instance._debugText;
        [SerializeField] TextMeshProUGUI _debugText;

        public static TextMeshProUGUI log => instance._log;
        [SerializeField] TextMeshProUGUI _log;
        private void Awake() {
            instance = this;
            gameOverPanel.SetActive(false);
        }

        public void GoToMainMenu() {
            SceneManager.LoadScene("MainMenu");
        }
    }
}