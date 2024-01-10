using BaseGameController;
using BaseGameController.RPG;
using UnityEngine;

namespace BaseGameCharacter.RPG
{
    public class CharacterActionController
    {
        #region Constant
        private readonly string PLAYER_NAME = "RPGPrefabs/Character/Player";
        private readonly string[] NPC_NAMES = new string[] { "RPGPrefabs/Character/NPC01", "RPGPrefabs/Character/NPC02", "RPGPrefabs/Character/NPC03" };
        private readonly float INTERACTION_DISTANCE = 3.0f;
        #endregion

        private CharacterPool characterPool;

        private Transform cameraTrans;

        private Vector3 cameraOffset;

        public ICharacter player;

        public ICharacter[] characters;

        private Transform characterRoot;

        private Transform[] bornTrans;

        private ICharacter target;

        public CharacterActionController()
        {
            characters = new ICharacter[8];
            characterPool = new CharacterPool();
        }

        public void DoInit()
        {
            RPGController controller = GameController.Instance.GetTargetController<RPGController>();
            characterRoot = controller.characterRoot;
            bornTrans = controller.bornTrans;
            characterPool.DoInit();
        }

        public void DoUpdate()
        {
            player?.DoUpdate();

            for (int i = 0; i < characters.Length; i++)
            {
                characters[i]?.DoUpdate();
            }
        }

        public void DoLateUpdate()
        {
            MoveCamera();
        }

        public void SetCameraTrans(Transform _camera)
        {
            cameraTrans = _camera;
        }

        private void MoveCamera()
        {
            if (player == null || cameraTrans == null)
                return;

            Vector3 playerPos = player.GetCharacterPos();
            if (cameraOffset == default)
            {
                cameraOffset = cameraTrans.position - playerPos;
            }

            cameraTrans.position = playerPos + cameraOffset;
        }

        private void CreatePlayer()
        {
            if (player == null)
            {
                GameObject playerObj = Object.Instantiate(Resources.Load(PLAYER_NAME) as GameObject, characterRoot);
                player = new Player(playerObj);
                player.SetBornData(bornTrans[0]);
            }
        }

        private void CreateNPC()
        {
            for (int i = 0; i < 3; i++)
            {
                if (i >= characters.Length)
                    break;

                if (characters[i] == null)
                {
                    GameObject npcObj = Object.Instantiate(Resources.Load(NPC_NAMES[i]) as GameObject, characterRoot);
                    characters[i] = new NPC(npcObj, i);
                    characters[i].SetBornData(bornTrans[i + 1]);
                }
            }
        }

        private void CreateChest()
        {
            int needNum = Random.Range(0, 3);
            int createNum = 0;
            for (int i = 3; i < characters.Length; i++)
            {
                if (characters[i] == null)
                {
                    characters[i] = characterPool.GetObjectFromPool(CharacterType.Chest);
                    characters[i].GetCharacterObject().SetActive(true);
                    characters[i].SetBornData(bornTrans[createNum + 4]);
                    createNum++;
                }

                if (createNum >= needNum)
                    break;
            }
        }

        private void CreateEnemy()
        {
            int createNum = 0;
            for (int i = 3; i < characters.Length; i++)
            {
                if (characters[i] == null)
                {
                    characters[i] = characterPool.GetObjectFromPool(CharacterType.Enemy);
                    characters[i].GetCharacterObject().SetActive(true);
                    characters[i].SetBornData(bornTrans[createNum + 7]);
                    createNum++;
                }

                if (createNum >= 3)
                    break;
            }
        }

        public void CharacterRecycle(ICharacter _character)
        {
            if (_character is Chest || _character is Enemy)
            {
                for (int i = 0; i < characters.Length; i++)
                {
                    if (characters[i] != null && characters[i] == _character)
                    {
                        characters[i] = null;
                        characterPool.RecycleObject(_character);
                    }
                }
            }
        }

        private void RecycleAllRandomCharacter()
        {
            for (int i = 3; i < characters.Length; i++)
            {
                if (characters[i] != null)
                {
                    characterPool.RecycleObject(characters[i]);
                    characters[i] = null;
                }
            }
        }

        public void DestroyAllCharacter()
        {
            if (player != null)
            {
                Object.Destroy(player.GetCharacterObject());
                player = null;
            }
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i] != null && i < 3)
                {
                    Object.Destroy(characters[i].GetCharacterObject());
                }
                characters[i] = null;
            }
            characterPool.ClearPool();
        }

        public void InitStartGameCharacter()
        {
            RecycleAllRandomCharacter();
            CreatePlayer();
            CreateNPC();
            CreateChest();
            CreateEnemy();
        }

        public void RefreshRandomCharacter()
        {
            RecycleAllRandomCharacter();
            CreateChest();
            CreateEnemy();
        }

        public bool CheckCharacterDistance(out CharacterType _type)
        {
            if (player == null)
            {
                target = null;
                _type = CharacterType.Player;
                return false;
            }

            CharacterState playerState = player.GetCharacterState();
            if (playerState != CharacterState.Idel && playerState != CharacterState.Move)
            {
                _type = CharacterType.Player;
                return false;
            }

            int index = -1;
            float distance = INTERACTION_DISTANCE;
            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i] != null)
                {
                    float currentDis = Vector3.Distance(player.GetCharacterPos(), characters[i].GetCharacterPos());
                    if (currentDis <= distance)
                    {
                        index = i;
                        distance = currentDis;
                    }
                    
                }
            }

            if (index != -1)
            {
                target = characters[index];
                _type = characters[index].GetCharacterType();
                return true;
            }

            _type = CharacterType.Player;
            return false;
        }

        public ICharacter GetTargetCharacter()
        {
            return target;
        }

        public void ExecuteAction()
        {
            if (target == null)
                return;

            player.SetActionRotate(target.GetCharacterPos());
            target.SetActionRotate(player.GetCharacterPos());
        }

        public void ResetCharacterState()
        {
            player?.BackToIdel();
            target?.BackToIdel();
            target = null;
        }
    }
}
