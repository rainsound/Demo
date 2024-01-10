using BaseGameData;
using BaseGameUI;
using UnityEngine;

namespace BaseGameController
{
    public class GameController
    {
        public static GameController Instance;

        public Transform cameraTrans;

        public UIController uiController;

        public IController[] controllers;

        public GameController(GameReference _reference)
        {
            Instance = this;
            uiController = new UIController(_reference);

            cameraTrans = _reference.cameraTrans;
            controllers = new IController[2];
            controllers[0] = new Plane.PlaneController(_reference.planeReference);
            controllers[1] = new RPG.RPGController(_reference.rpgReference);
        }

        public void DoInit()
        {
            for (int i = 0; i < controllers.Length; i++)
            {
                controllers[i].DoInit();
            }
            uiController.DoInit();
        }

        public void DoUpdate()
        {
            for (int i = 0; i < controllers.Length; i++)
            {
                controllers[i].DoUpdate();
            }
            uiController.DoUpdate();
        }

        public void DoLateUpdate()
        {
            for (int i = 0; i < controllers.Length; i++)
            {
                controllers[i].DoLateUpdate();
            }
        }

        public T GetTargetController<T>() where T : IController
        {
            for (int i = 0; i < controllers.Length; i++)
            {
                if (controllers[i] is T)
                    return (T)controllers[i];
            }
            return default;
        }

        public void ChangeToTargetController(ControllerType _type)
        {
            for (int i = 0; i < controllers.Length; i++)
            {
                if (controllers[i].GetControllerType() == _type)
                    controllers[i].SwitchToThisController();
                else
                    controllers[i].CloseThisController();
            }
        }

        public void CloseAllController()
        {
            for (int i = 0; i < controllers.Length; i++)
            {
                controllers[i].CloseThisController();
            }
        }
    }
}
