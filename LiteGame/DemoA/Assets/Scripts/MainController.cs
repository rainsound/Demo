using BaseGameController;
using BaseGameData.Plane;
using BaseGameData.RPG;
using BaseGameData;
using UnityEngine;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    public Transform cameraTrans;

    public CanvasGroup canvasMainMenu;

    public Button gamePlane;

    public Button gameRPG;

    public CanvasGroup[] canvasFuncUI;

    public PlaneReferenceObject planeReferenceObj;

    public RPGReferenceObject rpgReferenceObj;

    private GameController controller;

    private void Awake()
    {
        PlaneReference planeReference = InitPlaneReference();
        RPGReference rpgReference = InitRPGReference();
        GameReference reference = new GameReference()
        {
            cameraTrans = cameraTrans,
            canvasMainMenu = canvasMainMenu,
            gamePlane = gamePlane,
            gameRPG = gameRPG,
            canvasFuncUI = canvasFuncUI,
            planeReference = planeReference,
            rpgReference = rpgReference,
        };
        controller = new GameController(reference);
        controller.DoInit();
    }

    private void Update()
    {
        controller.DoUpdate();
    }

    private void LateUpdate()
    {
        controller.DoLateUpdate();
    }

    private PlaneReference InitPlaneReference()
    {
        PlaneReference reference = new PlaneReference()
        {
            sceneRoot = planeReferenceObj.sceneRoot,
            returnBtn = planeReferenceObj.returnBtn,
            loseDes = planeReferenceObj.loseDes,
            winDes = planeReferenceObj.winDes,
            resultPanel = planeReferenceObj.resultPanel,
            playerScoreNum = planeReferenceObj.playerScoreNum,
            playerFireLvNum = planeReferenceObj.playerFireLvNum,
            restartBtn = planeReferenceObj.restartBtn,
            playerHPNum = planeReferenceObj.playerHPNum,
            quitBtn = planeReferenceObj.quitBtn,
            startBtn = planeReferenceObj.startBtn,
            mainMenu = planeReferenceObj.mainMenu,
            bulletShootRoot = planeReferenceObj.bulletShootRoot,
            characterBornPoints = planeReferenceObj.characterBornPoints,
            characterRoot = planeReferenceObj.characterRoot,
            battlePanel = planeReferenceObj.battlePanel,
        };
        return reference;
    }

    private RPGReference InitRPGReference()
    {
        RPGReference reference = new RPGReference()
        {
            village = rpgReferenceObj.village,
            gotItems = rpgReferenceObj.gotItems,
            resultText = rpgReferenceObj.resultText,
            battleResult = rpgReferenceObj.battleResult,
            runBtn = rpgReferenceObj.runBtn,
            battleBtn = rpgReferenceObj.battleBtn,
            infoTexts = rpgReferenceObj.infoTexts,
            battleReady = rpgReferenceObj.battleReady,
            closeRewardBtn = rpgReferenceObj.closeRewardBtn,
            rewardTexts = rpgReferenceObj.rewardTexts,
            rewardPanel = rpgReferenceObj.rewardPanel,
            closeShopBtn = rpgReferenceObj.closeShopBtn,
            buyBigPotionBtn = rpgReferenceObj.buyBigPotionBtn,
            buySmallPotionBtn = rpgReferenceObj.buySmallPotionBtn,
            shopPanel = rpgReferenceObj.shopPanel,
            talkInfos = rpgReferenceObj.talkInfos,
            exitTalkBtn = rpgReferenceObj.exitTalkBtn,
            TalkPanel = rpgReferenceObj.TalkPanel,
            characterRoot = rpgReferenceObj.characterRoot,
            bornTrans = rpgReferenceObj.bornTrans,
            startPanel = rpgReferenceObj.startPanel,
            normalPanel = rpgReferenceObj.normalPanel,
            battlePanel = rpgReferenceObj.battlePanel,
            startGameBtn = rpgReferenceObj.startGameBtn,
            closeBtn = rpgReferenceObj.closeBtn,
            loadGameBtn = rpgReferenceObj.loadGameBtn,
            baseInfos = rpgReferenceObj.baseInfos,
            useSmallPotion = rpgReferenceObj.useSmallPotion,
            useBigPotion = rpgReferenceObj.useBigPotion,
            skipDayBtn = rpgReferenceObj.skipDayBtn,
            actionBtnText = rpgReferenceObj.actionBtnText,
            actionBtn = rpgReferenceObj.actionBtn,
            quitGameBtn = rpgReferenceObj.quitGameBtn,
        };
        return reference;
    }
}
