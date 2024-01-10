using BaseGameController;
using BaseGameController.RPG;
using UnityEngine;

namespace BaseGameCharacter.RPG
{
    public class NPC : ICharacter
    {
        #region Constant
        private readonly string[] NAME_STRS = new string[] { "商人", "铁匠", "村民" };
        private readonly string[] TALK_STRS = new string[] { "卖药水啦，只有药水...", "我本来是个训练师，直到我的膝盖中了一箭...", "这个做的也太简陋了吧，为啥模型相差这么大啊喂！" };
        #endregion

        private GameObject baseObj;

        public string[] talkInfo;

        public bool isShopNPC;

        private CharacterState state;

        private Transform baseTrans;

        private Quaternion originQuaternion;

        private Quaternion baseQuaternion;

        private Quaternion targetQuaternion;

        private CharacterState toState;

        private float rotateTime;

        private RPGController controller;

        public NPC(GameObject _obj, int _index)
        {
            baseObj = _obj;
            baseTrans = _obj.transform;
            controller = GameController.Instance.GetTargetController<RPGController>();

            talkInfo = new string[2];
            talkInfo[0] = NAME_STRS[_index];
            talkInfo[1] = TALK_STRS[_index];
            isShopNPC = _index == 0;
        }

        public void DoUpdate()
        {
            NPCMove();
            NPCRotate();
        }

        public void SetBornData(Transform _trans)
        {
            baseTrans.position = _trans.position;
            baseTrans.rotation = _trans.rotation;
            originQuaternion = baseTrans.rotation;
            state = CharacterState.Idel;
        }

        public CharacterType GetCharacterType()
        {
            return CharacterType.NPC;
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
            ActionRotateResume();
            rotateTime = 0;
        }

        private void ActionRotateResume()
        {
            toState = CharacterState.Idel;
            baseQuaternion = baseTrans.rotation;
            targetQuaternion = originQuaternion;
            state = CharacterState.Rotate;
        }

        private void NPCMove()
        {

        }

        private void NPCRotate()
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
                if (toState == CharacterState.Action)
                {
                    controller.SetUIAction(CharacterType.NPC, this);
                }
            }
        }
    }
}
