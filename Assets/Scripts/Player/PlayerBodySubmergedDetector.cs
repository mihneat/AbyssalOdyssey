using System;
using UnityEngine;

namespace Scripts.Player
{
    public class PlayerBodySubmergedDetector : MonoBehaviour
    {
        public event Action<bool> OnBodySubmergeChanged;
        
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
                OnBodySubmergeChanged?.Invoke(isSubmerged);
            }
            else if (airPocketCounter > 0 && isSubmerged)
            {
                isSubmerged = false;
                OnBodySubmergeChanged?.Invoke(isSubmerged);
            }
        }
    }
}
