using UnityEngine;
using UnityEngine.UI;

public class RPGReferenceObject : MonoBehaviour
{
    [Header("GmaeObject")]
    public GameObject village;

    [Header("Transforms")]
    public Transform characterRoot;
    public Transform[] bornTrans;

    [Header("UI/MainPanel")]
    public CanvasGroup startPanel;
    public CanvasGroup normalPanel;
    public CanvasGroup battlePanel;

    [Header("UI/StartPanel")]
    public Button startGameBtn;
    public Button loadGameBtn;
    public Button quitGameBtn;

    [Header("UI/NormalPanel")]
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

    [Header("UI/BattlePanel")]
    public CanvasGroup battleReady;
    public Text[] infoTexts;
    public Button battleBtn;
    public Button runBtn;
    public CanvasGroup battleResult;
    public Text resultText;
    public Text[] gotItems;
    public Button closeBtn;
}
