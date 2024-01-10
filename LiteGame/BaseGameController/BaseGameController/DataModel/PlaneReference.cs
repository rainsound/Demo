using UnityEngine;
using UnityEngine.UI;

namespace BaseGameData.Plane
{
    public class PlaneReference
    {
        public Transform sceneRoot;
        public Transform characterRoot;
        public Transform[] characterBornPoints;
        public Transform bulletShootRoot;

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
    }
}
