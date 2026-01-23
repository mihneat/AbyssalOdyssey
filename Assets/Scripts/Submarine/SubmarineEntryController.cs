using Scripts.Helper;
using Scripts.Player;
using Scripts.Player.VR;
using UnityEngine;

namespace Scripts.Submarine
{
    public class SubmarineEntryController : MonoBehaviour, IPlayerInteractable
    {
        [SerializeField] private string interactActionName;
        [SerializeField] private int priority;
        [SerializeField] private Transform entryPoint;
        [SerializeField] private GameObject interiorInteractablesParent;
        [SerializeField] private GameObject airPocket;

        public string GetInteractActionName() => interactActionName;
        public int GetPriority() => priority;

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

        public void Interact(PlayerControllerVR playerController)
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
            airPocket.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
