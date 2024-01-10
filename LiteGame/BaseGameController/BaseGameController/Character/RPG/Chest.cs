using BaseGameController;
using BaseGameController.RPG;
using UnityEngine;

namespace BaseGameCharacter.RPG
{
    public class Chest : ICharacter
    {
        private GameObject baseObj;

        private CharacterState state;

        private Transform baseTrans;

        private RPGController controller;

        public Chest(GameObject _obj)
        {
            baseObj = _obj;
            baseTrans = _obj.transform;
            controller = GameController.Instance.GetTargetController<RPGController>();
        }

        public void DoUpdate()
        {

        }

        public void SetBornData(Transform _trans)
        {
            baseTrans.position = _trans.position;
            baseTrans.rotation = _trans.rotation;
            state = CharacterState.Idel;
        }

        public CharacterType GetCharacterType()
        {
            return CharacterType.Chest;
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
            state = CharacterState.Action;
            controller.SetUIAction(CharacterType.Chest, this);
        }

        public void BackToIdel()
        {
            state = CharacterState.Idel;
        }
    }
}