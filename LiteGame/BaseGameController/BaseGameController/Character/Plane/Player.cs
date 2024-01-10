using BaseGameController;
using BaseGameController.Plane;
using UnityEngine;

namespace BaseGameCharacter.Plane
{
    public class Player : ICharacter
    {
        #region Constant
        private readonly string[] FIREPOINT_NAMES = new string[] { "FirePointA", "FirePointB", "FirePointC" };
        private readonly float PLAYER_TOP_LIMIT = 19.5f;
        private readonly float PLAYER_DOWN_LIMIT = -19.5f;
        private readonly float PLAYER_LEFT_LIMIT = -34.2f;
        private readonly float PLAYER_RIGHT_LIMIT = 34.2f;
        private readonly Quaternion BASE_LOCALROTATION = new Quaternion(-0.7f, 0.0f, 0.0f, 0.7f);
        private readonly float CHECKHIT_DIS = 1.0f;
        private readonly float CHECKBOOSTHIT_DIS = 1.5f;
        #endregion

        private Transform[] firePoints;

        private PlaneController controller;

        private GameObject baseObj;

        private Transform baseTrans;

        public int fireLv;

        public float shootInterval;

        private float shootWaitingTime;

        private float hitedPerformanceTime;

        private float hitedCoolDown;

        public Player(GameObject _obj)
        {
            baseObj = _obj;
            baseTrans = _obj.transform;
            firePoints = new Transform[3];
            firePoints[0] = baseTrans.Find(FIREPOINT_NAMES[0]);
            firePoints[1] = baseTrans.Find(FIREPOINT_NAMES[1]);
            firePoints[2] = baseTrans.Find(FIREPOINT_NAMES[2]);

            controller = GameController.Instance.GetTargetController<PlaneController>();
        }

        public void DoUpdate()
        {
            PlayerMove();
            PlayerAttack();
            PlayerHitedPerformance();
        }

        public void SetBornData(Vector3 _bornPoint)
        {
            baseTrans.position = _bornPoint;
            baseTrans.localRotation = BASE_LOCALROTATION;
            fireLv = controller.dataModel.GetPlayerFireLv();
            shootInterval = 0.2f;
            shootWaitingTime = 0f;
            hitedPerformanceTime = 3.0f;
            hitedCoolDown = 0;
        }

        public TeamType GetTeamType()
        {
            return TeamType.Player;
        }

        public GameObject GetCharacterObject()
        {
            return baseObj;
        }

        public bool CheckHit(TeamType _type, Vector3 _pos)
        {
            bool isHit = false;
            switch (_type)
            {
                case TeamType.Enemy:
                    if (hitedCoolDown <= 0)
                    {
                        isHit = Vector3.Distance(baseTrans.position, _pos) <= CHECKHIT_DIS;
                    }
                    break;
                case TeamType.Boost:
                    isHit = Vector3.Distance(baseTrans.position, _pos) <= CHECKBOOSTHIT_DIS;
                    break;
                default:
                    break;
            }
            if (isHit)
            {
                OnHit(_type);
            }
            return isHit;
        }

        private void OnHit(TeamType _type)
        {
            switch (_type)
            {
                case TeamType.Enemy:
                    controller.CutPlayerHp();
                    hitedCoolDown = hitedPerformanceTime;
                    break;
                case TeamType.Boost:
                    fireLv = controller.AddPlayerFireLv();
                    break;
                default:
                    break;
            }
        }

        public void DieAction()
        {

        }

        private void PlayerMove()
        {
            float localX = baseTrans.localPosition.x;
            float localY = baseTrans.localPosition.y;
            float horizontal = Input.GetAxis(PlaneController.HORIZONTAL) * Time.deltaTime * 30.0f;
            float vertical = Input.GetAxis(PlaneController.VERTICAL) * Time.deltaTime * 30.0f;
            if ((localX <= PLAYER_LEFT_LIMIT && horizontal < 0) || (localX >= PLAYER_RIGHT_LIMIT && horizontal > 0))
            {
                horizontal = 0;
            }
            if ((localY <= PLAYER_DOWN_LIMIT && vertical < 0) || (localY >= PLAYER_TOP_LIMIT && vertical > 0))
            {
                vertical = 0;
            }
            baseTrans.Translate(new Vector3(horizontal, 0, vertical));
        }

        private void PlayerAttack()
        {
            if (shootWaitingTime >= shootInterval)
            {
                int bulletCount = 0;
                int startIndex = fireLv - 1;
                startIndex = startIndex >= firePoints.Length - 1 ? 0 : startIndex;
                for (int i = startIndex; i < firePoints.Length; i++)
                {
                    FireBullet(firePoints[i].position);
                    bulletCount++;
                    if (bulletCount >= fireLv)
                    {
                        break;
                    }
                }
                shootWaitingTime = 0;
            }
            shootWaitingTime += Time.deltaTime;
        }

        private void PlayerHitedPerformance()
        {
            if (hitedCoolDown > 0)
            {
                hitedCoolDown -= Time.deltaTime;
            }
        }

        private void FireBullet(Vector3 _bornPosition)
        {
            controller.GetBullet(TeamType.Player, _bornPosition, Vector3.up);
        }
    }
}
