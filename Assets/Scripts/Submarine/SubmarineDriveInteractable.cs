using System;
using Scripts.Harpoon;
using Scripts.Helper;
using Scripts.Player;
using Scripts.Player.VR;
using Scripts.Submarine.VR;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Scripts.Submarine
{
    public class SubmarineDriveInteractable : MonoBehaviour, IPlayerInteractable
    {
        [SerializeField] private int priority;
        [SerializeField] private Transform vrPlayerSlot;
        [SerializeField] private SubmarineController submarineController;
        [SerializeField] private SubmarineControllerVR submarineControllerVR;
        
        public string GetInteractActionName() => "Drive";
        public int GetPriority() => priority;

        public void Interact(PlayerController playerController)
        {
            // Make the player kinematic and attach them to the ship
            playerController.GetComponent<Rigidbody>().isKinematic = true;
            playerController.transform.parent = transform;
            
            // Disable player interactor
            playerController.GetComponent<PlayerInteractor>().ToggleComponent(false);
            
            // Move the camera to the driver's position (cinemachine? switch between cameras?)
            // Disable the player camera
            playerController.cam.enabled = false;
            
            // Switch input from Player to Submarine
            playerController.playerInput.SwitchCurrentActionMap("Submarine");
            
            // Communicate the interaction to the SubmarineController
            submarineController.StartDriving(playerController);
        }

        public void Interact(PlayerControllerVR playerController)
        {
            // Attach them to the ship
            playerController.GetComponent<Rigidbody>().isKinematic = true;
            playerController.GetComponent<Collider>().isTrigger = true;
            playerController.transform.forward = submarineController.transform.forward;
            playerController.transform.parent = submarineControllerVR.transform;
            Vector3 prevPlayerLocalPosition = playerController.transform.localPosition;
            
            // Teleport the player to the appropriate slot
            playerController.transform.localPosition = vrPlayerSlot.localPosition;
            
            // Disable hand interactors
            HandInteractor[] handInteractors = FindObjectsByType<HandInteractor>(FindObjectsSortMode.None);
            foreach (HandInteractor handInteractor in handInteractors)
                handInteractor.ToggleComponent(false);

            // Lock player input
            playerController.ManuallyLockInput = true;
            
            // Turn off SnapTurn
            FindFirstObjectByType<SnapTurn>().enabled = false;
            
            // Hack: Parent the harpoon launcher to the submarine
            FindFirstObjectByType<HarpoonLauncherController>().transform.parent = submarineController.transform;
            
            // Communicate the interaction to the SubmarineControllerVR
            submarineControllerVR.StartDriving(playerController, prevPlayerLocalPosition);
        }
    }
}
