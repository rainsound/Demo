using BaseGameController.Plane;
using UnityEngine;

namespace BaseGameCharacter.Plane
{
    public interface ICharacter
    {
        void DoUpdate();

        void SetBornData(Vector3 _bornPoint);

        void DieAction();

        TeamType GetTeamType();

        GameObject GetCharacterObject();

        bool CheckHit(TeamType _type, Vector3 _pos);
    }
}