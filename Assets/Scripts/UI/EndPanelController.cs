using System;
using UnityEngine;
using UnityEngine.UI;

namespace Match3Game.UI
{
    public class EndPanelController : MonoBehaviour
    {
        [SerializeField]
        protected Text endText;
        [SerializeField]
        protected string winText;
        [SerializeField]
        protected string loseText;

        protected void Awake()
        {
#if UNITY_EDITOR
            if (endText == null)
                throw new Exception("EndPanelController: The endText can't be null");

            if (string.IsNullOrEmpty(winText))
                throw new Exception("EndPanelController: The winText can't be null or empty");

            if (string.IsNullOrEmpty(loseText))
                throw new Exception("EndPanelController: The loseText can't be null or empty");
#endif
        }

        public void Show(bool isWin)
        {
            endText.text = isWin ? winText : loseText;
            gameObject.SetActive(true);
        }
    }
}