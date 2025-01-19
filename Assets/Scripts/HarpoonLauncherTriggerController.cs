using System;
using UnityEngine;

namespace Scripts
{
    public class HarpoonLauncherTriggerController : MonoBehaviour
    {
        [SerializeField] private Vector2 rotationRange;
        [SerializeField] [Range(0, 1)] private float triggerAmount;

        private void OnValidate()
        {
            UpdateRotation();
        }

        public void SetInterpolationValue(float newT)
        {
            triggerAmount = Mathf.Clamp01(newT);
            
            UpdateRotation();
        }

        private void UpdateRotation()
        {
            transform.localRotation = Quaternion.Euler(Mathf.Lerp(rotationRange.x, rotationRange.y, triggerAmount), 0, 0);
        }
    }
}
