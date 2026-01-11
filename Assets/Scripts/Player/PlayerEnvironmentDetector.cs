using System;
using UnityEngine;

namespace Scripts.Player
{
    public enum Environment
    {
        Air,
        HalfUnderwater,
        Underwater,
    }
    
    public class PlayerEnvironmentDetector : MonoBehaviour
    {
        [SerializeField] private PlayerOxygenDetector oxygenDetector;
        [SerializeField] private PlayerBodySubmergedDetector bodySubmergedDetector;
        
        public Environment HeadEnvironment => currHeadEnvironment;
        private Environment currHeadEnvironment = Environment.Underwater;
        
        public Environment Environment => currEnvironment;
        private Environment currEnvironment = Environment.Underwater;

        private bool isHeadSubmerged = true;
        private bool isBodySubmerged = true;

        public event Action<Environment> OnHeadEnvironmentChanged;
        public event Action<Environment> OnEnvironmentChanged;

        private void OnEnable()
        {
            oxygenDetector.OnHeadSubmergeChanged += HandleOnHeadSubmergeChanged;
            bodySubmergedDetector.OnBodySubmergeChanged += HandleOnBodySubmergeChanged;
        }

        private void OnDisable()
        {
            oxygenDetector.OnHeadSubmergeChanged -= HandleOnHeadSubmergeChanged;
            bodySubmergedDetector.OnBodySubmergeChanged -= HandleOnBodySubmergeChanged;
        }

        private void HandleOnHeadSubmergeChanged(bool isSubmerged)
        {
            isHeadSubmerged = isSubmerged;
            
            currHeadEnvironment = isSubmerged ? Environment.Underwater : Environment.Air;
            OnHeadEnvironmentChanged?.Invoke(currHeadEnvironment);

            Debug.Log("[PlayerEnvironmentDetector] Head environment changed to: " + currHeadEnvironment);
            
            UpdateEnvironment();
        }

        private void HandleOnBodySubmergeChanged(bool isSubmerged)
        {
            isBodySubmerged = isSubmerged;
            
            UpdateEnvironment();
        }

        private void UpdateEnvironment()
        {
            Environment prevEnvironment = currEnvironment;
            if (!isHeadSubmerged && !isBodySubmerged)
                currEnvironment = Environment.Air;
            else if (!isHeadSubmerged && isBodySubmerged)
                currEnvironment = Environment.HalfUnderwater;
            else
                currEnvironment = Environment.Underwater;
            
            if (prevEnvironment != currEnvironment)
            {
                Debug.Log("[PlayerEnvironmentDetector] Environment changed to: " + currEnvironment);
                OnEnvironmentChanged?.Invoke(currEnvironment);
            }
        }

        public void ChangeEnvironment(Environment newEnvironment)
        {
            Debug.LogError("[PlayerEnvironmentDetector] This is deprecated, please move to AirPocket system");
            return;
            
            Debug.Log($"[PlayerEnvironmentDetector] Environment changed to {newEnvironment.ToString()}");
            currEnvironment = newEnvironment;
            
            OnEnvironmentChanged?.Invoke(newEnvironment);
        }
    }
}
