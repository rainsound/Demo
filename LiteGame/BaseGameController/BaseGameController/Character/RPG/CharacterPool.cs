using BaseGameController;
using BaseGameController.RPG;
using BaseGameData.RPG;
using UnityEngine;

namespace BaseGameCharacter.RPG
{
    public class CharacterPool
    {
        #region Constant
        private readonly string[] CHARACTER_NAMES = new string[] { "RPGPrefabs/Character/RPGBox", "RPGPrefabs/Monster/SlimePBR", "RPGPrefabs/Monster/TurtleShellPBR" };
        private readonly int[] POOL_LENGTH = new int[] { 2, 3, 3 };
        #endregion

        private RPGDataModel dataModel;

        private ICharacter[] characters;

        private Transform characterPoolRoot;

        public CharacterPool()
        {
            int characterLength = 0;
            for (int i = 0; i < POOL_LENGTH.Length; i++)
            {
                characterLength += POOL_LENGTH[i];
            }
            characters = new ICharacter[characterLength];
        }

        public void DoInit()
        {
            RPGController controller = GameController.Instance.GetTargetController<RPGController>();
            dataModel = controller.dataModel;
            characterPoolRoot = controller.characterRoot;
        }

        private ICharacter CreateCharacter(CharacterType _type, int _index)
        {
            switch (_type)
            {
                case CharacterType.Chest:
                    return CreateChest(_index);
                case CharacterType.Enemy:
                    return CreateEnemy(_index);
                default:
                    return null;
            }
        }

        private Chest CreateChest(int _index)
        {
            GameObject chestObj = Object.Instantiate(Resources.Load(CHARACTER_NAMES[_index]) as GameObject, characterPoolRoot);
            Chest chest = new Chest(chestObj);
            return chest;
        }

        private Enemy CreateEnemy(int _index)
        {
            GameObject enemyObj = Object.Instantiate(Resources.Load(CHARACTER_NAMES[_index]) as GameObject, characterPoolRoot);
            Enemy enemy = new Enemy(enemyObj);
            return enemy;
        }

        private int GetTargetPoolIndex(CharacterType _type)
        {
            switch (_type)
            {
                case CharacterType.Chest:
                    return 0;
                case CharacterType.Enemy:
                    int targetIndex = Random.Range(1, 3);
                    return targetIndex;
                default:
                    return -1;
            }
        }

        private ICharacter getTargetCharacter(CharacterType _type)
        {
            ICharacter target = null;
            int targetIndex = GetTargetPoolIndex(_type);
            if (targetIndex >= 0)
            {
                int startIndex = 0;
                for (int i = 0; i < targetIndex; i++)
                {
                    startIndex += POOL_LENGTH[i];
                }
                int endIndex = startIndex + POOL_LENGTH[targetIndex];
                for (int i = startIndex; i < endIndex; i++)
                {
                    if (characters[i] != null && !characters[i].GetCharacterObject().activeSelf)
                    {
                        target = characters[i];
                        break;
                    }

                    if (characters[i] == null)
                    {
                        characters[i] = CreateCharacter(_type, targetIndex);
                        target = characters[i];
                        break;
                    }
                }
            }
            return target;
        }

        public ICharacter GetObjectFromPool(CharacterType _type)
        {
            ICharacter target = getTargetCharacter(_type);

            if (target != null && _type == CharacterType.Enemy)
            {
                ((Enemy)target).SetEnemyData(dataModel.GetEnemyData());
            }

            return target;
        }

        public void RecycleObject(ICharacter _target)
        {
            _target.GetCharacterObject().SetActive(false);
        }

        public void ClearPool()
        {
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i] != null)
                {
                    Object.Destroy(characters[i].GetCharacterObject());
                    characters[i] = null;
                }
            }
        }
    }
}
