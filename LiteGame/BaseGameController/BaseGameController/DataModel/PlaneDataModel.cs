namespace BaseGameData.Plane
{
    public class PlaneDataModel
    {
        #region Constant
        private readonly int TARGETSCORE = 2000;
        private readonly int MAX_FIRELEVEL = 3;
        #endregion

        private int playerHp;

        private int playerFireLv;

        private int playerScore;

        public PlaneDataModel() { }

        public void ResetPlayerData()
        {
            playerHp = 10;
            playerFireLv = 1;
            playerScore = 0;
        }

        public int GetPlayerCurrentHp()
        {
            return playerHp;
        }

        public bool CutPlayerHp(int _cutNum = 1)
        {
            playerHp -= _cutNum;
            return playerHp > 0;
        }

        public int GetPlayerFireLv()
        {
            return playerFireLv;
        }

        public bool AddPlayerFireLv(out int _fireLv)
        {
            bool isAdd = playerFireLv < MAX_FIRELEVEL;
            if (isAdd)
            {
                playerFireLv++;
            }
            _fireLv = playerFireLv;
            return isAdd;
        }

        public bool AddPlayerScore(int _addNum, out int _score)
        {
            playerScore += _addNum;
            _score = playerScore;
            return playerScore >= TARGETSCORE;
        }
    }
}
