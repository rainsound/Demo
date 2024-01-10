namespace BaseGameCharacter.Plane
{
    public interface IObjectPool
    {
        void DoUpdate();

        T GetObjectFromPool<T>(int _type) where T : PoolObject;

        void RecycleObject<T>(T _target) where T : PoolObject;

        void ClearPool();
    }
}