using UnityEngine;
using UnityEngine.UI;

namespace BaseGameData
{
    public class GameReference
    {
        public Transform cameraTrans;

        public CanvasGroup canvasMainMenu;

        public Button gamePlane;

        public Button gameRPG;

        public CanvasGroup[] canvasFuncUI;

        public Plane.PlaneReference planeReference;

        public RPG.RPGReference rpgReference;
    }
}
