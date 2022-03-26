using UnityEngine;
using Match3Game.System;
using System;
using UnityEngine.SceneManagement;

namespace Match3Game.UI
{
    public class MainMenuController : MonoBehaviour
    {
        public void PlayLevel(LevelData data)
        {
            if (data == null)
                throw new Exception("MainMenuController: the PlayLevel was called with null.");

            GameController.level = data;
            SceneManager.LoadScene("Game");
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }
    }
}
