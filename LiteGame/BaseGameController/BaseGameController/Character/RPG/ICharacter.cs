using BaseGameController.RPG;
using UnityEngine;

namespace BaseGameCharacter.RPG
{
    public interface ICharacter
    {
        void DoUpdate();

        void SetBornData(Transform _trans);

        CharacterType GetCharacterType();

        CharacterState GetCharacterState();

        Vector3 GetCharacterPos();

        GameObject GetCharacterObject();

        void SetActionRotate(Vector3 _targetPos);

        void BackToIdel();
    }
}
