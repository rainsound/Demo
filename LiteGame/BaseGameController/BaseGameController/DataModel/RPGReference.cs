using UnityEngine;
using UnityEngine.UI;

namespace BaseGameData.RPG
{
    public class RPGReference
    {
        public GameObject village;
        public Transform characterRoot;
        public Transform[] bornTrans;
        public CanvasGroup startPanel;
        public CanvasGroup normalPanel;
        public CanvasGroup battlePanel;
        public Button startGameBtn;
        public Button loadGameBtn;
        public Button quitGameBtn;
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
        public CanvasGroup battleReady;
        public Text[] infoTexts;
        public Button battleBtn;
        public Button runBtn;
        public CanvasGroup battleResult;
        public Text resultText;
        public Text[] gotItems;
        public Button closeBtn;
    }
}
