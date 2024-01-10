using BaseGameController;
using BaseGameData;
using UnityEngine;
using UnityEngine.UI;

namespace BaseGameUI
{
    public class UIController
    {
        private CanvasGroup canvasMainMenu;

        private Button gamePlane;

        private Button gameRPG;

        private CanvasGroup[] canvasFuncUI;

        private IGameUI[] gameUIViews;

        private GameController controller;

        public UIController(GameReference _reference)
        {
            canvasMainMenu = _reference.canvasMainMenu;
            gamePlane = _reference.gamePlane;
            gameRPG = _reference.gameRPG;
            canvasFuncUI = _reference.canvasFuncUI;

            gameUIViews = new IGameUI[2];
            gameUIViews[0] = new Plane.PlaneUIPanelView(_reference.planeReference);
            gameUIViews[1] = new RPG.RPGUIPanelView(_reference.rpgReference);
        }

        public void DoInit()
        {
            controller = GameController.Instance;
            gamePlane.onClick.AddListener(() => ChangeToTarget(ControllerType.Plane));
            gameRPG.onClick.AddListener(() => ChangeToTarget(ControllerType.RPG));

            for (int i = 0; i < gameUIViews.Length; i++)
            {
                gameUIViews[i].DoInit();
            }
        }

        public void DoUpdate()
        {
            for (int i = 0; i < gameUIViews.Length; i++)
            {
                gameUIViews[i].DoUpdate();
            }
        }

        public T GetTargetUIView<T>() where T : IGameUI
        {
            for (int i = 0; i < gameUIViews.Length; i++)
            {
                if (gameUIViews[i] is T)
                    return (T)gameUIViews[i];
            }
            return default;
        }

        public void ChangePanelActive(CanvasGroup _panel, bool _isActive)
        {
            float targetValue = _isActive ? 1 : 0;
            if (_panel.alpha != targetValue)
            {
                _panel.alpha = targetValue;
                _panel.blocksRaycasts = _isActive;
                _panel.interactable = _isActive;
            }
        }

        private void ChangeToTarget(ControllerType _type)
        {
            ChangePanelActive(canvasMainMenu, false);
            for (int i = 0; i < canvasFuncUI.Length; i++)
            {
                if (i == (int)_type)
                {
                    ChangePanelActive(canvasFuncUI[i], true);
                }
                else
                {
                    ChangePanelActive(canvasFuncUI[i], false);
                }
            }
            controller.ChangeToTargetController(_type);
        }

        public void BackToMainMenu()
        {
            for (int i = 0; i < canvasFuncUI.Length; i++)
            {
                ChangePanelActive(canvasFuncUI[i], false);
            }
            ChangePanelActive(canvasMainMenu, true);
            controller.CloseAllController();
        }
    }
}
