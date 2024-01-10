using BaseGameController.RPG;
using UnityEngine;
using UnityEngine.AI;

namespace BaseGameCharacter.RPG
{
    public class Player : ICharacter
    {
        #region Constant
        private readonly string ANIMATOR_SPEED = "Speed";
        private readonly string ANIMATOR_IDELTRIGGER = "IdelAction";
        #endregion

        private GameObject baseObj;

        private NavMeshAgent playerAgent;

        private Animator animator;

        private CharacterState state;

        private Transform baseTrans;

        private Quaternion baseQuaternion;

        private Quaternion targetQuaternion;

        private CharacterState toState;

        private float currentSpeed = 0;

        private float rotateTime;

        private float idelTime;

        public Player(GameObject _obj)
        {
            baseObj = _obj;
            baseTrans = _obj.transform;
            playerAgent = _obj.GetComponent<NavMeshAgent>();
            animator = _obj.GetComponent<Animator>();
        }

        public void DoUpdate()
        {
            PlayerMove();
            PlayerRotate();
        }

        public void SetBornData(Transform _trans)
        {
            baseTrans.position = _trans.position;
            baseTrans.rotation = _trans.rotation;
            state = CharacterState.Idel;
            rotateTime = 0;
            idelTime = 0;
        }

        public CharacterType GetCharacterType()
        {
            return CharacterType.Player;
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
            SetMoveAnimation(0);
            toState = CharacterState.Action;
            baseQuaternion = baseTrans.rotation;
            _targetPos = new Vector3(_targetPos.x, baseTrans.position.y, _targetPos.z);
            baseTrans.LookAt(_targetPos);
            targetQuaternion = baseTrans.rotation;
            baseTrans.rotation = baseQuaternion;
            state = CharacterState.Rotate;
        }

        public void BackToIdel()
        {
            state = CharacterState.Idel;
            rotateTime = 0;
        }

        private void PlayerMove()
        {
            if (state != CharacterState.Idel && state != CharacterState.Move)
                return;

            float forwardDir = Input.GetAxis(RPGController.VERTICAL);
            float rightDir = Input.GetAxis(RPGController.HORIZONTAL);
            if (forwardDir != 0 || rightDir != 0)
            {
                state = CharacterState.Move;
                idelTime = 0;
                Vector3 dir = new Vector3(rightDir, 0, forwardDir);
                //Vector3 dir = camera.forward * forwardDir + camera.right * rightDir;
                float rotateAngle = Vector3.SignedAngle(baseTrans.forward, dir, Vector3.up);
                baseTrans.Rotate(Vector3.up, rotateAngle * Time.deltaTime * 10.0f);
                playerAgent.Move(dir.normalized * Time.deltaTime * 7f);
                SetMoveAnimation(Mathf.Clamp(dir.magnitude, 0, 1.0f));
            }
            else
            {
                state = CharacterState.Idel;
                idelTime += Time.deltaTime;
                if (idelTime > 15.0f)
                {
                    SetIdelTrigger();
                }
                if (currentSpeed > 0)
                {
                    SetMoveAnimation(0);
                }
            }
        }

        private void PlayerRotate()
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
                state = toState;
            }
        }

        private void SetMoveAnimation(float _value)
        {
            animator.SetFloat(ANIMATOR_SPEED, _value);
            currentSpeed = _value;
        }

        private void SetIdelTrigger()
        {
            animator.SetTrigger(ANIMATOR_IDELTRIGGER);
            idelTime = 0;
        }
    }
}