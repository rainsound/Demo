using BaseGameController;
using BaseGameController.Plane;
using BaseGameData.Plane;
using UnityEngine;
using UnityEngine.UI;

namespace BaseGameUI.Plane
{
    public class PlaneUIPanelView : IGameUI
    {
        #region Constant
        private readonly string ZEROSTRING = "0";
        #endregion

        public CanvasGroup mainMenu;
        public Button startBtn;
        public Button quitBtn;

        public CanvasGroup battlePanel;
        public Text playerHPNum;
        public Text playerFireLvNum;
        public Text playerScoreNum;

        public CanvasGroup resultPanel;
        public Text winDes;
        public Text loseDes;
        public Button returnBtn;
        public Button restartBtn;

        private PlaneController controller;
        private UIController baseUIController;
        private PlaneDataModel dataModel;

        public PlaneUIPanelView(PlaneReference _reference)
        {
            mainMenu = _reference.mainMenu;
            startBtn = _reference.startBtn;
            quitBtn = _reference.quitBtn;
            battlePanel = _reference.battlePanel;
            playerHPNum = _reference.playerHPNum;
            playerFireLvNum = _reference.playerFireLvNum;
            playerScoreNum = _reference.playerScoreNum;
            resultPanel = _reference.resultPanel;
            winDes = _reference.winDes;
            loseDes = _reference.loseDes;
            returnBtn = _reference.returnBtn;
            restartBtn = _reference.restartBtn;
        }

        public void DoInit()
        {
            controller = GameController.Instance.GetTargetController<PlaneController>();
            baseUIController = GameController.Instance.uiController;
            dataModel = controller.dataModel;
            startBtn.onClick.AddListener(StartGameAction);
            quitBtn.onClick.AddListener(QuitGameAction);
            returnBtn.onClick.AddListener(ReturnMainAction);
            restartBtn.onClick.AddListener(GameRestartAction);
        }

        public void DoUpdate()
        {

        }

        private void StartGameAction()
        {
            controller.StartGame();
            playerHPNum.text = dataModel.GetPlayerCurrentHp().ToString();
            playerFireLvNum.text = dataModel.GetPlayerFireLv().ToString();
            playerScoreNum.text = ZEROSTRING;

            baseUIController.ChangePanelActive(mainMenu, false);

            battlePanel.alpha = 1;
        }

        private void QuitGameAction()
        {
            baseUIController.BackToMainMenu();
        }

        private void ReturnMainAction()
        {
            baseUIController.ChangePanelActive(resultPanel, false);
            baseUIController.ChangePanelActive(mainMenu, true);
        }

        private void GameRestartAction()
        {
            StartGameAction();
            baseUIController.ChangePanelActive(resultPanel, false);
        }

        public void OpenResultPanel(bool _result)
        {
            int isWin = _result ? 1 : 0;
            winDes.color = new Color(1, 1, 1, isWin);
            loseDes.color = new Color(1, 1, 1, 1 - isWin);

            battlePanel.alpha = 0;

            baseUIController.ChangePanelActive(resultPanel, true);
        }

        public void SetPlayerHpNum()
        {
            playerHPNum.text = dataModel.GetPlayerCurrentHp().ToString();
        }

        public void SetPlayerFireLvNum(int _num)
        {
            playerFireLvNum.text = _num.ToString();
        }

        public void SetPlayerScoreNum(int _num)
        {
            playerScoreNum.text = _num.ToString();
        }
    }
}
