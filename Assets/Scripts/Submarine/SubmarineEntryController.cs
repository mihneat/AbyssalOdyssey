using Scripts.Helper;
using Scripts.Player;
using UnityEngine;

namespace Scripts.Submarine
{
    public class SubmarineEntryController : MonoBehaviour, IPlayerInteractable
    {
        [SerializeField] private string interactActionName;
        [SerializeField] private Transform entryPoint;
        [SerializeField] private GameObject interiorInteractablesParent;

        public string GetInteractActionName() => interactActionName;

        public void Interact(PlayerController playerController)
        {
            playerController.transform.position = entryPoint.transform.position;
            
            // Player = 9
            // Harpoon = 6
            // HarpoonLauncher = 7
            // SubmarineHull = 10
            Physics.IgnoreLayerCollision(9, 10, true);
            Physics.IgnoreLayerCollision(6, 10, true);
            Physics.IgnoreLayerCollision(7, 10, true);
            
            interiorInteractablesParent.SetActive(true);
            
            playerController.environmentDetector.ChangeEnvironment(Environment.Air);
        }
    }
}
