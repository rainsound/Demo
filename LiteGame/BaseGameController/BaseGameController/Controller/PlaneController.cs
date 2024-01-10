using UnityEngine;
using BaseGameCharacter.Plane;
using BaseGameData.Plane;
using BaseGameUI.Plane;

namespace BaseGameController.Plane
{
    public class PlaneController : IController
    {
        #region Constant
        public static readonly string HORIZONTAL = "Horizontal";
        public static readonly string VERTICAL = "Vertical";
        private readonly Vector3 CAMERA_STARTPOS = new Vector3(0, 0, -10);
        private readonly Quaternion CAMERA_STARTROT = Quaternion.identity;
        #endregion

        public PlaneDataModel dataModel;

        public CharacterActionController characterController;

        public PlaneUIPanelView panelView;

        public Transform cameraTrans;

        public Transform sceneRoot;

        public Transform characterRoot;

        public Transform[] characterBornPoints;

        public Transform bulletShootRoot;

        public IObjectPool bulletPool;

        public bool isCurrentModel;

        private bool isGameStart;

        public PlaneController(PlaneReference _reference)
        {
            dataModel = new PlaneDataModel();
            characterController = new CharacterActionController();
            bulletPool = new BulletPool();
            cameraTrans = GameController.Instance.cameraTrans;
            sceneRoot = _reference.sceneRoot;
            characterRoot = _reference.characterRoot;
            characterBornPoints = _reference.characterBornPoints;
            bulletShootRoot = _reference.bulletShootRoot;

            isCurrentModel = false;
        }

        public void DoInit()
        {
            characterController.DoInit();
            panelView = GameController.Instance.uiController.GetTargetUIView<PlaneUIPanelView>();
        }

        public void DoUpdate()
        {
            if (!isCurrentModel)
                return;

            if (isGameStart)
            {
                SceneRotate();
                characterController.DoUpdate();
                bulletPool.DoUpdate();
            }
        }

        public void DoLateUpdate()
        {

        }

        public ControllerType GetControllerType()
        {
            return ControllerType.Plane;
        }

        public void SwitchToThisController()
        {
            cameraTrans.position = CAMERA_STARTPOS;
            cameraTrans.rotation = CAMERA_STARTROT;
            isCurrentModel = true;
        }

        public void CloseThisController()
        {
            isCurrentModel = false;
        }

        public void StartGame()
        {
            dataModel.ResetPlayerData();
            characterController.InitStartGameCharacter();
            isGameStart = true;
        }

        public void GameOver(bool _result)
        {
            isGameStart = false;
            characterController.DestroyAllCharacter();
            bulletPool.ClearPool();
            panelView.OpenResultPanel(_result);
        }

        private void SceneRotate()
        {
            sceneRoot.RotateAround(cameraTrans.position, Vector3.left, 10.0f * Time.deltaTime);
            cameraTrans.Rotate(Vector3.left, 10.0f * Time.deltaTime);
        }

        public void CutPlayerHp()
        {
            bool isDie = !dataModel.CutPlayerHp();
            panelView.SetPlayerHpNum();
            if (isDie)
            {
                GameOver(false);
            }
        }

        public int AddPlayerFireLv()
        {
            bool isAdd = dataModel.AddPlayerFireLv(out int fireLv);
            if (isAdd)
            {
                panelView.SetPlayerFireLvNum(fireLv);
            }
            AddPlayerScore(50);
            return fireLv;
        }

        public void AddPlayerScore(int _addNum)
        {
            bool isWin = dataModel.AddPlayerScore(_addNum, out int currentScore);
            panelView.SetPlayerScoreNum(currentScore);

            if (isWin)
            {
                GameOver(true);
            }
        }

        public bool CheckHitCharacter(TeamType _type, Vector3 _pos)
        {
            return characterController.CheckCharactertOnHit(_type, _pos);
        }

        public void RecycleEnemy(Enemy _enemy)
        {
            characterController.EnemyDieRecycle(_enemy);
        }

        public void GetBullet(TeamType _type, Vector3 _bornPos, Vector3 _direction)
        {
            int typeInt = (int)_type;
            Bullet playerBullet = bulletPool.GetObjectFromPool<Bullet>(typeInt);
            playerBullet.baseTrans.parent = bulletShootRoot;
            playerBullet.baseTrans.position = _bornPos;
            playerBullet.baseTrans.localRotation = Quaternion.identity;
            playerBullet.SetMoveDirection(_direction);
        }

        public void RecycleBullet(Bullet _bullet)
        {
            bulletPool.RecycleObject(_bullet);
        }
    }

    public enum TeamType
    {
        Player,
        Enemy,
        Boost,
    }
}
