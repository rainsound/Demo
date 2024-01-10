using BaseGameController;
using BaseGameController.Plane;
using UnityEngine;

namespace BaseGameCharacter.Plane
{
    public class Bullet : PoolObject
    {
        #region Constant
        private readonly float BULLET_DISAPPEAR_TIME = 1.5f;
        private readonly float ENEMYBULLET_DISAPPEAR_TIME = 5f;
        private readonly float BOOST_DISAPPEAR_TIME = 3f;
        private readonly float PLAYER_BULLET_SPEED = 30.0f;
        private readonly float ENEMY_BULLET_SPEED = 10.0f;
        private readonly float BOOST_BULLET_SPEED = 8.0f;
        #endregion

        private PlaneController controller;

        private Vector3 moveDirection = Vector3.zero;

        public Bullet(GameObject _obj) : base(_obj) { }

        public override void DoCreate()
        {
            base.DoCreate();
            controller = GameController.Instance.GetTargetController<PlaneController>();
            ResetCountTime(GetBulletDisappearTime());
        }

        public override void DoUpdate()
        {
            CheckHit();
            baseTrans.Translate(GetCurrentSpeed() * moveDirection);
        }

        public override void DoRecycle()
        {
            base.DoRecycle();
            ResetCountTime(GetBulletDisappearTime());
            moveDirection = Vector3.zero;
        }

        public void SetMoveDirection(Vector3 _direction)
        {
            moveDirection = _direction;
        }

        private float GetCurrentSpeed()
        {
            switch ((TeamType)type)
            {
                case TeamType.Player:
                    return PLAYER_BULLET_SPEED * Time.deltaTime;
                case TeamType.Enemy:
                    return ENEMY_BULLET_SPEED * Time.deltaTime;
                case TeamType.Boost:
                    return BOOST_BULLET_SPEED * Time.deltaTime;
                default:
                    return PLAYER_BULLET_SPEED * Time.deltaTime;
            }
        }

        private float GetBulletDisappearTime()
        {
            switch ((TeamType)type)
            {
                case TeamType.Player:
                    return BULLET_DISAPPEAR_TIME;
                case TeamType.Enemy:
                    return ENEMYBULLET_DISAPPEAR_TIME;
                case TeamType.Boost:
                    return BOOST_DISAPPEAR_TIME;
                default:
                    return BULLET_DISAPPEAR_TIME;
            }
        }

        private void CheckHit()
        {
            bool isHit = controller.CheckHitCharacter((TeamType)type, baseTrans.position);
            if (isHit)
            {
                controller.RecycleBullet(this);
            }
        }
    }
}
