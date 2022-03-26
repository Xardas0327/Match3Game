using Match3Game.Field;
using Match3Game.System;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Match3Game.UI
{
    public class GameMenuController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField]
        protected Text pointText;
        [SerializeField]
        protected Text stepText;
        [SerializeField]
        protected EndPanelController endPanelController;
        [SerializeField]
        protected GameObject lockPanel;

        [Header("Game")]
        [SerializeField]
        protected GameController gameController;

        protected bool isActiveGame;
        protected bool isLock;

        protected void Awake()
        {
#if UNITY_EDITOR
            if (pointText == null)
                throw new Exception("GameMenuController: The pointText can't be null");

            if (stepText == null)
                throw new Exception("GameMenuController: The stepText can't be null");

            if (gameController == null)
                throw new Exception("GameMenuController: The gameController can't be null");

            if (endPanelController == null)
                throw new Exception("GameMenuController: The endPanelController can't be null");

            if (lockPanel == null)
                throw new Exception("GameMenuController: The lockPanel can't be null");
#endif
            endPanelController.gameObject.SetActive(false);
            lockPanel.SetActive(false);

            gameController.RefreshedSteps += RefreshSteps;
            gameController.RefreshedPoints += RefreshPoints;
            gameController.Lose += (s, e) => ShowEndPanel(false);
            gameController.Win += (s, e) => ShowEndPanel(true);
            gameController.LockMap += (s, e) => LockMap(true);
            gameController.UnlockMap += (s, e) => LockMap(false);
        }

        protected void Start()
        {
            RefreshSteps(null, gameController.GetMaxSteps());
            RefreshPoints(null, 0);
            isActiveGame = true;
        }

        protected void Update()
        {
            CheckMouse();
        }

        protected void CheckMouse()
        {
            if (!isActiveGame || isLock)
                return;

            if (Input.GetMouseButton(0))
            {
                var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null)
                {
                    IFieldController field = hit.collider.GetComponent<IFieldController>();
                    if (field != null)
                        gameController.SelectField(field);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                gameController.DoAction();
            }
        }

        protected void RefreshSteps(object sender, int e)
        {
            stepText.text = e.ToString();
        }

        protected void RefreshPoints(object sender, int e)
        {
            pointText.text = e + "/" + gameController.GetRequirementPoints().ToString();
        }

        protected void ShowEndPanel(bool isWin)
        {
            isActiveGame = false;
            endPanelController.Show(isWin);
            LockMap(false);
        }

        protected void LockMap(bool value)
        {
            isLock = value;
            lockPanel.SetActive(value);
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene("Menu");
        }
    }

}