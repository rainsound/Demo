using BaseGameCharacter.RPG;
using BaseGameData.RPG;
using UnityEngine;
using UnityEngine.UI;

namespace BaseGameUI.RPG
{
    public partial class RPGUIPanelView
    {
        #region Constant
        private readonly string BATTLE_WIN = "战斗胜利";
        private readonly string BATTLE_LOSE = "战斗失败";
        #endregion

        public CanvasGroup battleReady;

        public Text[] infoTexts;

        public Button battleBtn;

        public Button runBtn;

        public CanvasGroup battleResult;

        public Text resultText;

        public Text[] gotItems;

        public Button closeBtn;

        private EnemyData enemyData;

        private bool result;

        private void InitBattlePanel()
        {
            result = false;
            baseUIController.ChangePanelActive(battleReady, false);
            baseUIController.ChangePanelActive(battleResult, false);
            battleBtn.onClick.AddListener(BattleAction);
            runBtn.onClick.AddListener(RunAction);
            closeBtn.onClick.AddListener(CloseResultPanel);
        }

        private void OpenBattleReadyPanel()
        {
            if (target != null && target is Enemy)
            {
                enemyData = ((Enemy)target).GetEnemyData();
                baseUIController.ChangePanelActive(battlePanel, true);
                baseUIController.ChangePanelActive(battleResult, false);

                infoTexts[0].text = dataModel.GetPlayerData(DataType.Lv).ToString();
                infoTexts[1].text = dataModel.GetPlayerData(DataType.Hp).ToString();
                infoTexts[2].text = dataModel.GetPlayerData(DataType.Atk).ToString();
                infoTexts[3].text = dataModel.GetPlayerData(DataType.Def).ToString();
                infoTexts[4].text = dataModel.GetPlayerData(DataType.AtkSpeed).ToString();

                infoTexts[5].text = enemyData.enemyLv.ToString();
                infoTexts[6].text = enemyData.enemyHp.ToString();
                infoTexts[7].text = enemyData.enemyAtk.ToString();
                infoTexts[8].text = enemyData.enemyDef.ToString();
                infoTexts[9].text = enemyData.enemyAtkSpeed.ToString();

                baseUIController.ChangePanelActive(battleReady, true);
            }
            else
            {
                ResetState();
            }
        }

        private void BattleAction()
        {
            baseUIController.ChangePanelActive(battleReady, false);
            result = dataModel.DealPlayerBattleData(enemyData);
            OpenBattleResultPanel(result);
        }

        private void RunAction()
        {
            baseUIController.ChangePanelActive(battleReady, false);
            baseUIController.ChangePanelActive(battlePanel, false);
            ResetState();
        }

        private void OpenBattleResultPanel(bool _result)
        {
            resultText.text = _result ? BATTLE_WIN : BATTLE_LOSE;
            float addExp = _result ? enemyData.enemyLv * 50 : 0;
            float addGold = _result ? enemyData.enemyLv * 50 : 0;
            gotItems[0].text = addExp.ToString();
            gotItems[1].text = addGold.ToString();

            baseUIController.ChangePanelActive(battleResult, true);
        }

        private void CloseResultPanel()
        {
            if (result)
            {
                baseUIController.ChangePanelActive(battleResult, false);
                baseUIController.ChangePanelActive(battlePanel, false);
                characterController.CharacterRecycle(target);
                ResetState();
            }
            else
            {
                controller.ResetScene();
                baseUIController.ChangePanelActive(normalPanel, false);
                baseUIController.ChangePanelActive(battlePanel, false);
                ChangeLoadButtonState();
                baseUIController.ChangePanelActive(startPanel, true);
            }
        }
    }
}