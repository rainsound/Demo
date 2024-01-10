using BaseGameController;
using BaseGameController.RPG;
using System.IO;
using UnityEngine;

namespace BaseGameData.RPG
{
    public class RPGDataModel
    {
        #region Constant
        private readonly int[] PLAYERLV_LIMIT = new int[] { 0, 0, 100, 500, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500, 6000, 6500, 7000, 7500, 8000, 8500, 9000, int.MaxValue };
        private readonly int MAX_POTION_NUM = 99;
        private readonly string SAVEFILE_NAME = "SaveData.save";
        #endregion

        public struct PlayerData
        {
            public int playerLv;
            public int playerExp;
            public int playerMaxHp;
            public int playerCurrentHp;
            public int playerAtk;
            public int playerDef;
            public int playerAtkSpeed;
            public int playerGold;
            public int smallLifePotionNum;
            public int bigLifePotionNum;
            public int currentDay;
        }

        private PlayerData data;

        private RPGController controller;

        public RPGDataModel()
        {
            ResetGameData();
        }

        public void DoInit()
        {
            controller = GameController.Instance.GetTargetController<RPGController>();
        }

        public void LoadGameData()
        {
            if (!LoadGameDataWithSaveFile())
            {
                ResetGameData();
            }
        }

        public void SaveGameData()
        {
            PlayerData saveData = data;
            saveData.currentDay++;
            string dataStr = JsonUtility.ToJson(saveData);
            File.WriteAllText(SAVEFILE_NAME, dataStr);
        }

        private bool LoadGameDataWithSaveFile()
        {
            if (CheckSaveFile())
            {
                string jsonStr = File.ReadAllText(SAVEFILE_NAME);
                data = JsonUtility.FromJson<PlayerData>(jsonStr);
                return true;
            }
            return false;
        }

        public bool CheckSaveFile()
        {
            return File.Exists(SAVEFILE_NAME);
        }

        public void DeleteSaveFile()
        {
            if (CheckSaveFile())
            {
                File.Delete(SAVEFILE_NAME);
            }
        }

        public void ResetGameData()
        {
            data.playerLv = 1;
            data.playerExp = 0;
            data.playerMaxHp = 50;
            data.playerCurrentHp = 50;
            data.playerAtk = 5;
            data.playerDef = 5;
            data.playerAtkSpeed = 5;
            data.smallLifePotionNum = 5;
            data.bigLifePotionNum = 0;
            data.playerGold = 500;
            data.currentDay = 1;
        }

        public bool DealPlayerBattleData(EnemyData _enemy)
        {
            int playerDamage = Mathf.Clamp(data.playerAtk - _enemy.enemyDef, 1, int.MaxValue);
            int rounds = _enemy.enemyHp / playerDamage;
            if (_enemy.enemyHp % data.playerAtk != 0)
            {
                rounds++;
            }

            int enemyRounds = data.playerAtkSpeed >= _enemy.enemyAtkSpeed ? rounds - 1 : rounds;
            int enemyDamage = Mathf.Clamp(_enemy.enemyAtk - data.playerDef, 1, int.MaxValue);
            PlayerAddData(DataType.Hp, -enemyRounds * enemyDamage);
            bool result = data.playerCurrentHp > 0;
            if (result)
            {
                PlayerAddExp(_enemy.enemyLv);
            }
            else
            {
                PlayerDie();
            }
            return result;
        }

        private void PlayerAddExp(int _enemyLv)
        {
            PlayerAddData(DataType.Exp, _enemyLv * 50);
            PlayerAddData(DataType.Gold, _enemyLv * 50);
            int playerLvUpNum = CheckPlayerLvUpNum();
            if (playerLvUpNum > 0)
            {
                PlayerUpgrade(playerLvUpNum);
            }
        }

        private int CheckPlayerLvUpNum()
        {
            for (int i = 0; i < PLAYERLV_LIMIT.Length; i++)
            {
                if (data.playerExp < PLAYERLV_LIMIT[i])
                {
                    return i - 1 - data.playerLv;
                }
            }
            return 0;
        }

        private void PlayerUpgrade(int _addNum)
        {
            PlayerAddData(DataType.Lv, _addNum);
            PlayerAddData(DataType.MaxHp, 50 * _addNum);
            PlayerAddData(DataType.Hp, data.playerMaxHp - data.playerCurrentHp);
            PlayerAddData(DataType.Atk, 5 * _addNum);
            PlayerAddData(DataType.Def, 5 * _addNum);
            PlayerAddData(DataType.AtkSpeed, 5 * _addNum);
        }

        private void PlayerDie()
        {
            ResetGameData();
        }

        public int GetPlayerData(DataType _type)
        {
            switch (_type)
            {
                case DataType.Lv:
                    return data.playerLv;
                case DataType.Exp:
                    return data.playerExp;
                case DataType.MaxHp:
                    return data.playerMaxHp;
                case DataType.Hp:
                    return data.playerCurrentHp;
                case DataType.Atk:
                    return data.playerAtk;
                case DataType.Def:
                    return data.playerDef;
                case DataType.AtkSpeed:
                    return data.playerAtkSpeed;
                case DataType.Gold:
                    return data.playerGold;
                case DataType.SmallPotion:
                    return data.smallLifePotionNum;
                case DataType.BigPotion:
                    return data.bigLifePotionNum;
                case DataType.Day:
                    return data.currentDay;
                default:
                    return 0;
            }
        }

        public void PlayerAddData(DataType _type, int _addNum)
        {
            switch (_type)
            {
                case DataType.Lv:
                    data.playerLv += _addNum;
                    break;
                case DataType.Exp:
                    data.playerExp += _addNum;
                    break;
                case DataType.MaxHp:
                    data.playerMaxHp += _addNum;
                    break;
                case DataType.Hp:
                    data.playerCurrentHp = Mathf.Clamp(data.playerCurrentHp + _addNum, 0, data.playerMaxHp);
                    break;
                case DataType.Atk:
                    data.playerAtk += _addNum;
                    break;
                case DataType.Def:
                    data.playerDef += _addNum;
                    break;
                case DataType.AtkSpeed:
                    data.playerAtkSpeed += _addNum;
                    break;
                case DataType.Gold:
                    data.playerGold += _addNum;
                    break;
                case DataType.SmallPotion:
                    data.smallLifePotionNum = Mathf.Clamp(data.smallLifePotionNum + _addNum, 0, MAX_POTION_NUM);
                    break;
                case DataType.BigPotion:
                    data.bigLifePotionNum = Mathf.Clamp(data.bigLifePotionNum + _addNum, 0, MAX_POTION_NUM);
                    break;
                case DataType.Day:
                    data.currentDay += _addNum;
                    break;
                default:
                    break;
            }
            controller.DataChangeUI(_type, GetPlayerData(_type));
        }

        public void PlayerBuyItem(DataType _type)
        {
            bool canBuy = false;
            switch (_type)
            {
                case DataType.SmallPotion:
                    canBuy = CheckPlayerGoldCanBuy(100, true);
                    break;
                case DataType.BigPotion:
                    canBuy = CheckPlayerGoldCanBuy(600, true);
                    break;
                default:
                    break;
            }

            if (canBuy)
            {
                PlayerAddData(_type, 1);
            }
        }

        public void PlayerUsePotion(DataType _type)
        {
            switch (_type)
            {
                case DataType.SmallPotion:
                    if (data.smallLifePotionNum > 0 && data.playerCurrentHp < data.playerMaxHp)
                    {
                        PlayerAddData(DataType.SmallPotion, -1);
                        PlayerAddData(DataType.Hp, 50);
                    }
                    break;
                case DataType.BigPotion:
                    if (data.bigLifePotionNum > 0 && data.playerCurrentHp < data.playerMaxHp)
                    {
                        PlayerAddData(DataType.BigPotion, -1);
                        PlayerAddData(DataType.Hp, 300);
                    }
                    break;
                default:
                    break;
            }
        }

        public bool CheckPlayerGoldCanBuy(int _costNum, bool _needCost)
        {
            bool canBuy = data.playerGold >= _costNum;
            if (_needCost && canBuy)
            {
                PlayerAddData(DataType.Gold, -_costNum);
            }
            return canBuy;
        }

        public EnemyData GetEnemyData()
        {
            int lvIncrease = Random.Range(0, 2);
            int enemyLv = data.playerLv + lvIncrease;
            int enemyHp = enemyLv * 40;
            int enemyAtk = enemyLv * 3;
            int enemyDef = enemyLv * 3;
            int enemyAtkSpeed = enemyLv * 3;
            return new EnemyData(enemyLv, enemyHp, enemyAtk, enemyDef, enemyAtkSpeed);
        }
    }

    public struct EnemyData
    {
        public int enemyLv;
        public int enemyHp;
        public int enemyAtk;
        public int enemyDef;
        public int enemyAtkSpeed;

        public EnemyData(int _lv, int _hp, int _atk, int _def, int _atkSpeed)
        {
            enemyLv = _lv;
            enemyHp = _hp;
            enemyAtk = _atk;
            enemyDef = _def;
            enemyAtkSpeed = _atkSpeed;
        }
    }

    public enum DataType
    {
        Lv,
        Exp,
        MaxHp,
        Hp,
        Atk,
        Def,
        AtkSpeed,
        SmallPotion,
        BigPotion,
        Gold,
        Day,
    }
}
