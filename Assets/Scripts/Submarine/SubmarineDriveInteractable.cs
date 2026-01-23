using System;
using Scripts.Helper;
using Scripts.Player;
using Scripts.Player.VR;
using UnityEngine;

namespace Scripts.Submarine
{
    public class SubmarineDriveInteractable : MonoBehaviour, IPlayerInteractable
    {
        [SerializeField] private int priority;
        [SerializeField] private SubmarineController submarineController;
        
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
            // TODO idfk
            Debug.Log("[SubmarineDriveInteractable] TODO Drive meee");
        }
    }
}
