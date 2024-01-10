using BaseGameCharacter.RPG;
using BaseGameController;
using BaseGameController.RPG;
using BaseGameData.RPG;
using UnityEngine;
using UnityEngine.UI;

namespace BaseGameUI.RPG
{
    public partial class RPGUIPanelView : IGameUI
    {
        public CanvasGroup startPanel;

        public CanvasGroup normalPanel;

        public CanvasGroup battlePanel;

        private RPGController controller;

        private UIController baseUIController;

        private CharacterActionController characterController;

        private RPGDataModel dataModel;

        private ICharacter target;

        public RPGUIPanelView(RPGReference _reference)
        {
            startPanel = _reference.startPanel;
            normalPanel = _reference.normalPanel;
            battlePanel = _reference.battlePanel;
            startGameBtn = _reference.startGameBtn;
            loadGameBtn = _reference.loadGameBtn;
            quitGameBtn = _reference.quitGameBtn;
            baseInfos = _reference.baseInfos;
            useSmallPotion = _reference.useSmallPotion;
            useBigPotion = _reference.useBigPotion;
            skipDayBtn = _reference.skipDayBtn;
            actionBtnText = _reference.actionBtnText;
            actionBtn = _reference.actionBtn;
            TalkPanel = _reference.TalkPanel;
            exitTalkBtn = _reference.exitTalkBtn;
            talkInfos = _reference.talkInfos;
            shopPanel = _reference.shopPanel;
            buySmallPotionBtn = _reference.buySmallPotionBtn;
            buyBigPotionBtn = _reference.buyBigPotionBtn;
            closeShopBtn = _reference.closeShopBtn;
            rewardPanel = _reference.rewardPanel;
            rewardTexts = _reference.rewardTexts;
            closeRewardBtn = _reference.closeRewardBtn;
            battleReady = _reference.battleReady;
            infoTexts = _reference.infoTexts;
            battleBtn = _reference.battleBtn;
            runBtn = _reference.runBtn;
            battleResult = _reference.battleResult;
            resultText = _reference.resultText;
            gotItems = _reference.gotItems;
            closeBtn = _reference.closeBtn;
        }

        public void DoInit()
        {
            controller = GameController.Instance.GetTargetController<RPGController>();
            baseUIController = GameController.Instance.uiController;
            dataModel = controller.dataModel;
            characterController = controller.characterController;

            PanelActiveInit();
            InitStartPanel();
            InitNormalPanel();
            InitBattlePanel();
        }

        public void DoUpdate()
        {
            if (!controller.isCurrentModel)
                return;

            TriggerAction();
        }

        private void PanelActiveInit()
        {
            baseUIController.ChangePanelActive(startPanel, true);
            baseUIController.ChangePanelActive(normalPanel, false);
            baseUIController.ChangePanelActive(battlePanel, false);
        }

        private void ChangeButtonActive(Button _button, bool _isActive)
        {
            if (_button.gameObject.activeSelf != _isActive)
            {
                _button.gameObject.SetActive(_isActive);
            }
        }

        public void SetTargetAction(CharacterType _type, ICharacter _character)
        {
            target = _character;
            switch (_type)
            {
                case CharacterType.NPC:
                    OpenTalkPanel();
                    break;
                case CharacterType.Chest:
                    OpenRewwardPanel();
                    break;
                case CharacterType.Enemy:
                    OpenBattleReadyPanel();
                    break;
                default:
                    target = null;
                    break;
            }
        }

        private void ResetState()
        {
            characterController.ResetCharacterState();
            target = null;
        }
    }
}
