using UnityEngine;
using UnityEngine.UI;

    public class PlaneReferenceObject : MonoBehaviour
    {
        [Header("Transforms")]
        public Transform sceneRoot;
        public Transform characterRoot;
        public Transform[] characterBornPoints;
        public Transform bulletShootRoot;

        [Header("UI/MainMenu")]
        public CanvasGroup mainMenu;
        public Button startBtn;
        public Button quitBtn;

        [Header("UI/BattlePanel")]
        public CanvasGroup battlePanel;
        public Text playerHPNum;
        public Text playerFireLvNum;
        public Text playerScoreNum;

        [Header("UI/ResultPanel")]
        public CanvasGroup resultPanel;
        public Text winDes;
        public Text loseDes;
        public Button returnBtn;
        public Button restartBtn;
    }
