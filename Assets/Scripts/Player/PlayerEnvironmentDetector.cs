using System;
using UnityEngine;

namespace Scripts.Player
{
    public enum Environment
    {
        Air,
        Underwater
    }
    
    public class PlayerEnvironmentDetector : MonoBehaviour
    {
        public Environment Environment => currEnvironment;
        private Environment currEnvironment = Environment.Air;

        public event Action<Environment> OnEnvironmentChanged; 

        public void ChangeEnvironment(Environment newEnvironment)
        {
            Debug.Log($"[PlayerEnvironmentDetector] Environment changed to {newEnvironment.ToString()}");
            currEnvironment = newEnvironment;
            
            OnEnvironmentChanged?.Invoke(newEnvironment);
        }
    }
}
