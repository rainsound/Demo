using BaseGameController;
using BaseGameController.Plane;
using UnityEngine;

namespace BaseGameCharacter.Plane
{
    public class CharacterActionController
    {
        #region Constant
        private readonly string PLAYER_PLANE_NAME = "Prefabs/Plane/Stealth_Bomber";
        private readonly int SAMETIME_MAXGENERATE_ENEMY = 3;
        #endregion

        private ICharacter player;

        private IObjectPool enemyPool;

        private Transform characterRoot;

        private Transform[] bornPoints;

        private float generateEnemyBlank = 5.0f;

        private float waitGenerateEnemy = 0;

        private int[] generateEnemyIndexCache;

        public CharacterActionController()
        {
            enemyPool = new EnemyPool();
        }

        public void DoInit()
        {
            PlaneController controller = GameController.Instance.GetTargetController<PlaneController>();
            characterRoot = controller.characterRoot;
            bornPoints = controller.characterBornPoints;
        }

        public void DoUpdate()
        {
            player?.DoUpdate();

            AutoGenerateEnemy();
            enemyPool.DoUpdate();
        }

        private void CreateCharacter(TeamType _type, Vector3 _bornPos = default)
        {
            switch (_type)
            {
                case TeamType.Player:
                    if (player == null)
                    {
                        GameObject playerObj = Object.Instantiate(Resources.Load(PLAYER_PLANE_NAME) as GameObject, characterRoot);
                        player = new Player(playerObj);
                        player.SetBornData(bornPoints[0].position);
                    }
                    break;
                case TeamType.Enemy:
                    int enemyType = Random.Range(0, 2);
                    Enemy enemy = enemyPool.GetObjectFromPool<Enemy>(enemyType);
                    enemy.baseTrans.parent = characterRoot;
                    enemy.SetBornData(_bornPos);
                    break;
                default:
                    break;
            }
        }

        public void EnemyDieRecycle(Enemy _enemy)
        {
            _enemy.DieAction();
            enemyPool.RecycleObject(_enemy);
        }

        public void DestroyAllCharacter()
        {
            if (player != null)
            {
                Object.Destroy(player.GetCharacterObject());
                player = null;
            }
            enemyPool.ClearPool();
        }

        public void InitStartGameCharacter()
        {
            DestroyAllCharacter();
            CreateCharacter(TeamType.Player);
        }

        private void ResetGenerateEnemyIndexCache()
        {
            if (generateEnemyIndexCache == null)
            {
                generateEnemyIndexCache = new int[bornPoints.Length - 1];
            }
            for (int i = 0; i < generateEnemyIndexCache.Length; i++)
            {
                generateEnemyIndexCache[i] = i + 1;
            }
        }

        private void AutoGenerateEnemy()
        {
            if (waitGenerateEnemy >= generateEnemyBlank)
            {
                int generateNum = Random.Range(1, SAMETIME_MAXGENERATE_ENEMY + 1);
                ResetGenerateEnemyIndexCache();
                for (int i = 0; i < generateNum; i++)
                {
                    int maxIndex = generateEnemyIndexCache.Length - i;
                    int index = Random.Range(0, maxIndex);
                    CreateCharacter(TeamType.Enemy, bornPoints[generateEnemyIndexCache[index]].position);
                    generateEnemyIndexCache[index] = generateEnemyIndexCache[maxIndex - 1];
                    waitGenerateEnemy = 0;
                }
            }
            waitGenerateEnemy += Time.deltaTime;
        }

        public bool CheckCharactertOnHit(TeamType _hitType, Vector3 _pos)
        {
            bool isHit = false;
            switch (_hitType)
            {
                case TeamType.Player:
                    isHit = ((EnemyPool)enemyPool).EnemyCheckHit(_pos);
                    break;
                case TeamType.Enemy:
                case TeamType.Boost:
                    isHit = player.CheckHit(_hitType, _pos);
                    break;
                default:
                    break;
            }
            return isHit;
        }
    }
}
