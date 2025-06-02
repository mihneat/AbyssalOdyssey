using System;
using UnityEngine;

namespace Scripts
{
    public class ResistanceMeter : MonoBehaviour
    {
        [SerializeField] private RectTransform meterMask;
        
        private CanvasGroup canvasGroup;

        private Vector2 meterMaskInitialSize; 
        
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            meterMaskInitialSize = meterMask.sizeDelta;
            
            ToggleVisibility(false);
        }

        public void ToggleVisibility(bool state)
        {
            UpdateTension(0.0f);
            
            canvasGroup.alpha = state ? 1.0f : 0.0f;
            canvasGroup.interactable = state;
            canvasGroup.blocksRaycasts = state;
        }

        public void UpdateTension(float tension)
        {
            meterMask.sizeDelta = new Vector2(meterMaskInitialSize.x, tension * meterMaskInitialSize.y);
        }
    }
}
