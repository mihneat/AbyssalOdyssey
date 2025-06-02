using System;
using UnityEngine;

namespace Scripts.Harpoon
{
    public class HarpoonController : MonoBehaviour
    {
        public event Action<Harpoonable> OnHitSurface;

        private bool isAttached;
        
        private void OnCollisionEnter(Collision other)
        {
            // Debug.Log($"[HarpoonController] Collided with object: {other.gameObject.name}");
            
            if (isAttached)
                return;
            
            if (other.gameObject.TryGetComponent<Harpoonable>(out var harpoonable))
            {
                Debug.Log($"[HarpoonController] Found new harpoonable surface: {harpoonable.name}");
                
                isAttached = true;
                
                harpoonable.OnHarpoonHit();
                OnHitSurface?.Invoke(harpoonable);
            }
        }

        public void ReleaseHarpoon()
        {
            isAttached = false;
        }
    }
}
