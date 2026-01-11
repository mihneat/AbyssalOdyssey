using System;
using UnityEngine;

namespace Scripts.Volumes
{
    public class SubmergedDetector : MonoBehaviour
    {
        public event Action<bool> OnSubmergedStatusChanged;
        
        private int airPocketCounter;
        private bool isSubmerged = true;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("AirPocket"))
                return;

            AddToCounter(1);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("AirPocket"))
                return;

            AddToCounter(-1);
        }

        private void AddToCounter(int value)
        {
            airPocketCounter += value;
            if (airPocketCounter < 0)
                Debug.LogError("[PlayerOxygenDetector] Air pocket counter cannot be negative.");
            
            NotifyChange();
        }

        private void NotifyChange()
        {
            if (airPocketCounter <= 0 && !isSubmerged)
            {
                isSubmerged = true;
                OnSubmergedStatusChanged?.Invoke(isSubmerged);
            }
            else if (airPocketCounter > 0 && isSubmerged)
            {
                isSubmerged = false;
                OnSubmergedStatusChanged?.Invoke(isSubmerged);
            }
        }
    }
}
