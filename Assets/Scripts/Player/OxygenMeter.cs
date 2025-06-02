using UnityEngine;

namespace Scripts.Player
{
    public class OxygenMeter : MonoBehaviour
    {
        [SerializeField] private RectTransform meterMask;
        
        private CanvasGroup canvasGroup;

        private Vector2 meterMaskInitialSize; 
        
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            meterMaskInitialSize = meterMask.sizeDelta;
        }
        
        public void UpdateOxygen(float remainingOxygen)
        {
            meterMask.sizeDelta = new Vector2(meterMaskInitialSize.x, remainingOxygen * meterMaskInitialSize.y);
        }
    }
}
