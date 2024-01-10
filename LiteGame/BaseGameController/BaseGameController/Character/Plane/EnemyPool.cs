using BaseGameController.Plane;
using System.Collections.Generic;
using UnityEngine;

namespace BaseGameCharacter.Plane
{
    public class EnemyPool : IObjectPool
    {
        #region Constant
        private readonly string[] ENEMY_PLANE_NAME = new string[] { "Prefabs/Plane/Rafale", "Prefabs/Plane/CruiseFighter" };
        private readonly int[] POOL_LENGTH = new int[] { 8, 8 };
        private readonly Vector3 DISABLE_PLACE = new Vector3(3000, 3000, 3000);
        #endregion

        private PoolObject[] enemys;

        private Transform enemyPlanePoolRoot;

        private List<PoolObject> waitDestroyEnemys;

        public EnemyPool()
        {
            enemyPlanePoolRoot = new GameObject("EnemyPlanePoolRoot").transform;
            enemyPlanePoolRoot.position = DISABLE_PLACE;
            int enemyLength = 0;
            for (int i = 0; i < POOL_LENGTH.Length; i++)
            {
                enemyLength += POOL_LENGTH[i];
            }
            enemys = new Enemy[enemyLength];
            waitDestroyEnemys = new List<PoolObject>();
        }

        public void DoUpdate()
        {
            if (enemys != null && enemys.Length > 0)
            {
                for (int i = 0; i < enemys.Length; i++)
                {
                    AutoRecycleObject(enemys[i]);
                }
            }

            if (waitDestroyEnemys != null && waitDestroyEnemys.Count > 0)
            {
                for (int i = 0; i < waitDestroyEnemys.Count; i++)
                {
                    AutoRecycleObject(waitDestroyEnemys[i], () => i--);
                }
            }
        }

        private Enemy CreateEnemy(int _type, bool _setUse, bool _needDestory = false)
        {
            GameObject enemyObj = Object.Instantiate(Resources.Load(ENEMY_PLANE_NAME[_type]) as GameObject, enemyPlanePoolRoot);
            Enemy enemy = new Enemy(enemyObj);
            enemy.type = _type;
            enemy.DoCreate();
            enemy.isUse = _setUse;
            enemy.needDestory = _needDestory;
            enemy.baseTrans.position = DISABLE_PLACE;
            if (_needDestory)
            {
                waitDestroyEnemys.Add(enemy);
            }
            return enemy;
        }

        private void DestroyEnemy(PoolObject _enemy)
        {
            if (_enemy is Enemy)
            {
                waitDestroyEnemys.Remove(_enemy);
                _enemy.DoDestroy();
                Object.Destroy(_enemy.baseObj);
            }
        }

        private PoolObject getTargetEnemy(int _type)
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
                if (enemys[i] != null && !enemys[i].isUse)
                {
                    target = enemys[i];
                    enemys[i].isUse = true;
                    break;
                }

                if (enemys[i] == null)
                {
                    enemys[i] = CreateEnemy(_type, true);
                    target = enemys[i];
                    break;
                }
            }
            return target;
        }

        public T GetObjectFromPool<T>(int _type) where T : PoolObject
        {
            PoolObject target = getTargetEnemy(_type);

            if (target == null)
            {
                target = CreateEnemy(_type, true, true);
            }

            if (target != null)
            {
                target.baseObj.SetActive(true);
            }

            return (T)target;
        }

        public void RecycleObject<T>(T _target) where T : PoolObject
        {
            if (_target is Enemy)
            {
                if (_target.needDestory)
                {
                    DestroyEnemy(_target);
                }
                else
                {
                    _target.baseObj.SetActive(false);
                    _target.baseTrans.parent = enemyPlanePoolRoot;
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
            for (int i = 0; i < enemys.Length; i++)
            {
                if (enemys[i] != null)
                {
                    Object.Destroy(enemys[i].baseObj);
                    enemys[i] = null;
                }
            }

            for (int i = 0; i < waitDestroyEnemys.Count; i++)
            {
                Object.Destroy(waitDestroyEnemys[i].baseObj);
            }
            waitDestroyEnemys.Clear();
        }

        public bool EnemyCheckHit(Vector3 _pos)
        {
            bool isHit = false;
            for (int i = 0; i < enemys.Length; i++)
            {
                if (enemys[i] != null && enemys[i].isUse)
                {
                    isHit = ((Enemy)enemys[i]).CheckHit(TeamType.Player, _pos);
                    if (isHit)
                        break;
                }
            }
            return isHit;
        }
    }
}
