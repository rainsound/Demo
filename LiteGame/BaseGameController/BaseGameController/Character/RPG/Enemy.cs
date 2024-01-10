using BaseGameController;
using BaseGameController.RPG;
using BaseGameData.RPG;
using UnityEngine;

namespace BaseGameCharacter.RPG
{
    public class Enemy : ICharacter
    {
        #region Constant
        private readonly string ANIMATOR_RUN = "IsRun";
        private readonly string[] ANIMATOR_IDEL = new string[] { "Idel01", "Idel02" };
        #endregion

        private GameObject baseObj;

        private Animator animator;

        private EnemyData enemyData;

        private CharacterState state;

        private CharacterState lastState;

        private Transform baseTrans;

        private Vector3 originPos;

        private Vector3 basePos;

        private Vector3 movePos;

        private float needMoveTime;

        private float moveTime;

        private float waitTime;

        private Quaternion baseQuaternion;

        private Quaternion targetQuaternion;

        private float rotateTime;

        private RPGController controller;

        public Enemy(GameObject _obj)
        {
            baseObj = _obj;
            baseTrans = _obj.transform;
            animator = _obj.GetComponent<Animator>();
            controller = GameController.Instance.GetTargetController<RPGController>();
        }

        public void DoUpdate()
        {
            EnemyPatrol();
            EnemyRotate();
        }

        public void SetBornData(Transform _trans)
        {
            baseTrans.position = _trans.position;
            baseTrans.rotation = _trans.rotation;
            state = CharacterState.Idel;
            originPos = _trans.position;
            movePos = originPos;
            moveTime = 0;
            waitTime = 0;
            rotateTime = 0;
        }

        public void SetEnemyData(EnemyData _data)
        {
            enemyData = _data;
        }

        public EnemyData GetEnemyData()
        {
            return enemyData;
        }

        public CharacterType GetCharacterType()
        {
            return CharacterType.Enemy;
        }

        public CharacterState GetCharacterState()
        {
            return state;
        }

        public Vector3 GetCharacterPos()
        {
            return baseTrans.position;
        }

        public GameObject GetCharacterObject()
        {
            return baseObj;
        }

        public void SetActionRotate(Vector3 _targetPos)
        {
            lastState = state;
            state = CharacterState.Action;
            animator.SetBool(ANIMATOR_RUN, false);
            controller.SetUIAction(CharacterType.Enemy, this);
        }

        public void BackToIdel()
        {
            if (lastState == CharacterState.Move)
            {
                animator.SetBool(ANIMATOR_RUN, true);
            }
            state = lastState;
        }

        private void EnemyPatrol()
        {
            if (state != CharacterState.Idel && state != CharacterState.Move)
                return;

            if (state == CharacterState.Idel)
            {
                waitTime += Time.deltaTime;
                if (waitTime >= 8.0f)
                {
                    FindTargetPos();
                    waitTime = 0;
                    state = CharacterState.Rotate;
                }
            }

            if (state == CharacterState.Move)
            {
                EnemyMove();
            }
        }

        private void FindTargetPos()
        {
            if (movePos != originPos)
            {
                needMoveTime = Vector3.Distance(baseTrans.position, originPos) / 2.0f;
                basePos = baseTrans.position;
                movePos = originPos;
            }
            else
            {
                int distance = Random.Range(5,9);
                int angle = Random.Range(0, 360);
                float xOffset = Mathf.Cos(angle + 90) * distance;
                float zOffset = Mathf.Sin(angle + 90) * distance;
                basePos = baseTrans.position;
                movePos = new Vector3(xOffset + originPos.x, originPos.y, zOffset + originPos.z);
                needMoveTime = distance / 2.0f;
            }
            baseQuaternion = baseTrans.rotation;
            baseTrans.LookAt(movePos);
            targetQuaternion = baseTrans.rotation;
            baseTrans.rotation = baseQuaternion;
        }

        private void EnemyMove()
        {
            if (moveTime < 1.0f)
            {
                moveTime += Time.deltaTime / needMoveTime;
                Vector3 targetPos = Vector3.Lerp(basePos, movePos, moveTime);
                baseTrans.position = targetPos;
            }
            else
            {
                state = CharacterState.Idel;
                moveTime = 0;
                animator.SetBool(ANIMATOR_RUN, false);
                int index = Random.Range(0, 2);
                animator.SetTrigger(ANIMATOR_IDEL[index]);
            }
        }

        private void EnemyRotate()
        {
            if (state != CharacterState.Rotate)
                return;

            if (rotateTime <= 1.0f)
            {
                Quaternion target = Quaternion.Lerp(baseQuaternion, targetQuaternion, rotateTime);
                baseTrans.rotation = target;
                rotateTime += Time.deltaTime * 5;
            }
            else
            {
                rotateTime = 0;
                state = CharacterState.Move;
                animator.SetBool(ANIMATOR_RUN, true);
            }
        }
    }
}
