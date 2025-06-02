using UnityEngine;

namespace Scripts
{
    public abstract class Harpoonable : MonoBehaviour
    {
        [SerializeField] [Min(0.0f)] public float resistance;
        [SerializeField] [Min(0.0f)] public float value;

        public abstract void OnHarpoonHit();
        public abstract void OnReelResult(bool isSuccessful);

        public abstract void OnStartReeling();
        public abstract void OnStopReeling();
    }
}
