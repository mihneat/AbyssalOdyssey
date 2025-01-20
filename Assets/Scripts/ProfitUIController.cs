using System;
using TMPro;
using UnityEngine;

namespace Scripts
{
    public class ProfitUIController : MonoBehaviour
    {
        [SerializeField] private HarpoonLauncherController harpoonLauncherController;
        [SerializeField] private TMP_Text profitText;

        private float profit;
        
        private void Awake()
        {
            profitText.text = $"Profit: ${profit:F}";
        }

        private void OnEnable()
        {
            harpoonLauncherController.OnCaughtObject += HandleOnCaughtObject;
        }

        private void OnDisable()
        {
            harpoonLauncherController.OnCaughtObject -= HandleOnCaughtObject;
        }

        private void HandleOnCaughtObject(Harpoonable caughtObject)
        {
            AddProfit(caughtObject.value);
        }

        private void AddProfit(float sum)
        {
            profit += sum;
            
            profitText.text = $"Profit: ${profit:F}";
        }
    }
}
