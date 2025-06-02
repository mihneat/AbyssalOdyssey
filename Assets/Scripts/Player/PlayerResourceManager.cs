using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Scripts.Player
{
    public class PlayerResourceManager : MonoBehaviour
    {
        // Measured in minutes
        [SerializeField] private float oxygenTankCapacity;
        [SerializeField] private float oxygenRecoveryRate;
        [SerializeField] private PlayerEnvironmentDetector environmentDetector;
        [SerializeField] private Image deathImage;
        [SerializeField] private OxygenMeter oxygenMeter;

        private float oxygenTankCapacitySeconds;
        private float oxygenRecoveryRateSeconds;
        private float remainingOxygen;

        private void Awake()
        {
            oxygenTankCapacitySeconds = oxygenTankCapacity * 60;
            oxygenRecoveryRateSeconds = oxygenRecoveryRate * 60;
            remainingOxygen = oxygenTankCapacitySeconds;
        }

        private void Update()
        {
            if (environmentDetector.Environment == Environment.Air)
            {
                // Recover O2
                remainingOxygen = Mathf.Min(remainingOxygen + oxygenRecoveryRateSeconds * Time.deltaTime, oxygenTankCapacitySeconds);
            } else if (environmentDetector.Environment == Environment.Underwater)
            {
                // Lose O2
                remainingOxygen -= Time.deltaTime;
                
                // Drown when no oxygen left
                if (remainingOxygen < -1.0f)
                    StartCoroutine(Drown());
            }
            
            // Update the UI
            oxygenMeter.UpdateOxygen(Mathf.Max(remainingOxygen, 0.0f) / oxygenTankCapacitySeconds);
        }

        private IEnumerator Drown()
        {
            // Fade to black
            float t = 0.0f;
            float fadeOutTime = 2.0f;
            Color transparentBlack = Color.black;
            transparentBlack.a = 0.0f;
            while (t < fadeOutTime)
            {
                float u = t / fadeOutTime;
                deathImage.color = transparentBlack * (1 - u) + Color.black * u;
                
                t += Time.deltaTime;
                yield return null;
            }
            
            // Reload the scene
            SceneManager.LoadScene(0);
        } 
    }
}
