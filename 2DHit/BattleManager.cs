using System.Collections.Generic;
using UnityEngine;

namespace Hit
{
    public class BattleManager
    {
        public bool CheckHit(HitPack packA, HitPack packB)
        {
            if (packA.shape == ECollideShape.Rectangle && packB.shape == ECollideShape.Rectangle)
            {
                return CheckRectangle(packA, packB);
            }
            else if (packA.shape == ECollideShape.Circular && packB.shape == ECollideShape.Circular)
            {
                return Vector3.Distance(packA.pos, packB.pos) <= packA.radius + packB.radius;
            }
            else if (packA.shape != ECollideShape.None && packB.shape != ECollideShape.None)
            {
                var cPack = packA.shape == ECollideShape.Circular ? packA : packB;
                var rPack = packA.shape == ECollideShape.Circular ? packB : packA;
                return CheckCircularAndRectangle(cPack, rPack);
            }
            return false;
        }

        private bool CheckRectangle(HitPack packA, HitPack packB)
        {
            if (packA.angle == packB.angle || (packA.angle - packB.angle) % 180 == 0)
            {
                bool xHit = Mathf.Abs(packA.pos.x - packB.pos.x) <= (packA.range.y + packB.range.y) / 2;
                bool zHit = Mathf.Abs(packA.pos.z - packB.pos.z) <= (packA.range.x + packB.range.x) / 2;
                return xHit && zHit;
            }
            if ((packA.angle - packB.angle) % 90 == 0)
            {
                bool xHit = Mathf.Abs(packA.pos.x - packB.pos.x) <= (packA.range.y + packB.range.x) / 2;
                bool zHit = Mathf.Abs(packA.pos.z - packB.pos.z) <= (packA.range.x + packB.range.y) / 2;
                return xHit && zHit;
            }
            else
            {
                if (packA.points.CheckHit(packB.points))
                {
                    return packB.points.CheckHit(packA.points);
                }
            }
            return false;
        }

        private bool CheckCircularAndRectangle(HitPack cPack, HitPack rPack)
        {
            return rPack.points.CheckHit(cPack);
        }
    }

    public struct HitPack
    {
        public Vector3 pos;

        public ECollideShape shape;

        public Vector2 range;

        public float angle;

        public HitPackRectangle points;

        public float radius;

        public bool isValue;

        public HitPack(Vector3 _pos, ECollideShape _shape = ECollideShape.None, Vector2 _range = default, float _angle = 0, HitPackRectangle _points = default, float _radius = 0)
        {
            pos = new Vector3(_pos.x, 0, _pos.z);
            shape = _shape;
            range = _range;
            angle = _angle;
            points = _points;
            radius = _radius;
            isValue = true;
        }
    }

    public struct HitPackRectangle
    {
        public Vector2 pA;

        public Vector2 pB;

        public Vector2 pC;

        public Vector2 pD;

        public float kA;

        public float kB;

        public HitPackRectangle(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            pA = new Vector2(a.x, a.z);
            pB = new Vector2(b.x, b.z);
            pC = new Vector2(c.x, c.z);
            pD = new Vector2(d.x, d.z);
            kA = (pA.y - pD.y) / (pA.x - pD.x);
            kB = (pA.y - pB.y) / (pA.x - pB.x);
        }

        public bool CheckHit(HitPackRectangle pack)
        {
            if (kA == kB || pack.kA == pack.kB)
            {
                return false;
            }
            if (Check(kA, kB, pD, pack))
            {
                return Check(kB, kA, pB, pack);
            }
            return false;
        }

        private bool Check(float _kA, float _kB, Vector2 p, HitPackRectangle pack)
        {
            if (_kA == 0)
            {
                float _small_x_self = Mathf.Min(pA.x, p.x);
                float _big_x_self = Mathf.Max(pA.x, p.x);
                float _small_x_other = Mathf.Min(pack.pA.x, pack.pB.x, pack.pC.x, pack.pD.x);
                float _big_x_other = Mathf.Max(pack.pA.x, pack.pB.x, pack.pC.x, pack.pD.x);
                return _small_x_self <= _big_x_other && _big_x_self >= _small_x_other;
            }
            if (_kB == 0)
            {
                float _small_x_self = Mathf.Min(pA.y, p.y);
                float _big_x_self = Mathf.Max(pA.y, p.y);
                float _small_x_other = Mathf.Min(pack.pA.y, pack.pB.y, pack.pC.y, pack.pD.y);
                float _big_x_other = Mathf.Max(pack.pA.y, pack.pB.y, pack.pC.y, pack.pD.y);
                return _small_x_self <= _big_x_other && _big_x_self >= _small_x_other;
            }
            float small_x_self = (pA.y - (pA.x * _kB)) / (_kA - _kB);
            float big_x_self = small_x_self;
            float x_self = (p.y - (p.x * _kB)) / (_kA - _kB);
            small_x_self = Mathf.Min(small_x_self, x_self);
            big_x_self = Mathf.Max(big_x_self, x_self);
            float small_x_other = (pack.pA.y - (pack.pA.x * _kB)) / (_kA - _kB);
            float big_x_other = small_x_other;
            float x_other = (pack.pB.y - (pack.pB.x * _kB)) / (_kA - _kB);
            small_x_other = Mathf.Min(small_x_other, x_other);
            big_x_other = Mathf.Max(big_x_other, x_other);
            x_other = (pack.pC.y - (pack.pC.x * _kB)) / (_kA - _kB);
            small_x_other = Mathf.Min(small_x_other, x_other);
            big_x_other = Mathf.Max(big_x_other, x_other);
            x_other = (pack.pD.y - (pack.pD.x * _kB)) / (_kA - _kB);
            small_x_other = Mathf.Min(small_x_other, x_other);
            big_x_other = Mathf.Max(big_x_other, x_other);
            return small_x_self <= big_x_other && big_x_self >= small_x_other;
        }

        public bool CheckHit(HitPack pack)
        {
            if (pack.shape != ECollideShape.Circular)
            {
                return false;
            }
            if (kA == kB)
            {
                return false;
            }
            if (Check(kA, kB, pD, pack))
            {
                return Check(kB, kA, pB, pack);
            }
            return false;
        }

        private bool Check(float _kA, float _kB, Vector2 p, HitPack pack)
        {
            if (_kA == 0)
            {
                float _small_x_self = Mathf.Min(pA.x, p.x);
                float _big_x_self = Mathf.Max(pA.x, p.x);
                float _small_x_other = pack.pos.x - pack.radius;
                float _big_x_other = pack.pos.x + pack.radius;
                return _small_x_self <= _big_x_other && _big_x_self >= _small_x_other;
            }
            if (_kB == 0)
            {
                float _small_x_self = Mathf.Min(pA.y, p.y);
                float _big_x_self = Mathf.Max(pA.y, p.y);
                float _small_x_other = pack.pos.z - pack.radius;
                float _big_x_other = pack.pos.z + pack.radius;
                return _small_x_self <= _big_x_other && _big_x_self >= _small_x_other;
            }
            float point_a_x = (pA.y - (_kB * pA.x) - pack.pos.z + (_kA * pack.pos.x)) / (_kA - _kB);
            float point_a_y = (point_a_x * _kB) + pA.y - (_kB * pA.x);
            Vector3 point_a = new Vector3(point_a_x, 0, point_a_y);

            float point_p_x = (p.y - (_kB * p.x) - pack.pos.z + (_kA * pack.pos.x)) / (_kA - _kB);
            float point_p_y = (point_a_x * _kB) + p.y - (_kB * p.x);
            Vector3 point_p = new Vector3(point_p_x, 0, point_p_y);

            return Vector3.Distance(pack.pos, point_a) <= pack.radius || Vector3.Distance(pack.pos, point_p) <= pack.radius;
        }
    }

    public enum ECollideShape
    {
        None,
        Rectangle,
        Circular
    }
}
