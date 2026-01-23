using Scripts.Helper;
using Scripts.Player;
using Scripts.Player.VR;
using UnityEngine;

namespace Scripts.Submarine
{
    public class SubmarineExitController : MonoBehaviour, IPlayerInteractable
    {
        [SerializeField] private string interactActionName;
        [SerializeField] private int priority;
        [SerializeField] private Transform exitPoint;
        [SerializeField] private GameObject interiorInteractablesParent;
        [SerializeField] private GameObject airPocket;
        [SerializeField] private GameObject hatchInteractable;

        public string GetInteractActionName() => interactActionName;
        public int GetPriority() => priority;

        public void Interact(PlayerController playerController)
        {
            playerController.transform.position = exitPoint.transform.position;
            
            // Player = 9
            // Harpoon = 6
            // HarpoonLauncher = 7
            // SubmarineHull = 10
            Physics.IgnoreLayerCollision(9, 10, false);
            Physics.IgnoreLayerCollision(6, 10, false);
            Physics.IgnoreLayerCollision(7, 10, false);
            
            interiorInteractablesParent.SetActive(false);
            
            playerController.environmentDetector.ChangeEnvironment(Environment.Underwater);
        }

        public void Interact(PlayerControllerVR playerController)
        {
            playerController.transform.position = exitPoint.transform.position;
            
            // Player = 9
            // Harpoon = 6
            // HarpoonLauncher = 7
            // SubmarineHull = 10
            Physics.IgnoreLayerCollision(9, 10, false);
            Physics.IgnoreLayerCollision(6, 10, false);
            Physics.IgnoreLayerCollision(7, 10, false);
            
            interiorInteractablesParent.SetActive(false);
            airPocket.SetActive(false);
            hatchInteractable.SetActive(true);
        }
    }
}
