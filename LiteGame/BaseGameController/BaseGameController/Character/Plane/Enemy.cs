using BaseGameController;
using BaseGameController.Plane;
using UnityEngine;

namespace BaseGameCharacter.Plane
{
    public class Enemy : PoolObject, ICharacter
    {
        #region Constant
        private readonly string[] FIREPOINT_NAMES = new string[] { "FirePointB", "FirePointC" };
        private readonly float PLANE_DISAPPEAR_TIME = 12.0f;
        private readonly Quaternion BASE_LOCALROTATION = new Quaternion(0.0f, 0.7f, -0.7f, 0.0f);
        private readonly float CHECKHIT_DIS = 2.3f;
        #endregion

        private Transform[] firePoints;

        private PlaneController controller;

        private float planeSpeed;

        private Vector3 planeDirection;

        private int hp;

        private float shootInterval;

        private float shootWaitingTime;

        private int dropItemProbability = 10;

        public Enemy(GameObject _obj) : base(_obj)
        {
            firePoints = new Transform[2];
            firePoints[0] = baseTrans.Find(FIREPOINT_NAMES[0]);
            firePoints[1] = baseTrans.Find(FIREPOINT_NAMES[1]);

            controller = GameController.Instance.GetTargetController<PlaneController>();
        }

        public override void DoCreate()
        {
            base.DoCreate();
            ResetCountTime(PLANE_DISAPPEAR_TIME);
        }

        public override void DoUpdate()
        {
            CheckPlaneHit();
            EnemyMove();
            EnemyAttack();
        }

        public override void DoRecycle()
        {
            base.DoRecycle();
            ResetCountTime(PLANE_DISAPPEAR_TIME);
        }

        public void SetBornData(Vector3 _bornPoint)
        {
            hp = 5;
            shootInterval = 2.5f;
            shootWaitingTime = 2.3f;
            baseTrans.position = _bornPoint;
            baseTrans.localRotation = BASE_LOCALROTATION;
            planeSpeed = 5.0f + Random.Range(0, 2.5f);
            planeDirection = GetMoveDirection();
        }

        public TeamType GetTeamType()
        {
            return TeamType.Enemy;
        }

        public GameObject GetCharacterObject()
        {
            return baseObj;
        }

        public bool CheckHit(TeamType _type, Vector3 _pos)
        {
            bool isHit = Vector3.Distance(baseTrans.position, _pos) <= CHECKHIT_DIS;
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
                case TeamType.Player:
                    hp--;
                    if (hp <= 0)
                    {
                        controller.RecycleEnemy(this);
                        return;
                    }
                    break;
                default:
                    break;
            }
        }

        public void DieAction()
        {
            CheckItemDrop();
            controller.AddPlayerScore(100);
        }

        private Vector3 GetMoveDirection()
        {
            Vector3 dir = Vector3.forward;
            float leftFactor = Random.Range(-0.5f, 0.5f);
            if (Mathf.Abs(leftFactor) <= 0.25f)
            {
                dir += Vector3.right * leftFactor;
            }
            return dir.normalized;
        }

        private void EnemyMove()
        {
            baseTrans.Translate(planeSpeed * Time.deltaTime * planeDirection);
        }

        private void EnemyAttack()
        {
            if (shootWaitingTime >= shootInterval)
            {
                for (int i = 0; i < firePoints.Length; i++)
                {
                    FireBullet(firePoints[i].position);
                }
                shootWaitingTime = 0;
            }
            shootWaitingTime += Time.deltaTime;
        }

        private void FireBullet(Vector3 _bornPosition)
        {
            controller.GetBullet(TeamType.Enemy, _bornPosition, Vector3.down);
        }

        private void CheckItemDrop()
        {
            if (Random.Range(0, 100) < dropItemProbability)
            {
                controller.GetBullet(TeamType.Boost, baseTrans.position, Vector3.down);
            }
        }

        private void CheckPlaneHit()
        {
            controller.CheckHitCharacter(TeamType.Enemy, baseTrans.position);
        }
    }
}
