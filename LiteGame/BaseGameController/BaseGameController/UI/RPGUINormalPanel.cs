using BaseGameCharacter.RPG;
using BaseGameController.RPG;
using BaseGameData.RPG;
using UnityEngine;
using UnityEngine.UI;

namespace BaseGameUI.RPG
{
    public partial class RPGUIPanelView
    {
        #region Constant
        private readonly string[] BUTTON_STRING = new string[] { "对话", "打开", "战斗" };
        private readonly string REWARD_ATK_STRING = "获得 攻击力 +";
        private readonly string REWARD_DEF_STRING = "获得 防御力 +";
        private readonly string REWARD_GOLD_STRING = "获得 金币 +";
        #endregion

        public Text[] baseInfos;

        public Button useSmallPotion;

        public Button useBigPotion;

        public Button skipDayBtn;

        public Text actionBtnText;

        public Button actionBtn;

        public CanvasGroup TalkPanel;

        public Button exitTalkBtn;

        public Text[] talkInfos;

        public CanvasGroup shopPanel;

        public Button buySmallPotionBtn;

        public Button buyBigPotionBtn;

        public Button closeShopBtn;

        public CanvasGroup rewardPanel;

        public Text[] rewardTexts;

        public Button closeRewardBtn;

        private void InitNormalPanel()
        {
            actionBtn.onClick.AddListener(characterController.ExecuteAction);
            useSmallPotion.onClick.AddListener(() => dataModel.PlayerUsePotion(DataType.SmallPotion));
            useBigPotion.onClick.AddListener(() => dataModel.PlayerUsePotion(DataType.BigPotion));
            skipDayBtn.onClick.AddListener(SkipDayAction);
            buySmallPotionBtn.onClick.AddListener(() => dataModel.PlayerBuyItem(DataType.SmallPotion));
            buyBigPotionBtn.onClick.AddListener(() => dataModel.PlayerBuyItem(DataType.BigPotion));
            exitTalkBtn.onClick.AddListener(CloseTalkPanel);
            closeShopBtn.onClick.AddListener(CloseShopPanel);
            closeRewardBtn.onClick.AddListener(CloseRewardPanel);
        }

        private void OpenNormalPanel()
        {
            baseUIController.ChangePanelActive(normalPanel, true);
            ChangeButtonActive(actionBtn, false);
            for (int i = 0; i < baseInfos.Length; i++)
            {
                ChangeBaseUIInfo(i, dataModel.GetPlayerData((DataType)i));
            }
            baseUIController.ChangePanelActive(TalkPanel, false);
            baseUIController.ChangePanelActive(shopPanel, false);
            baseUIController.ChangePanelActive(rewardPanel, false);
        }

        public void ChangeBaseUIInfo(int _type, int _value)
        {
            baseInfos[_type].text = _value.ToString();
        }

        private void TriggerAction()
        {
            if (CheckCharacterDistance() && Input.GetKeyDown(KeyCode.E))
            {
                characterController.ExecuteAction();
            }
        }

        private bool CheckCharacterDistance()
        {
            if (characterController.CheckCharacterDistance(out CharacterType type))
            {
                actionBtnText.text = BUTTON_STRING[(int)type - 1];
                ChangeButtonActive(actionBtn, true);
                return true;
            }
            ChangeButtonActive(actionBtn, false);
            return false;
        }

        private void SkipDayAction()
        {
            dataModel.PlayerAddData(DataType.Hp, dataModel.GetPlayerData(DataType.MaxHp));
            dataModel.SaveGameData();
            dataModel.PlayerAddData(DataType.Day, 1);
            characterController.RefreshRandomCharacter();
        }

        private void OpenTalkPanel()
        {
            if (target != null && target is NPC)
            {
                NPC npc = (NPC)target;
                talkInfos[0].text = npc.talkInfo[0];
                talkInfos[1].text = npc.talkInfo[1];
                baseUIController.ChangePanelActive(TalkPanel, true);
                if (npc.isShopNPC)
                {
                    baseUIController.ChangePanelActive(shopPanel, true);
                }
            }
            else
            {
                ResetState();
            }
        }

        private void CloseTalkPanel()
        {
            baseUIController.ChangePanelActive(TalkPanel, false);
            ResetState();
        }

        private void CloseShopPanel()
        {
            baseUIController.ChangePanelActive(shopPanel, false);
            baseUIController.ChangePanelActive(TalkPanel, false);
            ResetState();
        }

        private void OpenRewwardPanel()
        {
            if (target != null && target is Chest)
            {
                int type = Random.Range(0, 100);
                int value = Random.Range(4, 7);
                if (type < 20)
                {
                    rewardTexts[0].text = REWARD_ATK_STRING;
                    
                    dataModel.PlayerAddData(DataType.Atk, value);
                }
                else if (type < 40)
                {
                    rewardTexts[0].text = REWARD_DEF_STRING;
                    dataModel.PlayerAddData(DataType.Def, value);
                }
                else
                {
                    value *= 100;
                    rewardTexts[0].text = REWARD_GOLD_STRING;
                    dataModel.PlayerAddData(DataType.Gold, value);
                }
                rewardTexts[1].text = value.ToString();
                baseUIController.ChangePanelActive(rewardPanel, true);
            }
            else
            {
                ResetState();
            }
        }

        private void CloseRewardPanel()
        {
            baseUIController.ChangePanelActive(rewardPanel, false);
            characterController.CharacterRecycle(target);
            ResetState();
        }
    }
}
