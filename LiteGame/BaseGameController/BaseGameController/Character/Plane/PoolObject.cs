using UnityEngine;

namespace BaseGameCharacter.Plane
{
    public abstract class PoolObject
    {
        public int type;

        public float disappearTime;

        public bool isUse = false;

        public bool needDestory = false;

        public GameObject baseObj;

        public Transform baseTrans;

        public PoolObject(GameObject _obj)
        {
            baseObj = _obj;
            baseTrans = _obj.transform;
        }

        public virtual void DoCreate() { }

        public virtual void DoUpdate() { }

        public virtual void DoRecycle() { }

        public virtual void DoDestroy() { }

        protected void ResetCountTime(float _time)
        {
            disappearTime = _time;
        }

    }
}
