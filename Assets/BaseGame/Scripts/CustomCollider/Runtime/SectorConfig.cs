using UnityEngine;

namespace TW.CustomCollider
{
    [CreateAssetMenu(fileName = "SectorConfig", menuName = "ScriptableObjects/SectorConfig", order = 1)]

    public class SectorConfig : ScriptableObject
    {
        public Vector3 Center;
        public float MinDistanceFromCenter;
        public float Depth;
        public float Length;
    }
}