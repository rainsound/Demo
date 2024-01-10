using System.Collections.Generic;
using UnityEngine;

namespace BaseGameCharacter.Plane
{
    public class BulletPool : IObjectPool
    {
        #region Constant
        private readonly string[] BULLET_NAME = new string[] { "Prefabs/Item/Bullet", "Prefabs/Item/EnemyBullet", "Prefabs/Item/BoostFire" };
        private readonly int[] POOL_LENGTH = new int[] { 42, 30, 3 };
        private readonly Vector3 DISABLE_PLACE = new Vector3(3000, 3000, 3000);
        #endregion

        private PoolObject[] bullets;

        private Transform bulletPoolRoot;

        private List<PoolObject> waitDestroyBullets;

        public BulletPool()
        {
            bulletPoolRoot = new GameObject("BulletPoolRoot").transform;
            bulletPoolRoot.position = DISABLE_PLACE;
            int bulletLength = 0;
            for (int i = 0; i < POOL_LENGTH.Length; i++)
            {
                bulletLength += POOL_LENGTH[i];
            }
            bullets = new Bullet[bulletLength];
            waitDestroyBullets = new List<PoolObject>();
        }

        public void DoUpdate()
        {
            if (bullets != null && bullets.Length > 0)
            {
                for (int i = 0; i < bullets.Length; i++)
                {
                    AutoRecycleObject(bullets[i]);
                }
            }

            if (waitDestroyBullets != null && waitDestroyBullets.Count > 0)
            {
                for (int i = 0; i < waitDestroyBullets.Count; i++)
                {
                    AutoRecycleObject(waitDestroyBullets[i], () => i--);
                }
            }
        }

        private Bullet CreateBullet(int _type, bool _setUse, bool _needDestory = false)
        {
            GameObject bulletObj = Object.Instantiate(Resources.Load(BULLET_NAME[_type]) as GameObject, bulletPoolRoot);
            Bullet bullet = new Bullet(bulletObj);
            bullet.type = _type;
            bullet.DoCreate();
            bullet.isUse = _setUse;
            bullet.needDestory = _needDestory;
            bullet.baseTrans.position = DISABLE_PLACE;
            if (_needDestory)
            {
                waitDestroyBullets.Add(bullet);
            }
            return bullet;
        }

        private void DestroyBullet(PoolObject _bullet)
        {
            if (_bullet is Bullet)
            {
                waitDestroyBullets.Remove(_bullet);
                _bullet.DoDestroy();
                Object.Destroy(_bullet.baseObj);
            }
        }

        private PoolObject getTargetBullet(int _type)
        {
            PoolObject target = null;
            int startIndex = 0;
            for (int i = 0; i < _type; i++)
            {
                startIndex += POOL_LENGTH[i];
            }
            int endIndex = startIndex + POOL_LENGTH[_type];

            for (int i = startIndex; i < endIndex; i++)
            {
                if (bullets[i] != null && !bullets[i].isUse)
                {
                    target = bullets[i];
                    bullets[i].isUse = true;
                    break;
                }

                if (bullets[i] == null)
                {
                    bullets[i] = CreateBullet(_type, true);
                    target = bullets[i];
                    break;
                }
            }
            return target;
        }

        public T GetObjectFromPool<T>(int _type) where T : PoolObject
        {
            PoolObject target = getTargetBullet(_type);

            if (target == null)
            {
                target = CreateBullet(_type, true, true);
            }

            if (target != null)
            {
                target.baseObj.SetActive(true);
            }
            return (T)target;
        }

        public void RecycleObject<T>(T _target) where T : PoolObject
        {
            if (_target is Bullet)
            {
                if (_target.needDestory)
                {
                    DestroyBullet(_target);
                }
                else
                {
                    _target.baseObj.SetActive(false);
                    _target.baseTrans.parent = bulletPoolRoot;
                    _target.baseTrans.position = DISABLE_PLACE;
                    _target.isUse = false;
                    _target.DoRecycle();
                }
            }
        }

        private void AutoRecycleObject(PoolObject _target, System.Action _action = null)
        {
            if (_target == null)
                return;

            if (_target.isUse)
            {
                _target.DoUpdate();
                _target.disappearTime -= Time.deltaTime;
                if (_target.disappearTime <= 0)
                {
                    RecycleObject(_target);
                    _action?.Invoke();
                }
            }
        }

        public void ClearPool()
        {
            for (int i = 0; i < bullets.Length; i++)
            {
                if (bullets[i] != null)
                {
                    Object.Destroy(bullets[i].baseObj);
                    bullets[i] = null;
                }
            }

            for (int i = 0; i < waitDestroyBullets.Count; i++)
            {
                Object.Destroy(waitDestroyBullets[i].baseObj);
            }
            waitDestroyBullets.Clear();
        }
    }
}