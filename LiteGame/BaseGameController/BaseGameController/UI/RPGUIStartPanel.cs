using UnityEngine;
using UnityEngine.UI;

namespace BaseGameUI.RPG
{
    public partial class RPGUIPanelView
    {
        public Button startGameBtn;

        public Button loadGameBtn;

        public Button quitGameBtn;

        public void InitStartPanel()
        {
            ChangeLoadButtonState();
            startGameBtn.onClick.AddListener(StartGameAction);
            loadGameBtn.onClick.AddListener(LoadGameAction);
            quitGameBtn.onClick.AddListener(QuitGameAction);
        }

        private void ChangeLoadButtonState()
        {
            loadGameBtn.interactable = dataModel.CheckSaveFile();
        }

        private void StartGameAction()
        {
            controller.StartGame();
            baseUIController.ChangePanelActive(startPanel, false);
            OpenNormalPanel();
        }

        private void LoadGameAction()
        {
            controller.LoadGame();
            baseUIController.ChangePanelActive(startPanel, false);
            OpenNormalPanel();
        }

        private void QuitGameAction()
        {
            baseUIController.BackToMainMenu();
        }
    }
}
