namespace BaseGameController
{
    public interface IController
    {
        void DoInit();

        void DoUpdate();

        void DoLateUpdate();

        ControllerType GetControllerType();

        void SwitchToThisController();

        void CloseThisController();
    }

    public enum ControllerType
    {
        Plane,
        RPG,
    }
}
