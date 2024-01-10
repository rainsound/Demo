using BaseGameCharacter.RPG;
using BaseGameData.RPG;
using BaseGameUI.RPG;
using UnityEngine;

namespace BaseGameController.RPG
{
    public class RPGController : IController
    {
        #region Constant
        public static readonly string HORIZONTAL = "Horizontal";
        public static readonly string VERTICAL = "Vertical";
        private readonly Vector3 cameraStartPos = new Vector3(62.7f, 27.3f, 24.5f);
        private readonly Quaternion cameraStartRot = new Quaternion(0.2f, 0.0f, 0.0f, 1.0f);
        #endregion

        public RPGDataModel dataModel;

        public CharacterActionController characterController;

        public RPGUIPanelView panelView;

        public Transform cameraTrans;

        public Transform characterRoot;

        public Transform[] bornTrans;

        public bool isCurrentModel;

        private GameObject village;

        public RPGController(RPGReference _reference)
        {
            dataModel = new RPGDataModel();
            characterController = new CharacterActionController();
            cameraTrans = GameController.Instance.cameraTrans;
            characterRoot = _reference.characterRoot;
            bornTrans = _reference.bornTrans;
            village = _reference.village; 

            isCurrentModel = false;
        }

        public void DoInit()
        {
            dataModel.DoInit();
            characterController.DoInit();
            characterController.SetCameraTrans(cameraTrans);
            panelView = GameController.Instance.uiController.GetTargetUIView<RPGUIPanelView>();

            village.SetActive(false);
        }

        public void DoUpdate()
        {
            if (!isCurrentModel)
                return;

            characterController.DoUpdate();
        }

        public void DoLateUpdate()
        {
            if (!isCurrentModel)
                return;

            characterController.DoLateUpdate();
        }

        public ControllerType GetControllerType()
        {
            return ControllerType.RPG;
        }

        public void SwitchToThisController()
        {
            village.SetActive(true);
            cameraTrans.position = cameraStartPos;
            cameraTrans.rotation = cameraStartRot;
            isCurrentModel = true;
        }

        public void CloseThisController()
        {
            village.SetActive(false);
            isCurrentModel = false;
        }

        public void StartGame()
        {
            dataModel.ResetGameData();
            dataModel.DeleteSaveFile();
            characterController.InitStartGameCharacter();
        }

        public void LoadGame()
        {
            dataModel.LoadGameData();
            characterController.InitStartGameCharacter();
        }

        public void ResetScene()
        {
            characterController.DestroyAllCharacter();
            cameraTrans.position = cameraStartPos;
            cameraTrans.rotation = cameraStartRot;
        }

        public void SetUIAction(CharacterType _type, ICharacter _character)
        {
            panelView.SetTargetAction(_type, _character);
        }

        public void DataChangeUI(DataType _type, int _num)
        {
            panelView.ChangeBaseUIInfo((int)_type, _num);
        }
    }

    public enum CharacterType
    {
        Player,
        NPC,
        Chest,
        Enemy,
    }

    public enum CharacterState
    {
        Idel,
        Move,
        Rotate,
        Action,
    }
}
